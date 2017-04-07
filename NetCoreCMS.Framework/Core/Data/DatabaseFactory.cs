﻿using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using NetCoreCMS.Framework.Core.Mvc.Models;
using NetCoreCMS.Framework.Utility;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace NetCoreCMS.Framework.Core.Data
{
    public class DatabaseFactory 
    {
        private static string _sqLiteConString = "Data Source={0}\\{1}.db";
        private static string _sqlLocalDb = "Server=(localdb)\\mssqllocaldb;Database=NetCoreCMS.Web.db;Trusted_Connection=True;MultipleActiveResultSets=true";
        private static string _mySqlConString = "server={0};port={1};database={2};userid={3};pwd={4};sslmode=none;";
        private static string _msSqlConString = "Data Source={0}; Initial Catalog={1}; User Id = {2}; Password = {3}; MultipleActiveResultSets=true";
        private static string _pgSqlConString = "Host={0}; Port={1}; Database={2}; User ID={3}; Password={4}; Pooling=true;";

        public static string GetConnectionString(IHostingEnvironment env, DatabaseEngine engine, DatabaseInfo dbInfo)
        {
            switch (engine)
            {
                case DatabaseEngine.MsSql:
                    if (string.IsNullOrEmpty(dbInfo.DatabasePort))
                        return string.Format(_msSqlConString, dbInfo.DatabaseHost, dbInfo.DatabaseName, dbInfo.DatabaseUserName, dbInfo.DatabasePassword);
                    else
                        return string.Format(_msSqlConString, dbInfo.DatabaseHost + "," + dbInfo.DatabasePort, dbInfo.DatabaseName, dbInfo.DatabaseUserName, dbInfo.DatabasePassword);

                case DatabaseEngine.MsSqlLocalStorage:
                    return _sqlLocalDb;

                case DatabaseEngine.MySql:
                    if (string.IsNullOrEmpty(dbInfo.DatabasePort))
                        return string.Format(_mySqlConString, dbInfo.DatabaseHost, "3306", dbInfo.DatabaseName, dbInfo.DatabaseUserName, dbInfo.DatabasePassword);
                    else
                        return string.Format(_mySqlConString, dbInfo.DatabaseHost, dbInfo.DatabasePort, dbInfo.DatabaseName, dbInfo.DatabaseUserName, dbInfo.DatabasePassword);

                case DatabaseEngine.PgSql:
                    if (string.IsNullOrEmpty(dbInfo.DatabasePort))
                        return string.Format(_pgSqlConString, dbInfo.DatabaseHost, "5432", dbInfo.DatabaseName, dbInfo.DatabaseUserName, dbInfo.DatabasePassword);
                    else
                        return string.Format(_pgSqlConString, dbInfo.DatabaseHost, dbInfo.DatabasePort, dbInfo.DatabaseName, dbInfo.DatabaseUserName, dbInfo.DatabasePassword);
                case DatabaseEngine.SqLite:
                    var path = GlobalConfig.ContentRootPath;
                    return string.Format(_sqLiteConString, Path.Combine(path,"Data"), "NetCoreCMS.Database.SqLite");
                default:
                    return "";

            }
        }

        public static bool CreateDatabase(IHostingEnvironment env, DatabaseEngine database, DatabaseInfo databaseInfo)
        {
            switch (database)
            {
                case DatabaseEngine.MsSql:
                    break;
                case DatabaseEngine.MsSqlLocalStorage:
                    break;
                case DatabaseEngine.MySql:
                    break;
                case DatabaseEngine.PgSql:
                    break;
                case DatabaseEngine.SqLite:
                    string path = GlobalConfig.ContentRootPath;
                    path = Path.Combine(path, "Data");
                    string dbFileName = Path.Combine(path, "NetCoreCMS.Database.SqLite.db");
                    using (var dbFile = File.Create(dbFileName)) { };                    
                    return File.Exists(dbFileName);
            }
            return false;
        }

        private static void RegisterEntities(ModelBuilder modelBuilder, IEnumerable<Type> typeToRegisters)
        {
            var entityTypes = typeToRegisters.Where(x => x.GetTypeInfo().IsSubclassOf(typeof(BaseModel)) && !x.GetTypeInfo().IsAbstract);
            foreach (var type in entityTypes)
            {
                modelBuilder.Entity(type);
            }
        }

        public static bool InitilizeDatabase(DatabaseEngine database, string connectionString)
        {
            var builder = new DbContextOptionsBuilder<NccDbContext>();
            builder.UseSqlite(connectionString, options => options.MigrationsAssembly("NetCoreCMS.Framework"));
            var dbContext = new NccDbContext(builder.Options);
            var migrator = dbContext.Database.GetPendingMigrations();
            dbContext.Database.Migrate();
            return dbContext.Database.EnsureCreated();
        }
    }
}
