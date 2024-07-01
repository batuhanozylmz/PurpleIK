using PurpleIK.Core.Entity.Abstract;
using PurpleIK.Core.Entity.Contract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PurpleIK.Entities
{
    public class PushMoney : BaseEntity<Guid>, IEntity<Guid>
    {
        public string Type { get; set; }
        public int Amount { get; set; }
        public string? CurrencyUnit { get; set; }
        public string? Description { get; set; }
        public virtual Person? Person { get; set; }
        public Guid? PersonId { get; set; }

    }
}
