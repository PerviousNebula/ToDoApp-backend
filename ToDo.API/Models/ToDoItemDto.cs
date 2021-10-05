using System;
using System.ComponentModel.DataAnnotations;
using ToDo.API.Contracts;
using ToDo.API.Enums;

namespace ToDo.API.Models
{
    public class ToDoItemDto : IEntity
    {
        public Guid Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Title { get; set; }

        public bool Archive { get; set; } = false;

        public Guid ToDoListId { get; set; }

        [Required]
        [EnumDataType(typeof(Status))]
        public Status StatusId { get; set; }
    }
}
