﻿using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AirTableDatabase.DBModels;

namespace AirTableDatabase
{
    public class ApplicationDBContext : DbContext
    {
        public ApplicationDBContext(DbContextOptions<ApplicationDBContext> opt) : base(opt)
        {

        }
        public DbSet<Project> Projects { get; set; }   
        public DbSet<SyncEvent> SyncEvents   { get; set; }
        public DbSet<SyncEventHistory> EventHistories { get; set; }  
        public DbSet<ClientPrefix> ClientPrefixes { get; set; }
        public DbSet<CountryPrefix> CountryPrefixes { get; set; }
        public DbSet<CollectionMode> CollectionModes { get; set; }  
        public DbSet<RelatedTable> RelatedTables { get; set; }  
        public DbSet<CollectionModeRelatedTable> CollectionModeRelatedTables { get; set; }
        public DbSet<UserProject> UserProjects { get; set; }
        public DbSet<WindowsServiceProjectSynchronization> ProjectSynchronization { get; set; }
        public DbSet<AirTableField> AirTableFields { get; set; }    


    }
}
