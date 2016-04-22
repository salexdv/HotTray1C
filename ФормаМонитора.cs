using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Threading;
using System.IO;
using MainStruct;

namespace TestMenuPopup
{
    public partial class ФормаМонитора : Form
    {
        // Стурктуры для файловых баз 8.х
        
        struct СтруктураБлокаДанных
        {
            public string[] hexДанные;
        }

        struct СтруктураНулевогоБлока
        {

            public char[] Сигнатура; // сигнатура “1CDBMSV8”
            public string Платформа;
            public string Вер1;
            public string Вер2;
            public string Вер3;
            public string Вер4;
            public int Длина; // В блоках. Каждый блок по 4096 байт
            public int unknown;
        }


        struct СтруктураЗаголовочногоБлока
        {
            public char[] Сигнатура;
            public string НаименованиеСигнатуры;
            public int Длина;
            public int Версия1;
            public int Версия2;
            public int Версия;
            public int[] БлокиОбъекта;
        }

        struct СтруктураБлоковДанных
        {
            public int КоличествоБлоков;
            public int[] ИндексыБлоков;
        }

        struct СтруктураКорневогоБлока
        {
            public char[] Язык;
            public int КоличествоТаблиц;
            public int[] БлокиТаблиц;

        }

        private СтруктураНастроекЭлемента База;
        private string ПутьКФайлу;
        private int АктивнаяСтрока;
        private string ТекКоличествоПользователей;
        private bool ВыполняетсяМониторинг;
        private ListViewItem[] МассивСтрокаМонитора;



        private СтруктураБлокаДанных ПолучитьБлокДанных(int КоличествоРазмещаемыхБлоков)
        {
            СтруктураБлокаДанных БлокСДанными = new СтруктураБлокаДанных();
            string[] МассивСтрок = new string[КоличествоРазмещаемыхБлоков * 4096];
            БлокСДанными.hexДанные = МассивСтрок;
            return БлокСДанными;
        }

        private void ЗаполнитьБлокДанными(ref СтруктураБлокаДанных БлокСДанными, byte[] МассивБайт, int НомерБлока)
        {
            string[] hexДанные = BitConverter.ToString(МассивБайт).Split('-');
            for (int i = 0; i < 4096; i++)
            {
                if (НомерБлока == 1)
                    БлокСДанными.hexДанные[i] = hexДанные[i];
                else
                    БлокСДанными.hexДанные[i + 4096 * (НомерБлока - 1)] = hexДанные[i];
            }
        }

        private СтруктураЗаголовочногоБлока ПолучитьЗалоговочныйБлок(byte[] МассивБайт)
        {
            СтруктураЗаголовочногоБлока ЗаголовочныйБлок = new СтруктураЗаголовочногоБлока();
            ЗаголовочныйБлок.Сигнатура = new char[8];
            ЗаголовочныйБлок.БлокиОбъекта = new int[1018];

            Decoder Декодер = Encoding.UTF8.GetDecoder();
            Декодер.GetChars(МассивБайт, 0, 8, ЗаголовочныйБлок.Сигнатура, 0);

            foreach (char Символ in ЗаголовочныйБлок.Сигнатура)
            {
                ЗаголовочныйБлок.НаименованиеСигнатуры = ЗаголовочныйБлок.НаименованиеСигнатуры + Char.ToString(Символ);
            }

            ЗаголовочныйБлок.Длина = МассивБайт[8] + МассивБайт[9] + МассивБайт[10] + МассивБайт[11];
            ЗаголовочныйБлок.Версия1 = МассивБайт[12] + МассивБайт[13] + МассивБайт[14] + МассивБайт[15];
            ЗаголовочныйБлок.Версия2 = МассивБайт[16] + МассивБайт[17] + МассивБайт[18] + МассивБайт[19];
            ЗаголовочныйБлок.Версия = МассивБайт[20] + МассивБайт[21] + МассивБайт[22] + МассивБайт[23];

            for (int i = 0; i < 1018; i = i + 4)
                ЗаголовочныйБлок.БлокиОбъекта[i] = МассивБайт[i + 24] + МассивБайт[i + 25] + МассивБайт[i + 26] + МассивБайт[i + 27];

            return ЗаголовочныйБлок;
        }

        private СтруктураНулевогоБлока ПолучитьНулевойБлок(byte[] Массив)
        {
            СтруктураНулевогоБлока НулевойБлок = new СтруктураНулевогоБлока();

            НулевойБлок.Сигнатура = new char[8];
            Decoder Декодер = Encoding.UTF8.GetDecoder();
            Декодер.GetChars(Массив, 0, 8, НулевойБлок.Сигнатура, 0);

            НулевойБлок.Вер1 = Convert.ToString(Convert.ToString(Массив[8]));
            НулевойБлок.Вер2 = Convert.ToString(Convert.ToString(Массив[9]));
            НулевойБлок.Вер3 = Convert.ToString(Convert.ToString(Массив[10]));
            НулевойБлок.Вер4 = Convert.ToString(Convert.ToString(Массив[11]));
            НулевойБлок.Платформа = НулевойБлок.Вер1 + НулевойБлок.Вер2 + НулевойБлок.Вер3 + НулевойБлок.Вер4;

            НулевойБлок.Длина = (int)(Массив[12] + Массив[13] + Массив[14] + Массив[15]);
            НулевойБлок.unknown = (int)(Массив[16] + Массив[17] + Массив[18] + Массив[19]);

            return НулевойБлок;
        }

