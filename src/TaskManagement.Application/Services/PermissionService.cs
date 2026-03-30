using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskManagement.Domain.Entities;

namespace TaskManagement.Application.Services
{
    public class PermissionService
    {
        public bool HasPermission(User user, string permission)
        {
            return user.Role?.Permissions?.Any(p => p.Code == permission) == true;
        }
    }
}
