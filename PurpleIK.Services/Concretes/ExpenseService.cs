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
    public class ExpenseService : BaseRepository<Expense>, IExpenseService
    {
        public ExpenseService(PurpleDbContext context) : base(context)
        {
        }
    }
}
