using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskManagement.Domain.Common;

namespace TaskManagement.Domain.Entities;

    public class Permission : IEntity
    {
        public Guid Id { get; set; }
        public string Code { get; set; } = string.Empty; // например "CREATE_TASK"
        public string? Description { get; set; }

        // Many-to-many with Role (через промежуточную таблицу)
        public virtual ICollection<Role> Roles { get; set; } = new List<Role>();
    }
