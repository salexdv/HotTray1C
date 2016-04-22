/*
 * Created by SharpDevelop.
 * User: Alex
 * Date: 19.07.2012
 * Time: 10:39
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
namespace TestMenuPopup
{
	partial class ФормаЗапуска
	{
		/// <summary>
		/// Designer variable used to keep track of non-visual components.
		/// </summary>
		private System.ComponentModel.IContainer components = null;
		
		/// <summary>
		/// Disposes resources used by the form.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing) {
				if (components != null) {
					components.Dispose();
				}
			}
			base.Dispose(disposing);
		}
		
		/// <summary>
		/// This method is required for Windows Forms designer support.
		/// Do not change the method contents inside the source code editor. The Forms designer might
		/// not be able to load this method if it was changed manually.
		/// </summary>
		private void InitializeComponent()
		{
			this.treeView1 = new System.Windows.Forms.TreeView();
			this.SuspendLayout();
			// 
			// treeView1
			// 
			this.treeView1.Location = new System.Drawing.Point(0, 0);
			this.treeView1.Name = "treeView1";
			this.treeView1.Size = new System.Drawing.Size(367, 404);
			this.treeView1.TabIndex = 0;
			// 
			// ФормаЗапуска
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(368, 406);
			this.Controls.Add(this.treeView1);
			this.Name = "ФормаЗапуска";
			this.Text = "ФормаЗапуска";
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.ФормаЗапускаFormClosing);
			this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.ФормаЗапускаFormClosed);
			this.Load += new System.EventHandler(this.ФормаЗапускаLoad);
			this.ResumeLayout(false);
		}
		private System.Windows.Forms.TreeView treeView1;
	}
}
