using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Midi
{
    namespace Device
    {
        class LaunchControlXL
        {
            public static string MIDI_Identifier = "Launch Control XL";
            public static bool IsDevice(string identifier) {
                return MIDI_Identifier == identifier;
            }

            public class ColumnDefinition
            {
                public int ColumnIndex;
                public Dictionary<ChannelKeys, MidiChannelInt> Channels;

                public MidiChannelInt this[int index]
                {
                    get
                    {
                        foreach (KeyValuePair<ChannelKeys, MidiChannelInt> pair in Channels)
                        {
                            if (pair.Value.ControllerID == index)
                            {
                                return pair.Value;
                            }
                        }
                        return null;
                    }
                }

                public MidiChannelInt this[ChannelKeys channelKeys]
                {
                    get
                    {
                        return Channels[channelKeys];
                    }
                }

                public ColumnDefinition(int columnIndex, int sendAID, int sentBID, int sentCID, int sliderID, int focusID, int controlID)
                {
                    ColumnIndex = columnIndex;

                    Channels = new Dictionary<ChannelKeys, MidiChannelInt>();
                    Channels[ChannelKeys.SendA] = new MidiChannelInt(ColumnIndex, sendAID, ChannelKeys.SendA);
                    Channels[ChannelKeys.SendB] = new MidiChannelInt(ColumnIndex, sentBID, ChannelKeys.SendB);
                    Channels[ChannelKeys.SendC] = new MidiChannelInt(ColumnIndex, sentCID, ChannelKeys.SendC);
                    Channels[ChannelKeys.Slider] = new MidiChannelInt(ColumnIndex, sliderID, ChannelKeys.Slider);
                    Channels[ChannelKeys.Focus] = new MidiChannelInt(ColumnIndex, focusID, ChannelKeys.Focus);
                    Channels[ChannelKeys.Control] = new MidiChannelInt(ColumnIndex, controlID, ChannelKeys.Control);
                }
            }

            public class ValueDefinition
            {
                public List<ColumnDefinition> Channels;
                public MidiChannelInt SendSelectMinus;
                public MidiChannelInt SendSelectPlus;
                public MidiChannelInt TrackMinus;
                public MidiChannelInt TrackPlus;

                public MidiChannelInt Device;
                public MidiChannelInt Mute;
                public MidiChannelInt Solo;
                public MidiChannelInt RecordArm;

                public MidiChannelInt this[int index]
                {
                    get
                    {
                        for (int i = 0; i < Channels.Count; i++)
                        {
                            MidiChannelInt channel = Channels[i][index];
                            if (channel != null)
                            {
                                return channel;
                            }
                        }

                        if (index == SendSelectMinus)
                        {
                            return SendSelectMinus;
                        }
                        if (index == SendSelectPlus)
                        {
                            return SendSelectPlus;
                        }
                        if (index == TrackMinus)
                        {
                            return TrackMinus;
                        }
                        if (index == TrackPlus)
                        {
                            return TrackPlus;
                        }
                        
                        
                        if (index == Device)
                        {
                            return Device;
                        }
                        if (index == Mute)
                        {
                            return Mute;
                        }
                        if (index == Solo)
                        {
                            return Solo;
                        }
                        if (index == RecordArm)
                        {
                            return RecordArm;
                        }

                        return null;
                    }
                }

                public ValueDefinition()
                {
                    Channels = new List<ColumnDefinition>() {
                        new ColumnDefinition(0, 13, 29, 49, 77, 41, 73),
                        new ColumnDefinition(1, 14, 30, 50, 78, 42, 74),
                        new ColumnDefinition(2, 15, 31, 51, 79, 43, 75),
                        new ColumnDefinition(3, 16, 32, 52, 80, 44, 76),
                        new ColumnDefinition(4, 17, 33, 53, 81, 57, 89),
                        new ColumnDefinition(5, 18, 34, 54, 82, 58, 90),
                        new ColumnDefinition(6, 19, 35, 55, 83, 59, 91),
                        new ColumnDefinition(7, 20, 36, 56, 84, 60, 92)
                    };

                    SendSelectMinus = new MidiChannelInt(-1, 104, ChannelKeys.SendSelectMinus);
                    SendSelectPlus = new MidiChannelInt(-1, 105, ChannelKeys.SendSelectPlus);
                    TrackMinus = new MidiChannelInt(-1, 106, ChannelKeys.TrackMinus);
                    TrackPlus = new MidiChannelInt(-1, 107, ChannelKeys.TrackPlus);

                    Device = new MidiChannelInt(-1, 108, ChannelKeys.Device);
                    Mute = new MidiChannelInt(-1, 109, ChannelKeys.Mute);
                    Solo = new MidiChannelInt(-1, 110, ChannelKeys.Solo);
                    RecordArm = new MidiChannelInt(-1, 111, ChannelKeys.RecordArm);
                }
            }

            public ValueDefinition Values = new ValueDefinition();

            protected Midi.Interface.LaunchControlXL AssociatedInterface;

            public LaunchControlXL(Midi.Interface.LaunchControlXL associatedInterface)
            {
                AssociatedInterface = associatedInterface;
            }
        }
    }
}
