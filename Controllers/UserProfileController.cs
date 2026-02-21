using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Http;
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
        [ProducesResponseType(typeof(IEnumerable<UserProfile>), StatusCodes.Status200OK)]
        public IActionResult GetUsers()
        {
            try
            {
                return Ok(_service.GetAll());
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = ex.Message });
            }
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(UserProfile), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult GetUserById(Guid id)
        {
            try
            {
                var user = _service.GetUserById(id);
                if (user == null)
                    return NotFound(new { message = "User not found" });

                return Ok(user);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = ex.Message });
            }
        }

        [HttpPost]
        [ProducesResponseType(typeof(UserProfile), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public IActionResult CreateUser([FromBody] UserProfile user)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var added = _service.AddUser(user);
                return CreatedAtAction(nameof(GetUserById), new { id = added.Id }, added);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = ex.Message });
            }
        }

        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public IActionResult UpdateUser(Guid id, [FromBody] UserProfile user)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var existing = _service.GetUserById(id);
                if (existing == null)
                    return NotFound(new { message = "User not found" });

                _service.UpdateUser(id, user);
                return Ok(existing);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult DeleteUser(Guid id)
        {
            try
            {
                var user = _service.GetUserById(id);
                if (user == null)
                    return NotFound(new { message = "User not found" });

                _service.DeleteUser(user);
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = ex.Message });
            }
        }

        [HttpGet("search")]
        [ProducesResponseType(typeof(IEnumerable<UserProfile>), StatusCodes.Status200OK)]
        public IActionResult SearchByName([FromQuery] string name)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(name))
                    return BadRequest(new { message = "Search name cannot be empty" });

                var users = _service.SearchByName(name);
                return Ok(users);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = ex.Message });
            }
        }

        [HttpGet("by-tier/{tier}")]
        [ProducesResponseType(typeof(IEnumerable<UserProfile>), StatusCodes.Status200OK)]
        public IActionResult GetByTier(AccountTier tier)
        {
            try
            {
                var users = _service.GetByTier(tier);
                return Ok(users);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = ex.Message });
            }
        }

        [HttpGet("by-balance-range")]
        [ProducesResponseType(typeof(IEnumerable<UserProfile>), StatusCodes.Status200OK)]
        public IActionResult GetByBalanceRange([FromQuery] decimal minBalance, [FromQuery] decimal maxBalance)
        {
            try
            {
                if (minBalance < 0 || maxBalance < 0 || minBalance > maxBalance)
                    return BadRequest(new { message = "Invalid balance range" });

                var users = _service.GetByBalanceRange(minBalance, maxBalance);
                return Ok(users);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = ex.Message });
            }
        }
    }
}
