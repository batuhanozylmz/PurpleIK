using Microsoft.EntityFrameworkCore;
using PurpleIK.Context;
using PurpleIK.Core.Enums;
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
    public class PersonService : BaseRepository<Person>, IPersonService
    {

        public PersonService(PurpleDbContext context) : base(context)
        {
         
        }     

    }
}

