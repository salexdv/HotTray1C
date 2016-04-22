using System;
using System.Collections.Generic;
using System.Threading;
using System.Windows.Forms;

namespace TestMenuPopup
{
    static class Program
    {
        /// <summary>
        /// Главная точка входа для приложения.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            
            bool onlyInstance;
            
            Mutex mtx = new Mutex(true, "Hot tray 1C .NET", out onlyInstance);
    
            // Если другие процессы не владеют мьютексом, то
            // приложение запущено в единственном экземпляре
            if (onlyInstance)
            {
                Application.Run(new ГлавноеОкно());
            }            
        }
    }
}
