using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;

using Audio;

namespace UI
{
    public class ProcessRow
    {
        public static void ResetCount()
        {
            _nextID = 0;
        }
        
        public static int _nextID = 0;
        public static int nextID
        {
            get
            {
                return _nextID++;
            }
        }
        public float Volume = 0.0f;
        public bool Muted = false;
        int ID = -1;
        System.Windows.Forms.Panel panel;
        System.Windows.Forms.CheckBox muted;
        System.Windows.Forms.TrackBar volume;
        System.Windows.Forms.Label label;
        System.Windows.Forms.PictureBox icon;
        System.Windows.Forms.TabPage associatedTabPage;
        System.Windows.Forms.TabControl associatedTabControl;
        public AudioProcess AudioProcess;
        Guid Guid;

        public void SetupChildren()
        {
            // label
            // 
            label.Dock = System.Windows.Forms.DockStyle.Left;
            label.Location = new System.Drawing.Point(68, 0);
            label.Size = new System.Drawing.Size(279, 61);
            label.Name = "process" + ID + "_label";
            label.TabIndex = 0;
            label.Click += label_Click;

            label.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;

            // icon
            // 
            icon.Dock = System.Windows.Forms.DockStyle.Left;
            icon.Location = new System.Drawing.Point(0, 0);
            icon.Size = new System.Drawing.Size(68, 61);
            icon.Name = "process" + ID + "_icon";
            icon.TabIndex = 1;

            icon.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            icon.TabStop = false;

            // muted
            // 
            muted.Dock = System.Windows.Forms.DockStyle.Left;
            muted.Location = new System.Drawing.Point(794, 0);
            muted.Size = new System.Drawing.Size(81, 61);
            muted.Name = "process" + ID + "_muted";
            muted.TabIndex = 2;

            muted.Text = "Muted";
            muted.UseVisualStyleBackColor = true;
            muted.CheckedChanged += muted_CheckedChanged;

            // volume
            // 
            volume.Dock = System.Windows.Forms.DockStyle.Left;
            volume.Location = new System.Drawing.Point(347, 0);
            volume.Size = new System.Drawing.Size(475, 61);
            volume.AutoSize = true;
            volume.Name = "process" + ID + "_volume";
            volume.TabIndex = 3;

            volume.Maximum = 100;
            volume.TickStyle = System.Windows.Forms.TickStyle.Both;
            volume.ValueChanged += volume_ValueChanged;
        }

        private void label_Click(object sender, EventArgs e)
        {
            Mapper.Mapper.SelectedProcessRow = this;
        }

        public void UpdateValues() {
            label.Text = AudioProcess.DisplayName;

            if (AudioProcess.Icon != null)
            {
                icon.Image = AudioProcess.Icon.ToBitmap();
            }

            AudioProcess.Process_SimpleAudioVolume.GetMasterVolume(out Volume);
            AudioProcess.Process_SimpleAudioVolume.GetMute(out Muted);
            if (muted.IsHandleCreated)
            {
                muted.Invoke(new Action(() => UpdateMutedValues()));
                volume.Invoke(new Action(() => UpdateVolumeValues()));
            }
            else
            {
                UpdateMutedValues();
                UpdateVolumeValues();
            }
        }

        void UpdateMutedValues()
        {
            muted.Checked = Muted;
        }

        void UpdateVolumeValues()
        {
            volume.Value = (int)(Volume * 100);
        }

        private void muted_CheckedChanged(object sender, EventArgs e)
        {
            System.Windows.Forms.CheckBox checkBox = (System.Windows.Forms.CheckBox)sender;
            AudioProcess.Process_SimpleAudioVolume.SetMute(checkBox.Checked, ref Guid);
        }

        private void volume_ValueChanged(object sender, EventArgs e)
        {
            System.Windows.Forms.TrackBar trackBar = (System.Windows.Forms.TrackBar)sender;
            AudioProcess.Process_SimpleAudioVolume.SetMasterVolume((float)trackBar.Value / 100, ref Guid);
        }

        [DllImport("Shell32.dll")]
        public extern static int ExtractIconEx(string libName, int iconIndex, IntPtr[] largeIcon, IntPtr[] smallIcon, int nIcons);

        public ProcessRow(AudioProcess audioProcess)
        {
            panel = new System.Windows.Forms.Panel();
            muted = new System.Windows.Forms.CheckBox();
            volume = new System.Windows.Forms.TrackBar();
            label = new System.Windows.Forms.Label();
            icon = new System.Windows.Forms.PictureBox();

            ID = nextID;
            AudioProcess = audioProcess;
            Guid = Guid.NewGuid();

            SetupChildren();
            UpdateValues();
            
            panel.Controls.Add(muted);
            panel.Controls.Add(volume);
            panel.Controls.Add(label);
            panel.Controls.Add(icon);

            panel.Location = new System.Drawing.Point(0, 61 * ID);
            panel.Name = "panel" + ID;
            panel.Size = new System.Drawing.Size(875, 61);
            panel.TabIndex = ID;
            panel.BackColor = System.Windows.Forms.Control.DefaultBackColor;
        }

        public void ToggleSelected(bool isSelected) {
            if (isSelected) {
                associatedTabPage.Invoke(new Action(() => associatedTabControl.SelectedTab = associatedTabPage));
            }

            Color backColor = Color.FromArgb(
                255,
                Math.Min(System.Windows.Forms.Control.DefaultBackColor.R + (isSelected?20:0), 255),
                Math.Min(System.Windows.Forms.Control.DefaultBackColor.G + (isSelected?20:0), 255),
                Math.Min(System.Windows.Forms.Control.DefaultBackColor.B + (isSelected?20:0), 255)
            );

            if (Mapper.Mapper.ShowNotifications)
            {
                if (AudioProcess.DisplayName.Contains("Hostpr"))
                {
                    Console.WriteLine("B");
                }
                Mapper.Mapper.NotificationIcon.ShowBalloonTip(2000, AudioProcess.AudioDevice.GetDisplayName(), AudioProcess.DisplayName == "" ? "A" : AudioProcess.DisplayName, System.Windows.Forms.ToolTipIcon.Info);
            }

            panel.Invoke(new Action(() => panel.BackColor = backColor));
        }

        public void AddToTab(System.Windows.Forms.TabControl tabControl, System.Windows.Forms.TabPage tabPage)
        {
            ((System.ComponentModel.ISupportInitialize)(volume)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(icon)).BeginInit();
            associatedTabControl = tabControl;
            associatedTabPage = tabPage;
            tabPage.Controls.Add(panel);
        }
    }

    public class DeviceTab
    {
        public static DeviceTab Active = null;

        private static int _id = 0;
        public static int nextID {
            get {
                return _id++;
            }
        }
        public System.Windows.Forms.TabPage TabPage;
        public AudioDevice AudioDevice;
        public DeviceTab(AudioDevice audioDevice)
        {
            AudioDevice = audioDevice;

            // tabPage
            //
            TabPage = new System.Windows.Forms.TabPage(audioDevice.GetDisplayName());
            TabPage.AutoScroll = true;
            TabPage.Location = new System.Drawing.Point(4, 22);
            TabPage.Name = "tabPage_" + (audioDevice.GetHashCode().ToString());
            TabPage.Padding = new System.Windows.Forms.Padding(3);
            TabPage.Size = new System.Drawing.Size(899, 343);
            TabPage.TabIndex = nextID;
            TabPage.UseVisualStyleBackColor = true;
        }
        ~DeviceTab()
        {

        }
    }
}