        public void Мониторинг()
        {
            ВыполняетсяМониторинг = true;
            if (База.ТипПлатформы == 0)
            {
                МассивСтрокаМонитора = new ListViewItem[50];
                
                string Соединения = (string)ПутьКФайлу.Clone();
                try
                {
                    List<DBUsers.StringMonitor> MonitorData = DBUsers.GetActiveUsers(Соединения);
                    if (MonitorData.Count == 0)
                    {
                        ТекКоличествоПользователей = "Нет активных пользователей";
                        ВыполняетсяМониторинг = false;
                        return;
                    }
                    
                    int i = 0;
                    foreach (DBUsers.StringMonitor Data in MonitorData)
                    {                        
                        ListViewItem СтрокаМонитора = new ListViewItem();
                        СтрокаМонитора.Text = Data.UserName;
                        string РежимЗапуска = Data.RunMode;
                        int КартинкаРежима = 0;
                        if (РежимЗапуска == "E")
                            РежимЗапуска = "1С Предприятие";
                        else if (РежимЗапуска == "C")
                        {
                            РежимЗапуска = "Конфигуратор";
                            КартинкаРежима = 1;
                        }
                        else if (РежимЗапуска == "D")
                        {
                            РежимЗапуска = "Отладчик";
                            КартинкаРежима = 2;
                        }
                        else if (РежимЗапуска == "M")
                        {
                            РежимЗапуска = "Монитор";
                            КартинкаРежима = 3;
                        }
                        СтрокаМонитора.ImageIndex = КартинкаРежима;
                        СтрокаМонитора.SubItems.Add(РежимЗапуска);

                        string РежимРаботы = Data.Mono;
                        if (РежимРаботы == "Y")
                            РежимРаботы = "Да";
                        else if (РежимРаботы == "N")
                            РежимРаботы = "Нет";

                        СтрокаМонитора.SubItems.Add(РежимРаботы);

                        СтрокаМонитора.SubItems.Add(Data.Date);
                        СтрокаМонитора.SubItems.Add(Data.ComputerName);
                        МассивСтрокаМонитора[i] = СтрокаМонитора;
                        i++;
                    }
                    MonitorData = null;
                }
                catch
                {
                    ТекКоличествоПользователей = "Err: " + Marshal.GetLastWin32Error().ToString();                    
                }
            }
            else
            {
                МассивСтрокаМонитора = new ListViewItem[50];
                if (База.ТипБазы == 1)
                {                    
                    string ИмяComОбъекта = String.Empty;
                    int СмещениеКартинки = 0;
                    if (База.ТипПлатформы == 2)
                        ИмяComОбъекта = "V81.ComConnector";
                    else
                    {
                        ИмяComОбъекта = "V82.ComConnector";
                        СмещениеКартинки = 2;
                    }
                    try
                    {
                        string[] ПараметрыСоединенияБазы = База.Путь.Split('/');
                        if (ПараметрыСоединенияБазы.Length == 2)
                        {
                            string ИмяТребуемойБазы = ПараметрыСоединенияБазы[1];

                            // Имя сервера (локального)
                            object[] ПараметрыСоединенияССеровером = new object[1];
                            ПараметрыСоединенияССеровером[0] = ПараметрыСоединенияБазы[0]; // Имя сервера
                            // Создаем объект V81.COMConnector
                            Type v80Type = Type.GetTypeFromProgID(ИмяComОбъекта);
                            Object _v80Connector = Activator.CreateInstance(v80Type);
                            // Получаем агент сервера 1С Предприятия                        
                            Object АгентСервера = v80Type.InvokeMember("ConnectAgent", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.InvokeMethod, null, _v80Connector, ПараметрыСоединенияССеровером);

                            // Получаем массив кластеров на данном сервере                        
                            Array МассивКластеров = (Array)v80Type.InvokeMember("GetClusters", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.InvokeMethod, null, АгентСервера, null);
                            int i = 0;
                            foreach (object Кластер in МассивКластеров)
                            {
                                object[] ПараметрыСоединения = new object[3];
                                ПараметрыСоединения[0] = Кластер;
                                ПараметрыСоединения[1] = "";
                                ПараметрыСоединения[2] = "";
                                v80Type.InvokeMember("Authenticate", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.InvokeMethod, null, АгентСервера, ПараметрыСоединения);
                               
                                object[] ПараметрыБаз = new object[1];
                                ПараметрыБаз[0] = Кластер;
                                Array МассивОписанийБазКластера = (Array)v80Type.InvokeMember("GetInfoBases", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.InvokeMethod, null, АгентСервера, ПараметрыБаз);
                                                               
                                foreach (object ОписаниеБазы in МассивОписанийБазКластера)
                                {
                                    string ИмяБазыДанных = (string)v80Type.InvokeMember("Name", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.GetProperty, null, ОписаниеБазы, null);
                                    
                                    if (ИмяТребуемойБазы.ToLower() == ИмяБазыДанных.ToLower())
                                    {
                                        string Описание = (string)v80Type.InvokeMember("Descr", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.GetProperty, null, ОписаниеБазы, null);

                                        string fieldApp = "Application";
                                        string fieldHost = "Host";
                                        string fieldConnID = "ConnID";
                                        string fieldConnectedAt = "ConnectedAt";
                                        string fieldUser = "";
										
                                        Array МассивСоединенийБазыВКластере;
                                        
										object[] ПараметрыОпределенияСоединений = new object[2];
                                        ПараметрыОпределенияСоединений[0] = Кластер;
                                        ПараметрыОпределенияСоединений[1] = ОписаниеБазы;
										
                                        try
                                        {
                                        	МассивСоединенийБазыВКластере = (Array)v80Type.InvokeMember("GetInfoBaseSessions", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.InvokeMethod, null, АгентСервера, ПараметрыОпределенияСоединений);
                                        	fieldApp = "AppID";
	                                        fieldHost = "Host";
                                        	fieldConnID = "SessionID";
    	                                    fieldConnectedAt = "StartedAt";
    	                                    fieldUser = "UserName";
                                        }
                                        catch
                                        {
                                        	МассивСоединенийБазыВКластере = (Array)v80Type.InvokeMember("GetInfoBaseConnections", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.InvokeMethod, null, АгентСервера, ПараметрыОпределенияСоединений);
                                        }
                                        
                                        foreach (object ОписаниеСоединений in МассивСоединенийБазыВКластере)
                                        {
                                            string ИмяПриложения = (string)v80Type.InvokeMember(fieldApp, System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.GetProperty, null, ОписаниеСоединений, null);
                                            string ИмяКомпьютера = (string)v80Type.InvokeMember(fieldHost, System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.GetProperty, null, ОписаниеСоединений, null);
                                            int ИдентификаторСоединения = (int)v80Type.InvokeMember(fieldConnID, System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.GetProperty, null, ОписаниеСоединений, null);
                                            string МоментСоединения = Convert.ToString((DateTime)v80Type.InvokeMember(fieldConnectedAt, System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.GetProperty, null, ОписаниеСоединений, null));
                                            string ИмяПользователя = "";
                                            
                                            if (!String.IsNullOrEmpty(fieldUser))                                            	                                           	                                          
                                            	ИмяПользователя = (string)v80Type.InvokeMember(fieldUser, System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.GetProperty, null, ОписаниеСоединений, null);

											if (String.IsNullOrEmpty(ИмяПользователя))
												ИмяПользователя	= " <Неопределено>";

                                            ListViewItem СтрокаМонитора = new ListViewItem();
                                            СтрокаМонитора.Text = ИмяПользователя + " (" + Convert.ToString(ИдентификаторСоединения) + ")";

                                            if (ИмяПриложения == "Designer")
                                            {
                                                СтрокаМонитора.SubItems.Add("Конфигуратор");
                                                СтрокаМонитора.ImageIndex = 5 + СмещениеКартинки;
                                            }
                                            else if (ИмяПриложения == "BackgroundJob")
                                            {
                                                СтрокаМонитора.SubItems.Add("Фоновое задание");
                                                СтрокаМонитора.ImageIndex = 4 + СмещениеКартинки;
                                            }
                                            else if (ИмяПриложения == "SrvrConsole")
                                            {
                                                СтрокаМонитора.SubItems.Add("Консоль кластера");
                                                СтрокаМонитора.ImageIndex = 4 + СмещениеКартинки;
                                            }
                                            else
                                            {
                                                СтрокаМонитора.SubItems.Add("1C Предприятие");
                                                СтрокаМонитора.ImageIndex = 4 + СмещениеКартинки;
                                            }


                                            СтрокаМонитора.SubItems.Add("");
                                            СтрокаМонитора.SubItems.Add(МоментСоединения);
                                            СтрокаМонитора.SubItems.Add(ИмяКомпьютера);
                                            МассивСтрокаМонитора[i] = СтрокаМонитора;
                                            i++;
                                        }
                                    }
                                }
                            }
                        }

                    }
                    catch (Exception e)
                    {
                        ТекКоличествоПользователей = "Err: " + e.Message;
                    }
                }
                else
                {
                    String ВремКаталог = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\HotTrayNET";
                    if (!Directory.Exists(ВремКаталог))
                        Directory.CreateDirectory(ВремКаталог);                

                    string GUIDФайла = Convert.ToString(Guid.NewGuid());
                    GUIDФайла = GUIDФайла.Replace("-", "");
                    bool GUIDСвободен = false;
                    while (!GUIDСвободен)
                    {
                        if (!File.Exists(ВремКаталог + "\\" + GUIDФайла + ".1cd"))
                            GUIDСвободен = true;
                        else
                        {
                            GUIDФайла = Convert.ToString(Guid.NewGuid());
                            GUIDФайла = GUIDФайла.Replace("-", "");
                        }

                    }

                    string ИмяФайла = ВремКаталог + "\\" + GUIDФайла + ".1cd";

                    string Откуда = База.Путь;
                    if (Откуда[Откуда.Length - 1] == '\\')
                        Откуда = Откуда.Substring(0, Откуда.Length - 1);


                    FileStream Read;
                    try
                    {
                        File.Copy(Откуда + "\\1cv8tmp.1cd", ИмяФайла, true);
                        Read = new FileStream(ИмяФайла, FileMode.Open);
                    }
                    catch
                    { 
                        
                        ВыполняетсяМониторинг = false;
                        return;
                    } 

                    //////////////////////////////////////////////////////////
                    //////////////////////////////////////////////////////////

                    СтруктураБлоковДанных БлокиКорневогоОбъекта = new СтруктураБлоковДанных();
                    СтруктураНулевогоБлока НулевойБлок = new СтруктураНулевогоБлока();

                    // ЗБ - заголовочный блок

                    byte[] МассивБайт = new byte[4096];
                    char[] Sign = new char[8];
                    int ПозицияВФайле = 0;
                    int ТекущийБлок = 0;
                    int ИндексЗБТаблицыРазмещенияКорневогоОбъекта = -1;
                    int j = 0;

                    // Для начала получим блоки корневого объекта
                    while (ПозицияВФайле != Read.Length)
                    {
                        Read.Read(МассивБайт, 0, 4096);

                        if (ТекущийБлок == 0)
                            НулевойБлок = ПолучитьНулевойБлок(МассивБайт);
                        else if (ТекущийБлок == 2)
                        {
                            // Получим заголовочный блок корневого объекта
                            СтруктураЗаголовочногоБлока ЗаголовочныйБлок = ПолучитьЗалоговочныйБлок(МассивБайт);
                            ИндексЗБТаблицыРазмещенияКорневогоОбъекта = ЗаголовочныйБлок.БлокиОбъекта[0];
                            if (ИндексЗБТаблицыРазмещенияКорневогоОбъекта == 0)
                            {                                                        
                                ТекКоличествоПользователей = "Err: Не найден заголовочный блок корневого объекта!";                                
                                break;
                            }
                        }
                        else if (ТекущийБлок == ИндексЗБТаблицыРазмещенияКорневогоОбъекта)
                        {
                            БлокиКорневогоОбъекта.КоличествоБлоков = МассивБайт[0] + МассивБайт[1] + МассивБайт[2] + МассивБайт[3];
                            БлокиКорневогоОбъекта.ИндексыБлоков = new int[БлокиКорневогоОбъекта.КоличествоБлоков];
                            int Счетик = 0;
                            for (int i = 0; i < 4096; i += 4)
                            {
                                int Блок = (int)(МассивБайт[i + 4] + МассивБайт[i + 5] + МассивБайт[i + 6] + МассивБайт[i + 7]);
                                if (Блок != 0)
                                {
                                    БлокиКорневогоОбъекта.ИндексыБлоков[Счетик] = Блок;
                                    Счетик++;
                                }

                                if (Счетик == БлокиКорневогоОбъекта.КоличествоБлоков)
                                    break;
                            }
                        }

                        ПозицияВФайле += 4096;
                        ТекущийБлок++;
                        Read.Seek(ПозицияВФайле, SeekOrigin.Begin);

                        if (БлокиКорневогоОбъекта.КоличествоБлоков != 0)
                            break;
                    }

                    if (БлокиКорневогоОбъекта.КоличествоБлоков != 0)
                    {
                        // Блоки корневого объекта успешно получены, возьмемся за таблицу активных пользователей
                        int ИндексТаблицыАктивныхПользователей = -1;
                        int ИндексЗБТаблицаАктивныхПользователей = -1;

                        Read.Seek(0, SeekOrigin.Begin);
                        ПозицияВФайле = 0;
                        ТекущийБлок = 0;

                        СтруктураКорневогоБлока КорневойОбъект = new СтруктураКорневогоБлока();

                        while (ПозицияВФайле != Read.Length)
                        {
                            Read.Read(МассивБайт, 0, 4096);

                            bool ЭтоКорневойБлок = false;
                            foreach (int ВремБлок in БлокиКорневогоОбъекта.ИндексыБлоков)
                            {
                                if (ВремБлок == ТекущийБлок)
                                {
                                    ЭтоКорневойБлок = true;
                                    break;
                                }
                            }

                            if (ЭтоКорневойБлок)
                            {

                                // Данные корневого объекта 
                                if (НулевойБлок.Платформа != "8050")
                                {
                                    КорневойОбъект.Язык = new char[32];
                                    int i = 0;
                                    for (i = 0; i < 32; i++)
                                        КорневойОбъект.Язык[i] = Convert.ToChar(МассивБайт[i]);

                                    КорневойОбъект.КоличествоТаблиц = МассивБайт[32] + МассивБайт[33] + МассивБайт[34] + МассивБайт[35];
                                    КорневойОбъект.БлокиТаблиц = new int[КорневойОбъект.КоличествоТаблиц];
                                    int b = 0;
                                    for (i = 36; i < МассивБайт.Length; i = i + 4)
                                    {
                                        byte a = (byte)(МассивБайт[i] + МассивБайт[i + 1] + МассивБайт[i + 2] + МассивБайт[i + 3]);
                                        if (a != 0)
                                        {
                                            a = (byte)(a + 3);
                                            КорневойОбъект.БлокиТаблиц[b] = a;
                                            b++;
                                        }
                                    }
                                }
                                else
                                {
                                    КорневойОбъект.Язык = new char[8];
                                    int i = 0;
                                    for (i = 0; i < 8; i++)
                                        КорневойОбъект.Язык[i] = Convert.ToChar(МассивБайт[i]);

                                    КорневойОбъект.КоличествоТаблиц = МассивБайт[8] + МассивБайт[9] + МассивБайт[10] + МассивБайт[11];
                                    КорневойОбъект.БлокиТаблиц = new int[КорневойОбъект.КоличествоТаблиц];
                                    int b = 0;
                                    for (i = 12; i < МассивБайт.Length; i = i + 4)
                                    {
                                        byte a = (byte)(МассивБайт[i] + МассивБайт[i + 1] + МассивБайт[i + 2] + МассивБайт[i + 3]);
                                        if (a != 0)
                                        {
                                            a = (byte)(a + 3);
                                            КорневойОбъект.БлокиТаблиц[b] = a;
                                            b++;
                                        }
                                    }
                                }
                            }
                            else if (КорневойОбъект.КоличествоТаблиц != 0)
                            {
                                bool ЭтоБлокТаблиц = false;
                                foreach (int ВремБлок in КорневойОбъект.БлокиТаблиц)
                                {
                                    if (ВремБлок == ТекущийБлок)
                                    {
                                        ЭтоБлокТаблиц = true;
                                        break;
                                    }
                                }

                                if (ЭтоБлокТаблиц)
                                {
                                    // Блок с описанием какой либо таблицы
                                    string ОписаниеТаблицы = String.Empty;
                                    for (int i = 0; i < 4096; i++)
                                    {
                                        if (МассивБайт[i] != 0)
                                            ОписаниеТаблицы = ОписаниеТаблицы + Convert.ToString((char)МассивБайт[i]);
                                    }
                                    ИндексТаблицыАктивныхПользователей = ОписаниеТаблицы.IndexOf("ACTIVEUSERS");
                                    if (ИндексТаблицыАктивныхПользователей != -1)
                                    {
                                        //Writer.WriteLine(ОписаниеТаблицы);
                                        ИндексЗБТаблицаАктивныхПользователей = ОписаниеТаблицы.IndexOf("{\"Files\",");
                                        ИндексЗБТаблицаАктивныхПользователей = Convert.ToUInt16(ОписаниеТаблицы.Substring(ИндексЗБТаблицаАктивныхПользователей + 9, 1));
                                    }
                                }
                            }
                            if (ИндексТаблицыАктивныхПользователей != -1)
                                break;

                            ПозицияВФайле += 4096;
                            ТекущийБлок++;
                            Read.Seek(ПозицияВФайле, SeekOrigin.Begin);
                        }

                        if ((ИндексТаблицыАктивныхПользователей != -1) && (ИндексЗБТаблицаАктивныхПользователей != -1))
                        {
                            // Индекс ЗБ таблицы активных пользователей получен
                            СтруктураБлоковДанных ДанныеТаблицыАктивныхПользователей = new СтруктураБлоковДанных();
                            СтруктураБлокаДанных Данные = new СтруктураБлокаДанных();

                            int ИндексЗБТаблицыРазмещения = -1;
                            Read.Seek(0, SeekOrigin.Begin);
                            ПозицияВФайле = 0;
                            ТекущийБлок = 0;
                            int КоличествоБлоковСДанными = 0;
                            while (ПозицияВФайле != Read.Length)
                            {
                                Read.Read(МассивБайт, 0, 4096);

                                if (ТекущийБлок == ИндексЗБТаблицаАктивныхПользователей)
                                {
                                    // Получим заголовочный блок таблицы активных пользователей
                                    СтруктураЗаголовочногоБлока ЗаголовочныйБлок = ПолучитьЗалоговочныйБлок(МассивБайт);
                                    ИндексЗБТаблицыРазмещения = ЗаголовочныйБлок.БлокиОбъекта[0];
                                }
                                else if (ТекущийБлок == ИндексЗБТаблицыРазмещения)
                                {
                                    // Получим данные таблицы размещения (то, в каких блоках находится нужная нам информация)
                                    ДанныеТаблицыАктивныхПользователей.КоличествоБлоков = МассивБайт[0] + МассивБайт[1] + МассивБайт[2] + МассивБайт[3];
                                    ДанныеТаблицыАктивныхПользователей.ИндексыБлоков = new int[ДанныеТаблицыАктивныхПользователей.КоличествоБлоков];
                                    int Счетик = 0;
                                    for (int i = 0; i < 4096; i += 4)
                                    {
                                        int Блок = (int)(МассивБайт[i + 4] + МассивБайт[i + 5] + МассивБайт[i + 6] + МассивБайт[i + 7]);
                                        if (Блок != 0)
                                        {
                                            ДанныеТаблицыАктивныхПользователей.ИндексыБлоков[Счетик] = Блок;
                                            Счетик++;
                                        }

                                        if (Счетик == ДанныеТаблицыАктивныхПользователей.КоличествоБлоков)
                                            break;
                                    }
                                    if (Счетик == 0)
                                    {
                                        ТекКоличествоПользователей = "Err: Нет данных по активным польщователям!";                                        
                                        break;
                                    }
                                    Данные = ПолучитьБлокДанных(Счетик);

                                }
                                else if (ДанныеТаблицыАктивныхПользователей.КоличествоБлоков != 0)
                                {
                                    // Собственно чтение данных об активных пользователях
                                    bool ЭтоБлокПользователей = false;
                                    foreach (int ВремБлок in ДанныеТаблицыАктивныхПользователей.ИндексыБлоков)
                                    {
                                        if (ВремБлок == ТекущийБлок)
                                        {
                                            ЭтоБлокПользователей = true;
                                            break;
                                        }
                                    }

                                    if (ЭтоБлокПользователей)
                                    {
                                        КоличествоБлоковСДанными++;
                                        ЗаполнитьБлокДанными(ref Данные, МассивБайт, КоличествоБлоковСДанными);

                                        if (ДанныеТаблицыАктивныхПользователей.КоличествоБлоков == КоличествоБлоковСДанными)
                                            break;
                                    }
                                }


                                ПозицияВФайле += 4096;
                                ТекущийБлок++;
                                Read.Seek(ПозицияВФайле, SeekOrigin.Begin);
                            }

                            if (КоличествоБлоковСДанными > 0)
                            {
                                int ТекИндекс = 0;

                                while (ТекИндекс < Данные.hexДанные.Length)
                                {
                                    // У разных платформ разные поля в таблице ACTIVEUSERS и разная длина записи
                                    if (НулевойБлок.Платформа == "8100")
                                    {
                                        // Платформа 8.1

                                        // Получим первый байт - признак того, что запись свободна
                                        if (Convert.ToInt32(Данные.hexДанные[ТекИндекс], 16) == 1)
                                        {
                                            // Запись свободная, пропускаем ее
                                            ТекИндекс += 1329;
                                            continue;
                                        }

                                        if (Convert.ToInt32(Данные.hexДанные[ТекИндекс + 1], 16) == 0)
                                        {
                                            // Запись свободная, пропускаем ее
                                            break;
                                        }

                                        // Пропускаем скрытую версию (8 байт)
                                        ТекИндекс += 8;
                                        // Еще один байт - признак того, что наша запись не удалена
                                        ТекИндекс++;

                                        // Поле CONNECTID 6 байт                    
                                        string CONNECTID = String.Empty;
                                        for (int i = 0; i < 6; i++)
                                        {
                                            CONNECTID += Данные.hexДанные[ТекИндекс + i];
                                        }
                                        int ЗнакЧисла = Convert.ToInt16(CONNECTID.Substring(0, 1));
                                        CONNECTID = CONNECTID.Substring(1, 10);
                                        int ID = Convert.ToInt16(CONNECTID);
                                        if (ЗнакЧисла == 0)
                                            ID = ID * -1;
                                        ТекИндекс += 6;

                                        // Поле USERID 16 байт
                                        string USERID = String.Empty;
                                        for (int i = 0; i < 16; i++)
                                        {
                                            USERID += Данные.hexДанные[ТекИндекс + i];
                                        }
                                        ТекИндекс += 16;

                                        // Поле USERNAME 514 байт
                                        int ДлинаПоляUSERNAME = Convert.ToInt32(Данные.hexДанные[ТекИндекс], 16) + Convert.ToInt32(Данные.hexДанные[ТекИндекс + 1], 16);
                                        ТекИндекс += 2;
                                        string USERNAME = String.Empty;
                                        for (int i = 0; i < ДлинаПоляUSERNAME * 2; i += 2)
                                        {
                                            // Строка в юникоде - каждый символ 2 байта
                                            USERNAME += Char.ConvertFromUtf32(Convert.ToInt32(Данные.hexДанные[ТекИндекс + i + 1] + Данные.hexДанные[ТекИндекс + i], 16));
                                        }
                                        ТекИндекс += 512;

                                        // Поле USERFULLNAME 514 байт
                                        int ДлинаПоляUSERFULLNAME = Convert.ToInt32(Данные.hexДанные[ТекИндекс], 16) + Convert.ToInt32(Данные.hexДанные[ТекИндекс + 1], 16);
                                        ТекИндекс += 2;
                                        string USERFULLNAME = String.Empty;
                                        for (int i = 0; i < ДлинаПоляUSERFULLNAME * 2; i += 2)
                                        {
                                            // Строка в юникоде - каждый символ 2 байта
                                            USERFULLNAME += Char.ConvertFromUtf32(Convert.ToInt32(Данные.hexДанные[ТекИндекс + i + 1] + Данные.hexДанные[ТекИндекс + i], 16));
                                        }
                                        ТекИндекс += 512;

                                        // Поле MODE 1 байт                    
                                        string MODESTRING = Данные.hexДанные[ТекИндекс];
                                        ЗнакЧисла = Convert.ToInt16(MODESTRING.Substring(0, 1));
                                        int MODE = Convert.ToInt16(MODESTRING.Substring(1, 1));
                                        if (ЗнакЧисла == 0)
                                            MODE = MODE * -1;
                                        ТекИндекс++;

                                        // Поле CONNECTEDAT 7 байт                    
                                        string CONNECTEDAT = Данные.hexДанные[ТекИндекс] + Данные.hexДанные[ТекИндекс + 1]; // год
                                        CONNECTEDAT = Данные.hexДанные[ТекИндекс + 2] + "." + CONNECTEDAT;
                                        CONNECTEDAT = Данные.hexДанные[ТекИндекс + 3] + "." + CONNECTEDAT;
                                        CONNECTEDAT += " " + Данные.hexДанные[ТекИндекс + 4] + ":";
                                        CONNECTEDAT += Данные.hexДанные[ТекИндекс + 5] + ":";
                                        CONNECTEDAT += Данные.hexДанные[ТекИндекс + 6];
                                        ТекИндекс += 7;

                                        // Поле HOSTNAME 130 байт
                                        int ДлинаПоляHOSTNAME = Convert.ToInt32(Данные.hexДанные[ТекИндекс], 16) + Convert.ToInt32(Данные.hexДанные[ТекИндекс + 1], 16);
                                        ТекИндекс += 2;
                                        string HOSTNAME = String.Empty;
                                        for (int i = 0; i < ДлинаПоляHOSTNAME * 2; i += 2)
                                        {
                                            // Строка в юникоде - каждый символ 2 байта
                                            HOSTNAME += Char.ConvertFromUtf32(Convert.ToInt32(Данные.hexДанные[ТекИндекс + i + 1] + Данные.hexДанные[ТекИндекс + i], 16));
                                        }
                                        ТекИндекс += 128;

                                        // Поле APPID 130 байт
                                        int ДлинаПоляAPPID = Convert.ToInt32(Данные.hexДанные[ТекИндекс], 16) + Convert.ToInt32(Данные.hexДанные[ТекИндекс + 1], 16);
                                        ТекИндекс += 2;
                                        string APPID = String.Empty;
                                        for (int i = 0; i < ДлинаПоляAPPID * 2; i += 2)
                                        {
                                            // Строка в юникоде - каждый символ 2 байта
                                            APPID += Char.ConvertFromUtf32(Convert.ToInt32(Данные.hexДанные[ТекИндекс + i + 1] + Данные.hexДанные[ТекИндекс + i], 16));
                                        }
                                        ТекИндекс += 128;

                                        // Поле DBMODE 1 байт                    
                                        string DBMODE = Данные.hexДанные[ТекИндекс];
                                        int NULL = Convert.ToInt16(DBMODE.Substring(0, 1));
                                        DBMODE = DBMODE.Substring(1, 1);
                                        if (NULL == 0)
                                            DBMODE = "{NULL}";
                                        ТекИндекс += 2;

                                        ListViewItem СтрокаМонитора = new ListViewItem();
                                        ДобавитьДанныеВоВременнуюСтрокуМонитора(ref СтрокаМонитора, USERNAME, APPID, CONNECTEDAT, HOSTNAME, ID, НулевойБлок.Платформа);
                                        МассивСтрокаМонитора[j] = СтрокаМонитора;
                                        j++;

                                    }
                                    else if (НулевойБлок.Платформа.StartsWith("82"))
                                    {
                                        // Платформа 8.2

                                        // Получим первый байт - признак того, что запись свободна
                                        if (Convert.ToInt32(Данные.hexДанные[ТекИндекс], 16) == 1)
                                        {
                                            // Запись свободная, пропускаем ее
                                            ТекИндекс += 1865;
                                            continue;
                                        }

                                        if (Convert.ToInt32(Данные.hexДанные[ТекИндекс + 1], 16) == 0)
                                        {
                                            // Запись свободная, пропускаем ее
                                            break;
                                        }

                                        // Пропускаем скрытую версию (8 байт)
                                        ТекИндекс += 8;
                                        // Еще один байт - признак того, что наша запись не удалена
                                        ТекИндекс++;

                                        // Поле CONNECTID 6 байт                    
                                        string CONNECTID = String.Empty;
                                        for (int i = 0; i < 6; i++)
                                        {
                                            CONNECTID += Данные.hexДанные[ТекИндекс + i];
                                        }
                                        int ЗнакЧисла = Convert.ToInt16(CONNECTID.Substring(0, 1));
                                        CONNECTID = CONNECTID.Substring(1, 10);
                                        int ID = Convert.ToInt16(CONNECTID);
                                        if (ЗнакЧисла == 0)
                                            ID = ID * -1;
                                        ТекИндекс += 6;

                                        // Поле SEANCENUMB 6 байт                    
                                        string SEANCENUMB = String.Empty;
                                        for (int i = 0; i < 6; i++)
                                        {
                                            SEANCENUMB += Данные.hexДанные[ТекИндекс + i];
                                        }
                                        ЗнакЧисла = Convert.ToInt16(SEANCENUMB.Substring(0, 1));
                                        SEANCENUMB = SEANCENUMB.Substring(1, 10);
                                        int SEANCEN = Convert.ToInt16(SEANCENUMB);
                                        if (ЗнакЧисла == 0)
                                            SEANCEN = SEANCEN * -1;
                                        ТекИндекс += 6;

                                        // Поле USERID 16 байт
                                        string USERID = String.Empty;
                                        for (int i = 0; i < 16; i++)
                                        {
                                            USERID += Данные.hexДанные[ТекИндекс + i];
                                        }
                                        ТекИндекс += 16;

                                        // Поле USERNAME 514 байт
                                        int ДлинаПоляUSERNAME = Convert.ToInt32(Данные.hexДанные[ТекИндекс], 16) + Convert.ToInt32(Данные.hexДанные[ТекИндекс + 1], 16);
                                        ТекИндекс += 2;
                                        string USERNAME = String.Empty;
                                        for (int i = 0; i < ДлинаПоляUSERNAME * 2; i += 2)
                                        {
                                            // Строка в юникоде - каждый символ 2 байта
                                            USERNAME += Char.ConvertFromUtf32(Convert.ToInt32(Данные.hexДанные[ТекИндекс + i + 1] + Данные.hexДанные[ТекИндекс + i], 16));
                                        }
                                        ТекИндекс += 512;

                                        // Поле USERFULLNAME 514 байт
                                        int ДлинаПоляUSERFULLNAME = Convert.ToInt32(Данные.hexДанные[ТекИндекс], 16) + Convert.ToInt32(Данные.hexДанные[ТекИндекс + 1], 16);
                                        ТекИндекс += 2;
                                        string USERFULLNAME = String.Empty;
                                        for (int i = 0; i < ДлинаПоляUSERFULLNAME * 2; i += 2)
                                        {
                                            // Строка в юникоде - каждый символ 2 байта
                                            USERFULLNAME += Char.ConvertFromUtf32(Convert.ToInt32(Данные.hexДанные[ТекИндекс + i + 1] + Данные.hexДанные[ТекИндекс + i], 16));
                                        }
                                        ТекИндекс += 512;

                                        // Поле MODE 1 байт                    
                                        string MODESTRING = Данные.hexДанные[ТекИндекс];
                                        ЗнакЧисла = Convert.ToInt16(MODESTRING.Substring(0, 1));
                                        int MODE = Convert.ToInt16(MODESTRING.Substring(1, 1));
                                        if (ЗнакЧисла == 0)
                                            MODE = MODE * -1;
                                        ТекИндекс++;

                                        // Поле CONNECTEDAT 7 байт                    
                                        string CONNECTEDAT = Данные.hexДанные[ТекИндекс] + Данные.hexДанные[ТекИндекс + 1]; // год
                                        CONNECTEDAT = Данные.hexДанные[ТекИндекс + 2] + "." + CONNECTEDAT;
                                        CONNECTEDAT = Данные.hexДанные[ТекИндекс + 3] + "." + CONNECTEDAT;
                                        CONNECTEDAT += " " + Данные.hexДанные[ТекИндекс + 4] + ":";
                                        CONNECTEDAT += Данные.hexДанные[ТекИндекс + 5] + ":";
                                        CONNECTEDAT += Данные.hexДанные[ТекИндекс + 6];
                                        ТекИндекс += 7;

                                        // Поле HOSTNAME 130 байт
                                        int ДлинаПоляHOSTNAME = Convert.ToInt32(Данные.hexДанные[ТекИндекс], 16) + Convert.ToInt32(Данные.hexДанные[ТекИндекс + 1], 16);
                                        ТекИндекс += 2;
                                        string HOSTNAME = String.Empty;
                                        for (int i = 0; i < ДлинаПоляHOSTNAME * 2; i += 2)
                                        {
                                            // Строка в юникоде - каждый символ 2 байта
                                            HOSTNAME += Char.ConvertFromUtf32(Convert.ToInt32(Данные.hexДанные[ТекИндекс + i + 1] + Данные.hexДанные[ТекИндекс + i], 16));
                                        }
                                        ТекИндекс += 128;

                                        // Поле APPID 130 байт
                                        int ДлинаПоляAPPID = Convert.ToInt32(Данные.hexДанные[ТекИндекс], 16) + Convert.ToInt32(Данные.hexДанные[ТекИндекс + 1], 16);
                                        ТекИндекс += 2;
                                        string APPID = String.Empty;
                                        for (int i = 0; i < ДлинаПоляAPPID * 2; i += 2)
                                        {
                                            // Строка в юникоде - каждый символ 2 байта
                                            APPID += Char.ConvertFromUtf32(Convert.ToInt32(Данные.hexДанные[ТекИндекс + i + 1] + Данные.hexДанные[ТекИндекс + i], 16));
                                        }
                                        ТекИндекс += 128;

                                        // Поле DBMODE 1 байт                    
                                        string DBMODE = Данные.hexДанные[ТекИндекс];
                                        int NULL = Convert.ToInt16(DBMODE.Substring(0, 1));
                                        DBMODE = DBMODE.Substring(1, 1);
                                        if (NULL == 0)
                                            DBMODE = "{NULL}";
                                        ТекИндекс += 2;

                                        // Поле IBURL 514 байт
                                        int ДлинаПоляIBURL = Convert.ToInt32(Данные.hexДанные[ТекИндекс], 16) + Convert.ToInt32(Данные.hexДанные[ТекИндекс + 1], 16);
                                        ТекИндекс += 2;
                                        string IBURL = String.Empty;
                                        for (int i = 0; i < ДлинаПоляIBURL * 2; i += 2)
                                        {
                                            // Строка в юникоде - каждый символ 2 байта
                                            IBURL += Char.ConvertFromUtf32(Convert.ToInt32(Данные.hexДанные[ТекИндекс + i + 1] + Данные.hexДанные[ТекИндекс + i], 16));
                                        }
                                        ТекИндекс += 512;

                                        // Поле USERID 16 байт
                                        string SEANCEID = String.Empty;
                                        for (int i = 0; i < 16; i++)
                                        {
                                            SEANCEID += Данные.hexДанные[ТекИндекс + i];
                                        }
                                        ТекИндекс += 16;

                                        if ((APPID == "WebServerExtension") && (USERNAME == ""))
                                            continue;
                                        
                                        ListViewItem СтрокаМонитора = new ListViewItem();
                                        ДобавитьДанныеВоВременнуюСтрокуМонитора(ref СтрокаМонитора, USERNAME, APPID, CONNECTEDAT, HOSTNAME, SEANCEN, НулевойБлок.Платформа);
                                        МассивСтрокаМонитора[j] = СтрокаМонитора;
                                        j++;
                                    }
                                    else if (НулевойБлок.Платформа == "8050")
                                    {
                                        // Платформа 8.0

                                        // Получим первый байт - признак того, что запись свободна
                                        if (Convert.ToInt32(Данные.hexДанные[ТекИндекс], 16) == 1)
                                        {
                                            // Запись свободная, пропускаем ее
                                            ТекИндекс += 593;
                                            continue;
                                        }

                                        if (Convert.ToInt32(Данные.hexДанные[ТекИндекс + 1], 16) == 0)
                                        {
                                            // Запись свободная, пропускаем ее
                                            break;
                                        }

                                        // Пропускаем скрытую версию (8 байт)
                                        ТекИндекс += 8;
                                        // Еще один байт - признак того, что наша запись не удалена
                                        ТекИндекс++;

                                        // Поле CONNECTID 6 байт                    
                                        string CONNECTID = String.Empty;
                                        for (int i = 0; i < 6; i++)
                                        {
                                            CONNECTID += Данные.hexДанные[ТекИндекс + i];
                                        }
                                        int ЗнакЧисла = Convert.ToInt16(CONNECTID.Substring(0, 1));
                                        CONNECTID = CONNECTID.Substring(1, 10);
                                        int ID = Convert.ToInt16(CONNECTID);
                                        if (ЗнакЧисла == 0)
                                            ID = ID * -1;
                                        ТекИндекс += 6;

                                        // Поле USERID 16 байт
                                        string USERID = String.Empty;
                                        for (int i = 0; i < 16; i++)
                                        {
                                            USERID += Данные.hexДанные[ТекИндекс + i];
                                        }
                                        ТекИндекс += 16;

                                        // Поле USERNAME 98 байт
                                        int ДлинаПоляUSERNAME = Convert.ToInt32(Данные.hexДанные[ТекИндекс], 16) + Convert.ToInt32(Данные.hexДанные[ТекИндекс + 1], 16);
                                        ТекИндекс += 2;
                                        string USERNAME = String.Empty;
                                        for (int i = 0; i < ДлинаПоляUSERNAME * 2; i += 2)
                                        {
                                            // Строка в юникоде - каждый символ 2 байта
                                            USERNAME += Char.ConvertFromUtf32(Convert.ToInt32(Данные.hexДанные[ТекИндекс + i + 1] + Данные.hexДанные[ТекИндекс + i], 16));
                                        }
                                        ТекИндекс += 96;

                                        // Поле USERFULLNAME 194 байт
                                        int ДлинаПоляUSERFULLNAME = Convert.ToInt32(Данные.hexДанные[ТекИндекс], 16) + Convert.ToInt32(Данные.hexДанные[ТекИндекс + 1], 16);
                                        ТекИндекс += 2;
                                        string USERFULLNAME = String.Empty;
                                        for (int i = 0; i < ДлинаПоляUSERFULLNAME * 2; i += 2)
                                        {
                                            // Строка в юникоде - каждый символ 2 байта
                                            USERFULLNAME += Char.ConvertFromUtf32(Convert.ToInt32(Данные.hexДанные[ТекИндекс + i + 1] + Данные.hexДанные[ТекИндекс + i], 16));
                                        }
                                        ТекИндекс += 192;

                                        // Поле MODE 1 байт                    
                                        string MODESTRING = Данные.hexДанные[ТекИндекс];
                                        ЗнакЧисла = Convert.ToInt16(MODESTRING.Substring(0, 1));
                                        int MODE = Convert.ToInt16(MODESTRING.Substring(1, 1));
                                        if (ЗнакЧисла == 0)
                                            MODE = MODE * -1;
                                        ТекИндекс++;

                                        // Поле CONNECTEDAT 7 байт                    
                                        string CONNECTEDAT = Данные.hexДанные[ТекИндекс] + Данные.hexДанные[ТекИндекс + 1]; // год
                                        CONNECTEDAT = Данные.hexДанные[ТекИндекс + 2] + "." + CONNECTEDAT;
                                        CONNECTEDAT = Данные.hexДанные[ТекИндекс + 3] + "." + CONNECTEDAT;
                                        CONNECTEDAT += " " + Данные.hexДанные[ТекИндекс + 4] + ":";
                                        CONNECTEDAT += Данные.hexДанные[ТекИндекс + 5] + ":";
                                        CONNECTEDAT += Данные.hexДанные[ТекИндекс + 6];
                                        ТекИндекс += 7;

                                        // Поле HOSTNAME 130 байт
                                        int ДлинаПоляHOSTNAME = Convert.ToInt32(Данные.hexДанные[ТекИндекс], 16) + Convert.ToInt32(Данные.hexДанные[ТекИндекс + 1], 16);
                                        ТекИндекс += 2;
                                        string HOSTNAME = String.Empty;
                                        for (int i = 0; i < ДлинаПоляHOSTNAME * 2; i += 2)
                                        {
                                            // Строка в юникоде - каждый символ 2 байта
                                            HOSTNAME += Char.ConvertFromUtf32(Convert.ToInt32(Данные.hexДанные[ТекИндекс + i + 1] + Данные.hexДанные[ТекИндекс + i], 16));
                                        }
                                        ТекИндекс += 128;

                                        // Поле APPID 130 байт
                                        int ДлинаПоляAPPID = Convert.ToInt32(Данные.hexДанные[ТекИндекс], 16) + Convert.ToInt32(Данные.hexДанные[ТекИндекс + 1], 16);
                                        ТекИндекс += 2;
                                        string APPID = String.Empty;
                                        for (int i = 0; i < ДлинаПоляAPPID * 2; i += 2)
                                        {
                                            // Строка в юникоде - каждый символ 2 байта
                                            APPID += Char.ConvertFromUtf32(Convert.ToInt32(Данные.hexДанные[ТекИндекс + i + 1] + Данные.hexДанные[ТекИндекс + i], 16));
                                        }
                                        ТекИндекс += 128;

                                        // Поле DBMODE 1 байт                    
                                        string DBMODE = Данные.hexДанные[ТекИндекс];
                                        int NULL = Convert.ToInt16(DBMODE.Substring(0, 1));
                                        DBMODE = DBMODE.Substring(1, 1);
                                        if (NULL == 0)
                                            DBMODE = "{NULL}";
                                        ТекИндекс += 2;

                                        ListViewItem СтрокаМонитора = new ListViewItem();
                                        ДобавитьДанныеВоВременнуюСтрокуМонитора(ref СтрокаМонитора, USERNAME, APPID, CONNECTEDAT, HOSTNAME, ID, НулевойБлок.Платформа);
                                        МассивСтрокаМонитора[j] = СтрокаМонитора;
                                        j++;
                                    }
                                    else
                                    {
                                        ТекКоличествоПользователей = "Err: Неизвестный тип платформы (" + НулевойБлок.Платформа + ")";
                                        break;
                                    }
                                }

                            }

                        }

                    }
                    //////////////////////////////////////////////////////////
                    //////////////////////////////////////////////////////////
                }

            }
            ВыполняетсяМониторинг = false;
        }

