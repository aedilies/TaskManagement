using Microsoft.EntityFrameworkCore;
using TaskManagement.Domain.Entities;
using TaskManagement.Infrastructure.Data;

namespace TaskManagement.Infrastructure.Seed;

public static class DbInitializer
{
    public static async Task InitializeAsync(AppDbContext context)
    {
        await context.Database.EnsureCreatedAsync();

        if (!context.Roles.Any())
        {
            var roles = new[]
            {
                new Role { Id = Guid.NewGuid(), Name = "Начальник", Code = "BOSS", CreatedAt = DateTime.UtcNow, IsActive = true },
                new Role { Id = Guid.NewGuid(), Name = "Сотрудник", Code = "EMPLOYEE", CreatedAt = DateTime.UtcNow, IsActive = true },
                new Role { Id = Guid.NewGuid(), Name = "Наблюдатель", Code = "OBSERVER", CreatedAt = DateTime.UtcNow, IsActive = true }
            };
            await context.Roles.AddRangeAsync(roles);
            await context.SaveChangesAsync();
        }

        if (!context.Permissions.Any())
        {
            var permissions = new[]
            {
                new Permission { Id = Guid.NewGuid(), Code = "CREATE_TASK", Description = "Создание задач" },
                new Permission { Id = Guid.NewGuid(), Code = "EDIT_TASK", Description = "Редактирование задач" },
                new Permission { Id = Guid.NewGuid(), Code = "DELETE_TASK", Description = "Удаление задач" },
                new Permission { Id = Guid.NewGuid(), Code = "ASSIGN_TASK", Description = "Назначение исполнителя" },
                new Permission { Id = Guid.NewGuid(), Code = "CHANGE_STATUS", Description = "Смена статуса" },
                new Permission { Id = Guid.NewGuid(), Code = "VIEW_ALL_TASKS", Description = "Просмотр всех задач" }
            };
            await context.Permissions.AddRangeAsync(permissions);
            await context.SaveChangesAsync();
        }

        var bossRole = await context.Roles.Include(r => r.Permissions).FirstOrDefaultAsync(r => r.Code == "BOSS");
        var allPermissions = await context.Permissions.ToListAsync();
        if (bossRole != null)
        {
            foreach (var perm in allPermissions)
            {
                if (!bossRole.Permissions.Any(p => p.Id == perm.Id))
                    bossRole.Permissions.Add(perm);
            }
            await context.SaveChangesAsync();
        }

        if (!context.Departments.Any())
        {
            var depts = new[]
            {
                new Department { Id = Guid.NewGuid(), Name = "IT", Description = "Информационные технологии", IsActive = true },
                new Department { Id = Guid.NewGuid(), Name = "HR", Description = "Кадры", IsActive = true },
                new Department { Id = Guid.NewGuid(), Name = "Sales", Description = "Продажи", IsActive = true }
            };
            await context.Departments.AddRangeAsync(depts);
            await context.SaveChangesAsync();
        }

        if (!context.Users.Any())
        {
            var itDept = await context.Departments.FirstAsync(d => d.Name == "IT");
            var hrDept = await context.Departments.FirstAsync(d => d.Name == "HR");
            var bossRoleObj = await context.Roles.FirstAsync(r => r.Code == "BOSS");
            var empRole = await context.Roles.FirstAsync(r => r.Code == "EMPLOYEE");
            var obsRole = await context.Roles.FirstAsync(r => r.Code == "OBSERVER");

            var users = new[]
            {
                new User { Id = Guid.NewGuid(), FullName = "123", Email = "boss@example.com", DepartmentId = itDept.Id, RoleId = bossRoleObj.Id },
                new User { Id = Guid.NewGuid(), FullName = "2", Email = "observer@example.com", DepartmentId = itDept.Id, RoleId = obsRole.Id },
                new User { Id = Guid.NewGuid(), FullName = "44", Email = "employee@example.com", DepartmentId = hrDept.Id, RoleId = empRole.Id }
            };
            await context.Users.AddRangeAsync(users);
            await context.SaveChangesAsync();
        }
    }
}