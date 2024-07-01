using PurpleIK.Core.Entity.Abstract;
using PurpleIK.Core.Entity.Contract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PurpleIK.Entities
{
    public class Expense : BaseEntity<Guid>, IEntity<Guid>
    {
        public string? Name { get; set; }
        public string? Description { get; set; }
        public DateTime? ExpenseDate { get; set; }
        public decimal? Price { get; set; }
        public byte[]? ExpenseForm { get; set; }
        public Guid? PersonId { get; set; }
        public Guid? ManagerId { get; set; }

        //nav
        public virtual Person? Person { get; set; }
        public virtual Person? Manager { get; set; }

    }
}
