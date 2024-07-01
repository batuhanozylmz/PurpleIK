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
    public class PersonPermissionService : BaseRepository<PersonPermission>, IPersonPermissionService
    {

        public PersonPermissionService(PurpleDbContext context) : base(context)
        {

        }
    }
}