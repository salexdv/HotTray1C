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
using installer1C;

namespace TestMenuPopup
{
    public partial class ФормаБазыДанных : Form
    {                
        public СтруктураНастроекЭлемента ТекущаяНастройка;
        public XmlNode СсылкаБазыДанных;
        public XmlNode РодительБазыДанных;        
        private bool РежимПодбораБазДанных = false;
        private bool ПроверятьНаличиеБаз = false;
        private string ПапкаСБазами = String.Empty;
        private ListView глДопПользователи = new ListView();
        private ListView глПервоначальныеПользователи = new ListView();        

        public ФормаБазыДанных()
        {            
            InitializeComponent();
            ЗаполнитьСписокРежимовЗапуска();
            ПроверятьНаличиеБаз = Convert.ToBoolean(ГлавноеОкно.ПолучитьЗначениеНастройки("ПроверятьНаличиеБазыПриДобавлении", "false"));
            ПапкаСБазами = ГлавноеОкно.ПолучитьЗначениеНастройки("ПапкаСБазамиПоУмолчанию", "");
            ПанельДополнительныхПользователей.Visible = false;
        }

        private void ПолучитьДопПользователей(XmlNode УзелXML)
        {
            глПервоначальныеПользователи = ГлавноеОкно.ПолучитьСписокДополнительныхПользователей(УзелXML);
        }

        private void ЗаполнитьДопПользователей()
        {
            ДополнительныеПользователи.Items.Clear();
            СкопироватьListView(ref глПервоначальныеПользователи, ref ДополнительныеПользователи);            
        }

        private void СкопироватьListView(ref ListView Откуда, ref ListView Куда)
        {
            Куда.Items.Clear();
            foreach (ListViewItem Элемент in Откуда.Items)
            {
                ListViewItem Копия = Куда.Items.Add((ListViewItem)Элемент.Clone());
                if (Куда == ДополнительныеПользователи)
                    Копия.SubItems[1].Text = "**********";
            }
        }

        // Процедура получает список пользователей 77
        //
        private void ЗаполнитьСписокПользователей77()
        {
            ИмяПользователя.Items.Clear();
            
            if (ТипПлатформы.SelectedIndex == 0)
            {
                // Пробуем найти файл с пользователями
                string КаталогБазы = Путь.Text;
                if (Directory.Exists(КаталогБазы))
                {
                    if (КаталогБазы[КаталогБазы.Length-1] == '\\')
                    {
                        КаталогБазы = КаталогБазы.Remove(КаталогБазы.Length - 1, 1);
                    }

                    string ФайлПользователей = String.Empty;
                    
                    if (File.Exists(КаталогБазы + "\\usrdef\\users.usr"))
                        ФайлПользователей = КаталогБазы + "\\usrdef\\users.usr";
                    else if (File.Exists(КаталогБазы + "\\users.usr"))
                        ФайлПользователей = КаталогБазы + "\\users.usr";

                    if (!String.IsNullOrEmpty(ФайлПользователей))
                    {
                        try
                        {                                                        
                            List<String> Users = DBUsers.GetUsers(ФайлПользователей);
                            foreach (string User in Users)
                            {
                                if (!String.IsNullOrEmpty(User))
                                    ИмяПользователя.Items.Add(User);
                            }                            
                        }
                        catch
                        { 
                        }
                    }                    
                }
            }
        }

