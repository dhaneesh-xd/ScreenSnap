using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Screensnap
{

    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
            MouseHook.RightButtonHold += MouseHook_RightButtonHold;
        }

        private void MainForm_SizeChanged(object sender, EventArgs e)
        {
            bool mouseNotOnTaskbar = Screen.GetWorkingArea(this).Contains(Cursor.Position);
            if (mouseNotOnTaskbar && this.WindowState == FormWindowState.Minimized)
            {

                notifyIcon.Text = "Screen Snap";
                notifyIcon.Visible = true;
                this.ShowInTaskbar = false;
            }
        }
        private void notifyIcon_Click(object sender, EventArgs e)
        {
            if (this.WindowState == FormWindowState.Minimized || !this.Visible)
            {
                this.WindowState = FormWindowState.Normal;
                this.ShowInTaskbar = false;
                this.Visible = true;
            }
            else
            {
                this.WindowState = FormWindowState.Minimized;
                this.ShowInTaskbar = false;
                this.Visible = true;
            }
        }
        private void MainForm_Load(object sender, EventArgs e)
        {
            LocationAlign();
        }
        private void LocationAlign()
        {
            int taskbarHeight = Screen.PrimaryScreen.Bounds.Height - Screen.PrimaryScreen.WorkingArea.Height;
            int x = Screen.PrimaryScreen.WorkingArea.Right - this.Width;
            int y = Screen.PrimaryScreen.WorkingArea.Bottom - taskbarHeight - this.Height;
            this.Location = new Point(x, y);
        }

        private void runApp_Click(object sender, EventArgs e)
        {
            if (runApp.Text == "ON")
            {
                instructionLabel.Enabled = true;
                runApp.ForeColor = Color.Red;
                runApp.Text = "OFF";
                MouseHook.Start();
            }
            else
            {
                runApp.ForeColor = Color.Green;
                instructionLabel.Enabled = false;
                runApp.Text = "ON";
                MouseHook.Stop();
            }
        }
        private void MouseHook_RightButtonHold(object sender, EventArgs e)
        {
            RightButtonHoldEventArgs args = (RightButtonHoldEventArgs)e;
            int holdDurationSeconds = (int)Math.Round(args.HoldDurationSeconds);
            if (holdDurationSeconds > 2)
            {
                MouseHook.CaptureScreen();
            }
            //MessageBox.Show($"Right mouse button held for {holdDurationSeconds} seconds.");
        }

        private void instructionLabel_Click(object sender, EventArgs e)
        {

        }
    }
}
