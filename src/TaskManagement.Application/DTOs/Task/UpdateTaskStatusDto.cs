using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskManagement.Application.DTOs.Task;

public class UpdateTaskStatusDto
{
    public Guid TaskId { get; set; }
    public int Status { get; set; }
}
public class UpdateTaskDto
{
    public Guid TaskId { get; set; }
    public string? Title { get; set; }
    public string? Description { get; set; }
    public int? Priority { get; set; }
}

public class AssignTaskDto
{
    public Guid TaskId { get; set; }
    public Guid AssignedToUserId { get; set; }
}