        // Процедура управляет видимостью и доступностью всех элементов в форме
        //
        private void УправлениеВидимостью()
        {
            if (РежимПодбораБазДанных)
            {
                Путь.Enabled = false;
                ВыбратьКаталогБД.Enabled = false;
                ТипБазыДанных.Enabled = false;
                ТипПлатформы.Enabled = false;
                СсылкаДополнительныеПользователи.Enabled = false;
            }

            ВидКлиента.Visible = (ТипПлатформы.SelectedIndex != 0);
            НадписьВидКлиента.Visible = (ТипПлатформы.SelectedIndex != 0);
            ВидКлиентаКакПунктМеню.Visible = (ТипПлатформы.SelectedIndex != 0);
            ВидКлиентаКакПунктМеню.Enabled = (ТипПлатформы.SelectedIndex > 2);
            ВидКлиента.Enabled = (ТипПлатформы.SelectedIndex > 2);            
            НадписьВидКлиента.Enabled = (ТипПлатформы.SelectedIndex > 2);
            РежимРаботы.Visible = (ТипПлатформы.SelectedIndex == 0);
            ТекстРежимРаботы.Visible = (ТипПлатформы.SelectedIndex == 0);

            if (ТипПлатформы.SelectedIndex > 2)
            {
                if (ВидКлиентаКакПунктМеню.Checked)
                {
                    ВидКлиента.Enabled = false;
                    НадписьВидКлиента.Enabled = false;
                }
                else
                {
                    ВидКлиента.Enabled = true;
                    НадписьВидКлиента.Enabled = true;
                }
            }
            
            КодДоступа.Enabled = ТекстКодДоступа.Enabled = (ТипПлатформы.SelectedIndex != 0);

            ИспользуетсяАутентификацияWindows.Enabled = (ТипПлатформы.SelectedIndex != 0);
            ИмяПользователя.Enabled = !ИспользуетсяАутентификацияWindows.Checked;
            Пароль.Enabled = !ИспользуетсяАутентификацияWindows.Checked;                        
            
            if ((ТипБазыДанных.SelectedIndex == 0) | (ТипПлатформы.SelectedIndex == 0))
            {
                ТекстПутьКБазе.Text = "Каталог базы данных:";
                ВыбратьКаталогБД.Enabled = true;
            }
            else
            {
                ТекстПутьКБазе.Text = "Сервер базы данных/Имя базы на сервере"; 
                ВыбратьКаталогБД.Enabled = false;
            }

            if (РежимЗапускаКакПунктМеню.Checked)
            {
                РежимРаботы.Enabled = false;
                РежимЗапуска.Enabled = false;
            }
            else
            {
                РежимЗапуска.Enabled = true;
                if (ТипПлатформы.SelectedIndex == 0)                
                    РежимРаботы.Enabled = true;                
                else
                    РежимРаботы.Enabled = false;
            }
            
            ВерсияПлатформы.Enabled = ТекстВерсия.Enabled = (ТипПлатформы.SelectedIndex > 2);
            
            СсылкаПараметрыХранилища.Enabled = (ТипПлатформы.SelectedIndex != 0);
        }

        public void СоздатьБазуДанных(XmlNode РодительБазы)
        {
            РодительБазыДанных = РодительБазы;
            Группа.Items.Add(ГлавноеОкно.ПолучитьАтрибутУзла(РодительБазы, "Наименование"));
            Группа.SelectedIndex = 0;
            Text = "Добавление базы данных";
            ИмяПользователя.Text = ГлавноеОкно.ПолучитьЗначениеНастройки("ИмяПользователяПоУмолчанию", "");
            Пароль.Text = ГлавноеОкно.Шифрование(ГлавноеОкно.ПолучитьЗначениеНастройки("ПарольПользователяПоУмолчанию", ""));
            ТипБазыДанных.SelectedIndex = 0;
            ТипПлатформы.SelectedIndex = Convert.ToInt16(ГлавноеОкно.ПолучитьЗначениеНастройки("ТипПлатформыПоУмолчанию", "2"));
            РежимЗапуска.SelectedIndex = Convert.ToInt16(ГлавноеОкно.ПолучитьЗначениеНастройки("РежимЗапускаПоУмолчанию", "0")); ;
            РежимРаботы.SelectedIndex = 0;
            ВидКлиента.SelectedIndex = 0;
            ПоказыватьВМенюЗапуска.Checked = true;            
            РежимЗапускаКакПунктМеню.Checked = false;
            ВидКлиентаКакПунктМеню.Checked = false;
            УправлениеВидимостью();
            ПанельПараметровХранилища.Visible = false;
        }

