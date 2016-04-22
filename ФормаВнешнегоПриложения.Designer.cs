namespace TestMenuPopup
{
    partial class ФормаВнешнегоПриложения
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ФормаВнешнегоПриложения));
            this.Наименование = new System.Windows.Forms.TextBox();
            this.ТекстНаименование = new System.Windows.Forms.Label();
            this.ТекстПутьКБазе = new System.Windows.Forms.Label();
            this.Путь = new System.Windows.Forms.TextBox();
            this.ПоказыватьВМенюЗапуска = new System.Windows.Forms.CheckBox();
            this.Группа = new System.Windows.Forms.ComboBox();
            this.ТекстГруппа = new System.Windows.Forms.Label();
            this.Статус = new System.Windows.Forms.StatusStrip();
            this.Подсказка = new System.Windows.Forms.ToolStripStatusLabel();
            this.ТекстОписание = new System.Windows.Forms.Label();
            this.Описание = new System.Windows.Forms.TextBox();
            this.ТекстСочетаниеКлавиш = new System.Windows.Forms.Label();
            this.ТекстСобственнаяПрограммаЗапуска = new System.Windows.Forms.Label();
            this.СобственнаяПрограммаЗапуска = new System.Windows.Forms.TextBox();
            this.ГруппаОсновныеПараметры = new System.Windows.Forms.GroupBox();
            this.ТекстПараметрыЗапуска = new System.Windows.Forms.Label();
            this.ПараметрыЗапуска = new System.Windows.Forms.TextBox();
            this.ВыбратьКаталогБД = new System.Windows.Forms.Button();
            this.ВыбратьСобственнуюПрограмму = new System.Windows.Forms.Button();
            this.КнопкаОтмена = new System.Windows.Forms.Button();
            this.КнопкаОК = new System.Windows.Forms.Button();
            this.СочетаниеКлавиш = new TestMenuPopup.MyHotKey();
            this.Статус.SuspendLayout();
            this.ГруппаОсновныеПараметры.SuspendLayout();
            this.SuspendLayout();
            // 
            // Наименование
            // 
            this.Наименование.Location = new System.Drawing.Point(95, 15);
            this.Наименование.Name = "Наименование";
            this.Наименование.Size = new System.Drawing.Size(363, 20);
            this.Наименование.TabIndex = 0;
            this.Наименование.MouseLeave += new System.EventHandler(this.Наименование_MouseLeave);
            this.Наименование.MouseHover += new System.EventHandler(this.Наименование_MouseHover);
            // 
            // ТекстНаименование
            // 
            this.ТекстНаименование.AutoSize = true;
            this.ТекстНаименование.Location = new System.Drawing.Point(3, 18);
            this.ТекстНаименование.Name = "ТекстНаименование";
            this.ТекстНаименование.Size = new System.Drawing.Size(86, 13);
            this.ТекстНаименование.TabIndex = 1;
            this.ТекстНаименование.Text = "Наименование:";
            // 
            // ТекстПутьКБазе
            // 
            this.ТекстПутьКБазе.AutoSize = true;
            this.ТекстПутьКБазе.Location = new System.Drawing.Point(3, 35);
            this.ТекстПутьКБазе.Name = "ТекстПутьКБазе";
            this.ТекстПутьКБазе.Size = new System.Drawing.Size(34, 13);
            this.ТекстПутьКБазе.TabIndex = 13;
            this.ТекстПутьКБазе.Text = "Путь:";
            // 
            // Путь
            // 
            this.Путь.Location = new System.Drawing.Point(6, 51);
            this.Путь.Name = "Путь";
            this.Путь.Size = new System.Drawing.Size(428, 20);
            this.Путь.TabIndex = 3;
            this.Путь.TextChanged += new System.EventHandler(this.Путь_TextChanged);
            this.Путь.MouseLeave += new System.EventHandler(this.Путь_MouseLeave);
            this.Путь.KeyDown += new System.Windows.Forms.KeyEventHandler(this.Путь_KeyDown);
            this.Путь.MouseHover += new System.EventHandler(this.Путь_MouseHover);
            // 
            // ПоказыватьВМенюЗапуска
            // 
            this.ПоказыватьВМенюЗапуска.AutoSize = true;
            this.ПоказыватьВМенюЗапуска.Location = new System.Drawing.Point(177, 247);
            this.ПоказыватьВМенюЗапуска.Name = "ПоказыватьВМенюЗапуска";
            this.ПоказыватьВМенюЗапуска.Size = new System.Drawing.Size(173, 17);
            this.ПоказыватьВМенюЗапуска.TabIndex = 16;
            this.ПоказыватьВМенюЗапуска.Text = "Показывать в меню запуска";
            this.ПоказыватьВМенюЗапуска.UseVisualStyleBackColor = true;
            this.ПоказыватьВМенюЗапуска.MouseLeave += new System.EventHandler(this.ПоказыватьВМенюЗапуска_MouseLeave);
            this.ПоказыватьВМенюЗапуска.MouseHover += new System.EventHandler(this.ПоказыватьВМенюЗапуска_MouseHover);
            // 
            // Группа
            // 
            this.Группа.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.Группа.DropDownHeight = 1;
            this.Группа.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.Группа.FormattingEnabled = true;
            this.Группа.IntegralHeight = false;
            this.Группа.ItemHeight = 16;
            this.Группа.Location = new System.Drawing.Point(49, 4);
            this.Группа.Name = "Группа";
            this.Группа.Size = new System.Drawing.Size(420, 22);
            this.Группа.TabIndex = 17;
            this.Группа.DrawItem += new System.Windows.Forms.DrawItemEventHandler(this.Группа_DrawItem);
            this.Группа.Click += new System.EventHandler(this.Группа_Click);
            // 
            // ТекстГруппа
            // 
            this.ТекстГруппа.AutoSize = true;
            this.ТекстГруппа.Location = new System.Drawing.Point(3, 8);
            this.ТекстГруппа.Name = "ТекстГруппа";
            this.ТекстГруппа.Size = new System.Drawing.Size(45, 13);
            this.ТекстГруппа.TabIndex = 19;
            this.ТекстГруппа.Text = "Группа:";
            // 
            // Статус
            // 
            this.Статус.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.Подсказка});
            this.Статус.Location = new System.Drawing.Point(0, 274);
            this.Статус.Name = "Статус";
            this.Статус.Size = new System.Drawing.Size(474, 22);
            this.Статус.TabIndex = 21;
            this.Статус.Text = "statusStrip1";
            // 
            // Подсказка
            // 
            this.Подсказка.Name = "Подсказка";
            this.Подсказка.Size = new System.Drawing.Size(0, 17);
            this.Подсказка.Paint += new System.Windows.Forms.PaintEventHandler(this.Подсказка_Paint);
            // 
            // ТекстОписание
            // 
            this.ТекстОписание.AutoSize = true;
            this.ТекстОписание.Location = new System.Drawing.Point(4, 144);
            this.ТекстОписание.Name = "ТекстОписание";
            this.ТекстОписание.Size = new System.Drawing.Size(60, 13);
            this.ТекстОписание.TabIndex = 25;
            this.ТекстОписание.Text = "Описание:";
            // 
            // Описание
            // 
            this.Описание.Location = new System.Drawing.Point(70, 146);
            this.Описание.Multiline = true;
            this.Описание.Name = "Описание";
            this.Описание.Size = new System.Drawing.Size(400, 50);
            this.Описание.TabIndex = 10;
            this.Описание.MouseLeave += new System.EventHandler(this.Описание_MouseLeave);
            this.Описание.MouseHover += new System.EventHandler(this.Описание_MouseHover);
            // 
            // ТекстСочетаниеКлавиш
            // 
            this.ТекстСочетаниеКлавиш.AutoSize = true;
            this.ТекстСочетаниеКлавиш.Location = new System.Drawing.Point(291, 218);
            this.ТекстСочетаниеКлавиш.Name = "ТекстСочетаниеКлавиш";
            this.ТекстСочетаниеКлавиш.Size = new System.Drawing.Size(63, 13);
            this.ТекстСочетаниеКлавиш.TabIndex = 31;
            this.ТекстСочетаниеКлавиш.Text = "Сочетание:";
            // 
            // ТекстСобственнаяПрограммаЗапуска
            // 
            this.ТекстСобственнаяПрограммаЗапуска.AutoSize = true;
            this.ТекстСобственнаяПрограммаЗапуска.Location = new System.Drawing.Point(4, 199);
            this.ТекстСобственнаяПрограммаЗапуска.Name = "ТекстСобственнаяПрограммаЗапуска";
            this.ТекстСобственнаяПрограммаЗапуска.Size = new System.Drawing.Size(180, 13);
            this.ТекстСобственнаяПрограммаЗапуска.TabIndex = 32;
            this.ТекстСобственнаяПрограммаЗапуска.Text = "Собственная программа запуска:";
            // 
            // СобственнаяПрограммаЗапуска
            // 
            this.СобственнаяПрограммаЗапуска.Location = new System.Drawing.Point(7, 215);
            this.СобственнаяПрограммаЗапуска.Name = "СобственнаяПрограммаЗапуска";
            this.СобственнаяПрограммаЗапуска.Size = new System.Drawing.Size(249, 20);
            this.СобственнаяПрограммаЗапуска.TabIndex = 11;
            this.СобственнаяПрограммаЗапуска.MouseLeave += new System.EventHandler(this.СобственнаяПрограммаЗапуска_MouseLeave);
            this.СобственнаяПрограммаЗапуска.KeyDown += new System.Windows.Forms.KeyEventHandler(this.СобственнаяПрограммаЗапуска_KeyDown);
            this.СобственнаяПрограммаЗапуска.MouseHover += new System.EventHandler(this.СобственнаяПрограммаЗапуска_MouseHover);
            // 
            // ГруппаОсновныеПараметры
            // 
            this.ГруппаОсновныеПараметры.Controls.Add(this.ТекстПараметрыЗапуска);
            this.ГруппаОсновныеПараметры.Controls.Add(this.ПараметрыЗапуска);
            this.ГруппаОсновныеПараметры.Controls.Add(this.ВыбратьКаталогБД);
            this.ГруппаОсновныеПараметры.Controls.Add(this.ТекстПутьКБазе);
            this.ГруппаОсновныеПараметры.Controls.Add(this.Путь);
            this.ГруппаОсновныеПараметры.Controls.Add(this.ТекстНаименование);
            this.ГруппаОсновныеПараметры.Controls.Add(this.Наименование);
            this.ГруппаОсновныеПараметры.Location = new System.Drawing.Point(6, 28);
            this.ГруппаОсновныеПараметры.Name = "ГруппаОсновныеПараметры";
            this.ГруппаОсновныеПараметры.Size = new System.Drawing.Size(463, 113);
            this.ГруппаОсновныеПараметры.TabIndex = 34;
            this.ГруппаОсновныеПараметры.TabStop = false;
            this.ГруппаОсновныеПараметры.Text = "Основные параметры";
            // 
            // ТекстПараметрыЗапуска
            // 
            this.ТекстПараметрыЗапуска.AutoSize = true;
            this.ТекстПараметрыЗапуска.Location = new System.Drawing.Point(3, 72);
            this.ТекстПараметрыЗапуска.Name = "ТекстПараметрыЗапуска";
            this.ТекстПараметрыЗапуска.Size = new System.Drawing.Size(113, 13);
            this.ТекстПараметрыЗапуска.TabIndex = 15;
            this.ТекстПараметрыЗапуска.Text = "Параметры запуска:";
            // 
            // ПараметрыЗапуска
            // 
            this.ПараметрыЗапуска.Location = new System.Drawing.Point(6, 88);
            this.ПараметрыЗапуска.Name = "ПараметрыЗапуска";
            this.ПараметрыЗапуска.Size = new System.Drawing.Size(452, 20);
            this.ПараметрыЗапуска.TabIndex = 14;
            // 
            // ВыбратьКаталогБД
            // 
            this.ВыбратьКаталогБД.Image = global::TestMenuPopup.Properties.Resources.Folder;
            this.ВыбратьКаталогБД.Location = new System.Drawing.Point(434, 50);
            this.ВыбратьКаталогБД.Name = "ВыбратьКаталогБД";
            this.ВыбратьКаталогБД.Size = new System.Drawing.Size(24, 22);
            this.ВыбратьКаталогБД.TabIndex = 4;
            this.ВыбратьКаталогБД.UseVisualStyleBackColor = true;
            this.ВыбратьКаталогБД.Click += new System.EventHandler(this.ВыбратьКаталогБД_Click);
            // 
            // ВыбратьСобственнуюПрограмму
            // 
            this.ВыбратьСобственнуюПрограмму.Image = global::TestMenuPopup.Properties.Resources.Folder;
            this.ВыбратьСобственнуюПрограмму.Location = new System.Drawing.Point(256, 214);
            this.ВыбратьСобственнуюПрограмму.Name = "ВыбратьСобственнуюПрограмму";
            this.ВыбратьСобственнуюПрограмму.Size = new System.Drawing.Size(24, 22);
            this.ВыбратьСобственнуюПрограмму.TabIndex = 12;
            this.ВыбратьСобственнуюПрограмму.UseVisualStyleBackColor = true;
            this.ВыбратьСобственнуюПрограмму.MouseLeave += new System.EventHandler(this.ВыбратьСобственнуюПрограмму_MouseLeave);
            this.ВыбратьСобственнуюПрограмму.Click += new System.EventHandler(this.ВыбратьСобственнуюПрограмму_Click);
            this.ВыбратьСобственнуюПрограмму.MouseHover += new System.EventHandler(this.ВыбратьСобственнуюПрограмму_MouseHover);
            // 
            // КнопкаОтмена
            // 
            this.КнопкаОтмена.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.КнопкаОтмена.Image = global::TestMenuPopup.Properties.Resources.Cancel;
            this.КнопкаОтмена.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.КнопкаОтмена.Location = new System.Drawing.Point(90, 239);
            this.КнопкаОтмена.Name = "КнопкаОтмена";
            this.КнопкаОтмена.Size = new System.Drawing.Size(81, 31);
            this.КнопкаОтмена.TabIndex = 15;
            this.КнопкаОтмена.Text = "Отмена";
            this.КнопкаОтмена.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.КнопкаОтмена.UseVisualStyleBackColor = true;
            this.КнопкаОтмена.Click += new System.EventHandler(this.button2_Click);
            // 
            // КнопкаОК
            // 
            this.КнопкаОК.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.КнопкаОК.Image = global::TestMenuPopup.Properties.Resources.OK;
            this.КнопкаОК.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.КнопкаОК.Location = new System.Drawing.Point(7, 239);
            this.КнопкаОК.Name = "КнопкаОК";
            this.КнопкаОК.Size = new System.Drawing.Size(81, 31);
            this.КнопкаОК.TabIndex = 14;
            this.КнопкаОК.Text = "    ОК";
            this.КнопкаОК.UseVisualStyleBackColor = true;
            this.КнопкаОК.Click += new System.EventHandler(this.КнопкаОК_Click);
            // 
            // СочетаниеКлавиш
            // 
            this.СочетаниеКлавиш.Location = new System.Drawing.Point(360, 214);
            this.СочетаниеКлавиш.Name = "СочетаниеКлавиш";
            this.СочетаниеКлавиш.Size = new System.Drawing.Size(110, 20);
            this.СочетаниеКлавиш.TabIndex = 13;
            // 
            // ФормаВнешнегоПриложения
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(474, 296);
            this.Controls.Add(this.ГруппаОсновныеПараметры);
            this.Controls.Add(this.ВыбратьСобственнуюПрограмму);
            this.Controls.Add(this.СобственнаяПрограммаЗапуска);
            this.Controls.Add(this.ТекстСобственнаяПрограммаЗапуска);
            this.Controls.Add(this.ТекстСочетаниеКлавиш);
            this.Controls.Add(this.СочетаниеКлавиш);
            this.Controls.Add(this.Описание);
            this.Controls.Add(this.ТекстОписание);
            this.Controls.Add(this.Статус);
            this.Controls.Add(this.ТекстГруппа);
            this.Controls.Add(this.Группа);
            this.Controls.Add(this.ПоказыватьВМенюЗапуска);
            this.Controls.Add(this.КнопкаОтмена);
            this.Controls.Add(this.КнопкаОК);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.KeyPreview = true;
            this.MaximizeBox = false;
            this.Name = "ФормаВнешнегоПриложения";
            this.ShowInTaskbar = false;
            this.Text = "SettingsDB";
            this.Shown += new System.EventHandler(this.ФормаБазыДанных_Shown);
            this.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.ФормаБазыДанных_KeyPress);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.ФормаБазыДанных_KeyDown);
            this.Статус.ResumeLayout(false);
            this.Статус.PerformLayout();
            this.ГруппаОсновныеПараметры.ResumeLayout(false);
            this.ГруппаОсновныеПараметры.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox Наименование;
        private System.Windows.Forms.Label ТекстНаименование;
        private System.Windows.Forms.Button КнопкаОК;
        private System.Windows.Forms.Button КнопкаОтмена;
        private System.Windows.Forms.Label ТекстПутьКБазе;
        private System.Windows.Forms.TextBox Путь;
        private System.Windows.Forms.CheckBox ПоказыватьВМенюЗапуска;
        private System.Windows.Forms.ComboBox Группа;
        private System.Windows.Forms.Label ТекстГруппа;
        private System.Windows.Forms.Button ВыбратьКаталогБД;
        private System.Windows.Forms.StatusStrip Статус;
        private System.Windows.Forms.ToolStripStatusLabel Подсказка;
        private System.Windows.Forms.Label ТекстОписание;
        private System.Windows.Forms.TextBox Описание;
        private MyHotKey СочетаниеКлавиш;
        private System.Windows.Forms.Label ТекстСочетаниеКлавиш;
        private System.Windows.Forms.Label ТекстСобственнаяПрограммаЗапуска;
        private System.Windows.Forms.TextBox СобственнаяПрограммаЗапуска;
        private System.Windows.Forms.Button ВыбратьСобственнуюПрограмму;
        private System.Windows.Forms.GroupBox ГруппаОсновныеПараметры;
        private System.Windows.Forms.Label ТекстПараметрыЗапуска;
        private System.Windows.Forms.TextBox ПараметрыЗапуска;
    }
}