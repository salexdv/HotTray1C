using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Windows;
using System.Xml;
using System.IO;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.Threading;
using Microsoft.Win32;
using MainStruct;
using gma.System.Windows;
using installer1C;

namespace TestMenuPopup
{  
    public partial class ГлавноеОкно : Form
    {     
        [DllImport("kernel32", SetLastError = true)]
        public static extern short GlobalAddAtom(string lpString);
        [DllImport("kernel32", SetLastError = true)]
        public static extern short GlobalDeleteAtom(short nAtom);
        [DllImport("user32", SetLastError = true)]
        public static extern int UnregisterHotKey(IntPtr hwnd, int id);
        
        #region МодификаторыКлавиш
        public static int MOD_ALT = 0x1;
        public static int MOD_CONTROL = 0x2;
        public static int MOD_SHIFT = 0x4;
        public static int MOD_WIN = 0x8;
        public static int WM_HOTKEY = 0x312;
        #endregion        

        [DllImport("user32.dll")]
        private static extern bool RegisterHotKey(IntPtr hWnd, short id, int fsModifiers, int vlc);

        [DllImport("user32", SetLastError = true)]
        public static extern int SetForegroundWindow(IntPtr hwnd);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll")]
        static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint lpdwProcessId);

        [DllImportAttribute("kernel32.dll", EntryPoint = "SetProcessWorkingSetSize", ExactSpelling = true, CharSet = CharSet.Ansi, SetLastError = true)]
        private static extern int SetProcessWorkingSetSize(IntPtr process, int minimumWorkingSetSize, int maximumWorkingSetSize);

        private Boolean НастройкиИзменены;
        private Boolean НеобходимоПерерисоватьМеню;
        private int ТекущаяКартинкаАнимации;
        private double Коэффициент;
        public static installer1CInfo installer1C; 
        public static Form ЭтаФорма;
        public static XmlNode УзелБазДанных;
        private static XmlNode УзелИсключений;
        private static int IDCTRLH;
        private string ВремФайлДесктопа;
		private Dictionary<string, string> ToolTipMessages = new Dictionary<string, string>();
		private static List<StructChangeStateIbases> OriginalStatesIBases = new List<StructChangeStateIbases>();
		

        public Thread ПотокПроверкиОбновления;
       
        
        //////////////////////////////////////////////////////////////////////////////////////////////
        //////////////////////////////////////////////////////////////////////////////////////////////
        //////////////////////////////////////////////////////////////////////////////////////////////
        
        // Настройки программы хранимые в XML-документе
        static private XmlDocument НастройкиHotTray;
        static private TreeNode ПоследнийАктивныйУзелДереваГрупп;
        static private ListBox СписокНеудачныхГорячихКлавиш = new ListBox();
        static private string КаталогПрограммы;
        
        static private СтруктураСобственныхНастроекМеню СтруктураОформления;
        static private bool СобственноеОформление;        

        static private ImageList КоллекцияСписка;

        UserActivityHook HookMouse;
        static void Пауза(int Миллисекунд)
        {
            Thread.Sleep(Миллисекунд);
        }
       
        static public void ОчисткаМусора()
        {
            GC.Collect();
            GC.WaitForPendingFinalizers();
            try
            {
                SetProcessWorkingSetSize(System.Diagnostics.Process.GetCurrentProcess().Handle, -1, -1);
            }
            catch
            {
            }
        }

        static bool НажатSHIFT()
        {
            return ((Control.ModifierKeys & Keys.Shift) == Keys.Shift);
        }

        static bool НажатCTRL()
        {
            return ((Control.ModifierKeys & Keys.Control) == Keys.Control);
        }        

        // Процедура выполняет показ информационного сообщения с заданным текстом
        //
        static public void ПоказатьИнфомационноеСообщение(string ТекстСообщения, IWin32Window ВладелецСообщения)
        {
            if (ВладелецСообщения != null)
                MessageBox.Show(ВладелецСообщения, ТекстСообщения, "Hot tray 1C .NET", MessageBoxButtons.OK, MessageBoxIcon.Information);
            else
                MessageBox.Show(ТекстСообщения, "Hot tray 1C .NET", MessageBoxButtons.OK, MessageBoxIcon.Information);            
        }

