using PurpleIK.Core.Entity.Abstract;
using PurpleIK.Core.Entity.Contract;
using PurpleIK.Core.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PurpleIK.Entities
{
    public class PersonPermission : BaseEntity<Guid>, IEntity<Guid>
    {

       
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public int? NumberOfDays { get; set; }
        public DateTime? DateOfReply { get; set; }
        public byte[]? PermissionFile { get; set; }
        public string? ReasonOfRejection { get; set; }
        public string? CompanyManagerName { get; set; }
        public string? CompanyManagerEmail { get; set; }
        public Guid PersonId { get; set; }
        public Guid PermissionId { get; set; }

        //nav
        public virtual Person? Person { get; set; }
        public virtual Permission? Permission { get; set; }
    }
}
