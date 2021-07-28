﻿using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.Web.CodeGeneration.EntityFrameworkCore;
using ObjectCubeServer.Models.DomainClasses;
using ObjectCubeServer.Models.DomainClasses.TagTypes;
using System;
using System.Configuration;

namespace ObjectCubeServer.Models.DataAccess
{
    /// <summary>
    /// The ObjectContext is the entrypoint to the database.
    /// 
    /// Before you can use the database, change the value of 'AttachDbFileName' to a valid directory on your machine.
    /// Eg: C:\\Databases\\ObjectDB.mdf
    /// The Databases directory needs to exist.
    /// 
    /// Common Package Manager Console (PMC) commands:
    ///     - Delete database and all its data:
    ///     Drop-Database      
    ///     - Add a new migration with the name "init". 
    ///       This commnad also creates the Migration folder if it does not exist:
    ///     Add-Migration 
    ///     - Update the database with current schema:
    ///     Update-Database
    /// 
    /// To clear database and migrations:
    ///     - Delete the Migrations folder.
    ///     - Run the command "drop-database" from the PMC (Package Manger Console).
    ///     - Check this issue for more information: https://github.com/aspnet/EntityFramework.Docs/issues/1048
    /// </summary>
    public class ObjectContext : DbContext
    {
        public ObjectContext()
        {

        }

        public ObjectContext(DbContextOptions<ObjectContext> options) : base(options)
        {

        }

        public ObjectContext(string connectionString)
        {
            this.connectionString = connectionString;
        }

        private string connectionString;

        /*
         * Exposing which DBSets are available to get, add, update and delete from:
         */
        public DbSet<CubeObject> CubeObjects { get; set; }
        public DbSet<Tagset> Tagsets { get; set; }
        public DbSet<Tag> Tags { get; set; }
        public DbSet<ObjectTagRelation> ObjectTagRelations { get; set; }
        public DbSet<Hierarchy> Hierarchies { get; set; }
        public DbSet<Node> Nodes { get; set; }
        public DbSet<TagType> TagTypes { get; set; }
        
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            //This is called "fluent API": https://docs.microsoft.com/en-us/ef/core/modeling/
            //And is a way to specify database relations explicitly for Entity Framework CORE (EF CORE)

            //Tells EF CORE that ObjectTag's primary key is composed of ObjectId and TagId:
            modelBuilder.Entity<ObjectTagRelation>()
                .HasKey(ot => new { ot.ObjectId, ot.TagId });
            //Tells EF CORE that there is a one-to-many relationship between ObjectTagRelation and CubeObject:
            modelBuilder.Entity<ObjectTagRelation>()
                .HasOne(otr => otr.CubeObject)
                .WithMany(co => co.ObjectTagRelations)
                .HasForeignKey(otr => otr.ObjectId);
            //Tells EF CORE that there is a one-to-many relationsship between ObjectTagRelation and Tag:
            modelBuilder.Entity<ObjectTagRelation>()
                .HasOne(otr => otr.Tag)
                .WithMany(t => t.ObjectTagRelations)
                .HasForeignKey(otr => otr.TagId);

            //Tells EF Core that tagset's name should be unique.
            modelBuilder.Entity<Tagset>()
                .HasIndex(ts => ts.Name)
                .IsUnique();

            //Many-to-one relationship, if tagset is deleted, then tags are also deleted.
            modelBuilder.Entity<Tagset>()
                .HasMany(ts => ts.Tags)
                .WithOne(t => t.Tagset)
                .OnDelete(DeleteBehavior.Cascade);

            //Many-to-one relationship, if tagset is deleted, then hierarchies are also deleted.
            modelBuilder.Entity<Tagset>()
                .HasMany(ts => ts.Hierarchies)
                .WithOne(h => h.Tagset)
                .OnDelete(DeleteBehavior.Cascade);

            //Every tag has exactly one tag type
            modelBuilder.Entity<Tag>()
                .HasOne(t => t.TagType);

            //Many-to-one relationship, if hierarchy is deleted, then nodes are also deleted.
            modelBuilder.Entity<Hierarchy>()
                .HasMany(h => h.Nodes)
                .WithOne(n => n.Hierarchy)
                .OnDelete(DeleteBehavior.Cascade);

            //Node has a reference to a tag, but a tag has no reference to the nodes.
            //If a Node is deleted the tag is not deleted.
            modelBuilder.Entity<Node>()
                .HasOne(n => n.Tag)
                .WithMany()
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);

            //Typed Tags store a replicate of tagsetid
            //Creates foreign key constraints for these properties
            modelBuilder.Entity<AlphanumericalTag>()
                .HasOne<Tagset>()
                .WithMany()
                .HasForeignKey(at => at.TagsetIdReplicate);

            modelBuilder.Entity<DateTag>()
                .HasOne<Tagset>()
                .WithMany()
                .HasForeignKey(dt => dt.TagsetIdReplicate);

            modelBuilder.Entity<TimeTag>()
                .HasOne<Tagset>()
                .WithMany()
                .HasForeignKey(tt => tt.TagsetIdReplicate);

            modelBuilder.Entity<NumericalTag>()
                .HasOne<Tagset>()
                .WithMany()
                .HasForeignKey(nt => nt.TagsetIdReplicate);

            //Enforce that a typed tag is unqiue within a tagset
            modelBuilder.Entity<DateTag>()
                .HasIndex(dt => new { dt.TagsetIdReplicate, dt.Name })
                .IsUnique();

            modelBuilder.Entity<AlphanumericalTag>()
                .HasIndex(at => new { at.TagsetIdReplicate, at.Name })
                .IsUnique();

            modelBuilder.Entity<TimeTag>()
                .HasIndex(tt => new { tt.TagsetIdReplicate, tt.Name })
                .IsUnique();

            modelBuilder.Entity<NumericalTag>()
                .HasIndex(nt => new { nt.TagsetIdReplicate, nt.Name })
                .IsUnique();

            //Calling on model creating:
            base.OnModelCreating(modelBuilder);
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            //base.OnConfiguring(optionsBuilder);
            string computerName = System.Environment.MachineName;
            switch (computerName)
            {
                //Postgres example
                case "DESKTOP-9RO8H19":
                    if (connectionString != null)
                    {
                        optionsBuilder.UseNpgsql(connectionString);
                    }
                    else
                    {
                        optionsBuilder.UseNpgsql("Server = localhost; Port = 5432; Database = PC; User Id = photocube; Password = postgres;");
                    }
                    break;
                //Postgres example
                //case "computerName":
                //    if (connectionString != null)
                //    {
                //        optionsBuilder.UseNpgsql(connectionString);
                //    }
                //    else
                //    {
                //        optionsBuilder.UseNpgsql("Server = localhost; Port = 5432; Database = lscAll-rawsql; User Id = photocube; Password = postgres;");
                //    }
                //    break;

                //MSSQL example (Not used after LSC performance tuning using Raw SQL)
                //case "computerName":
                //    if (connectionString != null)
                //    {
                //        optionsBuilder.UseSqlServer(connectionString);
                //    }
                //    else
                //    {
                //        optionsBuilder.UseSqlServer("Server = (localdb)\\mssqllocaldb; Database = LSC5KSQL; Trusted_Connection = True; AttachDbFileName=C:\\Databases\\LSC5KSQL.mdf");
                //    }
                //    break;
                default:
                    throw new System.Exception("Please specify the path to the database");
            }
        }
    }
}