using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Midi
{
    public enum ChannelKeys
    {
        SendA,
        SendB,
        SendC,
        Slider,
        Focus,
        Control,

        SendSelectMinus,
        SendSelectPlus,
        TrackMinus,
        TrackPlus,

        Device,
        Mute,
        Solo,
        RecordArm,
    }

    public class MidiChannelInt
    {
        public int ColumnIndex;
        public int ControllerID;
        public ChannelKeys ChannelKey;
        public Stopwatch ToggleStopwatch;
        public event MidiChannelIntChange Changed;

        private int value;
        public int Value
        {
            get
            {
                return this.value;
            }
            set
            {
                this.value = value;
                if (Changed != null)
                {
                    Changed(this, this);
                }
                ToggleStopwatch.Restart();
            }
        }

        #region Constructor
        public MidiChannelInt(int columnIndex, int controllerID, ChannelKeys channelKey)
        {
            value = -1;
            ToggleStopwatch = new Stopwatch();
            ColumnIndex = columnIndex;
            ControllerID = controllerID;
            ChannelKey = channelKey;
            Changed = delegate { };
        }
        #endregion

        #region Converter
        public static implicit operator int(MidiChannelInt midiChannelInt)
        {
            return midiChannelInt.ControllerID;
        }
        #endregion
        #region Comparison
        public static bool operator ==(MidiChannelInt midiChannelInt, int controllerID)
        {
            return midiChannelInt.ControllerID == controllerID;
        }

        public static bool operator !=(MidiChannelInt midiChannelInt, int controllerID)
        {
            return midiChannelInt.ControllerID != controllerID;
        }

        public static bool operator ==(MidiChannelInt midiChannelInt, ChannelKeys channelKey)
        {
            return midiChannelInt.ChannelKey == channelKey;
        }

        public static bool operator !=(MidiChannelInt midiChannelInt, ChannelKeys channelKey)
        {
            return midiChannelInt.ChannelKey != channelKey;
        }
        #endregion
    }

    public delegate void MidiChannelIntChange(object sender, MidiChannelInt e);
}
