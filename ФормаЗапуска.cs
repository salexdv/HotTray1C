/*
 * Created by SharpDevelop.
 * User: Alex
 * Date: 19.07.2012
 * Time: 10:39
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Drawing;
using System.Windows.Forms;

namespace TestMenuPopup
{
	/// <summary>
	/// Description of ФормаЗапуска.
	/// </summary>
	public partial class ФормаЗапуска : Form
	{
		public ФормаЗапуска()
		{
			//
			// The InitializeComponent() call is required for Windows Forms designer support.
			//
			InitializeComponent();
			
			//
			// TODO: Add constructor code after the InitializeComponent() call.
			//
		}
		
		void ФормаЗапускаLoad(object sender, EventArgs e)
		{
			
		}
		
		void ФормаЗапускаFormClosed(object sender, FormClosedEventArgs e)
		{
				
		}
		
		void ФормаЗапускаFormClosing(object sender, FormClosingEventArgs e)
		{
			e.Cancel = true;
			Hide();
		}
	}
}
