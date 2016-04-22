using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Xml;
using System.Windows.Forms;
using System.IO;
using System.Runtime.InteropServices;
using MainStruct;

namespace TestMenuPopup
{
    public partial class ФормаВнешнегоПриложения : Form
    {       
        public СтруктураНастроекЭлемента ТекущаяНастройка;
        public XmlNode СсылкаБазыДанных;
        public XmlNode РодительБазыДанных;        

        public ФормаВнешнегоПриложения()
        {            
            InitializeComponent();                                    
        }

        private void УправлениеВидимостью()
        {
            СобственнаяПрограммаЗапуска.Enabled = false;
            ВыбратьСобственнуюПрограмму.Enabled = false;
            String ПутьКФайлу = Путь.Text;
            ПутьКФайлу = ПутьКФайлу.Trim();
            СобственнаяПрограммаЗапуска.Enabled = false;
            ВыбратьСобственнуюПрограмму.Enabled = false;
            ПараметрыЗапуска.Enabled = false;
            if (File.Exists(ПутьКФайлу))
            {
                bool ЭтоПриложение = false;
                ЭтоПриложение = ПутьКФайлу.Contains(".exe");
                if (!ЭтоПриложение)
                    ЭтоПриложение = ПутьКФайлу.Contains(".com");
                СобственнаяПрограммаЗапуска.Enabled = (!ЭтоПриложение);
                ВыбратьСобственнуюПрограмму.Enabled = (!ЭтоПриложение);
                ПараметрыЗапуска.Enabled = ЭтоПриложение;
            }
        }
           
        public void СоздатьПриложение(XmlNode РодительБазы)
        {
            РодительБазыДанных = РодительБазы;
            Группа.Items.Add(ГлавноеОкно.ПолучитьАтрибутУзла(РодительБазы, "Наименование"));
            Группа.SelectedIndex = 0;
            Text = "Добавление нового приложения (файла)";            
            ПоказыватьВМенюЗапуска.Checked = true;
            УправлениеВидимостью();
        }

        public void ОткрытьПриложение(XmlNode РодительБазы, XmlNode УзелЭлемента, bool Копирование)
        {
            РодительБазыДанных = РодительБазы;
            СсылкаБазыДанных = УзелЭлемента;
            if (!Копирование)
            {
                СочетаниеКлавиш.ЗаполнитьСочетаниеКлавиш(ГлавноеОкно.ПолучитьАтрибутУзла(УзелЭлемента, "СочетаниеКлавиш"));
                Text = "Редактирование приложения (файла)";
            }
            else
                Text = "Добавление приложения (файла) копированием";
            Наименование.Text = ГлавноеОкно.ПолучитьАтрибутУзла(УзелЭлемента, "Наименование");
            Путь.Text = ГлавноеОкно.ПолучитьАтрибутУзла(УзелЭлемента, "Путь");
            ПоказыватьВМенюЗапуска.Checked = Convert.ToBoolean(ГлавноеОкно.ПолучитьАтрибутУзла(УзелЭлемента, "ПоказыватьВМенюЗапуска"));
            ПараметрыЗапуска.Text = ГлавноеОкно.ПолучитьАтрибутУзла(УзелЭлемента, "ИмяПользователя");
            Описание.Text = ГлавноеОкно.ПолучитьАтрибутУзла(УзелЭлемента, "Описание");            
            СобственнаяПрограммаЗапуска.Text = ГлавноеОкно.ПолучитьАтрибутУзла(УзелЭлемента, "ПрограммаЗапуска");
            УправлениеВидимостью();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }              

        private void НачатьВыборГруппы()
        {
            ФормаВыбораГруппы ФормаВыбора = new ФормаВыбораГруппы();
            ФормаВыбора.ОткрытьВыборГрупп(РодительБазыДанных);
            ФормаВыбора.ShowDialog();
            if (ФормаВыбора.DialogResult == DialogResult.OK)
            {
                РодительБазыДанных = ФормаВыбора.ВыбраннаяГруппа;
                Группа.Refresh();
            }
        }

