using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.Web.CodeGeneration.EntityFrameworkCore;
using ObjectCubeServer.Models.DomainClasses;
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

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //One to one relationship between CubeObject and Photo.
            //If CubeObject is deleted, then photo is also deleted.
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

            // Enforce that there are no duplicate tags within a tagset.
            modelBuilder.Entity<Tag>()
                .HasIndex(t => new { t.Name, t.TagsetId })
                .IsUnique();

            //Many-to-one relationship, if tagset is deleted, then tags are also deleted.
            modelBuilder.Entity<Tagset>()
                .HasMany(ts => ts.Tags)
                .WithOne(t => t.Tagset)
                .OnDelete(DeleteBehavior.Cascade);

            //Ids of Tags are generated when added... Needed for some reason...
            modelBuilder.Entity<Tag>()
               .Property(t => t.Id)
               .ValueGeneratedOnAdd();

            //Many-to-one relationship, if tagset is deleted, then hierarchies are also deleted.
            modelBuilder.Entity<Tagset>()
                .HasMany(ts => ts.Hierarchies)
                .WithOne(h => h.Tagset)
                .OnDelete(DeleteBehavior.Cascade);

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
                .OnDelete(DeleteBehavior.Restrict);

            //Calling on model creating:
            base.OnModelCreating(modelBuilder);
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            //base.OnConfiguring(optionsBuilder);
            OperatingSystem OS = Environment.OSVersion;
            PlatformID platformId = OS.Platform;
            switch (platformId)
            {
                case PlatformID.Unix: //Mac 
                    if (connectionString != null)
                    {
                        optionsBuilder.UseNpgsql(connectionString);
                    }
                    else
                    {
                        optionsBuilder.UseNpgsql("Server = localhost; Port = 5432; Database = lsc2000sql; User Id = photocube; Password = postgres;");
                    }
                    break;
                case PlatformID.Win32NT: //Windows
                    if (connectionString != null)
                    {
                        optionsBuilder.UseSqlServer(connectionString);
                    }
                    else
                    {
                        optionsBuilder.UseSqlServer("Server = (localdb)\\mssqllocaldb; Database = SQLTest; Trusted_Connection = True; AttachDbFileName=C:\\Databases\\SQLTest.mdf");
                    }
                    break;
                default:
                    throw new System.Exception("Please specify the path to the database");
            }
        }
    }
}