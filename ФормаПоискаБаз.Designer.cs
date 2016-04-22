namespace TestMenuPopup
{
    partial class ФормаПоискаБаз
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ФормаПоискаБаз));
            this.НачальныйКаталог = new System.Windows.Forms.TextBox();
            this.ТекстНачальныйКаталог = new System.Windows.Forms.Label();
            this.Статус = new System.Windows.Forms.StatusStrip();
            this.Подсказка = new System.Windows.Forms.ToolStripStatusLabel();
            this.ТекстГруппаБазДанных = new System.Windows.Forms.Label();
            this.ГруппаБазДанных = new System.Windows.Forms.TextBox();
            this.ТипПлатформы = new System.Windows.Forms.ComboBox();
            this.ТекстТипПлатформы = new System.Windows.Forms.Label();
            this.КнопкаОтмена = new System.Windows.Forms.Button();
            this.КнопкаОК = new System.Windows.Forms.Button();
            this.ВыбратьГруппу = new System.Windows.Forms.Button();
            this.ВыбратьНачальныйКаталог = new System.Windows.Forms.Button();
            this.Статус.SuspendLayout();
            this.SuspendLayout();
            // 
            // НачальныйКаталог
            // 
            this.НачальныйКаталог.Location = new System.Drawing.Point(7, 20);
            this.НачальныйКаталог.Name = "НачальныйКаталог";
            this.НачальныйКаталог.Size = new System.Drawing.Size(278, 20);
            this.НачальныйКаталог.TabIndex = 0;
            this.НачальныйКаталог.MouseLeave += new System.EventHandler(this.НачальныйКаталог_MouseLeave);
            this.НачальныйКаталог.MouseHover += new System.EventHandler(this.НачальныйКаталог_MouseHover);
            // 
            // ТекстНачальныйКаталог
            // 
            this.ТекстНачальныйКаталог.AutoSize = true;
            this.ТекстНачальныйКаталог.Location = new System.Drawing.Point(4, 4);
            this.ТекстНачальныйКаталог.Name = "ТекстНачальныйКаталог";
            this.ТекстНачальныйКаталог.Size = new System.Drawing.Size(149, 13);
            this.ТекстНачальныйКаталог.TabIndex = 1;
            this.ТекстНачальныйКаталог.Text = "Начальный каталог поиска:";
            // 
            // Статус
            // 
            this.Статус.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.Подсказка});
            this.Статус.Location = new System.Drawing.Point(0, 160);
            this.Статус.Name = "Статус";
            this.Статус.Size = new System.Drawing.Size(315, 22);
            this.Статус.TabIndex = 2;
            this.Статус.Text = "statusStrip1";
            // 
            // Подсказка
            // 
            this.Подсказка.Name = "Подсказка";
            this.Подсказка.Size = new System.Drawing.Size(0, 17);
            this.Подсказка.Paint += new System.Windows.Forms.PaintEventHandler(this.Подсказка_Paint);
            // 
            // ТекстГруппаБазДанных
            // 
            this.ТекстГруппаБазДанных.AutoSize = true;
            this.ТекстГруппаБазДанных.Location = new System.Drawing.Point(4, 43);
            this.ТекстГруппаБазДанных.Name = "ТекстГруппаБазДанных";
            this.ТекстГруппаБазДанных.Size = new System.Drawing.Size(106, 13);
            this.ТекстГруппаБазДанных.TabIndex = 5;
            this.ТекстГруппаБазДанных.Text = "Группа баз данных:";
            // 
            // ГруппаБазДанных
            // 
            this.ГруппаБазДанных.Location = new System.Drawing.Point(7, 59);
            this.ГруппаБазДанных.Name = "ГруппаБазДанных";
            this.ГруппаБазДанных.ReadOnly = true;
            this.ГруппаБазДанных.Size = new System.Drawing.Size(278, 20);
            this.ГруппаБазДанных.TabIndex = 2;
            this.ГруппаБазДанных.MouseLeave += new System.EventHandler(this.ГруппаБазДанных_MouseLeave);
            this.ГруппаБазДанных.MouseHover += new System.EventHandler(this.ГруппаБазДанных_MouseHover);
            // 
            // ТипПлатформы
            // 
            this.ТипПлатформы.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.ТипПлатформы.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.ТипПлатформы.FormattingEnabled = true;
            this.ТипПлатформы.Items.AddRange(new object[] {
            "1С Предприятие 8.0",
            "1С Предприятие 8.1",
            "1С Предприятие 8.2"});
            this.ТипПлатформы.Location = new System.Drawing.Point(7, 99);
            this.ТипПлатформы.Name = "ТипПлатформы";
            this.ТипПлатформы.Size = new System.Drawing.Size(181, 21);
            this.ТипПлатформы.TabIndex = 4;
            this.ТипПлатформы.MouseHover += new System.EventHandler(this.ТипПлатформы_MouseHover);
            this.ТипПлатформы.DrawItem += new System.Windows.Forms.DrawItemEventHandler(this.ТипПлатформы_DrawItem);
            this.ТипПлатформы.MouseLeave += new System.EventHandler(this.ТипПлатформы_MouseLeave);
            // 
            // ТекстТипПлатформы
            // 
            this.ТекстТипПлатформы.AutoSize = true;
            this.ТекстТипПлатформы.Location = new System.Drawing.Point(4, 83);
            this.ТекстТипПлатформы.Name = "ТекстТипПлатформы";
            this.ТекстТипПлатформы.Size = new System.Drawing.Size(184, 13);
            this.ТекстТипПлатформы.TabIndex = 10;
            this.ТекстТипПлатформы.Text = "Тип платформы для новых баз 8.х:";
            // 
            // КнопкаОтмена
            // 
            this.КнопкаОтмена.Image = global::TestMenuPopup.Properties.Resources.Cancel;
            this.КнопкаОтмена.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.КнопкаОтмена.Location = new System.Drawing.Point(86, 124);
            this.КнопкаОтмена.Name = "КнопкаОтмена";
            this.КнопкаОтмена.Size = new System.Drawing.Size(77, 31);
            this.КнопкаОтмена.TabIndex = 6;
            this.КнопкаОтмена.Text = "Отмена";
            this.КнопкаОтмена.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.КнопкаОтмена.UseVisualStyleBackColor = true;
            this.КнопкаОтмена.MouseLeave += new System.EventHandler(this.КнопкаОтмена_MouseLeave);
            this.КнопкаОтмена.Click += new System.EventHandler(this.КнопкаОтмена_Click);
            this.КнопкаОтмена.MouseHover += new System.EventHandler(this.КнопкаОтмена_MouseHover);
            // 
            // КнопкаОК
            // 
            this.КнопкаОК.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.КнопкаОК.Image = global::TestMenuPopup.Properties.Resources.OK;
            this.КнопкаОК.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.КнопкаОК.Location = new System.Drawing.Point(5, 124);
            this.КнопкаОК.Name = "КнопкаОК";
            this.КнопкаОК.Size = new System.Drawing.Size(77, 31);
            this.КнопкаОК.TabIndex = 5;
            this.КнопкаОК.Text = "    ОК";
            this.КнопкаОК.UseVisualStyleBackColor = true;
            this.КнопкаОК.MouseLeave += new System.EventHandler(this.КнопкаОК_MouseLeave);
            this.КнопкаОК.Click += new System.EventHandler(this.КнопкаОК_Click);            
            this.КнопкаОК.MouseHover += new System.EventHandler(this.КнопкаОК_MouseHover);            
            // 
            // ВыбратьГруппу
            // 
            this.ВыбратьГруппу.Image = global::TestMenuPopup.Properties.Resources.Folder;
            this.ВыбратьГруппу.Location = new System.Drawing.Point(287, 59);
            this.ВыбратьГруппу.Name = "ВыбратьГруппу";
            this.ВыбратьГруппу.Size = new System.Drawing.Size(25, 21);
            this.ВыбратьГруппу.TabIndex = 3;
            this.ВыбратьГруппу.UseVisualStyleBackColor = true;
            this.ВыбратьГруппу.MouseLeave += new System.EventHandler(this.ВыбратьГруппу_MouseLeave);
            this.ВыбратьГруппу.Click += new System.EventHandler(this.ВыбратьГруппу_Click);
            this.ВыбратьГруппу.MouseHover += new System.EventHandler(this.ВыбратьГруппу_MouseHover);
            // 
            // ВыбратьНачальныйКаталог
            // 
            this.ВыбратьНачальныйКаталог.Image = global::TestMenuPopup.Properties.Resources.Folder;
            this.ВыбратьНачальныйКаталог.Location = new System.Drawing.Point(287, 20);
            this.ВыбратьНачальныйКаталог.Name = "ВыбратьНачальныйКаталог";
            this.ВыбратьНачальныйКаталог.Size = new System.Drawing.Size(25, 21);
            this.ВыбратьНачальныйКаталог.TabIndex = 1;
            this.ВыбратьНачальныйКаталог.UseVisualStyleBackColor = true;
            this.ВыбратьНачальныйКаталог.MouseLeave += new System.EventHandler(this.ВыбратьНачальныйКаталог_MouseLeave);
            this.ВыбратьНачальныйКаталог.Click += new System.EventHandler(this.ВыбратьНачальныйКаталог_Click);
            this.ВыбратьНачальныйКаталог.MouseHover += new System.EventHandler(this.ВыбратьНачальныйКаталог_MouseHover);
            // 
            // ФормаПоискаБаз
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(315, 182);
            this.Controls.Add(this.ТекстТипПлатформы);
            this.Controls.Add(this.ТипПлатформы);
            this.Controls.Add(this.КнопкаОтмена);
            this.Controls.Add(this.КнопкаОК);
            this.Controls.Add(this.ВыбратьГруппу);
            this.Controls.Add(this.ТекстГруппаБазДанных);
            this.Controls.Add(this.ГруппаБазДанных);
            this.Controls.Add(this.ВыбратьНачальныйКаталог);
            this.Controls.Add(this.Статус);
            this.Controls.Add(this.ТекстНачальныйКаталог);
            this.Controls.Add(this.НачальныйКаталог);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.KeyPreview = true;
            this.MaximizeBox = false;
            this.Name = "ФормаПоискаБаз";
            this.ShowInTaskbar = false;
            this.Text = "Поиск и добавление баз данных";
            this.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.ФормаПоискаБаз_KeyPress);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.ФормаПоискаБаз_KeyDown);
            this.Статус.ResumeLayout(false);
            this.Статус.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox НачальныйКаталог;
        private System.Windows.Forms.Label ТекстНачальныйКаталог;
        private System.Windows.Forms.StatusStrip Статус;
        private System.Windows.Forms.ToolStripStatusLabel Подсказка;
        private System.Windows.Forms.Button ВыбратьНачальныйКаталог;
        private System.Windows.Forms.Button ВыбратьГруппу;
        private System.Windows.Forms.Label ТекстГруппаБазДанных;
        private System.Windows.Forms.TextBox ГруппаБазДанных;
        private System.Windows.Forms.Button КнопкаОК;
        private System.Windows.Forms.Button КнопкаОтмена;
        private System.Windows.Forms.ComboBox ТипПлатформы;
        private System.Windows.Forms.Label ТекстТипПлатформы;
    }
}