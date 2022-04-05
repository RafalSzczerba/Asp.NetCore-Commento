namespace Commento.Enums
{
    public enum LoginStatus
    {
        ProperHmacVerification,
        InvalidHmac,
        InvalidToken,
        WrongLoginData,
        Logged,
    }
}
