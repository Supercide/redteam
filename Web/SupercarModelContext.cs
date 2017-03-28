using System;
using System.Configuration;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using Web.Models;

namespace Web
{
  public class SupercarModelContext : DbContext
  {
      public SupercarModelContext() : this("DefaultConnection")
      {
          
      }
      public SupercarModelContext(string connectionName)
      {
            this.Database.Connection.ConnectionString = Environment.GetEnvironmentVariable(connectionName) ??
                                                        ConfigurationManager.ConnectionStrings[connectionName]
                                                            .ConnectionString;
        }
    public DbSet<UserProfile> UserProfiles { get; set; }
    public DbSet<Make> Makes { get; set; }
    public DbSet<Supercar> Supercars { get; set; }
    public DbSet<Vote> Votes { get; set; }

    protected override void OnModelCreating(DbModelBuilder modelBuilder)
    {
      modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
    }
  }
}