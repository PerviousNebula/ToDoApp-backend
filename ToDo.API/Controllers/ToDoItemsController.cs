using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using ToDo.API.ActionFilters;
using ToDo.API.Entities;
using ToDo.API.Models;
using ToDo.API.Services;

namespace ToDo.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class ToDoItemsController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IToDoItemRepository _toDoItemRepository;

        public ToDoItemsController
        (
            IToDoItemRepository toDoItemRepository,
            IMapper mapper
        )
        {
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _toDoItemRepository = toDoItemRepository ?? throw new ArgumentNullException(nameof(toDoItemRepository));
        }

        [HttpPost]
        public ActionResult<ToDoItemDto> Post([FromBody] ToDoItemDto model)
        {
            var toDoItem = _mapper.Map<ToDoItem>(model);

            _toDoItemRepository.Create(toDoItem);
            _toDoItemRepository.Save();

            return CreatedAtAction
            (
                "GetSingleToDoItem",
                "ToDoLists",
                new { toDoListId = toDoItem.ToDoListId, id = toDoItem.Id },
                toDoItem
            );
        }

        [HttpPut("{id:guid}")]
        [ServiceFilter(typeof(ValidationFilterAttribute))]
        [ServiceFilter(typeof(ValidateEntityExistsAttribute<ToDoItem>))]
        public ActionResult Put(Guid id, ToDoItemDto model)
        {
            var toDoItem = HttpContext.Items["entity"] as ToDoItem;

            _mapper.Map(model, toDoItem);

            _toDoItemRepository.Update(toDoItem);
            _toDoItemRepository.Save();

            return NoContent();
        }
    
        [HttpPut]
        public ActionResult Put(IEnumerable<ToDoItemDto> model)
        {
            var toDoItems = _mapper.Map<IEnumerable<ToDoItem>>(model);
            foreach (var t in toDoItems)
            {
                _toDoItemRepository.Update(t);
            }
            _toDoItemRepository.Save();

            return NoContent();
        }
    
        [HttpDelete("{id:guid}")]
        [ServiceFilter(typeof(ValidateEntityExistsAttribute<ToDoItem>))]
        public ActionResult Delete(Guid id)
        {
            var toDoItem = HttpContext.Items["entity"] as ToDoItem;

            _toDoItemRepository.Delete(toDoItem);
            _toDoItemRepository.Save();

            return NoContent();
        }
    }
}
