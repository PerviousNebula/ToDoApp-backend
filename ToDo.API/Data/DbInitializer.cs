using ToDo.API.Entities;
using System;
using System.Linq;
using ToDo.API.DbContexts;

namespace ToDo.API.Data
{
    public static class DbInitializer
    {
        public static void Initialize(ToDoContext context)
        {
            context.Database.EnsureCreated();

            // Look for any ToDoLists.
            if (context.ToDoLists.Any())
            {
                return;   // DB has been seeded
            }
            
            //var toDoLists = new ToDoList[]
            //{
            //new ToDoList
            //    {
            //        Id = Guid.Parse("d28888e9-2ba9-473a-a40f-e38cb54f9b35"),
            //        Name = "School",
            //        Archive = false
            //    }
            //};

            //foreach (var tl in toDoLists)
            //{
            //    context.ToDoLists.Add(tl);
            //}
            //context.SaveChanges();
        }
    }
}