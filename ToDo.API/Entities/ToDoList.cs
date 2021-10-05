using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ToDo.API.Contracts;

namespace ToDo.API.Entities
{
    public class ToDoList : IEntity
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        public bool Important { get; set; } = false;

        public bool Archive { get; set; } = false;

        [ForeignKey("UserId")]
        public Guid UserId { get; set; }

        public ICollection<ToDoItem> ToDos { get; set; } = new List<ToDoItem>();
    }
}
