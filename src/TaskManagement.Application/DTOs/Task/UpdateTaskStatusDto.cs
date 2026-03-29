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
