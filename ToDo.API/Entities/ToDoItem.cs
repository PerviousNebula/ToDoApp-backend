using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ToDo.API.Contracts;
using ToDo.API.Enums;

namespace ToDo.API.Entities
{
    public class ToDoItem : IEntity
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Title { get; set; }

        public bool Archive { get; set; } = false;

        [Required]
        [EnumDataType(typeof(Status))]
        public Status StatusId { get; set; }


        [ForeignKey("ToDoListId")]
        public Guid ToDoListId { get; set; }

        public ToDoList ToDoList { get; set; }
    }
}