        public void ОткрытьБазуДанных(XmlNode РодительБазы, XmlNode УзелБазыДанных, bool Копирование)
        {
            РодительБазыДанных = РодительБазы;
            СсылкаБазыДанных = УзелБазыДанных;            
            Наименование.Text = ГлавноеОкно.ПолучитьАтрибутУзла(УзелБазыДанных, "Наименование");
            Пароль.Text = ГлавноеОкно.Шифрование(ГлавноеОкно.ПолучитьАтрибутУзла(УзелБазыДанных, "ПарольПользователя"));
            ТипБазыДанных.SelectedIndex = Convert.ToInt16(ГлавноеОкно.ПолучитьАтрибутУзла(УзелБазыДанных, "ТипБазы"));
            ТипПлатформы.SelectedIndex = Convert.ToInt16(ГлавноеОкно.ПолучитьАтрибутУзла(УзелБазыДанных, "ТипПлатформы"));
            Путь.Text = ГлавноеОкно.ПолучитьАтрибутУзла(УзелБазыДанных, "Путь");
            ЗаполнитьСписокПользователей77();
            ИспользуетсяАутентификацияWindows.Checked = Convert.ToBoolean(ГлавноеОкно.ПолучитьАтрибутУзла(УзелБазыДанных, "ИспользуетсяАутентификацияWindows"));
            ИмяПользователя.Text = ГлавноеОкно.ПолучитьАтрибутУзла(УзелБазыДанных, "ИмяПользователя");
            ПоказыватьВМенюЗапуска.Checked = Convert.ToBoolean(ГлавноеОкно.ПолучитьАтрибутУзла(УзелБазыДанных, "ПоказыватьВМенюЗапуска"));
            РежимРаботы.SelectedIndex = Convert.ToInt16(ГлавноеОкно.ПолучитьАтрибутУзла(УзелБазыДанных, "РежимРаботы"));
            РежимЗапуска.SelectedIndex = Convert.ToInt16(ГлавноеОкно.ПолучитьАтрибутУзла(УзелБазыДанных, "РежимЗапуска"));
            РежимЗапускаКакПунктМеню.Checked = Convert.ToBoolean(ГлавноеОкно.ПолучитьАтрибутУзла(УзелБазыДанных, "РежимЗапускаКакПунктМеню"));
            try
            {
                ВидКлиента.SelectedIndex = Convert.ToInt16(ГлавноеОкно.ПолучитьАтрибутУзла(УзелБазыДанных, "ВидКлиента"));
            }
            catch
            {
                ВидКлиента.SelectedIndex = 0;
            }
            try
            {
                ВидКлиентаКакПунктМеню.Checked = Convert.ToBoolean(ГлавноеОкно.ПолучитьАтрибутУзла(УзелБазыДанных, "ВидКлиентаКакПунктМеню"));
            }
            catch
            {
                ВидКлиентаКакПунктМеню.Checked = false;
            }

            try
            {
                КодДоступа.Text = ГлавноеОкно.Шифрование(ГлавноеОкно.ПолучитьАтрибутУзла(УзелБазыДанных, "КодДоступа"));
            }
            catch
            {
                КодДоступа.Text = "";
            }

            Описание.Text = ГлавноеОкно.ПолучитьАтрибутУзла(УзелБазыДанных, "Описание");
            
            ТекущаяНастройка.ИмяПользователя = ГлавноеОкно.ПолучитьАтрибутУзла(УзелБазыДанных, "ПутьКХранилищу");
            ТекущаяНастройка.ИмяПользователяХранилища = ГлавноеОкно.ПолучитьАтрибутУзла(УзелБазыДанных, "ИмяПользователяХранилища");
            ТекущаяНастройка.ПарольПользователяХранилища = ГлавноеОкно.Шифрование(ГлавноеОкно.ПолучитьАтрибутУзла(УзелБазыДанных, "ПарольПользователяХранилища"));

            ПутьКХранилищу.Text = ТекущаяНастройка.ИмяПользователя;
            ИмяПользователяХранилища.Text = ТекущаяНастройка.ИмяПользователяХранилища;
            ПарольПользователяХранилища.Text = ТекущаяНастройка.ПарольПользователяХранилища;       
            
            УправлениеВидимостью();
            ПанельПараметровХранилища.Visible = false;
            ПриВыбореТипаПлатформы();
            
            if (!Копирование)
            {
                СочетаниеКлавиш.ЗаполнитьСочетаниеКлавиш(ГлавноеОкно.ПолучитьАтрибутУзла(УзелБазыДанных, "СочетаниеКлавиш"));
                Text = "Редактирование базы данных";
            }
            else
                Text = "Добавление базы копированием";
            
            СобственнаяПрограммаЗапуска.Text = ГлавноеОкно.ПолучитьАтрибутУзла(УзелБазыДанных, "ПрограммаЗапуска");
            
            string version = ГлавноеОкно.ПолучитьАтрибутУзла(УзелБазыДанных, "ВерсияПлатформы");            
            if (!String.IsNullOrEmpty(version))
            {
            	foreach (object item in ВерсияПлатформы.Items)
            	{
            		if (item.ToString() == version)
            		{
            			ВерсияПлатформы.SelectedIndex = ВерсияПлатформы.Items.IndexOf(item);
            			break;
            		}
            	}
            }            
            
            ПолучитьДопПользователей(УзелБазыДанных);
            СкопироватьListView(ref глПервоначальныеПользователи, ref глДопПользователи);
            ЗаполнитьДопПользователей();
        }        

