using System;
using System.Linq;
using ToDo.API.DbContexts;
using ToDo.API.Entities;
using ToDo.API.Services.Implementations;

namespace ToDo.API.Services
{
    public class UserRepository : RepositoryBase<User>, IUserRepository
    {
        private readonly ToDoContext _context;

        public UserRepository(ToDoContext context) : base(context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public static void CreatePasswordHash(string password, out byte[] hash, out byte[] salt)
        {
            if (String.IsNullOrEmpty(password))
            {
                throw new ArgumentNullException(nameof(password));
            }

            using (var hmac = new System.Security.Cryptography.HMACSHA512())
            {
                salt = hmac.Key;
                hash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            }
        }

        public static bool VerifyPassword(string password, byte[] passwordHash, byte[] passwordSalt)
        {
            using (var hmac = new System.Security.Cryptography.HMACSHA512(passwordSalt))
            {
                var newPasswordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
                return new ReadOnlySpan<byte>(passwordHash).SequenceEqual(new ReadOnlySpan<byte>(newPasswordHash));
            }
        }

    }
}
