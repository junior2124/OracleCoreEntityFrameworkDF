using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace OracleEntityFrameworkDF
{
    class Program
    {

        public class APCIContext : DbContext
        {
            public DbSet<WEBCUSTNAMES> WebCustNames { get; set; }
            public DbSet<AUDITTYPEMASTER> AutditTypeMaster { get; set; }

            protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
            {
                optionsBuilder.UseOracle(@"DATA SOURCE=odapri0:1521/prfd;PASSWORD=*****;PERSIST SECURITY INFO=True;USER ID=*s***");
            }
            
            protected override void OnModelCreating(ModelBuilder modelBuilder)
            {
                //base.OnModelCreating(modelBuilder);

                modelBuilder.Entity<WEBCUSTNAMES>(entity =>
                {
                    entity.HasKey(e => e.PERSONID);
                        //.HasName("SYS_C0021822");
                    entity.ToTable("WEBCUSTNAMES", "NAII");
                    //entity.Property(e => e.PERSONID)
                    //    .HasColumnName("PERSONID")
                    //    .HasColumnType("VARCHAR2")
                    //    .HasMaxLength(10);

                    //entity.Property(e => e.LASTNAME)
                    //    .HasColumnName("LASTNAME")
                    //    .HasColumnType("VARCHAR2")
                    //    .HasMaxLength(50);

                    //entity.Property(e => e.FULLNAME)
                    //    .HasColumnName("FULLNAME")
                    //    .HasColumnType("VARCHAR2")
                    //    .HasMaxLength(50);

                    //entity.Property(e => e.MAILADDRESS)
                    //    .HasColumnName("MAILADDRESS")
                    //    .HasColumnType("VARCHAR2")
                    //    .HasMaxLength(50);
                });

                modelBuilder.Entity<AUDITTYPEMASTER>(entity =>
                {
                    entity.HasKey(e => e.AUDITTYPEID);
                        //.HasName("SYS_C0022002");
                    entity.ToTable("AUDITTYPEMASTER", "NAII");
                    //entity.Property(e => e.AUDITTYPEID)
                    //    .HasColumnName("AUDITTYPEID");

                    //entity.HasKey(e => e.AUDITTYPE)
                    //    .HasName("SYS_C0022003");
                    //entity.ToTable("AUDITTYPEMASTER", "NAII");
                    //entity.Property(e => e.AUDITTYPE)
                    //    .HasColumnName("AUDITTYPE")
                    //    .HasColumnType("VARCHAR2")
                    //    .HasMaxLength(50);                    
                });
            }

            public class AUDITTYPEMASTER
            {
                public int AUDITTYPEID { get; set; }
                public string AUDITTYPE { get; set; }                
            }

            public class WEBCUSTNAMES
            {
                public string PERSONID { get; set; }
                public string FIRSTNAME { get; set; }
                public string LASTNAME { get; set; }
                public string FULLNAME { get; set; }
                public string MAILADDRESS { get; set; }
            }
        }
            
        static void Main(string[] args)
        {
            using (var db = new APCIContext())
            {
                // Add record.
                var auditTypeMaster = new APCIContext.AUDITTYPEMASTER { AUDITTYPEID = 2, AUDITTYPE = "Jon" };
                //db.AutditTypeMaster.Add(auditTypeMaster);
                // Update record.
                db.AutditTypeMaster.Update(auditTypeMaster);
                db.SaveChanges();
               
                // Read records.
                var auditTypes = db.AutditTypeMaster;                
                foreach (var type in auditTypes)
                {
                    var data = type.AUDITTYPE;
                }

                var custNames = db.WebCustNames;
                foreach (var name in custNames)
                {
                    var data = name.FIRSTNAME;
                }
            }

            Console.WriteLine("Hello World!");
        }
    }
}
