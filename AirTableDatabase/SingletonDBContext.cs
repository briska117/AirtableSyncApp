using AirTableDatabase.DBModels;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AirTableDatabase
{
    public class SingletonDBContext : DbContext
    {
        public SingletonDBContext(DbContextOptions<SingletonDBContext> opt) : base(opt)
        {

        }
        public DbSet<SyncEventHistory> EventHistories { get; set; }


    }
}
