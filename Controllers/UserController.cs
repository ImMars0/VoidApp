using Microsoft.AspNetCore.Mvc;
using user.Models;
using user.Services;


namespace user.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly UserService _userService;
        public UserController(UserService userService)
        {
            _userService = userService;
        }

        [HttpGet]
        public ActionResult<List<User>> GetAll()
        {
            return Ok(_userService.GetAll());
        }


        [HttpGet("{id}")]
        public ActionResult<User> GetById(int id)
        {
            var user = _userService.GetById(id);
            if (user == null)
            {
                return NotFound();
            }
            return Ok(user);
        }


        [HttpGet("search")]
        public ActionResult<List<User>> FindByName([FromQuery] string userName)
        {
            return Ok(_userService.FindByName(userName));
        }



        [HttpPost]
        public ActionResult Add([FromBody] User user)
        {
            if (user == null)
            {
                return BadRequest("User name is required");
            }


            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _userService.AddUser(user);
            return CreatedAtAction(nameof(GetById), new { id = user.Id }, user);
        }




        [HttpPut("{id}")]
        public IActionResult Update(int id, [FromBody] User updateUser)
        {
            if (updateUser == null || id != updateUser.Id)
            {
                return BadRequest("Invalid user data or ID mismatch.");
            }


            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (!_userService.Update(id, updateUser))
            {
                return NotFound();
            }

            return NoContent();
        }


        [HttpDelete("{id}")]
        public ActionResult Delete(int id)
        {
            try
            {
                _userService.Delete(id);
                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
        }
    }
}