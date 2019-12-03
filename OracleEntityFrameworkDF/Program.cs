using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Oracle.EntityFrameworkCore;
using Oracle.ManagedDataAccess.Client;
using System.Linq;
using AutoMapper;

namespace OracleEntityFrameworkDF
{
    class Program
    {

        public class APCIContext : DbContext
        {
            public DbSet<WEBCUSTNAMES> WebCustNames { get; set; }
            public DbSet<AUDITTYPEMASTER> AutditTypeMaster { get; set; }
            public DbSet<COMPANYADDRESS> CompanyAddress { get; set; }            
            public DbSet<COUNTRYMASTER> CountryMaster { get; set; }

            protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
            {
                optionsBuilder.UseOracle(@"DATA SOURCE=odapri0:1521/prfd;PASSWORD=*****;PERSIST SECURITY INFO=True;USER ID=****");
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

                modelBuilder.Entity<COMPANYADDRESS>(entity =>
                {
                    entity.ToTable("COMPANYADDRESS", "NAII");
                    entity.HasKey(e => new {  e.COMPANYID, e.MAINADDRESSESID });                                        
                });

                modelBuilder.Entity<COUNTRYMASTER>(entity =>
                {
                    entity.ToTable("COUNTRYMASTER", "NAII");
                    entity.HasKey(e => e.COUNTRYCODE);
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

            public class COMPANYADDRESS
            {
                public int COMPANYID { get; set; }
                public int MAINADDRESSESID { get; set; }
                public string STREETADDRESS { get; set; }                
                public string COUNTRYCODE { get; set; }
                public string LAT { get; set; }
                public string LNG { get; set; }
            }

            public class COMPANYADDRESSDETAILS
            {
                public int COMPANYID { get; set; }
                public int MAINADDRESSESID { get; set; }
                public string STREETADDRESS { get; set; }
                public string DESCRIPTION { get; set; }
                public string COUNTRYCODE { get; set; }
                public string LAT { get; set; }
                public string LNG { get; set; }
            }

            public class COUNTRYMASTER
            {
                public string COUNTRYCODE { get; set; }                
                public string DESCRIPTION { get; set; }
                public string USERID { get; set; }
                public string DATETIME { get; set; }
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
                //db.AutditTypeMaster.Update(auditTypeMaster);
                //db.SaveChanges();

                // Read records.
                //var auditTypes = db.AutditTypeMaster;                
                //foreach (var type in auditTypes)
                //{
                //    var data = type.AUDITTYPE;
                //}

                //var custNames = db.WebCustNames;
                //foreach (var name in custNames)
                //{
                //    var data = name.FIRSTNAME;
                //}

                //var query = db.CompanyAddress.Find(678, 33715);

                //// Eager Loading 
                //var query2 = (from c in db.CompanyAddress
                //              where c.COMPANYID == 678 & c.MAINADDRESSESID == 33715
                //              select c).ToList<APCIContext.COMPANYADDRESS>();

                //Join Example.
                var query2 = (from c in db.CompanyAddress
                              join cm in db.CountryMaster on c.COUNTRYCODE equals cm.COUNTRYCODE
                              where c.COMPANYID == 678 & c.MAINADDRESSESID == 33715
                              select new APCIContext.COMPANYADDRESSDETAILS() {COMPANYID = c.COMPANYID,
                                                                              MAINADDRESSESID = c.MAINADDRESSESID,
                                                                              STREETADDRESS = c.STREETADDRESS,
                                                                              COUNTRYCODE = c.COUNTRYCODE,
                                                                              LAT = c.LAT,
                                                                              LNG = c.LNG,
                                                                              DESCRIPTION = cm.DESCRIPTION }).ToList();



                // FROMSQL EXAMPLES: 
                // STORED PROCEDURE       
                var P_CUR = new OracleParameter("curParam", OracleDbType.RefCursor, System.Data.ParameterDirection.Output);
                var refCur = db.CompanyAddress.FromSql("BEGIN GET_COMPADDR_PROC(:curParam); END;", new object[] { P_CUR }).ToList();

                // SQL JOIN
                //var query3 = db.CompanyAddress
                //             .FromSql("SELECT C.*, CM.DESCRIPTION AS COUNTRY FROM COMPANYADDRESS C " +
                //                      "JOIN COUNTRYMASTER CM ON CM.COUNTRYCODE = C.COUNTRYCODE AND STREETADDRESS = '1800 West Larchmont Avenue' ")
                //             .ToList<APCIContext.COMPANYADDRESS>();
            }

            Console.WriteLine("Hello World!");
        }        
    }    
}
