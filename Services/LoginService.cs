using demo2.Helpers;
using demo2.Models;
using Microsoft.EntityFrameworkCore;

namespace demo2.Services
{
    public class LoginService
    {
        private readonly DemoDb2Context _context;

        public LoginService(DemoDb2Context context)
        {
            _context = context;
        }

        private User? GetUserByLoginAndPassword(string login, string password)
        {
            return _context.Users
                .Include(u => u.IdPersonNavigation)
                .Include(u => u.IdUserRoleNavigation)
                .FirstOrDefault(u => u.Login == login && u.Password == password);
        }

        public UserContext? Login(string login, string password, bool isGuest)
        {
            if (isGuest)
            {
                UserContext userContext = new()
                {
                    Id = 0,
                    UserName = "Гость",
                    Role = "Гость"
                };

                return userContext;
            }
            else
            {
                var user = GetUserByLoginAndPassword(login, password);

                if (user == null) return null;

                UserContext userContext = new()
                {
                    Id = user.IdUser,
                    UserName = $"{user.IdPersonNavigation.Surname} {user.IdPersonNavigation.FirstName} {user.IdPersonNavigation.Patronymic}",
                    Role = user.IdUserRoleNavigation.Name
                };

                return userContext;
            }

        }
    }
}
