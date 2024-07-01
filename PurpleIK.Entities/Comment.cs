using PurpleIK.Core.Entity.Abstract;
using PurpleIK.Core.Entity.Contract;
using PurpleIK.Core.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace PurpleIK.Entities
{
    public class Comment : BaseEntity<Guid>, IEntity<Guid>
    {
        public string CommentText { get; set; }
        public string? Title { get; set; }
        public string? Summary { get; set; } // başlık özet      
        public byte[]? Photo { get; set; }

        //NAV
        public virtual Person? Person { get; set; }
        public Guid? PersonId { get; set; }
    }
}
