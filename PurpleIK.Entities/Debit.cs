using PurpleIK.Core.Entity.Abstract;
using PurpleIK.Core.Entity.Contract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PurpleIK.Entities
{
    public class Debit : BaseEntity<Guid>, IEntity<Guid>
    {
        public string ProductName { get; set; }
        public DateTime? ReceiptDate { get; set; }//alma
        public DateTime? DeliveryDate { get; set; }//verme
        public byte[]? DebitForm { get; set; }
        public virtual Person? Person { get; set; }
        public Guid? PersonId { get; set; }
        public virtual Person? Manager { get; set; }
        public Guid? ManagerId { get; set; }
    }
    
}