        public void ОткрытьБазуДанныхВРежимеПодбора(СтруктураНастроекЭлемента НастройкаДобавляемойБазы)
        {
            РежимПодбораБазДанных = true;
            РодительБазыДанных = НастройкаДобавляемойБазы.Группа;
            Text = "Редактирование добавляемой базы";
            Наименование.Text = НастройкаДобавляемойБазы.Наименование;
            Пароль.Text = НастройкаДобавляемойБазы.Пароль;
            ТипБазыДанных.SelectedIndex = НастройкаДобавляемойБазы.ТипБазы;
            ТипПлатформы.SelectedIndex = НастройкаДобавляемойБазы.ТипПлатформы;
            Путь.Text = НастройкаДобавляемойБазы.Путь;
            ЗаполнитьСписокПользователей77();
            ИспользуетсяАутентификацияWindows.Checked = НастройкаДобавляемойБазы.ИспользуетсяАутентификацияWindows;
            ИмяПользователя.Text = НастройкаДобавляемойБазы.ИмяПользователя;
            ПоказыватьВМенюЗапуска.Checked = НастройкаДобавляемойБазы.ПоказыватьВМенюЗапуска;
            РежимРаботы.SelectedIndex = НастройкаДобавляемойБазы.РежимРаботы;
            РежимЗапуска.SelectedIndex = НастройкаДобавляемойБазы.РежимЗапуска;
            РежимЗапускаКакПунктМеню.Checked = НастройкаДобавляемойБазы.РежимЗапускаКакПунктМеню;
            ВидКлиента.SelectedIndex = НастройкаДобавляемойБазы.ВидКлиента;
            ВидКлиентаКакПунктМеню.Checked = НастройкаДобавляемойБазы.ВидКлиентаКакПунктМеню;
            Описание.Text = НастройкаДобавляемойБазы.Описание;
            КодДоступа.Text = НастройкаДобавляемойБазы.КодДоступа;
            
            ТекущаяНастройка.ИмяПользователя = НастройкаДобавляемойБазы.ПутьКХранилищу;
            ТекущаяНастройка.ИмяПользователяХранилища = НастройкаДобавляемойБазы.ИмяПользователяХранилища;
            ТекущаяНастройка.ПарольПользователяХранилища = НастройкаДобавляемойБазы.ПарольПользователяХранилища;
            
            ПутьКХранилищу.Text = НастройкаДобавляемойБазы.ПутьКХранилищу;
            ИмяПользователяХранилища.Text = НастройкаДобавляемойБазы.ИмяПользователяХранилища;
            ПарольПользователяХранилища.Text = НастройкаДобавляемойБазы.ПарольПользователяХранилища;

            УправлениеВидимостью();
            ПанельПараметровХранилища.Visible = false;
            ПриВыбореТипаПлатформы();
            ЗаполнитьСписокПользователей77();
            СочетаниеКлавиш.ЗаполнитьСочетаниеКлавиш(НастройкаДобавляемойБазы.СочетаниеКлавиш);
            СобственнаяПрограммаЗапуска.Text = НастройкаДобавляемойБазы.ПрограммаЗапуска;            
        }

        private void ЗаполнитьСписокВерсий()
        {
            ВерсияПлатформы.Items.Clear();

            if (ТипПлатформы.SelectedIndex > 2)
            {
                int platformType = 0;
                
                switch (ТипПлатформы.SelectedIndex)
                {
                    case 3:
                        platformType = 82;
                        break;
                    case 4:
                        platformType = 83;
                        break;
                    default:
                        platformType = 83;
                        break;
                }

                List<string> versions = ГлавноеОкно.installer1C.GetPlatformVersions(platformType);
                
                ВерсияПлатформы.Items.Add("");

                foreach (string version in versions)
                {
                    ВерсияПлатформы.Items.Add(version);
                }
            }
        }

