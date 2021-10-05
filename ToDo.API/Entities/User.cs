using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ToDo.API.Contracts;

namespace ToDo.API.Entities
{
    public class User : IEntity
    {
        [Key]
        public Guid Id { get; set; }

        [StringLength(50)]
        public string FirstName { get; set; }

        [StringLength(100)]
        public string LastName { get; set; }

        [Required]
        [EmailAddress]
        [StringLength(200)]
        public string Email { get; set; }

        public byte[] Hash { get; set; }

        public byte[] Salt { get; set; }

        public bool Facebook { get; set; } = false;

        public bool Google { get; set; } = false;

        public bool Archive { get; set; } = false;

        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime UpdatedAt { get; set; } = DateTime.Now;

        public ICollection<ToDoList> ToDoLists { get; set; } = new List<ToDoList>();
    }
}
