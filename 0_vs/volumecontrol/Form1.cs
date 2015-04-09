using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Reflection;
using System.Diagnostics;

using Audio;
using Audio.MM_API;

using UI;
using NAudio.Midi;

namespace volumecontrol
{
    public partial class Form1 : Form
    {
        AudioProcess processToControl;
        
        MidiIn midiIn;
        MidiOut midiOut;
        Midi.Interface.LaunchControlXL deviceinterface;
        public void MidiInit()
        {
            for (int i = 0; i < MidiIn.NumberOfDevices; i++)
            {
                if (MidiIn.DeviceInfo(i).ProductName.Contains("Launch Control XL"))
                {
                    midiIn = new MidiIn(i);
                    break;
                }
            }
            for (int i = 0; i < MidiOut.NumberOfDevices; i++)
            {
                if (MidiOut.DeviceInfo(i).ProductName.Contains("Launch Control XL"))
                {
                    midiOut = new MidiOut(i);
                    break;
                }
            }

            deviceinterface = new Midi.Interface.LaunchControlXL(midiIn, midiOut);
        }
        public void MidiUninit()
        {
            deviceinterface = new Midi.Interface.LaunchControlXL(midiIn, midiOut);
            deviceinterface.Dispose();
        }


        void Slider_Changed(object sender, Midi.MidiChannelInt e)
        {
            Console.WriteLine("Change is now: " + e.Value);
        }

        void midiIn_ErrorReceived(object sender, MidiInMessageEventArgs e)
        {
            throw new NotImplementedException();
        }

        void midiIn_MessageReceived(object sender, MidiInMessageEventArgs e)
        {
            //processToControl.Process_SimpleAudioVolume.SetMasterVolume((float)evt.ControllerValue / 128.0f, ref guid);
        }

        public void UpdateProcessList()
        {
            IMMDeviceEnumerator deviceEnumerator = (IMMDeviceEnumerator)(new MMDeviceEnumerator());
            IMMDeviceCollection outputDevices = null;
            IMMDeviceCollection inputDevices = null;

            deviceEnumerator.EnumAudioEndpoints(EDataFlow.eRender, STATE.ACTIVE, out outputDevices);
            deviceEnumerator.EnumAudioEndpoints(EDataFlow.eCapture, STATE.ACTIVE, out inputDevices);

            uint inputDeviceCount = 0;
            uint outputDeviceCount = 0;

            inputDevices.GetCount(out inputDeviceCount);
            outputDevices.GetCount(out outputDeviceCount);

            // Loop over every output device
            for (uint i = 0; i < outputDeviceCount; i++)
            {
                IMMDevice device;
                outputDevices.Item(i, out device);

                AudioDevice audioDevice = new AudioDevice(device);
                Mapper.Mapper.DevicesAndProcesses[audioDevice] = audioDevice.GetAllSessions();
            }
        }

        Mapper.Mapper mapper;

        public Form1()
        {
            InitializeComponent();
            mapper = new Mapper.Mapper();
            Mapper.Mapper.NotificationIcon = notificationIcon;

            UpdateProcessList();
            Draw();
            // TODO: Rework Interface-update routine (seperate each process and device to be able to structurally and cosmetically update itself)

            Console.WriteLine("Done!");
            MidiInit();
            mapper.SetDeviceInterface(deviceinterface);
            mapper.SetMappingMidi();
            mapper.ResetColors();
        }


        private void Form1_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            mapper.ResetColors();
            MidiUninit();
        }

        public void Draw()
        {
            foreach (KeyValuePair<AudioDevice, IList<AudioProcess>> pair in Mapper.Mapper.DevicesAndProcesses)
            {
                // prepare tab
                DeviceTab deviceTab = new DeviceTab(pair.Key);

                for (int i = 0; i < pair.Value.Count; i++)
                {
                    // prepare row
                    ProcessRow processRow = new ProcessRow(pair.Value[i]);
                    pair.Value[i].ProcessRow = processRow;

                    // add row to tab
                    processRow.AddToTab(this.tabControl_devices, deviceTab.TabPage);

                    // and refresh the values
                    processRow.UpdateValues();
                }

                pair.Key.DeviceTab = deviceTab;

                // add tab to tabcontrol
                this.tabControl_devices.Controls.Add(deviceTab.TabPage);

                // Reset rows
                ProcessRow.ResetCount();
            }
        }

        public void UpdateDevice(IMMDevice device)
        {
            AudioDevice audioDevice = new AudioDevice(device);
            UpdateDevice(audioDevice);
        }

        public void UpdateDevice(AudioDevice audioDevice)
        {
            Mapper.Mapper.DevicesAndProcesses[audioDevice] = audioDevice.GetAllSessions();
        }

        public void UpdateProcess(AudioProcess audioProcess)
        {
            audioProcess.Update();
        }

        public void RebuildUI()
        {

        }

        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            //.GetAllSessions()
        }

        private void notificationIcon_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            Show();
            WindowState = FormWindowState.Normal;
        }

        private void Form1_Resize(object sender, System.EventArgs e)
        {
            if (WindowState == FormWindowState.Minimized)
            {
                Hide();
            }
        }

        private void toggleNotificationsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Mapper.Mapper.ShowNotifications = !Mapper.Mapper.ShowNotifications;
            toggleNotificationsToolStripMenuItem.Checked = Mapper.Mapper.ShowNotifications;
        }

        public void RefreshList()
        {
            this.tabControl_devices.Controls.Clear();
            UpdateProcessList();
            Draw();
        }

        private void tabControl_devices_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.R)
            {
                RefreshList();
            }
        }
    }
}