        private void ДобавитьДанныеВоВременнуюСтрокуМонитора(ref ListViewItem СтрокаМонитора, string Пользователь, string Приложение, string НачалоРаботы, string Компьютер, int Сеанс, String ВерсияПлатформы)
        {
            
            int СмещениеКартинки = 0;
            int Картинка = 0;
            if (ВерсияПлатформы == "8200")
                СмещениеКартинки = 2;            
            // Определимся с названием приложения
            if ((Приложение == "Designer") | (Приложение == "Config"))
            {
                Приложение = "Конфигуратор";
                Картинка = 5 + СмещениеКартинки;
            }
            else if (Приложение == "1CV8")
            {
                Приложение = "1С Предприятие";
                Картинка = 4 + СмещениеКартинки;
            }
            else if (Приложение == "1CV8C")
            {
                Приложение = "Тонкий клиент";
                Картинка = 4 + СмещениеКартинки;
            }
            else if (Приложение == "WebServerExtension")
            {
                Приложение = "Веб клиент";
                Компьютер = "";
                Картинка = 4 + СмещениеКартинки;
                if (String.IsNullOrEmpty(Пользователь))
                    return;
            }
                       
            СтрокаМонитора.Text = Пользователь;
            СтрокаМонитора.ImageIndex = Картинка;
            СтрокаМонитора.SubItems.Add(Приложение);
            СтрокаМонитора.SubItems.Add("");
            СтрокаМонитора.SubItems.Add(НачалоРаботы);
            СтрокаМонитора.SubItems.Add(Компьютер);            
        }
    
