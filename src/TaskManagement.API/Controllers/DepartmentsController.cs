using Microsoft.AspNetCore.Mvc;
using TaskManagement.Domain.Entities;
using TaskManagement.Infrastructure.Repositories;

namespace TaskManagement.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DepartmentsController : ControllerBase
{
    private readonly IRepository<Department> _departmentRepo;

    public DepartmentsController(IRepository<Department> departmentRepo)
    {
        _departmentRepo = departmentRepo;
    }

    // GET: api/departments
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var departments = await _departmentRepo.GetAllAsync();
        return Ok(departments);
    }

    // GET: api/departments/{id}
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var dept = await _departmentRepo.GetByIdAsync(id);
        if (dept == null)
            return NotFound();

        return Ok(dept);
    }
}