using System;
using System.Collections.Generic;
using ToDo.API.Entities;
using ToDo.API.Models;

namespace ToDo.API.Services
{
    public interface IToDoListRepository : IRepositoryBase<ToDoList>
    {
        IEnumerable<ToDoList> GetToDoLists(Guid userId, ToDoListsParameters toDoListsParameters);
        void UpdateToDoListBulk(IEnumerable<ToDoList> toDoLists);
    }
}
