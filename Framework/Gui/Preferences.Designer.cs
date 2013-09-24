/******************************************************************************
* The MIT License
* Copyright (c) 2008 Rusty Howell, Thomas Wiest
*
* Permission is hereby granted, free of charge, to any person obtaining a copy
* of this software and associated documentation files (the Software), to deal
* in the Software without restriction, including without limitation the rights
* to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
* copies of the Software, and to permit persons to whom the Software is
* furnished to do so, subject to the following conditions:
*
* The above copyright notice and this permission notice shall be included in
* all copies or substantial portions of the Software.
*
* THE SOFTWARE IS PROVIDED AS IS, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
* IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
* FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
* AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
* LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
* OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
* SOFTWARE.
*******************************************************************************/

// Authors:
// 		Thomas Wiest  twiest@users.sourceforge.net
//		Rusty Howell  rhowell@users.sourceforge.net

namespace UvsChess.Gui
{
    internal partial class Preferences
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
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.txtTime = new System.Windows.Forms.TextBox();
            this.txtGrace = new System.Windows.Forms.TextBox();
            this.btnSavePrefs = new System.Windows.Forms.Button();
            this.txtCheckMove = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.btnCancelPrefs = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(15, 30);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(115, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "Time for each turn (ms)";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(15, 82);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(132, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "Grace period (for both, ms)";
            // 
            // txtTime
            // 
            this.txtTime.Location = new System.Drawing.Point(199, 27);
            this.txtTime.Name = "txtTime";
            this.txtTime.Size = new System.Drawing.Size(100, 20);
            this.txtTime.TabIndex = 5;
            // 
            // txtGrace
            // 
            this.txtGrace.Location = new System.Drawing.Point(199, 79);
            this.txtGrace.Name = "txtGrace";
            this.txtGrace.Size = new System.Drawing.Size(100, 20);
            this.txtGrace.TabIndex = 6;
            // 
            // btnSavePrefs
            // 
            this.btnSavePrefs.Location = new System.Drawing.Point(128, 119);
            this.btnSavePrefs.Name = "btnSavePrefs";
            this.btnSavePrefs.Size = new System.Drawing.Size(94, 23);
            this.btnSavePrefs.TabIndex = 7;
            this.btnSavePrefs.Text = "&Save";
            this.btnSavePrefs.UseVisualStyleBackColor = true;
            this.btnSavePrefs.Click += new System.EventHandler(this.btnSavePrefs_Click);
            // 
            // txtCheckMove
            // 
            this.txtCheckMove.Location = new System.Drawing.Point(199, 53);
            this.txtCheckMove.Name = "txtCheckMove";
            this.txtCheckMove.Size = new System.Drawing.Size(100, 20);
            this.txtCheckMove.TabIndex = 8;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(15, 56);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(127, 13);
            this.label3.TabIndex = 9;
            this.label3.Text = "Check Move timeout (ms)";
            // 
            // btnCancelPrefs
            // 
            this.btnCancelPrefs.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancelPrefs.Location = new System.Drawing.Point(228, 119);
            this.btnCancelPrefs.Name = "btnCancelPrefs";
            this.btnCancelPrefs.Size = new System.Drawing.Size(75, 23);
            this.btnCancelPrefs.TabIndex = 10;
            this.btnCancelPrefs.Text = "&Cancel";
            this.btnCancelPrefs.UseVisualStyleBackColor = true;
            // 
            // Preferences
            // 
            this.AcceptButton = this.btnSavePrefs;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancelPrefs;
            this.ClientSize = new System.Drawing.Size(315, 154);
            this.Controls.Add(this.btnCancelPrefs);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.txtCheckMove);
            this.Controls.Add(this.btnSavePrefs);
            this.Controls.Add(this.txtGrace);
            this.Controls.Add(this.txtTime);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Name = "Preferences";
            this.Text = "UvsChess Preferences";
            this.Load += new System.EventHandler(this.Preferences_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtTime;
        private System.Windows.Forms.TextBox txtGrace;
        private System.Windows.Forms.Button btnSavePrefs;
        private System.Windows.Forms.TextBox txtCheckMove;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button btnCancelPrefs;
    }
}