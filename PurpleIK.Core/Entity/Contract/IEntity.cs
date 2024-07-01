using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PurpleIK.Core.Entity.Contract
{
    public interface IEntity<T>
    {
        T Id { get; set; }
    }
}
