using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Microsoft.Win32;
using System.IO;
using System.Xml;
using MainStruct;

namespace TestMenuPopup
{
    public partial class ФормаПодбораБазДанных : Form
    {
        private int ТекущийТипПлатформы;
        private string ИмяПользователяПоУмолчанию = ГлавноеОкно.ПолучитьЗначениеНастройки("ИмяПользователяПоУмолчанию", "");
        private String ПарольПользователяПоУмолчанию = ГлавноеОкно.ПолучитьЗначениеНастройки("ПарольПользователяПоУмолчанию", "");
        private XmlNode ГруппаПоУмолчанию;
        private Boolean ДанныеИзменены;

        public ФормаПодбораБазДанных()
        {
            InitializeComponent();            
        }

        // Процедура формирует начальные настройки для базы данных
        //
        private СтруктураНастроекЭлемента ЗаполнитьНачальныеНастройкиБазы(string НаименованиеБазы, string ПутьБазы, Boolean БазаДанныхСерверная)
        {
            СтруктураНастроекЭлемента НастройкиБазыДанных = new СтруктураНастроекЭлемента();
            НастройкиБазыДанных.Группа = ГруппаПоУмолчанию;
            НастройкиБазыДанных.Наименование = НаименованиеБазы;
            НастройкиБазыДанных.Путь = ПутьБазы;
            НастройкиБазыДанных.ПоказыватьВМенюЗапуска = true;
            НастройкиБазыДанных.РежимЗапуска = 0;
            НастройкиБазыДанных.РежимРаботы = 0;
            if (БазаДанныхСерверная)
                НастройкиБазыДанных.ТипБазы = 1;
            else
                НастройкиБазыДанных.ТипБазы = 0;
            НастройкиБазыДанных.ТипПлатформы = ТекущийТипПлатформы;
            НастройкиБазыДанных.ИмяПользователя = ИмяПользователяПоУмолчанию;
            НастройкиБазыДанных.Пароль = ПарольПользователяПоУмолчанию;
            НастройкиБазыДанных.ДополнительныеПользователи = new ListView();
            НастройкиБазыДанных.ВидКлиента = 0;
            НастройкиБазыДанных.ВидКлиентаКакПунктМеню = false;

            return НастройкиБазыДанных;
        }

        // Процедура получает список баз 7.7
        //
        private void ПолучитьСписокБаз77()
        {
            string РазделСБазами77 = @"Software\1C\1Cv7\7.7\Titles";
            RegistryKey РазделДляЧтения =  Registry.CurrentUser.OpenSubKey(РазделСБазами77);
            if (РазделДляЧтения != null)
            {
                string[] ПутиБазДанных = РазделДляЧтения.GetValueNames();
                for (int i = 0; i < ПутиБазДанных.Length; i++)
                {
                    string ПутьБазыДанных = ПутиБазДанных[i];
                    string ИмяБазыДанных = (string)РазделДляЧтения.GetValue(ПутьБазыДанных);
                    ListViewItem НоваяСтрокаСписка = СписокЗарегистрированныхБаз.Items.Add(ИмяБазыДанных);
                    НоваяСтрокаСписка.SubItems.Add(ПутьБазыДанных);
                    НоваяСтрокаСписка.ImageIndex = ТекущийТипПлатформы;
                    НоваяСтрокаСписка.Tag = ЗаполнитьНачальныеНастройкиБазы(ИмяБазыДанных, ПутьБазыДанных, false);
                }
            }
            else
                ГлавноеОкно.ПоказатьИнфомационноеСообщение("Не удалось найти зарегистрированных баз 1С Предприятия 7.7", this);
        }

