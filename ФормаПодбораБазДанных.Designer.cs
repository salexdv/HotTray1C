namespace TestMenuPopup
{
    partial class ФормаПодбораБазДанных
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ФормаПодбораБазДанных));
            this.ПанельИнструментов = new System.Windows.Forms.ToolStrip();
            this.ТекстПлатформа = new System.Windows.Forms.ToolStripLabel();
            this.ВыборТипаПлатформы = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.КопироватьВсе = new System.Windows.Forms.ToolStripButton();
            this.УстановитьГруппу = new System.Windows.Forms.ToolStripButton();
            this.ОчиститьСписок = new System.Windows.Forms.ToolStripButton();
            this.СписокДобавляемыхБаз = new System.Windows.Forms.ListView();
            this.ИмяБазы = new System.Windows.Forms.ColumnHeader();
            this.Путь = new System.Windows.Forms.ColumnHeader();
            this.Группа = new System.Windows.Forms.ColumnHeader();
            this.КонтекстноеМеню = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.редактироватьПараметрыToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.установитьГруппуToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.удалитьToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.КоллекцияИконокПодбора = new System.Windows.Forms.ImageList(this.components);
            this.СписокЗарегистрированныхБаз = new System.Windows.Forms.ListView();
            this.ИмяБазыОригинал = new System.Windows.Forms.ColumnHeader();
            this.ПутьБазыОригинал = new System.Windows.Forms.ColumnHeader();
            this.МенюВыбораПлатформы = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.сПредприятие77ToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.сПредприятие80ToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.сПредприятие81ToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.сПредприятие82ToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.ПанельПодбора = new System.Windows.Forms.Panel();
            this.splitter1 = new System.Windows.Forms.Splitter();
            this.Статус = new System.Windows.Forms.StatusStrip();
            this.Подсказка = new System.Windows.Forms.ToolStripStatusLabel();
            this.КнопкаОтмена = new System.Windows.Forms.Button();
            this.КнопкаОК = new System.Windows.Forms.Button();
            this.ПанельИнструментов.SuspendLayout();
            this.КонтекстноеМеню.SuspendLayout();
            this.МенюВыбораПлатформы.SuspendLayout();
            this.ПанельПодбора.SuspendLayout();
            this.Статус.SuspendLayout();
            this.SuspendLayout();
            // 
            // ПанельИнструментов
            // 
            this.ПанельИнструментов.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.ПанельИнструментов.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.ТекстПлатформа,
            this.ВыборТипаПлатформы,
            this.toolStripSeparator1,
            this.КопироватьВсе,
            this.УстановитьГруппу,
            this.ОчиститьСписок});
            this.ПанельИнструментов.Location = new System.Drawing.Point(0, 0);
            this.ПанельИнструментов.Name = "ПанельИнструментов";
            this.ПанельИнструментов.Size = new System.Drawing.Size(724, 25);
            this.ПанельИнструментов.TabIndex = 0;
            this.ПанельИнструментов.Text = "toolStrip1";
            // 
            // ТекстПлатформа
            // 
            this.ТекстПлатформа.Name = "ТекстПлатформа";
            this.ТекстПлатформа.Size = new System.Drawing.Size(71, 22);
            this.ТекстПлатформа.Text = "Платформа: ";
            // 
            // ВыборТипаПлатформы
            // 
            this.ВыборТипаПлатформы.AutoToolTip = false;
            this.ВыборТипаПлатформы.Image = ((System.Drawing.Image)(resources.GetObject("ВыборТипаПлатформы.Image")));
            this.ВыборТипаПлатформы.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.ВыборТипаПлатформы.Name = "ВыборТипаПлатформы";
            this.ВыборТипаПлатформы.Size = new System.Drawing.Size(130, 22);
            this.ВыборТипаПлатформы.Text = "1С Предприятие Х.Х";
            this.ВыборТипаПлатформы.MouseHover += new System.EventHandler(this.ВыборТипаПлатформы_MouseHover);
            this.ВыборТипаПлатформы.MouseLeave += new System.EventHandler(this.ВыборТипаПлатформы_MouseLeave);
            this.ВыборТипаПлатформы.Click += new System.EventHandler(this.ВыборТипаПлатформы_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 25);
            // 
            // КопироватьВсе
            // 
            this.КопироватьВсе.AutoToolTip = false;
            this.КопироватьВсе.Image = ((System.Drawing.Image)(resources.GetObject("КопироватьВсе.Image")));
            this.КопироватьВсе.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.КопироватьВсе.Name = "КопироватьВсе";
            this.КопироватьВсе.Size = new System.Drawing.Size(108, 22);
            this.КопироватьВсе.Text = "Копировать все";
            this.КопироватьВсе.MouseHover += new System.EventHandler(this.КопироватьВсе_MouseHover);
            this.КопироватьВсе.MouseLeave += new System.EventHandler(this.КопироватьВсе_MouseLeave);
            this.КопироватьВсе.Click += new System.EventHandler(this.КопироватьВсе_Click);
            // 
            // УстановитьГруппу
            // 
            this.УстановитьГруппу.AutoToolTip = false;
            this.УстановитьГруппу.Image = global::TestMenuPopup.Properties.Resources.Folder;
            this.УстановитьГруппу.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.УстановитьГруппу.Name = "УстановитьГруппу";
            this.УстановитьГруппу.Size = new System.Drawing.Size(125, 22);
            this.УстановитьГруппу.Text = "Установить группу";
            this.УстановитьГруппу.MouseHover += new System.EventHandler(this.УстановитьГруппу_MouseHover);
            this.УстановитьГруппу.MouseLeave += new System.EventHandler(this.УстановитьГруппу_MouseLeave);
            this.УстановитьГруппу.Click += new System.EventHandler(this.УстановитьГруппу_Click);
            // 
            // ОчиститьСписок
            // 
            this.ОчиститьСписок.AutoToolTip = false;
            this.ОчиститьСписок.Image = ((System.Drawing.Image)(resources.GetObject("ОчиститьСписок.Image")));
            this.ОчиститьСписок.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.ОчиститьСписок.Name = "ОчиститьСписок";
            this.ОчиститьСписок.Size = new System.Drawing.Size(76, 22);
            this.ОчиститьСписок.Text = "Очистить";
            this.ОчиститьСписок.MouseHover += new System.EventHandler(this.ОчиститьСписок_MouseHover);
            this.ОчиститьСписок.MouseLeave += new System.EventHandler(this.ОчиститьСписок_MouseLeave);
            this.ОчиститьСписок.Click += new System.EventHandler(this.ОчиститьСписок_Click);
            // 
            // СписокДобавляемыхБаз
            // 
            this.СписокДобавляемыхБаз.AllowDrop = true;
            this.СписокДобавляемыхБаз.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.ИмяБазы,
            this.Путь,
            this.Группа});
            this.СписокДобавляемыхБаз.ContextMenuStrip = this.КонтекстноеМеню;
            this.СписокДобавляемыхБаз.Dock = System.Windows.Forms.DockStyle.Fill;
            this.СписокДобавляемыхБаз.FullRowSelect = true;
            this.СписокДобавляемыхБаз.GridLines = true;
            this.СписокДобавляемыхБаз.HideSelection = false;
            this.СписокДобавляемыхБаз.Location = new System.Drawing.Point(0, 227);
            this.СписокДобавляемыхБаз.Name = "СписокДобавляемыхБаз";
            this.СписокДобавляемыхБаз.ShowItemToolTips = true;
            this.СписокДобавляемыхБаз.Size = new System.Drawing.Size(721, 231);
            this.СписокДобавляемыхБаз.SmallImageList = this.КоллекцияИконокПодбора;
            this.СписокДобавляемыхБаз.TabIndex = 1;
            this.СписокДобавляемыхБаз.UseCompatibleStateImageBehavior = false;
            this.СписокДобавляемыхБаз.View = System.Windows.Forms.View.Details;
            this.СписокДобавляемыхБаз.DoubleClick += new System.EventHandler(this.СписокДобавляемыхБаз_DoubleClick);
            this.СписокДобавляемыхБаз.DragDrop += new System.Windows.Forms.DragEventHandler(this.СписокДобавляемыхБаз_DragDrop);
            this.СписокДобавляемыхБаз.DragEnter += new System.Windows.Forms.DragEventHandler(this.СписокДобавляемыхБаз_DragEnter);
            this.СписокДобавляемыхБаз.KeyDown += new System.Windows.Forms.KeyEventHandler(this.СписокДобавляемыхБаз_KeyDown);
            this.СписокДобавляемыхБаз.MouseHover += new System.EventHandler(this.СписокДобавляемыхБаз_MouseHover);
            this.СписокДобавляемыхБаз.MouseLeave += new System.EventHandler(this.СписокДобавляемыхБаз_MouseLeave);
            // 
            // ИмяБазы
            // 
            this.ИмяБазы.Text = "Имя базы";
            this.ИмяБазы.Width = 244;
            // 
            // Путь
            // 
            this.Путь.Text = "Путь";
            this.Путь.Width = 233;
            // 
            // Группа
            // 
            this.Группа.Text = "Группа";
            this.Группа.Width = 223;
            // 
            // КонтекстноеМеню
            // 
            this.КонтекстноеМеню.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.редактироватьПараметрыToolStripMenuItem,
            this.установитьГруппуToolStripMenuItem,
            this.удалитьToolStripMenuItem});
            this.КонтекстноеМеню.Name = "КонтекстноеМеню";
            this.КонтекстноеМеню.Size = new System.Drawing.Size(224, 70);
            // 
            // редактироватьПараметрыToolStripMenuItem
            // 
            this.редактироватьПараметрыToolStripMenuItem.Image = global::TestMenuPopup.Properties.Resources.SmallEdit;
            this.редактироватьПараметрыToolStripMenuItem.Name = "редактироватьПараметрыToolStripMenuItem";
            this.редактироватьПараметрыToolStripMenuItem.Size = new System.Drawing.Size(223, 22);
            this.редактироватьПараметрыToolStripMenuItem.Text = "Редактировать параметры";
            this.редактироватьПараметрыToolStripMenuItem.MouseHover += new System.EventHandler(this.редактироватьПараметрыToolStripMenuItem_MouseHover);
            this.редактироватьПараметрыToolStripMenuItem.MouseLeave += new System.EventHandler(this.редактироватьПараметрыToolStripMenuItem_MouseLeave);
            this.редактироватьПараметрыToolStripMenuItem.Click += new System.EventHandler(this.редактироватьПараметрыToolStripMenuItem_Click);
            // 
            // установитьГруппуToolStripMenuItem
            // 
            this.установитьГруппуToolStripMenuItem.Image = global::TestMenuPopup.Properties.Resources.Folder;
            this.установитьГруппуToolStripMenuItem.Name = "установитьГруппуToolStripMenuItem";
            this.установитьГруппуToolStripMenuItem.Size = new System.Drawing.Size(223, 22);
            this.установитьГруппуToolStripMenuItem.Text = "Установить группу";
            this.установитьГруппуToolStripMenuItem.MouseHover += new System.EventHandler(this.установитьГруппуToolStripMenuItem_MouseHover);
            this.установитьГруппуToolStripMenuItem.MouseLeave += new System.EventHandler(this.установитьГруппуToolStripMenuItem_MouseLeave);
            this.установитьГруппуToolStripMenuItem.Click += new System.EventHandler(this.установитьГруппуToolStripMenuItem_Click);
            // 
            // удалитьToolStripMenuItem
            // 
            this.удалитьToolStripMenuItem.Image = global::TestMenuPopup.Properties.Resources.SmallRemove;
            this.удалитьToolStripMenuItem.Name = "удалитьToolStripMenuItem";
            this.удалитьToolStripMenuItem.Size = new System.Drawing.Size(223, 22);
            this.удалитьToolStripMenuItem.Text = "Удалить из  списка";
            this.удалитьToolStripMenuItem.MouseHover += new System.EventHandler(this.удалитьToolStripMenuItem_MouseHover);
            this.удалитьToolStripMenuItem.MouseLeave += new System.EventHandler(this.удалитьToolStripMenuItem_MouseLeave);
            this.удалитьToolStripMenuItem.Click += new System.EventHandler(this.удалитьToolStripMenuItem_Click);
            // 
            // КоллекцияИконокПодбора
            // 
            this.КоллекцияИконокПодбора.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("КоллекцияИконокПодбора.ImageStream")));
            this.КоллекцияИконокПодбора.TransparentColor = System.Drawing.Color.Transparent;
            this.КоллекцияИконокПодбора.Images.SetKeyName(0, "77.ico");
            this.КоллекцияИконокПодбора.Images.SetKeyName(1, "80.ico");
            this.КоллекцияИконокПодбора.Images.SetKeyName(2, "81.ico");
            this.КоллекцияИконокПодбора.Images.SetKeyName(3, "82.ico");
            // 
            // СписокЗарегистрированныхБаз
            // 
            this.СписокЗарегистрированныхБаз.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.ИмяБазыОригинал,
            this.ПутьБазыОригинал});
            this.СписокЗарегистрированныхБаз.Dock = System.Windows.Forms.DockStyle.Top;
            this.СписокЗарегистрированныхБаз.FullRowSelect = true;
            this.СписокЗарегистрированныхБаз.GridLines = true;
            this.СписокЗарегистрированныхБаз.HideSelection = false;
            this.СписокЗарегистрированныхБаз.Location = new System.Drawing.Point(0, 0);
            this.СписокЗарегистрированныхБаз.MultiSelect = false;
            this.СписокЗарегистрированныхБаз.Name = "СписокЗарегистрированныхБаз";
            this.СписокЗарегистрированныхБаз.ShowItemToolTips = true;
            this.СписокЗарегистрированныхБаз.Size = new System.Drawing.Size(721, 227);
            this.СписокЗарегистрированныхБаз.SmallImageList = this.КоллекцияИконокПодбора;
            this.СписокЗарегистрированныхБаз.TabIndex = 3;
            this.СписокЗарегистрированныхБаз.UseCompatibleStateImageBehavior = false;
            this.СписокЗарегистрированныхБаз.View = System.Windows.Forms.View.Details;
            this.СписокЗарегистрированныхБаз.DoubleClick += new System.EventHandler(this.СписокЗарегистрированныхБаз_DoubleClick);
            this.СписокЗарегистрированныхБаз.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.СписокЗарегистрированныхБаз_KeyPress);
            this.СписокЗарегистрированныхБаз.MouseHover += new System.EventHandler(this.СписокЗарегистрированныхБаз_MouseHover);
            this.СписокЗарегистрированныхБаз.ItemDrag += new System.Windows.Forms.ItemDragEventHandler(this.СписокЗарегистрированныхБаз_ItemDrag);
            this.СписокЗарегистрированныхБаз.MouseLeave += new System.EventHandler(this.СписокЗарегистрированныхБаз_MouseLeave);
            // 
            // ИмяБазыОригинал
            // 
            this.ИмяБазыОригинал.Text = "Имя базы";
            this.ИмяБазыОригинал.Width = 365;
            // 
            // ПутьБазыОригинал
            // 
            this.ПутьБазыОригинал.Text = "Путь";
            this.ПутьБазыОригинал.Width = 335;
            // 
            // МенюВыбораПлатформы
            // 
            this.МенюВыбораПлатформы.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.сПредприятие77ToolStripMenuItem1,
            this.сПредприятие80ToolStripMenuItem1,
            this.сПредприятие81ToolStripMenuItem1,
            this.сПредприятие82ToolStripMenuItem1});
            this.МенюВыбораПлатформы.Name = "МенюВыбораПлатформы";
            this.МенюВыбораПлатформы.Size = new System.Drawing.Size(189, 92);
            // 
            // сПредприятие77ToolStripMenuItem1
            // 
            this.сПредприятие77ToolStripMenuItem1.Image = global::TestMenuPopup.Properties.Resources.Платформа77;
            this.сПредприятие77ToolStripMenuItem1.Name = "сПредприятие77ToolStripMenuItem1";
            this.сПредприятие77ToolStripMenuItem1.Size = new System.Drawing.Size(188, 22);
            this.сПредприятие77ToolStripMenuItem1.Text = "1С Предприятие 7.7";
            this.сПредприятие77ToolStripMenuItem1.Click += new System.EventHandler(this.сПредприятие77ToolStripMenuItem1_Click);
            // 
            // сПредприятие80ToolStripMenuItem1
            // 
            this.сПредприятие80ToolStripMenuItem1.Image = global::TestMenuPopup.Properties.Resources.Платформа80;
            this.сПредприятие80ToolStripMenuItem1.Name = "сПредприятие80ToolStripMenuItem1";
            this.сПредприятие80ToolStripMenuItem1.Size = new System.Drawing.Size(188, 22);
            this.сПредприятие80ToolStripMenuItem1.Text = "1С Предприятие 8.0";
            this.сПредприятие80ToolStripMenuItem1.Click += new System.EventHandler(this.сПредприятие80ToolStripMenuItem1_Click);
            // 
            // сПредприятие81ToolStripMenuItem1
            // 
            this.сПредприятие81ToolStripMenuItem1.Image = global::TestMenuPopup.Properties.Resources.Платформа81;
            this.сПредприятие81ToolStripMenuItem1.Name = "сПредприятие81ToolStripMenuItem1";
            this.сПредприятие81ToolStripMenuItem1.Size = new System.Drawing.Size(188, 22);
            this.сПредприятие81ToolStripMenuItem1.Text = "1С Предприятие 8.1";
            this.сПредприятие81ToolStripMenuItem1.Click += new System.EventHandler(this.сПредприятие81ToolStripMenuItem1_Click);
            // 
            // сПредприятие82ToolStripMenuItem1
            // 
            this.сПредприятие82ToolStripMenuItem1.Image = global::TestMenuPopup.Properties.Resources.Платформа82;
            this.сПредприятие82ToolStripMenuItem1.Name = "сПредприятие82ToolStripMenuItem1";
            this.сПредприятие82ToolStripMenuItem1.Size = new System.Drawing.Size(188, 22);
            this.сПредприятие82ToolStripMenuItem1.Text = "1С Предприятие 8.2";
            this.сПредприятие82ToolStripMenuItem1.Click += new System.EventHandler(this.сПредприятие82ToolStripMenuItem1_Click);
            // 
            // ПанельПодбора
            // 
            this.ПанельПодбора.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.ПанельПодбора.Controls.Add(this.splitter1);
            this.ПанельПодбора.Controls.Add(this.СписокДобавляемыхБаз);
            this.ПанельПодбора.Controls.Add(this.СписокЗарегистрированныхБаз);
            this.ПанельПодбора.Location = new System.Drawing.Point(0, 28);
            this.ПанельПодбора.Name = "ПанельПодбора";
            this.ПанельПодбора.Size = new System.Drawing.Size(721, 458);
            this.ПанельПодбора.TabIndex = 4;
            // 
            // splitter1
            // 
            this.splitter1.Cursor = System.Windows.Forms.Cursors.HSplit;
            this.splitter1.Dock = System.Windows.Forms.DockStyle.Top;
            this.splitter1.Location = new System.Drawing.Point(0, 227);
            this.splitter1.Name = "splitter1";
            this.splitter1.Size = new System.Drawing.Size(721, 3);
            this.splitter1.TabIndex = 4;
            this.splitter1.TabStop = false;
            // 
            // Статус
            // 
            this.Статус.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.Подсказка});
            this.Статус.Location = new System.Drawing.Point(0, 525);
            this.Статус.Name = "Статус";
            this.Статус.Size = new System.Drawing.Size(724, 22);
            this.Статус.TabIndex = 7;
            this.Статус.Text = "statusStrip1";
            // 
            // Подсказка
            // 
            this.Подсказка.Name = "Подсказка";
            this.Подсказка.Size = new System.Drawing.Size(0, 17);
            this.Подсказка.Paint += new System.Windows.Forms.PaintEventHandler(this.Подсказка_Paint);
            // 
            // КнопкаОтмена
            // 
            this.КнопкаОтмена.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.КнопкаОтмена.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.КнопкаОтмена.Image = global::TestMenuPopup.Properties.Resources.Cancel;
            this.КнопкаОтмена.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.КнопкаОтмена.Location = new System.Drawing.Point(97, 492);
            this.КнопкаОтмена.Name = "КнопкаОтмена";
            this.КнопкаОтмена.Size = new System.Drawing.Size(79, 31);
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
            this.КнопкаОК.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.КнопкаОК.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.КнопкаОК.Image = global::TestMenuPopup.Properties.Resources.OK;
            this.КнопкаОК.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.КнопкаОК.Location = new System.Drawing.Point(12, 492);
            this.КнопкаОК.Name = "КнопкаОК";
            this.КнопкаОК.Size = new System.Drawing.Size(79, 31);
            this.КнопкаОК.TabIndex = 5;
            this.КнопкаОК.Text = "    ОК";
            this.КнопкаОК.UseVisualStyleBackColor = true;
            this.КнопкаОК.MouseLeave += new System.EventHandler(this.КнопкаОК_MouseLeave);
            this.КнопкаОК.Click += new System.EventHandler(this.КнопкаОК_Click);
            this.КнопкаОК.MouseHover += new System.EventHandler(this.КнопкаОК_MouseHover);
            // 
            // ФормаПодбораБазДанных
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(724, 547);
            this.Controls.Add(this.Статус);
            this.Controls.Add(this.КнопкаОтмена);
            this.Controls.Add(this.КнопкаОК);
            this.Controls.Add(this.ПанельПодбора);
            this.Controls.Add(this.ПанельИнструментов);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.KeyPreview = true;
            this.Name = "ФормаПодбораБазДанных";
            this.ShowInTaskbar = false;
            this.Text = "Подбор баз данных";
            this.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.ФормаПодбораБазДанных_KeyPress);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.ФормаПодбораБазДанных_KeyDown);
            this.ПанельИнструментов.ResumeLayout(false);
            this.ПанельИнструментов.PerformLayout();
            this.КонтекстноеМеню.ResumeLayout(false);
            this.МенюВыбораПлатформы.ResumeLayout(false);
            this.ПанельПодбора.ResumeLayout(false);
            this.Статус.ResumeLayout(false);
            this.Статус.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ToolStrip ПанельИнструментов;
        private System.Windows.Forms.ListView СписокЗарегистрированныхБаз;
        private System.Windows.Forms.ColumnHeader ИмяБазы;
        private System.Windows.Forms.ColumnHeader Путь;
        private System.Windows.Forms.ColumnHeader Группа;
        private System.Windows.Forms.ColumnHeader ИмяБазыОригинал;
        private System.Windows.Forms.ColumnHeader ПутьБазыОригинал;
        private System.Windows.Forms.ToolStripButton ВыборТипаПлатформы;
        private System.Windows.Forms.ContextMenuStrip МенюВыбораПлатформы;
        private System.Windows.Forms.ToolStripMenuItem сПредприятие77ToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem сПредприятие80ToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem сПредприятие81ToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem сПредприятие82ToolStripMenuItem1;
        private System.Windows.Forms.ToolStripLabel ТекстПлатформа;
        private System.Windows.Forms.ImageList КоллекцияИконокПодбора;
        private System.Windows.Forms.Panel ПанельПодбора;
        private System.Windows.Forms.Splitter splitter1;
        private System.Windows.Forms.Button КнопкаОК;
        private System.Windows.Forms.Button КнопкаОтмена;
        private System.Windows.Forms.StatusStrip Статус;
        private System.Windows.Forms.ToolStripStatusLabel Подсказка;
        public System.Windows.Forms.ListView СписокДобавляемыхБаз;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripButton КопироватьВсе;
        private System.Windows.Forms.ToolStripButton УстановитьГруппу;
        private System.Windows.Forms.ToolStripButton ОчиститьСписок;
        private System.Windows.Forms.ContextMenuStrip КонтекстноеМеню;
        private System.Windows.Forms.ToolStripMenuItem редактироватьПараметрыToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem установитьГруппуToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem удалитьToolStripMenuItem;
    }
}