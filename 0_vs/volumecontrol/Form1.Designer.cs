namespace volumecontrol
{
    partial class Form1
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code
        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.tabControl_devices = new System.Windows.Forms.TabControl();
            this.notificationIcon = new System.Windows.Forms.NotifyIcon(this.components);
            this.notificationIconContextMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.toggleNotificationsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.notificationIconContextMenu.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabControl_devices
            // 
            this.tabControl_devices.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl_devices.Location = new System.Drawing.Point(0, 0);
            this.tabControl_devices.Name = "tabControl_devices";
            this.tabControl_devices.SelectedIndex = 0;
            this.tabControl_devices.Size = new System.Drawing.Size(907, 369);
            this.tabControl_devices.TabIndex = 0;
            this.tabControl_devices.KeyDown += new System.Windows.Forms.KeyEventHandler(this.tabControl_devices_KeyDown);
            // 
            // notificationIcon
            // 
            this.notificationIcon.ContextMenuStrip = this.notificationIconContextMenu;
            this.notificationIcon.Icon = ((System.Drawing.Icon)(resources.GetObject("notificationIcon.Icon")));
            this.notificationIcon.Text = "Window Mixer<->MIDI Interface";
            this.notificationIcon.Visible = true;
            this.notificationIcon.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.notificationIcon_MouseDoubleClick);
            // 
            // notificationIconContextMenu
            // 
            this.notificationIconContextMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toggleNotificationsToolStripMenuItem});
            this.notificationIconContextMenu.Name = "notificationIconContextMenu";
            this.notificationIconContextMenu.Size = new System.Drawing.Size(183, 26);
            // 
            // toggleNotificationsToolStripMenuItem
            // 
            this.toggleNotificationsToolStripMenuItem.Checked = true;
            this.toggleNotificationsToolStripMenuItem.CheckOnClick = true;
            this.toggleNotificationsToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.toggleNotificationsToolStripMenuItem.Name = "toggleNotificationsToolStripMenuItem";
            this.toggleNotificationsToolStripMenuItem.Size = new System.Drawing.Size(182, 22);
            this.toggleNotificationsToolStripMenuItem.Text = "Toggle Notifications";
            this.toggleNotificationsToolStripMenuItem.Click += new System.EventHandler(this.toggleNotificationsToolStripMenuItem_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(907, 369);
            this.Controls.Add(this.tabControl_devices);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "Form1";
            this.Text = "Windows Mixer<->MIDI Interface";
            this.Resize += new System.EventHandler(this.Form1_Resize);
            this.notificationIconContextMenu.ResumeLayout(false);
            this.ResumeLayout(false);

        }
        #endregion

        private System.Windows.Forms.TabControl tabControl_devices;
        private System.Windows.Forms.NotifyIcon notificationIcon;
        private System.Windows.Forms.ContextMenuStrip notificationIconContextMenu;
        private System.Windows.Forms.ToolStripMenuItem toggleNotificationsToolStripMenuItem;
    }
}

