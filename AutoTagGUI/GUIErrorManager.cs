using System;
using System.Windows.Forms;
using AutoTagLib;
using AutoTagLib.ErrorManager;

namespace AutoTagGUI
{
    public class GUIErrorManager: ErrorManager
    {
        public static IErrorManager GetInstance()
        {
            return _instance ?? (_instance = new GUIErrorManager());
        }

        protected override void ShowError(ErrorCodes code, string message)
        {
            MessageBox.Show($"{code}: {message}");
        }
    }
}