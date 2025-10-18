using ASI.Basecode.Data;
using ASI.Basecode.Data.Interfaces;
using ASI.Basecode.Data.Models;
using ASI.Basecode.Services.Interfaces;
using ASI.Basecode.Services.Manager;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using static ASI.Basecode.Resources.Constants.Enums;

namespace ASI.Basecode.Services.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _repository;
        private readonly NexDeskDbContext _nexDeskContext;
        private readonly IMapper _mapper;

        public UserService(IUserRepository repository, NexDeskDbContext nexDeskContext, IMapper mapper)
        {
            _mapper = mapper;
            _repository = repository;
            _nexDeskContext = nexDeskContext;
        }

        public LoginResult AuthenticateUser(string userId, string password, ref User user)
        {
            user = new User();
            var passwordKey = PasswordManager.EncryptPassword(password);
            user = _repository.GetUsers().Where(x => x.UserId == userId &&
                                                     x.Password == passwordKey).FirstOrDefault();

            return user != null ? LoginResult.Success : LoginResult.Failed;
        }

        public async Task<User> RetrieveUser(string userId, string password)
        {
            try
            {
                // For development, we'll check password as plain text
                // In production, you should hash the password and compare hashes
                var user = await _nexDeskContext.Users
                    .Where(x => (x.Username == userId || x.Email == userId || x.UserId == userId) && 
                                x.Password == password)
                    .FirstOrDefaultAsync();

                return user;
            }
            catch
            {
                return null;
            }
        }
    }
}
