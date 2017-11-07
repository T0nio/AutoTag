using System;

namespace AutoTagLib.ErrorManager
{
    public interface IErrorManager
    {
        void NewError(errorCodes errorCode);
    }
}