        private void ПриВыбореТипаПлатформы()
        {
            if ((ТипПлатформы.SelectedIndex == 0) && (РежимЗапуска.Items.Count == 3))
            {
                РежимЗапуска.Items.Add("Отладчик");
                РежимЗапуска.Items.Add("Монитор");
            }
            else if (ТипПлатформы.SelectedIndex != 0)
            {
                if (РежимЗапуска.Items.Count > 3)
                {
                    РежимЗапуска.Items.Remove(РежимЗапуска.Items[4]);
                    РежимЗапуска.Items.Remove(РежимЗапуска.Items[3]);
                }
            }
            if (РежимЗапуска.SelectedItem == null)
                РежимЗапуска.SelectedIndex = 0;

            ЗаполнитьСписокВерсий();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        
        /// Процедура прорисовки элементов выпадающего списка "Тип платформы"
        ///
        private void ТипПлатформы_DrawItem(object sender, DrawItemEventArgs e)
        {
            if (e.Index != -1)
            {
                e.DrawBackground();
                // Прорисовываем прямоугольник
                e.Graphics.DrawRectangle(new Pen(Color.White, 1), new Rectangle(e.Bounds.X, e.Bounds.Y, 200, 15));                
                // Рисуем справа рисунок
                e.Graphics.DrawImage(ГлавноеОкно.ПолучитьКартинкуПлатформы(e.Index), e.Bounds.X, e.Bounds.Y);
                // Выводим текст                
                e.Graphics.DrawString("     " + ТипПлатформы.Items[e.Index].ToString(), new Font(FontFamily.GenericSerif, 10, FontStyle.Regular), Brushes.Black, e.Bounds.X, e.Bounds.Y);
            }
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
                СообщениеОбОшибке = СообщениеОбОшибке + "\n -Наименование базы";
            if (String.IsNullOrEmpty(Путь.Text))
                СообщениеОбОшибке = СообщениеОбОшибке + "\n -Расположение базы";
            if (СообщениеОбОшибке.Length > 33)
            {
                ГлавноеОкно.ПоказатьИнфомационноеСообщение(СообщениеОбОшибке, this);
                return;
            }
            ТекущаяНастройка.Ссылка = СсылкаБазыДанных;
            ТекущаяНастройка.Наименование = Наименование.Text;
            ТекущаяНастройка.ИмяПользователя = ИмяПользователя.Text;
            ТекущаяНастройка.Пароль = Пароль.Text;
            ТекущаяНастройка.ИспользуетсяАутентификацияWindows = ИспользуетсяАутентификацияWindows.Checked;
            ТекущаяНастройка.Путь = Путь.Text;
            ТекущаяНастройка.ТипПлатформы = ТипПлатформы.SelectedIndex;
            ТекущаяНастройка.ТипБазы = ТипБазыДанных.SelectedIndex;
            ТекущаяНастройка.ПоказыватьВМенюЗапуска = ПоказыватьВМенюЗапуска.Checked;
            ТекущаяНастройка.РежимЗапуска = РежимЗапуска.SelectedIndex;
            ТекущаяНастройка.РежимРаботы = РежимРаботы.SelectedIndex;
            ТекущаяНастройка.РежимЗапускаКакПунктМеню = РежимЗапускаКакПунктМеню.Checked;
            ТекущаяНастройка.ВидКлиента = ВидКлиента.SelectedIndex;
            ТекущаяНастройка.ВидКлиентаКакПунктМеню = ВидКлиентаКакПунктМеню.Checked;
            ТекущаяНастройка.Описание = Описание.Text;
            ТекущаяНастройка.ПрограммаЗапуска = СобственнаяПрограммаЗапуска.Text;
            ТекущаяНастройка.КодДоступа = КодДоступа.Text;
            if (СочетаниеКлавиш.ГорячаяКлавишаВыбрана)
            {
                ТекущаяНастройка.СочетаниеКлавиш = Convert.ToString(СочетаниеКлавиш.КодПервогоСимвола) + "\\" + Convert.ToString(СочетаниеКлавиш.КодВторогоСимвола) + "\\" + Convert.ToString(СочетаниеКлавиш.КодТретьегоСимвола);
            }
            else
                ТекущаяНастройка.СочетаниеКлавиш = "0\\0\\0";
            
            ТекущаяНастройка.ПутьКХранилищу = ПутьКХранилищу.Text;
            ТекущаяНастройка.ИмяПользователяХранилища = ИмяПользователяХранилища.Text;
            ТекущаяНастройка.ПарольПользователяХранилища = ПарольПользователяХранилища.Text;
            ТекущаяНастройка.ДополнительныеПользователи = глПервоначальныеПользователи;

            if (ВерсияПлатформы.SelectedIndex > -1)
            	ТекущаяНастройка.ВерсияПлатформы = ВерсияПлатформы.Items[ВерсияПлатформы.SelectedIndex].ToString();
            else
				ТекущаяНастройка.ВерсияПлатформы = "";            
            
            if (РежимПодбораБазДанных)
            {
                ТекущаяНастройка.Группа = РодительБазыДанных;
            }
            DialogResult = DialogResult.OK;
            Close();
        }

        private void ВыборПути()
        {
            FolderBrowserDialog openFolder = new FolderBrowserDialog();
            openFolder.ShowNewFolderButton = true;

            if (!String.IsNullOrEmpty(Путь.Text))
                openFolder.SelectedPath = Путь.Text;
            else if (!String.IsNullOrEmpty(ПапкаСБазами))
                openFolder.SelectedPath = ПапкаСБазами;
            if (openFolder.ShowDialog() == DialogResult.OK)
            {
                string ПутьБазыДанных = openFolder.SelectedPath;
                Путь.Text = ПутьБазыДанных;
                ЗаполнитьСписокПользователей77();
                if (String.IsNullOrEmpty(Наименование.Text))
                {
                    string[] МассивПути = ПутьБазыДанных.Split('\\');
                    int Размерность = МассивПути.Length - 1;
                    if (Размерность >= 0)
                        Наименование.Text = МассивПути[Размерность];
                }
                if (ПроверятьНаличиеБаз)
                {
                    if (ТипБазыДанных.SelectedIndex == 0)
                    {
                        string ИмяФайлаДляПроверки = String.Empty;
                        if (ТипПлатформы.SelectedIndex == 0)
                            ИмяФайлаДляПроверки = "\\1Cv7.MD";
                        else
                            ИмяФайлаДляПроверки = "\\1Cv8.1CD";

                        if (!File.Exists(ПутьБазыДанных + ИмяФайлаДляПроверки))
                        {
                            ГлавноеОкно.ПоказатьИнфомационноеСообщение("По выбранному пути база данных не обнаружена!", this);
                        }
                    }
                }
            }
            openFolder.Dispose();
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
            ПоказатьИнформацию("Каталог, где располагается база данных");
        }

        private void Путь_MouseLeave(object sender, EventArgs e)
        {
            ОчиститьИнформацию();
        }

        private void Подсказка_Paint(object sender, PaintEventArgs e)
        {            
            e.Graphics.DrawImage(Properties.Resources.smallinfo, 1, 1);
        }

        private void ИспользуетсяАутентификацияWindows_CheckedChanged(object sender, EventArgs e)
        {
            УправлениеВидимостью();
        }

        private void ЗаполнитьСписокРежимовЗапуска()
        {
            РежимЗапуска.Items.Add("Запрашивать");
            РежимЗапуска.Items.Add("1С Предприятие");
            РежимЗапуска.Items.Add("Конфигуратор");
            РежимЗапуска.Items.Add("Отладчик");
            РежимЗапуска.Items.Add("Монитор");
        }

        private void ТипПлатформы_SelectedIndexChanged(object sender, EventArgs e)
        {
            УправлениеВидимостью();
            ПриВыбореТипаПлатформы();
            ЗаполнитьСписокПользователей77();
        }

        private void ТипБазыДанных_SelectedIndexChanged(object sender, EventArgs e)
        {
            УправлениеВидимостью();
        }

        private void РежимЗапускаКакПунктМеню_CheckedChanged(object sender, EventArgs e)
        {
            УправлениеВидимостью();
        }

        private void РежимЗапускаКакПунктМеню_MouseHover(object sender, EventArgs e)
        {
            ПоказатьИнформацию("Вариант запуска будет представлен отдельным пунктом в меню");
        }

        private void РежимЗапускаКакПунктМеню_MouseLeave(object sender, EventArgs e)
        {
            ОчиститьИнформацию();
        }

        private void Описание_MouseHover(object sender, EventArgs e)
        {
            ПоказатьИнформацию("Дополнительное описание для базы данных");
        }

        private void Описание_MouseLeave(object sender, EventArgs e)
        {
            ОчиститьИнформацию();
        }

        private void splitContainer1_Panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void ПутьКХранилищу_MouseHover(object sender, EventArgs e)
        {
            ПоказатьИнформацию("Каталог, где располагается хранилище конфигурации");
        }

        private void ПутьКХранилищу_MouseLeave(object sender, EventArgs e)
        {
            ОчиститьИнформацию();
        }

        private void НачатьРедактированиеПараметровХранилища()
        {
            Группа.Enabled = false;
            Наименование.Enabled = false;
            ТипБазыДанных.Enabled = false;
            ТипПлатформы.Enabled = false;
            Путь.Enabled = false;
            ИмяПользователя.Enabled = false;
            Пароль.Enabled = false;
            Описание.Enabled = false;
            РежимЗапуска.Enabled = false;
            РежимРаботы.Enabled = false;
            ПоказыватьВМенюЗапуска.Enabled = false;
            РежимЗапускаКакПунктМеню.Enabled = false;
            ВыбратьКаталогБД.Enabled = false;
            КнопкаОК.Enabled = false;
            КнопкаОтмена.Enabled = false;
            СсылкаПараметрыХранилища.Enabled = false;
            СсылкаДополнительныеПользователи.Enabled = false;
            ИспользуетсяАутентификацияWindows.Enabled = false;
            СочетаниеКлавиш.Enabled = false;
            ПанельПараметровХранилища.Visible = true;
            КодДоступа.Enabled = false;
            СобственнаяПрограммаЗапуска.Enabled = false;
            ВыбратьСобственнуюПрограмму.Enabled = false;
            ВидКлиентаКакПунктМеню.Enabled = false;
            ВидКлиента.Enabled = false;
            ВерсияПлатформы.Enabled = false;
        }

        private void НачатьРедактированиеДополнительныйПользователей()
        {
            Группа.Enabled = false;
            Наименование.Enabled = false;
            ТипБазыДанных.Enabled = false;
            ТипПлатформы.Enabled = false;
            Путь.Enabled = false;
            ИмяПользователя.Enabled = false;
            Пароль.Enabled = false;
            Описание.Enabled = false;
            РежимЗапуска.Enabled = false;
            РежимРаботы.Enabled = false;
            ПоказыватьВМенюЗапуска.Enabled = false;
            РежимЗапускаКакПунктМеню.Enabled = false;
            ВыбратьКаталогБД.Enabled = false;
            КнопкаОК.Enabled = false;
            КнопкаОтмена.Enabled = false;
            СсылкаПараметрыХранилища.Enabled = false;
            СсылкаДополнительныеПользователи.Enabled = false;
            ИспользуетсяАутентификацияWindows.Enabled = false;
            СочетаниеКлавиш.Enabled = false;
            СобственнаяПрограммаЗапуска.Enabled = false;
            ВыбратьСобственнуюПрограмму.Enabled = false;
            ПанельДополнительныхПользователей.Visible = true;
            КодДоступа.Enabled = false;
            ВидКлиентаКакПунктМеню.Enabled = false;
            ВидКлиента.Enabled = false;
            ВерсияПлатформы.Enabled = false;
        }

        private void ЗакончитьРедактированиеДополнительныхПользователей(bool УспешноеРедактирование)
        {
            Группа.Enabled = true;
            Наименование.Enabled = true;
            ТипБазыДанных.Enabled = true;
            ТипПлатформы.Enabled = true;
            Путь.Enabled = true;
            ИмяПользователя.Enabled = true;
            Пароль.Enabled = true;
            Описание.Enabled = true;
            РежимЗапуска.Enabled = true;
            РежимРаботы.Enabled = true;
            ПоказыватьВМенюЗапуска.Enabled = true;
            РежимЗапускаКакПунктМеню.Enabled = true;
            ВыбратьКаталогБД.Enabled = true;
            КнопкаОК.Enabled = true;
            КнопкаОтмена.Enabled = true;
            СсылкаПараметрыХранилища.Enabled = true;
            СсылкаДополнительныеПользователи.Enabled = true;
            ИспользуетсяАутентификацияWindows.Enabled = true;
            СочетаниеКлавиш.Enabled = true;
            СобственнаяПрограммаЗапуска.Enabled = true;
            ВыбратьСобственнуюПрограмму.Enabled = true;
            ПанельДополнительныхПользователей.Visible = false;
            КодДоступа.Enabled = true;
            ВидКлиента.Enabled = true;
            ВидКлиентаКакПунктМеню.Enabled = true;
            ВерсияПлатформы.Enabled = true;
            УправлениеВидимостью();

            if (!УспешноеРедактирование)
            {
                СкопироватьListView(ref глПервоначальныеПользователи, ref ДополнительныеПользователи);
                СкопироватьListView(ref глПервоначальныеПользователи, ref глДопПользователи);
            }
            else
                СкопироватьListView(ref глДопПользователи, ref глПервоначальныеПользователи);

        }

        private void ЗакончитьРедактированиеПараметровХранилища(bool УспешноеРедактирование)
        {
            Группа.Enabled = true;
            Наименование.Enabled = true;
            ТипБазыДанных.Enabled = true;
            ТипПлатформы.Enabled = true;
            Путь.Enabled = true;
            ИмяПользователя.Enabled = true;
            Пароль.Enabled = true;
            Описание.Enabled = true;
            РежимЗапуска.Enabled = true;
            РежимРаботы.Enabled = true;
            ПоказыватьВМенюЗапуска.Enabled = true;
            РежимЗапускаКакПунктМеню.Enabled = true;            
            ВыбратьКаталогБД.Enabled = true;
            КнопкаОК.Enabled = true;
            КнопкаОтмена.Enabled = true;
            СсылкаПараметрыХранилища.Enabled = true;
            СсылкаДополнительныеПользователи.Enabled = true;
            ИспользуетсяАутентификацияWindows.Enabled = true;
            СочетаниеКлавиш.Enabled = true;
            ПанельПараметровХранилища.Visible = false;
            КодДоступа.Enabled = true;
            ВидКлиента.Enabled = true;
            ВидКлиентаКакПунктМеню.Enabled = true;
            СобственнаяПрограммаЗапуска.Enabled = true;
            ВыбратьСобственнуюПрограмму.Enabled = true;
            ВерсияПлатформы.Enabled = true;
            УправлениеВидимостью();

            if (!УспешноеРедактирование)
            {
                ПутьКХранилищу.Text = ТекущаяНастройка.ИмяПользователя;
                ИмяПользователяХранилища.Text = ТекущаяНастройка.ИмяПользователяХранилища;
                ПарольПользователяХранилища.Text = ТекущаяНастройка.ПарольПользователяХранилища;
            }
        }

        private void СсылкаПараметрыХранилища_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            НачатьРедактированиеПараметровХранилища();
        }

