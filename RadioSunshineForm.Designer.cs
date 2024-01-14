namespace RadioSunshine {
    partial class RadioSunshineForm {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing) {
            if (disposing && (components != null)) {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(RadioSunshineForm));
            label1 = new Label();
            label2 = new Label();
            pathText = new TextBox();
            changeBtn = new Button();
            applyBtn = new Button();
            removeBtn = new Button();
            SuspendLayout();
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(12, 23);
            label1.Name = "label1";
            label1.Size = new Size(224, 15);
            label1.TabIndex = 0;
            label1.Text = "Set the fullpath to point to the '.conf' file.";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.ForeColor = SystemColors.ControlDarkDark;
            label2.Location = new Point(12, 38);
            label2.Name = "label2";
            label2.Size = new Size(370, 15);
            label2.TabIndex = 1;
            label2.Text = "e.g. 'C:\\sunshine-windows-portable\\Sunshine\\config\\sunshine.conf'";
            // 
            // pathText
            // 
            pathText.Location = new Point(12, 56);
            pathText.Name = "pathText";
            pathText.ReadOnly = true;
            pathText.Size = new Size(279, 23);
            pathText.TabIndex = 2;
            // 
            // changeBtn
            // 
            changeBtn.Location = new Point(297, 56);
            changeBtn.Name = "changeBtn";
            changeBtn.Size = new Size(75, 23);
            changeBtn.TabIndex = 3;
            changeBtn.Text = "Change";
            changeBtn.UseVisualStyleBackColor = true;
            changeBtn.Click += changeBtn_Click;
            // 
            // applyBtn
            // 
            applyBtn.Enabled = false;
            applyBtn.Location = new Point(12, 91);
            applyBtn.Name = "applyBtn";
            applyBtn.Size = new Size(360, 23);
            applyBtn.TabIndex = 4;
            applyBtn.Text = "Apply Update";
            applyBtn.UseVisualStyleBackColor = true;
            applyBtn.Click += applyBtn_Click;
            // 
            // removeBtn
            // 
            removeBtn.Enabled = false;
            removeBtn.Location = new Point(12, 126);
            removeBtn.Name = "removeBtn";
            removeBtn.Size = new Size(360, 23);
            removeBtn.TabIndex = 5;
            removeBtn.Text = "Remove Update";
            removeBtn.UseVisualStyleBackColor = true;
            removeBtn.Click += removeBtn_Click;
            // 
            // RadioSunshineForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(384, 161);
            Controls.Add(removeBtn);
            Controls.Add(applyBtn);
            Controls.Add(changeBtn);
            Controls.Add(pathText);
            Controls.Add(label2);
            Controls.Add(label1);
            Icon = (Icon)resources.GetObject("$this.Icon");
            MaximizeBox = false;
            MaximumSize = new Size(400, 200);
            MinimumSize = new Size(400, 200);
            Name = "RadioSunshineForm";
            Text = "Radio Sunshine";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label label1;
        private Label label2;
        private TextBox pathText;
        private Button changeBtn;
        private Button applyBtn;
        private Button removeBtn;
    }
}