        public ФормаМонитора(СтруктураНастроекЭлемента НастройкаБазы)
        {
            InitializeComponent();
            ТекстКоличествоПользователей.Text = "Нет активных пользователей";
            База = НастройкаБазы;
            ПутьКФайлу = База.Путь;
            Text = "Монитор пользователей " + НастройкаБазы.Наименование;
            ТекстНаименованиеБазы.Text = "База данных: " + НастройкаБазы.Наименование;
            ТекстПутьКБазе.Text = "Расположение базы: " + ПутьКФайлу;
            if (ПутьКФайлу[ПутьКФайлу.Length - 1] == '\\')
                ПутьКФайлу = ПутьКФайлу.Remove(ПутьКФайлу.Length - 1);

            ПутьКФайлу = ПутьКФайлу + "\\SYSLOG\\links.tmp";            
            if (База.ТипПлатформы == 0)
                ТаймерОбновления.Interval = 2500;
            else
                ТаймерОбновления.Interval = 5000;
            ТаймерОбновления.Enabled = true;
            ГлавноеОкно.ОчисткаМусора();
        }

        private void ФормаМонитора_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == 27)
                Close();
        }

        public void ЗаполнениеФормы()
        {
            int i = 0;
            foreach (ListViewItem Элемент in МассивСтрокаМонитора)
            {
                if (Элемент == null)
                    break;
                if (String.IsNullOrEmpty(Элемент.Text))
                    continue;
                i++;
            }

            if (i != СписокМонитора.Items.Count)
            {

                СписокМонитора.Items.Clear();
                try
                {
                    foreach (ListViewItem Элемент in МассивСтрокаМонитора)
                    {
                        if (String.IsNullOrEmpty(Элемент.Text))
                            continue;
                        СписокМонитора.Items.Add(Элемент);
                    }
                }
                catch
                {
                }

                if (СписокМонитора.Items.Count > 0)
                {
                    if (АктивнаяСтрока > СписокМонитора.Items.Count)
                        АктивнаяСтрока = 1;

                    if (АктивнаяСтрока > 0)
                    {
                        СписокМонитора.Items[АктивнаяСтрока - 1].Selected = true;
                    }
                    ТекКоличествоПользователей = "Количество активных пользователей: " + СписокМонитора.Items.Count;
                    СписокМонитора.Sort();
                }
                else
                    ТекКоличествоПользователей = "Нет активных пользователей";
            }
            else if (СписокМонитора.Items.Count == 0)
            {
                if ((ТекКоличествоПользователей == null) || (!ТекКоличествоПользователей.StartsWith("Err")))
                    ТекКоличествоПользователей = "Нет активных пользователей";
            }


        }

        private void ТаймерОбновления_Tick(object sender, EventArgs e)
        {
            if (!ВыполняетсяМониторинг)
            {
                ЗаполнениеФормы();
                Thread ФоновыйМониторинг = new Thread(Мониторинг);
                ФоновыйМониторинг.Start();

            }                        
            ТекстКоличествоПользователей.Text = ТекКоличествоПользователей;
            ГлавноеОкно.ОчисткаМусора();
        }

        private void СписокМонитора_MouseUp(object sender, MouseEventArgs e)
        {
            if (СписокМонитора.SelectedItems.Count != 0)
                АктивнаяСтрока = СписокМонитора.SelectedItems[0].Index + 1;
            else
                АктивнаяСтрока = 0;
        }

        private void ФормаМонитора_FormClosed(object sender, FormClosedEventArgs e)
        {
            ГлавноеОкно.ОчисткаМусора();
        }
    }
}