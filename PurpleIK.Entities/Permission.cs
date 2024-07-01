using PurpleIK.Core.Entity.Abstract;
using PurpleIK.Core.Entity.Contract;
using PurpleIK.Core.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PurpleIK.Entities
{
    public class Permission : BaseEntity<Guid>, IEntity<Guid>
    {
        public string PermissionType { get; set; }     
    }
}
