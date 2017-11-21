using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;

namespace AutoTagLib.ErrorManager
{
    public enum ErrorCodes
    {
        unknown, acr_timeout, acr_unknown, acr_decode_audio, acr_dll, acr_invalid_host, acr_invalid_key, acr_invalid_secret, id3v2_not_supported, invalid_encoding, acr_limit_reached
    }

    public abstract class ErrorManager : IErrorManager
    {
        protected static IErrorManager _instance;
        private Dictionary<ErrorCodes, string> _errors = new Dictionary<ErrorCodes, string>();

        public ErrorManager()
        {
            _errors.Add(ErrorCodes.acr_timeout, "Un problème réseau est survenu. Merci de vérifier votre connexion internet.");
            _errors.Add(ErrorCodes.acr_unknown, "Un problème est survenu lors de la reconnaissance audio des titres.");
            _errors.Add(ErrorCodes.acr_decode_audio, "Un problème est survenu lors de la lecture d'un des titres.");
            _errors.Add(ErrorCodes.acr_dll, "Un problème est survenue avec la DLL ACRCloud. Verifiez qu'elle est bien présent dans le repertoire courant (libacrcloud_extr_tool.dll/libacrcloud_extr_tool.so)");
            _errors.Add(ErrorCodes.acr_invalid_host, "Un problème est survenuelors de la reconnaissance audio des titres : aucun hôte spécifié.");
            _errors.Add(ErrorCodes.acr_invalid_key, "Un problème est survenu lors de la reconnaissance audio des titres : votre clé est manquante ou invalide.");
            _errors.Add(ErrorCodes.acr_invalid_secret, "Un problème est survenu lors de la reconnaissance audio des titres : votre signature est manquante ou invalide.");
            _errors.Add(ErrorCodes.acr_limit_reached, "Un problème est survenu lors de la reconnaissance audio des titres : vous avez atteint votre limite d'appels à l'API ACR.");
            _errors.Add(ErrorCodes.id3v2_not_supported, "Un problème est survenu lors du chargement. Les tags ID3V2 ne sont pas supportés.");
            _errors.Add(ErrorCodes.invalid_encoding, "Un problème d'encoding est survenue lors du chargement.");
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

        public void NewError(ErrorCodes errorCode,  string fileName)
        {
            try
            {
                _errors.TryGetValue(errorCode, out string msg);
                ShowError(errorCode, $"{msg}\nFichier concerné : {fileName}");
            }
            catch (Exception)
            {
                ShowError(ErrorCodes.unknown, "Erreur inconnue");
            }
        }

        protected abstract void ShowError(ErrorCodes code, string message);
    }
}