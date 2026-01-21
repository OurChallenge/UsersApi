using FIAP_Cloud_Games.Domain.Entities;
using FIAP_Cloud_Games.Infrastructure.Persistence.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FIAP_Cloud_Games.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly CloudGamesDbContext _context;
        public UserController(CloudGamesDbContext context) => _context = context;

        // GET: api/<UsersController>
        [HttpGet("GetUser")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<IEnumerable<User>>> GetUser()
        {
            // usa o _context normalmente
            return await _context.Users.AsNoTracking().ToListAsync();
        }

        // GET api/<UsersController>/5
        [HttpGet("GeUserById")]
        [Authorize]
        public async Task<ActionResult<User>> GetUserById(Guid id)
        {
            return await _context.Users.FindAsync(id);
        }

        // POST api/<UsersController>
        [HttpPost("CreateUser")]
        public async Task<ActionResult<User>> CreateUser(User user)
        {
            _context.Users.Add(user);
            await _context.SaveChangesAsync(); // executa o INSERT

            return CreatedAtAction(nameof(GetUser), new { id = user.Id }, user);
        }

        // PUT api/<UsersController>/5
        [HttpPut("UpdateUser")]
        [Authorize]
        public async Task<IActionResult> UpdateUser(Guid id, User user)
        {
            if (id != user.Id)
                return BadRequest("O ID da URL não corresponde ao do corpo da requisição.");

            _context.Entry(user).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync(); // executa o UPDATE
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UserExists(id))
                    return NotFound(); // se o usuário não existe mais
                else
                    throw; // re-lança se for outro tipo de erro
            }

            return NoContent(); // 204
        }

        // DELETE api/<UsersController>/{id}
        [HttpDelete("DeleteUser")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteUser(Guid id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
                return NotFound();

            _context.Users.Remove(user);
            await _context.SaveChangesAsync(); // executa o DELETE

            return NoContent();
        }

        private bool UserExists(Guid id)
        {
            return _context.Users.Any(e => e.Id == id);
        }
    }
}
