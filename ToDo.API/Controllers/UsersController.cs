using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using ToDo.API.ActionFilters;
using ToDo.API.Entities;
using ToDo.API.Models;
using ToDo.API.Services;

namespace ToDo.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly IUserRepository _userRepository;
        private readonly IToDoListRepository _toDoListRepository;
        private readonly IMapper _mapper;

        public UsersController
        (
            IUserRepository userRepository,
            IToDoListRepository toDoListRepository,
            IMapper mapper
        )
        {
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(_userRepository));
            _toDoListRepository = toDoListRepository ?? throw new ArgumentNullException(nameof(_toDoListRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        [HttpGet("{userId:guid}/ToDoLists")]
        public IActionResult GetToDoLists(Guid userId, [FromQuery] ToDoListsParameters queryParams)
        {
            var toDoLists = _toDoListRepository.GetToDoLists(userId, queryParams);

            return Ok(_mapper.Map<IEnumerable<ToDoList>>(toDoLists));
        }

        [HttpGet("{userId:guid}/ToDoLists/{id:guid}", Name = "GetToDoList")]
        [ServiceFilter(typeof(ValidateEntityExistsAttribute<ToDoList>))]
        public ActionResult<ToDoListDto> GetSingleToDoList(Guid userId, Guid id)
        {
            var toDoList = _toDoListRepository.FindByCondition(tl => tl.Id == id)
                                .Include(tl => tl.ToDos)
                                .FirstOrDefault();

            return Ok(_mapper.Map<ToDoListDto>(toDoList));
        }

        [AllowAnonymous]
        [HttpPost("signin")]
        public ActionResult Signin(LoginDto model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = _userRepository.FindByCondition(u => u.Email == model.Email).FirstOrDefault();

            if (user == null || !UserRepository.VerifyPassword(model.Password, user.Hash, user.Salt))
            {
                return NotFound();
            }

            if (user.Google || user.Facebook)
            {
                return BadRequest("Invalid credentials");
            }

            var token = CreateJwt(user);
            var userDto = _mapper.Map<UserDto>(user);

            return Ok(new {
                user = userDto,
                token
            });
        }

        [HttpPost("signin/social")]
        [AllowAnonymous]
        public ActionResult SigninSocial(SocialLoginDto model)
        {
            if (model == null)
            {
                return BadRequest("Object is null.");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = _userRepository.FindByCondition(u => u.Email == model.Email).FirstOrDefault();
            if (user == null)
            {
                return NotFound();
            }
            if (user.Google != model.Google || user.Facebook != model.Facebook)
            {
                return NotFound("Invalid credentials.");
            }

            var token = CreateJwt(user);

            var userModel = _mapper.Map<UserDto>(user);

            return Ok(new
            {
                user = userModel,
                token
            });
        }

        [HttpPost("signup")]
        [AllowAnonymous]
        [ServiceFilter(typeof(ValidationFilterAttribute))]
        public ActionResult Signup(UserDto model)
        {
            var user = _userRepository.FindByCondition(u => u.Email == model.Email).FirstOrDefault();
            if (user != null)
            {
                return BadRequest("The email address is already in use, try a different one");
            }

            user = _mapper.Map<User>(model);

            if (String.IsNullOrEmpty(model.Password))
            {
                return BadRequest("Password field is required");
            }

            UserRepository.CreatePasswordHash(model.Password, out byte[] hash, out byte[] salt);
            user.Hash = hash;
            user.Salt = salt;

            _userRepository.Create(user);
            _userRepository.Save();

            var token = CreateJwt(user);

            _mapper.Map(user, model);
            model.Password = null;

            return Ok(new
            {
                user = model,
                token
            });
        }

        [HttpPost("signup/social")]
        [AllowAnonymous]
        public ActionResult SignupSocial(SocialLoginDto model)
        {
            if (model == null)
            {
                return BadRequest("Object is null.");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = _userRepository.FindByCondition(u => u.Email == model.Email).FirstOrDefault();
            if (user != null)
            {
                return BadRequest("The email address is already in use, try a different one");
            }

            user = _mapper.Map<User>(model);

            _userRepository.Create(user);
            _userRepository.Save();

            var token = CreateJwt(user);

            var userModel = _mapper.Map<UserDto>(user);

            return Ok(new
            {
                user = userModel,
                token
            });
        }

        [HttpGet("{id:guid}", Name = "GetUser")]
        [ServiceFilter(typeof(ValidateEntityExistsAttribute<User>))]
        public ActionResult<UserDto> Get(Guid id)
        {
            if (!ValidUserJwt(id))
            {
                return Unauthorized();
            }

            var user = HttpContext.Items["entity"] as User;

            return Ok(_mapper.Map<UserDto>(user));
        }

        [HttpPut("{id:guid}")]
        [ServiceFilter(typeof(ValidationFilterAttribute))]
        [ServiceFilter(typeof(ValidateEntityExistsAttribute<User>))]
        public ActionResult Put(Guid id, [FromBody] UserDto model)
        {
            if (!ValidUserJwt(id))
            {
                return Unauthorized();
            }

            var user = HttpContext.Items["entity"] as User;
            _mapper.Map(model, user);

            if (!String.IsNullOrEmpty(model.Password))
            {
                UserRepository.CreatePasswordHash(model.Password, out byte[] hash, out byte[] salt);
                user.Hash = hash;
                user.Salt = salt;
            }

            _userRepository.Update(user);
            _userRepository.Save();

            return NoContent();
        }

        [HttpDelete("{id:guid}")]
        [ServiceFilter(typeof(ValidateEntityExistsAttribute<User>))]
        public ActionResult Delete(Guid id)
        {
            if (!ValidUserJwt(id))
            {
                return Unauthorized();
            }

            var user = HttpContext.Items["entity"] as User;

            _userRepository.Delete(user);
            if (!_userRepository.Save())
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Something went wrong");
            }

            return NoContent();
        }

        [HttpGet("renew_token")]
        public ActionResult RenewToken()
        {
            var identity = HttpContext.User.Identity as ClaimsIdentity;
            if (identity == null)
            {
                return Unauthorized();
            }

            var nameIdentClaim = identity.Claims.Where(c => c.Type == ClaimTypes.NameIdentifier).FirstOrDefault();
            if (nameIdentClaim == null)
            {
                return Unauthorized();
            }

            var userIdStr = nameIdentClaim.Value;
            if (userIdStr == null)
            {
                return Unauthorized();
            }

            var userId = new Guid(userIdStr);
            var user = _userRepository.FindByCondition(u => u.Id == userId).FirstOrDefault();
            if (user == null)
            {
                return NotFound();
            }

            var token = CreateJwt(user);
            var userDto = _mapper.Map<UserDto>(user);

            return Ok(new
            {
                user = userDto,
                token
            });
        }

        private string CreateJwt(User user)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.GivenName, user.FirstName ?? ""),
                new Claim(ClaimTypes.Surname, user.LastName ?? ""),
                new Claim(ClaimTypes.Role, "User")
            };

            var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("superSecretKey@345"));
            var signinCredentials = new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256);
            var tokeOptions = new JwtSecurityToken(
                issuer: "http://localhost:5000",
                audience: "http://localhost:5000",
                claims,
                expires: DateTime.Now.AddMinutes(5),
                signingCredentials: signinCredentials
            );

            var token = new JwtSecurityTokenHandler().WriteToken(tokeOptions);

            return token;
        }
        
        private bool ValidUserJwt(Guid id)
        {
            var identity = HttpContext.User.Identity as ClaimsIdentity;
            if (identity == null)
            {
                return false;
            }

            var reqId = identity.Claims.Where(c => c.Type == ClaimTypes.NameIdentifier).FirstOrDefault().Value;
            if (id.ToString() != reqId)
            {
                return false;
            }

            return true;
        }
    }
}
