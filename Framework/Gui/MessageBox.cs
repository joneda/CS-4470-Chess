using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace UvsChess.Gui
{
    internal partial class MessageBox : Form
    {
        public static Form MainForm = null;

        public MessageBox()
        {
            InitializeComponent();
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            this.Close();            
        }

        public static void Show(string text)
        {
            MessageBox mb = new MessageBox();
            mb.lblText.Text = text;
            MainForm.Enabled = false;
            mb.Show(MainForm);            
            mb.Location = new Point((MainForm.Width / 2) - (mb.Width / 2), (MainForm.Height / 2) - (mb.Height / 2));
        }

        private void MessageBox_FormClosing(object sender, FormClosingEventArgs e)
        {
            MainForm.Enabled = true;
        }
    }
}
