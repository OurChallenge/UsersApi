using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Users.Domain.Entities;
using Users.Infrastructure.Persistence.Db;

namespace Users.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class UserController : ControllerBase
{
    private readonly CloudGamesDbContext _context;

    public UserController(CloudGamesDbContext context)
    {
        _context = context;
    }

    [HttpGet("GetUser")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<IEnumerable<User>>> GetUser()
    {
        return await _context.Users.AsNoTracking().ToListAsync();
    }

    [HttpGet("GeUserById")]
    [Authorize]
    public async Task<ActionResult<User>> GetUserById(Guid id)
    {
        var user = await _context.Users.FindAsync(id);
        return user is null ? NotFound() : user;
    }

    [HttpPost("CreateUser")]
    public async Task<ActionResult<User>> CreateUser(User user)
    {
        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetUserById), new { id = user.Id }, user);
    }

    [HttpPut("UpdateUser")]
    [Authorize]
    public async Task<IActionResult> UpdateUser(Guid id, User user)
    {
        if (id != user.Id)
            return BadRequest("O ID da URL não corresponde ao do corpo da requisição.");

        _context.Entry(user).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!UserExists(id))
                return NotFound();

            throw;
        }

        return NoContent();
    }

    [HttpDelete("DeleteUser")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteUser(Guid id)
    {
        var user = await _context.Users.FindAsync(id);
        if (user is null)
            return NotFound();

        _context.Users.Remove(user);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    private bool UserExists(Guid id)
    {
        return _context.Users.Any(e => e.Id == id);
    }
}
