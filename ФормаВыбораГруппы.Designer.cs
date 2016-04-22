namespace TestMenuPopup
{
    partial class ФормаВыбораГруппы
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ФормаВыбораГруппы));
            this.ДеревоГрупп = new System.Windows.Forms.TreeView();
            this.КоллекцияКартинокДерева = new System.Windows.Forms.ImageList(this.components);
            this.SuspendLayout();
            // 
            // ДеревоГрупп
            // 
            this.ДеревоГрупп.ImageIndex = 0;
            this.ДеревоГрупп.ImageList = this.КоллекцияКартинокДерева;
            this.ДеревоГрупп.Location = new System.Drawing.Point(1, 0);
            this.ДеревоГрупп.Name = "ДеревоГрупп";
            this.ДеревоГрупп.SelectedImageIndex = 1;
            this.ДеревоГрупп.Size = new System.Drawing.Size(375, 448);
            this.ДеревоГрупп.TabIndex = 0;
            this.ДеревоГрупп.DoubleClick += new System.EventHandler(this.ДеревоГрупп_DoubleClick);
            // 
            // КоллекцияКартинокДерева
            // 
            this.КоллекцияКартинокДерева.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("КоллекцияКартинокДерева.ImageStream")));
            this.КоллекцияКартинокДерева.TransparentColor = System.Drawing.Color.Transparent;
            this.КоллекцияКартинокДерева.Images.SetKeyName(0, "folder.bmp");
            this.КоллекцияКартинокДерева.Images.SetKeyName(1, "folder_edit.png");
            // 
            // ФормаВыбораГруппы
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(377, 449);
            this.Controls.Add(this.ДеревоГрупп);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.KeyPreview = true;
            this.Name = "ФормаВыбораГруппы";
            this.Text = "Выбор группы";
            this.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.ФормаВыбораГруппы_KeyPress);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.ФормаВыбораГруппы_KeyDown);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TreeView ДеревоГрупп;
        private System.Windows.Forms.ImageList КоллекцияКартинокДерева;
    }
}