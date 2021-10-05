using AutoMapper;

namespace ToDo.API.Profiles
{
    public class ToDoItemProfile : Profile
    {
        public ToDoItemProfile()
        {
            CreateMap<Entities.ToDoItem, Models.ToDoItemDto>();
            CreateMap<Models.ToDoItemDto, Entities.ToDoItem>();
        }
    }
}
