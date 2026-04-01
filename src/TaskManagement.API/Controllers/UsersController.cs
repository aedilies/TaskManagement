using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TaskManagement.Domain.Entities;
using TaskManagement.Infrastructure.Data;

namespace TaskManagement.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly AppDbContext _context;

    public UsersController(AppDbContext context)
    {
        _context = context;
    }

    // GET: api/users
    [HttpGet]
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var users = await _context.Users
            .Include(u => u.Department)
            .Include(u => u.Role)
            .Where(u => u.Role != null && u.Role.Code != "OBSERVER")
            .Select(u => new
            {
                u.Id,
                u.FullName,
                u.Email,
                Department = u.Department != null ? u.Department.Name : null,
                Role = u.Role != null ? u.Role.Name : null
            })
            .ToListAsync();

        return Ok(users);
    }

    // GET: api/users/by-department/{deptId}
    [HttpGet("by-department/{deptId}")]
    public async Task<IActionResult> GetByDepartment(Guid deptId)
    {
        var users = await _context.Users
            .Where(u => u.DepartmentId == deptId)
            .Include(u => u.Department)
            .Include(u => u.Role)
            .Select(u => new
            {
                u.Id,
                u.FullName,
                u.Email,
                Department = u.Department != null ? u.Department.Name : null,
                Role = u.Role != null ? u.Role.Name : null
            })
            .ToListAsync();

        return Ok(users);
    }

    // GET: api/users/{id}
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var user = await _context.Users
            .Include(u => u.Department)
            .Include(u => u.Role)
            .Where(u => u.Id == id)
            .Select(u => new
            {
                u.Id,
                u.FullName,
                u.Email,
                Department = u.Department != null ? u.Department.Name : null,
                Role = u.Role != null ? u.Role.Name : null
            })
            .FirstOrDefaultAsync();

        if (user == null)
            return NotFound();

        return Ok(user);
    }
}