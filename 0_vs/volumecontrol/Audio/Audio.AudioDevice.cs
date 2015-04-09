using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

using Audio.MM_API;

namespace Audio
{
    public class AudioDevice
    {
        #region Members
        public UI.DeviceTab DeviceTab;
        public IMMDevice DeviceToUse;
        private PropertyStore PropStore;
        #endregion

        private void GetPropertyInformation()
        {
            IPropertyStore propstore;
            StorageAccessMode AccessMode = StorageAccessMode.Read;
            int msg = DeviceToUse.OpenPropertyStore(AccessMode, out propstore);
            PropStore = new PropertyStore(propstore);
        }

        #region Constructor
        /// <summary>
        /// Creates an instance for controlling the app audio.
        /// </summary>
        public AudioDevice(IMMDevice deviceToUse)
        {
            DeviceToUse = deviceToUse;
        }

        // Release object after usage using "Marshal.ReleaseComObject(VAR);
        public IAudioSessionManager2 GetDeviceManager()
        {
            object o;
            Guid guid = typeof(IAudioSessionManager2).GUID;
            if (DeviceToUse.Activate(ref guid, (int)CLSCTX.CLSCTX_ALL, IntPtr.Zero, out o) != 0 || o == null)
            {
                return null;
            }
            return o as IAudioSessionManager2;
        }
        #endregion

        public string GetFriendlyDeviceInterfaceName()
        {
            if (PropStore == null)
            {
                GetPropertyInformation();
            }

            if (PropStore.Contains(PropertyKeys.PKEY_DeviceInterface_FriendlyName))
            {
                return (string)PropStore[PropertyKeys.PKEY_DeviceInterface_FriendlyName].Value;
            }
            return "Unknown";
        }

        public string GetFriendlyDeviceName()
        {
            if (PropStore == null)
            {
                GetPropertyInformation();
            }

            if (PropStore.Contains(PropertyKeys.PKEY_Device_FriendlyName))
            {
                return (string)PropStore[PropertyKeys.PKEY_Device_FriendlyName].Value;
            }
            return "Unknown";
        }

        public string GetDisplayName()
        {
            string DeviceName = GetFriendlyDeviceName();
            string DeviceInterfaceName = GetFriendlyDeviceInterfaceName();

            return DeviceName + " (" + DeviceInterfaceName + ")";
        }

        public float GetApplicationVolume(ISimpleAudioVolume volume)
        {
            float level = 0;
            volume.GetMasterVolume(out level);
            return level * 100;
        }

        public bool GetApplicationMute(ISimpleAudioVolume volume)
        {
            bool mute = false;
            volume.GetMute(out mute);
            return mute;
        }

        public void SetApplicationVolume(ISimpleAudioVolume volume, float level)
        {
            Guid guid = Guid.Empty;
            volume.SetMasterVolume(level / 100, ref guid);
        }

        public void SetApplicationMute(ISimpleAudioVolume volume, bool mute)
        {
            Guid guid = Guid.Empty;
             volume.SetMute(mute, ref guid);
        }

        private List<ISimpleAudioVolume> GetVolumeObject(string processName)
        {
            // activate the session manager. we need the enumerator
            object o;
            Guid IID_IAudioSessionManager2 = typeof(IAudioSessionManager2).GUID;
            DeviceToUse.Activate(ref IID_IAudioSessionManager2, 0, IntPtr.Zero, out o);
            IAudioSessionManager2 mgr = (IAudioSessionManager2)o;

            // enumerate sessions for on this device
            IAudioSessionEnumerator sessionEnumerator;
            mgr.GetSessionEnumerator(out sessionEnumerator);
            int count;
            sessionEnumerator.GetCount(out count);
            List<ISimpleAudioVolume> volumeControls = new List<ISimpleAudioVolume>();
            for (int i = 0; i < count; i++)
            {
                IAudioSessionControl ctl;
                IAudioSessionControl2 ctl2;

                sessionEnumerator.GetSession(i, out ctl);

                ctl2 = ctl as IAudioSessionControl2;

                string sout1 = "";
                string sout2 = "";

                if (ctl2 != null)
                {
                    ctl2.GetSessionIdentifier(out sout1);
                    ctl2.GetSessionInstanceIdentifier(out sout2);
                }

                if (sout1.Contains(processName) || sout2.Contains(processName))
                {
                    volumeControls.Add(ctl as ISimpleAudioVolume);
                }
            }
            return volumeControls;
        }

        public IList<AudioProcess> GetAllSessions()
        {
            IAudioSessionManager2 mgr = GetDeviceManager();
            IAudioSessionEnumerator sessionEnumerator;
            mgr.GetSessionEnumerator(out sessionEnumerator);

            IList<AudioProcess> processes = new List<AudioProcess>();

            int count;
            sessionEnumerator.GetCount(out count);

            for (int i = 0; i < count; i++)
            {
                IAudioSessionControl ctl;
                sessionEnumerator.GetSession(i, out ctl);
                if (ctl == null)
                    continue;

                IAudioSessionControl2 ctl2 = ctl as IAudioSessionControl2;
                if (ctl2 != null)
                {
                    processes.Add(new AudioProcess(this, ctl2));
                }
            }
            Marshal.ReleaseComObject(sessionEnumerator);
            Marshal.ReleaseComObject(mgr);
            return processes;
        }

    }

}
