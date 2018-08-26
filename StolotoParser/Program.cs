using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using StolotoParser.Service;

namespace StolotoParser
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

            MainForm form = new MainForm();
            IMessageService service = new MessageService();
            IFileManager manager = new FileManager();

            Manager presenter = new Manager(form, manager, service);

            Application.Run(form);
        }
    }
}
