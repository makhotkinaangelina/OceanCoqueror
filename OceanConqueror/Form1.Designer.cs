namespace OceanConqueror
{
    partial class Form1
    {
        /// <summary>
        /// Обязательная переменная конструктора.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Освободить все используемые ресурсы.
        /// </summary>
        /// <param name="disposing">истинно, если управляемый ресурс должен быть удален; иначе ложно.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Код, автоматически созданный конструктором форм Windows

        /// <summary>
        /// Требуемый метод для поддержки конструктора — не изменяйте 
        /// содержимое этого метода с помощью редактора кода.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.bgIm2 = new System.Windows.Forms.PictureBox();
            this.scoreLabel = new System.Windows.Forms.Label();
            this.timeLabel = new System.Windows.Forms.Label();
            this.timer = new System.Windows.Forms.Timer(this.components);
            this.live = new System.Windows.Forms.PictureBox();
            this.live2 = new System.Windows.Forms.PictureBox();
            this.live3 = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.bgIm2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.live)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.live2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.live3)).BeginInit();
            this.SuspendLayout();
            // 
            // bgIm2
            // 
            this.bgIm2.Image = ((System.Drawing.Image)(resources.GetObject("bgIm2.Image")));
            this.bgIm2.Location = new System.Drawing.Point(0, -700);
            this.bgIm2.Margin = new System.Windows.Forms.Padding(0);
            this.bgIm2.MaximumSize = new System.Drawing.Size(1000, 700);
            this.bgIm2.MinimumSize = new System.Drawing.Size(1000, 700);
            this.bgIm2.Name = "bgIm2";
            this.bgIm2.Size = new System.Drawing.Size(1000, 700);
            this.bgIm2.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.bgIm2.TabIndex = 1;
            this.bgIm2.TabStop = false;
            this.bgIm2.Visible = false;
            // 
            // scoreLabel
            // 
            this.scoreLabel.Font = new System.Drawing.Font("Mistral", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.scoreLabel.Location = new System.Drawing.Point(1349, 56);
            this.scoreLabel.Name = "scoreLabel";
            this.scoreLabel.Size = new System.Drawing.Size(139, 38);
            this.scoreLabel.TabIndex = 2;
            // 
            // timeLabel
            // 
            this.timeLabel.Font = new System.Drawing.Font("Snap ITC", 10.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.timeLabel.Location = new System.Drawing.Point(814, 959);
            this.timeLabel.Margin = new System.Windows.Forms.Padding(0);
            this.timeLabel.Name = "timeLabel";
            this.timeLabel.Size = new System.Drawing.Size(177, 32);
            this.timeLabel.TabIndex = 3;
            // 
            // timer
            // 
            this.timer.Enabled = true;
            this.timer.Interval = 1000;
            this.timer.Tick += new System.EventHandler(this.timer_Tick);
            // 
            // live
            // 
            this.live.Image = ((System.Drawing.Image)(resources.GetObject("live.Image")));
            this.live.Location = new System.Drawing.Point(1330, -2);
            this.live.Name = "live";
            this.live.Size = new System.Drawing.Size(55, 55);
            this.live.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.live.TabIndex = 6;
            this.live.TabStop = false;
            // 
            // live2
            // 
            this.live2.Image = ((System.Drawing.Image)(resources.GetObject("live2.Image")));
            this.live2.Location = new System.Drawing.Point(1388, -2);
            this.live2.Name = "live2";
            this.live2.Size = new System.Drawing.Size(55, 55);
            this.live2.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.live2.TabIndex = 7;
            this.live2.TabStop = false;
            // 
            // live3
            // 
            this.live3.Image = ((System.Drawing.Image)(resources.GetObject("live3.Image")));
            this.live3.Location = new System.Drawing.Point(1446, -2);
            this.live3.Name = "live3";
            this.live3.Size = new System.Drawing.Size(55, 55);
            this.live3.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.live3.TabIndex = 8;
            this.live3.TabStop = false;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.CornflowerBlue;
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.ClientSize = new System.Drawing.Size(1500, 1000);
            this.Controls.Add(this.live3);
            this.Controls.Add(this.live2);
            this.Controls.Add(this.live);
            this.Controls.Add(this.timeLabel);
            this.Controls.Add(this.scoreLabel);
            this.Controls.Add(this.bgIm2);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximumSize = new System.Drawing.Size(1500, 1000);
            this.MinimumSize = new System.Drawing.Size(1500, 1000);
            this.Name = "Form1";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Sky Invader";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
            this.Load += new System.EventHandler(this.Form1_Load);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.Form1_KeyDown);
            this.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.Form1_KeyPress);
            ((System.ComponentModel.ISupportInitialize)(this.bgIm2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.live)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.live2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.live3)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.PictureBox curPlayer;
        private System.Windows.Forms.PictureBox bgIm2;
        private System.Windows.Forms.Label scoreLabel;
        private System.Windows.Forms.Label timeLabel;
        private System.Windows.Forms.Timer timer;
        private System.Windows.Forms.PictureBox live;
        private System.Windows.Forms.PictureBox live2;
        private System.Windows.Forms.PictureBox live3;
    }
}

