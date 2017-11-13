using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;

namespace AutoTagLib.ErrorManager
{
    public enum ErrorCodes
    {
        unknown, acr_timeout, acr_unknown, acr_decode_audio, acr_dll
    }


    public abstract class ErrorManager : IErrorManager
    {
        protected static IErrorManager _instance;
        private Dictionary<ErrorCodes, string> _errors = new Dictionary<ErrorCodes, string>();

        public ErrorManager()
        {
            _errors.Add(ErrorCodes.acr_timeout, "Un problème réseau est survenu. Merci de vérifier votre connexion internet.");
            _errors.Add(ErrorCodes.acr_unknown, "Un problème est survenue lors de la reconnaissance audio des titres.");
            _errors.Add(ErrorCodes.acr_decode_audio, "Un problème est survenue lors de la lecture d'un des titres.");
            _errors.Add(ErrorCodes.acr_dll, "Un problème est survenue avec la DLL ACRCloud. Verifiez qu'elle est bien présent dans le repertoire courant (libacrcloud_extr_tool.dll/libacrcloud_extr_tool.so)");
        }
        
        public void NewError(ErrorCodes errorCode)
        {
            try
            {
                _errors.TryGetValue(errorCode, out string msg);
                ShowError(errorCode, msg);
            }
            catch (Exception)
            {
                ShowError(ErrorCodes.unknown, "Erreur inconnue");
            }

        }

        protected abstract void ShowError(ErrorCodes code, string message);
    }
}