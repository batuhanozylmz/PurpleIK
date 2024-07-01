using PurpleIK.Core.Entity.Contract;
using PurpleIK.Core.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PurpleIK.Core.Entity.Abstract
{
    public abstract class BaseEntity<T> : IEntity<T>, IEntityBase
    {
        public T Id { get; set; }
        public Status? Status { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? CreatedDate { get; set; } = DateTime.Now;
        [Column(TypeName = "datetime")]
        public DateTime? ModifiedDate { get; set; }

        //[DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public int AutoId { get; set; }
    }
}
