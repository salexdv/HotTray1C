using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Net;
using System.Windows.Forms;
using System.IO;
using System.Reflection;
using System.Diagnostics;
using System.Xml;

namespace TestMenuPopup
{
    public partial class ФормаОбновления : Form
    {
        private WebClient Клиент = new WebClient();
        private Boolean ФайлНайден = false;

        public ФормаОбновления()
        {
            InitializeComponent();
        }

        class UpdateInfo
        {
            public string Version;
            public List<String> Changes;

            public UpdateInfo()
            {
                Changes = new List<string>();
            }
        }

        private bool CurrentVersionIsOutdated(string CurentVersion, string Version)
        {
            CurentVersion = CurentVersion.Replace(".", "");
            int CurentAsInt = Convert.ToInt32(CurentVersion);

            Version = Version.Replace(".", "");
            int VersionAsInt = Convert.ToInt32(Version);

            return CurentAsInt < VersionAsInt;
        }

        public bool ЕстьОбновление(Boolean ПоказыватьОшибкуПодключения)
        {
            List<UpdateInfo> Updates = new List<UpdateInfo>();

            string ОписаниеНовойВерсии = String.Empty;
            
            String ТекущаяВерсия = Assembly.GetExecutingAssembly().GetName().Version.ToString();
            string ВерсияПрограммы = String.Empty;            

            ТекстТекущаяВерсия.Text = "Текущая версия: " + ТекущаяВерсия;
            Boolean ВерсияИзменилась = false;            
            try
            {
                Клиент.BaseAddress = "http://shkuraev.ru/HotTrayNET/test";
                XmlReader ReaderUpdateXML = XmlReader.Create("http://shkuraev.ru/HotTrayNET/test/update.xml");
                XmlDocument UpdateXML = new XmlDocument();
                UpdateXML.Load(ReaderUpdateXML);
                foreach (XmlNode Node in UpdateXML.DocumentElement.ChildNodes)
                {
                    string version = ((XmlElement)Node).GetAttribute("version");
                    if (CurrentVersionIsOutdated(ТекущаяВерсия, version))
                    {
                        if (!ВерсияИзменилась)
                        {
                            ВерсияИзменилась = true;
                            ВерсияПрограммы = version;
                        }

                        UpdateInfo ThisUpdate = new UpdateInfo();
                        ThisUpdate.Version = version;                        
                                           
                        XmlNode ChangesNode = Node.FirstChild;
                        foreach (XmlNode ChangeNode in ChangesNode.ChildNodes)
                        {
                            ThisUpdate.Changes.Add(ChangeNode.InnerText);
                        }

                        Updates.Add(ThisUpdate);
                    }                    
                }                
               
                if (ВерсияИзменилась)
                {
                    foreach (UpdateInfo Update in Updates)
                    {
                        ОписаниеНовойВерсии = ОписаниеНовойВерсии + Update.Version + "\r\n";
                        foreach (string Change in Update.Changes)
                        {
                            ОписаниеНовойВерсии = ОписаниеНовойВерсии + "  - " + Change + "\r\n";
                        }
                    }                    
                    try
                    {
                        Stream Поток = Клиент.OpenRead("HotTrayNET.zip");                        
                        ФайлНайден = true;
                    }
                    catch
                    {
                    }
                }
            }
            catch (Exception e)
            {
                if (ПоказыватьОшибкуПодключения)
                    ГлавноеОкно.ПоказатьИнфомационноеСообщение("Ошибка получения информации об обновлении. Err: " + e.Message, this);
            }

            if (String.IsNullOrEmpty(ОписаниеНовойВерсии))
                ОписаниеНовойВерсии = "Новых версий программы не обнаружено";
            
            ТекстовойПоле.Text = ОписаниеНовойВерсии;

            if (String.IsNullOrEmpty(ВерсияПрограммы))
                ТекстВесияНаСервере.Text = "Версия на сервере: " + ТекущаяВерсия;
            else
                ТекстВесияНаСервере.Text = "Версия на сервере: " + ВерсияПрограммы;

            return ВерсияИзменилась;
        }

        private void КнопкаЗакрыть_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void ФормаОбновления_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == 27)
                Close();
        }

        private void ФормаОбновления_FormClosed(object sender, FormClosedEventArgs e)
        {
            Клиент.Dispose();
        }

        private void Скачать_Click(object sender, EventArgs e)
        {
            if (!ФайлНайден)
                ГлавноеОкно.ПоказатьИнфомационноеСообщение("Не удалось обнаружить файл с программой на сервере.", this);
            else
            {
                ProcessStartInfo ПараметрыПроцесса = new ProcessStartInfo("explorer.exe");
                ПараметрыПроцесса.Arguments = "http://shkuraev.ru/HotTrayNET/HotTrayNET.zip";
                ПараметрыПроцесса.UseShellExecute = false;
                Process.Start(ПараметрыПроцесса);
                ГлавноеОкно.ПоказатьИнфомационноеСообщение("Загрузка файла скоро начнется!", this);  
            }
        }        

        private void СайтПрограммы_MouseClick(object sender, MouseEventArgs e)
        {
            try
            {
                Process.Start("http://shkuraev.ru/?p=7");
            }
            catch
            {
            }
        }        
    }
}