        // Процедура получает список баз 8.0, 8.1 и 8.2
        //
        private void ПолучитьСписокБаз8()
        {
            string ФайлСБазами;
            String СообщениеОбОшибке;
            if (ТекущийТипПлатформы == 1)
            {
                ФайлСБазами = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\1C\\1Cv80\\ibases.v8i";
                СообщениеОбОшибке = "Не удалось найти зарегистрированных баз 1С Предприятия 8.0";
            }
            else if (ТекущийТипПлатформы == 2)
            {
                ФайлСБазами = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\1C\\1Cv81\\ibases.v8i";
                СообщениеОбОшибке = "Не удалось найти зарегистрированных баз 1С Предприятия 8.1";
            }
            else
            {
                ФайлСБазами = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\1C\\1CEStart\\ibases.v8i";
                СообщениеОбОшибке = "Не удалось найти зарегистрированных баз 1С Предприятия 8.2";
            }
            if (File.Exists(ФайлСБазами))
            {
                // Читаем файл с базами
                StreamReader ЧтениеФайла = new StreamReader(ФайлСБазами);
                ListBox СписокСтрокФайла = new ListBox();
                while (!ЧтениеФайла.EndOfStream)
                {
                    // Заносим все строки из файла в ListBox для более удобной обработки
                    СписокСтрокФайла.Items.Add(ЧтениеФайла.ReadLine());
                }
                int КоличествоСтрокВФале = СписокСтрокФайла.Items.Count;
                if (КоличествоСтрокВФале != 0)
                {
                    // Обрабатываем список строк
                    for (int i = 0; i < КоличествоСтрокВФале; i++)
                    {
                        string СтрокаФайла = (string)СписокСтрокФайла.Items[i];
                        // Наименование базы и папки должно начинаться с символа "["
                        if (СтрокаФайла[0] == (char)91)
                        {
                            string НаименованиеБазы = СтрокаФайла.Substring(1, СтрокаФайла.Length - 2);
                            String ПутьБазы = (string)СписокСтрокФайла.Items[i + 1];
                            if (!ПутьБазы.Contains("Connect"))
                            {
                                // Папки пропускаем
                                continue;
                            }

                            Boolean БазаДанныхСерверная = !ПутьБазы.Contains("Connect=File");

                            ПутьБазы = ПутьБазы.Replace("Connect", "");
                            ПутьБазы = ПутьБазы.Replace("=", "");
                            ПутьБазы = ПутьБазы.Replace("\"", "");
                            if (БазаДанныхСерверная)
                            {
                                string[] ПараметрыБазы = ПутьБазы.Split(';');                                
                                ПутьБазы = ПараметрыБазы[0].Replace("Srvr", "") + "/" + ПараметрыБазы[1].Replace("Ref", "");
                            }
                            else
                            {
                                ПутьБазы = ПутьБазы.Replace("File", "");
                                ПутьБазы = ПутьБазы.Replace(";", "");
                            }

                            ListViewItem НоваяСтрокаСписка = СписокЗарегистрированныхБаз.Items.Add(НаименованиеБазы);
                            НоваяСтрокаСписка.SubItems.Add(ПутьБазы);
                            НоваяСтрокаСписка.ImageIndex = ТекущийТипПлатформы;
                            НоваяСтрокаСписка.Tag = ЗаполнитьНачальныеНастройкиБазы(НаименованиеБазы, ПутьБазы, БазаДанныхСерверная);
                        }
                    }
                }
            }
            else
                ГлавноеОкно.ПоказатьИнфомационноеСообщение(СообщениеОбОшибке, this);
        }


        private void ПолучитьСписокБаз82()
        {
        }

        private void ЗаполнитьСписокЗарегистрированныхБаз()
        {
            СписокЗарегистрированныхБаз.Items.Clear();
            if (ТекущийТипПлатформы == 0)
                ПолучитьСписокБаз77();
            else 
                ПолучитьСписокБаз8();
        }

        private void ВывестиНаименованиеПлатформы()
        {
            ВыборТипаПлатформы.Text = ГлавноеОкно.ПолучитьПредставлениеПлатформы(ТекущийТипПлатформы);
            ВыборТипаПлатформы.Image = ГлавноеОкно.ПолучитьКартинкуПлатформы(ТекущийТипПлатформы);
            ЗаполнитьСписокЗарегистрированныхБаз();
        }

