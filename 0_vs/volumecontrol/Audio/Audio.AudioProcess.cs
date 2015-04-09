using System;
using System.Drawing;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows.Forms;

using Audio.MM_API;
using UI;

namespace Audio
{
    public class AudioProcess
    {
        public ProcessRow ProcessRow;
        public AudioDevice AudioDevice;
        public Process Process;
        public IAudioSessionControl2 Process_AudioSessionControl;
        public ISimpleAudioVolume Process_SimpleAudioVolume;
        public string DisplayName = "NONE";
        public Icon Icon;

        #region Members
        /// <summary>
        /// Gets or sets the volume level
        /// </summary>
        public float? Volume
        {
            get
            {
                return AudioDevice.GetApplicationVolume(Process_SimpleAudioVolume);
            }
            set
            {
                if (value != null)
                {
                    AudioDevice.SetApplicationVolume(Process_SimpleAudioVolume, value.Value);
                }
            }
        }

        /// <summary>
        /// Gets or sets the mute indicator.
        /// </summary>
        public bool Mute
        {
            get
            {
                return AudioDevice.GetApplicationMute(Process_SimpleAudioVolume);
            }
            set
            {
                AudioDevice.SetApplicationMute(Process_SimpleAudioVolume, value);
            }
        }
        #endregion

        public bool IsValid {
            get
            {
                return DisplayName == "NONE";
            }
        }

        public void Update()
        {
            Console.WriteLine("");
            // Update display icon
            try
            {
                if (((IAudioSessionControl2)Process_SimpleAudioVolume).IsSystemSoundsSession() == 0 /* Is System Sound */)
                {
                    // Get the path from the system-sound DLL
                    Icon = new Icon(Environment.ExpandEnvironmentVariables(@"%SystemRoot%\System32\AudioSrv.Dll"));
                }
                else
                {
                    // Get the icon path from the audio session
                    string path;
                    Int32 a = ((IAudioSessionControl)Process_SimpleAudioVolume).GetIconPath(out path);

                    // Fall back to the process if the audio session is empty
                    if (path == "" && Process != null)
                    {
                        path = Process.MainModule.FileName;
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Couldn't resolve IconPath.");
            }
            // TODO: Add icon-path parsing

            // Update display name
            if (((IAudioSessionControl2)Process_SimpleAudioVolume).IsSystemSoundsSession() == 0 /* Is System Sound */)
            {
                DisplayName = "System Sound Session";
            }
            else
            {
                if (Process != null)
                {
                    try
                    {
                        // Use window title
                        int a = ((IAudioSessionControl2)Process_SimpleAudioVolume).GetDisplayName(out DisplayName);
                        DisplayName = Process.MainWindowTitle;
                        if (DisplayName == "")
                        {
                            throw new Exception("No valid name found. Continue searching");
                        }
                    }
                    catch (Exception e)
                    {
                        try
                        {
                            // If no window, use file description
                            DisplayName = Process.Modules[0].FileVersionInfo.FileDescription;
                        }
                        catch (Exception e2)
                        {

                        }
                    }
                }
                if (DisplayName == "NONE" && Process == null)
                {
                    // Extreme rare callback using the 
                    ((IAudioSessionControl2)Process_SimpleAudioVolume).GetDisplayName(out DisplayName);
                }
            }
        }

        public AudioProcess(AudioDevice audioDevice, IAudioSessionControl2 ctl2)
        {
            // Set device
            AudioDevice = audioDevice;

            Process_AudioSessionControl = ctl2;

            // Set process
            uint processID;
            ctl2.GetProcessId(out processID);
            if (processID != 0)
            {
                try
                {
                    Process = Process.GetProcessById((int)processID);
                }
                catch (Exception e)
                {

                }
            }

            // Set SimpleAudioVolume
            Process_SimpleAudioVolume = ctl2 as ISimpleAudioVolume;

            Update();
        }
    }
}
