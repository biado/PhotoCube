﻿using Microsoft.EntityFrameworkCore;
using ObjectCubeServer.Models.DomainClasses;
using ObjectCubeServer.Models.DomainClasses.Tag_Types;
using ObjectCubeServer.Models.PublicClasses;

namespace ObjectCubeServer.Models.Contexts
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

        public ObjectContext(DbContextOptions<ObjectContext> options) : base(options)
        {

        }

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
        public DbSet<SingleObjectCell> SingleObjectCells { get; set; }
        public DbSet<PublicCubeObject> PublicCubeObjects { get; set; }

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
            
            modelBuilder.Entity<TimestampTag>()
                .HasOne<Tagset>()
                .WithMany()
                .HasForeignKey(tst => tst.TagsetIdReplicate);

            modelBuilder.Entity<DateTag>()
                .HasOne<Tagset>()
                .WithMany()
                .HasForeignKey(dt => dt.TagsetIdReplicate);

            modelBuilder.Entity<TimeTag>()
                .HasOne<Tagset>()
                .WithMany()
                .HasForeignKey(tt => tt.TagsetIdReplicate);

            modelBuilder.Entity<IntegerTag>()
                .HasOne<Tagset>()
                .WithMany()
                .HasForeignKey(it => it.TagsetIdReplicate);

            modelBuilder.Entity<FloatTag>()
                .HasOne<Tagset>()
                .WithMany()
                .HasForeignKey(ft => ft.TagsetIdReplicate);

            //Enforce that a typed tag is unqiue within a tagset
            modelBuilder.Entity<AlphanumericalTag>()
                .HasIndex(at => new { at.TagsetIdReplicate, at.Name })
                .IsUnique();
            
            modelBuilder.Entity<TimestampTag>()
                .HasIndex(tst => new { tst.TagsetIdReplicate, tst.Name })
                .IsUnique();
            
            modelBuilder.Entity<DateTag>()
                .HasIndex(dt => new { dt.TagsetIdReplicate, dt.Name })
                .IsUnique();

            modelBuilder.Entity<TimeTag>()
                .HasIndex(tt => new { tt.TagsetIdReplicate, tt.Name })
                .IsUnique();

            modelBuilder.Entity<IntegerTag>()
                .HasIndex(it => new { it.TagsetIdReplicate, it.Name })
                .IsUnique();

            modelBuilder.Entity<FloatTag>()
                .HasIndex(ft => new { ft.TagsetIdReplicate, ft.Name })
                .IsUnique();

            //Calling on model creating:
            base.OnModelCreating(modelBuilder);
        }
    }
}