        public void ИнициализацияПодбора(XmlNode Группа)
        {
            ГруппаПоУмолчанию = Группа;
            ДанныеИзменены = false;
            ТекущийТипПлатформы = Convert.ToInt16(ГлавноеОкно.ПолучитьЗначениеНастройки("ТипПлатформыПоУмолчанию", "2"));
            ВывестиНаименованиеПлатформы();
        }

        private void ВыборТипаПлатформы_Click(object sender, EventArgs e)
        {
            МенюВыбораПлатформы.Show(MousePosition.X, MousePosition.Y);
        }

        private void сПредприятие77ToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            ТекущийТипПлатформы = 0;
            ВывестиНаименованиеПлатформы();
        }

        private void сПредприятие80ToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            ТекущийТипПлатформы = 1;
            ВывестиНаименованиеПлатформы();
        }

        private void сПредприятие81ToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            ТекущийТипПлатформы = 2;
            ВывестиНаименованиеПлатформы();
        }

        private void сПредприятие82ToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            ТекущийТипПлатформы = 3;
            ВывестиНаименованиеПлатформы();
        }

        private void ПеренестиБазуВДобавляемые(ListViewItem КопируемаяБаза)
        {
            if (КопируемаяБаза == null)            
                КопируемаяБаза = (ListViewItem)СписокЗарегистрированныхБаз.SelectedItems[0].Clone();
            else
                КопируемаяБаза = (ListViewItem)КопируемаяБаза.Clone();
            
            string ПутьБазы = КопируемаяБаза.SubItems[1].Text;
            // Ищем, может база уже добавлена
            if (СписокДобавляемыхБаз.FindItemWithText(ПутьБазы) == null)
            {
                КопируемаяБаза.SubItems.Add(ГлавноеОкно.ПолучитьАтрибутУзла(ГруппаПоУмолчанию, "Наименование"));
                СписокДобавляемыхБаз.Items.Add(КопируемаяБаза);
                ДанныеИзменены = true;
            }            
        }

        private void СписокЗарегистрированныхБаз_DoubleClick(object sender, EventArgs e)
        {
            ПеренестиБазуВДобавляемые(null);            
        }

        private void РедактироватьПараметрыБазыДанных()
        {
            if (СписокДобавляемыхБаз.SelectedItems.Count != 0)
            {
                ListViewItem ТекущаяБаза = СписокДобавляемыхБаз.SelectedItems[0];
                ФормаБазыДанных ФормаБазы = new ФормаБазыДанных();
                ФормаБазы.ОткрытьБазуДанныхВРежимеПодбора((СтруктураНастроекЭлемента)ТекущаяБаза.Tag);
                if (ФормаБазы.ShowDialog() == DialogResult.OK)
                {
                    СтруктураНастроекЭлемента НастройкиБазы = ФормаБазы.ТекущаяНастройка;
                    ТекущаяБаза.Text = НастройкиБазы.Наименование;
                    if (НастройкиБазы.Группа != ГруппаПоУмолчанию)
                        ТекущаяБаза.SubItems[2].Text = ГлавноеОкно.ПолучитьАтрибутУзла(НастройкиБазы.Группа, "Наименование");
                    ТекущаяБаза.Tag = НастройкиБазы;
                }
            }
        }

        private void СписокДобавляемыхБаз_DoubleClick(object sender, EventArgs e)
        {
            РедактироватьПараметрыБазыДанных();
        }

        private void ПоказатьИнформацию(string Текст)
        {
            Статус.Items[0].Text = "       " + Текст;
        }

        private void ОчиститьИнформацию()
        {
            Статус.Items[0].Text = "";
        }

        private void Подсказка_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.DrawImage(Properties.Resources.smallinfo, 1, 1);
        }

        private void СписокЗарегистрированныхБаз_MouseHover(object sender, EventArgs e)
        {
            ПоказатьИнформацию("Список зарегистрированных в системе баз данных (" + ГлавноеОкно.ПолучитьПредставлениеПлатформы(ТекущийТипПлатформы) + ")");
        }

        private void СписокЗарегистрированныхБаз_MouseLeave(object sender, EventArgs e)
        {
            ОчиститьИнформацию();
        }

        private void СписокДобавляемыхБаз_MouseHover(object sender, EventArgs e)
        {
            ПоказатьИнформацию("Список баз для добавления в настройки программы");
        }

        private void СписокДобавляемыхБаз_MouseLeave(object sender, EventArgs e)
        {
            ОчиститьИнформацию();
        }

        private void КнопкаОК_MouseLeave(object sender, EventArgs e)
        {
            ОчиститьИнформацию();
        }

        private void КнопкаОК_MouseHover(object sender, EventArgs e)
        {
            ПоказатьИнформацию("Закончить подбор баз данных");
        }

        private void КнопкаОтмена_MouseHover(object sender, EventArgs e)
        {
            ПоказатьИнформацию("Отказаться от подбора баз данных");
        }

        private void КнопкаОтмена_MouseLeave(object sender, EventArgs e)
        {
            ОчиститьИнформацию();
        }

        private void ВыборТипаПлатформы_MouseHover(object sender, EventArgs e)
        {
            ПоказатьИнформацию("Выбрать тип платформы баз данных для подбора");
        }

        private void ВыборТипаПлатформы_MouseLeave(object sender, EventArgs e)
        {
            ОчиститьИнформацию();
        }

        private void ПриЗакрытии()
        {
            if (ДанныеИзменены)
            {
                if (ГлавноеОкно.Вопрос("Выйти без сохранения?", this) == DialogResult.No)
                    return;
            }
            Close();
        }

        private void КнопкаОтмена_Click(object sender, EventArgs e)
        {
            ПриЗакрытии();
        }

        private void КнопкаОК_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
            Close();
        }

        private void КопироватьВсе_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < СписокЗарегистрированныхБаз.Items.Count; i++)
            {
                ListViewItem КопируемаяБаза = (ListViewItem)СписокЗарегистрированныхБаз.Items[i].Clone();
                string ПутьБазы = КопируемаяБаза.SubItems[1].Text;
                if (СписокДобавляемыхБаз.FindItemWithText(ПутьБазы) == null)
                {
                    КопируемаяБаза.SubItems.Add("Группа баз данных");
                    СписокДобавляемыхБаз.Items.Add(КопируемаяБаза);
                }
                ДанныеИзменены = true;
            }
        }

        private void КопироватьВсе_MouseHover(object sender, EventArgs e)
        {
            ПоказатьИнформацию("Копировать все зарегистрированные базы данных в список добавляемых");
        }

        private void КопироватьВсе_MouseLeave(object sender, EventArgs e)
        {
            ОчиститьИнформацию();
        }

        private void УстановитьГруппу_MouseHover(object sender, EventArgs e)
        {
            ПоказатьИнформацию("Установить группу для всех выбранных баз");
        }

        private void УстановитьГруппу_MouseLeave(object sender, EventArgs e)
        {
            ОчиститьИнформацию();
        }

        private void УстановитьГруппу_Click(object sender, EventArgs e)
        {
            if (СписокДобавляемыхБаз.Items.Count != 0)
            {
                Boolean ГруппаДляВсех = false;
                String ТекстВопроса = String.Empty;
                if (СписокДобавляемыхБаз.SelectedItems.Count == 0)
                {
                    ГруппаДляВсех = true;
                    ТекстВопроса = "Установить группу для всех баз данных?";
                }
                else
                    ТекстВопроса = "Установить группу для выбранных баз данных?";

                if (ГлавноеОкно.Вопрос(ТекстВопроса, this) == DialogResult.No)
                    return;

                ФормаВыбораГруппы ФормаВыбора = new ФормаВыбораГруппы();
                ФормаВыбора.ОткрытьВыборГрупп(ГруппаПоУмолчанию);
                if (ФормаВыбора.ShowDialog() == DialogResult.OK)
                {
                    XmlNode ВыбраннаяГруппа = ФормаВыбора.ВыбраннаяГруппа;
                    string НаименованиеГруппы;
                    if (ВыбраннаяГруппа != ГруппаПоУмолчанию)
                        НаименованиеГруппы = ГлавноеОкно.ПолучитьАтрибутУзла(ВыбраннаяГруппа, "Наименование");
                    else
                        НаименованиеГруппы = "Группа баз данных";

                    if (ГруппаДляВсех)
                    {
                        for (int i = 0; i < СписокДобавляемыхБаз.Items.Count; i++)
                        {
                            ListViewItem ТекущаяСтрокаСписка = СписокДобавляемыхБаз.Items[i];
                            СтруктураНастроекЭлемента НастройкаБазы = (СтруктураНастроекЭлемента)ТекущаяСтрокаСписка.Tag;
                            НастройкаБазы.Группа = ВыбраннаяГруппа;
                            ТекущаяСтрокаСписка.Tag = НастройкаБазы;
                            ТекущаяСтрокаСписка.SubItems[2].Text = НаименованиеГруппы;
                        }
                    }
                    else
                    {
                        for (int i = 0; i < СписокДобавляемыхБаз.SelectedItems.Count; i++)
                        {
                            ListViewItem ТекущаяСтрокаСписка = СписокДобавляемыхБаз.SelectedItems[i];
                            СтруктураНастроекЭлемента НастройкаБазы = (СтруктураНастроекЭлемента)ТекущаяСтрокаСписка.Tag;
                            НастройкаБазы.Группа = ВыбраннаяГруппа;
                            ТекущаяСтрокаСписка.Tag = НастройкаБазы;
                            ТекущаяСтрокаСписка.SubItems[2].Text = НаименованиеГруппы;
                        }
                    }
                    ДанныеИзменены = true;
                }
            }
        }

        private void ОчиститьСписок_MouseHover(object sender, EventArgs e)
        {
            ПоказатьИнформацию("Очистить список добавляемых баз данных");
        }

        private void ОчиститьСписок_MouseLeave(object sender, EventArgs e)
        {
            ОчиститьИнформацию();
        }

        private void ОчиститьСписок_Click(object sender, EventArgs e)
        {
            if (СписокДобавляемыхБаз.Items.Count != 0)
            {
                if (ГлавноеОкно.Вопрос("Очистить список добавляемых баз?", this) == DialogResult.Yes)                
                {
                    СписокДобавляемыхБаз.Items.Clear();
                    ДанныеИзменены = true;
                }
            }
        }

        private void УдалитьБазуДанныхИзСписка()
        {
            if (СписокДобавляемыхБаз.SelectedItems.Count != 0)
            {
                string ТекстВопроса = String.Empty;
                if (СписокДобавляемыхБаз.SelectedItems.Count == 1)                
                    ТекстВопроса = "Удалить текущую базу из списка?";
                else
                    ТекстВопроса = "Удалить выбранные базы данных?";

                if (ГлавноеОкно.Вопрос(ТекстВопроса, this) == DialogResult.No)
                    return;

                if (СписокДобавляемыхБаз.SelectedItems.Count == 1)
                    СписокДобавляемыхБаз.Items.Remove(СписокДобавляемыхБаз.SelectedItems[0]);
                else
                {
                    int i = 0;
                    while (i < СписокДобавляемыхБаз.SelectedItems.Count)
                    {
                        СписокДобавляемыхБаз.Items.Remove(СписокДобавляемыхБаз.SelectedItems[i]);                        
                    }
                }

                if (СписокДобавляемыхБаз.Items.Count != 0)
                    СписокДобавляемыхБаз.Items[СписокДобавляемыхБаз.Items.Count-1].Selected = true;
                ДанныеИзменены = true;
            }
        }

        private void СписокДобавляемыхБаз_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyValue == 46)
                УдалитьБазуДанныхИзСписка();
        }

        private void редактироватьПараметрыToolStripMenuItem_Click(object sender, EventArgs e)
        {
            РедактироватьПараметрыБазыДанных();
        }

        private void удалитьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            УдалитьБазуДанныхИзСписка();
        }

        private void редактироватьПараметрыToolStripMenuItem_MouseHover(object sender, EventArgs e)
        {
            ПоказатьИнформацию("Редактировать параметры добаляемой базы данных");
        }

        private void редактироватьПараметрыToolStripMenuItem_MouseLeave(object sender, EventArgs e)
        {
            ОчиститьИнформацию();
        }

        private void удалитьToolStripMenuItem_MouseHover(object sender, EventArgs e)
        {
            ПоказатьИнформацию("Удалить выбранную базу данных из списка добавляемых");
        }

        private void удалитьToolStripMenuItem_MouseLeave(object sender, EventArgs e)
        {
            ОчиститьИнформацию();
        }

        private void установитьГруппуToolStripMenuItem_MouseHover(object sender, EventArgs e)
        {
            ПоказатьИнформацию("Установить группу для выбранной базы данных");
        }

        private void установитьГруппуToolStripMenuItem_MouseLeave(object sender, EventArgs e)
        {
            ОчиститьИнформацию();
        }

        private void установитьГруппуToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (СписокДобавляемыхБаз.SelectedItems.Count != 0)
            {
                ListViewItem ВыбраннаяСтрока = СписокДобавляемыхБаз.SelectedItems[0];
                СтруктураНастроекЭлемента НастройкаВыбраннойБазы = (СтруктураНастроекЭлемента)ВыбраннаяСтрока.Tag;
                
                ФормаВыбораГруппы ФормаВыбора = new ФормаВыбораГруппы();
                ФормаВыбора.ОткрытьВыборГрупп(НастройкаВыбраннойБазы.Группа);
                if (ФормаВыбора.ShowDialog() == DialogResult.OK)
                {
                    XmlNode ВыбраннаяГруппа = ФормаВыбора.ВыбраннаяГруппа;                    
                    if (ВыбраннаяГруппа != НастройкаВыбраннойБазы.Группа)
                    {
                        string НаименованиеГруппы = ГлавноеОкно.ПолучитьАтрибутУзла(ВыбраннаяГруппа, "Наименование");
                        НастройкаВыбраннойБазы.Группа = ВыбраннаяГруппа;
                        ВыбраннаяСтрока.Tag = НастройкаВыбраннойБазы;
                        ВыбраннаяСтрока.SubItems[2].Text = НаименованиеГруппы;
                        ДанныеИзменены = true;
                    }                                           
                }
            }
        }

        private void СписокЗарегистрированныхБаз_ItemDrag(object sender, ItemDragEventArgs e)
        {
            if (СписокЗарегистрированныхБаз.SelectedItems.Count != 0)
            {
                ListViewItem ВыбраннаяСтрока = СписокЗарегистрированныхБаз.SelectedItems[0];
                СписокЗарегистрированныхБаз.DoDragDrop(ВыбраннаяСтрока, DragDropEffects.All);
            }
        }

        private void СписокДобавляемыхБаз_DragDrop(object sender, DragEventArgs e)
        {
            ListViewItem ПеретаскиваемаяСтрока = (ListViewItem)e.Data.GetData(typeof(ListViewItem));
            ПеренестиБазуВДобавляемые(ПеретаскиваемаяСтрока);
        }

        private void СписокДобавляемыхБаз_DragEnter(object sender, DragEventArgs e)
        {
            e.Effect = DragDropEffects.All;
        }

        private void ФормаПодбораБазДанных_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == 27)
            {
                ПриЗакрытии();
            }

        }

        private void СписокЗарегистрированныхБаз_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == 13)
                if (СписокЗарегистрированныхБаз.SelectedItems.Count != 0)
                    ПеренестиБазуВДобавляемые(СписокЗарегистрированныхБаз.SelectedItems[0]);
        }

        private void ФормаПодбораБазДанных_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyValue == 13)
            {
                if ((e.Modifiers & Keys.Control) != 0)
                    КнопкаОК_Click(sender, e);
            }
        }

        
    }
}
