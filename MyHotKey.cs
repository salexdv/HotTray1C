using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace TestMenuPopup
{
    public partial class MyHotKey : UserControl
    {
        public int КодПервогоСимвола;
        public int КодВторогоСимвола;
        public int КодТретьегоСимвола;
        public Boolean ГорячаяКлавишаВыбрана;

        public MyHotKey()
        {
            InitializeComponent();
            ГорячаяКлавишаВыбрана = false;
        }

        private void ПолеВвода_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {

        }

        public void ЗаполнитьСочетаниеКлавиш(string Сочетание)
        {                                                
            if (!String.IsNullOrEmpty(Сочетание))
            {
                string[] СтрокаКлавиш = Сочетание.Split('\\');
                int ПервыйСимвол = Convert.ToInt16(СтрокаКлавиш[0]);
                int ВторойСимвол = Convert.ToInt16(СтрокаКлавиш[1]);
                int ТретийСимвол = 0;
                try
                {
                    ТретийСимвол = Convert.ToInt16(СтрокаКлавиш[2]);
                }
                catch
                {}

                if ((ПервыйСимвол == 0) |(ВторойСимвол == 0))
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
                ВтораяКлавиша = ВтораяКлавиша.Replace("D", "");
                ВтораяКлавиша = ВтораяКлавиша.Replace("OEMTILDE", "`");

                ТретьяКлавиша = ТретьяКлавиша.Replace("D", "");
                ТретьяКлавиша = ТретьяКлавиша.Replace("Oemtilde", "`");

                ПолеВвода.Text = ПерваяКлавиша + " + " + ВтораяКлавиша;
                if (ТретийСимвол != 0) 
                    ПолеВвода.Text = ПолеВвода.Text + " + " + ТретьяКлавиша;
                
                ГорячаяКлавишаВыбрана = true;
                КодПервогоСимвола = ПервыйСимвол;
                КодВторогоСимвола = ВторойСимвол;
                КодТретьегоСимвола = ТретийСимвол;
            }                    
        }

        private void ПолеВвода_KeyDown(object sender, KeyEventArgs e)
        {
            string Сочетание = String.Empty;
            ГорячаяКлавишаВыбрана = false;
            КодПервогоСимвола = 0;
            КодВторогоСимвола = 0;
            КодТретьегоСимвола = 0;

            if ((e.Alt) || (e.Control) || (e.Shift))
            {
                if ((e.Modifiers & Keys.Alt) != 0)
                    Сочетание = "ALT + ";
                else if ((e.Modifiers & Keys.Control) != 0)
                    Сочетание = "CTRL + ";
                else if ((e.Modifiers & Keys.Shift) != 0)
                    Сочетание = "SHIFT + ";

                Сочетание = Сочетание + e.KeyCode.ToString();
                Сочетание = Сочетание.Replace("ALT + Menu", "ALT + ");
                
                Сочетание = Сочетание.Replace("ControlKey", "CTRL");
                Сочетание = Сочетание.Replace("ShiftKey", "SHIFT");

                Сочетание = Сочетание.Replace("CTRL + CTRL", "CTRL + ");
                Сочетание = Сочетание.Replace("SHIFT + SHIFT", "SHIFT + ");

                Сочетание = Сочетание.Replace("CTRL + SHIFT", "CTRL + SHIFT + ");
                Сочетание = Сочетание.Replace("ALT + SHIFT", "ALT + SHIFT + ");
                

                if (Сочетание.Contains("Bac"))
                    Сочетание = Сочетание.Replace("Bac", "");
                else if (Сочетание.Contains("Space"))
                    Сочетание = Сочетание.Replace("Space", "");
                else if (Сочетание.Contains("Left"))
                    Сочетание = Сочетание.Replace("Left", "");
                else if (Сочетание.Contains("Right"))
                    Сочетание = Сочетание.Replace("Right", "");
                else if (Сочетание.Contains("Up"))
                    Сочетание = Сочетание.Replace("Up", "");
                else if (Сочетание.Contains("Down"))
                    Сочетание = Сочетание.Replace("Down", "");
                else if (Сочетание.Contains("Home"))
                    Сочетание = Сочетание.Replace("Home", "");
                else if (Сочетание.Contains("Insert"))
                    Сочетание = Сочетание.Replace("Insert", "");
                else if (Сочетание.Contains("Delete"))
                    Сочетание = Сочетание.Replace("Delete", "");
                else if (Сочетание.Contains("End"))
                    Сочетание = Сочетание.Replace("End", "");
                else if (Сочетание.Contains("Next"))
                    Сочетание = Сочетание.Replace("Next", "");
                else if (Сочетание.Contains("Pause"))
                    Сочетание = Сочетание.Replace("Pause", "");
                else if (Сочетание.Contains("Scrool"))
                    Сочетание = Сочетание.Replace("Scrool", "");
                else if (Сочетание.Contains("+ H"))
                    Сочетание = Сочетание.Replace("+ H", "+ ");
                else if (Сочетание.Contains("+ Oemtilde"))
                    Сочетание = Сочетание.Replace("+ Oemtilde", "+ `");                

                if ((Сочетание != "SHIFT + ") & (Сочетание != "ALT + ") & (Сочетание != "CTRL + ") & (Сочетание != "CTRL + SHIFT + ") & (Сочетание != "ALT + SHIFT + "))
                {
                    if ((e.Alt) | (e.Control) | (e.Shift))
                    {
                        int НачалоПоследнегоСимвола = Сочетание.IndexOf("D");

                        string НовоеСочетание = String.Empty;

                        Сочетание = Сочетание.Replace("+", "");
                        Сочетание = Сочетание.Replace("ALT", "");
                        Сочетание = Сочетание.Replace("CTRL", "");
                        Сочетание = Сочетание.Replace("SHIFT", "");
                        Сочетание = Сочетание.Replace("D", "");
                        Сочетание = Сочетание.Replace(" ", "");

                        if (e.Alt)
                        {
                            КодПервогоСимвола = 18;
                            НовоеСочетание = "ALT + ";
                            if (e.Shift)
                            {
                                КодВторогоСимвола = 16;
                                НовоеСочетание = НовоеСочетание + "SHIFT + ";
                            }
                        }
                        else if (e.Control)
                        {
                            КодПервогоСимвола = 17;
                            НовоеСочетание = "CTRL + ";
                            if (e.Shift)
                            {
                                КодВторогоСимвола = 16;
                                НовоеСочетание = НовоеСочетание + "SHIFT + ";
                            }
                        }
                        else
                        {
                            КодПервогоСимвола = 16;
                            НовоеСочетание = "SHIFT + ";
                        }

                        if (КодВторогоСимвола != 0)
                            КодТретьегоСимвола = (int)e.KeyValue;
                        else
                            КодВторогоСимвола = (int)e.KeyValue;

                        Сочетание = НовоеСочетание + Сочетание;                        
                        
                        ГорячаяКлавишаВыбрана = true;                        
                    }
                }                

            }
            else
                Сочетание = "НЕТ";
          
            ПолеВвода.Text = Сочетание;            
            ПолеВвода.SelectionStart = Сочетание.Length;            
        }

        private void ПолеВвода_KeyUp(object sender, KeyEventArgs e)
        {
            string Сочетание = ПолеВвода.Text;
            if ((Сочетание == "SHIFT + ") | (Сочетание == "ALT + ") | (Сочетание == "CTRL + ") | (Сочетание == "CTRL + SHIFT + ") | (Сочетание == "ALT + SHIFT + "))            
                ПолеВвода.Text = "НЕТ";
        }

        private void ПолеВвода_TextChanged(object sender, EventArgs e)
        {
            if (ПолеВвода.Text == "НЕ")
                ПолеВвода.Text = "НЕТ";
        }
    }
}
