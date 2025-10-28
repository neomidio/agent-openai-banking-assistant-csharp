
public class LoggedUserService : IUserService
{
    public LoggedUser GetLoggedUser()
    {
        return GetDefaultUser();
    }

    private LoggedUser GetDefaultUser()
    {
        return new LoggedUser("carlos.usuario@contoso.com", "carlos.usuario@contoso.com", "generic", "Carlos El Usuario");
    }
}

;