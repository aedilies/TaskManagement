using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskManagement.Domain.Common;

namespace TaskManagement.Domain.Entities;

public class User : IEntity 
{
    public Guid Id { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public Guid DepartmentId { get; set; }
    public Guid RoleId { get; set; }

    // Navigation properties
    public virtual Department? Department { get; set; }
    public virtual Role? Role { get; set; }

    // Задачи, созданные пользователем
    public virtual ICollection<TaskItem> CreatedTasks { get; set; } = new List<TaskItem>();
    // Задачи, назначенные пользователю
    public virtual ICollection<TaskItem> AssignedTasks { get; set; } = new List<TaskItem>();
}
