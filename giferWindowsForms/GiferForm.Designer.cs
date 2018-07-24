namespace gifer
{
    partial class GiferForm
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(GiferForm));
			this.pictureBox1 = new System.Windows.Forms.PictureBox();
			this.timer1 = new System.Windows.Forms.Timer(this.components);
			this.timerUpdateTaskbarIcon = new System.Windows.Forms.Timer(this.components);
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.label10 = new System.Windows.Forms.Label();
			this.label11 = new System.Windows.Forms.Label();
			this.label12 = new System.Windows.Forms.Label();
			this.label9 = new System.Windows.Forms.Label();
			this.label8 = new System.Windows.Forms.Label();
			this.label7 = new System.Windows.Forms.Label();
			this.label6 = new System.Windows.Forms.Label();
			this.label4 = new System.Windows.Forms.Label();
			this.label15 = new System.Windows.Forms.Label();
			this.label5 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.label3 = new System.Windows.Forms.Label();
			this.labelDragAndDrop = new System.Windows.Forms.Label();
			((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
			this.groupBox1.SuspendLayout();
			this.SuspendLayout();
			// 
			// pictureBox1
			// 
			this.pictureBox1.Location = new System.Drawing.Point(0, 0);
			this.pictureBox1.Name = "pictureBox1";
			this.pictureBox1.Size = new System.Drawing.Size(375, 375);
			this.pictureBox1.TabIndex = 0;
			this.pictureBox1.TabStop = false;
			this.pictureBox1.MouseClick += new System.Windows.Forms.MouseEventHandler(this.pictureBox1_MouseClick);
			this.pictureBox1.MouseDown += new System.Windows.Forms.MouseEventHandler(this.pictureBox1_MouseDown);
			this.pictureBox1.MouseMove += new System.Windows.Forms.MouseEventHandler(this.pictureBox1_MouseMove);
			this.pictureBox1.MouseUp += new System.Windows.Forms.MouseEventHandler(this.pictureBox1_MouseUp);
			this.pictureBox1.Resize += new System.EventHandler(this.pictureBox1_Resize);
			// 
			// timer1
			// 
			this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
			// 
			// timerUpdateTaskbarIcon
			// 
			this.timerUpdateTaskbarIcon.Tick += new System.EventHandler(this.timerUpdateTaskbarIcon_Tick);
			// 
			// groupBox1
			// 
			this.groupBox1.Controls.Add(this.label10);
			this.groupBox1.Controls.Add(this.label11);
			this.groupBox1.Controls.Add(this.label12);
			this.groupBox1.Controls.Add(this.label9);
			this.groupBox1.Controls.Add(this.label8);
			this.groupBox1.Controls.Add(this.label7);
			this.groupBox1.Controls.Add(this.label6);
			this.groupBox1.Controls.Add(this.label4);
			this.groupBox1.Controls.Add(this.label15);
			this.groupBox1.Controls.Add(this.label5);
			this.groupBox1.Controls.Add(this.label2);
			this.groupBox1.Controls.Add(this.label3);
			this.groupBox1.Location = new System.Drawing.Point(12, 6);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(350, 350);
			this.groupBox1.TabIndex = 5;
			this.groupBox1.TabStop = false;
			this.groupBox1.DragDrop += new System.Windows.Forms.DragEventHandler(this.groupBox1_DragDrop);
			// 
			// label10
			// 
			this.label10.AutoSize = true;
			this.label10.Font = new System.Drawing.Font("Consolas", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
			this.label10.ForeColor = System.Drawing.Color.DarkGray;
			this.label10.Location = new System.Drawing.Point(129, 117);
			this.label10.Name = "label10";
			this.label10.Size = new System.Drawing.Size(207, 19);
			this.label10.TabIndex = 22;
			this.label10.Text = "Previous←, next→ image";
			// 
			// label11
			// 
			this.label11.AutoSize = true;
			this.label11.Font = new System.Drawing.Font("Consolas", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
			this.label11.ForeColor = System.Drawing.Color.DarkGray;
			this.label11.Location = new System.Drawing.Point(315, 135);
			this.label11.Name = "label11";
			this.label11.Size = new System.Drawing.Size(25, 28);
			this.label11.TabIndex = 23;
			this.label11.Text = "🗑";
			// 
			// label12
			// 
			this.label12.AutoSize = true;
			this.label12.Font = new System.Drawing.Font("Consolas", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
			this.label12.ForeColor = System.Drawing.Color.DarkGray;
			this.label12.Location = new System.Drawing.Point(141, 139);
			this.label12.Name = "label12";
			this.label12.Size = new System.Drawing.Size(180, 19);
			this.label12.TabIndex = 24;
			this.label12.Text = "Move to Recycle Bin";
			// 
			// label9
			// 
			this.label9.AutoSize = true;
			this.label9.Font = new System.Drawing.Font("Consolas", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
			this.label9.ForeColor = System.Drawing.Color.DarkGray;
			this.label9.Location = new System.Drawing.Point(282, 73);
			this.label9.Name = "label9";
			this.label9.Size = new System.Drawing.Size(54, 19);
			this.label9.TabIndex = 21;
			this.label9.Text = "Zoom±";
			// 
			// label8
			// 
			this.label8.AutoSize = true;
			this.label8.Font = new System.Drawing.Font("Consolas", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
			this.label8.ForeColor = System.Drawing.Color.DarkGray;
			this.label8.Location = new System.Drawing.Point(15, 95);
			this.label8.Name = "label8";
			this.label8.Size = new System.Drawing.Size(238, 19);
			this.label8.TabIndex = 20;
			this.label8.Text = "Mouse🖱 right click, [Esc]";
			// 
			// label7
			// 
			this.label7.AutoSize = true;
			this.label7.Font = new System.Drawing.Font("Consolas", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
			this.label7.ForeColor = System.Drawing.Color.DarkGray;
			this.label7.Location = new System.Drawing.Point(15, 73);
			this.label7.Name = "label7";
			this.label7.Size = new System.Drawing.Size(131, 19);
			this.label7.TabIndex = 19;
			this.label7.Text = "Mouse🖱 wheel↻";
			// 
			// label6
			// 
			this.label6.AutoSize = true;
			this.label6.Font = new System.Drawing.Font("Consolas", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
			this.label6.ForeColor = System.Drawing.Color.DarkGray;
			this.label6.Location = new System.Drawing.Point(281, 51);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size(56, 19);
			this.label6.TabIndex = 18;
			this.label6.Text = "Move✢";
			// 
			// label4
			// 
			this.label4.AutoSize = true;
			this.label4.Font = new System.Drawing.Font("Consolas", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
			this.label4.ForeColor = System.Drawing.Color.DarkGray;
			this.label4.Location = new System.Drawing.Point(10, 19);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(94, 24);
			this.label4.TabIndex = 17;
			this.label4.Text = "Hotkeys";
			// 
			// label15
			// 
			this.label15.AutoSize = true;
			this.label15.Font = new System.Drawing.Font("Consolas", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
			this.label15.ForeColor = System.Drawing.Color.DarkGray;
			this.label15.Location = new System.Drawing.Point(271, 95);
			this.label15.Name = "label15";
			this.label15.Size = new System.Drawing.Size(66, 19);
			this.label15.TabIndex = 15;
			this.label15.Text = "Close❌";
			// 
			// label5
			// 
			this.label5.AutoSize = true;
			this.label5.Font = new System.Drawing.Font("Consolas", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
			this.label5.ForeColor = System.Drawing.Color.DarkGray;
			this.label5.Location = new System.Drawing.Point(12, 139);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(81, 19);
			this.label5.TabIndex = 7;
			this.label5.Text = "[Delete]";
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Font = new System.Drawing.Font("Consolas", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
			this.label2.ForeColor = System.Drawing.Color.DarkGray;
			this.label2.Location = new System.Drawing.Point(15, 51);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(157, 19);
			this.label2.TabIndex = 1;
			this.label2.Text = "Mouse🖱 left drag";
			// 
			// label3
			// 
			this.label3.AutoSize = true;
			this.label3.Font = new System.Drawing.Font("Consolas", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
			this.label3.ForeColor = System.Drawing.Color.DarkGray;
			this.label3.Location = new System.Drawing.Point(12, 117);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(81, 19);
			this.label3.TabIndex = 2;
			this.label3.Text = "[←], [→]";
			// 
			// labelDragAndDrop
			// 
			this.labelDragAndDrop.AutoSize = true;
			this.labelDragAndDrop.Font = new System.Drawing.Font("Consolas", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
			this.labelDragAndDrop.ForeColor = System.Drawing.Color.Silver;
			this.labelDragAndDrop.Location = new System.Drawing.Point(26, 340);
			this.labelDragAndDrop.Name = "labelDragAndDrop";
			this.labelDragAndDrop.Size = new System.Drawing.Size(322, 24);
			this.labelDragAndDrop.TabIndex = 16;
			this.labelDragAndDrop.Text = "[Drag&&Drop GIF/Image Here]";
			// 
			// GiferForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(375, 375);
			this.Controls.Add(this.labelDragAndDrop);
			this.Controls.Add(this.groupBox1);
			this.Controls.Add(this.pictureBox1);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "GiferForm";
			this.Activated += new System.EventHandler(this.GiferForm_Activated);
			this.Deactivate += new System.EventHandler(this.GiferForm_Deactivate);
			this.Load += new System.EventHandler(this.GiferForm_Load);
			this.DragDrop += new System.Windows.Forms.DragEventHandler(this.Form1_DragDrop);
			this.DragEnter += new System.Windows.Forms.DragEventHandler(this.Form1_DragEnter);
			this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.GiferForm_KeyDown);
			((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
			this.groupBox1.ResumeLayout(false);
			this.groupBox1.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox pictureBox1;
		private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.Timer timerUpdateTaskbarIcon;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label labelDragAndDrop;
        private System.Windows.Forms.Label label15;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label label11;
		private System.Windows.Forms.Label label12;
	}
}

