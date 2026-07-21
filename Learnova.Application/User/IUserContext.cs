namespace Learnova.Application.User
{
    public interface IUserContext
    {
        CurrentUser? GetCurrentUser();
    }
}