        private void Группа_DrawItem(object sender, DrawItemEventArgs e)
        {
            e.Graphics.DrawRectangle(new Pen(Color.White, 1), new Rectangle(e.Bounds.X, e.Bounds.Y, e.Bounds.Width, 15));
            e.Graphics.DrawImage(Properties.Resources.Folder, e.Bounds.X, e.Bounds.Y);
            string НаименованиеРодителя = ГлавноеОкно.ПолучитьАтрибутУзла(РодительБазыДанных, "Наименование");
            if (String.IsNullOrEmpty(НаименованиеРодителя))
                НаименованиеРодителя = "Группы баз данных";
            e.Graphics.DrawString("     " + НаименованиеРодителя, new Font(FontFamily.GenericSerif, 10, FontStyle.Regular), Brushes.Black, e.Bounds.X, e.Bounds.Y);
        }

        private void КнопкаОК_Click(object sender, EventArgs e)
        {
            // Проверим заполнение необходимых реквизитов
            string СообщениеОбОшибке = "Не заполнены следующий реквизиты:";
            if (String.IsNullOrEmpty(Наименование.Text))
                СообщениеОбОшибке = СообщениеОбОшибке + "\n -Наименование";
            if (String.IsNullOrEmpty(Путь.Text))
                СообщениеОбОшибке = СообщениеОбОшибке + "\n -Расположение";
            if (СообщениеОбОшибке.Length > 33)
            {
                ГлавноеОкно.ПоказатьИнфомационноеСообщение(СообщениеОбОшибке, this);
                return;
            }
            ТекущаяНастройка.ИмяПользователя = ПараметрыЗапуска.Text;
            ТекущаяНастройка.Приложение = true;
            ТекущаяНастройка.Ссылка = СсылкаБазыДанных;
            ТекущаяНастройка.Наименование = Наименование.Text;            
            ТекущаяНастройка.Путь = Путь.Text;            
            ТекущаяНастройка.ПоказыватьВМенюЗапуска = ПоказыватьВМенюЗапуска.Checked;            
            ТекущаяНастройка.Описание = Описание.Text;
            ТекущаяНастройка.ПрограммаЗапуска = СобственнаяПрограммаЗапуска.Text;
            if (СочетаниеКлавиш.ГорячаяКлавишаВыбрана)
            {
                ТекущаяНастройка.СочетаниеКлавиш = Convert.ToString(СочетаниеКлавиш.КодПервогоСимвола) + "\\" + Convert.ToString(СочетаниеКлавиш.КодВторогоСимвола) + "\\" + Convert.ToString(СочетаниеКлавиш.КодТретьегоСимвола);
            }
            else
                ТекущаяНастройка.СочетаниеКлавиш = "0\\0\\0";
            DialogResult = DialogResult.OK;
            Close();
        }

        private void ВыборПути()
        {           
            OpenFileDialog openFile = new OpenFileDialog();

            if (File.Exists(Путь.Text))
            {
                openFile.FileName = Путь.Text;
            }
            else
            {
                openFile.FileName = "";
            }
            openFile.Title = "Укажите добавляемый файл";
            openFile.Filter = "Все файлы|*.*";
            if (openFile.ShowDialog() == DialogResult.OK)
            {                
                string ПутьПриложения = openFile.FileName;
                Путь.Text = ПутьПриложения;
                if (String.IsNullOrEmpty(Наименование.Text))
                {
                    string[] МассивПути = ПутьПриложения.Split('\\');
                    int Размерность = МассивПути.Length - 1;
                    if (Размерность >= 0)
                    {
                        string НаименованиеФайла = МассивПути[Размерность];
                        int ПоискТочки = НаименованиеФайла.IndexOf('.');
                        if (ПоискТочки != 0)
                            Наименование.Text = НаименованиеФайла.Remove(ПоискТочки);
                        else
                            Наименование.Text = НаименованиеФайла;
                    }
                }                
            }
            УправлениеВидимостью();
            openFile.Dispose();
        }

