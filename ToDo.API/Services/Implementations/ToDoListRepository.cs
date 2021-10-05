using System;
using System.Collections.Generic;
using System.Linq;
using ToDo.API.DbContexts;
using ToDo.API.Entities;
using ToDo.API.Models;
using ToDo.API.Services.Implementations;

namespace ToDo.API.Services
{
    public class ToDoListRepository : RepositoryBase<ToDoList>, IToDoListRepository
    {
        private readonly ToDoContext _context;

        public ToDoListRepository(ToDoContext context) : base(context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public IEnumerable<ToDoList> GetToDoLists(Guid userId, ToDoListsParameters queryParams)
        {
            var toDoLists = FindByCondition(tl => tl.UserId == userId);

            SearchByName(ref toDoLists, queryParams.Name);
            SearchByImportant(ref toDoLists, queryParams.Important);
            SearchByArchive(ref toDoLists, queryParams.Archive);

            return toDoLists;
        }

        private void SearchByName(ref IQueryable<ToDoList> toDoLists, string? name)
        {
            if (name == null || string.IsNullOrEmpty(name) || !name.Any())
            {
                return;
            }

            toDoLists = toDoLists.Where(tl => tl.Name.ToLower().Contains(name.Trim().ToLower()));
        }

        private void SearchByImportant(ref IQueryable<ToDoList> toDoLists, bool? important)
        {
            if (important == null)
            {
                return;
            }

            toDoLists = toDoLists.Where(tl => tl.Important == important);
        }
        
        private void SearchByArchive(ref IQueryable<ToDoList> toDoLists, bool? archive)
        {
            if (archive == null)
            {
                return;
            }

            toDoLists = toDoLists.Where(tl => tl.Archive == archive);
        }

        public void UpdateToDoListBulk(IEnumerable<ToDoList> toDoLists)
        {
            _context.UpdateRange(toDoLists);
        }
    }
}
