using Diep.GameEngine.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Diep
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            for (int i = 0; i < 2; i++)
            {
                new Thread(() =>
                {
                    var messageFilter = new MessageFilter();
                    Application.AddMessageFilter(messageFilter);
                    Application.EnableVisualStyles();
                    Application.SetCompatibleTextRenderingDefault(false);
                    Application.Run(new GameUI(messageFilter, true));
                })
                {
                    ApartmentState = ApartmentState.STA
                }.Start();
            }
        }
    }
}
