using System; // <-- ეს უნდა დაემატოს
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using FinancialProfileManagerAPI.Models;
using FinancialProfileManagerAPI.Services;

namespace FinancialProfileManagerAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserProfileController : ControllerBase
    {
        private readonly UserProfileService _service;

        public UserProfileController(UserProfileService service)
        {
            _service = service;
        }

        [HttpGet]
        public IActionResult GetUsers()
        {
            return Ok(_service.GetAll());
        }

        [HttpPost]
        public IActionResult CreateUser([FromBody] UserProfile user)
        {
            var added = _service.AddUser(user);
            return Ok(added);
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteUser(Guid id) // Guid სწორად ამოღებულია System-დან
        {
            var user = _service.GetAll().FirstOrDefault(u => u.Id == id);
            if (user == null)
                return NotFound();

            _service.DeleteUser(user);
            return NoContent();
        }
    }
}
