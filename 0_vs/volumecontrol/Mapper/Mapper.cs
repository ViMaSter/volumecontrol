using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Timers;

namespace Mapper
{
    class Mapper
    {
        public static Dictionary<Audio.AudioDevice, IList<Audio.AudioProcess>> DevicesAndProcesses;
        public static bool ShowNotifications = true;
        public static System.Windows.Forms.NotifyIcon NotificationIcon;

        #region Dictionary
        public Dictionary<int, UI.ProcessRow> Mapping;
        public bool Add(int key , UI.ProcessRow value)
        {
            if (Mapping.ContainsKey(key))
            {
                return false;
            }

            Mapping.Add(key, value);
            return true;
        }

        public bool Remove(int key)
        {
            return Mapping.Remove(key);
        }

        public bool Remove(int key, UI.ProcessRow value)
        {
            if (Mapping.ContainsKey(key) && Mapping[key] == value)
            {
                return Mapping.Remove(key);
            }
            return false;
        }
        #endregion
        public Midi.Interface.LaunchControlXL DeviceInterface;
        private static UI.ProcessRow selectedProcessRow;
        public static UI.ProcessRow SelectedProcessRow
        {
            get
            {
                return selectedProcessRow;
            }
            set
            {
                if (SelectedProcessRow != null)
                {
                    selectedProcessRow.ToggleSelected(false);
                }

                value.ToggleSelected(true);
                selectedProcessRow = value;
            }
        }

        public static List<UI.ProcessRow> ProcessRows
        {
            get
            {
                return DevicesAndProcesses.Values.SelectMany(x => x).Select(x => x.ProcessRow).ToList();
            }
        }

        public static void ShiftSelection(int direction)
        {
            List<UI.ProcessRow> processRow = ProcessRows;
            int currentIndex = processRow.IndexOf(SelectedProcessRow);
            Console.WriteLine(currentIndex);
            SelectedProcessRow = processRow[(currentIndex + direction + processRow.Count) % processRow.Count];
        }

        public bool SetColumnToProcess(int midiColumn, UI.ProcessRow uiRow) {
            if (uiRow == null)
            {
                Remove(midiColumn);

                if (!DeviceInterface.SetColor(
                        Midi.Interface.LaunchControlXL.SystemExclusiveIndexes.Control,
                        midiColumn,
                        3,
                        0
                    ))
                {
                    return false;
                }

                if (!DeviceInterface.SetColor(
                        Midi.Interface.LaunchControlXL.SystemExclusiveIndexes.Focus,
                        midiColumn,
                        0,
                        0
                    ))
                {
                    return false;
                }

                return true;
            }
            else
            {
                Add(midiColumn, uiRow);

                if (!DeviceInterface.SetColor(
                        Midi.Interface.LaunchControlXL.SystemExclusiveIndexes.Control,
                        midiColumn,
                        0,
                        3
                    ))
                {
                    return false;
                }

                if (!DeviceInterface.SetColor(
                        Midi.Interface.LaunchControlXL.SystemExclusiveIndexes.Focus,
                        midiColumn,
                        uiRow.Muted ? 3 : 0,
                        !uiRow.Muted ? 3 : 0
                    ))
                {
                    return false;
                }

                return true;
            }
        }

        public void SetMappingMidi()
        {
            foreach (Midi.Device.LaunchControlXL.ColumnDefinition column in DeviceInterface.Device.Values.Channels)
            {
                column.Channels[Midi.ChannelKeys.Slider].Changed += Slider_Changed;
                column.Channels[Midi.ChannelKeys.Focus].Changed += Focus_Changed;
                column.Channels[Midi.ChannelKeys.Control].Changed += Control_Changed;
            }
            DeviceInterface.Device.Values.TrackPlus.Changed += TrackPlus_Changed;
            DeviceInterface.Device.Values.TrackMinus.Changed += TrackMinus_Changed;
            DeviceInterface.Device.Values.Device.Changed += Device_Changed;
        }

        void Device_Changed(object sender, Midi.MidiChannelInt e)
        {
            if (e.Value != 0)
            {
                for (int i = 0; i < System.Windows.Forms.Application.OpenForms.Count; i++)
                {
                    System.Windows.Forms.Application.OpenForms[i].Invoke(new Action(() => ((volumecontrol.Form1)(System.Windows.Forms.Application.OpenForms[i])).RefreshList()));
                }
            }
        }

