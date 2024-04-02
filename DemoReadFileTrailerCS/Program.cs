using DemoReadFileTrailerCS_Automation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DemoReadFileTrailerCS_AutomationCS
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {

            string arg = "0";
            if (args.Any())
                arg = args[0];
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Console.WriteLine(arg);
            Application.Run(new Form1(Convert.ToInt16(arg)));
        }
    }
}
