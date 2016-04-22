namespace TestMenuPopup
{
    partial class ФормаМонитора
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ФормаМонитора));
            this.СписокМонитора = new System.Windows.Forms.ListView();
            this.ИмяПользователя = new System.Windows.Forms.ColumnHeader();
            this.Режим = new System.Windows.Forms.ColumnHeader();
            this.Монопольно = new System.Windows.Forms.ColumnHeader();
            this.НачалоРаботы = new System.Windows.Forms.ColumnHeader();
            this.Компьютер = new System.Windows.Forms.ColumnHeader();
            this.КоллекцияИконокМонитора = new System.Windows.Forms.ImageList(this.components);
            this.ТаймерОбновления = new System.Windows.Forms.Timer(this.components);
            this.ТекстНаименованиеБазы = new System.Windows.Forms.Label();
            this.ТекстПутьКБазе = new System.Windows.Forms.Label();
            this.ТекстКоличествоПользователей = new System.Windows.Forms.Label();
            this.ГруппаИнформация = new System.Windows.Forms.GroupBox();
            this.ГруппаИнформация.SuspendLayout();
            this.SuspendLayout();
            // 
            // СписокМонитора
            // 
            this.СписокМонитора.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.СписокМонитора.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.ИмяПользователя,
            this.Режим,
            this.Монопольно,
            this.НачалоРаботы,
            this.Компьютер});
            this.СписокМонитора.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.СписокМонитора.FullRowSelect = true;
            this.СписокМонитора.GridLines = true;
            this.СписокМонитора.HideSelection = false;
            this.СписокМонитора.Location = new System.Drawing.Point(2, 58);
            this.СписокМонитора.Name = "СписокМонитора";
            this.СписокМонитора.Size = new System.Drawing.Size(691, 327);
            this.СписокМонитора.SmallImageList = this.КоллекцияИконокМонитора;
            this.СписокМонитора.Sorting = System.Windows.Forms.SortOrder.Ascending;
            this.СписокМонитора.TabIndex = 0;
            this.СписокМонитора.UseCompatibleStateImageBehavior = false;
            this.СписокМонитора.View = System.Windows.Forms.View.Details;
            this.СписокМонитора.MouseUp += new System.Windows.Forms.MouseEventHandler(this.СписокМонитора_MouseUp);
            // 
            // ИмяПользователя
            // 
            this.ИмяПользователя.Text = "Имя пользователя";
            this.ИмяПользователя.Width = 165;
            // 
            // Режим
            // 
            this.Режим.Text = "Режим";
            this.Режим.Width = 113;
            // 
            // Монопольно
            // 
            this.Монопольно.Text = "Монопольно";
            this.Монопольно.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.Монопольно.Width = 78;
            // 
            // НачалоРаботы
            // 
            this.НачалоРаботы.Text = "Начало работы";
            this.НачалоРаботы.Width = 152;
            // 
            // Компьютер
            // 
            this.Компьютер.Text = "Компьютер";
            this.Компьютер.Width = 158;
            // 
            // КоллекцияИконокМонитора
            // 
            this.КоллекцияИконокМонитора.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("КоллекцияИконокМонитора.ImageStream")));
            this.КоллекцияИконокМонитора.TransparentColor = System.Drawing.Color.Transparent;
            this.КоллекцияИконокМонитора.Images.SetKeyName(0, "77.ico");
            this.КоллекцияИконокМонитора.Images.SetKeyName(1, "Конфигуратор.ico");
            this.КоллекцияИконокМонитора.Images.SetKeyName(2, "Отладчик.ico");
            this.КоллекцияИконокМонитора.Images.SetKeyName(3, "Монитор.ico");
            this.КоллекцияИконокМонитора.Images.SetKeyName(4, "81.png");
            this.КоллекцияИконокМонитора.Images.SetKeyName(5, "Режим22.png");
            this.КоллекцияИконокМонитора.Images.SetKeyName(6, "82.ico");
            this.КоллекцияИконокМонитора.Images.SetKeyName(7, "Режим32.ico");
            // 
            // ТаймерОбновления
            // 
            this.ТаймерОбновления.Interval = 2500;
            this.ТаймерОбновления.Tick += new System.EventHandler(this.ТаймерОбновления_Tick);
            // 
            // ТекстНаименованиеБазы
            // 
            this.ТекстНаименованиеБазы.AutoSize = true;
            this.ТекстНаименованиеБазы.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.ТекстНаименованиеБазы.ForeColor = System.Drawing.Color.Green;
            this.ТекстНаименованиеБазы.Location = new System.Drawing.Point(6, 16);
            this.ТекстНаименованиеБазы.Name = "ТекстНаименованиеБазы";
            this.ТекстНаименованиеБазы.Size = new System.Drawing.Size(173, 13);
            this.ТекстНаименованиеБазы.TabIndex = 1;
            this.ТекстНаименованиеБазы.Text = "База: ООО \"Рога и копыта\"";
            // 
            // ТекстПутьКБазе
            // 
            this.ТекстПутьКБазе.AutoSize = true;
            this.ТекстПутьКБазе.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.ТекстПутьКБазе.ForeColor = System.Drawing.Color.Green;
            this.ТекстПутьКБазе.Location = new System.Drawing.Point(6, 33);
            this.ТекстПутьКБазе.Name = "ТекстПутьКБазе";
            this.ТекстПутьКБазе.Size = new System.Drawing.Size(181, 13);
            this.ТекстПутьКБазе.TabIndex = 2;
            this.ТекстПутьКБазе.Text = "Расположение базы: C:\\temp";
            // 
            // ТекстКоличествоПользователей
            // 
            this.ТекстКоличествоПользователей.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.ТекстКоличествоПользователей.AutoEllipsis = true;
            this.ТекстКоличествоПользователей.AutoSize = true;
            this.ТекстКоличествоПользователей.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.ТекстКоличествоПользователей.ForeColor = System.Drawing.Color.RoyalBlue;
            this.ТекстКоличествоПользователей.Location = new System.Drawing.Point(0, 388);
            this.ТекстКоличествоПользователей.Name = "ТекстКоличествоПользователей";
            this.ТекстКоличествоПользователей.Size = new System.Drawing.Size(245, 13);
            this.ТекстКоличествоПользователей.TabIndex = 3;
            this.ТекстКоличествоПользователей.Text = "Количество активных пользователей: 5";
            this.ТекстКоличествоПользователей.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // ГруппаИнформация
            // 
            this.ГруппаИнформация.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.ГруппаИнформация.Controls.Add(this.ТекстПутьКБазе);
            this.ГруппаИнформация.Controls.Add(this.ТекстНаименованиеБазы);
            this.ГруппаИнформация.Location = new System.Drawing.Point(2, 1);
            this.ГруппаИнформация.Name = "ГруппаИнформация";
            this.ГруппаИнформация.Size = new System.Drawing.Size(691, 51);
            this.ГруппаИнформация.TabIndex = 4;
            this.ГруппаИнформация.TabStop = false;
            this.ГруппаИнформация.Text = "Информация";
            // 
            // ФормаМонитора
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(697, 404);
            this.Controls.Add(this.ГруппаИнформация);
            this.Controls.Add(this.ТекстКоличествоПользователей);
            this.Controls.Add(this.СписокМонитора);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.KeyPreview = true;
            this.Name = "ФормаМонитора";
            this.Text = "Монитор пользователей";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.ФормаМонитора_FormClosed);
            this.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.ФормаМонитора_KeyPress);
            this.ГруппаИнформация.ResumeLayout(false);
            this.ГруппаИнформация.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ListView СписокМонитора;
        private System.Windows.Forms.ColumnHeader ИмяПользователя;
        private System.Windows.Forms.ColumnHeader Режим;
        private System.Windows.Forms.ColumnHeader Монопольно;
        private System.Windows.Forms.ColumnHeader НачалоРаботы;
        private System.Windows.Forms.ColumnHeader Компьютер;
        private System.Windows.Forms.Timer ТаймерОбновления;
        private System.Windows.Forms.Label ТекстНаименованиеБазы;
        private System.Windows.Forms.Label ТекстПутьКБазе;
        private System.Windows.Forms.Label ТекстКоличествоПользователей;
        private System.Windows.Forms.GroupBox ГруппаИнформация;
        private System.Windows.Forms.ImageList КоллекцияИконокМонитора;
    }
}