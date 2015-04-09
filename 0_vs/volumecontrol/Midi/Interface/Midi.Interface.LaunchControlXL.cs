using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace Midi
{
    namespace Interface
    {
        class LaunchControlXL : IDisposable
        {
            #region Statics
            public enum SystemExclusiveIndexes
            {
                SendA = 0x00,
                SendB = 0x08,
                SendC = 0x10,
                Slider = 0xFF,
                Focus = 0x18,
                Control = 0x20
            }

            public struct ColorChange
            {
                #region Internal layout
                // Internal storage
                internal BitVector32 data;

                internal static readonly BitVector32.Section i_RedIntensity = BitVector32.CreateSection(3);
                internal static readonly BitVector32.Section i_Copy = BitVector32.CreateSection(1, i_RedIntensity);
                internal static readonly BitVector32.Section i_Clear = BitVector32.CreateSection(1, i_Copy);
                internal static readonly BitVector32.Section i_GreenIntensity = BitVector32.CreateSection(3, i_Clear);
                internal static readonly BitVector32.Section i_EmptyBit = BitVector32.CreateSection(1, i_GreenIntensity);
                #endregion

                #region Access wrapper
                public uint RedIntensity
                {
                    get { return (uint)data[i_RedIntensity]; }
                    set { data[i_RedIntensity] = (int)value; }
                }
                public uint Copy
                {
                    get { return (uint)data[i_Copy]; }
                    set { data[i_Copy] = (int)value; }
                }
                public uint Clear
                {
                    get { return (uint)data[i_Clear]; }
                    set { data[i_Clear] = (int)value; }
                }
                public uint GreenIntensity
                {
                    get { return (uint)data[i_GreenIntensity]; }
                    set { data[i_GreenIntensity] = (int)value; }
                }
                public uint EmptyBit
                {
                    get { return (uint)data[i_EmptyBit]; }
                    set { data[i_EmptyBit] = (int)value; }
                }
                #endregion
                #region Constructor
                public ColorChange(uint redIntensity, uint greenIntensity)
                {
                    data = new BitVector32();

                    RedIntensity = redIntensity;
                    GreenIntensity = greenIntensity;

                    Copy = 1;
                    Clear = 1;
                    EmptyBit = 0;
                }

                public ColorChange(int redIntensity, int greenIntensity)
                {
                    this = new ColorChange((uint)redIntensity, (uint)greenIntensity);
                }
                #endregion

                public byte ToByte()
                {
                    int size = Marshal.SizeOf(this);
                    byte[] target = new byte[size];
                    IntPtr pointer = Marshal.AllocHGlobal(size);

                    Marshal.StructureToPtr(this, pointer, true);
                    Marshal.Copy(pointer, target, 0, size);
                    Marshal.FreeHGlobal(pointer);

                    return target[0];
                }
            }

            public struct SystemExlusiveMessage
            {
                public enum SystemExlusiveMessageIndexes
                {
                    TemplateID = 7,
                    Index = 8,
                    Value = 9
                }

                public static readonly byte[] SystemExlusiveMessageTemplate = {
                    0xF0,
                    0x00,
                    0x20,
                    0x29,
                    0x02,
                    0x11,
                    0x78,
                    0xFF,     // replace with Template ID
                    0xFF,     // replace with Index (see SystemExclusiveIndexes)
                    0xFF,     // replace with Value ()
                    0xF7,
                };

                public byte[] RawMessage;

                public SystemExlusiveMessage(byte templateID, byte index, byte value)
                {
                    RawMessage = SystemExlusiveMessageTemplate;

                    RawMessage[(int)SystemExlusiveMessageIndexes.TemplateID] = templateID;
                    RawMessage[(int)SystemExlusiveMessageIndexes.Index] = index;
                    RawMessage[(int)SystemExlusiveMessageIndexes.Value] = value;
                }

                public SystemExlusiveMessage(int templateID, int index, ColorChange value)
                {
                    this = new SystemExlusiveMessage((byte)templateID, (byte)index, value.ToByte());
                }
            }
			#endregion
            
            #region Members
            private NAudio.Midi.MidiIn MidiInDevice;
            private NAudio.Midi.MidiOut MidiOutDevice;

            public Midi.Device.LaunchControlXL Device;
            public Midi.Device.LaunchControlXL.ValueDefinition Values
            {
                get
                {
                    return Device.Values;
                }
            }

            public event MidiChannelIntChange ValueChange;
            #endregion

            #region Constructor
            public bool Start()
            {
                if (MidiInDevice == null)
                {
                    return false;
                }

                MidiInDevice.Start();
                SetupEvents();
                return true;
            }

            public LaunchControlXL(NAudio.Midi.MidiIn midiInDevice, NAudio.Midi.MidiOut midiOutDevice)
            {
                MidiInDevice = midiInDevice;
                MidiOutDevice = midiOutDevice;
                Device = new Midi.Device.LaunchControlXL(this);
                Start();
            }
			#endregion
            #region Destructor
            public bool Stop()
            {
                if (MidiInDevice == null)
                {
                    return false;
                }

                DetachEvents();
                MidiInDevice.Stop();
                return true;
            }

            ~LaunchControlXL()
            {
                DetachEvents();
            }
            #endregion

            #region Methods
            public bool SetColor(SystemExclusiveIndexes index, int ColumnIndex, int red, int green)
            {
                Interface.LaunchControlXL.SystemExlusiveMessage message =
                    new Interface.LaunchControlXL.SystemExlusiveMessage(
                        0,
                        (int)index + ColumnIndex,
                        new Interface.LaunchControlXL.ColorChange(red, green)
                    );

                if (index != SystemExclusiveIndexes.Slider)
                {
                    MidiOutDevice.SendBuffer(message.RawMessage);
                }

                return index != SystemExclusiveIndexes.Slider;
            }
            #endregion

            #region Events
            private void ErrorReceived(object sender, NAudio.Midi.MidiInMessageEventArgs e)
            {
                throw new Exception(e.ToString());
            }

            private void MessageReceived(object sender, NAudio.Midi.MidiInMessageEventArgs e)
            {
                int id = -1;
                int value = -1;

                try
                {
                    NAudio.Midi.ControlChangeEvent evt = (NAudio.Midi.ControlChangeEvent)e.MidiEvent;
                    id = (int)evt.Controller;
                    value = evt.ControllerValue;
                }
                catch (Exception ex) { }

                try
                {
                    NAudio.Midi.NoteEvent evt = (NAudio.Midi.NoteEvent)e.MidiEvent;
                    id = evt.NoteNumber;
                    value = evt.Velocity > 0 ? 1 : 0;
                }
                catch (Exception ex) { }

                if (id == -1)
                {
                    return;
                }

                try
                {
                    Device.Values[id].Value = value;

                    if (ValueChange != null)
                    {
                        ValueChange(this, Device.Values[id]);
                    }
                }
                catch (Exception ex) { }

                Console.WriteLine(id);
                Console.WriteLine(Device.Values[id].ColumnIndex + "-" + Device.Values[id].ChannelKey + ": " + value);
            }

            public void SetupEvents()
            {
                MidiInDevice.MessageReceived += MessageReceived;
                MidiInDevice.ErrorReceived += ErrorReceived;
            }

            public void DetachEvents()
            {
                MidiInDevice.MessageReceived -= MessageReceived;
                MidiInDevice.ErrorReceived -= ErrorReceived;
            }
            #endregion

            public void Dispose()
            {
                MidiInDevice.Stop();
                MidiOutDevice.Close();
            }
        }
    }
}
