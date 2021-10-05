using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using ToDo.API.Contracts;

namespace ToDo.API.Models
{
    public class ToDoListDto : IEntity
    {
        public Guid Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        public bool Important { get; set; } = false;
        
        public bool Archive { get; set; } = false;

        [Required]
        public Guid UserId { get; set; }

        public IEnumerable<ToDoItemDto> ToDos { get; set; }
    }
}