        void TrackMinus_Changed(object sender, Midi.MidiChannelInt e)
        {
            if (e.Value != 0) {
                ShiftSelection(-1);
            }
        }

        void TrackPlus_Changed(object sender, Midi.MidiChannelInt e)
        {
            if (e.Value != 0) {
                ShiftSelection(+1);
            }
        }

        void Slider_Changed(object sender, Midi.MidiChannelInt e)
        {
            Mapping[e.ColumnIndex].AudioProcess.Volume = e.Value / 127.0f * 100.0f;
            Mapping[e.ColumnIndex].UpdateValues();
        }

        void Focus_Changed(object sender, Midi.MidiChannelInt e)
        {
            if (e.Value > 0)
            {
                bool NewMuted = !Mapping[e.ColumnIndex].AudioProcess.Mute;
                if (!DeviceInterface.SetColor(
                    Midi.Interface.LaunchControlXL.SystemExclusiveIndexes.Focus,
                    e.ColumnIndex,
                    NewMuted ? 3 : 0,
                    NewMuted ? 0 : 3
                ))
                {
                    return;
                }

                Mapping[e.ColumnIndex].AudioProcess.Mute = NewMuted;
                Mapping[e.ColumnIndex].UpdateValues();
            }
        }

        private static float Timer_SecondsBetweenMapping = 0.2f;
        private static Timer Timer;
        private static int TimerPhase = 0;
        private static Midi.MidiChannelInt TimerHeldButton;
        void Control_Changed(object sender, Midi.MidiChannelInt e)
        {
            TimerHeldButton = e;
            if (e.Value == 0)   // button released
            {
                Timer.Stop();

                float elapsedSeconds = (float)e.ToggleStopwatch.ElapsedMilliseconds/1000;
                if (elapsedSeconds > Timer_SecondsBetweenMapping)
                {
                    SetColumnToProcess(e.ColumnIndex, SelectedProcessRow);
                }
                else
                {
                    SetColumnToProcess(e.ColumnIndex, null);
                }
                TimerPhase = 0;
            }
            else
            {
                Timer.Stop();
                Timer.Start();
            }
            // TODO: Implement "hold button for X seconds-check"
        }

        public void DeviceUpdateTimer(object sender, EventArgs e) {
            TimerPhase++;
            Console.WriteLine(12);
            if (TimerPhase > 3)
            {
                return;
            }

            DeviceInterface.SetColor(
                Midi.Interface.LaunchControlXL.SystemExclusiveIndexes.Control,
                TimerHeldButton.ColumnIndex,
                3,
                TimerPhase
            );
        }

        public void SetDeviceInterface(Midi.Interface.LaunchControlXL deviceInterface)
        {
            DeviceInterface = deviceInterface;
        }

        public void ResetColors()
        {
            for (int i = 0; i < 8; i++)
            {
                DeviceInterface.SetColor(
                    Midi.Interface.LaunchControlXL.SystemExclusiveIndexes.SendA,
                    i,
                    0,
                    0
                );
                DeviceInterface.SetColor(
                    Midi.Interface.LaunchControlXL.SystemExclusiveIndexes.SendB,
                    i,
                    0,
                    0
                );
                DeviceInterface.SetColor(
                    Midi.Interface.LaunchControlXL.SystemExclusiveIndexes.SendC,
                    i,
                    0,
                    0
                );

                DeviceInterface.SetColor(
                    Midi.Interface.LaunchControlXL.SystemExclusiveIndexes.Control,
                    i,
                    3,
                    0
                );
                DeviceInterface.SetColor(
                    Midi.Interface.LaunchControlXL.SystemExclusiveIndexes.Focus,
                    i,
                    0,
                    0
                );
            }
        }
        
        public Mapper()
        {
            DevicesAndProcesses = new Dictionary<Audio.AudioDevice, IList<Audio.AudioProcess>>();
            Mapping = new Dictionary<int, UI.ProcessRow>();
            Timer = new Timer();
            Timer.Interval = (int)((Timer_SecondsBetweenMapping/3f)*1000);
            Timer.Elapsed += new ElapsedEventHandler(DeviceUpdateTimer);
        }
    }
}
