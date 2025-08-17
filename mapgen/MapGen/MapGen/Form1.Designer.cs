namespace MapGen
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            btn_Generate = new Button();
            num_SizeX = new NumericUpDown();
            label1 = new Label();
            label2 = new Label();
            num_SizeY = new NumericUpDown();
            pbx_Map = new PictureBox();
            label3 = new Label();
            num_Seed = new NumericUpDown();
            label4 = new Label();
            num_Threshold = new NumericUpDown();
            label5 = new Label();
            num_EndPoints = new NumericUpDown();
            ((System.ComponentModel.ISupportInitialize)num_SizeX).BeginInit();
            ((System.ComponentModel.ISupportInitialize)num_SizeY).BeginInit();
            ((System.ComponentModel.ISupportInitialize)pbx_Map).BeginInit();
            ((System.ComponentModel.ISupportInitialize)num_Seed).BeginInit();
            ((System.ComponentModel.ISupportInitialize)num_Threshold).BeginInit();
            ((System.ComponentModel.ISupportInitialize)num_EndPoints).BeginInit();
            SuspendLayout();
            // 
            // btn_Generate
            // 
            btn_Generate.Location = new Point(12, 501);
            btn_Generate.Name = "btn_Generate";
            btn_Generate.Size = new Size(144, 23);
            btn_Generate.TabIndex = 1;
            btn_Generate.Text = "Generate";
            btn_Generate.UseVisualStyleBackColor = true;
            btn_Generate.Click += btn_Generate_Click;
            // 
            // num_SizeX
            // 
            num_SizeX.Location = new Point(63, 443);
            num_SizeX.Maximum = new decimal(new int[] { 1000, 0, 0, 0 });
            num_SizeX.Name = "num_SizeX";
            num_SizeX.Size = new Size(93, 23);
            num_SizeX.TabIndex = 2;
            num_SizeX.Value = new decimal(new int[] { 100, 0, 0, 0 });
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(12, 447);
            label1.Name = "label1";
            label1.Size = new Size(40, 15);
            label1.TabIndex = 3;
            label1.Text = "Size X:";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(12, 476);
            label2.Name = "label2";
            label2.Size = new Size(40, 15);
            label2.TabIndex = 5;
            label2.Text = "Size Y:";
            // 
            // num_SizeY
            // 
            num_SizeY.Location = new Point(63, 472);
            num_SizeY.Maximum = new decimal(new int[] { 1000, 0, 0, 0 });
            num_SizeY.Name = "num_SizeY";
            num_SizeY.Size = new Size(93, 23);
            num_SizeY.TabIndex = 4;
            num_SizeY.Value = new decimal(new int[] { 100, 0, 0, 0 });
            // 
            // pbx_Map
            // 
            pbx_Map.BackColor = Color.Black;
            pbx_Map.Location = new Point(162, 12);
            pbx_Map.Name = "pbx_Map";
            pbx_Map.Size = new Size(512, 512);
            pbx_Map.SizeMode = PictureBoxSizeMode.CenterImage;
            pbx_Map.TabIndex = 6;
            pbx_Map.TabStop = false;
            pbx_Map.Paint += pbx_Map_Paint;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(12, 418);
            label3.Name = "label3";
            label3.Size = new Size(35, 15);
            label3.TabIndex = 8;
            label3.Text = "Seed:";
            // 
            // num_Seed
            // 
            num_Seed.Location = new Point(63, 414);
            num_Seed.Maximum = new decimal(new int[] { 1000, 0, 0, 0 });
            num_Seed.Name = "num_Seed";
            num_Seed.Size = new Size(93, 23);
            num_Seed.TabIndex = 7;
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Location = new Point(12, 389);
            label4.Name = "label4";
            label4.Size = new Size(63, 15);
            label4.TabIndex = 10;
            label4.Text = "Threshold:";
            // 
            // num_Threshold
            // 
            num_Threshold.DecimalPlaces = 2;
            num_Threshold.Increment = new decimal(new int[] { 1, 0, 0, 65536 });
            num_Threshold.Location = new Point(81, 385);
            num_Threshold.Maximum = new decimal(new int[] { 1, 0, 0, 0 });
            num_Threshold.Minimum = new decimal(new int[] { 1, 0, 0, int.MinValue });
            num_Threshold.Name = "num_Threshold";
            num_Threshold.Size = new Size(75, 23);
            num_Threshold.TabIndex = 9;
            num_Threshold.Value = new decimal(new int[] { 5, 0, 0, -2147418112 });
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Location = new Point(12, 360);
            label5.Name = "label5";
            label5.Size = new Size(66, 15);
            label5.TabIndex = 12;
            label5.Text = "End points:";
            // 
            // num_EndPoints
            // 
            num_EndPoints.Location = new Point(81, 356);
            num_EndPoints.Maximum = new decimal(new int[] { 1000, 0, 0, 0 });
            num_EndPoints.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
            num_EndPoints.Name = "num_EndPoints";
            num_EndPoints.Size = new Size(75, 23);
            num_EndPoints.TabIndex = 11;
            num_EndPoints.Value = new decimal(new int[] { 1, 0, 0, 0 });
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(686, 536);
            Controls.Add(label5);
            Controls.Add(num_EndPoints);
            Controls.Add(label4);
            Controls.Add(num_Threshold);
            Controls.Add(label3);
            Controls.Add(num_Seed);
            Controls.Add(pbx_Map);
            Controls.Add(label2);
            Controls.Add(num_SizeY);
            Controls.Add(label1);
            Controls.Add(num_SizeX);
            Controls.Add(btn_Generate);
            Name = "Form1";
            Text = "Form1";
            ((System.ComponentModel.ISupportInitialize)num_SizeX).EndInit();
            ((System.ComponentModel.ISupportInitialize)num_SizeY).EndInit();
            ((System.ComponentModel.ISupportInitialize)pbx_Map).EndInit();
            ((System.ComponentModel.ISupportInitialize)num_Seed).EndInit();
            ((System.ComponentModel.ISupportInitialize)num_Threshold).EndInit();
            ((System.ComponentModel.ISupportInitialize)num_EndPoints).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion
        private Button btn_Generate;
        private NumericUpDown num_SizeX;
        private Label label1;
        private Label label2;
        private NumericUpDown num_SizeY;
        private PictureBox pbx_Map;
        private Label label3;
        private NumericUpDown num_Seed;
        private Label label4;
        private NumericUpDown num_Threshold;
        private Label label5;
        private NumericUpDown num_EndPoints;
    }
}
