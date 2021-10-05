using AutoMapper;

namespace ToDo.API.Profiles
{
    public class ToDoListsProfile : Profile
    {
        public ToDoListsProfile()
        {
            CreateMap<Entities.ToDoList, Models.ToDoListDto>();
            CreateMap<Models.ToDoListDto, Entities.ToDoList>();
        }
    }
}