        private void КнопкаОКХран_Click(object sender, EventArgs e)
        {
            ЗакончитьРедактированиеПараметровХранилища(true);
        }

        private void КнопкаОтменаХран_Click(object sender, EventArgs e)
        {
            ЗакончитьРедактированиеПараметровХранилища(false);
        }

        private void ИмяПользователяХранилища_MouseHover(object sender, EventArgs e)
        {
            ПоказатьИнформацию("Имя пользователя для подключения к хранилищу конфигурации");
        }

        private void ИмяПользователяХранилища_MouseLeave(object sender, EventArgs e)
        {
            ОчиститьИнформацию();
        }

        private void ПарольПользователяХранилища_MouseHover(object sender, EventArgs e)
        {
            ПоказатьИнформацию("Пароль пользователя для подключения к хранилищу конфигурации");
        }

        private void ПарольПользователяХранилища_MouseLeave(object sender, EventArgs e)
        {
            ОчиститьИнформацию();
        }

        private void ФормаБазыДанных_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == 27)
            {
                if (ПанельПараметровХранилища.Visible)
                    ЗакончитьРедактированиеПараметровХранилища(false);
                else if (ПанельДополнительныхПользователей.Visible)
                    ЗакончитьРедактированиеДополнительныхПользователей(false);
                else
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
            if (ТипПлатформы.SelectedIndex == 0)
            {
            	openFile.Title = "Укажите файл запуска платформы 7.7";
            	openFile.Filter = "Файл запуска 1С Предприятия|1cv7*.exe";
            }
            else
            {
            	openFile.Title = "Укажите файл запуска 8.x";
            	openFile.Filter = "Файл запуска 1С Предприятия|1cv8*.exe;starter.exe;1cestart.exe";
            }
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
            ПоказатьИнформацию("Собственная программа, с помощью которой будет запускаться база");
        }

