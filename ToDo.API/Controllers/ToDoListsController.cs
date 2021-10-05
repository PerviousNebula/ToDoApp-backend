using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using ToDo.API.ActionFilters;
using ToDo.API.Entities;
using ToDo.API.Models;
using ToDo.API.Services;

namespace ToDo.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class ToDoListsController : ControllerBase
    {
        private readonly IToDoListRepository _toDoListRepository;
        private readonly IMapper _mapper;

        public ToDoListsController
        (
            IToDoListRepository toDoListRepository,
            IMapper mapper
        )
        {
            _toDoListRepository = toDoListRepository ?? throw new ArgumentNullException(nameof(toDoListRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        [HttpGet("{id:guid}/ToDoItems")]
        [ServiceFilter(typeof(ValidateEntityExistsAttribute<ToDoList>))]
        public ActionResult<ToDoListDto> GetSingleWithToDos(Guid id)
        {
            var toDoList = _toDoListRepository.FindByCondition(tl => tl.Id == id)
                                .Include(tl => tl.ToDos)
                                .FirstOrDefault();

            return Ok(_mapper.Map<ToDoListDto>(toDoList));
        }

        [HttpGet("{toDoListId:guid}/ToDoItem/{id:guid}", Name = "GetToDoItem")]
        [ServiceFilter(typeof(ValidateEntityExistsAttribute<ToDoItem>))]
        public ActionResult<ToDoItemDto> GetSingleToDoItem(Guid toDoListId, Guid id)
        {
            var toDoItem = HttpContext.Items["entity"] as ToDoItem;

            return Ok(_mapper.Map<ToDoItemDto>(toDoItem));
        }

        [HttpPost]
        [ServiceFilter(typeof(ValidationFilterAttribute))]
        public ActionResult<ToDoListDto> Post(ToDoListDto model)
        {
            var toDoList = _mapper.Map<ToDoList>(model);

            _toDoListRepository.Create(toDoList);
            _toDoListRepository.Save();

            return CreatedAtAction
            (
                "GetSingleToDoList",
                "Users",
                new { userId = toDoList.UserId, id = toDoList.Id },
                toDoList
            );
        }

        [HttpPut("{id:guid}")]
        [ServiceFilter(typeof(ValidationFilterAttribute))]
        [ServiceFilter(typeof(ValidateEntityExistsAttribute<ToDoList>))]
        public ActionResult Put(Guid id, ToDoListDto model)
        {
            var toDoList = HttpContext.Items["entity"] as ToDoList;

            _mapper.Map(model, toDoList);

            _toDoListRepository.Update(toDoList);
            _toDoListRepository.Save();

            _mapper.Map(toDoList, model);

            return NoContent();
        }

        [HttpPut]
        public ActionResult Put(List<ToDoListDto> model)
        {
            if (model == null || model.Count == 0)
            {
                return BadRequest();
            }

            var toDoLists = _mapper.Map<IEnumerable<ToDoList>>(model);

            _toDoListRepository.UpdateToDoListBulk(toDoLists);
            _toDoListRepository.Save();

            return NoContent();
        }
        
        [HttpDelete("{id:guid}")]
        [ServiceFilter(typeof(ValidateEntityExistsAttribute<ToDoList>))]
        public ActionResult Delete(Guid id)
        {
            var toDoList = HttpContext.Items["entity"] as ToDoList;

            _toDoListRepository.Delete(toDoList);
            _toDoListRepository.Save();

            return NoContent();
        }
    }
}
