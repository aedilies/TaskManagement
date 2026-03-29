using Microsoft.EntityFrameworkCore;
using TaskManagement.Domain.Entities;
using TaskManagement.Domain.Enums;
using TaskManagement.Infrastructure.Data;

namespace TaskManagement.Infrastructure.Seed;

public static class DbInitializer
{
    public static async Task InitializeAsync(AppDbContext context)
    {
        await context.Database.EnsureCreatedAsync();

        // Роли
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

        // Разрешения (примеры)
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

        // Связи Role-Permission (пример для Начальника - все права)
        var bossRole = await context.Roles.FirstOrDefaultAsync(r => r.Code == "BOSS");
        var allPermissions = await context.Permissions.ToListAsync();
        if (bossRole != null && !bossRole.Permissions.Any())
        {
            foreach (var perm in allPermissions)
                bossRole.Permissions.Add(perm);
            await context.SaveChangesAsync();
        }

        // Отделы
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

        // Пользователи
        if (!context.Users.Any())
        {
            var itDept = await context.Departments.FirstAsync(d => d.Name == "IT");
            var hrDept = await context.Departments.FirstAsync(d => d.Name == "HR");
            var bossRoleObj = await context.Roles.FirstAsync(r => r.Code == "BOSS");
            var empRole = await context.Roles.FirstAsync(r => r.Code == "EMPLOYEE");
            var obsRole = await context.Roles.FirstAsync(r => r.Code == "OBSERVER");

            var users = new[]
            {
                new User { Id = Guid.NewGuid(), FullName = "Иван Иванов", Email = "ivan@example.com", DepartmentId = itDept.Id, RoleId = bossRoleObj.Id },
                new User { Id = Guid.NewGuid(), FullName = "Петр Петров", Email = "petr@example.com", DepartmentId = itDept.Id, RoleId = empRole.Id },
                new User { Id = Guid.NewGuid(), FullName = "Сидор Сидоров", Email = "sidor@example.com", DepartmentId = hrDept.Id, RoleId = obsRole.Id }
            };
            await context.Users.AddRangeAsync(users);
            await context.SaveChangesAsync();
        }
    }
}