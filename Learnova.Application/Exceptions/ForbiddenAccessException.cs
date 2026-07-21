namespace Learnova.Application.Exceptions
{
    public class ForbiddenAccessException : BaseException
    {
        public ForbiddenAccessException(string message) : base(message, 403)
        {
        }
    }
}
