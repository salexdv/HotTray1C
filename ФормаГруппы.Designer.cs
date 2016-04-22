namespace TestMenuPopup
{
    partial class ФормаГруппы
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ФормаГруппы));
            this.Наименование = new System.Windows.Forms.TextBox();
            this.ТекстНаименование = new System.Windows.Forms.Label();
            this.КнопкаОК = new System.Windows.Forms.Button();
            this.ТекстОписание = new System.Windows.Forms.Label();
            this.Описание = new System.Windows.Forms.TextBox();
            this.КнопкаОтмена = new System.Windows.Forms.Button();
            this.ТекстСочетаниеКлавиш = new System.Windows.Forms.Label();
            this.Статус = new System.Windows.Forms.StatusStrip();
            this.Подсказка = new System.Windows.Forms.ToolStripStatusLabel();
            this.СочетаниеКлавиш = new TestMenuPopup.MyHotKey();
            this.Статус.SuspendLayout();
            this.SuspendLayout();
            // 
            // Наименование
            // 
            this.Наименование.Location = new System.Drawing.Point(96, 3);
            this.Наименование.Name = "Наименование";
            this.Наименование.Size = new System.Drawing.Size(244, 20);
            this.Наименование.TabIndex = 0;
            this.Наименование.MouseLeave += new System.EventHandler(this.Наименование_MouseLeave);
            this.Наименование.MouseHover += new System.EventHandler(this.Наименование_MouseHover);
            // 
            // ТекстНаименование
            // 
            this.ТекстНаименование.AutoSize = true;
            this.ТекстНаименование.Location = new System.Drawing.Point(3, 6);
            this.ТекстНаименование.Name = "ТекстНаименование";
            this.ТекстНаименование.Size = new System.Drawing.Size(86, 13);
            this.ТекстНаименование.TabIndex = 1;
            this.ТекстНаименование.Text = "Наименование:";
            // 
            // КнопкаОК
            // 
            this.КнопкаОК.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.КнопкаОК.Image = global::TestMenuPopup.Properties.Resources.OK;
            this.КнопкаОК.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.КнопкаОК.Location = new System.Drawing.Point(3, 144);
            this.КнопкаОК.Name = "КнопкаОК";
            this.КнопкаОК.Size = new System.Drawing.Size(79, 31);
            this.КнопкаОК.TabIndex = 3;
            this.КнопкаОК.Text = "    ОК";
            this.КнопкаОК.UseVisualStyleBackColor = true;
            this.КнопкаОК.MouseLeave += new System.EventHandler(this.КнопкаОК_MouseLeave);
            this.КнопкаОК.Click += new System.EventHandler(this.button1_Click);
            this.КнопкаОК.MouseHover += new System.EventHandler(this.КнопкаОК_MouseHover);
            // 
            // ТекстОписание
            // 
            this.ТекстОписание.AutoSize = true;
            this.ТекстОписание.Location = new System.Drawing.Point(3, 29);
            this.ТекстОписание.Name = "ТекстОписание";
            this.ТекстОписание.Size = new System.Drawing.Size(60, 13);
            this.ТекстОписание.TabIndex = 5;
            this.ТекстОписание.Text = "Описание:";
            // 
            // Описание
            // 
            this.Описание.Location = new System.Drawing.Point(96, 29);
            this.Описание.Multiline = true;
            this.Описание.Name = "Описание";
            this.Описание.Size = new System.Drawing.Size(244, 88);
            this.Описание.TabIndex = 1;
            this.Описание.MouseLeave += new System.EventHandler(this.Описание_MouseLeave);
            this.Описание.MouseHover += new System.EventHandler(this.Описание_MouseHover);
            // 
            // КнопкаОтмена
            // 
            this.КнопкаОтмена.Image = global::TestMenuPopup.Properties.Resources.Cancel;
            this.КнопкаОтмена.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.КнопкаОтмена.Location = new System.Drawing.Point(87, 144);
            this.КнопкаОтмена.Name = "КнопкаОтмена";
            this.КнопкаОтмена.Size = new System.Drawing.Size(79, 31);
            this.КнопкаОтмена.TabIndex = 4;
            this.КнопкаОтмена.Text = "Отмена";
            this.КнопкаОтмена.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.КнопкаОтмена.UseVisualStyleBackColor = true;
            this.КнопкаОтмена.MouseLeave += new System.EventHandler(this.КнопкаОтмена_MouseLeave);
            this.КнопкаОтмена.Click += new System.EventHandler(this.button2_Click);
            this.КнопкаОтмена.MouseHover += new System.EventHandler(this.КнопкаОтмена_MouseHover);
            // 
            // ТекстСочетаниеКлавиш
            // 
            this.ТекстСочетаниеКлавиш.AutoSize = true;
            this.ТекстСочетаниеКлавиш.Location = new System.Drawing.Point(136, 125);
            this.ТекстСочетаниеКлавиш.Name = "ТекстСочетаниеКлавиш";
            this.ТекстСочетаниеКлавиш.Size = new System.Drawing.Size(104, 13);
            this.ТекстСочетаниеКлавиш.TabIndex = 8;
            this.ТекстСочетаниеКлавиш.Text = "Сочетание клавиш:";
            // 
            // Статус
            // 
            this.Статус.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.Подсказка});
            this.Статус.Location = new System.Drawing.Point(0, 181);
            this.Статус.Name = "Статус";
            this.Статус.Size = new System.Drawing.Size(347, 22);
            this.Статус.TabIndex = 9;
            this.Статус.Text = "statusStrip1";
            // 
            // Подсказка
            // 
            this.Подсказка.Name = "Подсказка";
            this.Подсказка.Size = new System.Drawing.Size(0, 17);
            this.Подсказка.Paint += new System.Windows.Forms.PaintEventHandler(this.Подсказка_Paint);
            // 
            // СочетаниеКлавиш
            // 
            this.СочетаниеКлавиш.Location = new System.Drawing.Point(243, 122);
            this.СочетаниеКлавиш.Name = "СочетаниеКлавиш";
            this.СочетаниеКлавиш.Size = new System.Drawing.Size(98, 20);
            this.СочетаниеКлавиш.TabIndex = 2;
            this.СочетаниеКлавиш.MouseLeave += new System.EventHandler(this.СочетаниеКлавиш_MouseLeave);
            this.СочетаниеКлавиш.MouseHover += new System.EventHandler(this.СочетаниеКлавиш_MouseHover);
            // 
            // ФормаГруппы
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(347, 203);
            this.Controls.Add(this.Статус);
            this.Controls.Add(this.ТекстСочетаниеКлавиш);
            this.Controls.Add(this.СочетаниеКлавиш);
            this.Controls.Add(this.Описание);
            this.Controls.Add(this.ТекстОписание);
            this.Controls.Add(this.КнопкаОтмена);
            this.Controls.Add(this.КнопкаОК);
            this.Controls.Add(this.ТекстНаименование);
            this.Controls.Add(this.Наименование);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.KeyPreview = true;
            this.Name = "ФормаГруппы";
            this.ShowInTaskbar = false;
            this.Text = "SettingsGroup";
            this.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.ФормаГруппы_KeyPress);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.ФормаГруппы_KeyDown);
            this.Статус.ResumeLayout(false);
            this.Статус.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox Наименование;
        private System.Windows.Forms.Label ТекстНаименование;
        private System.Windows.Forms.Button КнопкаОК;
        private System.Windows.Forms.Button КнопкаОтмена;
        private System.Windows.Forms.Label ТекстОписание;
        private System.Windows.Forms.TextBox Описание;
        private MyHotKey СочетаниеКлавиш;
        private System.Windows.Forms.Label ТекстСочетаниеКлавиш;
        private System.Windows.Forms.StatusStrip Статус;
        private System.Windows.Forms.ToolStripStatusLabel Подсказка;
    }
}