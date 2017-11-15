using System.Windows.Forms;
using AutoTagLib.ErrorManager;
using System.Linq;
using System.Threading;
using System.Collections.Generic;

namespace AutoTagGUI
{
    public class GUIErrorManager: ErrorManager
    {
        public List<Thread> ErrorThreads { get; set; }

        private GUIErrorManager() : base()
        {
            ErrorThreads = new List<Thread>();
        }

        public static IErrorManager GetInstance()
        {
            return _instance ?? (_instance = new GUIErrorManager());
        }

        public void ClearErrorManager()
        {
            ErrorThreads = new List<Thread>();
        }

        public void WaitErrorManager()
        {
            foreach(Thread thread in ErrorThreads)
            {
                thread.Join();
            }
        }

        protected override void ShowError(ErrorCodes code, string message)
        {
            ErrorThreads.Add(new Thread(() => {
                DisplayErrorMessage($"{code} : {message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }));
            ErrorThreads.Last().Start();
        }

        private void DisplayErrorMessage(string msg, string title, MessageBoxButtons buttons, MessageBoxIcon icon)
        {
            MessageBox.Show(msg, title, buttons, icon);
        }
    }
}