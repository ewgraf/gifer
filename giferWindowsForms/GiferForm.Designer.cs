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
            this.labelMoveText = new System.Windows.Forms.Label();
            this.labelMoveIcon = new System.Windows.Forms.Label();
            this.labelDeleteIcon = new System.Windows.Forms.Label();
            this.labelCloseKey1 = new System.Windows.Forms.Label();
            this.labelCloseKey0 = new System.Windows.Forms.Label();
            this.labelZoomKey = new System.Windows.Forms.Label();
            this.labelHelpText = new System.Windows.Forms.Label();
            this.labelHelpKey = new System.Windows.Forms.Label();
            this.labelPrevNextText = new System.Windows.Forms.Label();
            this.labelDeleteText = new System.Windows.Forms.Label();
            this.labelZoomText = new System.Windows.Forms.Label();
            this.labelCloseText = new System.Windows.Forms.Label();
            this.labelDeleteKey = new System.Windows.Forms.Label();
            this.labelPrevNextKey = new System.Windows.Forms.Label();
            this.labelMoveKey = new System.Windows.Forms.Label();
            this.labelZoomIcon = new System.Windows.Forms.Label();
            this.labelCloseIcon = new System.Windows.Forms.Label();
            this.labelDragAndDrop = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // pictureBox1
            // 
            this.pictureBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pictureBox1.BackColor = System.Drawing.SystemColors.Control;
            this.pictureBox1.Location = new System.Drawing.Point(0, 0);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(450, 431);
            this.pictureBox1.TabIndex = 0;
            this.pictureBox1.TabStop = false;
            this.pictureBox1.MouseClick += new System.Windows.Forms.MouseEventHandler(this.pictureBox1_MouseClick);
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
            this.groupBox1.Controls.Add(this.labelMoveText);
            this.groupBox1.Controls.Add(this.labelMoveIcon);
            this.groupBox1.Controls.Add(this.labelDeleteIcon);
            this.groupBox1.Controls.Add(this.labelCloseKey1);
            this.groupBox1.Controls.Add(this.labelCloseKey0);
            this.groupBox1.Controls.Add(this.labelZoomKey);
            this.groupBox1.Controls.Add(this.labelHelpText);
            this.groupBox1.Controls.Add(this.labelHelpKey);
            this.groupBox1.Controls.Add(this.labelPrevNextText);
            this.groupBox1.Controls.Add(this.labelDeleteText);
            this.groupBox1.Controls.Add(this.labelZoomText);
            this.groupBox1.Controls.Add(this.labelCloseText);
            this.groupBox1.Controls.Add(this.labelDeleteKey);
            this.groupBox1.Controls.Add(this.labelPrevNextKey);
            this.groupBox1.Controls.Add(this.labelMoveKey);
            this.groupBox1.Controls.Add(this.labelZoomIcon);
            this.groupBox1.Controls.Add(this.labelCloseIcon);
            this.groupBox1.Font = new System.Drawing.Font("Consolas", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.groupBox1.ForeColor = System.Drawing.Color.DarkGray;
            this.groupBox1.Location = new System.Drawing.Point(22, 8);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(406, 402);
            this.groupBox1.TabIndex = 5;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Hotkeys";
            this.groupBox1.DragDrop += new System.Windows.Forms.DragEventHandler(this.groupBox1_DragDrop);
            // 
            // labelMoveText
            // 
            this.labelMoveText.AutoSize = true;
            this.labelMoveText.Font = new System.Drawing.Font("Consolas", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.labelMoveText.ForeColor = System.Drawing.Color.DarkGray;
            this.labelMoveText.Location = new System.Drawing.Point(191, 105);
            this.labelMoveText.Name = "labelMoveText";
            this.labelMoveText.Size = new System.Drawing.Size(50, 22);
            this.labelMoveText.TabIndex = 18;
            this.labelMoveText.Text = "Move";
            // 
            // labelMoveIcon
            // 
            this.labelMoveIcon.AutoSize = true;
            this.labelMoveIcon.Font = new System.Drawing.Font("Consolas", 26.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.labelMoveIcon.ForeColor = System.Drawing.Color.DarkGray;
            this.labelMoveIcon.Location = new System.Drawing.Point(232, 94);
            this.labelMoveIcon.Name = "labelMoveIcon";
            this.labelMoveIcon.Size = new System.Drawing.Size(45, 41);
            this.labelMoveIcon.TabIndex = 31;
            this.labelMoveIcon.Text = "✢";
            // 
            // labelDeleteIcon
            // 
            this.labelDeleteIcon.AutoSize = true;
            this.labelDeleteIcon.Font = new System.Drawing.Font("Consolas", 26.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.labelDeleteIcon.ForeColor = System.Drawing.Color.DarkGray;
            this.labelDeleteIcon.Location = new System.Drawing.Point(344, 276);
            this.labelDeleteIcon.Name = "labelDeleteIcon";
            this.labelDeleteIcon.Size = new System.Drawing.Size(39, 41);
            this.labelDeleteIcon.TabIndex = 23;
            this.labelDeleteIcon.Text = "🗑";
            // 
            // labelCloseKey1
            // 
            this.labelCloseKey1.AutoSize = true;
            this.labelCloseKey1.Font = new System.Drawing.Font("Consolas", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.labelCloseKey1.ForeColor = System.Drawing.Color.DarkGray;
            this.labelCloseKey1.Location = new System.Drawing.Point(98, 183);
            this.labelCloseKey1.Name = "labelCloseKey1";
            this.labelCloseKey1.Size = new System.Drawing.Size(80, 22);
            this.labelCloseKey1.TabIndex = 20;
            this.labelCloseKey1.Text = ", [Esc]";
            // 
            // labelCloseKey0
            // 
            this.labelCloseKey0.AutoSize = true;
            this.labelCloseKey0.Font = new System.Drawing.Font("Consolas", 26.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.labelCloseKey0.ForeColor = System.Drawing.Color.DarkGray;
            this.labelCloseKey0.Location = new System.Drawing.Point(52, 168);
            this.labelCloseKey0.Name = "labelCloseKey0";
            this.labelCloseKey0.Size = new System.Drawing.Size(63, 41);
            this.labelCloseKey0.TabIndex = 30;
            this.labelCloseKey0.Text = "🖰\'";
            // 
            // labelZoomKey
            // 
            this.labelZoomKey.AutoSize = true;
            this.labelZoomKey.Font = new System.Drawing.Font("Consolas", 26.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.labelZoomKey.ForeColor = System.Drawing.Color.DarkGray;
            this.labelZoomKey.Location = new System.Drawing.Point(52, 129);
            this.labelZoomKey.Name = "labelZoomKey";
            this.labelZoomKey.Size = new System.Drawing.Size(69, 41);
            this.labelZoomKey.TabIndex = 28;
            this.labelZoomKey.Text = "🖰↻";
            // 
            // labelHelpText
            // 
            this.labelHelpText.AutoSize = true;
            this.labelHelpText.Font = new System.Drawing.Font("Consolas", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.labelHelpText.ForeColor = System.Drawing.Color.DarkGray;
            this.labelHelpText.Location = new System.Drawing.Point(191, 254);
            this.labelHelpText.Name = "labelHelpText";
            this.labelHelpText.Size = new System.Drawing.Size(120, 22);
            this.labelHelpText.TabIndex = 26;
            this.labelHelpText.Text = "Help (this)";
            // 
            // labelHelpKey
            // 
            this.labelHelpKey.AutoSize = true;
            this.labelHelpKey.Font = new System.Drawing.Font("Consolas", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.labelHelpKey.ForeColor = System.Drawing.Color.DarkGray;
            this.labelHelpKey.Location = new System.Drawing.Point(58, 254);
            this.labelHelpKey.Name = "labelHelpKey";
            this.labelHelpKey.Size = new System.Drawing.Size(40, 22);
            this.labelHelpKey.TabIndex = 25;
            this.labelHelpKey.Text = "[H]";
            // 
            // labelPrevNextText
            // 
            this.labelPrevNextText.AutoSize = true;
            this.labelPrevNextText.Font = new System.Drawing.Font("Consolas", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.labelPrevNextText.ForeColor = System.Drawing.Color.DarkGray;
            this.labelPrevNextText.Location = new System.Drawing.Point(191, 220);
            this.labelPrevNextText.Name = "labelPrevNextText";
            this.labelPrevNextText.Size = new System.Drawing.Size(190, 22);
            this.labelPrevNextText.TabIndex = 22;
            this.labelPrevNextText.Text = "Prev←, next→ image";
            // 
            // labelDeleteText
            // 
            this.labelDeleteText.AutoSize = true;
            this.labelDeleteText.Font = new System.Drawing.Font("Consolas", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.labelDeleteText.ForeColor = System.Drawing.Color.DarkGray;
            this.labelDeleteText.Location = new System.Drawing.Point(191, 288);
            this.labelDeleteText.Name = "labelDeleteText";
            this.labelDeleteText.Size = new System.Drawing.Size(160, 22);
            this.labelDeleteText.TabIndex = 24;
            this.labelDeleteText.Text = "Move to Recycle";
            // 
            // labelZoomText
            // 
            this.labelZoomText.AutoSize = true;
            this.labelZoomText.Font = new System.Drawing.Font("Consolas", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.labelZoomText.ForeColor = System.Drawing.Color.DarkGray;
            this.labelZoomText.Location = new System.Drawing.Point(191, 143);
            this.labelZoomText.Name = "labelZoomText";
            this.labelZoomText.Size = new System.Drawing.Size(50, 22);
            this.labelZoomText.TabIndex = 21;
            this.labelZoomText.Text = "Zoom";
            // 
            // labelCloseText
            // 
            this.labelCloseText.AutoSize = true;
            this.labelCloseText.Font = new System.Drawing.Font("Consolas", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.labelCloseText.ForeColor = System.Drawing.Color.DarkGray;
            this.labelCloseText.Location = new System.Drawing.Point(191, 183);
            this.labelCloseText.Name = "labelCloseText";
            this.labelCloseText.Size = new System.Drawing.Size(60, 22);
            this.labelCloseText.TabIndex = 15;
            this.labelCloseText.Text = "Close";
            // 
            // labelDeleteKey
            // 
            this.labelDeleteKey.AutoSize = true;
            this.labelDeleteKey.Font = new System.Drawing.Font("Consolas", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.labelDeleteKey.ForeColor = System.Drawing.Color.DarkGray;
            this.labelDeleteKey.Location = new System.Drawing.Point(57, 288);
            this.labelDeleteKey.Name = "labelDeleteKey";
            this.labelDeleteKey.Size = new System.Drawing.Size(90, 22);
            this.labelDeleteKey.TabIndex = 7;
            this.labelDeleteKey.Text = "[Delete]";
            // 
            // labelPrevNextKey
            // 
            this.labelPrevNextKey.AutoSize = true;
            this.labelPrevNextKey.Font = new System.Drawing.Font("Consolas", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.labelPrevNextKey.ForeColor = System.Drawing.Color.DarkGray;
            this.labelPrevNextKey.Location = new System.Drawing.Point(58, 219);
            this.labelPrevNextKey.Name = "labelPrevNextKey";
            this.labelPrevNextKey.Size = new System.Drawing.Size(100, 22);
            this.labelPrevNextKey.TabIndex = 2;
            this.labelPrevNextKey.Text = "[←] , [→]";
            // 
            // labelMoveKey
            // 
            this.labelMoveKey.AutoSize = true;
            this.labelMoveKey.Font = new System.Drawing.Font("Consolas", 26.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.labelMoveKey.ForeColor = System.Drawing.Color.DarkGray;
            this.labelMoveKey.Location = new System.Drawing.Point(33, 90);
            this.labelMoveKey.Name = "labelMoveKey";
            this.labelMoveKey.Size = new System.Drawing.Size(82, 41);
            this.labelMoveKey.TabIndex = 27;
            this.labelMoveKey.Text = "\'🖰→";
            // 
            // labelZoomIcon
            // 
            this.labelZoomIcon.AutoSize = true;
            this.labelZoomIcon.Font = new System.Drawing.Font("Consolas", 26.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.labelZoomIcon.ForeColor = System.Drawing.Color.DarkGray;
            this.labelZoomIcon.Location = new System.Drawing.Point(235, 133);
            this.labelZoomIcon.Name = "labelZoomIcon";
            this.labelZoomIcon.Size = new System.Drawing.Size(37, 41);
            this.labelZoomIcon.TabIndex = 32;
            this.labelZoomIcon.Text = "±";
            // 
            // labelCloseIcon
            // 
            this.labelCloseIcon.AutoSize = true;
            this.labelCloseIcon.Font = new System.Drawing.Font("Consolas", 21.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.labelCloseIcon.ForeColor = System.Drawing.Color.DarkGray;
            this.labelCloseIcon.Location = new System.Drawing.Point(244, 176);
            this.labelCloseIcon.Name = "labelCloseIcon";
            this.labelCloseIcon.Size = new System.Drawing.Size(36, 34);
            this.labelCloseIcon.TabIndex = 33;
            this.labelCloseIcon.Text = "❌";
            // 
            // labelDragAndDrop
            // 
            this.labelDragAndDrop.AutoSize = true;
            this.labelDragAndDrop.Font = new System.Drawing.Font("Consolas", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.labelDragAndDrop.ForeColor = System.Drawing.Color.DarkGray;
            this.labelDragAndDrop.Location = new System.Drawing.Point(63, 394);
            this.labelDragAndDrop.Name = "labelDragAndDrop";
            this.labelDragAndDrop.Size = new System.Drawing.Size(322, 24);
            this.labelDragAndDrop.TabIndex = 16;
            this.labelDragAndDrop.Text = "[Drag&&Drop GIF/Image Here]";
            // 
            // GiferForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(450, 431);
            this.Controls.Add(this.labelDragAndDrop);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.pictureBox1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "GiferForm";
            this.Activated += new System.EventHandler(this.GiferForm_Activated);
            this.Deactivate += new System.EventHandler(this.GiferForm_Deactivate);
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
		private System.Windows.Forms.Label labelMoveKey;
		private System.Windows.Forms.Label labelHelpText;
		private System.Windows.Forms.Label labelHelpKey;
		private System.Windows.Forms.Label labelPrevNextText;
		private System.Windows.Forms.Label labelDeleteIcon;
		private System.Windows.Forms.Label labelDeleteText;
		private System.Windows.Forms.Label labelZoomText;
		private System.Windows.Forms.Label labelCloseKey1;
		private System.Windows.Forms.Label labelMoveText;
		private System.Windows.Forms.Label labelCloseText;
		private System.Windows.Forms.Label labelDeleteKey;
		private System.Windows.Forms.Label labelPrevNextKey;
		private System.Windows.Forms.Label labelZoomKey;
		private System.Windows.Forms.Label labelCloseKey0;
        private System.Windows.Forms.Label labelMoveIcon;
        private System.Windows.Forms.Label labelZoomIcon;
        private System.Windows.Forms.Label labelCloseIcon;
    }
}

