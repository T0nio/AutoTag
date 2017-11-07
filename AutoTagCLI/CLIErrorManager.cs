using System;
using AutoTagLib;
using AutoTagLib.ErrorManager;

namespace AutoTagCLI
{
    public class CLIErrorManager: ErrorManager
    {
        public static IErrorManager GetInstance()
        {
            return _instance ?? (_instance = new CLIErrorManager());
        }


        protected override void showError(errorCodes code, string message)
        {
            Console.WriteLine($"Erreur {(int)code}: {message}");
        }
    }
}