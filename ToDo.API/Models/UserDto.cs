using System;
using System.ComponentModel.DataAnnotations;
using ToDo.API.Contracts;

namespace ToDo.API.Models
{
    public class UserDto : IEntity
    {
        public Guid Id { get; set; }
        
        public string FirstName { get; set; }
        
        public string LastName { get; set; }
        
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        
        public string Password { get; set; }
        
        public bool Facebook { get; set; }
        
        public bool Google { get; set; }
        
        public bool Archive { get; set; }
        
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        
        public DateTime UpdatedAt { get; set; } = DateTime.Now;
    }
}
