using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Xml;
using System.Windows.Forms;

namespace TestMenuPopup
{
    public partial class ФормаВыбораГруппы : Form
    {
        public XmlNode ВыбраннаяГруппа;
        
        public ФормаВыбораГруппы()
        {
            InitializeComponent();
        }

        public void ОткрытьВыборГрупп(XmlNode АктивнаяГруппа)
        {
            // Заполним дерево групп элементами
            ГлавноеОкно.ЗаполнениеДереваГрупп(ДеревоГрупп, ГлавноеОкно.ПолучитьУзелБазДанных(), null, АктивнаяГруппа);
            ГлавноеОкно.ДеревоГруппПослеЗаполнения(ДеревоГрупп.Nodes[0]);
        }

        private void ДеревоГрупп_DoubleClick(object sender, EventArgs e)
        {
            ВыбраннаяГруппа = (XmlNode)ДеревоГрупп.SelectedNode.Tag;
            DialogResult = DialogResult.OK;
            Close();
        }

        private void ФормаВыбораГруппы_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == 27)
                Close();
        }

        private void ФормаВыбораГруппы_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyValue == 13)
            {
                if ((e.Modifiers & Keys.Control) != 0)
                    ДеревоГрупп_DoubleClick(sender, e);
            }
        }        
    }
}
