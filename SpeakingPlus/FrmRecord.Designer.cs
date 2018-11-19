namespace SpeakingPlus
{
    partial class FrmRecord
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FrmRecord));
            this.btnRecord = new System.Windows.Forms.Button();
            this.textBoxTimer = new System.Windows.Forms.TextBox();
            this.pictureBoxVolumeMeterBack = new System.Windows.Forms.PictureBox();
            this.pictureBoxVolumeMeterFront = new System.Windows.Forms.PictureBox();
            this.timerVolumeMeter = new System.Windows.Forms.Timer(this.components);
            this.timerForDisplay = new System.Windows.Forms.Timer(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxVolumeMeterBack)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxVolumeMeterFront)).BeginInit();
            this.SuspendLayout();
            // 
            // btnRecord
            // 
            this.btnRecord.Location = new System.Drawing.Point(80, 55);
            this.btnRecord.Name = "btnRecord";
            this.btnRecord.Size = new System.Drawing.Size(109, 33);
            this.btnRecord.TabIndex = 0;
            this.btnRecord.Text = "Record";
            this.btnRecord.UseVisualStyleBackColor = true;
            this.btnRecord.Click += new System.EventHandler(this.btnRecord_Click);
            // 
            // textBoxTimer
            // 
            this.textBoxTimer.Font = new System.Drawing.Font("MS UI Gothic", 18F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.textBoxTimer.Location = new System.Drawing.Point(80, 110);
            this.textBoxTimer.Multiline = true;
            this.textBoxTimer.Name = "textBoxTimer";
            this.textBoxTimer.ReadOnly = true;
            this.textBoxTimer.Size = new System.Drawing.Size(109, 37);
            this.textBoxTimer.TabIndex = 1;
            this.textBoxTimer.Text = "00:00:00";
            // 
            // pictureBoxVolumeMeterBack
            // 
            this.pictureBoxVolumeMeterBack.BackColor = System.Drawing.Color.Black;
            this.pictureBoxVolumeMeterBack.Location = new System.Drawing.Point(70, 171);
            this.pictureBoxVolumeMeterBack.Name = "pictureBoxVolumeMeterBack";
            this.pictureBoxVolumeMeterBack.Size = new System.Drawing.Size(130, 40);
            this.pictureBoxVolumeMeterBack.TabIndex = 2;
            this.pictureBoxVolumeMeterBack.TabStop = false;
            // 
            // pictureBoxVolumeMeterFront
            // 
            this.pictureBoxVolumeMeterFront.BackColor = System.Drawing.Color.Green;
            this.pictureBoxVolumeMeterFront.Location = new System.Drawing.Point(70, 171);
            this.pictureBoxVolumeMeterFront.Name = "pictureBoxVolumeMeterFront";
            this.pictureBoxVolumeMeterFront.Size = new System.Drawing.Size(10, 40);
            this.pictureBoxVolumeMeterFront.TabIndex = 3;
            this.pictureBoxVolumeMeterFront.TabStop = false;
            // 
            // timerVolumeMeter
            // 
            this.timerVolumeMeter.Enabled = true;
            this.timerVolumeMeter.Tick += new System.EventHandler(this.timerVolumeMeter_Tick);
            // 
            // timerForDisplay
            // 
            this.timerForDisplay.Interval = 1000;
            this.timerForDisplay.Tick += new System.EventHandler(this.timerForDisplay_Tick);
            // 
            // FrmRecord
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(284, 261);
            this.Controls.Add(this.pictureBoxVolumeMeterFront);
            this.Controls.Add(this.pictureBoxVolumeMeterBack);
            this.Controls.Add(this.textBoxTimer);
            this.Controls.Add(this.btnRecord);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "FrmRecord";
            this.Text = "Speaking Plus";
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxVolumeMeterBack)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxVolumeMeterFront)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnRecord;
        private System.Windows.Forms.TextBox textBoxTimer;
        private System.Windows.Forms.PictureBox pictureBoxVolumeMeterBack;
        private System.Windows.Forms.PictureBox pictureBoxVolumeMeterFront;
        private System.Windows.Forms.Timer timerVolumeMeter;
        private System.Windows.Forms.Timer timerForDisplay;

    }
}