        private void СобственнаяПрограммаЗапуска_MouseLeave(object sender, EventArgs e)
        {
            ОчиститьИнформацию();
        }

        private void ВыбратьСобственнуюПрограмму_MouseHover(object sender, EventArgs e)
        {
            ПоказатьИнформацию("Выбрать собственную программу запуска для данной базы");
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
                if (ТипБазыДанных.SelectedIndex == 0)
                    ВыборПути();
            }
        }

        private void СобственнаяПрограммаЗапуска_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyValue == 115)
            {
                if (ТипПлатформы.SelectedIndex == 0)
                    ВыборСобственнойПрограммы();
            }
        }

        private void ФормаБазыДанных_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyValue == 13)
            {
                if ((e.Modifiers & Keys.Control) != 0)
                {
                    if (ПанельПараметровХранилища.Visible)
                        ЗакончитьРедактированиеПараметровХранилища(true);
                    else if (ПанельДополнительныхПользователей.Visible)
                        ЗакончитьРедактированиеДополнительныхПользователей(true);
                    else
                        КнопкаОК_Click(sender, e);
                }                    
            }
        }        

        private void СсылкаДополнительныеПользователи_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            НачатьРедактированиеДополнительныйПользователей();
        }

        private void ДополнительныеПользователиОК_Click(object sender, EventArgs e)
        {
            ЗакончитьРедактированиеДополнительныхПользователей(true);
        }

        private void ДополнительныеПользователиОтмена_Click(object sender, EventArgs e)
        {
            ЗакончитьРедактированиеДополнительныхПользователей(false);
        }

        private void КнопкаДобавить_Click(object sender, EventArgs e)
        {
            if (ДополнительныйИмя.Text != "")
            {
                ListViewItem НовыйПользователь = глДопПользователи.Items.Add(ДополнительныйИмя.Text);
                НовыйПользователь.SubItems.Add(ДополнительныйПароль.Text);
                НовыйПользователь = ДополнительныеПользователи.Items.Add(ДополнительныйИмя.Text);
                НовыйПользователь.SubItems.Add("**********");
                ДополнительныйИмя.Text = "";
                ДополнительныйПароль.Text = "";
                ДополнительныйИмя.Select();
            }
            else
                ГлавноеОкно.ПоказатьИнфомационноеСообщение("Не указано имя пользователя!", this);            
        }

        private void УдалитьДополнительногоПользователя()
        {
            if (ДополнительныеПользователи.SelectedItems.Count > 0)
            {
                int ИндексТекДопПользователя = ДополнительныеПользователи.SelectedItems[0].Index;
                ДополнительныеПользователи.Items.RemoveAt(ИндексТекДопПользователя);
                глДопПользователи.Items.RemoveAt(ИндексТекДопПользователя);
            }
        }

        private void КнопкаУдалить_Click(object sender, EventArgs e)
        {
            УдалитьДополнительногоПользователя();    
        }

        private void ДополнительныеПользователи_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyValue == 46)
            {
                УдалитьДополнительногоПользователя();
            }
        }

        private void ВидКлиентаКакПунктМеню_CheckedChanged(object sender, EventArgs e)
        {
            УправлениеВидимостью();
        }

        private void КодДоступа_MouseHover(object sender, EventArgs e)
        {
            ПоказатьИнформацию("Код доступа при блокировке соединений с базой");
        }

        private void КодДоступа_MouseLeave(object sender, EventArgs e)
        {
            ОчиститьИнформацию();
        }
    }
}
