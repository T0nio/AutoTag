using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;

namespace AutoTagLib.ErrorManager
{
    public enum errorCodes
    {
        unknown, acr_timeout, acr_unknown, acr_decode_audio
    }
    public abstract class ErrorManager:IErrorManager
    {
        protected static IErrorManager _instance;
        private Dictionary<errorCodes, string> _errors = new Dictionary<errorCodes, string>();

        public ErrorManager()
        {
            _errors.Add(errorCodes.acr_timeout, "Un problème réseau est survenu. Merci de vérifier votre connexion internet.");
            _errors.Add(errorCodes.acr_unknown, "Un problème est survenue lors de la reconnaissance audio des titres.");
            _errors.Add(errorCodes.acr_decode_audio, "Un problème est survenue lors de la lecture d'un des titres.");

        }
        
        public void NewError(errorCodes errorCode)
        {
            string msg;
            try
            {
                _errors.TryGetValue(errorCode, out msg);
                showError(errorCode, msg);
            }
            catch (Exception e)
            {
                showError(errorCodes.unknown, "Erreur inconnue");
            }
            
        }

        protected abstract void showError(errorCodes code, string message);
    }
}