        private void ВыбратьКаталогБД_Click(object sender, EventArgs e)
        {
            ВыборПути();
        }

        private void ПоказатьИнформацию(string Текст)
        {
            Статус.Items[0].Text = "       " + Текст;
        }

        private void ОчиститьИнформацию()
        {
            Статус.Items[0].Text = "";
        }

        private void Путь_MouseHover(object sender, EventArgs e)
        {
            ПоказатьИнформацию("Путь, по которому располагается приложение (файл)");
        }

        private void Путь_MouseLeave(object sender, EventArgs e)
        {
            ОчиститьИнформацию();
        }

        private void Подсказка_Paint(object sender, PaintEventArgs e)
        {            
            e.Graphics.DrawImage(Properties.Resources.smallinfo, 1, 1);
        }        

        private void Описание_MouseHover(object sender, EventArgs e)
        {
            ПоказатьИнформацию("Дополнительное описание для приложения (файла)");
        }

        private void Описание_MouseLeave(object sender, EventArgs e)
        {
            ОчиститьИнформацию();
        }

        private void ФормаБазыДанных_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == 27)
            {                
                Close();
            }            

        }

        private void ВыборСобственнойПрограммы()
        {
            OpenFileDialog openFile = new OpenFileDialog();

            if (File.Exists(СобственнаяПрограммаЗапуска.Text))
            {
                openFile.FileName = СобственнаяПрограммаЗапуска.Text;
            }
            else
            {
                openFile.FileName = "";
            }
            openFile.Title = "Укажите программу запуска для файла";
            openFile.Filter = "Исполняемые файлы|*.exe";
            if (openFile.ShowDialog() == DialogResult.OK)
            {
                СобственнаяПрограммаЗапуска.Text = openFile.FileName;
            }

            openFile.Dispose();
        }

        private void ВыбратьСобственнуюПрограмму_Click(object sender, EventArgs e)
        {
            ВыборСобственнойПрограммы();
        }

        private void СобственнаяПрограммаЗапуска_MouseHover(object sender, EventArgs e)
        {
            ПоказатьИнформацию("Собственная программа, с помощью которой будет запускаться файл");
        }

        private void СобственнаяПрограммаЗапуска_MouseLeave(object sender, EventArgs e)
        {
            ОчиститьИнформацию();
        }

        private void ВыбратьСобственнуюПрограмму_MouseHover(object sender, EventArgs e)
        {
            ПоказатьИнформацию("Выбрать собственную программу запуска для данного файла");
        }

        private void ВыбратьСобственнуюПрограмму_MouseLeave(object sender, EventArgs e)
        {
            ОчиститьИнформацию();
        }

        private void Группа_Click(object sender, EventArgs e)
        {
            НачатьВыборГруппы();
        }

        private void ФормаБазыДанных_Shown(object sender, EventArgs e)
        {
            Наименование.Select();
        }

        private void Путь_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyValue == 115)
            {                
                ВыборПути();
            }
        }

        private void СобственнаяПрограммаЗапуска_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyValue == 115)
            {                
                ВыборСобственнойПрограммы();
            }
        }

        private void ФормаБазыДанных_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyValue == 13)
            {
                if ((e.Modifiers & Keys.Control) != 0)
                    КнопкаОК_Click(sender, e);
            }
        }

        private void Путь_TextChanged(object sender, EventArgs e)
        {
            УправлениеВидимостью();
        }

        private void Наименование_MouseHover(object sender, EventArgs e)
        {
            ПоказатьИнформацию("Наименование для внешнего приложения (файла)");
        }

        private void Наименование_MouseLeave(object sender, EventArgs e)
        {
            ОчиститьИнформацию();
        }

        private void ПоказыватьВМенюЗапуска_MouseHover(object sender, EventArgs e)
        {
            ПоказатьИнформацию("Признак того, что приложение (файл) будет показываться в меню запуска");
        }

        private void ПоказыватьВМенюЗапуска_MouseLeave(object sender, EventArgs e)
        {
            ОчиститьИнформацию();
        }        
    }
}
