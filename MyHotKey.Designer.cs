namespace TestMenuPopup
{
    partial class MyHotKey
    {
        /// <summary> 
        /// Требуется переменная конструктора.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Освободить все используемые ресурсы.
        /// </summary>
        /// <param name="disposing">истинно, если управляемый ресурс должен быть удален; иначе ложно.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Код, автоматически созданный конструктором компонентов

        /// <summary> 
        /// Обязательный метод для поддержки конструктора - не изменяйте 
        /// содержимое данного метода при помощи редактора кода.
        /// </summary>
        private void InitializeComponent()
        {
            this.ПолеВвода = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // ПолеВвода
            // 
            this.ПолеВвода.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.ПолеВвода.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.ПолеВвода.Location = new System.Drawing.Point(0, 0);
            this.ПолеВвода.MaxLength = 1;
            this.ПолеВвода.Name = "ПолеВвода";
            this.ПолеВвода.Size = new System.Drawing.Size(71, 20);
            this.ПолеВвода.TabIndex = 0;
            this.ПолеВвода.Text = "НЕТ";
            this.ПолеВвода.TextChanged += new System.EventHandler(this.ПолеВвода_TextChanged);
            this.ПолеВвода.KeyDown += new System.Windows.Forms.KeyEventHandler(this.ПолеВвода_KeyDown);
            this.ПолеВвода.KeyUp += new System.Windows.Forms.KeyEventHandler(this.ПолеВвода_KeyUp);
            // 
            // MyHotKey
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.ПолеВвода);
            this.Name = "MyHotKey";
            this.Size = new System.Drawing.Size(72, 20);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox ПолеВвода;
    }
}
