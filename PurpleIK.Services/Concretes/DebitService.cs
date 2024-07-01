using PurpleIK.Context;
using PurpleIK.Entities;
using PurpleIK.Repositories;
using PurpleIK.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PurpleIK.Services.Concretes
{
    public class DebitService : BaseRepository<Debit>, IDebitService
    {
        public DebitService(PurpleDbContext context) : base(context)
        {

        }
    }
}
