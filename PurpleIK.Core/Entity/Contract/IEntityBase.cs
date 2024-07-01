using PurpleIK.Core.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PurpleIK.Core.Entity.Contract
{
    public interface IEntityBase
    {
        Status? Status { get; set; }
        [Column(TypeName = "datetime")]
        DateTime? CreatedDate { get; set; }
        [Column(TypeName = "datetime")]
        DateTime? ModifiedDate { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        int AutoId { get; set; }
    }
}
