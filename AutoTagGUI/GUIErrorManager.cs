using System.Windows.Forms;
using AutoTagLib.ErrorManager;
using System.Linq;
using System.Threading;
using System.Collections.Generic;
using System.IO;
using System;

namespace AutoTagGUI
{
    public class GUIErrorManager : ErrorManager
    {
        private List<Thread> _errorThreads;
        private Dictionary<string, List<string>> _musicRelatedErrors;
        private StreamWriter _writer;
        private string _writerFileName;

        private GUIErrorManager() : base()
        {
            _errorThreads = new List<Thread>();
            _musicRelatedErrors = new Dictionary<string, List<string>>();
            _writerFileName = $"logs{Path.DirectorySeparatorChar}Errors_{DateTime.Now.ToString("yyyyMMddHHmm")}.txt";
            _writer = new StreamWriter(_writerFileName) { AutoFlush = true };
        }

        public static IErrorManager GetInstance()
        {
            return _instance ?? (_instance = new GUIErrorManager());
        }

        public void ClearErrorManager()
        {
            _errorThreads = new List<Thread>();
            _musicRelatedErrors = new Dictionary<string, List<string>>();
        }

        public void WaitErrorManager()
        {
            foreach(Thread thread in _errorThreads)
            {
                thread.Join();
            }

            foreach(string errorType in _musicRelatedErrors.Keys)
            {
                _musicRelatedErrors.TryGetValue(errorType, out List<string> errorList);
                _writer.WriteLine($"{errorType} ({errorList.Count} fichiers)");
                foreach (string file in errorList)
                {
                    _writer.WriteLine(file);
                }
                MessageBox.Show($"{errorType}\nCette erreur concerne {errorList.Count} fichiers. Voir le fichier {_writerFileName} pour plus d'informations.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        protected override void ShowError(ErrorCodes code, string message)
        {
            _errorThreads.Add(new Thread(() => {
                DisplayErrorMessage($"Erreur : {code}. \n{message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }));
            _errorThreads.Last().Start();
        }

        protected override void ShowError(ErrorCodes code, string message, string musicFile)
        {
            if (_musicRelatedErrors.TryGetValue($"Erreur : {code}. \n{message}", out List<string> fileList))
            {
                fileList.Add(musicFile);
            } else
            {
                _musicRelatedErrors.Add($"Erreur : {code}. \n{message}", new List<string>() { musicFile });
            }
        }

        private void DisplayErrorMessage(string msg, string title, MessageBoxButtons buttons, MessageBoxIcon icon)
        {
            MessageBox.Show(msg, title, buttons, icon);
        }
    }
}