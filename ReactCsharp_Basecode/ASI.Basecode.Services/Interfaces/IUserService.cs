using ASI.Basecode.Data.Models;
using System.Threading.Tasks;
using static ASI.Basecode.Resources.Constants.Enums;

namespace ASI.Basecode.Services.Interfaces
{
    public interface IUserService
    {
        LoginResult AuthenticateUser(string userid, string password, ref User user);
        Task<User> RetrieveUser(string userId, string password);
    }
}