        static public DialogResult Вопрос(String ТекстВопроса, IWin32Window ВладелецСообщения)
        {
            return MessageBox.Show(ВладелецСообщения, ТекстВопроса, "Hot tray 1C .NET", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
        }

        // Процедура сохраняет настройки программы
        //
        static public void СохранитьНастройкиПрограммы()
        {            
            НастройкиHotTray.Save(КаталогПрограммы + "\\Settings.xml");            
        }

        // Процедура получает значение заданного атрибута у узла XML
        //
        static public string ПолучитьАтрибутУзла(XmlNode УзелXML, string НаименованиеАтрибута, string DefValue = "")
        {
            XmlAttribute Attr = (XmlAttribute)УзелXML.Attributes.GetNamedItem(НаименованиеАтрибута);
            if (Attr != null)
                return Attr.Value;
            else
                return DefValue;            
        }

        // Процедура устанавливает атрибут заданного узла
        //
        static public void УстановитьАтрибутУзла(XmlElement УзелXML, String НаименованиеАтрибута, String ЗначениеАтрибута)
        {
            УзелXML.SetAttribute(НаименованиеАтрибута, ЗначениеАтрибута);
        }

        // Процедура получает значение указанной настройки программы
        //
        static public string ПолучитьЗначениеНастройки(string НаименованиеНастройки, string ЗначениеПоУмолчанию)
        {
            if (НастройкиHotTray.HasChildNodes)
            {
                string Значение = ПолучитьАтрибутУзла(НастройкиHotTray.DocumentElement, НаименованиеНастройки);
                if (String.IsNullOrEmpty(Значение))
                    return ЗначениеПоУмолчанию;
                else
                    return Значение;
            }
            return "";
        }

        // Процедура устанавливает значение указанной настройки программы
        //
        static public void УстановитьЗначениеНастройки(string НаименованиеНастройки, string ЗначениеНастройки)
        {
            УстановитьАтрибутУзла(НастройкиHotTray.DocumentElement, НаименованиеНастройки, ЗначениеНастройки);
        }

        static public Image ПолучитьКартинкуПлатформы(int Платформа)
        {
            switch (Платформа)
            {
                case 0: return Properties.Resources.Платформа77;
                case 1: return Properties.Resources.Платформа80;
                case 2: return Properties.Resources.Платформа81;
                case 3: return Properties.Resources.Платформа82;
                case 4: return Properties.Resources.Платформа82;
                default: return null;
            }
        }

        static public string ПолучитьПредставлениеПлатформы(int Платформа)
        {
            switch (Платформа)
            {
                case 0: return "1С Предприятие 7.7";
                case 1: return "1С Предприятие 8.0";
                case 2: return "1С Предприятие 8.1";
                case 3: return "1С Предприятие 8.2";
                case 4: return "1С Предприятие 8.3";
                default: return "";
            }

        }

        static string ПолучитьПредставлениеТипаБазы(int Тип)
        {
            switch (Тип)
            {
                case 0: return "Файловая";
                case 1: return "Серверная";
                default: return "";
            }
        }

        // Процедура формирует подсказку для базы данных
        //
        static public string СформироватьОписаниеБазыДанных(XmlNode УзелXML)
        {
            string Подсказка;
            Подсказка = "База: " + ПолучитьАтрибутУзла(УзелXML, "Наименование") + "\n";
            Подсказка = Подсказка + "Платформа: " + ПолучитьПредставлениеПлатформы(Convert.ToInt16(ПолучитьАтрибутУзла(УзелXML, "ТипПлатформы"))) + "\n";
            Подсказка = Подсказка + "Тип базы: " + ПолучитьПредставлениеТипаБазы(Convert.ToInt16(ПолучитьАтрибутУзла(УзелXML, "ТипБазы"))) + "\n";
            Подсказка = Подсказка + "Расположение: " + ПолучитьАтрибутУзла(УзелXML, "Путь");
            string ИмяПользователя = ПолучитьАтрибутУзла(УзелXML, "ИмяПользователя");
            if (!String.IsNullOrEmpty(ИмяПользователя))
                Подсказка = Подсказка + "\nИмя пользователя: " + ПолучитьАтрибутУзла(УзелXML, "ИмяПользователя");

            return Подсказка;
        }

        static string ПолучитьСостояниеБазы(СтруктураНастроекЭлемента Настройки)
        {
            string СостояниеБазы = String.Empty;

            if (БазаДанныхСуществуетПоУказанномуПути(Настройки))
            {
                if (Настройки.ТипПлатформы > 2)
                    СостояниеБазы = Convert.ToString(Convert.ToInt16(Настройки.ПоказыватьВМенюЗапуска)) + "3";
                else
                    СостояниеБазы = Convert.ToString(Convert.ToInt16(Настройки.ПоказыватьВМенюЗапуска)) + Convert.ToString(Настройки.ТипПлатформы);
                
                if ((Настройки.РежимЗапуска == 0) || (Настройки.РежимЗапуска == 1))
                {
                    СостояниеБазы = СостояниеБазы + "0";
                }
                else
                {
                    if (Настройки.ТипПлатформы == 0)
                    {
                        СостояниеБазы = СостояниеБазы + Convert.ToString(Настройки.РежимЗапуска);
                    }
                    else
                        СостояниеБазы = СостояниеБазы + "1";

                }                
            }
            else
            {
                СостояниеБазы = Convert.ToString(Convert.ToInt16(Настройки.ПоказыватьВМенюЗапуска)) + "1";                
            }
            return СостояниеБазы;
        }

        static void РекурсивнаяОбработкаДополнительныхПользователей(XmlNode УзелXML, ref ListView Список)
        {
            if (УзелXML.Name != "Пользователь")
            {
                if (УзелXML.HasChildNodes)
                {
                    РекурсивнаяОбработкаДополнительныхПользователей(УзелXML.FirstChild, ref Список);
                    return;
                }

            }
            ListViewItem Пользователь = Список.Items.Add(ПолучитьАтрибутУзла(УзелXML, "Имя"));
            Пользователь.SubItems.Add(Шифрование(ПолучитьАтрибутУзла(УзелXML, "Пароль")));
            if (УзелXML.NextSibling != null)
                РекурсивнаяОбработкаДополнительныхПользователей(УзелXML.NextSibling, ref Список);
        }

        static public ListView ПолучитьСписокДополнительныхПользователей(XmlNode УзелXML)
        {
            ListView СписокДопПользователей = new ListView();
            if (УзелXML.HasChildNodes)
                РекурсивнаяОбработкаДополнительныхПользователей(УзелXML.FirstChild, ref СписокДопПользователей);

            return СписокДопПользователей;
        }

        // Процедура поочередно обходит базы узла и добавляет их в список
        //
        static void ПолучитьСписокБазУзла(XmlNode УзелXML, ListView СписокБаз)
        {
            if (УзелXML == null)
                return;
            if (УзелXML.Name == "БазаДанных")
            {
                СтруктураНастроекЭлемента НастройкиБазы = ПолучитьНастройкиБазыДанных(УзелXML);

                ListViewItem НоваяСтрокаСписка = СписокБаз.Items.Add(НастройкиБазы.Наименование);
                                
                string СостояниеБазы = String.Empty;
                
                if (!НастройкиБазы.Приложение)
                {
                    if (БазаДанныхСуществуетПоУказанномуПути(НастройкиБазы))
                    {
                        СостояниеБазы = ПолучитьСостояниеБазы(НастройкиБазы);
                        НоваяСтрокаСписка.ImageKey = "Состояние" + СостояниеБазы + ".png";
                    }
                    else
                    {
                        СостояниеБазы = ПолучитьСостояниеБазы(НастройкиБазы);
                        НоваяСтрокаСписка.ImageKey = "Отсутствует" + СостояниеБазы + ".png";
                    }
                }
                else
                {
                    int ИндексИконки = 0;                    
                    if (УдалосьДобавитьИконкуВКоллекцию(НастройкиБазы, ref ИндексИконки))
                        НоваяСтрокаСписка.ImageIndex = ИндексИконки;
                    else
                    {
                        string СостояниеПриложения = Convert.ToString(Convert.ToInt16(НастройкиБазы.ПоказыватьВМенюЗапуска)) + "1";
                        НоваяСтрокаСписка.ImageKey = НоваяСтрокаСписка.ImageKey = "Отсутствует" + СостояниеПриложения + ".png";
                    }                    
                }

                НоваяСтрокаСписка.Tag = УзелXML;
                НоваяСтрокаСписка.SubItems.Add(ПолучитьАтрибутУзла(УзелXML, "Путь"));                
                if (НастройкиБазы.Приложение)
                {
                    НоваяСтрокаСписка.SubItems.Add("");
                    НоваяСтрокаСписка.SubItems.Add("Да");
                }
                else
                    НоваяСтрокаСписка.SubItems.Add(ПолучитьАтрибутУзла(УзелXML, "ИмяПользователя"));

            }
            else if (УзелXML.Name == "Группа")
            {
                ListViewItem НоваяСтрокаСписка = СписокБаз.Items.Add(ПолучитьАтрибутУзла(УзелXML, "Наименование"));
                НоваяСтрокаСписка.ImageKey = "Папка";
                НоваяСтрокаСписка.Tag = УзелXML;
            }

            if (УзелXML.NextSibling != null)
            {
                ПолучитьСписокБазУзла(УзелXML.NextSibling, СписокБаз);
            }
        }

        // Процедура заполняет список баз по выбранной группе (родителю)
        //
        static public void ЗаполнениеСпискаБаз(TreeView ДеревоГрупп, ListView СписокБаз)
        {
            if (ДеревоГрупп.SelectedNode == null)
            {
                ДеревоГрупп.SelectedNode = ДеревоГрупп.Nodes[0];
            }

            XmlElement ТекущаяГруппа;
            try
            {
                ТекущаяГруппа = (XmlElement)ДеревоГрупп.SelectedNode.Tag;
            }
            catch
            {
                ТекущаяГруппа = (XmlElement)УзелБазДанных;
            }

            СписокБаз.Items.Clear();

            //if (ТекущаяГруппа.HasChildNodes)
            //{
                if (ТекущаяГруппа.Name == "Группа")
                {
                    ListViewItem НоваяСтрокаСписка = СписокБаз.Items.Add("");
                    НоваяСтрокаСписка.Tag = ТекущаяГруппа;
                    НоваяСтрокаСписка.ImageKey = "ВыходИзГруппы";
                }
                ПолучитьСписокБазУзла(ТекущаяГруппа.FirstChild, СписокБаз);
            //}

            УстановитьАтрибутУзла(ТекущаяГруппа, "ГруппаАктивна", "true");

            // Переместим группы вверх
            int i = 0;
            ListBox СписокДляСортировки = new ListBox();
            for (i = 0; i < СписокБаз.Items.Count; i++)
            {
                СписокДляСортировки.Items.Add(СписокБаз.Items[i]);
            }

            СписокБаз.Items.Clear();

            i = 0;
            while (i < СписокДляСортировки.Items.Count)
            {
                ListViewItem ТекЭлементСписка = (ListViewItem)СписокДляСортировки.Items[i];
                if (((XmlNode)ТекЭлементСписка.Tag).Name == "Группа")
                {
                    СписокБаз.Items.Add(ТекЭлементСписка);
                    СписокДляСортировки.Items.Remove(СписокДляСортировки.Items[i]);
                }
                else
                    i++;
            }

            i = 0;
            while (i < СписокДляСортировки.Items.Count)
            {
                ListViewItem ТекЭлементСписка = (ListViewItem)СписокДляСортировки.Items[i];
                СписокБаз.Items.Add(ТекЭлементСписка);
                i++;
            }
            СписокДляСортировки.Dispose();
        }

        // Процедура управляет положением узла дерева в зависимости от значения
        // его атрибута "ГруппаРаскрыта"
        static void УправлениеПоложениемУзлаДереваГрупп(XmlNode УзелXML, TreeNode УзелДерева)
        {
            if (Convert.ToBoolean(ПолучитьАтрибутУзла(УзелXML, "ГруппаРаскрыта")))
            {
                УзелДерева.Expand();
            }
            else
            {
                УзелДерева.Collapse();
            }
        }

        // Процедура определяет является ли данный узел дерева активным
        //
        static TreeNode ОпределениеАктивностиУзлаДереваГруппа(XmlNode УзелXML, TreeNode УзелДерева, ref XmlNode АктивныйЭлемент)
        {
            if (АктивныйЭлемент != null)
            {
                if (АктивныйЭлемент == УзелXML)
                {
                    return УзелДерева;
                }
                else
                    return null;
            }
            if (Convert.ToBoolean(ПолучитьАтрибутУзла(УзелXML, "ГруппаАктивна")))
            {
                return УзелДерева;
            }
            else
            {
                return null;
            }
        }

        // Процедура заполняет дерево групп, обходя рекурсивно настройки программы
        //
        public static TreeNode ЗаполнениеДереваГрупп(TreeView ДеревоГрупп, XmlNode УзелXML, TreeNode ТекущаяГруппа, XmlNode АктивныйЭлемент)
        {
            TreeNode ВременнаяГруппа = null;
            TreeNode Группа = null;
            Boolean СозданаНоваяГруппа = false;
            TreeNode НовыйРодитель = new TreeNode();
            if (УзелXML == null)
                return null;
            if (УзелXML is XmlElement)
            {
                if (УзелXML.Name == "БазыДанных")
                {
                    TreeNode КорневаяГруппа = new TreeNode();
                    КорневаяГруппа.Text = "Группы баз данных";
                    ДеревоГрупп.Nodes.Add(КорневаяГруппа);
                    КорневаяГруппа.Tag = УзелXML;
                    ТекущаяГруппа = ДеревоГрупп.Nodes[0];

                    // УправлениеПоложениемУзлаДереваГрупп(УзелXML, КорневаяГруппа);
                    TreeNode АктивнаяГруппа = ОпределениеАктивностиУзлаДереваГруппа(УзелXML, КорневаяГруппа, ref АктивныйЭлемент);
                    if (АктивнаяГруппа != null)
                        ВременнаяГруппа = АктивнаяГруппа;
                }
                if (УзелXML.Name == "Группа")
                {
                    Группа = new TreeNode();
                    Группа.Text = ПолучитьАтрибутУзла(УзелXML, "Наименование");
                    ТекущаяГруппа.Nodes.Add(Группа);
                    Группа.Tag = УзелXML;
                    НовыйРодитель = Группа;
                    СозданаНоваяГруппа = true;
                    
                    TreeNode АктивнаяГруппа = ОпределениеАктивностиУзлаДереваГруппа(УзелXML, Группа, ref АктивныйЭлемент);
                    if (АктивнаяГруппа != null)
                        ВременнаяГруппа = АктивнаяГруппа;
                }
            }
            if (УзелXML.HasChildNodes)
            {
                if (!СозданаНоваяГруппа)
                {
                    TreeNode АктивнаяГруппа = ЗаполнениеДереваГрупп(ДеревоГрупп, УзелXML.FirstChild, ТекущаяГруппа, АктивныйЭлемент);
                    if (АктивнаяГруппа != null)
                        ВременнаяГруппа = АктивнаяГруппа;
                }
                else
                {
                    TreeNode АктивнаяГруппа = ЗаполнениеДереваГрупп(ДеревоГрупп, УзелXML.FirstChild, НовыйРодитель, АктивныйЭлемент);
                    if (АктивнаяГруппа != null)
                        ВременнаяГруппа = АктивнаяГруппа;
                }
            }
            if (УзелXML.NextSibling != null)
            {
                TreeNode АктивнаяГруппа = ЗаполнениеДереваГрупп(ДеревоГрупп, УзелXML.NextSibling, ТекущаяГруппа, АктивныйЭлемент);
                if (АктивнаяГруппа != null)
                    ВременнаяГруппа = АктивнаяГруппа;
            }            

            if (ВременнаяГруппа != null)
            {
                ДеревоГрупп.SelectedNode = ВременнаяГруппа;
                ДеревоГрупп.Select();
                ПоследнийАктивныйУзелДереваГрупп = ВременнаяГруппа;
            }

            return ВременнаяГруппа;
        }

        static public СтруктураНастроекЭлемента ПолучитьНастройкиБазыДанных(XmlNode УзелXML)
        {
            СтруктураНастроекЭлемента НоваяНастройка = new СтруктураНастроекЭлемента();
            НоваяНастройка.Группа = УзелXML.ParentNode;
            НоваяНастройка.Ссылка = УзелXML;
            НоваяНастройка.Наименование = ПолучитьАтрибутУзла(УзелXML, "Наименование");
            НоваяНастройка.ИмяПользователя = ПолучитьАтрибутУзла(УзелXML, "ИмяПользователя");
            НоваяНастройка.Пароль = Шифрование(ПолучитьАтрибутУзла(УзелXML, "ПарольПользователя"));
            НоваяНастройка.Путь = ПолучитьАтрибутУзла(УзелXML, "Путь");
            НоваяНастройка.ТипПлатформы = Convert.ToInt16(ПолучитьАтрибутУзла(УзелXML, "ТипПлатформы"));
            НоваяНастройка.ТипБазы = Convert.ToInt16(ПолучитьАтрибутУзла(УзелXML, "ТипБазы"));
            НоваяНастройка.ИспользуетсяАутентификацияWindows = Convert.ToBoolean(ПолучитьАтрибутУзла(УзелXML, "ИспользуетсяАутентификацияWindows"));
            НоваяНастройка.ПоказыватьВМенюЗапуска = Convert.ToBoolean(ПолучитьАтрибутУзла(УзелXML, "ПоказыватьВМенюЗапуска"));
            НоваяНастройка.РежимРаботы = Convert.ToInt16(ПолучитьАтрибутУзла(УзелXML, "РежимРаботы"));
            НоваяНастройка.РежимЗапуска = Convert.ToInt16(ПолучитьАтрибутУзла(УзелXML, "РежимЗапуска"));
            НоваяНастройка.РежимЗапускаКакПунктМеню = Convert.ToBoolean(ПолучитьАтрибутУзла(УзелXML, "РежимЗапускаКакПунктМеню"));
            НоваяНастройка.ВидКлиента = Convert.ToInt16(ПолучитьАтрибутУзла(УзелXML, "ВидКлиента", "0"));
            НоваяНастройка.ВидКлиентаКакПунктМеню = Convert.ToBoolean(ПолучитьАтрибутУзла(УзелXML, "ВидКлиентаКакПунктМеню", "false"));
            НоваяНастройка.КодДоступа = Шифрование(ПолучитьАтрибутУзла(УзелXML, "КодДоступа"));            
            НоваяНастройка.Описание = ПолучитьАтрибутУзла(УзелXML, "Описание");
            НоваяНастройка.ПутьКХранилищу = ПолучитьАтрибутУзла(УзелXML, "ПутьКХранилищу");
            НоваяНастройка.ПарольПользователяХранилища = Шифрование(ПолучитьАтрибутУзла(УзелXML, "ПарольПользователяХранилища"));
            НоваяНастройка.ИмяПользователяХранилища = ПолучитьАтрибутУзла(УзелXML, "ИмяПользователяХранилища");
            НоваяНастройка.СочетаниеКлавиш = ПолучитьАтрибутУзла(УзелXML, "СочетаниеКлавиш");
            НоваяНастройка.ПрограммаЗапуска = ПолучитьАтрибутУзла(УзелXML, "ПрограммаЗапуска");
            НоваяНастройка.ДополнительныеПользователи = ПолучитьСписокДополнительныхПользователей(УзелXML);
            НоваяНастройка.Приложение = Convert.ToBoolean(ПолучитьАтрибутУзла(УзелXML, "Приложение", "false"));
			НоваяНастройка.ВерсияПлатформы = ПолучитьАтрибутУзла(УзелXML, "ВерсияПлатформы");            

            return НоваяНастройка;
        }

        static public СтруктураНастроекГруппыЭлементов ПолучитьНастройкиГруппы(XmlNode УзелXML)
        {            
            СтруктураНастроекГруппыЭлементов НоваяНастройка = new СтруктураНастроекГруппыЭлементов();
            НоваяНастройка.Группа = УзелXML.ParentNode;
            НоваяНастройка.Ссылка = УзелXML;
            НоваяНастройка.Наименование = ПолучитьАтрибутУзла(УзелXML, "Наименование");

            return НоваяНастройка;
        }

        // Функция возвращает иконку для пункта меню "Режимы"
        //
        static Image ПолучитьИконкуРежима(int Платформа, int Режим)
        {
            if ((Платформа == 0) && (Режим == 0))
                return TestMenuPopup.Properties.Resources.Платформа77;
            else if ((Платформа == 0) && (Режим == 1))
                return TestMenuPopup.Properties.Resources.Режим01;
            else if ((Платформа == 0) && (Режим == 2))
                return TestMenuPopup.Properties.Resources.Режим02;
            else if ((Платформа == 0) && (Режим == 3))
                return TestMenuPopup.Properties.Resources.Режим03;
            else if ((Платформа == 0) && (Режим == 4))
                return TestMenuPopup.Properties.Resources.Режим04;
            else if ((Платформа == 1) && (Режим == 0))
                return TestMenuPopup.Properties.Resources.Режим11;
            else if ((Платформа == 1) && (Режим == 1))
                return TestMenuPopup.Properties.Resources.Режим11;
            else if ((Платформа == 1) && (Режим == 2))
                return TestMenuPopup.Properties.Resources.Режим12;
            else if ((Платформа == 2) && (Режим == 0))
                return TestMenuPopup.Properties.Resources.Режим21;
            else if ((Платформа == 2) && (Режим == 1))
                return TestMenuPopup.Properties.Resources.Режим21;
            else if ((Платформа == 2) && (Режим == 2))
                return TestMenuPopup.Properties.Resources.Режим22;
            else if ((Платформа > 2) && (Режим == 0))
                return TestMenuPopup.Properties.Resources.Режим31;
            else if ((Платформа > 2) && (Режим == 1))
                return TestMenuPopup.Properties.Resources.Режим31;
            else if ((Платформа > 2) && (Режим == 1))
                return TestMenuPopup.Properties.Resources.Режим31;
            else if ((Платформа > 0) && (Режим == 4))
                return TestMenuPopup.Properties.Resources.Режим34;
            else
                return TestMenuPopup.Properties.Resources.Режим32;
        }

        // Функция возвращает иконку для пункта меню "Вид клиента"
        //
        static Image ПолучитьИконкуКлиента(int ВидКлиента)
        {
            if (ВидКлиента == 0)
                return TestMenuPopup.Properties.Resources.Автоматически;
            else if (ВидКлиента == 1)
                return TestMenuPopup.Properties.Resources.Автоматически;
            else if (ВидКлиента == 2)
                return TestMenuPopup.Properties.Resources.ТонкийКлиент;
            else
                return TestMenuPopup.Properties.Resources.ТолстыйКлиент;            
        }


        static void ЗапуститьБазуДанных(СтруктураНастроекЭлемента НастройкаБазы, string ПутьЗапускаПлатформы)
        {
            string СтрокаЗапуска = "";
            
            StructChangeStateIbases StatesDB = new StructChangeStateIbases();
			StatesDB.BaseType = НастройкаБазы.ТипБазы;
			StatesDB.BasePath = НастройкаБазы.Путь;

            if (!НастройкаБазы.Приложение)
            {
                int РежимЗапуска = НастройкаБазы.РежимЗапуска;                
                string СтрокаРежимаЗапуска = "";
                if (РежимЗапуска == 1)
                    СтрокаРежимаЗапуска = " ENTERPRISE /";
                else if (РежимЗапуска == 2)
                    СтрокаРежимаЗапуска = " CONFIG /";
                else if (РежимЗапуска == 3)
                    СтрокаРежимаЗапуска = " DEBUG /";
                else if (РежимЗапуска == 4)
                    СтрокаРежимаЗапуска = " MONITOR /";

                int ТипПлатформы = НастройкаБазы.ТипПлатформы;
                if (ТипПлатформы == 0)
                {
                    РегистрацияБазы77(НастройкаБазы);
                    СтрокаЗапуска = СтрокаРежимаЗапуска + "D\"" + НастройкаБазы.Путь + "\"";
                }
                else
                {                    
                    if (НастройкаБазы.ТипБазы == 0)
                    {
                        СтрокаЗапуска = СтрокаРежимаЗапуска + "F\"" + НастройкаБазы.Путь + "\"";
                    }
                    else
                    {
                        string[] ПараметрыСоединения = НастройкаБазы.Путь.Split('/');
                        if (ПараметрыСоединения.Length != 2)
                        {
                            ПоказатьИнфомационноеСообщение("Неверные параметры соединения с серверной базой данных", null);
                            return;
                        }
                        СтрокаЗапуска = СтрокаРежимаЗапуска + "S" + ПараметрыСоединения[0] + "\\" + ПараметрыСоединения[1];
                    }

                    if (ТипПлатформы > 2)
                    {
                        if (iBasesEdit.Read_v8i())
                        {
                            // Check db parameters in ibases.v8i
                            string PlatformVersion = НастройкаБазы.ВерсияПлатформы.Trim();
                            string AppID = "";

                            if (String.IsNullOrEmpty(PlatformVersion))
                            {
                                PlatformVersion = (ТипПлатформы == 3) ? "8.2" : "8.3";
                            }

                            switch (НастройкаБазы.ВидКлиента)
                            {
                                case 0:
                                    AppID = "Auto";
                                    break;
                                case 1:
                                    AppID = "Auto";
                                    break;
                                case 2:
                                    AppID = "ThinClient";
                                    break;
                                case 3:
                                    AppID = "ThickClient";
                                    break;
                            }

                            if (iBasesEdit.BaseExists(НастройкаБазы.ТипБазы, НастройкаБазы.Путь))
                            {
                                // Check application id
                                if (РежимЗапуска == 1)
                                {
                                    StatesDB.OriginalAppID = iBasesEdit.GetParameterValue(НастройкаБазы.ТипБазы, НастройкаБазы.Путь, "App");
                                    if (StatesDB.OriginalAppID != AppID)
                                    {                                        
                                        StatesDB.ChangeAppID = (iBasesEdit.SetParameterValue(НастройкаБазы.ТипБазы, НастройкаБазы.Путь, "App", AppID));
                                    }
                                }
                                
                                // Check platform version
                                StatesDB.OriginalVersion = iBasesEdit.GetParameterValue(НастройкаБазы.ТипБазы, НастройкаБазы.Путь, "Version");
                                if (StatesDB.OriginalVersion != PlatformVersion)
                                {
                                    StatesDB.ChangeVersion = (iBasesEdit.SetParameterValue(НастройкаБазы.ТипБазы, НастройкаБазы.Путь, "Version", PlatformVersion));
                                }
                            }
                            else
                            {
                                //Creating a new entry, followed by removal
                                StatesDB.DelFromFile = iBasesEdit.CreateNewBase(НастройкаБазы.ТипБазы, НастройкаБазы.Путь, PlatformVersion, AppID, НастройкаБазы.ИспользуетсяАутентификацияWindows);                                
                            }

                        }
                    }
               
                }

                if (!НастройкаБазы.ИспользуетсяАутентификацияWindows)
                    СтрокаЗапуска = СтрокаЗапуска + " /N\"" + НастройкаБазы.ИмяПользователя + "\" /P\"" + НастройкаБазы.Пароль + "\"";

                if (НастройкаБазы.РежимРаботы == 1)
                    СтрокаЗапуска = СтрокаЗапуска + " /M";

                if ((ТипПлатформы != 0) && (РежимЗапуска == 2) && (!(String.IsNullOrEmpty(НастройкаБазы.ИмяПользователяХранилища))))
                {
                    СтрокаЗапуска = СтрокаЗапуска + " /ConfigurationRepositoryF \"" + НастройкаБазы.ПутьКХранилищу + "\" /ConfigurationRepositoryN \"" + НастройкаБазы.ИмяПользователяХранилища + "\" /ConfigurationRepositoryP \"" + НастройкаБазы.ПарольПользователяХранилища + "\"";
                }

                if (ТипПлатформы > 2)
                {
                    if (НастройкаБазы.ВидКлиента == 0)
                    {
                        СтрокаЗапуска = СтрокаЗапуска + " /AppAutoCheckMode";
                    }
                    else if (НастройкаБазы.ВидКлиента == 3)
                    {
                        СтрокаЗапуска = СтрокаЗапуска + " /RunModeOrdinaryApplication";
                    }
                }

                if ((ТипПлатформы != 0) && (!string.IsNullOrEmpty(НастройкаБазы.КодДоступа)))
                {
                    СтрокаЗапуска = СтрокаЗапуска + " /UC" + НастройкаБазы.КодДоступа;                    
                }
                                
            }
            else
            {                
                bool ЭтоПриложение = false;
                ЭтоПриложение = ПутьЗапускаПлатформы.Contains(".exe");
                if (!ЭтоПриложение)
                    ЭтоПриложение = ПутьЗапускаПлатформы.Contains(".com");
                if (!ЭтоПриложение)
                {
                    if (File.Exists(НастройкаБазы.ПрограммаЗапуска))
                        ПутьЗапускаПлатформы = НастройкаБазы.ПрограммаЗапуска;
                    else
                    {
                        try
                        {
                            Process.Start(ПутьЗапускаПлатформы);
                        }
                        catch
                        {
                        }
                        return;
                    }
                    СтрокаЗапуска = НастройкаБазы.Путь;
                }
                else
                    СтрокаЗапуска = НастройкаБазы.ИмяПользователя;
            }

            ProcessStartInfo ПараметрыПроцесса = new ProcessStartInfo(ПутьЗапускаПлатформы);
            ПараметрыПроцесса.Arguments = СтрокаЗапуска;
            ПараметрыПроцесса.UseShellExecute = false;			           
            Process ExecutingProcesss = Process.Start(ПараметрыПроцесса);            

            // Return ibases.v8i settings to original state
            if (StatesDB.DelFromFile || StatesDB.ChangeAppID || StatesDB.ChangeVersion)
            {
            	OriginalStatesIBases.Add(StatesDB);
            	Thread ChangeOriginalStatesDB = new Thread(ReturnOriginalStateIbases);
            	ChangeOriginalStatesDB.Start();                        	
            }                     
        }
        
        static void ReturnOriginalStateIbases()
        {
        	Пауза(15000);
        	
        	int Index = 0;
        	
        	while (Index < OriginalStatesIBases.Count)
        	{
        		bool SuccessDel = true;
        		bool SuccessAppID = true;
        		bool SuccessVersion = true;
        		
        		StructChangeStateIbases StatesDB = OriginalStatesIBases[Index];
        		
        		if (StatesDB.DelFromFile)
            	{
	                SuccessDel = iBasesEdit.DeleteBaseFromFile(StatesDB.BaseType, StatesDB.BasePath);
            	}
	            else
	            {
	                if (StatesDB.ChangeAppID)
	                    SuccessAppID = iBasesEdit.SetParameterValue(StatesDB.BaseType, StatesDB.BasePath, "App", StatesDB.OriginalAppID);
	
	                if (StatesDB.ChangeVersion)
	                    SuccessVersion = iBasesEdit.SetParameterValue(StatesDB.BaseType, StatesDB.BasePath, "Version", StatesDB.OriginalVersion);
	            }
        		
	            if (SuccessDel && SuccessAppID && SuccessVersion)	            
	            	OriginalStatesIBases.Remove(StatesDB);
	            else
	            	Index++;
        	}
        	        	
        }

        static void НачатьЗапускБазыДанных(СтруктураНастроекЭлемента НастройкаБазы)
        {
            // Перед запуском выполним некоторые проверки
            string ПутьЗапускаПлатформы = "";            
            
            if (!НастройкаБазы.Приложение)
            {
                int ТипПлатформы = НастройкаБазы.ТипПлатформы;

                if (String.IsNullOrEmpty(НастройкаБазы.ПрограммаЗапуска))
                {
                    int PlatformID = 0;

                    switch (ТипПлатформы)
                    {
                        case 0:
                            PlatformID = 77;
                            break;
                        case 1:
                            PlatformID = 80;
                            break;
                        case 2:
                            PlatformID = 81;
                            break;
                        case 3:
                            PlatformID = 82;
                            break;
                        case 4:
                            PlatformID = 83;
                            break;
                    }

                    ПутьЗапускаПлатформы = ПолучитьЗначениеНастройки("ПутьЗапуска" + PlatformID, "");
                }
                else
                {
                    ПутьЗапускаПлатформы = НастройкаБазы.ПрограммаЗапуска;
                }
                
                if (!(File.Exists(ПутьЗапускаПлатформы)))
                {
                    ПоказатьИнфомационноеСообщение("Для данного типа платформы не указан или указан неверно путь запуска 1С Предприятия", null);
                    return;
                }
            }
            else
            {
                ПутьЗапускаПлатформы = НастройкаБазы.Путь;
                if (!File.Exists(ПутьЗапускаПлатформы))
                {
                    ПоказатьИнфомационноеСообщение("Запускаемый файл " + ПутьЗапускаПлатформы + " не существует!", null);
                    return;
                }
            }

            ЗапуститьБазуДанных(НастройкаБазы, ПутьЗапускаПлатформы);
        }

        private void ПунктРежимаРаботы_Click(object sender, EventArgs e)
        {            
            ToolStripItem ЭлементМеню = (ToolStripItem)sender;
            СтруктураНастроекЭлемента НастройкаБазы = (СтруктураНастроекЭлемента)ЭлементМеню.Tag;
            if (НажатSHIFT())
            {
                НастройкаБазы.ИмяПользователя = "";
                НастройкаБазы.Пароль = "";
            }
            else if (НажатCTRL())
            {
                ContextMenuStrip МенюВыбораДопПользователя = СоздатьМенюВыбораДопПользователя(НастройкаБазы);
                МенюВыбораДопПользователя.Show(MousePosition);
                return;
            }
            НачатьЗапускБазыДанных(НастройкаБазы);            
        }

        private void ПунктДопПользователь_Click(object sender, EventArgs e)
        {
            ToolStripItem ЭлементМеню = (ToolStripItem)sender;
            СтруктураНастроекЭлемента НастройкаБазы = (СтруктураНастроекЭлемента)ЭлементМеню.Tag;
            if (НажатSHIFT())
            {
                НастройкаБазы.ИмяПользователя = "";
                НастройкаБазы.Пароль = "";
            }            
            НачатьЗапускБазыДанных(НастройкаБазы);
        }

        private void ПунктРежимаЗапуска_Click(object sender, EventArgs e)
        {
            ToolStripItem ЭлементМеню = (ToolStripItem)sender;
            СтруктураНастроекЭлемента НастройкаБазы = (СтруктураНастроекЭлемента)ЭлементМеню.Tag;
            if (НажатSHIFT())
            {
                НастройкаБазы.ИмяПользователя = "";
                НастройкаБазы.Пароль = "";
            }
            else if (НажатCTRL())
            {
                if ((НастройкаБазы.ТипПлатформы != 0) | (НастройкаБазы.РежимРаботы != 0))
                {
                    ContextMenuStrip МенюВыбораДопПользователя = СоздатьМенюВыбораДопПользователя(НастройкаБазы);
                    МенюВыбораДопПользователя.Show(MousePosition);
                    return;
                }
            }
            if ((НастройкаБазы.ТипПлатформы == 0) && (НастройкаБазы.РежимЗапуска == 1) && НастройкаБазы.РежимРаботы == 0)
            {
                // 1C Предприятие 7.7 выбор режима работы (монопольный/разделенный)
                Пауза(100);
                ContextMenuStrip МенюВыбораРежимаРаботы = СоздатьМенюВыбораРежимаРаботы(НастройкаБазы);
                МенюВыбораРежимаРаботы.Show(MousePosition);
            }
            if ((НастройкаБазы.ТипПлатформы > 2) && (НастройкаБазы.ВидКлиента == 1) && (НастройкаБазы.РежимЗапуска == 1))
            {
                // Меню с выбором клиента
                Пауза(100);
                ContextMenuStrip МенюВыбораКлиента = СоздатьМенюВыбораКлиента(НастройкаБазы);
                МенюВыбораКлиента.Show(MousePosition);
            }
            else
                НачатьЗапускБазыДанных(НастройкаБазы);
        }

        private void ПунктВидаКлиента_Click(object sender, EventArgs e)
        {
            ToolStripItem ЭлементМеню = (ToolStripItem)sender;
            СтруктураНастроекЭлемента НастройкаБазы = (СтруктураНастроекЭлемента)ЭлементМеню.Tag;
            if (НажатSHIFT())
            {
                НастройкаБазы.ИмяПользователя = "";
                НастройкаБазы.Пароль = "";
            }
            else if (НажатCTRL())
            {
                if ((НастройкаБазы.ТипПлатформы != 0) | (НастройкаБазы.РежимРаботы != 0))
                {
                    ContextMenuStrip МенюВыбораДопПользователя = СоздатьМенюВыбораДопПользователя(НастройкаБазы);
                    МенюВыбораДопПользователя.Show(MousePosition);
                    return;
                }
            }
            
            НачатьЗапускБазыДанных(НастройкаБазы);
        }     
          

        static void ПоказатьМониторПользователей(СтруктураНастроекЭлемента НастройкаБазы)
        {
            ФормаМонитора Монитор = new ФормаМонитора((СтруктураНастроекЭлемента)НастройкаБазы);
            Монитор.Show();
            Монитор.Мониторинг();
            Монитор.ЗаполнениеФормы();
        }

        static void МониторПользователей_Click(object sender, EventArgs e)
        {
            ToolStripItem ЭлементМеню = (ToolStripItem)sender;
            СтруктураНастроекЭлемента НастройкаБазы = (СтруктураНастроекЭлемента)ЭлементМеню.Tag;
            ПоказатьМониторПользователей(НастройкаБазы);
        }
        
        private ContextMenuStrip СоздатьМенюВыбораДопПользователя(СтруктураНастроекЭлемента НастройкаБазы)
        {            
            ContextMenuStrip МенюВыбораРежимаРаботы = new ContextMenuStrip();
            if (НастройкаБазы.ДополнительныеПользователи.Items.Count == 0)
                ПоказатьИнфомационноеСообщение("Для данной базы данных не указаны дополнительные пользователи!", this);
            else
            {
                ToolStripItem ЭлементМенюПользователь;
                foreach (ListViewItem Пользователь in НастройкаБазы.ДополнительныеПользователи.Items)
                {
                    ЭлементМенюПользователь = МенюВыбораРежимаРаботы.Items.Add(Пользователь.Text);
                    НастройкаБазы.ИмяПользователя = Пользователь.Text;
                    НастройкаБазы.Пароль = Пользователь.SubItems[1].Text;
                    ЭлементМенюПользователь.Click += new EventHandler(ПунктДопПользователь_Click);
                    ЭлементМенюПользователь.Tag = НастройкаБазы;
                    if (СобственноеОформление)
                    {
                        ЭлементМенюПользователь.BackColor = СтруктураОформления.ЦветФона;
                        ЭлементМенюПользователь.ForeColor = Color.Transparent;
                        ЭлементМенюПользователь.Font = new Font(FontFamily.GenericSansSerif, СтруктураОформления.РазмерШрифта, СтруктураОформления.НачертаниеШрифта);
                        ЭлементМенюПользователь.Paint += new PaintEventHandler(ПрорисовкаМенюДопПользователь);
                    }
                    else
                        ЭлементМенюПользователь.Image = Properties.Resources.ДопПользователь;
                }
            }

            return МенюВыбораРежимаРаботы;
        }

        // Процедура создает меню выбора режима работы для 7.7
        // Происходит в случае установленного режима у базы "Запрашивать";
        private ContextMenuStrip СоздатьМенюВыбораРежимаРаботы(СтруктураНастроекЭлемента НастройкаБазы)
        {
            ContextMenuStrip МенюВыбораРежимаРаботы = new ContextMenuStrip();
            ToolStripItem ЭлементМенюРежимаРаботы;
            // Монопольный режим
            ЭлементМенюРежимаРаботы = МенюВыбораРежимаРаботы.Items.Add("Монопольный");
            НастройкаБазы.РежимРаботы = 1;
            ЭлементМенюРежимаРаботы.Tag = НастройкаБазы;
            ЭлементМенюРежимаРаботы.Click += new EventHandler(ПунктРежимаРаботы_Click);            
            if (СобственноеОформление)
            {
                ЭлементМенюРежимаРаботы.BackColor = СтруктураОформления.ЦветФона;
                ЭлементМенюРежимаРаботы.ForeColor = Color.Transparent;
                ЭлементМенюРежимаРаботы.Font = new Font(FontFamily.GenericSansSerif, СтруктураОформления.РазмерШрифта, СтруктураОформления.НачертаниеШрифта);
                ЭлементМенюРежимаРаботы.Paint += new PaintEventHandler(ПрорисовкаМенюЗапускаРежимРаботы);
            }
            else
                ЭлементМенюРежимаРаботы.Image = TestMenuPopup.Properties.Resources.Монопольный;
            // Разделенный режим
            ЭлементМенюРежимаРаботы = МенюВыбораРежимаРаботы.Items.Add("Разделенный");
            НастройкаБазы.РежимРаботы = 2;
            ЭлементМенюРежимаРаботы.Tag = НастройкаБазы;
            ЭлементМенюРежимаРаботы.Click += new EventHandler(ПунктРежимаРаботы_Click);            
            if (СобственноеОформление)
            {
                ЭлементМенюРежимаРаботы.BackColor = СтруктураОформления.ЦветФона;
                ЭлементМенюРежимаРаботы.ForeColor = Color.Transparent;
                ЭлементМенюРежимаРаботы.Font = new Font(FontFamily.GenericSansSerif, СтруктураОформления.РазмерШрифта, СтруктураОформления.НачертаниеШрифта);
                ЭлементМенюРежимаРаботы.Paint += new PaintEventHandler(ПрорисовкаМенюЗапускаРежимРаботы);
            }
            else
                ЭлементМенюРежимаРаботы.Image = TestMenuPopup.Properties.Resources.Разделенный;

            return МенюВыбораРежимаРаботы;
        }

        // Процедура создает меню выбора режима запуска.
        // Происходит в случае установленного режима у базы "Запрашивать";
        private ContextMenuStrip СоздатьМенюВыбораРежимаЗапуска(СтруктураНастроекЭлемента НастройкаБазы)
        {
            int ТипПлатформы = НастройкаБазы.ТипПлатформы;
            ContextMenuStrip МенюВыбораРежима = new ContextMenuStrip();
            ToolStripItem ЭлементМенюРежима;
            if (НастройкаБазы.ТипПлатформы == 0)
            {
                // 1C Предприятие 7.7
                ЭлементМенюРежима = МенюВыбораРежима.Items.Add("1С Предприятие");
                НастройкаБазы.РежимЗапуска = 1;
                ЭлементМенюРежима.Tag = НастройкаБазы;
                ЭлементМенюРежима.Click += new EventHandler(ПунктРежимаЗапуска_Click);                
                if (СобственноеОформление)
                {
                    ЭлементМенюРежима.BackColor = СтруктураОформления.ЦветФона;
                    ЭлементМенюРежима.ForeColor = Color.Transparent;
                    ЭлементМенюРежима.Font = new Font(FontFamily.GenericSansSerif, СтруктураОформления.РазмерШрифта, СтруктураОформления.НачертаниеШрифта);
                    ЭлементМенюРежима.Paint += new PaintEventHandler(ПрорисовкаПодПунктаМенюЗапуска);
                }
                else
                    ЭлементМенюРежима.Image = ПолучитьИконкуРежима(ТипПлатформы, 1);

                // Конфигуратор 7.7
                ЭлементМенюРежима = МенюВыбораРежима.Items.Add("Конфигуратор");
                НастройкаБазы.РежимЗапуска = 2;
                ЭлементМенюРежима.Tag = НастройкаБазы;
                ЭлементМенюРежима.Click += new EventHandler(ПунктРежимаЗапуска_Click);                
                if (СобственноеОформление)
                {
                    ЭлементМенюРежима.BackColor = СтруктураОформления.ЦветФона;
                    ЭлементМенюРежима.ForeColor = Color.Transparent;
                    ЭлементМенюРежима.Font = new Font(FontFamily.GenericSansSerif, СтруктураОформления.РазмерШрифта, СтруктураОформления.НачертаниеШрифта);
                    ЭлементМенюРежима.Paint += new PaintEventHandler(ПрорисовкаПодПунктаМенюЗапуска);
                }
                else
                    ЭлементМенюРежима.Image = ПолучитьИконкуРежима(ТипПлатформы, 2);

                // Отладчик 7.7
                ЭлементМенюРежима = МенюВыбораРежима.Items.Add("Отладчик");
                НастройкаБазы.РежимЗапуска = 3;
                ЭлементМенюРежима.Tag = НастройкаБазы;
                ЭлементМенюРежима.Click += new EventHandler(ПунктРежимаЗапуска_Click);                
                if (СобственноеОформление)
                {
                    ЭлементМенюРежима.BackColor = СтруктураОформления.ЦветФона;
                    ЭлементМенюРежима.ForeColor = Color.Transparent;
                    ЭлементМенюРежима.Font = new Font(FontFamily.GenericSansSerif, СтруктураОформления.РазмерШрифта, СтруктураОформления.НачертаниеШрифта);
                    ЭлементМенюРежима.Paint += new PaintEventHandler(ПрорисовкаПодПунктаМенюЗапуска);
                }
                else
                    ЭлементМенюРежима.Image = ПолучитьИконкуРежима(ТипПлатформы, 3);

                // Монитор 7.7
                ЭлементМенюРежима = МенюВыбораРежима.Items.Add("Монитор");
                НастройкаБазы.РежимЗапуска = 4;
                ЭлементМенюРежима.Tag = НастройкаБазы;
                ЭлементМенюРежима.Click += new EventHandler(ПунктРежимаЗапуска_Click);                
                if (СобственноеОформление)
                {
                    ЭлементМенюРежима.BackColor = СтруктураОформления.ЦветФона;
                    ЭлементМенюРежима.ForeColor = Color.Transparent;
                    ЭлементМенюРежима.Font = new Font(FontFamily.GenericSansSerif, СтруктураОформления.РазмерШрифта, СтруктураОформления.НачертаниеШрифта);
                    ЭлементМенюРежима.Paint += new PaintEventHandler(ПрорисовкаПодПунктаМенюЗапуска);
                }
                else
                    ЭлементМенюРежима.Image = ПолучитьИконкуРежима(ТипПлатформы, 4);
            }
            else
            {
                // 1С Предприятие 8.х
                ЭлементМенюРежима = МенюВыбораРежима.Items.Add("1С Предприятие");
                НастройкаБазы.РежимЗапуска = 1;
                ЭлементМенюРежима.Tag = НастройкаБазы;                
                if (СобственноеОформление)
                {
                    ЭлементМенюРежима.BackColor = СтруктураОформления.ЦветФона;
                    ЭлементМенюРежима.ForeColor = Color.Transparent;
                    ЭлементМенюРежима.Font = new Font(FontFamily.GenericSansSerif, СтруктураОформления.РазмерШрифта, СтруктураОформления.НачертаниеШрифта);
                    ЭлементМенюРежима.Paint += new PaintEventHandler(ПрорисовкаПодПунктаМенюЗапуска);
                }
                else
                    ЭлементМенюРежима.Image = ПолучитьИконкуРежима(ТипПлатформы, 1);
                
                // Если это 8.2 возможно надо добавить подменю клиента
                if ((НастройкаБазы.ТипПлатформы > 2) && (НастройкаБазы.ВидКлиентаКакПунктМеню))
                {
                    ToolStripMenuItem Подменю1СПредприятия = (ToolStripMenuItem)ЭлементМенюРежима;
                    ToolStripItem ЭлементСВидомКлиента;
                    // Автоматический режим
                    ЭлементСВидомКлиента = Подменю1СПредприятия.DropDownItems.Add("Автоматически");
                    НастройкаБазы.ВидКлиента = 0;
                    ЭлементСВидомКлиента.Tag = НастройкаБазы;
                    ЭлементСВидомКлиента.Click += new EventHandler(ПунктВидаКлиента_Click);
                    if (СобственноеОформление)
                    {
                        ЭлементСВидомКлиента.BackColor = СтруктураОформления.ЦветФона;
                        ЭлементСВидомКлиента.ForeColor = Color.Transparent;
                        ЭлементСВидомКлиента.Paint += new PaintEventHandler(ПрорисовкаПодПунктаВидаКлиента);
                    }
                    // Тонкий клиент
                    ЭлементСВидомКлиента = Подменю1СПредприятия.DropDownItems.Add("Тонкий клиент");
                    НастройкаБазы.ВидКлиента = 2;
                    ЭлементСВидомКлиента.Tag = НастройкаБазы;
                    ЭлементСВидомКлиента.Click += new EventHandler(ПунктВидаКлиента_Click);
                    if (СобственноеОформление)
                    {
                        ЭлементСВидомКлиента.BackColor = СтруктураОформления.ЦветФона;
                        ЭлементСВидомКлиента.ForeColor = Color.Transparent;
                        ЭлементСВидомКлиента.Paint += new PaintEventHandler(ПрорисовкаПодПунктаВидаКлиента);
                    }
                    // Толстый клиент
                    ЭлементСВидомКлиента = Подменю1СПредприятия.DropDownItems.Add("Толстый клиент");
                    НастройкаБазы.ВидКлиента = 3;
                    ЭлементСВидомКлиента.Tag = НастройкаБазы;
                    ЭлементСВидомКлиента.Click += new EventHandler(ПунктВидаКлиента_Click);
                    if (СобственноеОформление)
                    {
                        ЭлементСВидомКлиента.BackColor = СтруктураОформления.ЦветФона;
                        ЭлементСВидомКлиента.ForeColor = Color.Transparent;
                        ЭлементСВидомКлиента.Paint += new PaintEventHandler(ПрорисовкаПодПунктаВидаКлиента);
                    }
                }
                else
                    ЭлементМенюРежима.Click += new EventHandler(ПунктРежимаЗапуска_Click);                                        
                                
                // Конфигуратор 8.х
                ЭлементМенюРежима = МенюВыбораРежима.Items.Add("Конфигуратор");
                НастройкаБазы.РежимЗапуска = 2;
                ЭлементМенюРежима.Tag = НастройкаБазы;
                ЭлементМенюРежима.Click += new EventHandler(ПунктРежимаЗапуска_Click);                
                if (СобственноеОформление)
                {
                    ЭлементМенюРежима.BackColor = СтруктураОформления.ЦветФона;
                    ЭлементМенюРежима.ForeColor = Color.Transparent;
                    ЭлементМенюРежима.Font = new Font(FontFamily.GenericSansSerif, СтруктураОформления.РазмерШрифта, СтруктураОформления.НачертаниеШрифта);
                    ЭлементМенюРежима.Paint += new PaintEventHandler(ПрорисовкаПодПунктаМенюЗапуска);
                }
                else
                    ЭлементМенюРежима.Image = ПолучитьИконкуРежима(ТипПлатформы, 2);
                
               	// Монитор 8.x
                ЭлементМенюРежима = МенюВыбораРежима.Items.Add("Монитор пользователей");
                НастройкаБазы.РежимЗапуска = 4;
                ЭлементМенюРежима.Tag = НастройкаБазы;
                ЭлементМенюРежима.Click += new EventHandler(МониторПользователей_Click);                
                if (СобственноеОформление)
                {
                    ЭлементМенюРежима.BackColor = СтруктураОформления.ЦветФона;
                    ЭлементМенюРежима.ForeColor = Color.Transparent;
                    ЭлементМенюРежима.Font = new Font(FontFamily.GenericSansSerif, СтруктураОформления.РазмерШрифта, СтруктураОформления.НачертаниеШрифта);
                    ЭлементМенюРежима.Paint += new PaintEventHandler(ПрорисовкаПодПунктаМенюЗапуска);
                }
                else
                    ЭлементМенюРежима.Image = ПолучитьИконкуРежима(ТипПлатформы, 4);
            }

            return МенюВыбораРежима;
        }

        // Процедура создает меню выбора клиента.
        // Происходит в случае установленного режима у базы "Запрашивать";
        private ContextMenuStrip СоздатьМенюВыбораКлиента(СтруктураНастроекЭлемента НастройкаБазы)
        {            
            ContextMenuStrip МенюВыбораКлиента = new ContextMenuStrip();
            ToolStripItem ЭлементМенюКлиента;

            // Автоматический режим
            ЭлементМенюКлиента = МенюВыбораКлиента.Items.Add("Автоматически");
            НастройкаБазы.ВидКлиента = 0;
            ЭлементМенюКлиента.Tag = НастройкаБазы;
            ЭлементМенюКлиента.Click += new EventHandler(ПунктВидаКлиента_Click);
            if (СобственноеОформление)
            {
                ЭлементМенюКлиента.BackColor = СтруктураОформления.ЦветФона;
                ЭлементМенюКлиента.ForeColor = Color.Transparent;
                ЭлементМенюКлиента.Paint += new PaintEventHandler(ПрорисовкаПодПунктаВидаКлиента);
            }
            // Тонкий клиент
            ЭлементМенюКлиента = МенюВыбораКлиента.Items.Add("Тонкий клиент");
            НастройкаБазы.ВидКлиента = 2;
            ЭлементМенюКлиента.Tag = НастройкаБазы;
            ЭлементМенюКлиента.Click += new EventHandler(ПунктВидаКлиента_Click);
            if (СобственноеОформление)
            {
                ЭлементМенюКлиента.BackColor = СтруктураОформления.ЦветФона;
                ЭлементМенюКлиента.ForeColor = Color.Transparent;
                ЭлементМенюКлиента.Paint += new PaintEventHandler(ПрорисовкаПодПунктаВидаКлиента);
            }
            // Толстый клиент
            ЭлементМенюКлиента = МенюВыбораКлиента.Items.Add("Толстый клиент");
            НастройкаБазы.ВидКлиента = 3;
            ЭлементМенюКлиента.Tag = НастройкаБазы;
            ЭлементМенюКлиента.Click += new EventHandler(ПунктВидаКлиента_Click);
            if (СобственноеОформление)
            {
                ЭлементМенюКлиента.BackColor = СтруктураОформления.ЦветФона;
                ЭлементМенюКлиента.ForeColor = Color.Transparent;
                ЭлементМенюКлиента.Paint += new PaintEventHandler(ПрорисовкаПодПунктаВидаКлиента);
            }

            return МенюВыбораКлиента;
        }

        // Процедура Изменения/Удаления базы из меню запуска
        //
        private void КликКонтекстногоМенюБаза_Click(object sender, EventArgs e)
        {            
            ToolStripItem ЭлементМеню = (ToolStripItem)sender;
            СтруктураНастроекЭлемента НастройкаБазы = (СтруктураНастроекЭлемента)ЭлементМеню.Tag;
            if (НастройкаБазы.РежимЗапуска == 10)
            {
                // Редактирование базы данных                
                ОтменитьРегистрациюВсехГорячихКлавиш(УзелБазДанных, 0);
                if (!НастройкаБазы.Приложение)
                {
                    ФормаБазыДанных ФормаНастроек = new ФормаБазыДанных();
                    ФормаНастроек.ОткрытьБазуДанных(НастройкаБазы.Группа, НастройкаБазы.Ссылка, false);
                    XmlNode СтарыйРодитель = НастройкаБазы.Группа;
                    if (ФормаНастроек.ShowDialog() == DialogResult.OK)
                    {
                        НастройкаБазы = ФормаНастроек.ТекущаяНастройка;
                        ИзменитьНастройкуБазыДанных((XmlElement)НастройкаБазы.Ссылка, НастройкаБазы);
                        КорректировкаГорячихКлавиш((XmlElement)НастройкаБазы.Ссылка, УзелБазДанных);

                        XmlElement РодительБазы = (XmlElement)ФормаНастроек.РодительБазыДанных;
                        if (РодительБазы != (XmlElement)СтарыйРодитель)
                        {
                            ПоменятьРодителяЭлемента(СтарыйРодитель, (XmlNode)РодительБазы, НастройкаБазы.Ссылка);
                        }
                        СохранитьНастройкиПрограммы();
                        ИнициализацияМенюЗапуска(МенюЗапуска);
                    }
                    ФормаНастроек.Dispose();
                }
                else
                {
                    ФормаВнешнегоПриложения ФормаНастроек = new ФормаВнешнегоПриложения();
                    ФормаНастроек.ОткрытьПриложение(НастройкаБазы.Группа, НастройкаБазы.Ссылка, false);
                    XmlNode СтарыйРодитель = НастройкаБазы.Группа;
                    if (ФормаНастроек.ShowDialog() == DialogResult.OK)
                    {
                        НастройкаБазы = ФормаНастроек.ТекущаяНастройка;
                        ИзменитьНастройкуБазыДанных((XmlElement)НастройкаБазы.Ссылка, НастройкаБазы);
                        КорректировкаГорячихКлавиш((XmlElement)НастройкаБазы.Ссылка, УзелБазДанных);

                        XmlElement РодительБазы = (XmlElement)ФормаНастроек.РодительБазыДанных;
                        if (РодительБазы != (XmlElement)СтарыйРодитель)
                        {
                            ПоменятьРодителяЭлемента(СтарыйРодитель, (XmlNode)РодительБазы, НастройкаБазы.Ссылка);
                        }
                        СохранитьНастройкиПрограммы();
                        ИнициализацияМенюЗапуска(МенюЗапуска);
                    }
                    ФормаНастроек.Dispose();
                }
                ЗарегистрироватьВсеГорячиеКлавиши(УзелБазДанных, 0);
            }
            else if (НастройкаБазы.РежимЗапуска == 11)
            {
                // Удаление базы данных
                string ТекстВопроса = String.Empty;
                if (НастройкаБазы.Приложение)
                    ТекстВопроса = "Удалить выбранное внешнее приложение?";
                else
                    ТекстВопроса = "Удалить выбранную базу данных?";
                if (Вопрос(ТекстВопроса, null) == DialogResult.Yes)
                {
                    УдалитьЭлементНастройки((XmlElement)НастройкаБазы.Группа, (XmlElement)НастройкаБазы.Ссылка);
                    СохранитьНастройкиПрограммы();
                    ИнициализацияМенюЗапуска(МенюЗапуска);
                }
            }
            else
            {
                if (НажатSHIFT())
                {
                    НастройкаБазы.ИмяПользователя = "";
                    НастройкаБазы.Пароль = "";
                }                
                НачатьЗапускБазыДанных(НастройкаБазы);
            }
        }

        // Процедура Изменения/Удаления группы из меню запуска
        //
        private void КликКонтекстногоМенюГруппа_Click(object sender, EventArgs e)
        {
            ToolStripItem ЭлементМеню = (ToolStripItem)sender;
            СтруктураНастроекГруппыЭлементов НастройкаГруппы = (СтруктураНастроекГруппыЭлементов)ЭлементМеню.Tag;
            XmlNode Ссылка = НастройкаГруппы.Ссылка;
            if (НастройкаГруппы.Действие == 10)
            {                
                // Редактирование группы                
                ОтменитьРегистрациюВсехГорячихКлавиш(УзелБазДанных, 0);
                ФормаГруппы ФормаРедактированияГруппы = new ФормаГруппы();
                ФормаРедактированияГруппы.ОткрытьГруппу(Ссылка);
                ФормаРедактированияГруппы.ShowDialog();
                if (ФормаРедактированияГруппы.DialogResult == DialogResult.OK)
                {
                    СтруктураНастроекГруппыЭлементов НастройкиГруппы = ФормаРедактированияГруппы.ТекущаяБаза;                    
                    ИзменитьНастройкуГруппы((XmlElement)Ссылка, НастройкиГруппы);
                }
                ФормаРедактированияГруппы.Dispose();
                СохранитьНастройкиПрограммы();
                ЗарегистрироватьВсеГорячиеКлавиши(УзелБазДанных, 0);
                ИнициализацияМенюЗапуска(МенюЗапуска);
            }
            else
            {
                // Удаление группы
                if (Вопрос("Удалить выбранную группу?", null) == DialogResult.Yes)
                {
                    if (ГруппаИмеетВложенныеБазы(Ссылка, false))
                    {
                        if (Вопрос("Группа имеет вложенные базы данных. Все равно продолжить?", null) == DialogResult.No)
                        {
                            return;
                        }
                    }                    
                    УдалитьЭлементНастройки((XmlElement)НастройкаГруппы.Группа, (XmlElement)Ссылка);
                    СохранитьНастройкиПрограммы();
                    ИнициализацияМенюЗапуска(МенюЗапуска);
                }
            }
        }

        private ContextMenuStrip СоздатьКонтекстноеМенюДляМенюЗапуска(ToolStripItem ЭлементМеню, СтруктураНастроекЭлемента НастройкаБазы, СтруктураНастроекГруппыЭлементов НастройкаГруппы)
        {
            // Создадим контекстное менб для меню запуска
            ContextMenuStrip КонтекстноеМеню = new ContextMenuStrip();
            ToolStripItem ЭлементКонтекстногоМеню;
            if (!(String.IsNullOrEmpty(НастройкаБазы.Наименование)))
            {
                // Для базы данных
                ЭлементКонтекстногоМеню = КонтекстноеМеню.Items.Add("Изменить");                
                НастройкаБазы.РежимЗапуска = 10; // Изменение пункта
                ЭлементКонтекстногоМеню.Tag = НастройкаБазы;
                ЭлементКонтекстногоМеню.Click += new EventHandler(КликКонтекстногоМенюБаза_Click);
                if (СобственноеОформление)
                {
                    ЭлементКонтекстногоМеню.BackColor = СтруктураОформления.ЦветФона;
                    ЭлементКонтекстногоМеню.ForeColor = Color.Transparent;
                    ЭлементКонтекстногоМеню.Font = new Font(FontFamily.GenericSansSerif, СтруктураОформления.РазмерШрифта, СтруктураОформления.НачертаниеШрифта);
                    ЭлементКонтекстногоМеню.Paint += new PaintEventHandler(ПрорисовкаЭлементаКонтекстногоМеню);
                }
                else
                    ЭлементКонтекстногоМеню.Image = Properties.Resources.SmallEdit;

                ЭлементКонтекстногоМеню = КонтекстноеМеню.Items.Add("Удалить");                
                НастройкаБазы.РежимЗапуска = 11; // Удаление пункта
                ЭлементКонтекстногоМеню.Tag = НастройкаБазы;
                ЭлементКонтекстногоМеню.Click += new EventHandler(КликКонтекстногоМенюБаза_Click);
                if (СобственноеОформление)
                {
                    ЭлементКонтекстногоМеню.BackColor = СтруктураОформления.ЦветФона;
                    ЭлементКонтекстногоМеню.ForeColor = Color.Transparent;
                    ЭлементКонтекстногоМеню.Font = new Font(FontFamily.GenericSansSerif, СтруктураОформления.РазмерШрифта, СтруктураОформления.НачертаниеШрифта);
                    ЭлементКонтекстногоМеню.Paint += new PaintEventHandler(ПрорисовкаЭлементаКонтекстногоМеню);
                }
                else
                    ЭлементКонтекстногоМеню.Image = Properties.Resources.SmallRemove;            

                if (!НастройкаБазы.Приложение)
                {

                    if (СобственноеОформление)
                    {
                        // Прортсовываем собственный разделитель                            
                        ЭлементКонтекстногоМеню = КонтекстноеМеню.Items.Add("");                        
                        ЭлементКонтекстногоМеню.BackColor = СтруктураОформления.ЦветСепаратора;
                        ЭлементКонтекстногоМеню.Size = new Size(400, 1);
                        ЭлементКонтекстногоМеню.AutoSize = false;                        
                    }
                    else
                        ЭлементКонтекстногоМеню = КонтекстноеМеню.Items.Add("-");
                    
                    // Монитор пользователей
                    ЭлементКонтекстногоМеню = КонтекстноеМеню.Items.Add("Монитор пользователей");
                    ЭлементКонтекстногоМеню.Tag = НастройкаБазы;
                    ЭлементКонтекстногоМеню.Image = Properties.Resources.МониторПользователей;
                    ЭлементКонтекстногоМеню.Click += new EventHandler(МониторПользователей_Click);
                    if (СобственноеОформление)
                    {
                        ЭлементКонтекстногоМеню.BackColor = СтруктураОформления.ЦветФона;
                        ЭлементКонтекстногоМеню.ForeColor = Color.Transparent;
                        ЭлементКонтекстногоМеню.Font = new Font(FontFamily.GenericSansSerif, СтруктураОформления.РазмерШрифта, СтруктураОформления.НачертаниеШрифта);
                        ЭлементКонтекстногоМеню.Paint += new PaintEventHandler(ПрорисовкаЭлементаКонтекстногоМенюМонитор);
                    }
                    else
                        ЭлементКонтекстногоМеню.Image = Properties.Resources.МониторПользователей;
                    
                    if (СобственноеОформление)
                    {
                        // Прортсовываем собственный разделитель                                                        
                        ЭлементКонтекстногоМеню = КонтекстноеМеню.Items.Add("");                            
                        ЭлементКонтекстногоМеню.BackColor = СтруктураОформления.ЦветСепаратора;
                        ЭлементКонтекстногоМеню.Size = new Size(400, 1);
                        ЭлементКонтекстногоМеню.AutoSize = false;  
                    }
                    else
                        ЭлементКонтекстногоМеню = КонтекстноеМеню.Items.Add("-");
                   

                    if (НастройкаБазы.ТипПлатформы == 0)
                    {
                        // 1С Предприятие 7.7
                        ЭлементКонтекстногоМеню = КонтекстноеМеню.Items.Add("1С Предприятие");
                        НастройкаБазы.РежимЗапуска = 1;
                        ЭлементКонтекстногоМеню.Tag = НастройкаБазы;                       
                        if (СобственноеОформление)
                        {
                            ЭлементКонтекстногоМеню.BackColor = СтруктураОформления.ЦветФона;
                            ЭлементКонтекстногоМеню.ForeColor = Color.Transparent;
                            ЭлементКонтекстногоМеню.Paint += new PaintEventHandler(ПрорисовкаПодПунктаМенюЗапуска);
                        }
                        else
                            ЭлементКонтекстногоМеню.Image = ПолучитьИконкуРежима(0, 1);
                        // Создание подменю с выбором режима работы
                        ToolStripMenuItem Подменю1СПредприятия = (ToolStripMenuItem)ЭлементКонтекстногоМеню;

                        // Монопольный
                        ЭлементКонтекстногоМеню = Подменю1СПредприятия.DropDownItems.Add("Монопольный");
                        НастройкаБазы.РежимРаботы = 1;
                        ЭлементКонтекстногоМеню.Tag = НастройкаБазы;                        
                        ЭлементКонтекстногоМеню.Click += new EventHandler(КликКонтекстногоМенюБаза_Click);
                        if (СобственноеОформление)
                        {
                            ЭлементКонтекстногоМеню.BackColor = СтруктураОформления.ЦветФона;
                            ЭлементКонтекстногоМеню.ForeColor = Color.Transparent;
                            ЭлементКонтекстногоМеню.Font = new Font(FontFamily.GenericSansSerif, СтруктураОформления.РазмерШрифта, СтруктураОформления.НачертаниеШрифта);
                            ЭлементКонтекстногоМеню.Paint += new PaintEventHandler(ПрорисовкаМенюЗапускаРежимРаботы);
                        }
                        else
                            ЭлементКонтекстногоМеню.Image = Properties.Resources.Монопольный;

                        // Разделенный
                        ЭлементКонтекстногоМеню = Подменю1СПредприятия.DropDownItems.Add("Разделенный");
                        НастройкаБазы.РежимРаботы = 2;
                        ЭлементКонтекстногоМеню.Tag = НастройкаБазы;                        
                        ЭлементКонтекстногоМеню.Click += new EventHandler(КликКонтекстногоМенюБаза_Click);
                        if (СобственноеОформление)
                        {
                            ЭлементКонтекстногоМеню.BackColor = СтруктураОформления.ЦветФона;
                            ЭлементКонтекстногоМеню.ForeColor = Color.Transparent;
                            ЭлементКонтекстногоМеню.Font = new Font(FontFamily.GenericSansSerif, СтруктураОформления.РазмерШрифта, СтруктураОформления.НачертаниеШрифта);
                            ЭлементКонтекстногоМеню.Paint += new PaintEventHandler(ПрорисовкаМенюЗапускаРежимРаботы);
                        }
                        else
                            ЭлементКонтекстногоМеню.Image = Properties.Resources.Разделенный;

                        // Конфигуратор 7.7
                        ЭлементКонтекстногоМеню = КонтекстноеМеню.Items.Add("Кофигуратор");
                        НастройкаБазы.РежимЗапуска = 2;
                        ЭлементКонтекстногоМеню.Tag = НастройкаБазы;                        
                        ЭлементКонтекстногоМеню.Click += new EventHandler(КликКонтекстногоМенюБаза_Click);
                        if (СобственноеОформление)
                        {
                            ЭлементКонтекстногоМеню.BackColor = СтруктураОформления.ЦветФона;
                            ЭлементКонтекстногоМеню.ForeColor = Color.Transparent;
                            ЭлементКонтекстногоМеню.Font = new Font(FontFamily.GenericSansSerif, СтруктураОформления.РазмерШрифта, СтруктураОформления.НачертаниеШрифта);
                            ЭлементКонтекстногоМеню.Paint += new PaintEventHandler(ПрорисовкаПодПунктаМенюЗапуска);
                        }
                        else
                            ЭлементКонтекстногоМеню.Image = ПолучитьИконкуРежима(0, 2);

                        // Отладчик 7.7
                        ЭлементКонтекстногоМеню = КонтекстноеМеню.Items.Add("Отладчик");
                        НастройкаБазы.РежимЗапуска = 3;
                        ЭлементКонтекстногоМеню.Tag = НастройкаБазы;                        
                        ЭлементКонтекстногоМеню.Click += new EventHandler(КликКонтекстногоМенюБаза_Click);
                        if (СобственноеОформление)
                        {
                            ЭлементКонтекстногоМеню.BackColor = СтруктураОформления.ЦветФона;
                            ЭлементКонтекстногоМеню.ForeColor = Color.Transparent;
                            ЭлементКонтекстногоМеню.Font = new Font(FontFamily.GenericSansSerif, СтруктураОформления.РазмерШрифта, СтруктураОформления.НачертаниеШрифта);
                            ЭлементКонтекстногоМеню.Paint += new PaintEventHandler(ПрорисовкаПодПунктаМенюЗапуска);
                        }
                        else
                            ЭлементКонтекстногоМеню.Image = ПолучитьИконкуРежима(0, 3);

                        // Монитор 7.7
                        ЭлементКонтекстногоМеню = КонтекстноеМеню.Items.Add("Монитор");
                        НастройкаБазы.РежимЗапуска = 4;
                        ЭлементКонтекстногоМеню.Tag = НастройкаБазы;                        
                        ЭлементКонтекстногоМеню.Click += new EventHandler(КликКонтекстногоМенюБаза_Click);
                        if (СобственноеОформление)
                        {
                            ЭлементКонтекстногоМеню.BackColor = СтруктураОформления.ЦветФона;
                            ЭлементКонтекстногоМеню.ForeColor = Color.Transparent;
                            ЭлементКонтекстногоМеню.Font = new Font(FontFamily.GenericSansSerif, СтруктураОформления.РазмерШрифта, СтруктураОформления.НачертаниеШрифта);
                            ЭлементКонтекстногоМеню.Paint += new PaintEventHandler(ПрорисовкаПодПунктаМенюЗапуска);
                        }
                        else
                            ЭлементКонтекстногоМеню.Image = ПолучитьИконкуРежима(0, 4);
                    }
                    else
                    {
                        // 1С Предприятие 8.х
                        ЭлементКонтекстногоМеню = КонтекстноеМеню.Items.Add("1С Предприятие");
                        НастройкаБазы.РежимЗапуска = 1;
                        ЭлементКонтекстногоМеню.Tag = НастройкаБазы;                        
                        ЭлементКонтекстногоМеню.Click += new EventHandler(КликКонтекстногоМенюБаза_Click);
                        if (СобственноеОформление)
                        {
                            ЭлементКонтекстногоМеню.BackColor = СтруктураОформления.ЦветФона;
                            ЭлементКонтекстногоМеню.ForeColor = Color.Transparent;
                            ЭлементКонтекстногоМеню.Font = new Font(FontFamily.GenericSansSerif, СтруктураОформления.РазмерШрифта, СтруктураОформления.НачертаниеШрифта);
                            ЭлементКонтекстногоМеню.Paint += new PaintEventHandler(ПрорисовкаПодПунктаМенюЗапуска);
                        }
                        else
                            ЭлементКонтекстногоМеню.Image = ПолучитьИконкуРежима(НастройкаБазы.ТипПлатформы, 1);
                        // Кофигуратор 8.х
                        ЭлементКонтекстногоМеню = КонтекстноеМеню.Items.Add("Конфигуратор");
                        НастройкаБазы.РежимЗапуска = 2;
                        ЭлементКонтекстногоМеню.Tag = НастройкаБазы;                        
                        ЭлементКонтекстногоМеню.Click += new EventHandler(КликКонтекстногоМенюБаза_Click);
                        if (СобственноеОформление)
                        {
                            ЭлементКонтекстногоМеню.BackColor = СтруктураОформления.ЦветФона;
                            ЭлементКонтекстногоМеню.ForeColor = Color.Transparent;
                            ЭлементКонтекстногоМеню.Font = new Font(FontFamily.GenericSansSerif, СтруктураОформления.РазмерШрифта, СтруктураОформления.НачертаниеШрифта);
                            ЭлементКонтекстногоМеню.Paint += new PaintEventHandler(ПрорисовкаПодПунктаМенюЗапуска);
                        }
                        else
                            ЭлементКонтекстногоМеню.Image = ПолучитьИконкуРежима(НастройкаБазы.ТипПлатформы, 2);
                    }
                }
            }
            else
            {
                // Для группы базы данных
                ЭлементКонтекстногоМеню = КонтекстноеМеню.Items.Add("Изменить");
                НастройкаГруппы.Действие = 10; // Изменение пункта
                ЭлементКонтекстногоМеню.Tag = НастройкаГруппы;
                ЭлементКонтекстногоМеню.Click += new EventHandler(КликКонтекстногоМенюГруппа_Click);
                if (СобственноеОформление)
                {
                    ЭлементКонтекстногоМеню.BackColor = СтруктураОформления.ЦветФона;
                    ЭлементКонтекстногоМеню.ForeColor = Color.Transparent;
                    ЭлементКонтекстногоМеню.Font = new Font(FontFamily.GenericSansSerif, СтруктураОформления.РазмерШрифта, СтруктураОформления.НачертаниеШрифта);
                    ЭлементКонтекстногоМеню.Paint += new PaintEventHandler(ПрорисовкаЭлементаКонтекстногоМенюГруппа);
                }
                else
                    ЭлементКонтекстногоМеню.Image = Properties.Resources.SmallEdit;

                ЭлементКонтекстногоМеню = КонтекстноеМеню.Items.Add("Удалить");
                НастройкаГруппы.Действие = 11; // Удаление пункта
                ЭлементКонтекстногоМеню.Tag = НастройкаГруппы;
                ЭлементКонтекстногоМеню.Click += new EventHandler(КликКонтекстногоМенюГруппа_Click);
                if (СобственноеОформление)
                {
                    ЭлементКонтекстногоМеню.BackColor = СтруктураОформления.ЦветФона;
                    ЭлементКонтекстногоМеню.ForeColor = Color.Transparent;
                    ЭлементКонтекстногоМеню.Font = new Font(FontFamily.GenericSansSerif, СтруктураОформления.РазмерШрифта, СтруктураОформления.НачертаниеШрифта);
                    ЭлементКонтекстногоМеню.Paint += new PaintEventHandler(ПрорисовкаЭлементаКонтекстногоМенюГруппа);
                }
                else
                    ЭлементКонтекстногоМеню.Image = Properties.Resources.SmallRemove;            
            }

            return КонтекстноеМеню;
        }

        // Процедура обработки клика на пункте меню запуска
        private void ПунктМенюЗапуска_MouseClick(object sender, MouseEventArgs e)
        {
            ToolStripItem ЭлементМеню = (ToolStripItem)sender;
            СтруктураНастроекЭлемента НастройкаБазы = (СтруктураНастроекЭлемента)ЭлементМеню.Tag;
            if (e.Button == MouseButtons.Left)
            {
                if (!НастройкаБазы.Приложение)
                {
                    if (НажатSHIFT())
                    {
                        НастройкаБазы.ИмяПользователя = "";
                        НастройкаБазы.Пароль = "";
                    }
                    if (НастройкаБазы.РежимЗапуска == 0)
                    {
                        // Формируем новое меню с режимами запуска
                        SetForegroundWindow((IntPtr)ЭтаФорма.Handle);
                        Пауза(100);
                        ContextMenuStrip МенюВыбораРежима = СоздатьМенюВыбораРежимаЗапуска(НастройкаБазы);
                        МенюВыбораРежима.Show(MousePosition);
                    }
                    else if ((НастройкаБазы.ВидКлиента == 1) && (НастройкаБазы.ТипПлатформы > 2) && (НастройкаБазы.РежимЗапуска == 1))
                    {
                        // Формируем меню с выбором клиента
                        SetForegroundWindow((IntPtr)ЭтаФорма.Handle);
                        Пауза(100);
                        ContextMenuStrip МенюВыбораКлиента = СоздатьМенюВыбораКлиента(НастройкаБазы);
                        МенюВыбораКлиента.Show(MousePosition);
                    }
                    else
                    {
                        НачатьЗапускБазыДанных(НастройкаБазы);
                    }
                }
                else
                {
                    НачатьЗапускБазыДанных(НастройкаБазы);
                }
            }
            else if (e.Button == MouseButtons.Right)
            {
                // Создадим контекстное меню для меню запуска
                SetForegroundWindow((IntPtr)ЭтаФорма.Handle);
                Пауза(100);
                СтруктураНастроекГруппыЭлементов ПустаяНастройкаГруппы = new СтруктураНастроекГруппыЭлементов();
                ContextMenuStrip КонтекстноеМеню = СоздатьКонтекстноеМенюДляМенюЗапуска(ЭлементМеню, НастройкаБазы, ПустаяНастройкаГруппы);
                SetForegroundWindow((IntPtr)ЭтаФорма.Handle);
                КонтекстноеМеню.Show(MousePosition);
            }
        }

        private void ПодпунктМенюЗапуска_MouseClick(object sender, MouseEventArgs e)
        {
            ToolStripItem ЭлементМеню = (ToolStripItem)sender;
            СтруктураНастроекЭлемента НастройкаБазы = (СтруктураНастроекЭлемента)ЭлементМеню.Tag;
            if (e.Button == MouseButtons.Right)
            {
                Пауза(100);
                СтруктураНастроекГруппыЭлементов ПустаяНастройкаГруппы = new СтруктураНастроекГруппыЭлементов();
                ContextMenuStrip КонтекстноеМеню = СоздатьКонтекстноеМенюДляМенюЗапуска(ЭлементМеню, НастройкаБазы, ПустаяНастройкаГруппы);
                SetForegroundWindow((IntPtr)ЭтаФорма.Handle);
                КонтекстноеМеню.Show(MousePosition);
            }
        }

        // Процедура обрабатывает клик по группе в меню запуска
        //
        private void Группа_MouseClick(object sender, MouseEventArgs e)
        {
            ToolStripItem ЭлементМеню = (ToolStripItem)sender;
            СтруктураНастроекГруппыЭлементов НастройкаГруппы = (СтруктураНастроекГруппыЭлементов)ЭлементМеню.Tag;
            if (e.Button == MouseButtons.Right)
            {
                Пауза(100);
                СтруктураНастроекЭлемента ПустаяНастройкаБазы = new СтруктураНастроекЭлемента();
                ContextMenuStrip КонтекстноеМеню = СоздатьКонтекстноеМенюДляМенюЗапуска(ЭлементМеню, ПустаяНастройкаБазы, НастройкаГруппы);
                SetForegroundWindow((IntPtr)ЭтаФорма.Handle);
                КонтекстноеМеню.Show(MousePosition);
            }
        }

        private int ПолучитьПоправкуПоВысотеДляШрифта()
        {
            if (СтруктураОформления.РазмерШрифта == 12)
                return 1;
            else if (СтруктураОформления.РазмерШрифта == 13)
                return 2;
            else if (СтруктураОформления.РазмерШрифта == 14)
                return 3;
            else
                return 0;
        }

        private void ПрорисовкаПунктаМенюЗапуска(object sender, PaintEventArgs e)
        {
            ToolStripItem Отправитель = (ToolStripItem)sender;            
            СтруктураНастроекЭлемента НастройкаТекущейБазы = (СтруктураНастроекЭлемента)Отправитель.Tag;
            if (БазаДанныхСуществуетПоУказанномуПути(НастройкаТекущейБазы))
                e.Graphics.DrawImage(ПолучитьИконкуРежима(НастройкаТекущейБазы.ТипПлатформы, НастройкаТекущейБазы.РежимЗапуска), 4, 2 + ПолучитьПоправкуПоВысотеДляШрифта());
            else
                e.Graphics.DrawImage(Properties.Resources.БазаОтсутствует, 4, 2 + ПолучитьПоправкуПоВысотеДляШрифта());
            e.Graphics.DrawString(Отправитель.Text, new Font(FontFamily.GenericSansSerif, СтруктураОформления.РазмерШрифта, СтруктураОформления.НачертаниеШрифта), new SolidBrush(СтруктураОформления.ЦветШрифта) , e.ClipRectangle.X + 19, e.ClipRectangle.Y + 3);            
        }

        private void ПрорисовкаМенюЗапускаРежимРаботы(object sender, PaintEventArgs e)
        {            
            ToolStripItem Отправитель = (ToolStripItem)sender;
            СтруктураНастроекЭлемента НастройкаТекущейБазы = (СтруктураНастроекЭлемента)Отправитель.Tag;            
            if (НастройкаТекущейБазы.РежимРаботы == 1)
                e.Graphics.DrawImage(Properties.Resources.Монопольный, 4, 2 + ПолучитьПоправкуПоВысотеДляШрифта());
            else
                e.Graphics.DrawImage(Properties.Resources.Разделенный, 4, 2 + ПолучитьПоправкуПоВысотеДляШрифта());            
            e.Graphics.DrawString(Отправитель.Text, new Font(FontFamily.GenericSansSerif, СтруктураОформления.РазмерШрифта, СтруктураОформления.НачертаниеШрифта), new SolidBrush(СтруктураОформления.ЦветШрифта), e.ClipRectangle.X + 19, e.ClipRectangle.Y + 3);
        }

        private void ПрорисовкаМенюДопПользователь(object sender, PaintEventArgs e)
        {
            ToolStripItem Отправитель = (ToolStripItem)sender;
            СтруктураНастроекЭлемента НастройкаТекущейБазы = (СтруктураНастроекЭлемента)Отправитель.Tag;            
            e.Graphics.DrawImage(Properties.Resources.ДопПользователь, 4, 2 + ПолучитьПоправкуПоВысотеДляШрифта());            
            e.Graphics.DrawString(Отправитель.Text, new Font(FontFamily.GenericSansSerif, СтруктураОформления.РазмерШрифта, СтруктураОформления.НачертаниеШрифта), new SolidBrush(СтруктураОформления.ЦветШрифта), e.ClipRectangle.X + 19, e.ClipRectangle.Y + 3);
        }

        private void ПрорисовкаПодПунктаМенюЗапуска(object sender, PaintEventArgs e)
        {            
            ToolStripItem Отправитель = (ToolStripItem)sender;
            СтруктураНастроекЭлемента НастройкаТекущейБазы = (СтруктураНастроекЭлемента)Отправитель.Tag;
            e.Graphics.DrawImage(ПолучитьИконкуРежима(НастройкаТекущейБазы.ТипПлатформы, НастройкаТекущейБазы.РежимЗапуска), 4, 2 + ПолучитьПоправкуПоВысотеДляШрифта());
            e.Graphics.DrawString(Отправитель.Text, new Font(FontFamily.GenericSansSerif, СтруктураОформления.РазмерШрифта, СтруктураОформления.НачертаниеШрифта), new SolidBrush(СтруктураОформления.ЦветШрифта), e.ClipRectangle.X + 19, e.ClipRectangle.Y + 3);
        }

        private void ПрорисовкаПодПунктаВидаКлиента(object sender, PaintEventArgs e)
        {
            ToolStripItem Отправитель = (ToolStripItem)sender;
            СтруктураНастроекЭлемента НастройкаТекущейБазы = (СтруктураНастроекЭлемента)Отправитель.Tag;
            e.Graphics.DrawImage(ПолучитьИконкуКлиента(НастройкаТекущейБазы.ВидКлиента), 4, 2 + ПолучитьПоправкуПоВысотеДляШрифта());
            e.Graphics.DrawString(Отправитель.Text, new Font(FontFamily.GenericSansSerif, СтруктураОформления.РазмерШрифта, СтруктураОформления.НачертаниеШрифта), new SolidBrush(СтруктураОформления.ЦветШрифта), e.ClipRectangle.X + 19, e.ClipRectangle.Y + 3);
        }

        private void ПрорисовкаЭлементаКонтекстногоМеню(object sender, PaintEventArgs e)
        {
            ToolStripItem Отправитель = (ToolStripItem)sender;
            СтруктураНастроекЭлемента НастройкаТекущейБазы = (СтруктураНастроекЭлемента)Отправитель.Tag;
            if (НастройкаТекущейБазы.РежимЗапуска == 10)
                e.Graphics.DrawImage(Properties.Resources.SmallEdit, 4, 2 + ПолучитьПоправкуПоВысотеДляШрифта());
            else
                e.Graphics.DrawImage(Properties.Resources.SmallRemove, 4, 2 + ПолучитьПоправкуПоВысотеДляШрифта());
            e.Graphics.DrawString(Отправитель.Text, new Font(FontFamily.GenericSansSerif, СтруктураОформления.РазмерШрифта, СтруктураОформления.НачертаниеШрифта), new SolidBrush(СтруктураОформления.ЦветШрифта), e.ClipRectangle.X + 19, e.ClipRectangle.Y + 3);
        }

        private void ПрорисовкаЭлементаКонтекстногоМенюГруппа(object sender, PaintEventArgs e)
        {
            ToolStripItem Отправитель = (ToolStripItem)sender;
            СтруктураНастроекГруппыЭлементов НастройкаТекущейБазы = (СтруктураНастроекГруппыЭлементов)Отправитель.Tag;
            if (НастройкаТекущейБазы.Действие == 10)
                e.Graphics.DrawImage(Properties.Resources.SmallEdit, 4, 2 + ПолучитьПоправкуПоВысотеДляШрифта());
            else
                e.Graphics.DrawImage(Properties.Resources.SmallRemove, 4, 2 + ПолучитьПоправкуПоВысотеДляШрифта());
            e.Graphics.DrawString(Отправитель.Text, new Font(FontFamily.GenericSansSerif, СтруктураОформления.РазмерШрифта, СтруктураОформления.НачертаниеШрифта), new SolidBrush(СтруктураОформления.ЦветШрифта), e.ClipRectangle.X + 19, e.ClipRectangle.Y + 3);
        }

        private void ПрорисовкаЭлементаКонтекстногоМенюМонитор(object sender, PaintEventArgs e)
        {
            ToolStripItem Отправитель = (ToolStripItem)sender;
            e.Graphics.DrawImage(Properties.Resources.МониторПользователей, 4, 2 + ПолучитьПоправкуПоВысотеДляШрифта());
            e.Graphics.DrawString(Отправитель.Text, new Font(FontFamily.GenericSansSerif, СтруктураОформления.РазмерШрифта, СтруктураОформления.НачертаниеШрифта), new SolidBrush(СтруктураОформления.ЦветШрифта), e.ClipRectangle.X + 19, e.ClipRectangle.Y + 3);
        }

        private void ПрорисовкаПодПунктаМенюЗапускаПриложение(object sender, PaintEventArgs e)
        {            
            ToolStripItem Отправитель = (ToolStripItem)sender;
            СтруктураНастроекЭлемента НастройкаТекущейБазы = (СтруктураНастроекЭлемента)Отправитель.Tag;
            e.Graphics.DrawImage(ПолучитьИконкуВнешнегоПриложения(НастройкаТекущейБазы), 4, 2 + ПолучитьПоправкуПоВысотеДляШрифта());
            e.Graphics.DrawString(Отправитель.Text, new Font(FontFamily.GenericSansSerif, СтруктураОформления.РазмерШрифта, СтруктураОформления.НачертаниеШрифта), new SolidBrush(СтруктураОформления.ЦветШрифта), e.ClipRectangle.X + 19, e.ClipRectangle.Y + 3);
        }

        private void ПрорисовкаГруппыМенюЗапуска(object sender, PaintEventArgs e)
        {
            ToolStripItem Отправитель = (ToolStripItem)sender;
            e.Graphics.DrawImage(Properties.Resources.Folder, 4, 2 + ПолучитьПоправкуПоВысотеДляШрифта());
            e.Graphics.DrawString(Отправитель.Text, new Font(FontFamily.GenericSansSerif, СтруктураОформления.РазмерШрифта, СтруктураОформления.НачертаниеШрифта), new SolidBrush(СтруктураОформления.ЦветШрифта), e.ClipRectangle.X + 19, e.ClipRectangle.Y + 3);
        }

        private void ПрорисовкаДобавитьГруппуБазу(object sender, PaintEventArgs e)
        {
            ToolStripItem Отправитель = (ToolStripItem)sender;
            if (Отправитель.Text == "Добавить группу")
                e.Graphics.DrawImage(Properties.Resources.folderSmall, 4, 2 + ПолучитьПоправкуПоВысотеДляШрифта());
            else
                e.Graphics.DrawImage(Properties.Resources.SmallAdd, 4, 2 + ПолучитьПоправкуПоВысотеДляШрифта());
            e.Graphics.DrawString(Отправитель.Text, new Font(FontFamily.GenericSansSerif, СтруктураОформления.РазмерШрифта, СтруктураОформления.НачертаниеШрифта), new SolidBrush(СтруктураОформления.ЦветШрифта), e.ClipRectangle.X + 19, e.ClipRectangle.Y + 3);
        }        

        // Процедура заполняет меню запуска, обходя рекурсивно настройки программы
        //
        private void ЗаполнениеМенюЗапуска(ContextMenuStrip МенюЗапуска, XmlNode УзелXML, ToolStripMenuItem ТекущаяГруппа, bool ДобавлятьПунктыВМеню)
        {            
            Boolean СозданаНоваяГруппа = false;
            ToolStripMenuItem НовыйРодитель = new ToolStripMenuItem();
            if (УзелXML == null)
                return;
            if (УзелXML is XmlElement)
            {
                if (УзелXML.Name == "Группа")
                {
                    if (ГруппаИмеетВложенныеБазы(УзелXML, true) | (ДобавлятьПунктыВМеню))
                    {
                        СтруктураНастроекГруппыЭлементов НастройкиТекущейГруппы = ПолучитьНастройкиГруппы(УзелXML);

                        if (ТекущаяГруппа == null)
                        {
                            НовыйРодитель = (ToolStripMenuItem)МенюЗапуска.Items.Add(ПолучитьАтрибутУзла(УзелXML, "Наименование"));                            
                            НовыйРодитель.Tag = НастройкиТекущейГруппы;
                            НовыйРодитель.MouseDown += new System.Windows.Forms.MouseEventHandler(Группа_MouseClick);
                            if (СобственноеОформление)
                            {
                                НовыйРодитель.BackColor = СтруктураОформления.ЦветФона;
                                НовыйРодитель.ForeColor = Color.Transparent;
                                НовыйРодитель.Paint += new PaintEventHandler(ПрорисовкаГруппыМенюЗапуска);
                            }
                            else
                                НовыйРодитель.Image = TestMenuPopup.Properties.Resources.Folder;
                        }
                        else
                        {
                            НовыйРодитель = (ToolStripMenuItem)ТекущаяГруппа.DropDownItems.Add(ПолучитьАтрибутУзла(УзелXML, "Наименование"));                            
                            НовыйРодитель.Tag = НастройкиТекущейГруппы;
                            НовыйРодитель.MouseDown += new System.Windows.Forms.MouseEventHandler(Группа_MouseClick);
                            if (СобственноеОформление)
                            {
                                НовыйРодитель.BackColor = СтруктураОформления.ЦветФона;
                                НовыйРодитель.ForeColor = Color.Transparent;
                                НовыйРодитель.Paint += new PaintEventHandler(ПрорисовкаГруппыМенюЗапуска);
                            }
                            else
                                НовыйРодитель.Image = TestMenuPopup.Properties.Resources.Folder;
                        }
                        СозданаНоваяГруппа = true;                        
                    }                    
                }
                if (УзелXML.Name == "БазаДанных")
                {
                    ToolStripItem ЭлементМеню;
                    СтруктураНастроекЭлемента НастройкаТекущейБазы = ПолучитьНастройкиБазыДанных(УзелXML);

                    if (НастройкаТекущейБазы.ПоказыватьВМенюЗапуска)
                    {
                        if (!НастройкаТекущейБазы.Приложение)
                        {
                            if (ТекущаяГруппа == null)
                            {
                                ЭлементМеню = МенюЗапуска.Items.Add(НастройкаТекущейБазы.Наименование);
                            }
                            else
                            {
                                ЭлементМеню = ТекущаяГруппа.DropDownItems.Add(НастройкаТекущейБазы.Наименование);
                            }

                            if (!СобственноеОформление)
                            {
                                if (БазаДанныхСуществуетПоУказанномуПути(НастройкаТекущейБазы))
                                    ЭлементМеню.Image = ПолучитьКартинкуПлатформы(НастройкаТекущейБазы.ТипПлатформы);
                                else
                                    ЭлементМеню.Image = Properties.Resources.БазаОтсутствует;
                            }

                            if (!НастройкаТекущейБазы.РежимЗапускаКакПунктМеню)
                            {
                                ЭлементМеню.Tag = НастройкаТекущейБазы;
                                ЭлементМеню.MouseDown += new System.Windows.Forms.MouseEventHandler(ПунктМенюЗапуска_MouseClick);                                                                                                
                                if (СобственноеОформление)
                                {
                                    ЭлементМеню.BackColor = СтруктураОформления.ЦветФона;
                                    ЭлементМеню.ForeColor = Color.Transparent;
                                    ЭлементМеню.Paint += new PaintEventHandler(ПрорисовкаПунктаМенюЗапуска);
                                }
                                else
                                {
                                    if (БазаДанныхСуществуетПоУказанномуПути(НастройкаТекущейБазы))
                                        ЭлементМеню.Image = ПолучитьКартинкуПлатформы(НастройкаТекущейБазы.ТипПлатформы);
                                    else
                                        ЭлементМеню.Image = Properties.Resources.БазаОтсутствует;
                                }

                            }
                            else
                            {
                                // Создаем подменю с режимами запуска для базы                            
                                ЭлементМеню.MouseDown += new System.Windows.Forms.MouseEventHandler(ПодпунктМенюЗапуска_MouseClick);
                                ЭлементМеню.Tag = НастройкаТекущейБазы;                                
                                // Собственная отрисовка меню
                                if (СобственноеОформление)
                                {
                                    ЭлементМеню.Paint += new PaintEventHandler(ПрорисовкаПунктаМенюЗапуска);
                                    ЭлементМеню.BackColor = СтруктураОформления.ЦветФона;
                                    ЭлементМеню.ForeColor = Color.Transparent;
                                }

                                ToolStripItem ЭлементМенюСРежимамиЗапуска;
                                ToolStripMenuItem Подменю = (ToolStripMenuItem)ЭлементМеню;
                                int ТекПлатформа = НастройкаТекущейБазы.ТипПлатформы;
                                if (НастройкаТекущейБазы.ТипПлатформы == 0)
                                {
                                    // 1С Предприятие 7.7
                                    ЭлементМенюСРежимамиЗапуска = Подменю.DropDownItems.Add("1С Предприятие");                                                                                                            
                                    НастройкаТекущейБазы.РежимЗапуска = 1;                                    
                                    ЭлементМенюСРежимамиЗапуска.Tag = НастройкаТекущейБазы;
                                    if (СобственноеОформление)
                                    {
                                        ЭлементМенюСРежимамиЗапуска.BackColor = СтруктураОформления.ЦветФона;
                                        ЭлементМенюСРежимамиЗапуска.ForeColor = Color.Transparent;
                                        ЭлементМенюСРежимамиЗапуска.Paint += new PaintEventHandler(ПрорисовкаПодПунктаМенюЗапуска);
                                    }
                                    else
                                        ЭлементМенюСРежимамиЗапуска.Image = ПолучитьИконкуРежима(0, 1);

                                    // Создание подменю с выбором режима работы
                                    ToolStripMenuItem Подменю1СПредприятия = (ToolStripMenuItem)ЭлементМенюСРежимамиЗапуска;
                                    ToolStripItem ЭлементМенюСРежимамиРаботы;
                                    // Монопольный
                                    ЭлементМенюСРежимамиРаботы = Подменю1СПредприятия.DropDownItems.Add("Монопольный");
                                    НастройкаТекущейБазы.РежимРаботы = 1;
                                    ЭлементМенюСРежимамиРаботы.Tag = НастройкаТекущейБазы;                                    
                                    ЭлементМенюСРежимамиРаботы.Click += new EventHandler(ПунктРежимаРаботы_Click);
                                    if (СобственноеОформление)
                                    {
                                        ЭлементМенюСРежимамиРаботы.BackColor = СтруктураОформления.ЦветФона;
                                        ЭлементМенюСРежимамиРаботы.ForeColor = Color.Transparent;
                                        ЭлементМенюСРежимамиРаботы.Paint += new PaintEventHandler(ПрорисовкаМенюЗапускаРежимРаботы);
                                    }
                                    else
                                        ЭлементМенюСРежимамиРаботы.Image = TestMenuPopup.Properties.Resources.Монопольный;

                                    // Разделенный
                                    ЭлементМенюСРежимамиРаботы = Подменю1СПредприятия.DropDownItems.Add("Разделенный");
                                    НастройкаТекущейБазы.РежимРаботы = 2;
                                    ЭлементМенюСРежимамиРаботы.Tag = НастройкаТекущейБазы;                                    
                                    ЭлементМенюСРежимамиРаботы.Click += new EventHandler(ПунктРежимаРаботы_Click);
                                    if (СобственноеОформление)
                                    {
                                        ЭлементМенюСРежимамиРаботы.BackColor = СтруктураОформления.ЦветФона;
                                        ЭлементМенюСРежимамиРаботы.ForeColor = Color.Transparent;
                                        ЭлементМенюСРежимамиРаботы.Paint += new PaintEventHandler(ПрорисовкаМенюЗапускаРежимРаботы);
                                    }
                                    else
                                        ЭлементМенюСРежимамиРаботы.Image = TestMenuPopup.Properties.Resources.Разделенный;

                                    // Конфигуратор 7.7
                                    ЭлементМенюСРежимамиЗапуска = Подменю.DropDownItems.Add("Кофигуратор");
                                    НастройкаТекущейБазы.РежимЗапуска = 2;
                                    ЭлементМенюСРежимамиЗапуска.Tag = НастройкаТекущейБазы;                                    
                                    ЭлементМенюСРежимамиЗапуска.Click += new EventHandler(ПунктРежимаЗапуска_Click);
                                    if (СобственноеОформление)
                                    {
                                        ЭлементМенюСРежимамиЗапуска.BackColor = СтруктураОформления.ЦветФона;
                                        ЭлементМенюСРежимамиЗапуска.ForeColor = Color.Transparent;
                                        ЭлементМенюСРежимамиЗапуска.Paint += new PaintEventHandler(ПрорисовкаПодПунктаМенюЗапуска);
                                    }
                                    else
                                        ЭлементМенюСРежимамиЗапуска.Image = ПолучитьИконкуРежима(0, 2);
                                    // Отладчик 7.7
                                    ЭлементМенюСРежимамиЗапуска = Подменю.DropDownItems.Add("Отладчик");
                                    НастройкаТекущейБазы.РежимЗапуска = 3;
                                    ЭлементМенюСРежимамиЗапуска.Tag = НастройкаТекущейБазы;                                    
                                    ЭлементМенюСРежимамиЗапуска.Click += new EventHandler(ПунктРежимаЗапуска_Click);
                                    if (СобственноеОформление)
                                    {
                                        ЭлементМенюСРежимамиЗапуска.BackColor = СтруктураОформления.ЦветФона;
                                        ЭлементМенюСРежимамиЗапуска.ForeColor = Color.Transparent;
                                        ЭлементМенюСРежимамиЗапуска.Paint += new PaintEventHandler(ПрорисовкаПодПунктаМенюЗапуска);
                                    }
                                    else
                                        ЭлементМенюСРежимамиЗапуска.Image = ПолучитьИконкуРежима(0, 3);
                                    // Монитор 7.7
                                    ЭлементМенюСРежимамиЗапуска = Подменю.DropDownItems.Add("Монитор");
                                    НастройкаТекущейБазы.РежимЗапуска = 4;
                                    ЭлементМенюСРежимамиЗапуска.Tag = НастройкаТекущейБазы;                                    
                                    ЭлементМенюСРежимамиЗапуска.Click += new EventHandler(ПунктРежимаЗапуска_Click);
                                    if (СобственноеОформление)
                                    {
                                        ЭлементМенюСРежимамиЗапуска.BackColor = СтруктураОформления.ЦветФона;
                                        ЭлементМенюСРежимамиЗапуска.ForeColor = Color.Transparent;
                                        ЭлементМенюСРежимамиЗапуска.Paint += new PaintEventHandler(ПрорисовкаПодПунктаМенюЗапуска);
                                    }
                                    else
                                        ЭлементМенюСРежимамиЗапуска.Image = ПолучитьИконкуРежима(0, 4);
                                }                                
                                else
                                {
                                    // 1С Предприятие 8.х
                                    ЭлементМенюСРежимамиЗапуска = Подменю.DropDownItems.Add("1С Предприятие");
                                    НастройкаТекущейБазы.РежимЗапуска = 1;
                                    ЭлементМенюСРежимамиЗапуска.Tag = НастройкаТекущейБазы;                                                                        
                                    if (СобственноеОформление)
                                    {
                                        ЭлементМенюСРежимамиЗапуска.BackColor = СтруктураОформления.ЦветФона;
                                        ЭлементМенюСРежимамиЗапуска.ForeColor = Color.Transparent;
                                        ЭлементМенюСРежимамиЗапуска.Paint += new PaintEventHandler(ПрорисовкаПодПунктаМенюЗапуска);
                                    }
                                    else
                                        ЭлементМенюСРежимамиЗапуска.Image = ПолучитьИконкуРежима(ТекПлатформа, 1);

                                    // Если это 8.2 возможно надо добавить подменю клиента
                                    if ((НастройкаТекущейБазы.ТипПлатформы > 2) && (НастройкаТекущейБазы.ВидКлиентаКакПунктМеню))
                                    {
                                        ToolStripMenuItem Подменю1СПредприятия = (ToolStripMenuItem)ЭлементМенюСРежимамиЗапуска;
                                        ToolStripItem ЭлементСВидомКлиента;
                                        // Автоматический режим
                                        ЭлементСВидомКлиента = Подменю1СПредприятия.DropDownItems.Add("Автоматически");
                                        НастройкаТекущейБазы.ВидКлиента = 0;
                                        ЭлементСВидомКлиента.Tag = НастройкаТекущейБазы;
                                        ЭлементСВидомКлиента.Click += new EventHandler(ПунктВидаКлиента_Click);
                                        if (СобственноеОформление)
                                        {
                                            ЭлементСВидомКлиента.BackColor = СтруктураОформления.ЦветФона;
                                            ЭлементСВидомКлиента.ForeColor = Color.Transparent;
                                            ЭлементСВидомКлиента.Paint += new PaintEventHandler(ПрорисовкаПодПунктаВидаКлиента);
                                        }
                                        // Тонкий клиент
                                        ЭлементСВидомКлиента = Подменю1СПредприятия.DropDownItems.Add("Тонкий клиент");
                                        НастройкаТекущейБазы.ВидКлиента = 2;
                                        ЭлементСВидомКлиента.Tag = НастройкаТекущейБазы;
                                        ЭлементСВидомКлиента.Click += new EventHandler(ПунктВидаКлиента_Click);
                                        if (СобственноеОформление)
                                        {
                                            ЭлементСВидомКлиента.BackColor = СтруктураОформления.ЦветФона;
                                            ЭлементСВидомКлиента.ForeColor = Color.Transparent;
                                            ЭлементСВидомКлиента.Paint += new PaintEventHandler(ПрорисовкаПодПунктаВидаКлиента);
                                        }
                                        // Толстый клиент
                                        ЭлементСВидомКлиента = Подменю1СПредприятия.DropDownItems.Add("Толстый клиент");
                                        НастройкаТекущейБазы.ВидКлиента = 3;
                                        ЭлементСВидомКлиента.Tag = НастройкаТекущейБазы;
                                        ЭлементСВидомКлиента.Click += new EventHandler(ПунктВидаКлиента_Click);
                                        if (СобственноеОформление)
                                        {
                                            ЭлементСВидомКлиента.BackColor = СтруктураОформления.ЦветФона;
                                            ЭлементСВидомКлиента.ForeColor = Color.Transparent;
                                            ЭлементСВидомКлиента.Paint += new PaintEventHandler(ПрорисовкаПодПунктаВидаКлиента);
                                        }
                                    }
                                    else
                                        ЭлементМенюСРежимамиЗапуска.Click += new EventHandler(ПунктРежимаЗапуска_Click);

                                    // Кофигуратор 8.х
                                    ЭлементМенюСРежимамиЗапуска = Подменю.DropDownItems.Add("Конфигуратор");
                                    НастройкаТекущейБазы.РежимЗапуска = 2;
                                    ЭлементМенюСРежимамиЗапуска.Tag = НастройкаТекущейБазы;                                    
                                    ЭлементМенюСРежимамиЗапуска.Click += new EventHandler(ПунктРежимаЗапуска_Click);
                                    if (СобственноеОформление)
                                    {
                                        ЭлементМенюСРежимамиЗапуска.BackColor = СтруктураОформления.ЦветФона;
                                        ЭлементМенюСРежимамиЗапуска.ForeColor = Color.Transparent;
                                        ЭлементМенюСРежимамиЗапуска.Paint += new PaintEventHandler(ПрорисовкаПодПунктаМенюЗапуска);
                                    }
                                    else
                                        ЭлементМенюСРежимамиЗапуска.Image = ПолучитьИконкуРежима(ТекПлатформа, 2);

                                    // Монитор 8.x
                                    ЭлементМенюСРежимамиЗапуска = Подменю.DropDownItems.Add("Монитор пользователей");
                                    НастройкаТекущейБазы.РежимЗапуска = 4;
                                    ЭлементМенюСРежимамиЗапуска.Tag = НастройкаТекущейБазы;
                                    ЭлементМенюСРежимамиЗапуска.Click += new EventHandler(МониторПользователей_Click);
                                    if (СобственноеОформление)
                                    {
                                        ЭлементМенюСРежимамиЗапуска.BackColor = СтруктураОформления.ЦветФона;
                                        ЭлементМенюСРежимамиЗапуска.ForeColor = Color.Transparent;
                                        ЭлементМенюСРежимамиЗапуска.Paint += new PaintEventHandler(ПрорисовкаПодПунктаМенюЗапуска);
                                    }
                                    else
                                        ЭлементМенюСРежимамиЗапуска.Image = ПолучитьИконкуРежима(ТекПлатформа, 4);
                                }
                            }
                        }
                        else
                        {
                            if (ТекущаяГруппа == null)                           
                                ЭлементМеню = МенюЗапуска.Items.Add(НастройкаТекущейБазы.Наименование);                            
                            else                            
                                ЭлементМеню = ТекущаяГруппа.DropDownItems.Add(НастройкаТекущейБазы.Наименование);
                            
                            ЭлементМеню.Tag = НастройкаТекущейБазы;
                            ЭлементМеню.MouseDown += new System.Windows.Forms.MouseEventHandler(ПунктМенюЗапуска_MouseClick);
                            if (СобственноеОформление)
                            {
                                ЭлементМеню.BackColor = СтруктураОформления.ЦветФона;
                                ЭлементМеню.ForeColor = Color.Transparent;
                                ЭлементМеню.Paint += new PaintEventHandler(ПрорисовкаПодПунктаМенюЗапускаПриложение);
                            }
                            else
                                ЭлементМеню.Image = ПолучитьИконкуВнешнегоПриложения(НастройкаТекущейБазы);

                            GC.Collect();
                            GC.WaitForPendingFinalizers();
                        }
                    }
                }
            }
            if (УзелXML.HasChildNodes)
            {
                if (!СозданаНоваяГруппа)
                {
                    ЗаполнениеМенюЗапуска(МенюЗапуска, УзелXML.FirstChild, ТекущаяГруппа, ДобавлятьПунктыВМеню);                    
                }
                else
                {
                    ЗаполнениеМенюЗапуска(МенюЗапуска, УзелXML.FirstChild, НовыйРодитель, ДобавлятьПунктыВМеню);
                    
                    // Сортировка элементов
                    СортировкаЭлементовМеню(НовыйРодитель.DropDownItems);            

                    if (ДобавлятьПунктыВМеню)
                    {
                        ToolStripItem ДополнительныйЭлементМеню;                        
                        if (СобственноеОформление)
                        {
                            // Прорисовываем собственный разделитель                            
                            ДополнительныйЭлементМеню = НовыйРодитель.DropDownItems.Add("");
                            ДополнительныйЭлементМеню.BackColor = СтруктураОформления.ЦветСепаратора;                            
                            ДополнительныйЭлементМеню.Size = new Size(400, 1);
                            ДополнительныйЭлементМеню.AutoSize = false;
                        }
                        else
                            ДополнительныйЭлементМеню = НовыйРодитель.DropDownItems.Add("-");
                        ДополнительныйЭлементМеню = НовыйРодитель.DropDownItems.Add("Добавить группу");
                        ДополнительныйЭлементМеню.Image = Properties.Resources.folderSmall;
                        ДополнительныйЭлементМеню.Tag = УзелXML; // Ссылка на родителя для новой базы/группы
                        ДополнительныйЭлементМеню.Click += new EventHandler(ДобавитьГруппу_Click);
                        if (СобственноеОформление)
                        {
                            ДополнительныйЭлементМеню.BackColor = СтруктураОформления.ЦветФона;
                            ДополнительныйЭлементМеню.ForeColor = Color.Transparent;
                            ДополнительныйЭлементМеню.Paint += new PaintEventHandler(ПрорисовкаДобавитьГруппуБазу);
                        }
                        ДополнительныйЭлементМеню = НовыйРодитель.DropDownItems.Add("Добавить базу");
                        ДополнительныйЭлементМеню.Tag = УзелXML;
                        ДополнительныйЭлементМеню.Image = Properties.Resources.SmallAdd;
                        ДополнительныйЭлементМеню.Click += new EventHandler(ДобавитьБазу_Click);
                        if (СобственноеОформление)
                        {
                            ДополнительныйЭлементМеню.BackColor = СтруктураОформления.ЦветФона;
                            ДополнительныйЭлементМеню.ForeColor = Color.Transparent;
                            ДополнительныйЭлементМеню.Paint += new PaintEventHandler(ПрорисовкаДобавитьГруппуБазу);
                        }
                    }
                }
            }
            else if (УзелXML.Name == "Группа")
            {
                if (ДобавлятьПунктыВМеню)
                {
                    ToolStripItem ДополнительныйЭлементМеню;                    
                    ДополнительныйЭлементМеню = НовыйРодитель.DropDownItems.Add("Добавить группу");
                    ДополнительныйЭлементМеню.Image = Properties.Resources.folderSmall;
                    ДополнительныйЭлементМеню.Tag = УзелXML; // Ссылка на родителя для новой базы/группы
                    ДополнительныйЭлементМеню.Click += new EventHandler(ДобавитьГруппу_Click);
                    if (СобственноеОформление)
                    {
                        ДополнительныйЭлементМеню.BackColor = СтруктураОформления.ЦветФона;
                        ДополнительныйЭлементМеню.ForeColor = Color.Transparent;
                        ДополнительныйЭлементМеню.Paint += new PaintEventHandler(ПрорисовкаДобавитьГруппуБазу);
                    }
                    ДополнительныйЭлементМеню = НовыйРодитель.DropDownItems.Add("Добавить базу");
                    ДополнительныйЭлементМеню.Tag = УзелXML;
                    ДополнительныйЭлементМеню.Image = Properties.Resources.SmallAdd;
                    ДополнительныйЭлементМеню.Click += new EventHandler(ДобавитьБазу_Click);
                    if (СобственноеОформление)
                    {
                        ДополнительныйЭлементМеню.BackColor = СтруктураОформления.ЦветФона;
                        ДополнительныйЭлементМеню.ForeColor = Color.Transparent;
                        ДополнительныйЭлементМеню.Paint += new PaintEventHandler(ПрорисовкаДобавитьГруппуБазу);
                    }
                }
            }
            if (УзелXML.NextSibling != null)
            {
                ЗаполнениеМенюЗапуска(МенюЗапуска, УзелXML.NextSibling, ТекущаяГруппа, ДобавлятьПунктыВМеню);
            }
        }

        
        // Процедура обрабатывает клие по пункту меню "Добавить группу"
        //
        private void ДобавитьГруппу_Click(object sender, EventArgs e)
        {
            XmlNode РодительГруппы = (XmlNode)((ToolStripItem)sender).Tag;                        

            ФормаГруппы ФормаГруппы = new ФормаГруппы();
            ФормаГруппы.СоздатьГруппу();
            ФормаГруппы.ShowDialog();
            ОтменитьРегистрациюВсехГорячихКлавиш(УзелБазДанных, 0);
            if (ФормаГруппы.DialogResult == DialogResult.OK)
            {
                СтруктураНастроекГруппыЭлементов НастройкаГруппы = ФормаГруппы.ТекущаяБаза;                
                                                
                XmlElement УзелГруппы = ДобавитьГруппуБазВНастройки(НастройкаГруппы, РодительГруппы);

                КорректировкаГорячихКлавиш(УзелГруппы, УзелБазДанных);

                СохранитьНастройкиПрограммы();
                ИнициализацияМенюЗапуска(МенюЗапуска);                
            }
            ФормаГруппы.Dispose();
            ЗарегистрироватьВсеГорячиеКлавиши(УзелБазДанных, 0);
        }


        // Процедура обрабатывает клие по пункту меню "Добавить базу"
        //
        private void ДобавитьБазу_Click(object sender, EventArgs e)
        {
            ToolStripItem ЭлементМеню = (ToolStripItem)sender;
            XmlNode РодительБазы = (XmlNode)ЭлементМеню.Tag;

            СтруктураНастроекЭлемента НоваяБазаДанных;
            ФормаБазыДанных ФормаНастроек = new ФормаБазыДанных();
            ФормаНастроек.СоздатьБазуДанных(РодительБазы);
            ОтменитьРегистрациюВсехГорячихКлавиш(УзелБазДанных, 0);
            if (ФормаНастроек.ShowDialog() == DialogResult.OK)
            {
                РодительБазы = (XmlElement)ФормаНастроек.РодительБазыДанных;

                // Добавляем базу в глобальный настройки
                НоваяБазаДанных = ФормаНастроек.ТекущаяНастройка;
                XmlElement УзелБазыДанных = ДобавитьБазуДанныхВНастройки(НоваяБазаДанных, РодительБазы);
                КорректировкаГорячихКлавиш(УзелБазыДанных, УзелБазДанных);
                СохранитьНастройкиПрограммы();
                ИнициализацияМенюЗапуска(МенюЗапуска);
                РегистрацияБазы77(НоваяБазаДанных);
            }
            ФормаНастроек.Dispose();
            ЗарегистрироватьВсеГорячиеКлавиши(УзелБазДанных, 0);
        }

        // Процедура сортирует пункты меню запуска в следующем порядке:
        // вверху находятся группы, ниже базы
        static void СортировкаЭлементовМеню(ToolStripItemCollection ПунктыМеню)
        {
            int i = 0;
            ListBox СписокДляСортировки = new ListBox();
            for (i = 0; i < ПунктыМеню.Count; i++)
            {
                СписокДляСортировки.Items.Add(ПунктыМеню[i]);
            }

            ПунктыМеню.Clear();

            i = 0;
            while (i < СписокДляСортировки.Items.Count)
            {
                ToolStripItem ТекЭлементМеню = (ToolStripItem)СписокДляСортировки.Items[i];
                if (ТекЭлементМеню.Tag.GetType().Name == "СтруктураНастроекГруппыЭлементов")
                {
                    ПунктыМеню.Add(ТекЭлементМеню);
                    СписокДляСортировки.Items.Remove(СписокДляСортировки.Items[i]);
                }
                else
                    i++;
            }

            i = 0;
            while (i < СписокДляСортировки.Items.Count)
            {
                ToolStripItem ТекЭлементМеню = (ToolStripItem)СписокДляСортировки.Items[i];
                ПунктыМеню.Add(ТекЭлементМеню);
                i++;
            }
            СписокДляСортировки.Dispose();
        }

        private СтруктураСобственныхНастроекМеню ПолучитьСтруктуруОформленияМеню()
        {
            СтруктураОформления = new СтруктураСобственныхНастроекМеню();
            СтруктураОформления.ЦветФона = ПолучитьЦветИзСтроки(ПолучитьЗначениеНастройки("ЦветФонаМеню", ""), Color.White);
            СтруктураОформления.ЦветШрифта = ПолучитьЦветИзСтроки(ПолучитьЗначениеНастройки("ЦветТекстаМеню", ""), Color.Black);
            СтруктураОформления.ЦветСепаратора = ПолучитьЦветИзСтроки(ПолучитьЗначениеНастройки("ЦветСепаратора", ""), Color.Black);
            СтруктураОформления.РазмерШрифта = Convert.ToInt16(ПолучитьЗначениеНастройки("РазмерШрифтаМеню", "8"));            

            FontStyle ТекНачертание;
            string НачертаниеШрифта = ПолучитьЗначениеНастройки("НачертаниеШрифтаМеню", "обычный");
            switch (НачертаниеШрифта)
            {
                case "обычный":
                    ТекНачертание = FontStyle.Regular;
                    break;
                case "жирный":
                    ТекНачертание = FontStyle.Bold;
                    break;
                case "курсив":
                    ТекНачертание = FontStyle.Italic;
                    break;
                default:
                    ТекНачертание = FontStyle.Bold | FontStyle.Italic;
                    break;
            }

            СтруктураОформления.НачертаниеШрифта = ТекНачертание;

            return СтруктураОформления;
        }


        // Процедура запускает заполнение меню запуска
        //
        private void ИнициализацияМенюЗапуска(ContextMenuStrip МенюЗапуска)
        {
            Debug.WriteLine("Start build: " + DateTime.Now);

            МенюЗапуска.Items.Clear();
            bool ДобавлятьПунктыВМеню = Convert.ToBoolean(ПолучитьЗначениеНастройки("ДобавлятьПунктыНепосредственноВМенюЗапуска", "true"));
            СобственноеОформление = Convert.ToBoolean(ПолучитьЗначениеНастройки("НастраиваемоеМенюЗапуска", "false"));            
            
            if (СобственноеОформление)
            {

                СтруктураОформления = ПолучитьСтруктуруОформленияМеню();

                МенюЗапуска.ShowImageMargin = false;
                МенюЗапуска.BackColor = СтруктураОформления.ЦветФона;
                МенюЗапуска.Font = new Font(FontFamily.GenericSansSerif, СтруктураОформления.РазмерШрифта, СтруктураОформления.НачертаниеШрифта);
                МенюЗапуска.ForeColor = СтруктураОформления.ЦветШрифта;
            }
            else
                МенюЗапуска.ShowImageMargin = true;


            ЗаполнениеМенюЗапуска(МенюЗапуска, УзелБазДанных, null, ДобавлятьПунктыВМеню);

            // Сортировка элементов
            СортировкаЭлементовМеню(МенюЗапуска.Items);            

            if (ДобавлятьПунктыВМеню)
            {
                ToolStripItem ДополнительныйЭлементМеню;
                if (МенюЗапуска.Items.Count > 0)
                {
                    if (СобственноеОформление)
                    {
                        // Прортсовываем собственный разделитель
                        ToolStripItem Разделитель = МенюЗапуска.Items.Add("");
                        Разделитель.AutoSize = false;
                        Разделитель.BackColor = СтруктураОформления.ЦветСепаратора;
                        Разделитель.Size = new Size(400, 1);                        
                    }
                    else
                        ДополнительныйЭлементМеню = МенюЗапуска.Items.Add("-");                    
                }
                ДополнительныйЭлементМеню = МенюЗапуска.Items.Add("Добавить группу");
                ДополнительныйЭлементМеню.Image = Properties.Resources.folderSmall;
                ДополнительныйЭлементМеню.Tag = УзелБазДанных;
                ДополнительныйЭлементМеню.Click += new EventHandler(ДобавитьГруппу_Click);
                if (СобственноеОформление)
                {
                    ДополнительныйЭлементМеню.BackColor = СтруктураОформления.ЦветФона;
                    ДополнительныйЭлементМеню.ForeColor = Color.Transparent;
                    ДополнительныйЭлементМеню.Paint += new PaintEventHandler(ПрорисовкаДобавитьГруппуБазу);
                }
                ДополнительныйЭлементМеню = МенюЗапуска.Items.Add("Добавить базу");
                ДополнительныйЭлементМеню.Image = Properties.Resources.SmallAdd;
                ДополнительныйЭлементМеню.Tag = УзелБазДанных;
                ДополнительныйЭлементМеню.Click += new EventHandler(ДобавитьБазу_Click);
                if (СобственноеОформление)
                {
                    ДополнительныйЭлементМеню.BackColor = СтруктураОформления.ЦветФона;
                    ДополнительныйЭлементМеню.ForeColor = Color.Transparent;
                    ДополнительныйЭлементМеню.Paint += new PaintEventHandler(ПрорисовкаДобавитьГруппуБазу);
                }
            }

            Debug.WriteLine("End build: " + DateTime.Now);
        }


        static void СоздатьНастройкиПоУмолчанию()
        {
        	if (installer1C == null)
        		installer1C = new installer1CInfo();
        	
            XmlWriter НовыеНастройки = XmlWriter.Create(КаталогПрограммы + "\\Settings.xml");
            НовыеНастройки.WriteStartDocument();
            НовыеНастройки.WriteStartElement("НастройкиПрограммы");

            НовыеНастройки.WriteStartElement("БазыДанных");
            НовыеНастройки.WriteStartAttribute("ГруппаРаскрыта");
            НовыеНастройки.WriteValue(true);
            НовыеНастройки.WriteEndAttribute();
            НовыеНастройки.WriteStartAttribute("ГруппаАктивна");
            НовыеНастройки.WriteValue(true);
            НовыеНастройки.WriteEndAttribute();
            НовыеНастройки.WriteEndElement();

            НовыеНастройки.WriteStartElement("Исключения");
            НовыеНастройки.WriteEndElement();

            НовыеНастройки.WriteEndElement();
            НовыеНастройки.Close();
            НовыеНастройки = null;
            
            НастройкиHotTray.Load(КаталогПрограммы + "\\Settings.xml");

            XmlElement КорневойЭлемент = НастройкиHotTray.DocumentElement;

            УстановитьАтрибутУзла(КорневойЭлемент, "ИмяПользователяПоУмолчанию", "");
            УстановитьАтрибутУзла(КорневойЭлемент, "ПарольПользователяПоУмолчанию", "");
            УстановитьАтрибутУзла(КорневойЭлемент, "ДобавлятьПунктыНепосредственноВМенюЗапуска", "true");
            УстановитьАтрибутУзла(КорневойЭлемент, "ЗапускатьВместеСWindows", "false");            
            УстановитьАтрибутУзла(КорневойЭлемент, "СохранятьПоложениеОкна", "false");
            УстановитьАтрибутУзла(КорневойЭлемент, "АктивироватьСреднююКнопку", "true");
            УстановитьАтрибутУзла(КорневойЭлемент, "ПроверятьНаличиеБазыПриДобавлении", "false");            
            УстановитьАтрибутУзла(КорневойЭлемент, "ДобавлятьПунктыНепосредственноВМенюЗапуска", "true");
            УстановитьАтрибутУзла(КорневойЭлемент, "ТипПлатформыПоУмолчанию", "4");
            УстановитьАтрибутУзла(КорневойЭлемент, "ПутьЗапуска77", installer1C.GetStarter(77));
            УстановитьАтрибутУзла(КорневойЭлемент, "ПутьЗапуска80", installer1C.GetStarter(80));
            УстановитьАтрибутУзла(КорневойЭлемент, "ПутьЗапуска81", installer1C.GetStarter(81));
            УстановитьАтрибутУзла(КорневойЭлемент, "ПутьЗапуска82", installer1C.GetStarter(82));
            УстановитьАтрибутУзла(КорневойЭлемент, "ПутьЗапуска83", installer1C.GetStarter(83));
            

            СохранитьНастройкиПрограммы();
            
            НастройкиHotTray.Load(КаталогПрограммы + "\\Settings.xml");            
        }

        static private void ПолучитьИсключения(XmlNode Узел, ref ListBox СписокФайлов)
        {
            if (Узел == null)
                return;
            if (Узел.Name == "Исключение")
            {
                string ИмяИсполняемогоФайла = ПолучитьАтрибутУзла(Узел, "Путь");
                СписокФайлов.Items.Add(ИмяИсполняемогоФайла);
            }
            if (Узел.HasChildNodes)
                ПолучитьИсключения(Узел.FirstChild, ref СписокФайлов);
            if (Узел.NextSibling != null)
                ПолучитьИсключения(Узел.NextSibling, ref СписокФайлов);
        }
    

        static private void ЗаполнениеСпискаИсключений(XmlNode Узел, ListView СписокИсключений)
        {
            if (Узел == null)
                return;
            if (Узел.Name == "Исключение")
            {
                string ИмяИсполняемогоФайла = ПолучитьАтрибутУзла(Узел, "Путь");
                string[] МассивПути = ИмяИсполняемогоФайла.Split('\\');

                ListViewItem НовоеИсключение = СписокИсключений.Items.Add(Convert.ToString(СписокИсключений.Items.Count + 1));
                НовоеИсключение.SubItems.Add(МассивПути[МассивПути.Length - 1]);
                НовоеИсключение.Tag = Узел;
            }
            if (Узел.HasChildNodes)
                ЗаполнениеСпискаИсключений(Узел.FirstChild, СписокИсключений);
            if (Узел.NextSibling != null)
                ЗаполнениеСпискаИсключений(Узел.NextSibling, СписокИсключений);
        }

        static public void ДеревоГруппПослеЗаполнения(TreeNode УзелДерева)
        {
            XmlNode УзелXML = (XmlNode)УзелДерева.Tag;
            УправлениеПоложениемУзлаДереваГрупп(УзелXML, УзелДерева);
            if (УзелДерева.Nodes.Count != 0)
                ДеревоГруппПослеЗаполнения(УзелДерева.Nodes[0]);
            if (УзелДерева.NextNode != null)
                ДеревоГруппПослеЗаполнения(УзелДерева.NextNode);
        }


        // Процедура загружает настройки программы
        // При отсутствии настроек (при первом запуске) они создаются
        // После загрузки в память настройки загружаются еще и в форму (дерево групп и список баз)
        static public void ЗагрузитьНастройкиПрограммы(TreeView ДеревоГрупп, ListView СписокИсключенний)
        {
            // Проверим наличие файла настроек
            if (File.Exists(КаталогПрограммы + "\\Settings.xml"))
            {                
                // Файл настроек наден, загрузим из него данные                
                try
                {
                    НастройкиHotTray.Load(КаталогПрограммы + "\\Settings.xml");
                }
                catch
                {
                    ПоказатьИнфомационноеСообщение("Не удалось загрузить файл с настройками программы (возможно он поврежден)."
                                                    + "\n                    Настройки будут заполнены по умолчанию.", null);
                    DateTime Сегодня = DateTime.Now;
                    string ДатаФайла = Convert.ToString(Сегодня.Year) + Convert.ToString(Сегодня.Month) + Convert.ToString(Сегодня.Day) + Convert.ToString(Сегодня.Hour) + Convert.ToString(Сегодня.Minute) + Convert.ToString(Сегодня.Second);
                    File.Move(КаталогПрограммы + "\\Settings.xml", КаталогПрограммы + "\\Settings.xml" + ДатаФайла + ".xml");
                    СоздатьНастройкиПоУмолчанию();                    
                }
                // Теперь данные необходимо загрузить в форму
                УзелБазДанных = ПолучитьУзелБазДанных();
                УзелИсключений = ПолучитьУзелИсключений();
                ДеревоГрупп.Nodes.Clear();
                ЗаполнениеДереваГрупп(ДеревоГрупп, УзелБазДанных, null, null);
                СписокИсключенний.Items.Clear();
                ЗаполнениеСпискаИсключений(УзелИсключений, СписокИсключенний);
                ДеревоГруппПослеЗаполнения(ДеревоГрупп.Nodes[0]);
            }
            else
            {
                // Файл настроек не был наден, создадим его
                СоздатьНастройкиПоУмолчанию();
                УзелБазДанных = ПолучитьУзелБазДанных();
                УзелИсключений = ПолучитьУзелИсключений();
                ЗаполнениеДереваГрупп(ДеревоГрупп, УзелБазДанных, null, null);
                ЗаполнениеСпискаИсключений(УзелИсключений, СписокИсключенний);
            }
        }

        static public String Шифрование(string СтрокаДляШифрованияДешифрования)
        {

            string Результат = String.Empty;

            if (!String.IsNullOrEmpty(СтрокаДляШифрованияДешифрования))
            {

                int Ключ = 2;
                char[] МассивСимволов = СтрокаДляШифрованияДешифрования.ToCharArray();

                for (int i = 0; i < МассивСимволов.Length; i++)
                    МассивСимволов[i] = ((char)(МассивСимволов[i] ^ Ключ));

                for (int i = 0; i < МассивСимволов.Length; i++)
                    Результат = Результат.Insert(i, char.ToString(МассивСимволов[i]));
            }
            
            return Результат;              
        }

        // Процедура осуществляет поиск XML-элемента в настройках по его наименованию
        //
        static XmlNode ПоискЭлементаXMLПоНаименованию(XmlNode УзелXML, string НаименованиеЭлемента)
        {
            if (УзелXML is XmlElement)
            {
                if (УзелXML.Name == НаименованиеЭлемента)
                {
                    return УзелXML;
                }
                if (УзелXML.NextSibling != null)
                {
                    return ПоискЭлементаXMLПоНаименованию(УзелXML.NextSibling, НаименованиеЭлемента);
                }
                if (УзелXML.HasChildNodes)
                {
                    return ПоискЭлементаXMLПоНаименованию(УзелXML.FirstChild, НаименованиеЭлемента);
                }                

            }

            return null;
        }

        // Процедура возвращает узел содержащий базы данных, если такого узла нет, то содает его
        //
        public static XmlNode ПолучитьУзелБазДанных()
        {
            // Попробуем найти узел баз по наименованию
            XmlNode УзелБДанных = ПоискЭлементаXMLПоНаименованию(НастройкиHotTray.DocumentElement, "БазыДанных");
            if (УзелБДанных == null)
            {
                // Узел баз данных пока не создан, сделаем это
                УзелБДанных = НастройкиHotTray.CreateElement("БазыДанных");
                УзелБДанных = НастройкиHotTray.DocumentElement.AppendChild(УзелБДанных);
            }
            return УзелБДанных;
        }

        // Процедура возвращает узел содержащий исключения
        //
        public static XmlNode ПолучитьУзелИсключений()
        {
            // Попробуем найти узел баз по наименованию
            XmlNode УзелИскл = ПоискЭлементаXMLПоНаименованию(НастройкиHotTray.DocumentElement, "Исключения");
            if (УзелИскл == null)
            {
                // Узел баз данных пока не создан, сделаем это
                УзелИскл = НастройкиHotTray.CreateElement("Исключения");
                УзелИскл = НастройкиHotTray.DocumentElement.AppendChild(УзелБазДанных);
            }
            return УзелИскл;
        }        

        // Процедура устанавливает все атрибуты для базы данных
        // исходя из ее настроек
        static void УстановитьАтрибутыБазыДанных(ref XmlElement БазаДанных, СтруктураНастроекЭлемента НастройкиБазаДанных)
        {
            БазаДанных.SetAttribute("Наименование", НастройкиБазаДанных.Наименование);
            БазаДанных.SetAttribute("ИмяПользователя", НастройкиБазаДанных.ИмяПользователя);
            БазаДанных.SetAttribute("ПарольПользователя", Шифрование(НастройкиБазаДанных.Пароль));
            БазаДанных.SetAttribute("Путь", НастройкиБазаДанных.Путь);
            БазаДанных.SetAttribute("ТипПлатформы", Convert.ToString(НастройкиБазаДанных.ТипПлатформы));
            БазаДанных.SetAttribute("ТипБазы", Convert.ToString(НастройкиБазаДанных.ТипБазы));
            БазаДанных.SetAttribute("ИспользуетсяАутентификацияWindows", Convert.ToString(НастройкиБазаДанных.ИспользуетсяАутентификацияWindows));
            БазаДанных.SetAttribute("ПоказыватьВМенюЗапуска", Convert.ToString(НастройкиБазаДанных.ПоказыватьВМенюЗапуска));
            БазаДанных.SetAttribute("РежимРаботы", Convert.ToString(НастройкиБазаДанных.РежимРаботы));
            БазаДанных.SetAttribute("РежимЗапуска", Convert.ToString(НастройкиБазаДанных.РежимЗапуска));
            БазаДанных.SetAttribute("РежимЗапускаКакПунктМеню", Convert.ToString(НастройкиБазаДанных.РежимЗапускаКакПунктМеню));
            БазаДанных.SetAttribute("ВидКлиента", Convert.ToString(НастройкиБазаДанных.ВидКлиента));
            БазаДанных.SetAttribute("ВидКлиентаКакПунктМеню", Convert.ToString(НастройкиБазаДанных.ВидКлиентаКакПунктМеню));
            БазаДанных.SetAttribute("Описание", НастройкиБазаДанных.Описание);
            БазаДанных.SetAttribute("ПутьКХранилищу", НастройкиБазаДанных.ПутьКХранилищу);
            БазаДанных.SetAttribute("ИмяПользователяХранилища", НастройкиБазаДанных.ИмяПользователяХранилища);
            БазаДанных.SetAttribute("ПарольПользователяХранилища", Шифрование(НастройкиБазаДанных.ПарольПользователяХранилища));
            БазаДанных.SetAttribute("ПрограммаЗапуска", НастройкиБазаДанных.ПрограммаЗапуска);
            БазаДанных.SetAttribute("СочетаниеКлавиш", НастройкиБазаДанных.СочетаниеКлавиш);
            БазаДанных.SetAttribute("Приложение", Convert.ToString(НастройкиБазаДанных.Приложение));
            БазаДанных.SetAttribute("КодДоступа", Шифрование(НастройкиБазаДанных.КодДоступа));      
			БазаДанных.SetAttribute("ВерсияПлатформы", Convert.ToString(НастройкиБазаДанных.ВерсияПлатформы));            
        }

        static void УстановитьАтрибутыГруппы(ref XmlElement Группа, СтруктураНастроекГруппыЭлементов НастройкиГруппы)
        {
            Группа.SetAttribute("Наименование", НастройкиГруппы.Наименование);
            Группа.SetAttribute("Описание", НастройкиГруппы.Описание);
            Группа.SetAttribute("СочетаниеКлавиш", НастройкиГруппы.СочетаниеКлавиш);
        }

        // Процедура добавляет новую базу данных в настройки программы
        //
        static public XmlElement ДобавитьБазуДанныхВНастройки(СтруктураНастроекЭлемента НоваяБазаДанных, XmlNode РодительБазы)
        {
            // Если родитель базы не задан, используем непосредственно узел баз данных (корень)
            if (РодительБазы == null)
            {
                РодительБазы = УзелБазДанных;
            }

            XmlElement БазаДанных = НастройкиHotTray.CreateElement("БазаДанных");
            УстановитьАтрибутыБазыДанных(ref БазаДанных, НоваяБазаДанных);

            // Добавляем дополнительных пользователей
            if (!НоваяБазаДанных.Приложение)
            {
                ДобавитьДополнительныхПользователейВБазу(НоваяБазаДанных, БазаДанных);                
            }

            РодительБазы.AppendChild(БазаДанных);

            return БазаДанных;
        }

        static void ДобавитьДополнительныхПользователейВБазу(СтруктураНастроекЭлемента НастройкиБазыДанных, XmlNode БазаДанных)
        {
            XmlElement ДопПользователи = НастройкиHotTray.CreateElement("ДопПользователи");
            if (!НастройкиБазыДанных.Приложение)
            {
                foreach (ListViewItem Пользователь in НастройкиБазыДанных.ДополнительныеПользователи.Items)
                {
                    XmlElement ДопПользователь = НастройкиHotTray.CreateElement("Пользователь");
                    УстановитьАтрибутУзла(ДопПользователь, "Имя", Пользователь.Text);
                    УстановитьАтрибутУзла(ДопПользователь, "Пароль", Шифрование(Пользователь.SubItems[1].Text));
                    ДопПользователи.AppendChild(ДопПользователь);
                }
                БазаДанных.AppendChild(ДопПользователи);
            }
        }

        // Процедура изменяет настройки уже существующей базы данных
        //
        static public void ИзменитьНастройкуБазыДанных(XmlElement БазаДанных, СтруктураНастроекЭлемента НастройкиБазыДанных)
        {
            УстановитьАтрибутыБазыДанных(ref БазаДанных, НастройкиБазыДанных);
            if (БазаДанных.HasChildNodes)
            {
                БазаДанных.RemoveChild(БазаДанных.FirstChild);
            }
            ДобавитьДополнительныхПользователейВБазу(НастройкиБазыДанных, БазаДанных);
        }

        static public void ИзменитьНастройкуГруппы(XmlElement Группа, СтруктураНастроекГруппыЭлементов НастройкиГруппы)
        {
            УстановитьАтрибутыГруппы(ref Группа, НастройкиГруппы);
        }

        // Процедура добавляет новую группу баз данных в настройки
        //
        static public XmlElement ДобавитьГруппуБазВНастройки(СтруктураНастроекГруппыЭлементов НоваяГруппа, XmlNode РодительГруппы)
        {
            // Если родитель группы не указан, используем непосредственно узел баз данных (корень групп)
            if (РодительГруппы == null)
            {
                РодительГруппы = УзелБазДанных;
            }

            XmlElement ГруппаБазДанных = НастройкиHotTray.CreateElement("Группа");

            УстановитьАтрибутыГруппы(ref ГруппаБазДанных, НоваяГруппа);
            ГруппаБазДанных.SetAttribute("ГруппаРаскрыта", Convert.ToString(НоваяГруппа.ГруппаРаскрыта));
            ГруппаБазДанных.SetAttribute("ГруппаАктивна", Convert.ToString(НоваяГруппа.ГруппаАктивна));

            РодительГруппы.AppendChild(ГруппаБазДанных);                       

            return ГруппаБазДанных;
        }

        static public XmlNode ДобавитьИсключениеВНастройки(string ПутьИсполняемогоФайла)
        {
            
            XmlElement Исключения = (XmlElement)УзелИсключений;
            XmlElement НовоеИсключение = НастройкиHotTray.CreateElement("Исключение");
            НовоеИсключение.SetAttribute("Путь", ПутьИсполняемогоФайла);

            Исключения.AppendChild(НовоеИсключение);

            return (XmlNode)НовоеИсключение;
        }

        // Процедура удаляет указанный элемент из настроек XML
        //
        static public void УдалитьЭлементНастройки(XmlElement РодительЭлемента, XmlElement Элемент)
        {
            РодительЭлемента.RemoveChild((XmlNode)Элемент);
        }

        // Процедура перемещает узел базы данных либо на один вверх
        // либо на один уровень вниз
        static public void ПереместитьУзелБазыДанных(XmlNode УзелБазыДанных, XmlNode УзелРодителя, XmlNode СмещаемыйУзел, int Направление)
        {
            if (Направление == 1)
                УзелРодителя.InsertBefore(УзелБазыДанных, СмещаемыйУзел);
            else
                УзелРодителя.InsertAfter(УзелБазыДанных, СмещаемыйУзел);
        }

        // Процедура осуществляет сортировку списка баз в указанном направлении
        //
        static public Boolean СортироватьСписокБаз(XmlNode РодительБаз, ListView СписокБазДанных, SortOrder НаправлениеСортировки)
        {
            Boolean ТребуетсяПерерисовкаДерева = false;
            // Сортируем список
            int i = 0;
            if (СписокБазДанных.Items.Count != 0)
            {   
                ListViewItem КорневойЭлемент = null;
                if (String.IsNullOrEmpty(СписокБазДанных.Items[0].Text))
                    КорневойЭлемент = СписокБазДанных.Items[0];
                СписокБазДанных.Sorting = НаправлениеСортировки;
                СписокБазДанных.Sort();
                СписокБазДанных.Sorting = SortOrder.None;

                // Переместим группы вверх
                i = 0;
                ListBox СписокДляСортировки = new ListBox();
                for (i = 0; i < СписокБазДанных.Items.Count; i++)
                {
                    СписокДляСортировки.Items.Add(СписокБазДанных.Items[i]);
                }

                СписокБазДанных.Items.Clear();

                i = 0;
                while (i < СписокДляСортировки.Items.Count)
                {
                    ListViewItem ТекЭлементСписка = (ListViewItem)СписокДляСортировки.Items[i];
                    if (((XmlNode)ТекЭлементСписка.Tag).Name != "БазаДанных")
                    {
                        СписокБазДанных.Items.Add(ТекЭлементСписка);
                        СписокДляСортировки.Items.Remove(СписокДляСортировки.Items[i]);
                        ТребуетсяПерерисовкаДерева = true;
                    }
                    else
                        i++;
                }

                i = 0;
                while (i < СписокДляСортировки.Items.Count)
                {
                    ListViewItem ТекЭлементСписка = (ListViewItem)СписокДляСортировки.Items[i];
                    СписокБазДанных.Items.Add(ТекЭлементСписка);
                    i++;
                }
                СписокДляСортировки.Dispose();

                // Удаляем базы из текущего узла XML
                for (i = 0; i < СписокБазДанных.Items.Count; i++)
                {
                    XmlNode ТекУзел = (XmlNode)СписокБазДанных.Items[i].Tag;
                    if (ТекУзел != РодительБаз)
                        РодительБаз.RemoveChild(ТекУзел);
                }
                // Заново добавляем в новом порядке
                for (i = 0; i < СписокБазДанных.Items.Count; i++)
                {
                    XmlNode ТекУзел = (XmlNode)СписокБазДанных.Items[i].Tag;
                    if (ТекУзел != РодительБаз)
                        РодительБаз.AppendChild(ТекУзел);
                }

                if (КорневойЭлемент != null)
                {
                    ListViewItem КопияЭлемента = (ListViewItem)КорневойЭлемент.Clone();
                    КорневойЭлемент.Remove();
                    СписокБазДанных.Items.Insert(0, КопияЭлемента);
                }
            }

            return ТребуетсяПерерисовкаДерева;
        }

        // Процедура заменяет родителя базы данных
        // Используется при перетаскивании объектов
        static public void ПоменятьРодителяЭлемента(XmlNode СтарыйРодитель, XmlNode НовыйРодитель, XmlNode УзелДанных)
        {
            СтарыйРодитель.RemoveChild(УзелДанных);
            НовыйРодитель.AppendChild(УзелДанных);
        }

        static private Boolean ПроверкаВложенностиБаз(XmlNode УзелXML, bool УчитыватьТолькоРабочие)
        {
            Boolean ЕстьБазы = false;
            if (УзелXML.HasChildNodes)
            {
                if (ПроверкаВложенностиБаз(УзелXML.FirstChild, УчитыватьТолькоРабочие))
                    ЕстьБазы = true;
            }
            if (УзелXML.NextSibling != null)
            {
                if (ПроверкаВложенностиБаз(УзелXML.NextSibling, УчитыватьТолькоРабочие))
                    ЕстьБазы = true;
            }
            if (УзелXML.Name == "БазаДанных")
            {
                if (УчитыватьТолькоРабочие)
                    ЕстьБазы = Convert.ToBoolean(ПолучитьАтрибутУзла(УзелXML, "ПоказыватьВМенюЗапуска"));
                else
                    ЕстьБазы = true;
            }

            return ЕстьБазы;
        }

        static public Boolean ГруппаИмеетВложенныеБазы(XmlNode УзелГруппы, bool УчитыватьТолькоРабочие)
        {
            Boolean ЕстьБазы = false;
            if (УзелГруппы.HasChildNodes)
            {
                if (ПроверкаВложенностиБаз(УзелГруппы.FirstChild, УчитыватьТолькоРабочие))
                    ЕстьБазы = true;
            }

            return ЕстьБазы;
        }

        // Процедура обходит узлы дерева сформированного при поиске баз данных и 
        // переносит базы данных на один уровень вверх (для более правильного представления дерева)        
        static public void РекурсивнаяОбработкаПослеПоиска(XmlNode УзелXML, XmlNode Родитель, XmlNode Прародитель, ListBox СписокОбработанныхУзлов, ListBox СтарыеБазы)
        {
            Application.DoEvents();
            if (УзелXML.Name == "БазаДанных")
            {
                if (!СтарыеБазы.Items.Contains(УзелXML))
                {
                    if (Прародитель != null)
                    {
                        if (!СписокОбработанныхУзлов.Items.Contains(УзелXML))
                        {   // Смещаем узел на один уровень вверх
                            ПоменятьРодителяЭлемента(Родитель, Прародитель, УзелXML);
                            СписокОбработанныхУзлов.Items.Add(УзелXML);
                        }
                    }
                }
            }
            if (УзелXML.HasChildNodes)
            {
                РекурсивнаяОбработкаПослеПоиска(УзелXML.FirstChild, УзелXML, УзелXML.ParentNode, СписокОбработанныхУзлов, СтарыеБазы);
            }
            if (УзелXML.NextSibling != null)
            {
                РекурсивнаяОбработкаПослеПоиска(УзелXML.NextSibling, Родитель, Прародитель, СписокОбработанныхУзлов, СтарыеБазы);
            }
        }

        // Процедура удаляет папки дерева в которых нет баз данных
        // Выполняется за рекурсивной обработкой после поиска баз данных
        static public void РекурсивноеУдалениеПустыхПапок(XmlNode УзелXML)
        {
            Application.DoEvents();
            if ((УзелXML.Name == "Группа") || (УзелXML.Name == "БазыДанных"))
            {
                if (!ГруппаИмеетВложенныеБазы(УзелXML, false))
                {
                    if (УзелXML.NextSibling != null)
                        РекурсивноеУдалениеПустыхПапок(УзелXML.NextSibling);                    
                    УдалитьЭлементНастройки((XmlElement)УзелXML.ParentNode, (XmlElement)УзелXML);
                }
                else
                {
                    if (УзелXML.HasChildNodes)
                        РекурсивноеУдалениеПустыхПапок(УзелXML.FirstChild);
                    if (УзелXML.NextSibling != null)
                        РекурсивноеУдалениеПустыхПапок(УзелXML.NextSibling);
                }
            }
            else
            {
                if (УзелXML.NextSibling != null)
                    РекурсивноеУдалениеПустыхПапок(УзелXML.NextSibling);
            }
        }

        //////////////////////////////////////////////////////////////////////////////////////////////
        //////////////////////////////////////////////////////////////////////////////////////////////
        //////////////////////////////////////////////////////////////////////////////////////////////

        # region БлокРаботыСГорячимиКлавишами

        static private XmlNode ПолучитьУзелПоГорячейКлавише(int Клавиша, XmlNode УзелДерева)
        {
            XmlNode ПолученныйУзел = null;
            if (УзелДерева == null)
                return null;
            if (УзелДерева.HasChildNodes)
            {
                ПолученныйУзел = ПолучитьУзелПоГорячейКлавише(Клавиша, УзелДерева.FirstChild);
                if (ПолученныйУзел != null)
                    return ПолученныйУзел;
            }
            if (УзелДерева.NextSibling != null)
            {
                ПолученныйУзел = ПолучитьУзелПоГорячейКлавише(Клавиша, УзелДерева.NextSibling);
                if (ПолученныйУзел != null)
                    return ПолученныйУзел;
            }
            if ((УзелДерева.Name == "БазаДанных") || (УзелДерева.Name == "Группа"))
            {
                // Получить сочетание клавиш базы
                string Сочетание = ПолучитьАтрибутУзла(УзелДерева, "СочетаниеКлавиш");
                if (String.IsNullOrEmpty(Сочетание))
                    return null;
                string СтрокаРегистрации = String.Empty;

                string[] СтрокаКлавиш = Сочетание.Split('\\');
                int ПервыйСимвол = Convert.ToInt16(СтрокаКлавиш[0]);
                int ВторойСимвол = Convert.ToInt16(СтрокаКлавиш[1]);
                int ТретийСимвол = 0;
                try
                {
                    ТретийСимвол = Convert.ToInt16(СтрокаКлавиш[2]);
                }
                catch
                { }

                if ((ПервыйСимвол == 0) | (ВторойСимвол == 0))
                    return null;

                Keys ПервыйКлюч = Keys.D0;
                Keys ВторойКлюч = Keys.D0;
                if (ПервыйСимвол == 18)
                {
                    ПервыйКлюч = Keys.Alt;
                    if (ВторойСимвол == 16)
                        ВторойКлюч = Keys.Shift;
                    else
                        ВторойКлюч = (Keys)ВторойСимвол;
                }
                else if (ПервыйСимвол == 17)
                {
                    ПервыйКлюч = Keys.Control;
                    {
                        if (ВторойСимвол == 16)
                            ВторойКлюч = Keys.Shift;
                        else
                            ВторойКлюч = (Keys)ВторойСимвол;
                    }
                }
                else if (ПервыйСимвол == 16)
                {
                    ПервыйКлюч = Keys.Shift;
                    ВторойКлюч = (Keys)ВторойСимвол;
                }
                

                string ПерваяКлавиша = ПервыйКлюч.ToString();
                string ВтораяКлавиша = ВторойКлюч.ToString();
                СтрокаРегистрации = ПерваяКлавиша + "+" + ВтораяКлавиша;
                
                if (ТретийСимвол != 0)                
                    СтрокаРегистрации = СтрокаРегистрации + "+" + ((Keys)ТретийСимвол).ToString();                

                if (Клавиша == GlobalAddAtom(СтрокаРегистрации))
                    return УзелДерева;                    
            }
            return null;
        }

        static private void КорректировкаГорячихКлавиш(XmlNode УзелДанных, XmlNode УзелДерева)
        {
            if (УзелДерева == null)
                return;
            
            string СочетаниеКлавиш = ПолучитьАтрибутУзла(УзелДанных, "СочетаниеКлавиш");
            string[] СтрокаКлавиш = СочетаниеКлавиш.Split('\\');
            int ПервыйСимвол = Convert.ToInt16(СтрокаКлавиш[0]);
            int ВторойСимвол = Convert.ToInt16(СтрокаКлавиш[1]);            
            int ТретийСимвол = 0;
            try
            {
                ТретийСимвол = Convert.ToInt16(СтрокаКлавиш[2]);
            }
            catch
            { }

            string ТекСтрокаСочетания = ((Keys)ПервыйСимвол).ToString() + ((Keys)ВторойСимвол).ToString() + ((Keys)ТретийСимвол).ToString();
            string СтрокаСочетания = "";

            if (УзелДерева.HasChildNodes)
                КорректировкаГорячихКлавиш(УзелДанных, УзелДерева.FirstChild);
            if (УзелДерева.NextSibling != null)
                КорректировкаГорячихКлавиш(УзелДанных, УзелДерева.NextSibling);
            if (String.IsNullOrEmpty(СочетаниеКлавиш) || (СочетаниеКлавиш == "0\\0") || (СочетаниеКлавиш == "\\") || (СочетаниеКлавиш == "0\\0\\0"))
                return;            
            if (УзелДанных != УзелДерева)
            {
                if ((УзелДерева.Name == "БазаДанных") | (УзелДерева.Name == "Группа"))
                {
                    string ТекСочетание = ПолучитьАтрибутУзла(УзелДерева, "СочетаниеКлавиш");
                    {
                        if (String.IsNullOrEmpty(ТекСочетание) || (ТекСочетание == "0\\0") || (ТекСочетание == "\\") || (ТекСочетание == "0\\0\\0"))
                            return;            

                        СтрокаКлавиш = ТекСочетание.Split('\\');                                                
                        ПервыйСимвол = Convert.ToInt16(СтрокаКлавиш[0]);
                        ВторойСимвол = Convert.ToInt16(СтрокаКлавиш[1]);
                        ТретийСимвол = 0;
                        try
                        {
                            ТретийСимвол = Convert.ToInt16(СтрокаКлавиш[2]);
                        }
                        catch
                        { }

                        СтрокаСочетания = ((Keys)ПервыйСимвол).ToString() + ((Keys)ВторойСимвол).ToString() + ((Keys)ТретийСимвол).ToString();                        
                    }
                    if (ТекСтрокаСочетания == СтрокаСочетания)
                        УстановитьАтрибутУзла((XmlElement)УзелДерева, "СочетаниеКлавиш", "");
                }
            }            
        }

        static private void ПолучитьИнформациюПоГорячимКлавишам(XmlNode УзелДерева, ref string Информация)
        {
            string Описание = String.Empty;
            if (УзелДерева == null)
                return;
            if (УзелДерева.HasChildNodes)
            {
                ПолучитьИнформациюПоГорячимКлавишам(УзелДерева.FirstChild, ref Информация);                
            }
            if (УзелДерева.NextSibling != null)
            {
                ПолучитьИнформациюПоГорячимКлавишам(УзелДерева.NextSibling, ref Информация);  
            }
            if ((УзелДерева.Name == "БазаДанных") || (УзелДерева.Name == "Группа"))
            {                
                string Сочетание = ПолучитьАтрибутУзла(УзелДерева, "СочетаниеКлавиш");
                if (String.IsNullOrEmpty(Сочетание))
                    return;
                string СтрокаРегистрации = String.Empty;

                string[] СтрокаКлавиш = Сочетание.Split('\\');
                int ПервыйСимвол = Convert.ToInt16(СтрокаКлавиш[0]);
                int ВторойСимвол = Convert.ToInt16(СтрокаКлавиш[1]);
                int ТретийСимвол = 0;
                try
                {
                    ТретийСимвол = Convert.ToInt16(СтрокаКлавиш[2]);
                }
                catch
                { }

                if ((ПервыйСимвол == 0) | (ВторойСимвол == 0))
                    return;

                string ПерваяКлавиша = ((Keys)ПервыйСимвол).ToString();
                string ВтораяКлавиша = ((Keys)ВторойСимвол).ToString();
                string ТретьяКлавиша = ((Keys)ТретийСимвол).ToString();

                ПерваяКлавиша = ПерваяКлавиша.Replace("Menu", "ALT");
                ПерваяКлавиша = ПерваяКлавиша.Replace("ControlKey", "CTRL");
                ПерваяКлавиша = ПерваяКлавиша.Replace("ShiftKey", "SHIFT");
                ПерваяКлавиша = ПерваяКлавиша.Replace("Shift", "SHIFT");
                ПерваяКлавиша = ПерваяКлавиша.Replace("Control", "CTRL");
                ПерваяКлавиша = ПерваяКлавиша.Replace("SHIFTKEY", "SHIFT");

                ВтораяКлавиша = ВтораяКлавиша.Replace("Menu", "ALT");
                ВтораяКлавиша = ВтораяКлавиша.Replace("ControlKey", "CTRL");
                ВтораяКлавиша = ВтораяКлавиша.Replace("ShiftKey", "SHIFT");
                ВтораяКлавиша = ВтораяКлавиша.Replace("Shift", "SHIFT");
                ВтораяКлавиша = ВтораяКлавиша.Replace("Control", "CTRL");
                ВтораяКлавиша = ВтораяКлавиша.Replace("SHIFTKEY", "SHIFT");
                ВтораяКлавиша = ВтораяКлавиша.Replace("D", "");

                ТретьяКлавиша = ТретьяКлавиша.Replace("D", "");

                СтрокаРегистрации = ПерваяКлавиша + " + " + ВтораяКлавиша;
                if (ТретийСимвол != 0)
                    СтрокаРегистрации = СтрокаРегистрации + " + " + ТретьяКлавиша;

                if (!String.IsNullOrEmpty(Информация))
                    Информация = Информация + "\n";

                if (УзелДерева.Name == "Группа")
                    Информация = Информация + "Группа: ";

                Информация = Информация + (ПолучитьАтрибутУзла(УзелДерева, "Наименование")).Trim() + " - " + СтрокаРегистрации;
            }            
        }

        static public void ЗарегистрироватьВсеГорячиеКлавиши(XmlNode УзелДерева, int НомерПрохода)
        {
            if (НомерПрохода == 0)
            {
                // Регистрируем комбинацию (ALT+H)
                Keys ПервыйКлюч = Keys.Alt;
                Keys ВторойКлюч = Keys.H;

                string ПерваяКлавиша = ПервыйКлюч.ToString();
                string ВтораяКлавиша = ВторойКлюч.ToString();

                string СтрокаРегистрации = ПерваяКлавиша + "+" + ВтораяКлавиша;

                Keys КлючРегистрации = ПервыйКлюч | ВторойКлюч;

                RegisterHotKey(ЭтаФорма, КлючРегистрации, СтрокаРегистрации, true);
            }

            НомерПрохода++;

            if (УзелДерева == null)
                return;
            if (УзелДерева.HasChildNodes)
                ЗарегистрироватьВсеГорячиеКлавиши(УзелДерева.FirstChild, НомерПрохода);
            if (УзелДерева.NextSibling != null)
                ЗарегистрироватьВсеГорячиеКлавиши(УзелДерева.NextSibling, НомерПрохода);
            if ((УзелДерева.Name == "БазаДанных") || (УзелДерева.Name == "Группа"))
            {
                // Получить сочетание клавиш базы
                string Сочетание = ПолучитьАтрибутУзла(УзелДерева, "СочетаниеКлавиш");
                if (String.IsNullOrEmpty(Сочетание))
                    return;
                string СтрокаРегистрации = String.Empty;
                
                string[] СтрокаКлавиш = Сочетание.Split('\\');
                int ПервыйСимвол = Convert.ToInt16(СтрокаКлавиш[0]);
                int ВторойСимвол = Convert.ToInt16(СтрокаКлавиш[1]);
                int ТретийСимвол = 0;
                try
                {
                    ТретийСимвол = Convert.ToInt16(СтрокаКлавиш[2]);
                }
                catch
                { }

                if ((ПервыйСимвол == 0) | (ВторойСимвол == 0))
                    return;

                Keys ПервыйКлюч = Keys.D0;
                Keys ВторойКлюч = Keys.D0;
                if (ПервыйСимвол == 18)
                {
                    ПервыйКлюч = Keys.Alt;
                    if (ВторойСимвол == 16)
                        ВторойКлюч = Keys.Shift;
                    else
                        ВторойКлюч = (Keys)ВторойСимвол;
                }
                else if (ПервыйСимвол == 17)
                {
                    ПервыйКлюч = Keys.Control;
                    {
                        if (ВторойСимвол == 16)
                            ВторойКлюч = Keys.Shift;
                        else
                            ВторойКлюч = (Keys)ВторойСимвол;
                    }
                }
                else if (ПервыйСимвол == 16)
                {
                    ПервыйКлюч = Keys.Shift;
                    ВторойКлюч = (Keys)ВторойСимвол;
                }


                string ПерваяКлавиша = ПервыйКлюч.ToString();
                string ВтораяКлавиша = ВторойКлюч.ToString();
                СтрокаРегистрации = ПерваяКлавиша + "+" + ВтораяКлавиша;
                Keys КлючРегистрации = ПервыйКлюч | ВторойКлюч;

                if (ТретийСимвол != 0)
                {
                    СтрокаРегистрации = СтрокаРегистрации + "+" + ((Keys)ТретийСимвол).ToString();
                    КлючРегистрации = КлючРегистрации | (Keys)ТретийСимвол;
                }              

                RegisterHotKey(ЭтаФорма, КлючРегистрации, СтрокаРегистрации, false);
            }            
        }

        static public void ОтменитьРегистрациюВсехГорячихКлавиш(XmlNode УзелДерева, int НомерПрохода)
        {
            if (НомерПрохода == 0)
            {
                // Отменяем регистрацию ALT+H                                
                keyId = GlobalAddAtom(Keys.Alt.ToString() + "+" + Keys.H.ToString());

                Func ff = delegate()
                {
                    UnregisterHotKey(ЭтаФорма.Handle, keyId);
                    GlobalDeleteAtom(keyId);
                };

                ЭтаФорма.Invoke(ff);
            }

            НомерПрохода++;

            if (УзелДерева == null)
                return;
            if (УзелДерева.HasChildNodes)
                ОтменитьРегистрациюВсехГорячихКлавиш(УзелДерева.FirstChild, НомерПрохода);
            if (УзелДерева.NextSibling != null)
                ОтменитьРегистрациюВсехГорячихКлавиш(УзелДерева.NextSibling, НомерПрохода);
            if ((УзелДерева.Name == "БазаДанных") | (УзелДерева.Name == "Группа"))
            {                
                string Сочетание = ПолучитьАтрибутУзла(УзелДерева, "СочетаниеКлавиш");
                if (String.IsNullOrEmpty(Сочетание))
                    return;
               
                string[] СтрокаКлавиш = Сочетание.Split('\\');
                int ПервыйСимвол = Convert.ToInt16(СтрокаКлавиш[0]);
                int ВторойСимвол = Convert.ToInt16(СтрокаКлавиш[1]);
                int ТретийСимвол = 0;
                try
                {
                    ТретийСимвол = Convert.ToInt16(СтрокаКлавиш[2]);
                }
                catch
                { }

                if ((ПервыйСимвол == 0) | (ВторойСимвол == 0))
                    return;

                Keys ПервыйКлюч = Keys.D0;
                Keys ВторойКлюч = Keys.D0;
                if (ПервыйСимвол == 18)
                {
                    ПервыйКлюч = Keys.Alt;
                    if (ВторойСимвол == 16)
                        ВторойКлюч = Keys.Shift;
                    else
                        ВторойКлюч = (Keys)ВторойСимвол;
                }
                else if (ПервыйСимвол == 17)
                {
                    ПервыйКлюч = Keys.Control;
                    {
                        if (ВторойСимвол == 16)
                            ВторойКлюч = Keys.Shift;
                        else
                            ВторойКлюч = (Keys)ВторойСимвол;
                    }
                }
                else if (ПервыйСимвол == 16)
                {
                    ПервыйКлюч = Keys.Shift;
                    ВторойКлюч = (Keys)ВторойСимвол;
                }


                string ПерваяКлавиша = ПервыйКлюч.ToString();
                string ВтораяКлавиша = ВторойКлюч.ToString();
                string СтрокаРегистрации = ПерваяКлавиша + "+" + ВтораяКлавиша;                

                if (ТретийСимвол != 0)                
                    СтрокаРегистрации = СтрокаРегистрации + "+" + ((Keys)ТретийСимвол).ToString();                

                keyId = GlobalAddAtom(СтрокаРегистрации);

                Func ff = delegate()
                {
                    UnregisterHotKey(ЭтаФорма.Handle, keyId);
                    GlobalDeleteAtom(keyId);
                };

                ЭтаФорма.Invoke(ff);
            }            
        }

        private static short keyId;

        public static void RegisterHotKey(Form f, Keys key, string textAtom, Boolean ЭтоCTRLH)
        {
            int modifiers = 0;

            if ((key & Keys.Alt) == Keys.Alt)
                modifiers = modifiers | MOD_ALT;

            if ((key & Keys.Control) == Keys.Control)
                modifiers = modifiers | MOD_CONTROL;

            if ((key & Keys.Shift) == Keys.Shift)
                modifiers = modifiers | MOD_SHIFT;

            Keys k = key & ~Keys.Control & ~Keys.Shift & ~Keys.Alt;

            try
            {
                keyId = GlobalAddAtom(textAtom);
                if (ЭтоCTRLH)
                    IDCTRLH = keyId;    

                if (keyId == 0)
                {
                    if (СписокНеудачныхГорячихКлавиш.FindString(textAtom) == -1)
                    {
                        ПоказатьИнфомационноеСообщение("Не могу зарегистрировать горячую клавишу (" + textAtom + "). Error: " + Marshal.GetLastWin32Error().ToString(), ЭтаФорма);
                        СписокНеудачныхГорячихКлавиш.Items.Add(textAtom);
                    }
                    

                    throw new Exception();                    
                }

                if (!RegisterHotKey((IntPtr)f.Handle, keyId, modifiers, (int)k))
                {
                    if (СписокНеудачныхГорячихКлавиш.FindString(textAtom) == -1)
                    {
                        ПоказатьИнфомационноеСообщение("Не могу зарегистрировать горячую клавишу (" + textAtom + "), возможна она уже занята другим приложением. Error: " + Marshal.GetLastWin32Error().ToString(), ЭтаФорма);
                        СписокНеудачныхГорячихКлавиш.Items.Add(textAtom);
                    }
                    throw new Exception();                    
                }

            }
            catch
            {                
                UnregisterHotKey(f.Handle, keyId);
            }

        }

        private delegate void Func();          
        #endregion

        // Процедура инициализации главного окна        
        //
        public ГлавноеОкно()
        {
            InitializeComponent();
            HookMouse = new UserActivityHook(false, false);
            НастройкиHotTray = new XmlDocument();            

            КоллекцияСписка = new ImageList();
            КоллекцияСписка.ColorDepth = ColorDepth.Depth32Bit;
            КоллекцияСписка.ImageSize = new Size(33, 16);
            int i = 0;
            foreach (Image Картинка in КоллекцияИконокСписка.Images)
            {
                КоллекцияСписка.Images.Add(КоллекцияИконокСписка.Images.Keys[i], Картинка);
                i++;
            }
            КоллекцияИконокСписка.Dispose();
            СтраницаБазДанных.SmallImageList = КоллекцияСписка;

            КаталогПрограммы = System.Windows.Forms.Application.StartupPath;
            Opacity = 0;
            ЗаполнениеНастроекФормы();            
            НастройкиИзменены = false;
            УправлениеХуком();
            ТекущаяКартинкаАнимации = 0;
            ПанельАнимации.Visible = false;
            Коэффициент = 2;
            ЭтаФорма = this;
            ЗарегистрироватьВсеГорячиеКлавиши(УзелБазДанных, 0);            
            // Проверка обновлений
            if (ПроверятьОбновленияПриЗапуске.Checked)
            {
                ПотокПроверкиОбновления = new Thread(НачатьПроверкуОбновления);
                ПотокПроверкиОбновления.Start();
            }            

            НеобходимоПерерисоватьМеню = true;
            
            CreateToolTipMessages();
        }
        
        private void CreateToolTipMessages()
        {   			        	
			string substring = "";
        	string ToolTips = System.Text.Encoding.UTF8.GetString(Properties.Resources.ToolTips);
			
        	StringReader reader = new StringReader(ToolTips);
			
			substring = reader.ReadLine();
			while (substring != null)
			{        					
				string[] sToolTip = substring.Split(':');
				if (sToolTip.Length == 2)
				{	 
					 ToolTipMessages.Add(sToolTip[0], sToolTip[1]);
				}
				substring = reader.ReadLine();
			}			
			
			reader.Close();
			reader.Dispose();			        	
        }

        static void НачатьПроверкуОбновления()
        {
            ФормаОбновления ПроверкаОбновления = new ФормаОбновления();
            if (ПроверкаОбновления.ЕстьОбновление(false))
            {
                if (MessageBox.Show(null, "Обнаружена новая версия программы. Открыть окно обновления?", "Hot tray 1C .NET", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                    ПроверкаОбновления.ShowDialog();
            }
            ПроверкаОбновления.Dispose();
            ОчисткаМусора();
        }
       
        private void СохранениеПараметровФормы()
        {
            try
            {
                УстановитьЗначениеНастройки("ЦветФонаМеню", Convert.ToString(ИндикаторЦветФонаМеню.BackColor.R) + ";" + Convert.ToString(ИндикаторЦветФонаМеню.BackColor.G) + ";" + Convert.ToString(ИндикаторЦветФонаМеню.BackColor.B));
                УстановитьЗначениеНастройки("ЦветТекстаМеню", Convert.ToString(ИндикаторЦветШрифта.BackColor.R) + ";" + Convert.ToString(ИндикаторЦветШрифта.BackColor.G) + ";" + Convert.ToString(ИндикаторЦветШрифта.BackColor.B));
                УстановитьЗначениеНастройки("ЦветСепаратора", Convert.ToString(ИндикаторЦветСепаратора.BackColor.R) + ";" + Convert.ToString(ИндикаторЦветСепаратора.BackColor.G) + ";" + Convert.ToString(ИндикаторЦветСепаратора.BackColor.B));
                УстановитьЗначениеНастройки("РазмерШрифтаМеню", РазмерШрифта.Text);
                УстановитьЗначениеНастройки("НачертаниеШрифтаМеню", НачертаниеШрифта.Text);
                if (СохранятьПоложениеОкна.Checked)
                {
                    Properties.Settings.Default.ЛевыйКрай = Left;
                    Properties.Settings.Default.ВерхнийКрай = Top;
                    Properties.Settings.Default.Ширина = Width;
                    Properties.Settings.Default.Высота = Height;
                    Properties.Settings.Default.ШиринаДереваГрупп = ДеревоГруппБазДанных.Width;
                    Properties.Settings.Default.ТекущаяСтраница = ПанельПриложения.SelectedIndex;
                    Properties.Settings.Default.Save();
                }
            }
            catch
            {                
            }
        }

        private void ВывестиСочетанияКлавиш()
        {
            string ОписаниеКлавиш = String.Empty;
            ПолучитьИнформациюПоГорячимКлавишам(УзелБазДанных, ref ОписаниеКлавиш);
            if (!String.IsNullOrEmpty(ОписаниеКлавиш))
            {
                ИконкаВТрее.BalloonTipText = ОписаниеКлавиш;
                ИконкаВТрее.BalloonTipIcon = ToolTipIcon.Info;
                ИконкаВТрее.BalloonTipTitle = "Список горячих клавиш";
                ИконкаВТрее.ShowBalloonTip(3000);
            }
        }

        // Процедура обрабатывающее сообщения посылаемые форме приложения
        //
        protected override void WndProc(ref Message m)
        {            
            if (m.Msg == WM_HOTKEY)
            {                
                if ((int)m.WParam == IDCTRLH)
                {
                    ВывестиСочетанияКлавиш();
                }
                else
                {
                    int XScreen = Screen.PrimaryScreen.Bounds.Width;
                    int YScreen = Screen.PrimaryScreen.Bounds.Height;

                    int x = Convert.ToInt16(ПолучитьЗначениеНастройки("HotX", Convert.ToString(XScreen / 2)));
                    int y = Convert.ToInt16(ПолучитьЗначениеНастройки("HotY", Convert.ToString(YScreen / 2)));

                    if (x > XScreen)
                        x = XScreen / 2;
                    if (y > YScreen)
                        y = YScreen / 2;

                    XmlNode Узел = ПолучитьУзелПоГорячейКлавише((int)m.WParam, УзелБазДанных);
                    if (Узел != null)
                    {
                        МенюЗапуска.Items.Clear();
                        Boolean ДобавлятьПунктыВМеню = Convert.ToBoolean(ПолучитьЗначениеНастройки("ДобавлятьПунктыНепосредственноВМенюЗапуска", "true"));

                        СобственноеОформление = Convert.ToBoolean(ПолучитьЗначениеНастройки("НастраиваемоеМенюЗапуска", "false"));
                        
                        if (СобственноеОформление)
                        {
                            СтруктураОформления = ПолучитьСтруктуруОформленияМеню();                            
                        }

                        if (Узел.Name == "Группа")
                        {
                            ЗаполнениеМенюЗапуска(МенюЗапуска, Узел.FirstChild, null, ДобавлятьПунктыВМеню);

                            // Сортировка элементов
                            СортировкаЭлементовМеню(МенюЗапуска.Items);

                            if (ДобавлятьПунктыВМеню)
                            {
                                ToolStripItem ДополнительныйЭлементМеню;
                                if (МенюЗапуска.Items.Count > 0)
                                {
                                    if (СобственноеОформление)
                                    {
                                        // Прортсовываем собственный разделитель                            
                                        ДополнительныйЭлементМеню = МенюЗапуска.Items.Add("");
                                        ДополнительныйЭлементМеню.BackColor = СтруктураОформления.ЦветСепаратора;                                        
                                        ДополнительныйЭлементМеню.AutoSize = false;
                                        ДополнительныйЭлементМеню.Size = new Size(500, 1);
                                    }
                                    else
                                        ДополнительныйЭлементМеню = МенюЗапуска.Items.Add("-");
                                }
                                ДополнительныйЭлементМеню = МенюЗапуска.Items.Add("Добавить группу");
                                ДополнительныйЭлементМеню.Image = Properties.Resources.folderSmall;
                                ДополнительныйЭлементМеню.Tag = Узел;
                                ДополнительныйЭлементМеню.Click += new EventHandler(ДобавитьГруппу_Click);
                                if (СобственноеОформление)
                                {
                                    ДополнительныйЭлементМеню.BackColor = СтруктураОформления.ЦветФона;
                                    ДополнительныйЭлементМеню.ForeColor = Color.Transparent;
                                    ДополнительныйЭлементМеню.Paint += new PaintEventHandler(ПрорисовкаДобавитьГруппуБазу);
                                }
                                ДополнительныйЭлементМеню = МенюЗапуска.Items.Add("Добавить базу");
                                ДополнительныйЭлементМеню.Image = Properties.Resources.SmallAdd;
                                ДополнительныйЭлементМеню.Tag = Узел;
                                ДополнительныйЭлементМеню.Click += new EventHandler(ДобавитьБазу_Click);                                
                                if (СобственноеОформление)
                                {
                                    ДополнительныйЭлементМеню.BackColor = СтруктураОформления.ЦветФона;
                                    ДополнительныйЭлементМеню.ForeColor = Color.Transparent;
                                    ДополнительныйЭлементМеню.Paint += new PaintEventHandler(ПрорисовкаДобавитьГруппуБазу);
                                }
                            }
                            SetForegroundWindow((IntPtr)ЭтаФорма.Handle);
                            МенюЗапуска.Show(x, y);
                            НеобходимоПерерисоватьМеню = true;
                        }
                        else
                        {
                            СтруктураНастроекЭлемента НастройкаБазы = ПолучитьНастройкиБазыДанных(Узел);
                            if (!НастройкаБазы.Приложение)
                            {
                                if (!НастройкаБазы.РежимЗапускаКакПунктМеню)
                                {
                                    if (НастройкаБазы.РежимЗапуска == 0)
                                    {
                                        // Формируем новое меню с режимами запуска
                                        Пауза(100);
                                        ContextMenuStrip МенюВыбораРежима = СоздатьМенюВыбораРежимаЗапуска(НастройкаБазы);
                                        SetForegroundWindow((IntPtr)Handle);
                                        МенюВыбораРежима.Show(x, y);
                                    }
                                    else
                                    {
                                        НачатьЗапускБазыДанных(НастройкаБазы);
                                    }
                                    НеобходимоПерерисоватьМеню = true;
                                }
                                else
                                {
                                    ToolStripItem ЭлементМенюСРежимамиЗапуска;
                                    int ТекПлатформа = НастройкаБазы.ТипПлатформы;

                                    if (ТекПлатформа == 0)
                                    {
                                        // 1С Предприятие 7.7
                                        ЭлементМенюСРежимамиЗапуска = МенюЗапуска.Items.Add("1С Предприятие");
                                        НастройкаБазы.РежимЗапуска = 1;
                                        ЭлементМенюСРежимамиЗапуска.Tag = НастройкаБазы; 
                                        if (СобственноеОформление)
                                        {
                                            ЭлементМенюСРежимамиЗапуска.BackColor = СтруктураОформления.ЦветФона;
                                            ЭлементМенюСРежимамиЗапуска.ForeColor = Color.Transparent;
                                            ЭлементМенюСРежимамиЗапуска.Paint += new PaintEventHandler(ПрорисовкаПодПунктаМенюЗапуска);
                                        }
                                        else
                                            ЭлементМенюСРежимамиЗапуска.Image = ПолучитьИконкуРежима(0, 1);
                                        
                                        // Создание подменю с выбором режима работы
                                        ToolStripMenuItem Подменю1СПредприятия = (ToolStripMenuItem)ЭлементМенюСРежимамиЗапуска;
                                        ToolStripItem ЭлементМенюСРежимамиРаботы;
                                        // Монопольный
                                        ЭлементМенюСРежимамиРаботы = Подменю1СПредприятия.DropDownItems.Add("Монопольный");
                                        НастройкаБазы.РежимРаботы = 1;
                                        ЭлементМенюСРежимамиРаботы.Tag = НастройкаБазы;                                        
                                        ЭлементМенюСРежимамиРаботы.Click += new EventHandler(ПунктРежимаРаботы_Click);
                                        if (СобственноеОформление)
                                        {
                                            ЭлементМенюСРежимамиРаботы.BackColor = СтруктураОформления.ЦветФона;
                                            ЭлементМенюСРежимамиРаботы.ForeColor = Color.Transparent;
                                            ЭлементМенюСРежимамиРаботы.Paint += new PaintEventHandler(ПрорисовкаМенюЗапускаРежимРаботы);
                                        }
                                        else
                                            ЭлементМенюСРежимамиРаботы.Image = TestMenuPopup.Properties.Resources.Монопольный;

                                        // Разделенный
                                        ЭлементМенюСРежимамиРаботы = Подменю1СПредприятия.DropDownItems.Add("Разделенный");
                                        НастройкаБазы.РежимРаботы = 2;
                                        ЭлементМенюСРежимамиРаботы.Tag = НастройкаБазы;                                        
                                        ЭлементМенюСРежимамиРаботы.Click += new EventHandler(ПунктРежимаРаботы_Click);
                                        if (СобственноеОформление)
                                        {
                                            ЭлементМенюСРежимамиРаботы.BackColor = СтруктураОформления.ЦветФона;
                                            ЭлементМенюСРежимамиРаботы.ForeColor = Color.Transparent;
                                            ЭлементМенюСРежимамиРаботы.Paint += new PaintEventHandler(ПрорисовкаМенюЗапускаРежимРаботы);
                                        }
                                        else
                                            ЭлементМенюСРежимамиРаботы.Image = TestMenuPopup.Properties.Resources.Разделенный;

                                        // Конфигуратор 7.7
                                        ЭлементМенюСРежимамиЗапуска = МенюЗапуска.Items.Add("Кофигуратор");
                                        НастройкаБазы.РежимЗапуска = 2;
                                        ЭлементМенюСРежимамиЗапуска.Tag = НастройкаБазы;                                        
                                        ЭлементМенюСРежимамиЗапуска.Click += new EventHandler(ПунктРежимаЗапуска_Click);
                                        if (СобственноеОформление)
                                        {
                                            ЭлементМенюСРежимамиЗапуска.BackColor = СтруктураОформления.ЦветФона;
                                            ЭлементМенюСРежимамиЗапуска.ForeColor = Color.Transparent;
                                            ЭлементМенюСРежимамиЗапуска.Paint += new PaintEventHandler(ПрорисовкаПодПунктаМенюЗапуска);
                                        }
                                        else
                                            ЭлементМенюСРежимамиЗапуска.Image = ПолучитьИконкуРежима(0, 2);
                                        // Отладчик 7.7
                                        ЭлементМенюСРежимамиЗапуска = МенюЗапуска.Items.Add("Отладчик");
                                        НастройкаБазы.РежимЗапуска = 3;
                                        ЭлементМенюСРежимамиЗапуска.Tag = НастройкаБазы;                                        
                                        ЭлементМенюСРежимамиЗапуска.Click += new EventHandler(ПунктРежимаЗапуска_Click);
                                        if (СобственноеОформление)
                                        {
                                            ЭлементМенюСРежимамиЗапуска.BackColor = СтруктураОформления.ЦветФона;
                                            ЭлементМенюСРежимамиЗапуска.ForeColor = Color.Transparent;
                                            ЭлементМенюСРежимамиЗапуска.Paint += new PaintEventHandler(ПрорисовкаПодПунктаМенюЗапуска);
                                        }
                                        else
                                            ЭлементМенюСРежимамиЗапуска.Image = ПолучитьИконкуРежима(0, 3);
                                        // Монитор 7.7
                                        ЭлементМенюСРежимамиЗапуска = МенюЗапуска.Items.Add("Монитор");
                                        НастройкаБазы.РежимЗапуска = 4;
                                        ЭлементМенюСРежимамиЗапуска.Tag = НастройкаБазы;                                        
                                        ЭлементМенюСРежимамиЗапуска.Click += new EventHandler(ПунктРежимаЗапуска_Click);
                                        if (СобственноеОформление)
                                        {
                                            ЭлементМенюСРежимамиЗапуска.BackColor = СтруктураОформления.ЦветФона;
                                            ЭлементМенюСРежимамиЗапуска.ForeColor = Color.Transparent;
                                            ЭлементМенюСРежимамиЗапуска.Paint += new PaintEventHandler(ПрорисовкаПодПунктаМенюЗапуска);
                                        }
                                        else
                                            ЭлементМенюСРежимамиЗапуска.Image = ПолучитьИконкуРежима(0, 4);
                                    }
                                    else
                                    {
                                        // 1С Предприятие 8.х
                                        ЭлементМенюСРежимамиЗапуска = МенюЗапуска.Items.Add("1С Предприятие");
                                        НастройкаБазы.РежимЗапуска = 1;
                                        ЭлементМенюСРежимамиЗапуска.Tag = НастройкаБазы;                                                                                
                                        if (СобственноеОформление)
                                        {
                                            ЭлементМенюСРежимамиЗапуска.BackColor = СтруктураОформления.ЦветФона;
                                            ЭлементМенюСРежимамиЗапуска.ForeColor = Color.Transparent;
                                            ЭлементМенюСРежимамиЗапуска.Paint += new PaintEventHandler(ПрорисовкаПодПунктаМенюЗапуска);
                                        }
                                        else
                                            ЭлементМенюСРежимамиЗапуска.Image = ПолучитьИконкуРежима(ТекПлатформа, 1);


                                        // Если это 8.2 возможно надо добавить подменю клиента
                                        if ((НастройкаБазы.ТипПлатформы > 2) && (НастройкаБазы.ВидКлиентаКакПунктМеню))
                                        {
                                            ToolStripMenuItem Подменю1СПредприятия = (ToolStripMenuItem)ЭлементМенюСРежимамиЗапуска;
                                            ToolStripItem ЭлементСВидомКлиента;
                                            // Автоматический режим
                                            ЭлементСВидомКлиента = Подменю1СПредприятия.DropDownItems.Add("Автоматически");
                                            НастройкаБазы.ВидКлиента = 0;
                                            ЭлементСВидомКлиента.Tag = НастройкаБазы;
                                            ЭлементСВидомКлиента.Click += new EventHandler(ПунктВидаКлиента_Click);
                                            if (СобственноеОформление)
                                            {
                                                ЭлементСВидомКлиента.BackColor = СтруктураОформления.ЦветФона;
                                                ЭлементСВидомКлиента.ForeColor = Color.Transparent;
                                                ЭлементСВидомКлиента.Paint += new PaintEventHandler(ПрорисовкаПодПунктаВидаКлиента);
                                            }
                                            // Тонкий клиент
                                            ЭлементСВидомКлиента = Подменю1СПредприятия.DropDownItems.Add("Тонкий клиент");
                                            НастройкаБазы.ВидКлиента = 2;
                                            ЭлементСВидомКлиента.Tag = НастройкаБазы;
                                            ЭлементСВидомКлиента.Click += new EventHandler(ПунктВидаКлиента_Click);
                                            if (СобственноеОформление)
                                            {
                                                ЭлементСВидомКлиента.BackColor = СтруктураОформления.ЦветФона;
                                                ЭлементСВидомКлиента.ForeColor = Color.Transparent;
                                                ЭлементСВидомКлиента.Paint += new PaintEventHandler(ПрорисовкаПодПунктаВидаКлиента);
                                            }
                                            // Толстый клиент
                                            ЭлементСВидомКлиента = Подменю1СПредприятия.DropDownItems.Add("Толстый клиент");
                                            НастройкаБазы.ВидКлиента = 3;
                                            ЭлементСВидомКлиента.Tag = НастройкаБазы;
                                            ЭлементСВидомКлиента.Click += new EventHandler(ПунктВидаКлиента_Click);
                                            if (СобственноеОформление)
                                            {
                                                ЭлементСВидомКлиента.BackColor = СтруктураОформления.ЦветФона;
                                                ЭлементСВидомКлиента.ForeColor = Color.Transparent;
                                                ЭлементСВидомКлиента.Paint += new PaintEventHandler(ПрорисовкаПодПунктаВидаКлиента);
                                            }
                                        }
                                        else
                                            ЭлементМенюСРежимамиЗапуска.Click += new EventHandler(ПунктРежимаЗапуска_Click);


                                        // Кофигуратор 8.х
                                        ЭлементМенюСРежимамиЗапуска = МенюЗапуска.Items.Add("Конфигуратор");
                                        НастройкаБазы.РежимЗапуска = 2;
                                        ЭлементМенюСРежимамиЗапуска.Tag = НастройкаБазы;                                        
                                        ЭлементМенюСРежимамиЗапуска.Click += new EventHandler(ПунктРежимаЗапуска_Click);
                                        if (СобственноеОформление)
                                        {
                                            ЭлементМенюСРежимамиЗапуска.BackColor = СтруктураОформления.ЦветФона;
                                            ЭлементМенюСРежимамиЗапуска.ForeColor = Color.Transparent;
                                            ЭлементМенюСРежимамиЗапуска.Paint += new PaintEventHandler(ПрорисовкаПодПунктаМенюЗапуска);
                                        }
                                        else
                                            ЭлементМенюСРежимамиЗапуска.Image = ПолучитьИконкуРежима(ТекПлатформа, 2);

                                        // Монитор 8.x
                                        ЭлементМенюСРежимамиЗапуска = МенюЗапуска.Items.Add("Монитор пользователей");
                                        НастройкаБазы.РежимЗапуска = 4;
                                        ЭлементМенюСРежимамиЗапуска.Tag = НастройкаБазы;
                                        ЭлементМенюСРежимамиЗапуска.Click += new EventHandler(МониторПользователей_Click);
                                        if (СобственноеОформление)
                                        {
                                            ЭлементМенюСРежимамиЗапуска.BackColor = СтруктураОформления.ЦветФона;
                                            ЭлементМенюСРежимамиЗапуска.ForeColor = Color.Transparent;
                                            ЭлементМенюСРежимамиЗапуска.Paint += new PaintEventHandler(ПрорисовкаПодПунктаМенюЗапуска);
                                        }
                                        else
                                            ЭлементМенюСРежимамиЗапуска.Image = ПолучитьИконкуРежима(ТекПлатформа, 4);
                                    }

                                    SetForegroundWindow((IntPtr)Handle);
                                    МенюЗапуска.Show(x, y);
                                    НеобходимоПерерисоватьМеню = true;
                                }
                            }
                            else
                            {
                                НачатьЗапускБазыДанных(НастройкаБазы);
                            }
                        }
                    }
                }
            }
            base.WndProc(ref m);
        }       

        private void УстановитьПараметрыОкна()
        {
            if (СохранятьПоложениеОкна.Checked)
            {
                try
                {
                    int ЛевыйКрай = Properties.Settings.Default.ЛевыйКрай;
                    int ВерхнийКрай = Properties.Settings.Default.ВерхнийКрай;
                    int Ширина = Properties.Settings.Default.Ширина;
                    int Высота = Properties.Settings.Default.Высота;
                    int ШиринаДереваГрупп = Properties.Settings.Default.ШиринаДереваГрупп;
                    if ((ЛевыйКрай != 0) & (ВерхнийКрай != 0) & (Ширина != 0) & (Высота != 0) & (ШиринаДереваГрупп != 0))
                    {
                        Left = ЛевыйКрай;
                        Top = ВерхнийКрай;
                        Width = Ширина;
                        Height = Высота;
                        ДеревоГруппБазДанных.Width = ШиринаДереваГрупп;
                        ПанельПриложения.SelectedIndex = Properties.Settings.Default.ТекущаяСтраница;
                    }
                }
                catch
                {                 
                }
            }
        }

        private Color ПолучитьЦветИзСтроки(string СтрокаЦвета, Color ПредопределенныйЦвет)
        {
            string[] Гамма = СтрокаЦвета.Split(';');
            try
            {
                return Color.FromArgb(Convert.ToInt16(Гамма[0]), Convert.ToInt16(Гамма[1]), Convert.ToInt16(Гамма[2]));
            }
            catch
            {
                return ПредопределенныйЦвет;
            }
        }

        // Процедура заполняет настройки в форме исходя из "глобальных" настроек
        //
        public void ЗаполнениеНастроекФормы()
        {
            ЗагрузитьНастройкиПрограммы(ДеревоГруппБазДанных, СписокИсключений);
            ПутьЗапуска77.Text = ПолучитьЗначениеНастройки("ПутьЗапуска77", "");
            ПутьЗапуска80.Text = ПолучитьЗначениеНастройки("ПутьЗапуска80", "");
            ПутьЗапуска81.Text = ПолучитьЗначениеНастройки("ПутьЗапуска81", "");
            ПутьЗапуска82.Text = ПолучитьЗначениеНастройки("ПутьЗапуска82", "");
            ПутьЗапуска83.Text = ПолучитьЗначениеНастройки("ПутьЗапуска83", "");
            ИмяПользователяПоУмолчанию.Text = ПолучитьЗначениеНастройки("ИмяПользователяПоУмолчанию", "");
            ПарольПользователяПоУмолчанию.Text = Шифрование(ПолучитьЗначениеНастройки("ПарольПользователяПоУмолчанию", ""));
            ЗапускатьВместеСWindows.Checked = Convert.ToBoolean(ПолучитьЗначениеНастройки("ЗапускатьВместеСWindows", "true"));            
            СохранятьПоложениеОкна.Checked = Convert.ToBoolean(ПолучитьЗначениеНастройки("СохранятьПоложениеОкна", "false"));
            АктивироватьСреднююКнопку.Checked = Convert.ToBoolean(ПолучитьЗначениеНастройки("АктивироватьСреднююКнопку", "true"));
            ПроверятьНаличиеБазыПриДобавлении.Checked = Convert.ToBoolean(ПолучитьЗначениеНастройки("ПроверятьНаличиеБазыПриДобавлении", "false"));
            ДобавлятьПунктыНепосредственноВМенюЗапуска.Checked = Convert.ToBoolean(ПолучитьЗначениеНастройки("ДобавлятьПунктыНепосредственноВМенюЗапуска", "true"));
            ТипПлатформыПоУмолчанию.SelectedIndex = Convert.ToInt16(ПолучитьЗначениеНастройки("ТипПлатформыПоУмолчанию", "4"));            
            РежимЗапускаПоУмолчанию.SelectedIndex = Convert.ToInt16(ПолучитьЗначениеНастройки("РежимЗапускаПоУмолчанию", "0"));
            ПапкаСБазамиПоУмолчанию.Text = ПолучитьЗначениеНастройки("ПапкаСБазамиПоУмолчанию", "");
            УдалятьИз1СПриУдалении.Checked = Convert.ToBoolean(ПолучитьЗначениеНастройки("УдалятьИз1СПриУдалении", "false"));
            ПроверятьОбновленияПриЗапуске.Checked = Convert.ToBoolean(ПолучитьЗначениеНастройки("ПроверятьОбновлениеПриЗапуске", "true"));
            НастраиваемоеМенюЗапуска.Checked = Convert.ToBoolean(ПолучитьЗначениеНастройки("НастраиваемоеМенюЗапуска", "false"));
            ИндикаторЦветФонаМеню.BackColor = ПолучитьЦветИзСтроки(ПолучитьЗначениеНастройки("ЦветФонаМеню", ""), Color.White);
            ИндикаторЦветШрифта.BackColor = ПолучитьЦветИзСтроки(ПолучитьЗначениеНастройки("ЦветТекстаМеню", ""), Color.Black);
            ИндикаторЦветСепаратора.BackColor = ПолучитьЦветИзСтроки(ПолучитьЗначениеНастройки("ЦветСепаратора", ""), Color.Black);
            РазмерШрифта.Text = ПолучитьЗначениеНастройки("РазмерШрифтаМеню", "8");
            НачертаниеШрифта.Text = ПолучитьЗначениеНастройки("НачертаниеШрифтаМеню", "обычный");
            УправлениеДоступностью();
            УправлениеРежимомЗапуска();
            ЗагрузитьРабочийСтол();
        }

        static Image ИзменитьРазмерИзображения(Image source, int width, int height)
        {

            Image dest = new Bitmap(width, height);
            using (Graphics gr = Graphics.FromImage(dest))
            {                
                gr.FillRectangle(Brushes.Transparent, 0, 0, width, height);  // Очищаем экран
                gr.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;

                float srcwidth = source.Width;
                float srcheight = source.Height;
                float dstwidth = width;
                float dstheight = height;

                if (srcwidth <= dstwidth && srcheight <= dstheight)  // Исходное изображение меньше целевого
                {
                    int left = (width - source.Width) / 2;
                    int top = (height - source.Height) / 2;
                    gr.DrawImage(source, left, top, source.Width, source.Height);
                }
                else if (srcwidth / srcheight > dstwidth / dstheight)  // Пропорции исходного изображения более широкие
                {
                    float cy = srcheight / srcwidth * dstwidth;
                    float top = ((float)dstheight - cy) / 2.0f;
                    if (top < 1.0f) top = 0;
                    gr.DrawImage(source, 0, top, dstwidth, cy);
                }
                else  // Пропорции исходного изображения более узкие
                {
                    float cx = srcwidth / srcheight * dstheight;
                    float left = ((float)dstwidth - cx) / 2.0f;
                    if (left < 1.0f) left = 0;
                    gr.DrawImage(source, left, 0, cx, dstheight);
                }

                return dest;
            }
        }
        
        static Image ПолучитьИконкуВнешнегоПриложения(СтруктураНастроекЭлемента Настройка)
        {
            try
            {
                Icon ИконкаПриложения = Icon.ExtractAssociatedIcon(Настройка.Путь);
                
                String ВремКаталог = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\HotTrayNET";
                if (!Directory.Exists(ВремКаталог))                
                    Directory.CreateDirectory(ВремКаталог);                

                ИконкаПриложения.ToBitmap().Save(ВремКаталог + "\\Icons.ico");            
                Image Изображение = ИзменитьРазмерИзображения(Image.FromFile(ВремКаталог + "\\Icons.ico"), 16, 16);
                GC.Collect();
                GC.WaitForPendingFinalizers();
                return Изображение;
            }
            catch
            {
                return Properties.Resources.Монопольный;
            }            
        }

        static bool УдалосьДобавитьИконкуВКоллекцию(СтруктураНастроекЭлемента Настройка, ref int ИндексИконки)
        {
            try
            {
                // Получаем изображение состояния из ресурсов
                Bitmap ЗначокСостояние;
                if (Настройка.ПоказыватьВМенюЗапуска)
                    ЗначокСостояние = Properties.Resources.Отображать;
                else
                    ЗначокСостояние = Properties.Resources.НеОтображать;

                // Извлекаем иконку файла
                Icon ИконкаПриложения = Icon.ExtractAssociatedIcon(Настройка.Путь);

                String ВремКаталог = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\HotTrayNET";
                if (!Directory.Exists(ВремКаталог))
                {
                    Directory.CreateDirectory(ВремКаталог);
                }

                ИконкаПриложения.ToBitmap().Save(ВремКаталог + "\\Icons.ico");
                Image Изображение = Image.FromFile(ВремКаталог + "\\Icons.ico");
                // Меняем размер на 16х16
                Изображение = ИзменитьРазмерИзображения(Изображение, 16, 16);
                Изображение.Save(ВремКаталог + "\\IconsConv.ico");
                Изображение.Dispose();
                Bitmap СтарыйРисунок = new Bitmap(ВремКаталог + "\\IconsConv.ico");

                // Создаем финальный рисунок размером 33х16
                Bitmap НовыйРисунок = new Bitmap(33, 16);
                // Переносим в него изображение состояния
                for (int i = 0; i < 16; i++)
                {
                    for (int y = 0; y < 16; y++)
                    {
                        Color Цвет = ЗначокСостояние.GetPixel(i, y);
                        НовыйРисунок.SetPixel(i, y, Цвет);
                    }
                }
                // Переносим в него изображение приложения
                for (int i = 0; i < 16; i++)
                {
                    for (int y = 0; y < 16; y++)
                    {
                        Color Цвет = СтарыйРисунок.GetPixel(i, y);
                        НовыйРисунок.SetPixel(i + 16, y, Цвет);
                    }
                }

                НовыйРисунок.Save(ВремКаталог + "\\IconsFinal.ico");

                ИконкаПриложения.Dispose();
                НовыйРисунок.Dispose();
                СтарыйРисунок.Dispose();
                
                КоллекцияСписка.Images.Add(Image.FromFile(ВремКаталог + "\\IconsFinal.ico"));
                ИндексИконки = КоллекцияСписка.Images.Count - 1;
                
                GC.Collect();
                GC.WaitForPendingFinalizers();                

                return true;
            }
            catch
            {
                return false;
            }
        }

        private void СозданиеНовогоПриложения()
        {
            ОтменитьРегистрациюВсехГорячихКлавиш(УзелБазДанных, 0);
            // Добавление новой базы данных
            if (ДеревоГруппБазДанных.SelectedNode == null)
            {
                ДеревоГруппБазДанных.SelectedNode = ДеревоГруппБазДанных.Nodes[0];
            }
            СтруктураНастроекЭлемента НовоеПриложение;
            ФормаВнешнегоПриложения ФормаНастроек = new ФормаВнешнегоПриложения();
            ФормаНастроек.СоздатьПриложение((XmlNode)ДеревоГруппБазДанных.SelectedNode.Tag);
            if (ФормаНастроек.ShowDialog(this) == DialogResult.OK)
            {
                XmlElement РодительБазы = (XmlElement)ФормаНастроек.РодительБазыДанных;

                // Добавляем базу в глобальный настройки
                НовоеПриложение = ФормаНастроек.ТекущаяНастройка;
                XmlElement УзелПриложения = ДобавитьБазуДанныхВНастройки(НовоеПриложение, РодительБазы);
                // Базу в список добавляем только в том случае, если группа ее совпадает с выбранной
                if (РодительБазы == (XmlElement)ДеревоГруппБазДанных.SelectedNode.Tag)
                {
                    ListViewItem НоваяСтрокаСтраницы = СтраницаБазДанных.Items.Add(НовоеПриложение.Наименование);
                    НоваяСтрокаСтраницы.SubItems.Add(НовоеПриложение.Путь);
                    НоваяСтрокаСтраницы.SubItems.Add("");
                    
                    int ИндексИконки = 0;
                    if (УдалосьДобавитьИконкуВКоллекцию(НовоеПриложение, ref ИндексИконки))
                        НоваяСтрокаСтраницы.ImageIndex = ИндексИконки;
                    else                        
                    {
                        string СостояниеПриложения = Convert.ToString(Convert.ToInt16(НовоеПриложение.ПоказыватьВМенюЗапуска)) + "1";
                        НоваяСтрокаСтраницы.ImageKey = НоваяСтрокаСтраницы.ImageKey = "Отсутствует" + СостояниеПриложения + ".png";
                    }                    

                    НоваяСтрокаСтраницы.SubItems.Add("Да");
                    НоваяСтрокаСтраницы.Tag = УзелПриложения;
                }                
                КорректировкаГорячихКлавиш(УзелПриложения, УзелБазДанных);
                НастройкиИзменены = true;
            }
            ФормаНастроек.Dispose();
            ЗарегистрироватьВсеГорячиеКлавиши(УзелБазДанных, 0);
            ОчисткаМусора();
        }

        private void СозданиеНовойБазыДанных()
        {
            ОтменитьРегистрациюВсехГорячихКлавиш(УзелБазДанных, 0);
            // Добавление новой базы данных
            if (ДеревоГруппБазДанных.SelectedNode == null)
            {
                ДеревоГруппБазДанных.SelectedNode = ДеревоГруппБазДанных.Nodes[0];
            }
            СтруктураНастроекЭлемента НоваяБазаДанных;
            ФормаБазыДанных ФормаНастроек = new ФормаБазыДанных();
            ФормаНастроек.СоздатьБазуДанных((XmlNode)ДеревоГруппБазДанных.SelectedNode.Tag);
            if (ФормаНастроек.ShowDialog(this) == DialogResult.OK)
            {
                XmlElement РодительБазы = (XmlElement)ФормаНастроек.РодительБазыДанных;

                // Добавляем базу в глобальный настройки
                НоваяБазаДанных = ФормаНастроек.ТекущаяНастройка;
                XmlElement УзелБазыДанных = ДобавитьБазуДанныхВНастройки(НоваяБазаДанных, РодительБазы);
                // Базу в список добавляем только в том случае, если группа ее совпадает с выбранной
                if (РодительБазы == (XmlElement)ДеревоГруппБазДанных.SelectedNode.Tag)
                {
                    ListViewItem НоваяСтрокаСтраницы = СтраницаБазДанных.Items.Add(НоваяБазаДанных.Наименование);
                    
                    string СостояниеБазы = String.Empty;

                    if (БазаДанныхСуществуетПоУказанномуПути(НоваяБазаДанных))
                    {
                        СостояниеБазы = ПолучитьСостояниеБазы(НоваяБазаДанных);
                        НоваяСтрокаСтраницы.ImageKey = "Состояние" + СостояниеБазы + ".png";
                    }
                    else
                    {
                        СостояниеБазы = ПолучитьСостояниеБазы(НоваяБазаДанных);
                        НоваяСтрокаСтраницы.ImageKey = "Отсутствует" + СостояниеБазы + ".png";
                    } 
                    НоваяСтрокаСтраницы.SubItems.Add(НоваяБазаДанных.Путь);
                    НоваяСтрокаСтраницы.SubItems.Add(НоваяБазаДанных.ИмяПользователя);
                    НоваяСтрокаСтраницы.Tag = УзелБазыДанных;
                }
                РегистрацияБазы77(НоваяБазаДанных);
                КорректировкаГорячихКлавиш(УзелБазыДанных, УзелБазДанных);
                НастройкиИзменены = true;
            }
            ФормаНастроек.Dispose();
            ЗарегистрироватьВсеГорячиеКлавиши(УзелБазДанных, 0);
            ОчисткаМусора();
        }

        static private bool БазаДанныхСуществуетПоУказанномуПути(СтруктураНастроекЭлемента НастройкиБазы)
        {
            if (НастройкиБазы.ТипБазы == 1)
                return true;
            
            string ИмяФайлаДляПроверки = String.Empty;

            if (НастройкиБазы.ТипПлатформы == 0)
                ИмяФайлаДляПроверки = "\\1Cv7.MD";
            else
                ИмяФайлаДляПроверки = "\\1Cv8.1CD";

            string КаталогБазыДанных = НастройкиБазы.Путь;

            if (КаталогБазыДанных[КаталогБазыДанных.Length - 1] == '\\')
            {
                КаталогБазыДанных = КаталогБазыДанных.Remove(КаталогБазыДанных.Length - 1, 1);
            }

            if (!File.Exists(КаталогБазыДанных + ИмяФайлаДляПроверки))
                return false;
            
            return true;
        }        

        private void ВыборТипаДобавляемогоОбъекта_Click(object sender, EventArgs e)
        {
            ToolStripItem Отправитель = (ToolStripItem)sender;
            if ((int)Отправитель.Tag == 0)
                СозданиеНовойБазыДанных();
            else
                СозданиеНовогоПриложения();
        }

        private void ДобавитьБазуДанных_Click(object sender, EventArgs e)
        {
            ContextMenuStrip ВыборДействия = new ContextMenuStrip();
            ВыборДействия.ShowImageMargin = false;
            ToolStripItem ЭлементМеню;
            ЭлементМеню = ВыборДействия.Items.Add("                                      ");
            ЭлементМеню.Paint += new PaintEventHandler(ПрорисовкаВерхнегоЭлементаМенюДействий);
            ЭлементМеню.Click += new EventHandler(УстановитьРежиЗапуска_Click);
            ЭлементМеню.Tag = 1;
            ЭлементМеню.Enabled = false;
            ЭлементМеню = ВыборДействия.Items.Add("-");
            ЭлементМеню = ВыборДействия.Items.Add("База данных");
            ЭлементМеню.Click += new EventHandler(ВыборТипаДобавляемогоОбъекта_Click);
            ЭлементМеню.Tag = 0;
            ЭлементМеню = ВыборДействия.Items.Add("Приложение");
            ЭлементМеню.Click += new EventHandler(ВыборТипаДобавляемогоОбъекта_Click);
            ЭлементМеню.Tag = 1;
            ВыборДействия.Show(MousePosition.X, MousePosition.Y);            
        }

        static private void ОчисткаРеестраБаз77(XmlNode УзелXML)
        {
            if (УзелXML == null)
                return;
            if (УзелXML.Name == "БазаДанных")
                УдалениеБазы77(УзелXML);
            if (УзелXML.HasChildNodes)
                ОчисткаРеестраБаз77(УзелXML.FirstChild);
            if (УзелXML.NextSibling != null)
                ОчисткаРеестраБаз77(УзелXML.NextSibling);
        }

        static private void УдалениеБазы77(XmlNode УзелXML)
        {
            if (Convert.ToBoolean(ПолучитьЗначениеНастройки("УдалятьИз1СПриУдалении", "false")))
            {
                if (Convert.ToInt16(ПолучитьАтрибутУзла(УзелXML, "ТипПлатформы")) == 0)
                {
                    string ТекПуть = ПолучитьАтрибутУзла(УзелXML, "Путь").ToLower();
                    if (ТекПуть[ТекПуть.Length - 1] != '\\')
                    {
                        ТекПуть = ТекПуть + "\\";
                    }

                    string РазделСБазами77 = @"Software\1C\1Cv7\7.7\Titles";
                    try
                    {
                        RegistryKey Раздел77 = Registry.CurrentUser.OpenSubKey(РазделСБазами77, true);
                        if (Раздел77 != null)
                        {
                            string[] ПутиБазДанных = Раздел77.GetValueNames();
                            for (int i = 0; i < ПутиБазДанных.Length; i++)
                            {
                                string ПутьБазыДанных = ПутиБазДанных[i];
                                string ПервоначальныйПуть = ПутиБазДанных[i];

                                ПутьБазыДанных = ПутьБазыДанных.ToLower();
                                if (ПутьБазыДанных[ПутьБазыДанных.Length - 1] != '\\')
                                {
                                    ПутьБазыДанных = ПутьБазыДанных + "\\";
                                }

                                if (ПутьБазыДанных == ТекПуть)
                                {
                                    try
                                    {
                                        Раздел77.DeleteValue(ПервоначальныйПуть);
                                        return;
                                    }
                                    catch
                                    {
                                        ПоказатьИнфомационноеСообщение("Не удалось удалить базу из реестра. Возможно недостаточно прав!", ЭтаФорма);
                                    }
                                }

                            }
                        }
                    }
                    catch
                    {
                        ПоказатьИнфомационноеСообщение("При удалении базы данных из реестра произошла ошибка. Error: " + Marshal.GetLastWin32Error().ToString(), ЭтаФорма);
                    }
                }
            }
        }

        static private void РегистрацияБазы77(СтруктураНастроекЭлемента НастройкиБазы)
        {
            if (НастройкиБазы.ТипПлатформы == 0)
            {
                string ТекПуть = НастройкиБазы.Путь.ToLower();
                if (ТекПуть[ТекПуть.Length - 1] != '\\')
                {
                    ТекПуть = ТекПуть + "\\";
                }
                
                string РазделСБазами77 = @"Software\1C\1Cv7\7.7\Titles";
                try
                {
                    RegistryKey Раздел77 = Registry.CurrentUser.OpenSubKey(РазделСБазами77, true);
                    if (Раздел77 != null)
                    {
                        Boolean БазаЕстьВСписке = false;
                        string[] ПутиБазДанных = Раздел77.GetValueNames();
                        for (int i = 0; i < ПутиБазДанных.Length; i++)
                        {
                            string ПутьБазыДанных = ПутиБазДанных[i];

                            ПутьБазыДанных = ПутьБазыДанных.ToLower();
                            if (ПутьБазыДанных[ПутьБазыДанных.Length - 1] != '\\')
                            {
                                ПутьБазыДанных = ПутьБазыДанных + "\\";
                            }

                            if (ПутьБазыДанных == ТекПуть)
                                БазаЕстьВСписке = true;
                        }

                        if (!БазаЕстьВСписке)
                        {
                            try
                            {
                                Раздел77.SetValue(НастройкиБазы.Путь, НастройкиБазы.Наименование);
                            }
                            catch
                            {
                                ПоказатьИнфомационноеСообщение("Не удалось зарегистрировать базу в реестре. Возможно недостаточно прав!", ЭтаФорма);
                            }
                        }
                    }
                }
                catch
                {
                    ПоказатьИнфомационноеСообщение("При регистрации базы данных в реестре произошла ошибка. Error: " + Marshal.GetLastWin32Error().ToString(), ЭтаФорма);
                }
            }
        }

        private void УдалитьБазуДанных()
        {
            ListView.SelectedListViewItemCollection ВыделенныеЭлементы = СтраницаБазДанных.SelectedItems;

            if (ВыделенныеЭлементы.Count != 0)
            {
                string ТекстВопроса = String.Empty;
                if (ВыделенныеЭлементы.Count == 1)
                {
                    if (String.IsNullOrEmpty(ВыделенныеЭлементы[0].Text))
                        return;

                    if (((XmlNode)ВыделенныеЭлементы[0].Tag).Name == "Группа")
                        ТекстВопроса = "Удалить группу базу данных?";
                    else
                    {
                        Boolean ЭтоПриложение = false;                        
                        ЭтоПриложение = Convert.ToBoolean(ПолучитьАтрибутУзла((XmlNode)ВыделенныеЭлементы[0].Tag, "Приложение", "false"));                        
                        if (!ЭтоПриложение)
                            ТекстВопроса = "Удалить базу данных?";
                        else
                            ТекстВопроса = "Удалить внешнее приложение?";
                    }
                }
                else
                    ТекстВопроса = "Удалить выбранные элементы?";

                if (Вопрос(ТекстВопроса, ЭтаФорма) == DialogResult.No)
                    return;

                // Определяем родителя текущей базы
                TreeNode РодительБазы = ДеревоГруппБазДанных.SelectedNode;
                if (ВыделенныеЭлементы.Count == 1)
                {                    
                    XmlElement УзелБазыДанных = (XmlElement)ВыделенныеЭлементы[0].Tag;
                    XmlNode XMLУзел = (XmlNode)УзелБазыДанных;
                    if (УзелБазыДанных.Name == "БазаДанных")
                    {
                        // Удаляется база
                        УдалениеБазы77(XMLУзел);
                        УдалитьЭлементНастройки((XmlElement)РодительБазы.Tag, УзелБазыДанных);
                        СтраницаБазДанных.Items.Remove(СтраницаБазДанных.SelectedItems[0]);
                    }
                    else
                    {
                        // Удаляется группа
                        if (ГруппаИмеетВложенныеБазы(XMLУзел, false))
                        {
                            if (Вопрос("Группа имеет вложенные базы данных. Все равно продолжить?", this) == DialogResult.No)
                            {
                                return;
                            }
                        }
                        // Определяем родителя текущей группы
                        TreeNode РодительГруппы = ДеревоГруппБазДанных.SelectedNode;
                        СтраницаБазДанных.Items.Remove(СтраницаБазДанных.SelectedItems[0]);
                        foreach (TreeNode ТекУзелДерева in ДеревоГруппБазДанных.SelectedNode.Nodes)
                        {
                            if ((XmlNode)ТекУзелДерева.Tag == XMLУзел)
                            {
                                ТекУзелДерева.Remove();
                                break;
                            }
                        }
                        ОчисткаРеестраБаз77(XMLУзел);
                        УдалитьЭлементНастройки((XmlElement)РодительГруппы.Tag, УзелБазыДанных);
                    }
                    НастройкиИзменены = true;
                }                 
                else
                {
                    int i = 0;
                    while (i < ВыделенныеЭлементы.Count)
                    {
                        TreeNode РодительГруппы = ДеревоГруппБазДанных.SelectedNode;
                        XmlElement УзелБазыДанных = (XmlElement)ВыделенныеЭлементы[i].Tag;
                        XmlNode XMLУзел = (XmlNode)УзелБазыДанных;
                        if (УзелБазыДанных.Name == "БазаДанных")
                        {
                            // Удаляется база
                            УдалениеБазы77((XmlNode)УзелБазыДанных);
                            УдалитьЭлементНастройки((XmlElement)РодительБазы.Tag, УзелБазыДанных);
                            СтраницаБазДанных.Items.Remove(СтраницаБазДанных.SelectedItems[i]);                        
                        }
                        else
                        {
                            if (String.IsNullOrEmpty(СтраницаБазДанных.SelectedItems[i].Text))
                            {
                                i++;
                                continue;
                            }
                            // Удаляется группа
                            if (ГруппаИмеетВложенныеБазы(XMLУзел, false))
                            {
                                if (Вопрос("Группа " + ПолучитьАтрибутУзла(XMLУзел, "Наименование") + " имеет вложенные базы данных. Все равно удалить?", this) == DialogResult.No)
                                {
                                    i++;
                                    continue;
                                }
                            }
                            // Определяем родителя текущей группы
                            СтраницаБазДанных.Items.Remove(СтраницаБазДанных.SelectedItems[i]);
                            foreach (TreeNode ТекУзелДерева in ДеревоГруппБазДанных.SelectedNode.Nodes)
                            {
                                if ((XmlNode)ТекУзелДерева.Tag == XMLУзел)
                                {
                                    ТекУзелДерева.Remove();
                                    break;
                                }
                            }
                            ОчисткаРеестраБаз77(XMLУзел);
                            УдалитьЭлементНастройки((XmlElement)РодительГруппы.Tag, УзелБазыДанных);
                        }
                    }
                }
                НастройкиИзменены = true;
            }
            ОчисткаМусора();
        }
       
        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            УдалитьБазуДанных();
        }

        private void ДобавитьГруппуБазДанных()
        {
            ОтменитьРегистрациюВсехГорячихКлавиш(УзелБазДанных, 0);
            
            Boolean НеобходимоРазвернутьУзел = false;
            if (ДеревоГруппБазДанных.SelectedNode == null)
            {
                ДеревоГруппБазДанных.SelectedNode = ДеревоГруппБазДанных.Nodes[0];
            }
            XmlNode РодительГруппы;
            try
            {
                РодительГруппы = (XmlNode)ДеревоГруппБазДанных.SelectedNode.Tag;
            }
            catch
            {
                РодительГруппы = null;
            }

            ФормаГруппы ФормаГруппы = new ФормаГруппы();
            ФормаГруппы.СоздатьГруппу();
            ФормаГруппы.ShowDialog();
            if (ФормаГруппы.DialogResult == DialogResult.OK)
            {
                НеобходимоРазвернутьУзел = (ДеревоГруппБазДанных.SelectedNode.GetNodeCount(true) == 0);

                TreeNode НоваяГруппа = new TreeNode();
                СтруктураНастроекГруппыЭлементов НастройкаГруппы = ФормаГруппы.ТекущаяБаза;
                НоваяГруппа.Text = НастройкаГруппы.Наименование;
                ДеревоГруппБазДанных.SelectedNode.Nodes.Add(НоваяГруппа);
                XmlElement УзелГруппыБазДанных = ДобавитьГруппуБазВНастройки(НастройкаГруппы, РодительГруппы);
                НоваяГруппа.Tag = УзелГруппыБазДанных;

                if (НеобходимоРазвернутьУзел)
                {
                    ДеревоГруппБазДанных.SelectedNode.Expand();
                }

                КорректировкаГорячихКлавиш(УзелГруппыБазДанных, УзелБазДанных);
                НастройкиИзменены = true;
            }

            ЗарегистрироватьВсеГорячиеКлавиши(УзелБазДанных, 0);

            ФормаГруппы.Dispose();
            ОчисткаМусора();
        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
             ДобавитьГруппуБазДанных();
        }

        private void ДеревоГруппБазДанных_AfterExpand(object sender, TreeViewEventArgs e)
        {

        }

        private void ПриЗакрытии()
        {
            if (НастройкиИзменены)
                if (ГлавноеОкно.Вопрос("Выйти без сохранения параметров", this) == DialogResult.No)
                    return;
                else
                    ГлавноеОкно.ЗагрузитьНастройкиПрограммы(ДеревоГруппБазДанных, СписокИсключений);
            НастройкиИзменены = false;
            Hide();
            ОчисткаМусора();
            if (ПотокПроверкиОбновления != null)
                ПотокПроверкиОбновления.Abort();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            ПриЗакрытии();
        }

        private void УправлениеХуком()
        {
            if (ПанельАнимации.Visible)
            {
                ПоказатьИнфомационноеСообщение("Дождитесь окончания поиска!", this);
                return;
            }
            if (Convert.ToBoolean(ПолучитьЗначениеНастройки("АктивироватьСреднююКнопку", "true")))
            {
                HookMouse.OnMouseActivity += new MouseEventHandler(HookMouse_OnMouseActivity);                
                HookMouse.Start(true, false);                
            }
            else
            {                
                HookMouse.Stop();                
            }
        }

        void HookMouse_OnMouseActivity(object sender, MouseEventArgs e)
        {
            if (e.Clicks > 0)
            {
                if (e.Button == MouseButtons.Middle)
                {
                    // Получим имя файла запуска процееса над которым произошол щелчок кнопкой мыши                
                    uint processId = 0;
                    IntPtr HDProc  = GetForegroundWindow();
                    GetWindowThreadProcessId(HDProc, out processId);                    
                    
                    Process[] СписокПроцессов = Process.GetProcesses();
                    foreach (Process Процесс in СписокПроцессов)
                    {
                        try
                        {
                            if (Процесс.Id == processId)
                            {
                                string ФайлЗапускаПроцесса = Процесс.MainModule.FileName;
                                // Ищем файл в исключениях
                                ListBox СписокФайлов = new ListBox();
                                ПолучитьИсключения(УзелИсключений, ref СписокФайлов);
                                if (СписокФайлов.FindString(ФайлЗапускаПроцесса) == -1)
                                {
                                    ПоказатьМенюИзФормы();
                                }
                                else
                                    SetForegroundWindow(HDProc);
                            }
                        }
                        catch
                        {
                        }

                    }
                }
            }
        }

        private void ОбработкаАвтозапуска()
        {            
            try
            {
                RegistryKey РазделАвтозапуска = Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Run", true);
                if (ЗапускатьВместеСWindows.Checked)
                {                    
                    РазделАвтозапуска.SetValue("HotTrayNET", Application.ExecutablePath);
                }
                else
                    РазделАвтозапуска.DeleteValue("HotTrayNET", false);
            }
            catch
            {
                ГлавноеОкно.ПоказатьИнфомационноеСообщение("Не удалось сохранить настройку автозапуска. Возможно недостаточно прав!", this);                
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            if (ПанельАнимации.Visible)
            {
                ПоказатьИнфомационноеСообщение("Дождитесь окончания поиска!", this);
                return;
            }
            ОбработкаАвтозапуска();
            СохранениеПараметровФормы();
            if (НастройкиИзменены)
            {
                СохранитьНастройкиПрограммы();
                ИнициализацияМенюЗапуска(МенюЗапуска);
            }
            УправлениеХуком();
            НастройкиИзменены = false;
            ОчисткаМусора();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            if (ПанельАнимации.Visible)
            {
                ПоказатьИнфомационноеСообщение("Дождитесь окончания поиска!", this);
                return;
            }
            ОбработкаАвтозапуска();
            СохранениеПараметровФормы();            
            УправлениеХуком();
            if (НастройкиИзменены)
            {
                СохранитьНастройкиПрограммы();
                ИнициализацияМенюЗапуска(МенюЗапуска);
            }
            НастройкиИзменены = false;
            Hide();
            ОчисткаМусора();
        }

        private void ДеревоГруппБазДанных_AfterSelect(object sender, TreeViewEventArgs e)
        {
            ЗаполнениеСпискаБаз(ДеревоГруппБазДанных, СтраницаБазДанных);
            if ((ПоследнийАктивныйУзелДереваГрупп != null) && (ПоследнийАктивныйУзелДереваГрупп != ДеревоГруппБазДанных.SelectedNode))
                УстановитьАтрибутУзла((XmlElement)ПоследнийАктивныйУзелДереваГрупп.Tag, "ГруппаАктивна", "false");
            ПоследнийАктивныйУзелДереваГрупп = ДеревоГруппБазДанных.SelectedNode;
            УправлениеКнопкойМонитора();
        }

        private void ДеревоГруппБазДанных_AfterExpand_1(object sender, TreeViewEventArgs e)
        {
            УстановитьАтрибутУзла((XmlElement)e.Node.Tag, "ГруппаРаскрыта", "true");
        }

        private void ДеревоГруппБазДанных_AfterCollapse(object sender, TreeViewEventArgs e)
        {
            УстановитьАтрибутУзла((XmlElement)e.Node.Tag, "ГруппаРаскрыта", "false");
        }

        private void ПунктВыход_Click(object sender, EventArgs e)
        {
            Close();
        }


        private void ИконкаВТрее_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                SetForegroundWindow((IntPtr)Handle);
                if (НеобходимоПерерисоватьМеню)
                {
                    ИнициализацияМенюЗапуска(МенюЗапуска);
                    НеобходимоПерерисоватьМеню = false;
                }
                
                МенюЗапуска.Show(MousePosition.X, MousePosition.Y);                
            }
        }

        private void ОбработкаКликаНаПунктеМенюЗапуска(object sender, MouseEventArgs e)
        {
        }

        private void МенюЗапуска_MouseClick(object sender, MouseEventArgs e)
        {
            
        }

        private string ВыборПутиЗапускаПлатформы(string Заголовок, string Фильтр, ref TextBox ПутьЗапускаПлатформы)
        {

            OpenFileDialog openFile = new OpenFileDialog();

            if (File.Exists(ПутьЗапускаПлатформы.Text))
            {
                openFile.FileName = ПутьЗапускаПлатформы.Text;
            }
            else
            {
                openFile.FileName = "";
            }
            openFile.Title = Заголовок;
            openFile.Filter = Фильтр;
            if (openFile.ShowDialog() == DialogResult.OK)
            {
                ПутьЗапускаПлатформы.Text = openFile.FileName;
                НастройкиИзменены = true;
                return ПутьЗапускаПлатформы.Text;                
            }

            openFile.Dispose();
            
            return ПутьЗапускаПлатформы.Text;
        }

        // Процедура выбора пути к файлу запуска 1С Предприятия 7.7
        private void КнопкаВыбратьПутьЗапуска77_Click(object sender, EventArgs e)
        {
            УстановитьЗначениеНастройки("ПутьЗапуска77", ВыборПутиЗапускаПлатформы("Укажите файл запуска 1С Предприятия 7.7", "Файл запуска 1С Предприятия|1cv7*.exe", ref ПутьЗапуска77));            
        }

        // Процедура выбора пути к файлу запуска 1С Предприятия 8.1
        private void КнопкаВыбратьПутьЗапуска81_Click_1(object sender, EventArgs e)
        {
            УстановитьЗначениеНастройки("ПутьЗапуска81", ВыборПутиЗапускаПлатформы("Укажите файл запуска 1С Предприятия 8.1", "Файл запуска 1С Предприятия|1cv8.exe", ref ПутьЗапуска81));            
        }


        // Процедура выбора пути к файлу запуска 1С Предприятия 8.0
        private void КнопкаВыбратьПутьЗапуска80_Click(object sender, EventArgs e)
        {
            УстановитьЗначениеНастройки("ПутьЗапуска80", ВыборПутиЗапускаПлатформы("Укажите файл запуска 1С Предприятия 8.0", "Файл запуска 1С Предприятия|1cv8.exe", ref ПутьЗапуска80));            
        }

        // Процедура выбора пути к файлу запуска 1С Предприятия 8.2
        private void КнопкаВыбратьПутьЗапуска82_Click(object sender, EventArgs e)
        {
            УстановитьЗначениеНастройки("ПутьЗапуска82", ВыборПутиЗапускаПлатформы("Укажите файл запуска 1С Предприятия 8.2", "Файл запуска 1С Предприятия|1cv8*.exe;1cestart.exe;starter.exe", ref ПутьЗапуска82));            
        }

        private void КнопкаВыбратьПутьЗапуска83_Click(object sender, EventArgs e)
        {
            УстановитьЗначениеНастройки("ПутьЗапуска83", ВыборПутиЗапускаПлатформы("Укажите файл запуска 1С Предприятия 8.3", "Файл запуска 1С Предприятия|1cv8*.exe;1cestart.exe;starter.exe", ref ПутьЗапуска83));
        }
              
        private void ДобавлятьПунктыНепосредственноВМенюЗапуска_Click(object sender, EventArgs e)
        {
            УстановитьЗначениеНастройки("ДобавлятьПунктыНепосредственноВМенюЗапуска", Convert.ToString(ДобавлятьПунктыНепосредственноВМенюЗапуска.Checked));
            НастройкиИзменены = true;
        }
     
        /// Процедура прорисовки элементов выпадающего списка "Тип платформы по умолчанию"
        ///
        private void ТипПлатформыПоУмолчанию_DrawItem(object sender, DrawItemEventArgs e)
        {
            if (e.Index != -1)
            {
                e.DrawBackground();
                // Прорисовываем прямоугольник
                e.Graphics.DrawRectangle(new Pen(Color.White, 1), new Rectangle(e.Bounds.X, e.Bounds.Y, 200, 15));
                // Рисуем справа рисунок
                e.Graphics.DrawImage(ПолучитьКартинкуПлатформы(e.Index), e.Bounds.X, e.Bounds.Y);
                // Выводим текст                
                e.Graphics.DrawString("     " + ТипПлатформыПоУмолчанию.Items[e.Index].ToString(), new Font(FontFamily.GenericSerif, 10, FontStyle.Regular), Brushes.Black, e.Bounds.X, e.Bounds.Y);                
            }
        }

        private void ТипПлатформыПоУмолчанию_SelectedIndexChanged(object sender, EventArgs e)
        {
            УстановитьЗначениеНастройки("ТипПлатформыПоУмолчанию", Convert.ToString(ТипПлатформыПоУмолчанию.SelectedIndex));
            УправлениеРежимомЗапуска();
        }

        private void РедактированиеГруппыВСписке(ListViewItem СтрокаСписка)
        {
            XmlNode УзелГруппы = (XmlNode)СтрокаСписка.Tag;
            ОтменитьРегистрациюВсехГорячихКлавиш(УзелБазДанных, 0);
            ФормаГруппы ФормаРедактированияГруппы = new ФормаГруппы();
            ФормаРедактированияГруппы.ОткрытьГруппу(УзелГруппы);
            ФормаРедактированияГруппы.ShowDialog();
            if (ФормаРедактированияГруппы.DialogResult == DialogResult.OK)
            {
                СтруктураНастроекГруппыЭлементов НастройкиГруппы = ФормаРедактированияГруппы.ТекущаяБаза;
                ДеревоГруппБазДанных.SelectedNode.Text = НастройкиГруппы.Наименование;
                ИзменитьНастройкуГруппы((XmlElement)УзелГруппы, НастройкиГруппы);
                КорректировкаГорячихКлавиш(УзелГруппы, УзелБазДанных);

                foreach (TreeNode ТекГруппа in ДеревоГруппБазДанных.SelectedNode.Nodes)
                {
                    if ((XmlNode)ТекГруппа.Tag == УзелГруппы)
                    {
                        ТекГруппа.Text = НастройкиГруппы.Наименование;
                        СтрокаСписка.Text = НастройкиГруппы.Наименование;
                        break;
                    }
                }

                НастройкиИзменены = true;
            }
            ФормаРедактированияГруппы.Dispose();

            ЗарегистрироватьВсеГорячиеКлавиши(УзелБазДанных, 0);
            ОчисткаМусора();
        }

        private void РедактированиеНастройкиБазыДанных()
        {
            if (СтраницаБазДанных.SelectedItems.Count == 1)
            {
                ListViewItem ВыбраннаяСтрока = СтраницаБазДанных.SelectedItems[0];

                if (String.IsNullOrEmpty(ВыбраннаяСтрока.Text))
                    return;

                if (((XmlNode)ВыбраннаяСтрока.Tag).Name != "БазаДанных")
                {
                    РедактированиеГруппыВСписке(ВыбраннаяСтрока);
                    return;
                }
                
                ОтменитьРегистрациюВсехГорячихКлавиш(УзелБазДанных, 0);
                XmlNode СтарыйРодитель = null;
                if (ДеревоГруппБазДанных.SelectedNode == null)
                {
                    ДеревоГруппБазДанных.SelectedNode = ДеревоГруппБазДанных.Nodes[0];                    
                }
                СтарыйРодитель = (XmlNode)ДеревоГруппБазДанных.SelectedNode.Tag;
                XmlNode УзелБазыДанных = (XmlNode)ВыбраннаяСтрока.Tag;
                // Редактирование выбранной базы данных
                СтруктураНастроекЭлемента НастройкиБазыДанных;
                                
                Boolean ЭтоПриложение = Convert.ToBoolean(ПолучитьАтрибутУзла(УзелБазыДанных, "Приложение", "false"));                

                if (!ЭтоПриложение)
                {

                    ФормаБазыДанных ФормаНастроек = new ФормаБазыДанных();
                    ФормаНастроек.ОткрытьБазуДанных(СтарыйРодитель, УзелБазыДанных, false);
                    if (ФормаНастроек.ShowDialog(this) == DialogResult.OK)
                    {
                        НастройкиБазыДанных = ФормаНастроек.ТекущаяНастройка;
                        ИзменитьНастройкуБазыДанных((XmlElement)УзелБазыДанных, НастройкиБазыДанных);

                        XmlElement РодительБазы = (XmlElement)ФормаНастроек.РодительБазыДанных;
                        if (РодительБазы != (XmlElement)СтарыйРодитель)
                        {
                            ПоменятьРодителяЭлемента(СтарыйРодитель, (XmlNode)РодительБазы, УзелБазыДанных);
                        }
                        ЗаполнениеСпискаБаз(ДеревоГруппБазДанных, СтраницаБазДанных);
                        КорректировкаГорячихКлавиш(УзелБазыДанных, УзелБазДанных);
                        НастройкиИзменены = true;
                        РегистрацияБазы77(НастройкиБазыДанных);
                        ФормаНастроек.Dispose();
                    }
                }
                else
                {
                    ФормаВнешнегоПриложения ФормаНастроек = new ФормаВнешнегоПриложения();
                    ФормаНастроек.ОткрытьПриложение(СтарыйРодитель, УзелБазыДанных, false);
                    if (ФормаНастроек.ShowDialog(this) == DialogResult.OK)
                    {
                        НастройкиБазыДанных = ФормаНастроек.ТекущаяНастройка;
                        ИзменитьНастройкуБазыДанных((XmlElement)УзелБазыДанных, НастройкиБазыДанных);

                        XmlElement РодительБазы = (XmlElement)ФормаНастроек.РодительБазыДанных;
                        if (РодительБазы != (XmlElement)СтарыйРодитель)
                        {
                            ПоменятьРодителяЭлемента(СтарыйРодитель, (XmlNode)РодительБазы, УзелБазыДанных);
                        }
                        ЗаполнениеСпискаБаз(ДеревоГруппБазДанных, СтраницаБазДанных);
                        КорректировкаГорячихКлавиш(УзелБазыДанных, УзелБазДанных);
                        НастройкиИзменены = true;
                        ФормаНастроек.Dispose();
                    }
                }                
                ЗарегистрироватьВсеГорячиеКлавиши(УзелБазДанных, 0);
            }
            ОчисткаМусора();
        }

       
        private void СтраницаБазДанных_DoubleClick(object sender, EventArgs e)
        {
            ListViewItem ВыбранныйЭлемент = СтраницаБазДанных.SelectedItems[0];
            XmlNode УзелXML = (XmlNode)ВыбранныйЭлемент.Tag;
            if (УзелXML.Name == "БазаДанных")
                РедактированиеНастройкиБазыДанных();
            else
            {
                if (String.IsNullOrEmpty(ВыбранныйЭлемент.Text))
                {
                    // Выход на уровень вверх
                    TreeNode ВерхняяГруппа = ДеревоГруппБазДанных.SelectedNode.Parent;
                    ДеревоГруппБазДанных.SelectedNode = ВерхняяГруппа;
                    ЗаполнениеСпискаБаз(ДеревоГруппБазДанных, СтраницаБазДанных);
                    foreach (ListViewItem Элемент in СтраницаБазДанных.Items)
                    {
                        if ((XmlNode)Элемент.Tag == УзелXML)
                        {
                            Элемент.Selected = true;
                            break;
                        }
                    }
                }
                else
                {
                    // Вход в папку
                    foreach (TreeNode УзелДерева in ДеревоГруппБазДанных.SelectedNode.Nodes)
                    {
                        if ((XmlNode)УзелДерева.Tag == УзелXML)
                        {
                            ДеревоГруппБазДанных.SelectedNode = УзелДерева;
                            ЗаполнениеСпискаБаз(ДеревоГруппБазДанных, СтраницаБазДанных);
                            СтраницаБазДанных.Items[0].Selected = true;
                            break;
                        }
                    }
                }
            }
        }

        private void ГлавноеОкно_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == 27)            
                if (Visible)                
                    button3_Click(sender, e);                           
        }

        private void toolStripButton4_Click(object sender, EventArgs e)
        {
            РедактированиеНастройкиБазыДанных();            
        }

        private void РедактированиеНастройкиГруппы()
        {
            if ((ДеревоГруппБазДанных.SelectedNode != null) && (ДеревоГруппБазДанных.Nodes.IndexOf(ДеревоГруппБазДанных.SelectedNode) != 0 ))
            {
                ОтменитьРегистрациюВсехГорячихКлавиш(УзелБазДанных, 0);

                XmlNode УзелГруппы = (XmlNode)ДеревоГруппБазДанных.SelectedNode.Tag;
                
                ФормаГруппы ФормаРедактированияГруппы = new ФормаГруппы();
                ФормаРедактированияГруппы.ОткрытьГруппу(УзелГруппы);
                ФормаРедактированияГруппы.ShowDialog();
                if (ФормаРедактированияГруппы.DialogResult == DialogResult.OK)
                {
                    СтруктураНастроекГруппыЭлементов НастройкиГруппы = ФормаРедактированияГруппы.ТекущаяБаза;
                    ДеревоГруппБазДанных.SelectedNode.Text = НастройкиГруппы.Наименование;
                    ИзменитьНастройкуГруппы((XmlElement)УзелГруппы, НастройкиГруппы);
                    КорректировкаГорячихКлавиш(УзелГруппы, УзелБазДанных);
                    НастройкиИзменены = true;
                }
                ФормаРедактированияГруппы.Dispose();

                ЗарегистрироватьВсеГорячиеКлавиши(УзелБазДанных, 0);
                ОчисткаМусора();
            }
        }

        private void РедактироватьГруппу_Click(object sender, EventArgs e)
        {
            РедактированиеНастройкиГруппы();            
        }

        private void УдалитьГруппуБазДанных()
        {
            if ((ДеревоГруппБазДанных.SelectedNode != null) && (ДеревоГруппБазДанных.Nodes.IndexOf(ДеревоГруппБазДанных.SelectedNode) != 0))
            {
                if (Вопрос("Удалить группу?", this) == DialogResult.Yes)                
                {
                    if (ГруппаИмеетВложенныеБазы((XmlNode)ДеревоГруппБазДанных.SelectedNode.Tag, false))
                    {
                        if (Вопрос("Группа имеет вложенные базы данных. Все равно продолжить?", this) == DialogResult.No)
                        {
                            return;
                        }
                    }
                    // Определяем родителя текущей группы
                    TreeNode РодительГруппы = ДеревоГруппБазДанных.SelectedNode.Parent;
                    ОчисткаРеестраБаз77((XmlNode)ДеревоГруппБазДанных.SelectedNode.Tag);
                    УдалитьЭлементНастройки((XmlElement)РодительГруппы.Tag, (XmlElement)ДеревоГруппБазДанных.SelectedNode.Tag);
                    ДеревоГруппБазДанных.SelectedNode.Remove();
                    НастройкиИзменены = true;
                }
            }
            ОчисткаМусора();
        }

        private void УдалитьГруппу_Click(object sender, EventArgs e)
        {
            УдалитьГруппуБазДанных();
        }

        // Процедура перемещает выбранную базу данных на одну позицию вверх
        //
        private void ПереместитьБазуВверх_Click(object sender, EventArgs e)
        {
            ListView.SelectedListViewItemCollection Выделение = СтраницаБазДанных.SelectedItems;
            if (Выделение.Count != 0)
            {

                if (Выделение.Count > 1)
                {
                    ПоказатьИнфомационноеСообщение("Перемещать базы вверх и вниз можно только по одной!", this);
                    return;
                }                

                XmlNode УзелБазыДанных = (XmlNode)Выделение[0].Tag;
                
                if (Выделение[0].Index != 0)
                {
                    if (Выделение[0].Index == 1)
                    {
                        if (String.IsNullOrEmpty(СтраницаБазДанных.Items[0].Text))
                            return;
                    }

                    int ИндексБазыВСписке = СтраницаБазДанных.Items.IndexOf(Выделение[0]);
                    if (((XmlNode)СтраницаБазДанных.Items[ИндексБазыВСписке - 1].Tag).Name != "БазаДанных")
                    {
                        if (((XmlNode)Выделение[0].Tag).Name == "БазаДанных")
                            return;
                    }
                    XmlNode РодительБазы = (XmlNode)ДеревоГруппБазДанных.SelectedNode.Tag;
                    // Перемещаем сначала узел в XML-настройках
                    ПереместитьУзелБазыДанных(УзелБазыДанных, РодительБазы, УзелБазыДанных.PreviousSibling, 1);
                    
                    ListViewItem ТекущийЭлементСписка = СтраницаБазДанных.Items[ИндексБазыВСписке];
                    ListViewItem ПредыдущийЭлементСписка = СтраницаБазДанных.Items[ИндексБазыВСписке - 1];
                    // Удаляем базы из списке
                    СтраницаБазДанных.Items.Remove(ТекущийЭлементСписка);
                    СтраницаБазДанных.Items.Remove(ПредыдущийЭлементСписка);
                    // Заново вставляем
                    СтраницаБазДанных.Items.Insert(ИндексБазыВСписке-1, ТекущийЭлементСписка);
                    СтраницаБазДанных.Items.Insert(ИндексБазыВСписке, ПредыдущийЭлементСписка);
                    НастройкиИзменены = true;
                }

                if (((XmlNode)Выделение[0].Tag).Name != "БазаДанных")
                {
                    TreeNode ВетвьДерева = ДеревоГруппБазДанных.SelectedNode;
                    ВетвьДерева.Nodes.Clear();
                    ЗаполнениеДереваГрупп(ДеревоГруппБазДанных, ((XmlNode)ВетвьДерева.Tag).FirstChild, ВетвьДерева, null);
                    ДеревоГруппПослеЗаполнения(ВетвьДерева.Nodes[0]);
                }
            }
        }

        private void ПереместитьБазуВниз_Click(object sender, EventArgs e)
        {
            ListView.SelectedListViewItemCollection Выделение = СтраницаБазДанных.SelectedItems;
            if (Выделение.Count != 0)
            {
                if (Выделение.Count > 1)
                {
                    ПоказатьИнфомационноеСообщение("Перемещать базы вверх и вниз можно только по одной!", this);
                    return;
                }

                if (String.IsNullOrEmpty(Выделение[0].Text))
                    return;
                
                XmlNode УзелБазыДанных = (XmlNode)Выделение[0].Tag;
                if (Выделение[0].Index != СтраницаБазДанных.Items.Count-1)
                {
                    // Определяем индекс базы в списке
                    int ИндексБазыВСписке = СтраницаБазДанных.Items.IndexOf(СтраницаБазДанных.SelectedItems[0]);

                    if (((XmlNode)Выделение[0].Tag).Name != "БазаДанных")
                    {
                        if (((XmlNode)СтраницаБазДанных.Items[ИндексБазыВСписке + 1].Tag).Name == "БазаДанных")
                            return;
                    }

                    XmlNode РодительБазы = (XmlNode)ДеревоГруппБазДанных.SelectedNode.Tag;
                    // Перемещаем сначала узел в XML-настройках
                    ПереместитьУзелБазыДанных(УзелБазыДанных, РодительБазы, УзелБазыДанных.NextSibling, 0);
                    ListViewItem ТекущийЭлементСписка = СтраницаБазДанных.Items[ИндексБазыВСписке];
                    ListViewItem СледующийЭлементСписка = СтраницаБазДанных.Items[ИндексБазыВСписке + 1];
                    // Удаляем базы из списке
                    СтраницаБазДанных.Items.Remove(ТекущийЭлементСписка);
                    СтраницаБазДанных.Items.Remove(СледующийЭлементСписка);
                    // Заново вставляем
                    СтраницаБазДанных.Items.Insert(ИндексБазыВСписке, СледующийЭлементСписка);
                    СтраницаБазДанных.Items.Insert(ИндексБазыВСписке + 1, ТекущийЭлементСписка);
                    НастройкиИзменены = true;
                }

                if (((XmlNode)Выделение[0].Tag).Name != "БазаДанных")
                {
                    TreeNode ВетвьДерева = ДеревоГруппБазДанных.SelectedNode;
                    ВетвьДерева.Nodes.Clear();
                    ЗаполнениеДереваГрупп(ДеревоГруппБазДанных, ((XmlNode)ВетвьДерева.Tag).FirstChild, ВетвьДерева, null);
                    ДеревоГруппПослеЗаполнения(ВетвьДерева.Nodes[0]);
                }
            }
        }

        private void СортироватьБазыПоВозрастанию_Click(object sender, EventArgs e)
        {
            Boolean ТребуетсяПерерисовкаДерева = СортироватьСписокБаз((XmlNode)ДеревоГруппБазДанных.SelectedNode.Tag, СтраницаБазДанных, SortOrder.Ascending);
            if (ТребуетсяПерерисовкаДерева)
            {
                TreeNode ВетвьДерева = ДеревоГруппБазДанных.SelectedNode;
                ВетвьДерева.Nodes.Clear();
                ЗаполнениеДереваГрупп(ДеревоГруппБазДанных, ((XmlNode)ВетвьДерева.Tag).FirstChild, ВетвьДерева, null);
                if (ВетвьДерева.Nodes.Count > 0)
                    ДеревоГруппПослеЗаполнения(ВетвьДерева.Nodes[0]);
            }
            НастройкиИзменены = true;
        }

        private void СортироватьБазыПоУбыванию_Click(object sender, EventArgs e)
        {
            Boolean ТребуетсяПерерисовкаДерева = СортироватьСписокБаз((XmlNode)ДеревоГруппБазДанных.SelectedNode.Tag, СтраницаБазДанных, SortOrder.Descending);
            
            if (ТребуетсяПерерисовкаДерева)
            {
                TreeNode ВетвьДерева = ДеревоГруппБазДанных.SelectedNode;
                ВетвьДерева.Nodes.Clear();
                ЗаполнениеДереваГрупп(ДеревоГруппБазДанных, ((XmlNode)ВетвьДерева.Tag).FirstChild, ВетвьДерева, null);
                if (ВетвьДерева.Nodes.Count > 0)
                    ДеревоГруппПослеЗаполнения(ВетвьДерева.Nodes[0]);
            }
            НастройкиИзменены = true;
        }

        private void ПунктОткрыть_Click(object sender, EventArgs e)
        {
            ЗаполнениеНастроекФормы();
            Show();
            ОчисткаМусора();
        }

        private void ДеревоГруппБазДанных_DragEnter(object sender, DragEventArgs e)
        {            
            e.Effect = DragDropEffects.Copy;
        }
                
        
        private void ДеревоГруппБазДанных_DragDrop(object sender, DragEventArgs e)
        {
            Boolean ТребуетсяПерерисовкаДерева = false;
            ListViewItem ЭлементСписка = (ListViewItem)e.Data.GetData(typeof(ListViewItem));
            TreeNode ПолучательДанных = ДеревоГруппБазДанных.GetNodeAt(ДеревоГруппБазДанных.PointToClient(new Point(e.X, e.Y)));
            XmlNode ПолучательДанныхXML = (XmlNode)ПолучательДанных.Tag;
            XmlNode АктивныйЭлемент = null;

            TreeNode УзелДляПерерисовки = ПолучательДанных;
            if (ПолучательДанных.Parent != null)
                УзелДляПерерисовки = ПолучательДанных.Parent;

            if (ЭлементСписка == null)
            {
                TreeNode ЭлементДерева = (TreeNode)e.Data.GetData(typeof(TreeNode));
                if (ЭлементДерева == ПолучательДанных)
                    return;
                // Перетаскивается группа
                // Определяем родителя
                XmlNode УзелГруппы = (XmlNode)ЭлементДерева.Tag;
                XmlNode РодительГруппы = УзелГруппы.ParentNode;
                if (ПолучательДанныхXML != РодительГруппы)
                {
                    // Перемещаем группу в дереве группу из дерева
                    ЭлементДерева.Remove();
                    ПолучательДанных.Nodes.Add(ЭлементДерева);
                    ПолучательДанных.Expand();
                    ДеревоГруппБазДанных.SelectedNode = ПолучательДанных;
                    ДеревоГруппБазДанных.Select();
                    // Меняем родителя в настройках XML
                    ПоменятьРодителяЭлемента(РодительГруппы, ПолучательДанныхXML, УзелГруппы);
                    АктивныйЭлемент = УзелГруппы;
                    ЗаполнениеСпискаБаз(ДеревоГруппБазДанных, СтраницаБазДанных);
                }
            }
            else
            {
                // Перетаскивается база                
                // Посмотрим, сколько объектов
                ListView.SelectedListViewItemCollection Выделение = СтраницаБазДанных.SelectedItems;
                // Определяем родителя                
                if (Выделение.Count == 1)
                {
                    if (String.IsNullOrEmpty(Выделение[0].Text))
                        return;
                    XmlNode УзелБазыДанных = (XmlNode)ЭлементСписка.Tag;
                    XmlNode РодительБазы = УзелБазыДанных.ParentNode;
                    if (ПолучательДанныхXML != РодительБазы)
                    {
                        if (УзелБазыДанных.Name == "Группа")
                            ТребуетсяПерерисовкаДерева = true;                        
                        // Удаляем базу текущего списка
                        СтраницаБазДанных.Items.Remove(ЭлементСписка);
                        // Меняем родителя у нее
                        ПоменятьРодителяЭлемента(РодительБазы, ПолучательДанныхXML, УзелБазыДанных);
                    }
                }
                else
                {
                    int i = 0;
                    while (i < Выделение.Count)
                    {
                        if (String.IsNullOrEmpty(Выделение[i].Text))
                        {
                            i++;
                            continue;
                        }
                        XmlNode УзелБазыДанных = (XmlNode)Выделение[i].Tag;
                        if (УзелБазыДанных.Name == "Группа")
                            ТребуетсяПерерисовкаДерева = true;
                        XmlNode РодительБазы = УзелБазыДанных.ParentNode;
                        if (ПолучательДанныхXML != РодительБазы)
                        {
                            // Удаляем базу текущего списка
                            СтраницаБазДанных.Items.Remove(Выделение[i]);
                            // Меняем родителя у нее
                            ПоменятьРодителяЭлемента(РодительБазы, ПолучательДанныхXML, УзелБазыДанных);
                        }
                        else
                            return;
                    }
                }
            }
            НастройкиИзменены = true;

            if (ТребуетсяПерерисовкаДерева)
            {
                УзелДляПерерисовки.Nodes.Clear();
                ЗаполнениеДереваГрупп(ДеревоГруппБазДанных, ((XmlNode)УзелДляПерерисовки.Tag).FirstChild, УзелДляПерерисовки, АктивныйЭлемент);
                if (УзелДляПерерисовки.Nodes.Count > 0)
                    ДеревоГруппПослеЗаполнения(УзелДляПерерисовки.Nodes[0]);
            }
        }

        private void ДеревоГруппБазДанных_ItemDrag(object sender, ItemDragEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                if (((TreeNode)e.Item).Parent != null)
                {
                    ДеревоГруппБазДанных.DoDragDrop(e.Item, DragDropEffects.All);
                }
            }
        }

        private void СтраницаБазДанных_ItemDrag(object sender, ItemDragEventArgs e)
        {
            // Начало перетаскивания елемента из списка                        
            if (e.Button == MouseButtons.Left)
                СтраницаБазДанных.DoDragDrop(e.Item, DragDropEffects.All);
        }

        private void ОткрытьПодбор()
        {
            XmlNode ТекущаяГруппа;
            if (ДеревоГруппБазДанных.SelectedNode == null)
                ТекущаяГруппа = УзелБазДанных;
            else
                ТекущаяГруппа = (XmlNode)ДеревоГруппБазДанных.SelectedNode.Tag;

            ФормаПодбораБазДанных ФормаПодбора = new ФормаПодбораБазДанных();
            ФормаПодбора.ИнициализацияПодбора(ТекущаяГруппа);
            if (ФормаПодбора.ShowDialog() == DialogResult.OK)
            {
                ListView.ListViewItemCollection СписокДобавляемыхБаз = ФормаПодбора.СписокДобавляемыхБаз.Items;
                if (СписокДобавляемыхБаз.Count != 0)
                {
                    for (int i = 0; i < СписокДобавляемыхБаз.Count; i++)
                    {
                        СтруктураНастроекЭлемента НастройкаДобавляемойБазы = (СтруктураНастроекЭлемента)СписокДобавляемыхБаз[i].Tag;
                        ДобавитьБазуДанныхВНастройки(НастройкаДобавляемойБазы, НастройкаДобавляемойБазы.Группа);
                    }
                    if (ДеревоГруппБазДанных.SelectedNode != null)
                    {
                        ЗаполнениеСпискаБаз(ДеревоГруппБазДанных, СтраницаБазДанных);
                    }
                    НастройкиИзменены = true;
                }
            }
            ФормаПодбора.Dispose();
            ОчисткаМусора();
        }

        private void ПодборБазДанных_Click(object sender, EventArgs e)
        {
            ОткрытьПодбор();
        }

        private void СтраницаБазДанных_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyValue == 46)
                УдалитьБазуДанных();
            else if (e.KeyValue == 45)
                СозданиеНовойБазыДанных();
            else if (e.KeyValue == 120)
                КопироватьЭлемент();
            else if ((e.KeyValue == 113) | (e.KeyValue == 13))
                РедактированиеНастройкиБазыДанных();
            else if (e.Shift & e.Control & (e.KeyValue == 77))
            {
                if (СтраницаБазДанных.SelectedItems.Count != 0)
                {
                    СтруктураНастроекЭлемента НастройкиБазы = ПолучитьНастройкиБазыДанных((XmlNode)СтраницаБазДанных.SelectedItems[0].Tag);
                    if (БазаДанныхСуществуетПоУказанномуПути(НастройкиБазы))
                        ПоказатьМониторПользователей(НастройкиБазы);
                }
            }
        }

        private void ДеревоГруппБазДанных_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyValue == 46)
                УдалитьГруппуБазДанных();
            else if (e.KeyValue == 45)
                ДобавитьГруппуБазДанных();
            else if (e.KeyValue == 113)
                РедактированиеНастройкиГруппы();
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
            if (!ПанельАнимации.Visible)
                e.Graphics.DrawImage(Properties.Resources.smallinfo, 1, 1);
            else
                e.Graphics.DrawImage(Properties.Resources.SmallSearch, 1, 1);
        }
            
        // Процедура автоматичести определяет путь запуска 77
        //
        private void КнопкаНайтиПуть77_Click(object sender, EventArgs e)
        {          
            string СтарыйПутьЗапуска = ПутьЗапуска77.Text;

            string path77 = installer1C.GetStarter(77);

            if (String.IsNullOrEmpty(path77))
                ПутьЗапуска77.Text = path77;

            if (!ПутьЗапуска77.Text.Equals(СтарыйПутьЗапуска))                
                НастройкиИзменены = true;
        }

        private string ПолучитьПутьЗапуска8(int platformType)
        {            
            return installer1C.GetStarter(platformType);            
        }

        private void КнопкаНайтиПуть80_Click(object sender, EventArgs e)
        {
            string Путь80 = ПолучитьПутьЗапуска8(80);
            if (!String.IsNullOrEmpty(Путь80))
            {
                string СтарыйПутьЗапуска = ПутьЗапуска80.Text;
                if (!СтарыйПутьЗапуска.Equals(Путь80))
                {
                    НастройкиИзменены = true;
                    ПутьЗапуска80.Text = Путь80;
                }
            }
        }

        private void КнопкаНайтиПуть81_Click(object sender, EventArgs e)
        {
            string Путь81 = ПолучитьПутьЗапуска8(81);
            if (!String.IsNullOrEmpty(Путь81))
            {
                string СтарыйПутьЗапуска = ПутьЗапуска81.Text;
                if (!СтарыйПутьЗапуска.Equals(Путь81))
                {
                    НастройкиИзменены = true;
                    ПутьЗапуска81.Text = Путь81;
                }
            }
        }

        private void КнопкаНайтиПуть82_Click(object sender, EventArgs e)
        {
            string Путь82 = ПолучитьПутьЗапуска8(82);
            if (!String.IsNullOrEmpty(Путь82))
            {
                string СтарыйПутьЗапуска = ПутьЗапуска82.Text;
                if (!СтарыйПутьЗапуска.Equals(Путь82))
                {
                    НастройкиИзменены = true;
                    ПутьЗапуска82.Text = Путь82;
                }
            }
        }

        private void КнопкаНайтиПуть83_Click(object sender, EventArgs e)
        {
            string Путь83 = ПолучитьПутьЗапуска8(83);
            if (!String.IsNullOrEmpty(Путь83))
            {
                string СтарыйПутьЗапуска = ПутьЗапуска83.Text;
                if (!СтарыйПутьЗапуска.Equals(Путь83))
                {
                    НастройкиИзменены = true;
                    ПутьЗапуска83.Text = Путь83;
                }
            }
        }
      
        private void ГлавноеОкно_FormClosed(object sender, FormClosedEventArgs e)
        {
            ПриЗакрытии();
            //UnHook();            
        }

        private Boolean ИмеютсяВложенныеБазы(DirectoryInfo Каталог)
        {
            Application.DoEvents();

            Boolean БазыНайдены = false;

            try
            {
                FileInfo[] СписокФайлов8x = Каталог.GetFiles("*.1CD");
                if (СписокФайлов8x.Length != 0)
                    return true;

                FileInfo[] СписокФайлов77 = Каталог.GetFiles("*.md");
                if (СписокФайлов77.Length != 0)
                    return true;
            }
            catch
            {
                return false;
            }

            DirectoryInfo[] СписокПодкаталогов;

            try
            {
                СписокПодкаталогов = Каталог.GetDirectories();
            }
            catch
            {
                return false;
            }

            foreach (DirectoryInfo Подкаталог in СписокПодкаталогов)
            {
                БазыНайдены = ИмеютсяВложенныеБазы(Подкаталог);
                if (БазыНайдены)
                    return true;
            }                       

            if (БазыНайдены)
                return true;
            else
                return false;
        }

        private void РекурсивнаяОбработкаКаталогов(DirectoryInfo Каталог, XmlNode Группа, string ИмяПользователяПоУмолчанию, string ПарольПользователяПоУмолчанию, int ТипПлатформы8х, ref int КоличествоНайденныхБаз)
        {
            Application.DoEvents();

            string ИмяКаталога = Каталог.Name;
            string ПолноеИмяКаталога = Каталог.FullName;

            ПоказатьИнформацию(ПолноеИмяКаталога);

            if (ИмяКаталога.Equals("NEW_STRU"))
                return;

            DirectoryInfo[] СписокПодкаталогов;

            try
            {
                СписокПодкаталогов = Каталог.GetDirectories();
            }
            catch
            {
                return;
            }

            Boolean ЕстьВложенныеБазы = ИмеютсяВложенныеБазы(Каталог);
            
            if (ЕстьВложенныеБазы)
            {
                СтруктураНастроекГруппыЭлементов НоваяНастройка = new СтруктураНастроекГруппыЭлементов();
                НоваяНастройка.ГруппаАктивна = false;
                НоваяНастройка.ГруппаРаскрыта = true;
                НоваяНастройка.Наименование = ИмяКаталога;                

                XmlNode НоваяГруппа = (XmlNode)ДобавитьГруппуБазВНастройки(НоваяНастройка, Группа);

                foreach (DirectoryInfo Подкаталог in СписокПодкаталогов)
                {
                    РекурсивнаяОбработкаКаталогов(Подкаталог, НоваяГруппа, ИмяПользователяПоУмолчанию, ПарольПользователяПоУмолчанию, ТипПлатформы8х, ref КоличествоНайденныхБаз);
                }
                {
                    FileInfo[] СписокФайлов8x = Каталог.GetFiles("*.1CD");
                    if (СписокФайлов8x.Length != 0)
                    {
                        СтруктураНастроекЭлемента НоваяНастройкаБазы = new СтруктураНастроекЭлемента();
                        НоваяНастройкаБазы.Наименование = ИмяКаталога;
                        НоваяНастройкаБазы.ИмяПользователя = ИмяПользователяПоУмолчанию;
                        НоваяНастройкаБазы.Пароль = ПарольПользователяПоУмолчанию;
                        НоваяНастройкаБазы.Путь = ПолноеИмяКаталога;
                        НоваяНастройкаБазы.РежимРаботы = 0;
                        НоваяНастройкаБазы.РежимЗапуска = 0;
                        НоваяНастройкаБазы.ТипБазы = 0;
                        НоваяНастройкаБазы.ВидКлиента = 0;
                        НоваяНастройкаБазы.ВидКлиентаКакПунктМеню = false;
                        НоваяНастройкаБазы.ТипПлатформы = ТипПлатформы8х;
                        НоваяНастройкаБазы.ПоказыватьВМенюЗапуска = true;
                        НоваяНастройкаБазы.ДополнительныеПользователи = new ListView();
                        ДобавитьБазуДанныхВНастройки(НоваяНастройкаБазы, НоваяГруппа);
                        КоличествоНайденныхБаз++;
                    }


                    FileInfo[] СписокФайлов77 = Каталог.GetFiles("*.md");
                    if (СписокФайлов77.Length != 0)
                    {
                        СтруктураНастроекЭлемента НоваяНастройкаБазы = new СтруктураНастроекЭлемента();
                        НоваяНастройкаБазы.Наименование = ИмяКаталога;
                        НоваяНастройкаБазы.ИмяПользователя = ИмяПользователяПоУмолчанию;
                        НоваяНастройкаБазы.Пароль = ПарольПользователяПоУмолчанию;
                        НоваяНастройкаБазы.Путь = ПолноеИмяКаталога;
                        НоваяНастройкаБазы.РежимРаботы = 0;
                        НоваяНастройкаБазы.РежимЗапуска = 0;
                        НоваяНастройкаБазы.ТипБазы = 0;
                        НоваяНастройкаБазы.ТипПлатформы = 0;
                        НоваяНастройкаБазы.ВидКлиента = 0;
                        НоваяНастройкаБазы.ВидКлиентаКакПунктМеню = false;
                        НоваяНастройкаБазы.ПоказыватьВМенюЗапуска = true;
                        НоваяНастройкаБазы.ДополнительныеПользователи = new ListView();
                        ДобавитьБазуДанныхВНастройки(НоваяНастройкаБазы, НоваяГруппа);
                        КоличествоНайденныхБаз++;
                    }                    
                }
            }
        }

        private void ОткрытьПоискБазДанных()
        {
            XmlNode ГруппаПоиска;

            if (ДеревоГруппБазДанных.SelectedNode != null)
                ГруппаПоиска = (XmlNode)ДеревоГруппБазДанных.SelectedNode.Tag;
            else
                ГруппаПоиска = УзелБазДанных;

            ФормаПоискаБаз ФормаПоиска = new ФормаПоискаБаз();
            ФормаПоиска.Инициализация(ГруппаПоиска);

            ListBox СтарыеБазы = new ListBox();
            ПолучитьСписокБаз(ref СтарыеБазы, УзелБазДанных);

            if (ФормаПоиска.ShowDialog() == DialogResult.OK)
            {
                int КоличествоНайденныхБаз = 0;

                СброситьПрогрессАнимации();
                ПанельАнимации.Visible = true;
                ТаймерАнимации.Enabled = true;
                УстановитьПоложениеПанелиАнимации();
                ПанельПриложения.Enabled = false;

                string НачальныйКаталог = ФормаПоиска.НачальныйКаталогПоиска;
                ГруппаПоиска = ФормаПоиска.ГруппаРодительДляНовыхБаз;
                DirectoryInfo НачКаталог = new DirectoryInfo(НачальныйКаталог);

                string ИмяПользователяПоУмолчанию = ПолучитьЗначениеНастройки("ИмяПользователяПоУмолчанию", "");
                string ПарольПользователяПоУмолчанию = ПолучитьЗначениеНастройки("ПарольПользователяПоУмолчанию", "");

                РекурсивнаяОбработкаКаталогов(НачКаталог, ГруппаПоиска, ИмяПользователяПоУмолчанию, ПарольПользователяПоУмолчанию, ФормаПоиска.ТипПлатформы8х + 1, ref КоличествоНайденныхБаз);

                ListBox СписокОбработанныхУзлов = new ListBox();

                ПоказатьИнформацию("Обработка результатов поиска...");
                if (ГруппаПоиска.HasChildNodes)
                    РекурсивнаяОбработкаПослеПоиска(ГруппаПоиска.FirstChild, ГруппаПоиска, null, СписокОбработанныхУзлов, СтарыеБазы);

                ПоказатьИнформацию("Заполнение настроек программы...");
                РекурсивноеУдалениеПустыхПапок(ГруппаПоиска);

                ДеревоГруппБазДанных.Nodes.Clear();
                ЗаполнениеДереваГрупп(ДеревоГруппБазДанных, УзелБазДанных, null, null);
                ДеревоГруппПослеЗаполнения(ДеревоГруппБазДанных.Nodes[0]);
                ДеревоГруппБазДанных.SelectedNode.Expand();

                ПанельПриложения.Enabled = true;
                ПанельАнимации.Visible = false;
                ТаймерАнимации.Enabled = false;

                ОчиститьИнформацию();

                ПоказатьИнфомационноеСообщение("Поиск завершен. Найдено баз данных: " + Convert.ToString(КоличествоНайденныхБаз), this);

                if (КоличествоНайденныхБаз > 0)
                    НастройкиИзменены = true;
            }
            ФормаПоиска.Dispose();
            ОчисткаМусора();
        }       

        private void ПоискБазДанных_Click(object sender, EventArgs e)
        {
            ОткрытьПоискБазДанных();
        }

        private void СброситьПрогрессАнимации()
        {
            Image1.Image = (Image)Properties.Resources.Brick2;
            Image2.Image = (Image)Properties.Resources.Brick2;
            Image3.Image = (Image)Properties.Resources.Brick2;
            Image4.Image = (Image)Properties.Resources.Brick2;
            Image5.Image = (Image)Properties.Resources.Brick2;
        }

        private void ТаймерАнимации_Tick(object sender, EventArgs e)
        {
            СброситьПрогрессАнимации();
           
            if (ТекущаяКартинкаАнимации == 5)
            {
                ТекущаяКартинкаАнимации = 0;
            }

            if (ТекущаяКартинкаАнимации == 0)
                Image1.Image = (Image)Properties.Resources.Brick;
            else if (ТекущаяКартинкаАнимации == 1)
                Image2.Image = (Image)Properties.Resources.Brick;
            else if (ТекущаяКартинкаАнимации == 2)
                Image3.Image = (Image)Properties.Resources.Brick;
            else if (ТекущаяКартинкаАнимации == 3)
                Image4.Image = (Image)Properties.Resources.Brick;
            else if (ТекущаяКартинкаАнимации == 4)
                Image5.Image = (Image)Properties.Resources.Brick;

            ТекущаяКартинкаАнимации++;
        }

        private void УстановитьПоложениеПанелиАнимации()
        {
            if (ПанельАнимации.Visible)
            {
                ПанельАнимации.Left = Width / 2 - ПанельАнимации.Width / 2;
                ПанельАнимации.Top = Height / 2 - ПанельАнимации.Height;
            }
        }

        private void ГлавноеОкно_Resize(object sender, EventArgs e)
        {
            УстановитьРазмерДереваГрупп();
            УстановитьПоложениеПанелиАнимации();            
        }
        

        private void СохранятьПоложениеОкна_MouseHover(object sender, EventArgs e)
        {
            ПоказатьИнформацию("При установке флажка будут сохраняться размеры и положение главного окна приложения");
        }

        private void удалитьToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            УдалитьГруппуБазДанных();
        }

        private void изменитьToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            РедактированиеНастройкиГруппы();
        }

        private void добавитьToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            ДобавитьГруппуБазДанных();
        }
     
        private void ГлавноеОкно_Load(object sender, EventArgs e)
        {
            УстановитьПараметрыОкна();
        }

        private void ОпределитьКоэффициентДереваГрупп()
        {
            Коэффициент = Convert.ToDouble(Width) / Convert.ToDouble(ДеревоГруппБазДанных.Width);
        }

        private void ГлавноеОкно_ResizeBegin(object sender, EventArgs e)
        {
            // Запомним соотношение дерева и списка
            ОпределитьКоэффициентДереваГрупп();
        }

        private void УстановитьРазмерДереваГрупп()
        {
            ДеревоГруппБазДанных.Width = Convert.ToInt32(Width / Коэффициент);
        }

        private void ГлавноеОкно_MaximumSizeChanged(object sender, EventArgs e)
        {
            УстановитьРазмерДереваГрупп();
        }

        private void Разделитель_SplitterMoved(object sender, SplitterEventArgs e)
        {
            ОпределитьКоэффициентДереваГрупп();
        }

        private void добавитьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ДобавитьБазуДанных_Click(sender, e);
        }

        private void изменитьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            РедактированиеНастройкиБазыДанных();
        }

        private void удалитьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            УдалитьБазуДанных();
        }

        private void ДобавитьИсключение_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFile = new OpenFileDialog();
            openFile.Filter = "Исполняемые файлы приложений|*.exe";            
            if (openFile.ShowDialog() == DialogResult.OK)
            {
                string ИмяИсполняемогоФайла = openFile.FileName;
                string[] МассивПути = ИмяИсполняемогоФайла.Split('\\');

                ListViewItem НовоеИсключение = СписокИсключений.Items.Add(Convert.ToString(СписокИсключений.Items.Count + 1));
                НовоеИсключение.SubItems.Add(МассивПути[МассивПути.Length - 1]);
                XmlNode УзелИсключения = ДобавитьИсключениеВНастройки(ИмяИсполняемогоФайла);
                НовоеИсключение.Tag = УзелИсключения;
                НастройкиИзменены = true;
            }
        }

        private void УдалитьИсключение_Click(object sender, EventArgs e)
        {
            if (СписокИсключений.SelectedItems.Count != 0)
            {
                if (ГлавноеОкно.Вопрос("Удалить выбранное исключение", this) == DialogResult.Yes)
                {
                    ListViewItem ВыбраннаяСтрока = СписокИсключений.SelectedItems[0];
                    XmlNode УзелИсключения = (XmlNode)ВыбраннаяСтрока.Tag;
                    УдалитьЭлементНастройки((XmlElement)УзелИсключений, (XmlElement)УзелИсключения);
                    ВыбраннаяСтрока.Remove();
                    for (int i = 0; i < СписокИсключений.Items.Count; i++)
                    {
                        ListViewItem ЭлементИсключений = СписокИсключений.Items[i];
                        ЭлементИсключений.Text = Convert.ToString(i + 1);
                    }
                    НастройкиИзменены = true;
                }
            }
        }

        private void ГлавноеОкно_VisibleChanged(object sender, EventArgs e)
        {
            if (Visible)
            {
                ЗагрузитьНастройкиПрограммы(ДеревоГруппБазДанных, СписокИсключений);                
            }
        }

        private void ПоказатьМенюИзФормы()
        {
            SetForegroundWindow((IntPtr)Handle);
            if (НеобходимоПерерисоватьМеню)
            {
                ИнициализацияМенюЗапуска(МенюЗапуска);
                НеобходимоПерерисоватьМеню = false;
            }
            МенюЗапуска.Show(MousePosition.X, MousePosition.Y);
        }

        private void ГлавноеОкно_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Middle)
            {
                ПоказатьМенюИзФормы();
            }
        }
        
        private void RefreshInstaller1C()
        {
        	if (installer1C == null)
        		installer1C = new installer1CInfo();
        	else
        		installer1C.Refresh();
        }

        private void ГлавноеОкно_Shown(object sender, EventArgs e)
        {
            RefreshInstaller1C();
        	Hide();
            Opacity = 1;
            ОчисткаМусора();
        }

        private void сочетанияКлавишToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ВывестиСочетанияКлавиш();
        }

        private void оПрограммеToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ФормаОПрограмме ОПрограмме = new ФормаОПрограмме();
            ОПрограмме.ShowDialog();
            ОПрограмме.Dispose();
            ОчисткаМусора();
        }

        private void ГлавноеОкно_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (Visible)
            {
                e.Cancel = true;
                ПриЗакрытии();
            }
            else
                ПриЗакрытииПриложения();

        }

        private void ПриЗакрытииПриложения()
        {
            if (File.Exists(ВремФайлДесктопа))
            {
                try
                {
                    РабочийСтол.Image.Dispose();
                    File.Delete(ВремФайлДесктопа);
                }
                catch
                {
                }

                iBasesEdit.DeleteTempBasesFromFile();
            }
        }

        static private void ПолучитьСписокБаз(ref ListBox СтарыеБазы, XmlNode УзелXML)
        {
            if (УзелXML == null)
                return;
            if (УзелXML.Name == "БазаДанных")
                СтарыеБазы.Items.Add(УзелXML);
            if (УзелXML.HasChildNodes)
                ПолучитьСписокБаз(ref СтарыеБазы, УзелXML.FirstChild);
            if (УзелXML.NextSibling != null)
                ПолучитьСписокБаз(ref СтарыеБазы, УзелXML.NextSibling);
        }

        private void КонтекстноеМенюСписка_Opening(object sender, CancelEventArgs e)
        {
            XmlNode УзелXML = null;

            Boolean ПоказатьМенюБаз = false;
            Boolean ПоказатьМенюГрупп = false;

            if ((СтраницаБазДанных.SelectedItems.Count == 0))
                ПоказатьМенюБаз = true;
            if (!ПоказатьМенюБаз)
            {
                ListViewItem ВыбранныйЭлемент = СтраницаБазДанных.SelectedItems[0];
                УзелXML = (XmlNode)ВыбранныйЭлемент.Tag;
                if (УзелXML.Name == "БазаДанных")
                    ПоказатьМенюБаз = true;
                else if (!String.IsNullOrEmpty(ВыбранныйЭлемент.Text))
                    ПоказатьМенюГрупп = true;
            }

            if (ПоказатьМенюБаз)
            {
                while (КонтекстноеМенюСписка.Items.Count > 3)
                {
                    КонтекстноеМенюСписка.Items.Remove(КонтекстноеМенюСписка.Items[КонтекстноеМенюСписка.Items.Count - 1]);
                }

                if (СтраницаБазДанных.SelectedItems.Count == 1)
                {
                    ToolStripItem ЭлементКонтекстногоМеню;
                    ЭлементКонтекстногоМеню = КонтекстноеМенюСписка.Items.Add("-");
                    СтруктураНастроекЭлемента НастройкиБазы = ПолучитьНастройкиБазыДанных(УзелXML);

                    if (!НастройкиБазы.Приложение)
                    {

                        Boolean ПоказыватьМонитор = false;
                        if (НастройкиБазы.ТипПлатформы == 0)
                        {
                            if (БазаДанныхСуществуетПоУказанномуПути(НастройкиБазы))
                                ПоказыватьМонитор = true;
                        }
                        else if ((НастройкиБазы.ТипБазы == 1) && (НастройкиБазы.ТипПлатформы != 1))
                            ПоказыватьМонитор = true;

                        if (ПоказыватьМонитор)
                        {
                            ЭлементКонтекстногоМеню = КонтекстноеМенюСписка.Items.Add("Монитор пользователей");
                            ЭлементКонтекстногоМеню.Tag = НастройкиБазы;
                            ЭлементКонтекстногоМеню.Image = Properties.Resources.МониторПользователей;
                            ЭлементКонтекстногоМеню.Click += new EventHandler(МониторПользователей_Click);
                            ЭлементКонтекстногоМеню.MouseHover += new EventHandler(Default_MouseHover);
                            ЭлементКонтекстногоМеню.MouseLeave += new EventHandler(Default_MouseLeave);
                            КонтекстноеМенюСписка.Items.Add("-");
                        }

                        if (НастройкиБазы.ТипПлатформы == 0)
                        {
                            // 1С Предприятие 7.7
                            ЭлементКонтекстногоМеню = КонтекстноеМенюСписка.Items.Add("1С Предприятие");
                            НастройкиБазы.РежимЗапуска = 1;
                            ЭлементКонтекстногоМеню.Image = ПолучитьИконкуРежима(0, 1);
                            // Создание подменю с выбором режима работы
                            ToolStripMenuItem Подменю1СПредприятия = (ToolStripMenuItem)ЭлементКонтекстногоМеню;

                            // Монопольный
                            ЭлементКонтекстногоМеню = Подменю1СПредприятия.DropDownItems.Add("Монопольный");
                            НастройкиБазы.РежимРаботы = 1;
                            ЭлементКонтекстногоМеню.Tag = НастройкиБазы;
                            ЭлементКонтекстногоМеню.Image = Properties.Resources.Монопольный;
                            ЭлементКонтекстногоМеню.Click += new EventHandler(ПунктРежимаРаботы_Click);
                            // Разделенный
                            ЭлементКонтекстногоМеню = Подменю1СПредприятия.DropDownItems.Add("Разделенный");
                            НастройкиБазы.РежимРаботы = 2;
                            ЭлементКонтекстногоМеню.Tag = НастройкиБазы;
                            ЭлементКонтекстногоМеню.Image = Properties.Resources.Разделенный;
                            ЭлементКонтекстногоМеню.Click += new EventHandler(ПунктРежимаРаботы_Click);

                            // Конфигуратор 7.7
                            ЭлементКонтекстногоМеню = КонтекстноеМенюСписка.Items.Add("Кофигуратор");
                            НастройкиБазы.РежимЗапуска = 2;
                            ЭлементКонтекстногоМеню.Tag = НастройкиБазы;
                            ЭлементКонтекстногоМеню.Image = ПолучитьИконкуРежима(0, 2);
                            ЭлементКонтекстногоМеню.Click += new EventHandler(ПунктРежимаЗапуска_Click);
                            // Отладчик 7.7
                            ЭлементКонтекстногоМеню = КонтекстноеМенюСписка.Items.Add("Отладчик");
                            НастройкиБазы.РежимЗапуска = 3;
                            ЭлементКонтекстногоМеню.Tag = НастройкиБазы;
                            ЭлементКонтекстногоМеню.Image = ПолучитьИконкуРежима(0, 3);
                            ЭлементКонтекстногоМеню.Click += new EventHandler(ПунктРежимаЗапуска_Click);
                            // Монитор 7.7
                            ЭлементКонтекстногоМеню = КонтекстноеМенюСписка.Items.Add("Монитор");
                            НастройкиБазы.РежимЗапуска = 4;
                            ЭлементКонтекстногоМеню.Tag = НастройкиБазы;
                            ЭлементКонтекстногоМеню.Image = ПолучитьИконкуРежима(0, 4);
                            ЭлементКонтекстногоМеню.Click += new EventHandler(ПунктРежимаЗапуска_Click);
                        }
                        else
                        {
                            // 1С Предприятие 8.х
                            ЭлементКонтекстногоМеню = КонтекстноеМенюСписка.Items.Add("1С Предприятие");
                            НастройкиБазы.РежимЗапуска = 1;
                            ЭлементКонтекстногоМеню.Tag = НастройкиБазы;
                            ЭлементКонтекстногоМеню.Image = ПолучитьИконкуРежима(НастройкиБазы.ТипПлатформы, 1);
                            ЭлементКонтекстногоМеню.Click += new EventHandler(ПунктРежимаЗапуска_Click);
                            // Кофигуратор 8.х
                            ЭлементКонтекстногоМеню = КонтекстноеМенюСписка.Items.Add("Конфигуратор");
                            НастройкиБазы.РежимЗапуска = 2;
                            ЭлементКонтекстногоМеню.Tag = НастройкиБазы;
                            ЭлементКонтекстногоМеню.Image = ПолучитьИконкуРежима(НастройкиБазы.ТипПлатформы, 2);
                            ЭлементКонтекстногоМеню.Click += new EventHandler(ПунктРежимаЗапуска_Click);
                        }
                    }
                    else
                    {
                        ЭлементКонтекстногоМеню = КонтекстноеМенюСписка.Items.Add("Запустить приложение");
                        НастройкиБазы.РежимРаботы = 1;
                        ЭлементКонтекстногоМеню.Tag = НастройкиБазы;
                        ЭлементКонтекстногоМеню.Image = ПолучитьИконкуВнешнегоПриложения(НастройкиБазы);
                        ЭлементКонтекстногоМеню.Click += new EventHandler(ПунктРежимаРаботы_Click);
                        
                        GC.Collect();
                        GC.WaitForPendingFinalizers();
                    }
                }
            }
            else
            {
                e.Cancel = true;
                if (ПоказатьМенюГрупп)
                    ДопМенюГрупп.Show(MousePosition.X, MousePosition.Y);
            }
        }

        private void МенюЗапуска_Closed(object sender, ToolStripDropDownClosedEventArgs e)
        {
            ОчисткаМусора();
        }

        private void ПутьЗапуска77_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyValue == 115)
            {
                УстановитьЗначениеНастройки("ПутьЗапуска77", ВыборПутиЗапускаПлатформы("Укажите файл запуска 1С Предприятия 7.7", "Файл запуска 1С Предприятия|1cv7*.exe", ref ПутьЗапуска77));               
            }
        }

        private void ПутьЗапуска80_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyValue == 115)
            {
                УстановитьЗначениеНастройки("ПутьЗапуска80", ВыборПутиЗапускаПлатформы("Укажите файл запуска 1С Предприятия 8.0", "Файл запуска 1С Предприятия|1cv8.exe", ref ПутьЗапуска80));
            }
        }

        private void ПутьЗапуска81_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyValue == 115)
            {
                УстановитьЗначениеНастройки("ПутьЗапуска81", ВыборПутиЗапускаПлатформы("Укажите файл запуска 1С Предприятия 8.1", "Файл запуска 1С Предприятия|1cv8.exe", ref ПутьЗапуска81));
            }
        }

        private void ПутьЗапуска82_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyValue == 115)
            {
                УстановитьЗначениеНастройки("ПутьЗапуска82", ВыборПутиЗапускаПлатформы("Укажите файл запуска 1С Предприятия 8.2", "Файл запуска 1С Предприятия|1cv8.exe;1cestart.exe", ref ПутьЗапуска82));
            }
        }
        
        private void ПутьЗапуска83_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyValue == 115)
            {
                УстановитьЗначениеНастройки("ПутьЗапуска83", ВыборПутиЗапускаПлатформы("Укажите файл запуска 1С Предприятия 8.3", "Файл запуска 1С Предприятия|1cv8.exe;1cestart.exe", ref ПутьЗапуска83));
            }
        }


        private void УправлениеРежимомЗапуска()
        {
            if ((ТипПлатформыПоУмолчанию.SelectedIndex == 0) && (РежимЗапускаПоУмолчанию.Items.Count == 3))
            {
                РежимЗапускаПоУмолчанию.Items.Add("Отладчик");
                РежимЗапускаПоУмолчанию.Items.Add("Монитор");
            }
            else if (ТипПлатформыПоУмолчанию.SelectedIndex != 0)
            {
                if (РежимЗапускаПоУмолчанию.Items.Count > 3)
                {
                    РежимЗапускаПоУмолчанию.Items.Remove(РежимЗапускаПоУмолчанию.Items[4]);
                    РежимЗапускаПоУмолчанию.Items.Remove(РежимЗапускаПоУмолчанию.Items[3]);
                }
            }
            if (РежимЗапускаПоУмолчанию.SelectedItem == null)
                РежимЗапускаПоУмолчанию.SelectedIndex = 0;
        }

        private void РежимЗапускаПоУмолчанию_SelectedIndexChanged(object sender, EventArgs e)
        {
            УстановитьЗначениеНастройки("РежимЗапускаПоУмолчанию", Convert.ToString(РежимЗапускаПоУмолчанию.SelectedIndex));
            НастройкиИзменены = true;
        }

        private void ВыбратьПапкуБазДанных_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog openFolder = new FolderBrowserDialog();
            openFolder.ShowNewFolderButton = true;

            if (!String.IsNullOrEmpty(ПапкаСБазамиПоУмолчанию.Text))
                openFolder.SelectedPath = ПапкаСБазамиПоУмолчанию.Text;
            if (openFolder.ShowDialog() == DialogResult.OK)
            {
                string ПутьБаз = openFolder.SelectedPath;
                ПапкаСБазамиПоУмолчанию.Text = ПутьБаз;                
                НастройкиИзменены = true;
            }
            openFolder.Dispose();
        }

        private void ГлавноеОкно_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyValue == 13)
            {
                if ((e.Modifiers & Keys.Control) != 0)
                    button5_Click(sender, e);
            }
            else if (e.KeyValue == 118)
                ОткрытьПоискБазДанных();
            else if (e.KeyValue == 117)
                ОткрытьПодбор();
        }
               
        private void МониторНаПанели_Click(object sender, EventArgs e)
        {
            СтруктураНастроекЭлемента НастройкиБазы = ПолучитьНастройкиБазыДанных((XmlNode)СтраницаБазДанных.SelectedItems[0].Tag);
            ПоказатьМониторПользователей(НастройкиБазы);
        }

        private void УправлениеКнопкойМонитора()
        {
            Boolean НужнаКнопка = false;
            if (СтраницаБазДанных.SelectedItems.Count == 1)
            {
                try
                {
                    СтруктураНастроекЭлемента НастройкиБазы = ПолучитьНастройкиБазыДанных((XmlNode)СтраницаБазДанных.SelectedItems[0].Tag);

                    if (!НастройкиБазы.Приложение)
                    {
                        НужнаКнопка = true;                        
                    }
                }
                catch
                {
                }
            }
            if (!НужнаКнопка)
            {
                ToolStripItem[] ЭлементыПанели = ПанельИнструментов.Items.Find("КнопкаМонитора", false);
                if (ЭлементыПанели.Length > 0)
                    ПанельИнструментов.Items.Remove(ЭлементыПанели[0]);
            }
            else
            {
                if (ПанельИнструментов.Items.Find("КнопкаМонитора", false).Length == 0)
                {
                    ToolStripButton КнопкаМонитора = new ToolStripButton("", Properties.Resources.Монитор32, new EventHandler(Default_MouseLeave));
                    КнопкаМонитора.Name = "КнопкаМонитора";
                    КнопкаМонитора.Click += new EventHandler(МониторНаПанели_Click);
                    КнопкаМонитора.MouseHover += new EventHandler(Default_MouseHover);
                    КнопкаМонитора.MouseLeave += new EventHandler(Default_MouseLeave);
                    ПанельИнструментов.Items.Add(КнопкаМонитора);
                }
            }
        }
               
        private void Исключения_MouseLeave(object sender, EventArgs e)
        {
            ОчиститьИнформацию();
        }
        
        private void ДопИзменитьГруппу_Click(object sender, EventArgs e)
        {
            РедактированиеГруппыВСписке(СтраницаБазДанных.SelectedItems[0]);
        }

        private void ДопУдалитьГруппу_Click(object sender, EventArgs e)
        {
            if (Вопрос("Удалить группу?", this) == DialogResult.Yes)
            {
                ListViewItem ВыбраннаяСтрока = СтраницаБазДанных.SelectedItems[0];
                XmlNode XMLУзел = (XmlNode)ВыбраннаяСтрока.Tag;

                if (ГруппаИмеетВложенныеБазы(XMLУзел, false))
                {
                    if (Вопрос("Группа имеет вложенные базы данных. Все равно продолжить?", this) == DialogResult.No)
                    {
                        return;
                    }
                }
                // Определяем родителя текущей группы
                TreeNode РодительГруппы = ДеревоГруппБазДанных.SelectedNode;
                СтраницаБазДанных.Items.Remove(ВыбраннаяСтрока);
                foreach (TreeNode ТекУзелДерева in ДеревоГруппБазДанных.SelectedNode.Nodes)
                {
                    if ((XmlNode)ТекУзелДерева.Tag == XMLУзел)
                    {
                        ТекУзелДерева.Remove();
                        break;
                    }
                }
                ОчисткаРеестраБаз77(XMLУзел);
                УдалитьЭлементНастройки((XmlElement)РодительГруппы.Tag, (XmlElement)XMLУзел);
                НастройкиИзменены = true;
            }
        }

        private void проверитьОбновлениеToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ПотокПроверкиОбновления = new Thread(ВызовПроверкиОбновления);
            ПотокПроверкиОбновления.Start();
        }                

        static void ВызовПроверкиОбновления()
        {
            ФормаОбновления ПроверкаОбновления = new ФормаОбновления();
            if (ПроверкаОбновления.ЕстьОбновление(true))
            {
                if (MessageBox.Show(null, "Обнаружена новая версия программы. Открыть окно обновления?", "Hot tray 1C .NET", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                    ПроверкаОбновления.ShowDialog();
            }
            else
                MessageBox.Show(null, "Новых версий программы не обнаружено!", "Hot tray 1C .NET", MessageBoxButtons.OK, MessageBoxIcon.Information);
            ПроверкаОбновления.Dispose();
            ОчисткаМусора();
        }
     
        private void СтраницаБазДанных_ItemSelectionChanged(object sender, ListViewItemSelectionChangedEventArgs e)
        {
            УправлениеКнопкойМонитора();
        }
              
        private void КопироватьЭлемент()
        {
            if (СтраницаБазДанных.SelectedItems.Count > 1)
                ПоказатьИнфомационноеСообщение("Нельзя копировать несколько элементов", this);
            else if (СтраницаБазДанных.SelectedItems.Count == 1)
            {
                ListViewItem ВыбранныйЭлемент = СтраницаБазДанных.SelectedItems[0];
                XmlNode УзелЭлемента = (XmlNode)ВыбранныйЭлемент.Tag;
                if (УзелЭлемента.Name != "Группа")
                {
                    ОтменитьРегистрациюВсехГорячихКлавиш(УзелБазДанных, 0);

                    СтруктураНастроекЭлемента НовыйЭлемент;

                    bool КопируетсяБаза = !Convert.ToBoolean(ПолучитьАтрибутУзла(УзелЭлемента, "Приложение", "false"));                    

                    if (КопируетсяБаза)
                    {
                        ФормаБазыДанных ФормаНастроек = new ФормаБазыДанных();
                        ФормаНастроек.ОткрытьБазуДанных((XmlNode)ДеревоГруппБазДанных.SelectedNode.Tag, УзелЭлемента, true);
                        if (ФормаНастроек.ShowDialog(this) == DialogResult.OK)
                        {
                            XmlElement РодительБазы = (XmlElement)ФормаНастроек.РодительБазыДанных;

                            // Добавляем базу в глобальный настройки
                            НовыйЭлемент = ФормаНастроек.ТекущаяНастройка;
                            XmlElement УзелБазыДанных = ДобавитьБазуДанныхВНастройки(НовыйЭлемент, РодительБазы);

                            // Базу в список добавляем только в том случае, если группа ее совпадает с выбранной
                            if (РодительБазы == (XmlElement)ДеревоГруппБазДанных.SelectedNode.Tag)
                            {
                                ListViewItem НоваяСтрокаСтраницы = СтраницаБазДанных.Items.Add(НовыйЭлемент.Наименование);

                                string СостояниеБазы = String.Empty;

                                if (БазаДанныхСуществуетПоУказанномуПути(НовыйЭлемент))
                                {
                                    СостояниеБазы = ПолучитьСостояниеБазы(НовыйЭлемент);
                                    НоваяСтрокаСтраницы.ImageKey = "Состояние" + СостояниеБазы + ".png";
                                }
                                else
                                {
                                    СостояниеБазы = ПолучитьСостояниеБазы(НовыйЭлемент);
                                    НоваяСтрокаСтраницы.ImageKey = "Отсутствует" + СостояниеБазы + ".png";
                                }
                                НоваяСтрокаСтраницы.SubItems.Add(НовыйЭлемент.Путь);
                                НоваяСтрокаСтраницы.SubItems.Add(НовыйЭлемент.ИмяПользователя);
                                НоваяСтрокаСтраницы.Tag = УзелБазыДанных;
                            }
                            РегистрацияБазы77(НовыйЭлемент);
                            КорректировкаГорячихКлавиш(УзелБазыДанных, УзелБазДанных);
                            НастройкиИзменены = true;
                        }
                        ФормаНастроек.Dispose();
                    }
                    else
                    {
                        ФормаВнешнегоПриложения ФормаНастроек = new ФормаВнешнегоПриложения();
                        ФормаНастроек.ОткрытьПриложение((XmlNode)ДеревоГруппБазДанных.SelectedNode.Tag, УзелЭлемента, true);
                        if (ФормаНастроек.ShowDialog(this) == DialogResult.OK)
                        {
                            XmlElement РодительПриложения = (XmlElement)ФормаНастроек.РодительБазыДанных;

                            // Добавляем в глобальные настройки
                            НовыйЭлемент = ФормаНастроек.ТекущаяНастройка;
                            XmlElement УзелПриложения = ДобавитьБазуДанныхВНастройки(НовыйЭлемент, РодительПриложения);
                            // Базу в список добавляем только в том случае, если группа ее совпадает с выбранной
                            if (РодительПриложения == (XmlElement)ДеревоГруппБазДанных.SelectedNode.Tag)
                            {
                                ListViewItem НоваяСтрокаСтраницы = СтраницаБазДанных.Items.Add(НовыйЭлемент.Наименование);
                                НоваяСтрокаСтраницы.SubItems.Add(НовыйЭлемент.Путь);
                                НоваяСтрокаСтраницы.SubItems.Add("");

                                int ИндексИконки = 0;
                                if (УдалосьДобавитьИконкуВКоллекцию(НовыйЭлемент, ref ИндексИконки))
                                    НоваяСтрокаСтраницы.ImageIndex = ИндексИконки;

                                НоваяСтрокаСтраницы.SubItems.Add("Да");
                                НоваяСтрокаСтраницы.Tag = УзелПриложения;
                            }
                            КорректировкаГорячихКлавиш(УзелПриложения, УзелБазДанных);
                            НастройкиИзменены = true;
                        }
                    }
                    ЗарегистрироватьВсеГорячиеКлавиши(УзелБазДанных, 0);
                    ОчисткаМусора();
                }
            }
        }

        private void ДобавитьКопированием_Click(object sender, EventArgs e)
        {
            КопироватьЭлемент();   
        }

        private void УстановкаРежимаЗапуска_Click(object sender, EventArgs e)
        {
            ToolStripItem Отправитель = (ToolStripItem)sender;
            int РежимЗапуска = (int)Отправитель.Tag;
            for (int i = 0; i < СтраницаБазДанных.SelectedItems.Count; i++)
            {
                XmlNode УзелЭлемента = (XmlNode)СтраницаБазДанных.SelectedItems[i].Tag;
                if (УзелЭлемента.Name != "Группа")
                {
                    УстановитьАтрибутУзла((XmlElement)УзелЭлемента, "РежимЗапуска", Convert.ToString(РежимЗапуска));
                }
            }
            ЗаполнениеСпискаБаз(ДеревоГруппБазДанных, СтраницаБазДанных);
        }

        private void ПрорисовкаВерхнегоЭлементаМенюДействий(object sender, PaintEventArgs e)
        {
            ToolStripItem Отправитель = (ToolStripItem)sender;
            if ((int)Отправитель.Tag == 0)
            {
                e.Graphics.DrawImage(Properties.Resources.УстановитьРежимSmall, 4, 2);
                e.Graphics.DrawString("Установка режима запуска", new Font(FontFamily.GenericSansSerif, 8, FontStyle.Regular), Brushes.Black, e.ClipRectangle.X + 19, e.ClipRectangle.Y + 3);
            }
            else
            {
                e.Graphics.DrawImage(Properties.Resources.ВопросSmall, 4, 2);
                e.Graphics.DrawString("Тип добавляемого элемента", new Font(FontFamily.GenericSansSerif, 8, FontStyle.Regular), Brushes.Black, e.ClipRectangle.X + 19, e.ClipRectangle.Y + 3);
            }
        }

        private void УстановитьРежиЗапуска_Click(object sender, EventArgs e)
        {
            if (СтраницаБазДанных.SelectedItems.Count != 0)
            {
                ContextMenuStrip ВыборДействия = new ContextMenuStrip();
                ВыборДействия.ShowImageMargin = false;
                ToolStripItem ЭлементМеню;
                ЭлементМеню = ВыборДействия.Items.Add("                                       ");
                ЭлементМеню.Paint += new PaintEventHandler(ПрорисовкаВерхнегоЭлементаМенюДействий);
                ЭлементМеню.Click += new EventHandler(УстановитьРежиЗапуска_Click);
                ЭлементМеню.Tag = 0;
                ЭлементМеню.Enabled = false;
                ЭлементМеню = ВыборДействия.Items.Add("-");
                ЭлементМеню = ВыборДействия.Items.Add("Запрашивать");
                ЭлементМеню.Click += new EventHandler(УстановкаРежимаЗапуска_Click);
                ЭлементМеню.Tag = 0;
                ЭлементМеню = ВыборДействия.Items.Add("1С Предприятие");
                ЭлементМеню.Click += new EventHandler(УстановкаРежимаЗапуска_Click);
                ЭлементМеню.Tag = 1;
                ЭлементМеню = ВыборДействия.Items.Add("Конфигуратор");
                ЭлементМеню.Click += new EventHandler(УстановкаРежимаЗапуска_Click);
                ЭлементМеню.Tag = 2;
                ВыборДействия.Show(MousePosition.X, MousePosition.Y);               
            }
        }

        private void УправлениеДоступностью()
        {
            ГруппаЦвета.Enabled = НастраиваемоеМенюЗапуска.Checked;
            ГруппаШрифт.Enabled = НастраиваемоеМенюЗапуска.Checked;
        }

        private void ИндикаторЦветФонаМеню_Click(object sender, EventArgs e)
        {
            if (ВыборЦвета.ShowDialog() == DialogResult.OK)
            {
                ИндикаторЦветФонаМеню.BackColor = ВыборЦвета.Color;
                НастройкиИзменены = true;
            }
        }

        private void ИндикаторЦветШрифта_Click(object sender, EventArgs e)
        {
            if (ВыборЦвета.ShowDialog() == DialogResult.OK)
            {
                ИндикаторЦветШрифта.BackColor = ВыборЦвета.Color;
                НастройкиИзменены = true;
            }
        }

        private void ИндикаторЦветСепаратора_Click(object sender, EventArgs e)
        {
            if (ВыборЦвета.ShowDialog() == DialogResult.OK)
            {
                ИндикаторЦветСепаратора.BackColor = ВыборЦвета.Color;
                НастройкиИзменены = true;
            }
        }

        private void РазмерШрифта_SelectedIndexChanged(object sender, EventArgs e)
        {
            НастройкиИзменены = true;    
        }

        private void НачертаниеШрифта_SelectedIndexChanged(object sender, EventArgs e)
        {
            НастройкиИзменены = true;
        }

        private bool ЕстьОбоиРабочегоСтола(ref string ПутьКФайлу)
        {
            try
            {
                RegistryKey Desktop = Registry.CurrentUser.OpenSubKey(@"Control Panel\Desktop\", true);
                ПутьКФайлу = (string)Desktop.GetValue("Wallpaper");                
                if (!String.IsNullOrEmpty(ПутьКФайлу))
                {
                    if (File.Exists(ПутьКФайлу))
                    {
                        if (String.IsNullOrEmpty(ВремФайлДесктопа))
                            ВремФайлДесктопа = Path.GetTempFileName();
                        File.Copy(ПутьКФайлу, ВремФайлДесктопа, true);
                        ПутьКФайлу = ВремФайлДесктопа;
                        return true;
                    }
                    else
                        return false;
                }
                else
                    return false;
            }
            catch
            {
                return false;
            }
        }

        private void ЗагрузитьРабочийСтол()
        {            
            int XScreen = Screen.PrimaryScreen.Bounds.Width;
            int YScreen = Screen.PrimaryScreen.Bounds.Height;

            int x = Convert.ToInt16(ПолучитьЗначениеНастройки("HotX", Convert.ToString(XScreen/2)));
            int y = Convert.ToInt16(ПолучитьЗначениеНастройки("HotY", Convert.ToString(YScreen/2)));

            if (x > XScreen)
                x = XScreen / 2;
            if (y > YScreen)
                y = YScreen / 2;

            double СоотношениеСторонЭкрана = (double)XScreen / (double)YScreen;

            try { РабочийСтол.Image.Dispose(); }
            catch
            {}

            Bitmap РисунокРабочегоСтола = null;
            string Обои = String.Empty;
            if (ЕстьОбоиРабочегоСтола(ref Обои))
            {
                РисунокРабочегоСтола = new Bitmap(Обои);                
            }
            else
                РисунокРабочегоСтола = new Bitmap(XScreen, YScreen);

            // Рисуем крест           
            // Вертикальная линия
            x = (int)(x * (double)РисунокРабочегоСтола.Width / (double)XScreen);
            int i = 0;
            for (i = 0; i < РисунокРабочегоСтола.Height; i++)
            {
                if (x > 0)
                    РисунокРабочегоСтола.SetPixel(x - 1, i, Color.Black);
                if (x > 1)
                    РисунокРабочегоСтола.SetPixel(x - 2, i, Color.Black);
                if (x > 2)
                    РисунокРабочегоСтола.SetPixel(x - 3, i, Color.Black);
                РисунокРабочегоСтола.SetPixel(x, i, Color.Black);
                РисунокРабочегоСтола.SetPixel(x + 1, i, Color.Black);
                РисунокРабочегоСтола.SetPixel(x + 2, i, Color.Black);
                РисунокРабочегоСтола.SetPixel(x + 3, i, Color.Black);
            }

            // Горизонтальная линия            
            y = (int)(y * (double)РисунокРабочегоСтола.Height / (double)YScreen);
            for (i = 0; i < РисунокРабочегоСтола.Width; i++)
            {
                if (y > 0)
                    РисунокРабочегоСтола.SetPixel(i, y - 1, Color.Black);
                if (y > 1)
                    РисунокРабочегоСтола.SetPixel(i, y - 2, Color.Black);
                if (y > 2)
                    РисунокРабочегоСтола.SetPixel(i, y - 3, Color.Black);
                РисунокРабочегоСтола.SetPixel(i, y, Color.Black);
                РисунокРабочегоСтола.SetPixel(i, y + 1, Color.Black);
                РисунокРабочегоСтола.SetPixel(i, y + 2, Color.Black);
                РисунокРабочегоСтола.SetPixel(i, y + 3, Color.Black);
            }

            РабочийСтол.Image = РисунокРабочегоСтола;
            РабочийСтол.Height = (int)((double)РабочийСтол.Width / СоотношениеСторонЭкрана);
            ОчисткаМусора();
        }

        private void УправлениеРабочимСтолом(MouseEventArgs e)
        {
            int XScreen = Screen.PrimaryScreen.Bounds.Width;
            int YScreen = Screen.PrimaryScreen.Bounds.Height;
            double СоотношениеСторонЭкрана = (double)XScreen / (double)YScreen;

            Bitmap РисунокРабочегоСтола = null;
            string Обои = String.Empty;
            if (ЕстьОбоиРабочегоСтола(ref Обои))
            {
                РисунокРабочегоСтола = new Bitmap(Обои);                
            }
            else
                РисунокРабочегоСтола = new Bitmap(XScreen, YScreen);

            // Рисуем крест на изображении
            double Соотношение = (double)РисунокРабочегоСтола.Width / (double)РабочийСтол.Width;
            int x = (int)((double)e.X * Соотношение);
            int Координата = (int)(e.X * (double)XScreen / (double)РабочийСтол.Width);
            УстановитьЗначениеНастройки("HotX", Convert.ToString(Координата));

            // Вертикальная линия
            int i = 0;
            for (i = 0; i < РисунокРабочегоСтола.Height; i++)
            {
                if (x > 0)
                    РисунокРабочегоСтола.SetPixel(x - 1, i, Color.Black);
                if (x > 1)
                    РисунокРабочегоСтола.SetPixel(x - 2, i, Color.Black);
                if (x > 2)
                    РисунокРабочегоСтола.SetPixel(x - 3, i, Color.Black);                                    
                РисунокРабочегоСтола.SetPixel(x, i, Color.Black);
                РисунокРабочегоСтола.SetPixel(x + 1, i, Color.Black);
                РисунокРабочегоСтола.SetPixel(x + 2, i, Color.Black);
                РисунокРабочегоСтола.SetPixel(x + 3, i, Color.Black);
            }

            // Горизонтальная линия
            Соотношение = (double)РисунокРабочегоСтола.Height / (double)РабочийСтол.Height;
            x = (int)((double)e.Y * Соотношение);
            Координата = (int)(e.Y * (double)YScreen / (double)РабочийСтол.Height);
            УстановитьЗначениеНастройки("HotY", Convert.ToString(Координата));
            for (i = 0; i < РисунокРабочегоСтола.Width; i++)
            {
                if (x > 0)
                    РисунокРабочегоСтола.SetPixel(i, x - 1, Color.Black);
                if (x > 1)
                    РисунокРабочегоСтола.SetPixel(i, x - 2, Color.Black);
                if (x > 2)
                    РисунокРабочегоСтола.SetPixel(i, x - 3, Color.Black);                    
                РисунокРабочегоСтола.SetPixel(i, x, Color.Black);
                РисунокРабочегоСтола.SetPixel(i, x + 1, Color.Black);
                РисунокРабочегоСтола.SetPixel(i, x + 2, Color.Black);
                РисунокРабочегоСтола.SetPixel(i, x + 3, Color.Black);
            }
            РабочийСтол.Image = (Image)РисунокРабочегоСтола;

            РабочийСтол.Height = (int)((double)РабочийСтол.Width / СоотношениеСторонЭкрана);
            НастройкиИзменены = true;
            ОчисткаМусора();
        }

        private void pictureBox_MouseDown(object sender, MouseEventArgs e)
        {
            УправлениеРабочимСтолом(e);
        }

        private void СсылкаПоЦентру_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            int XScreen = Screen.PrimaryScreen.Bounds.Width/2;
            int YScreen = Screen.PrimaryScreen.Bounds.Height/2;
            УстановитьЗначениеНастройки("HotX", Convert.ToString(XScreen));
            УстановитьЗначениеНастройки("HotY", Convert.ToString(YScreen));
            ЗагрузитьРабочийСтол();
            НастройкиИзменены = true;
        }
       
        private void Default_MouseLeave(object sender, EventArgs e)
        {
            ОчиститьИнформацию(); 
        }

		private void Default_MouseHover(object sender, EventArgs e)
        {			
			string ControlName = "";
			try
			{
				ControlName = ((Control)sender).Name;
			}
			catch
			{
				try
				{
					ControlName = ((ToolStripItem)sender).Name;										
				}
				catch
				{
					return;
				}
			}
            string Message = "";
                        
            if (ToolTipMessages.TryGetValue(ControlName, out Message))
            {
            	ПоказатьИнформацию(Message);
            }                        
        }
		
		private void Default_TextChanged(object sender, EventArgs e)
        {            
			TextBox SenderControl = (TextBox)sender;			
			УстановитьЗначениеНастройки(SenderControl.Name, SenderControl.Text);
            НастройкиИзменены = true;
        }
		
		private void Default_CheckedChanged(object sender, EventArgs e)
        {
            CheckBox CheckControl = (CheckBox)sender;			
			УстановитьЗначениеНастройки(CheckControl.Name, Convert.ToString(CheckControl.Checked));
            НастройкиИзменены = true;
            
            if (CheckControl.Name == "НастраиваемоеМенюЗапуска")
                УправлениеДоступностью();
        }
                
    }
}