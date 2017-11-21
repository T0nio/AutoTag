namespace AutoTagLib.ErrorManager
{
    public interface IErrorManager
    {
        void NewError(ErrorCodes errorCode);
        void NewError(ErrorCodes errorCode, string fileName);
    }
}