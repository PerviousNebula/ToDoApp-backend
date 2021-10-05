using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ToDo.API.DbContexts;
using ToDo.API.Entities;

namespace ToDo.API.Services.Implementations
{
    public class ToDoItemRepository : RepositoryBase<ToDoItem>, IToDoItemRepository
    {
        private readonly ToDoContext _context;

        public ToDoItemRepository(ToDoContext context) : base(context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }
    }
}
