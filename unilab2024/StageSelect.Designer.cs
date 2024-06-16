namespace unilab2024
{
    partial class StageSelect
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
            this.labelWorld = new System.Windows.Forms.Label();
            this.buttonToMap = new System.Windows.Forms.Button();
            this.button_Stage1 = new System.Windows.Forms.Button();
            this.button_Stage2 = new System.Windows.Forms.Button();
            this.button_Stage3 = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // labelWorld
            // 
            this.labelWorld.AutoSize = true;
            this.labelWorld.Font = new System.Drawing.Font("游ゴシック", 48F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.labelWorld.Location = new System.Drawing.Point(30, 30);
            this.labelWorld.Name = "labelWorld";
            this.labelWorld.Size = new System.Drawing.Size(395, 82);
            this.labelWorld.TabIndex = 0;
            this.labelWorld.Text = "WorldName";
            // 
            // buttonToMap
            // 
            this.buttonToMap.BackColor = System.Drawing.Color.Gray;
            this.buttonToMap.Cursor = System.Windows.Forms.Cursors.Hand;
            this.buttonToMap.Font = new System.Drawing.Font("游明朝 Demibold", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.buttonToMap.ForeColor = System.Drawing.Color.White;
            this.buttonToMap.Location = new System.Drawing.Point(1350, 10);
            this.buttonToMap.Name = "buttonToMap";
            this.buttonToMap.Size = new System.Drawing.Size(160, 50);
            this.buttonToMap.TabIndex = 1;
            this.buttonToMap.Text = "学年選択に戻る";
            this.buttonToMap.UseVisualStyleBackColor = false;
            this.buttonToMap.Click += new System.EventHandler(this.buttonToMap_Click);
            // 
            // button_Stage1
            // 
            this.button_Stage1.Cursor = System.Windows.Forms.Cursors.Hand;
            this.button_Stage1.Font = new System.Drawing.Font("游ゴシック", 36F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.button_Stage1.Location = new System.Drawing.Point(243, 600);
            this.button_Stage1.Name = "button_Stage1";
            this.button_Stage1.Size = new System.Drawing.Size(250, 80);
            this.button_Stage1.TabIndex = 3;
            this.button_Stage1.Text = "レベル1";
            this.button_Stage1.UseVisualStyleBackColor = true;
            this.button_Stage1.Click += new System.EventHandler(this.button_Stage1_Click);
            // 
            // button_Stage2
            // 
            this.button_Stage2.Cursor = System.Windows.Forms.Cursors.Hand;
            this.button_Stage2.Font = new System.Drawing.Font("游ゴシック", 36F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.button_Stage2.Location = new System.Drawing.Point(643, 600);
            this.button_Stage2.Name = "button_Stage2";
            this.button_Stage2.Size = new System.Drawing.Size(250, 80);
            this.button_Stage2.TabIndex = 4;
            this.button_Stage2.Text = "レベル2";
            this.button_Stage2.UseVisualStyleBackColor = true;
            this.button_Stage2.Click += new System.EventHandler(this.button_Stage2_Click);
            // 
            // button_Stage3
            // 
            this.button_Stage3.Cursor = System.Windows.Forms.Cursors.Hand;
            this.button_Stage3.Font = new System.Drawing.Font("游ゴシック", 36F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.button_Stage3.Location = new System.Drawing.Point(1043, 600);
            this.button_Stage3.Name = "button_Stage3";
            this.button_Stage3.Size = new System.Drawing.Size(250, 80);
            this.button_Stage3.TabIndex = 5;
            this.button_Stage3.Text = "レベル3";
            this.button_Stage3.UseVisualStyleBackColor = true;
            this.button_Stage3.Click += new System.EventHandler(this.button_Stage3_Click);
            // 
            // StageSelect
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1520, 861);
            this.Controls.Add(this.button_Stage3);
            this.Controls.Add(this.button_Stage2);
            this.Controls.Add(this.button_Stage1);
            this.Controls.Add(this.buttonToMap);
            this.Controls.Add(this.labelWorld);
            this.Name = "StageSelect";
            this.Text = "StageSelect";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.Load += new System.EventHandler(this.StageSelect_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label labelWorld;
        private System.Windows.Forms.Button buttonToMap;
        private System.Windows.Forms.Button button_Stage1;
        private System.Windows.Forms.Button button_Stage2;
        private System.Windows.Forms.Button button_Stage3;
    }
}