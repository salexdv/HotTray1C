namespace TestMenuPopup
{
    partial class ФормаОбновления
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ФормаОбновления));
            this.ТекстовойПоле = new System.Windows.Forms.RichTextBox();
            this.ТекстТекущаяВерсия = new System.Windows.Forms.Label();
            this.ТекстВесияНаСервере = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.КнопкаЗакрыть = new System.Windows.Forms.Button();
            this.Скачать = new System.Windows.Forms.Button();
            this.СайтПрограммы = new System.Windows.Forms.LinkLabel();
            this.SuspendLayout();
            // 
            // ТекстовойПоле
            // 
            this.ТекстовойПоле.Location = new System.Drawing.Point(5, 55);
            this.ТекстовойПоле.Name = "ТекстовойПоле";
            this.ТекстовойПоле.ReadOnly = true;
            this.ТекстовойПоле.Size = new System.Drawing.Size(362, 224);
            this.ТекстовойПоле.TabIndex = 0;
            this.ТекстовойПоле.Text = "";
            // 
            // ТекстТекущаяВерсия
            // 
            this.ТекстТекущаяВерсия.AutoSize = true;
            this.ТекстТекущаяВерсия.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.ТекстТекущаяВерсия.ForeColor = System.Drawing.Color.Green;
            this.ТекстТекущаяВерсия.Location = new System.Drawing.Point(3, 4);
            this.ТекстТекущаяВерсия.Name = "ТекстТекущаяВерсия";
            this.ТекстТекущаяВерсия.Size = new System.Drawing.Size(0, 13);
            this.ТекстТекущаяВерсия.TabIndex = 17;
            // 
            // ТекстВесияНаСервере
            // 
            this.ТекстВесияНаСервере.AutoSize = true;
            this.ТекстВесияНаСервере.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.ТекстВесияНаСервере.ForeColor = System.Drawing.Color.Green;
            this.ТекстВесияНаСервере.Location = new System.Drawing.Point(3, 20);
            this.ТекстВесияНаСервере.Name = "ТекстВесияНаСервере";
            this.ТекстВесияНаСервере.Size = new System.Drawing.Size(0, 13);
            this.ТекстВесияНаСервере.TabIndex = 18;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Underline, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label1.Location = new System.Drawing.Point(3, 39);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(119, 13);
            this.label1.TabIndex = 19;
            this.label1.Text = "Описание изменений:";
            // 
            // КнопкаЗакрыть
            // 
            this.КнопкаЗакрыть.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.КнопкаЗакрыть.Image = global::TestMenuPopup.Properties.Resources.Cancel;
            this.КнопкаЗакрыть.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.КнопкаЗакрыть.Location = new System.Drawing.Point(287, 283);
            this.КнопкаЗакрыть.Name = "КнопкаЗакрыть";
            this.КнопкаЗакрыть.Size = new System.Drawing.Size(81, 31);
            this.КнопкаЗакрыть.TabIndex = 16;
            this.КнопкаЗакрыть.Text = "Закрыть";
            this.КнопкаЗакрыть.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.КнопкаЗакрыть.UseVisualStyleBackColor = true;
            this.КнопкаЗакрыть.Click += new System.EventHandler(this.КнопкаЗакрыть_Click);
            // 
            // Скачать
            // 
            this.Скачать.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.Скачать.Image = ((System.Drawing.Image)(resources.GetObject("Скачать.Image")));
            this.Скачать.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.Скачать.Location = new System.Drawing.Point(200, 283);
            this.Скачать.Name = "Скачать";
            this.Скачать.Size = new System.Drawing.Size(81, 31);
            this.Скачать.TabIndex = 20;
            this.Скачать.Text = "Скачать";
            this.Скачать.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.Скачать.UseVisualStyleBackColor = true;
            this.Скачать.Click += new System.EventHandler(this.Скачать_Click);
            // 
            // СайтПрограммы
            // 
            this.СайтПрограммы.AutoSize = true;
            this.СайтПрограммы.Location = new System.Drawing.Point(3, 292);
            this.СайтПрограммы.Name = "СайтПрограммы";
            this.СайтПрограммы.Size = new System.Drawing.Size(93, 13);
            this.СайтПрограммы.TabIndex = 26;
            this.СайтПрограммы.TabStop = true;
            this.СайтПрограммы.Text = "Сайт программы";            
            this.СайтПрограммы.MouseClick += new System.Windows.Forms.MouseEventHandler(this.СайтПрограммы_MouseClick);
            // 
            // ФормаОбновления
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(371, 318);
            this.Controls.Add(this.СайтПрограммы);
            this.Controls.Add(this.Скачать);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.ТекстВесияНаСервере);
            this.Controls.Add(this.ТекстТекущаяВерсия);
            this.Controls.Add(this.КнопкаЗакрыть);
            this.Controls.Add(this.ТекстовойПоле);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.KeyPreview = true;
            this.Name = "ФормаОбновления";
            this.ShowInTaskbar = false;
            this.Text = "Проверка обновлений";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.ФормаОбновления_FormClosed);
            this.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.ФормаОбновления_KeyPress);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.RichTextBox ТекстовойПоле;
        private System.Windows.Forms.Button КнопкаЗакрыть;
        private System.Windows.Forms.Label ТекстТекущаяВерсия;
        private System.Windows.Forms.Label ТекстВесияНаСервере;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button Скачать;
        private System.Windows.Forms.LinkLabel СайтПрограммы;
    }
}