using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using ToDo.API.Entities;

namespace ToDo.API.Services
{
    public interface IUserRepository : IRepositoryBase<User>
    { }
}
