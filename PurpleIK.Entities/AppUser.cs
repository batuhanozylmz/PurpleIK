using Microsoft.AspNetCore.Identity;
using PurpleIK.Core.Entity.Contract;
using PurpleIK.Core.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PurpleIK.Entities
{
    public class AppUser : IdentityUser<Guid>, IEntityBase
    {
        public Status? Status { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? ModifiedDate { get; set; }
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public int AutoId { get; set; }
        public virtual ICollection<Person> Persons { get; set; }
    }
}
