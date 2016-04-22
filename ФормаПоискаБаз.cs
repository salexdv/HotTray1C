using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Xml;
using System.Windows.Forms;
using System.IO;

namespace TestMenuPopup
{
    public partial class ФормаПоискаБаз : Form
    {
        public String НачальныйКаталогПоиска;
        public XmlNode ГруппаРодительДляНовыхБаз;
        public int ТипПлатформы8х;

        public ФормаПоискаБаз()
        {
            InitializeComponent();
        }

        private void ВывестиНаименованиеГруппы()
        {
            if (ГруппаРодительДляНовыхБаз != ГлавноеОкно.УзелБазДанных)
                ГруппаБазДанных.Text = ГлавноеОкно.ПолучитьАтрибутУзла(ГруппаРодительДляНовыхБаз, "Наименование");
            else
                ГруппаБазДанных.Text = "Группа баз данных";
        }
        
        public void Инициализация(XmlNode Группа)
        {
            ТипПлатформы.SelectedIndex = 1;
            ГруппаРодительДляНовыхБаз = Группа;
            ВывестиНаименованиеГруппы();
        }

        private void Подсказка_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.DrawImage(Properties.Resources.smallinfo, 1, 1);
        }

        private void ПоказатьИнформацию(string Текст)
        {
            Статус.Items[0].Text = "       " + Текст;
        }

        private void ОчиститьИнформацию()
        {
            Статус.Items[0].Text = "";
        }

        private void НачальныйКаталог_MouseLeave(object sender, EventArgs e)
        {
            ОчиститьИнформацию();
        }

        private void ВыбратьНачальныйКаталог_MouseHover(object sender, EventArgs e)
        {
            ПоказатьИнформацию("Выбрать начальный каталог поиска");
        }

        private void ВыбратьНачальныйКаталог_MouseLeave(object sender, EventArgs e)
        {
            ОчиститьИнформацию();
        }

        private void ГруппаБазДанных_MouseLeave(object sender, EventArgs e)
        {
            ОчиститьИнформацию();
        }

        private void ВыбратьГруппу_MouseHover(object sender, EventArgs e)
        {
            ПоказатьИнформацию("Выбрать группу баз данных");
        }

        private void ВыбратьГруппу_MouseLeave(object sender, EventArgs e)
        {
            ОчиститьИнформацию();
        }

        private void НачальныйКаталог_MouseHover(object sender, EventArgs e)
        {
            ПоказатьИнформацию("Каталог, в котором будет осуществляться поиск");
        }

        private void ГруппаБазДанных_MouseHover(object sender, EventArgs e)
        {
            ПоказатьИнформацию("Группа, в которую будут добавляться найденные базы");
        }

        private void ВыбратьНачальныйКаталог_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog openFolder = new FolderBrowserDialog();

            if (!String.IsNullOrEmpty(НачальныйКаталог.Text))
                openFolder.SelectedPath = НачальныйКаталог.Text;
            if (openFolder.ShowDialog() == DialogResult.OK)
            {
                НачальныйКаталог.Text = openFolder.SelectedPath;
            }

            openFolder.Dispose();
        }

        private void ВыбратьГруппу_Click(object sender, EventArgs e)
        {
            ФормаВыбораГруппы ФормаВыбора = new ФормаВыбораГруппы();
            ФормаВыбора.ОткрытьВыборГрупп(ГруппаРодительДляНовыхБаз);
            if (ФормаВыбора.ShowDialog() == DialogResult.OK)
            {
                ГруппаРодительДляНовыхБаз = ФормаВыбора.ВыбраннаяГруппа;
                ВывестиНаименованиеГруппы();
            }
        }

        private void КнопкаОК_MouseHover(object sender, EventArgs e)
        {
            ПоказатьИнформацию("Начать поиск");
        }

        private void КнопкаОтмена_MouseHover(object sender, EventArgs e)
        {
            ПоказатьИнформацию("Отказаться от поиска");
        }

        private void КнопкаОтмена_MouseLeave(object sender, EventArgs e)
        {
            ОчиститьИнформацию();
        }

        private void КнопкаОК_MouseLeave(object sender, EventArgs e)
        {
            ОчиститьИнформацию();
        }

        private void КнопкаОК_Click(object sender, EventArgs e)
        {
            string КаталогПоиска = НачальныйКаталог.Text;
            if (String.IsNullOrEmpty(КаталогПоиска))
            {
                ГлавноеОкно.ПоказатьИнфомационноеСообщение("Не указан начальный каталог поиска", this);
                return;
            }
            if (!Directory.Exists(КаталогПоиска))
            {
                ГлавноеОкно.ПоказатьИнфомационноеСообщение("Указан неверный начальный каталог поиска", this);
                return;
            }

            НачальныйКаталогПоиска = КаталогПоиска;
            ТипПлатформы8х = ТипПлатформы.SelectedIndex;

            DialogResult = DialogResult.OK;

            Close();
        }

        private void КнопкаОтмена_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void ТипПлатформы_DrawItem(object sender, DrawItemEventArgs e)
        {
            if (e.Index != -1)
            {
                e.DrawBackground();
                // Прорисовываем прямоугольник
                e.Graphics.DrawRectangle(new Pen(Color.White, 1), new Rectangle(e.Bounds.X, e.Bounds.Y, 200, 15));
                // Рисуем справа рисунок
                e.Graphics.DrawImage(ГлавноеОкно.ПолучитьКартинкуПлатформы(e.Index + 1), e.Bounds.X, e.Bounds.Y);
                // Выводим текст                
                e.Graphics.DrawString("     " + ТипПлатформы.Items[e.Index].ToString(), new Font(FontFamily.GenericSerif, 10, FontStyle.Regular), Brushes.Black, e.Bounds.X, e.Bounds.Y);
            }
        }

        private void ТипПлатформы_MouseHover(object sender, EventArgs e)
        {
            ПоказатьИнформацию("Будет использоваться при добавлении новых баз 8.x");
        }

        private void ТипПлатформы_MouseLeave(object sender, EventArgs e)
        {
            ОчиститьИнформацию();
        }        

        private void ФормаПоискаБаз_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == 27)
                Close();
        }

        private void ФормаПоискаБаз_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyValue == 13)
            {
                if ((e.Modifiers & Keys.Control) != 0)
                    КнопкаОК_Click(sender, e);
            }
        }
        
    }
}
