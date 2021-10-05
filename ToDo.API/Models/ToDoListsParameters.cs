namespace ToDo.API.Models
{
    public class ToDoListsParameters
    {
        public string Name { get; set; }

        public bool? Important { get; set; }

        public bool? Archive { get; set; }
    }
}
