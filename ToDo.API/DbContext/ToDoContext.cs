using ToDo.API.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ToDo.API.DbContexts
{
    public class ToDoContext : DbContext
    {
        public ToDoContext(DbContextOptions<ToDoContext> options) : base(options)
        { }

        public DbSet<ToDoItem> ToDoItems { get; set;  }
        public DbSet<ToDoList> ToDoLists { get; set;  }
        public DbSet<User> Users { get; set;  }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ToDoList>().ToTable("ToDoList");
            modelBuilder.Entity<ToDoItem>().ToTable("ToDoItem");
            modelBuilder.Entity<User>().ToTable("User");

            base.OnModelCreating(modelBuilder);
        }
    }
}
