using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ToDo.API.Entities;

namespace ToDo.API.Services
{
    public interface IToDoItemRepository : IRepositoryBase<ToDoItem>
    { }
}
