using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using MainStruct;

namespace TestMenuPopup
{
    public partial class ФормаГруппы : Form
    {
        public СтруктураНастроекГруппыЭлементов ТекущаяБаза;
        
        public ФормаГруппы()
        {
            InitializeComponent();
        }

        public void СоздатьГруппу()
        {
            Text = "Создание группы";
        }

        public void ОткрытьГруппу(XmlNode УзелГруппы)
        {
            Text = "Редактирование группы";
            Наименование.Text = ГлавноеОкно.ПолучитьАтрибутУзла(УзелГруппы, "Наименование");
            Описание.Text = ГлавноеОкно.ПолучитьАтрибутУзла(УзелГруппы, "Описание");
            СочетаниеКлавиш.ЗаполнитьСочетаниеКлавиш(ГлавноеОкно.ПолучитьАтрибутУзла(УзелГруппы, "СочетаниеКлавиш"));
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (Наименование.Text == "")
            {
                ГлавноеОкно.ПоказатьИнфомационноеСообщение("Не указано наименование группы", this);                
                return;
            }
            DialogResult = DialogResult.OK;
            ТекущаяБаза.Наименование = Наименование.Text;
            ТекущаяБаза.Описание = Описание.Text;
            ТекущаяБаза.ГруппаРаскрыта = true;
            if (СочетаниеКлавиш.ГорячаяКлавишаВыбрана)
            {
                ТекущаяБаза.СочетаниеКлавиш = Convert.ToString(СочетаниеКлавиш.КодПервогоСимвола) + "\\" + Convert.ToString(СочетаниеКлавиш.КодВторогоСимвола) + "\\" + Convert.ToString(СочетаниеКлавиш.КодТретьегоСимвола);
            }
            else
                ТекущаяБаза.СочетаниеКлавиш = "0\\0\\0";

            Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void ФормаГруппы_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)27)
            {
                Close();
            }
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

        private void Наименование_MouseHover(object sender, EventArgs e)
        {
            ПоказатьИнформацию("Наименование для группы баз данных");
        }

        private void Наименование_MouseLeave(object sender, EventArgs e)
        {
            ОчиститьИнформацию();
        }

        private void Описание_MouseHover(object sender, EventArgs e)
        {
            ПоказатьИнформацию("Дополнительное описание группы");
        }

        private void Описание_MouseLeave(object sender, EventArgs e)
        {
            ОчиститьИнформацию();
        }

        private void СочетаниеКлавиш_MouseHover(object sender, EventArgs e)
        {
            ПоказатьИнформацию("Сочетание клавиш для группы баз");
        }

        private void СочетаниеКлавиш_MouseLeave(object sender, EventArgs e)
        {
            ОчиститьИнформацию();
        }

        private void КнопкаОК_MouseHover(object sender, EventArgs e)
        {
            ПоказатьИнформацию("Сохранить настройки");
        }

        private void КнопкаОтмена_MouseHover(object sender, EventArgs e)
        {
            ПоказатьИнформацию("Отказаться от редактирования");
        }

        private void КнопкаОтмена_MouseLeave(object sender, EventArgs e)
        {
            ОчиститьИнформацию();
        }

        private void КнопкаОК_MouseLeave(object sender, EventArgs e)
        {
            ОчиститьИнформацию();
        }

        private void ФормаГруппы_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyValue == 13)
            {
                if ((e.Modifiers & Keys.Control) != 0)
                    button1_Click(sender, e);
            }
        }
    }
}