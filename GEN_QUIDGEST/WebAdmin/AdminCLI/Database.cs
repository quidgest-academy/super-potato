using CommandLine;
using CSGenio.core.persistence;
using CSGenio.framework;
using CSGenio.persistence;
using DbAdmin;
using System;
using System.IO;

namespace AdminCLI
{
    [Verb("backup", HelpText = "Performs a backup of a database")]
    class BackupOptions
    {
        [Option('u', "username", Required = true, HelpText = "Database instance username")]
        public string Username { get; set; }

        [Option('p', "password", Required = true, HelpText = "Database instance password")]
        public string Password { get; set; }

        [Option("year", Required = true, HelpText = "Database year")]
        public string Year { get; set; }

        [Option("location", HelpText = "Folder where the backup is meant to be saved")]
        public string Location { get; set; }
    }

    [Verb("restore", HelpText = "Restores a database backup")]
    class RestoreOptions
    {
        [Option('u', "username", Required = true, HelpText = "Database instance username")]
        public string Username { get; set; }

        [Option('p', "password", Required = true, HelpText = "Database instance password")]
        public string Password { get; set; }

        [Option("year", Required = true, HelpText = "Database year")]
        public string Year { get; set; }

        [Option("file", Required = true, HelpText = "Database backup file name")]
        public string Filename { get; set; }

        [Option("location", HelpText = "Backup save folder location")]
        public string Location { get; set; }
    }

    [Verb("delete-backup", HelpText = "Removes a database backup")]
    class RemoveBackupOptions
    {
        [Option("path", Required = true, HelpText = "Backup save path")]
        public string Path { get; set; }
    }

    [Verb("db-status", HelpText = "Checks the current version of the database against the application version")]
    class DbStatusOptions
    {
        [Option('u', "username", Required = true, HelpText = "Database instance username")]
        public string Username { get; set; }

        [Option('p', "password", Required = true, HelpText = "Database instance password")]
        public string Password { get; set; }

        [Option("schema", HelpText = "Database schema (year)")]
        public string Schema { get; set; }

    }


    partial class AdminCLI
    {
        /// <summary>
        /// Backsups a database
        /// </summary>
        /// <param name="options"></param>
        /// <returns></returns>
        private static int Backup(BackupOptions options)
        {
            try
            {
                string filepath = DBMaintenance.BackupDatabase(options.Year, options.Username, options.Password, options.Location);
                Console.WriteLine("File backed up successfully: " + filepath);

                return 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine("The following error has ocurred while backing up the database: \n" + ex.Message);
                return 1;
            }
        }

        /// <summary>
        /// Restores a database
        /// </summary>
        /// <param name="options"></param>
        /// <returns></returns>
        private static int Restore(RestoreOptions options)
        {
            try
            {
                string location = options.Location;
                if (string.IsNullOrWhiteSpace(location))
                    location = Path.Combine(Environment.CurrentDirectory, "dbs", "backup");

                DBMaintenance.RestoreDatabase(options.Year, options.Username, options.Password, location, options.Filename);
                Console.WriteLine("The desired database backup was restored successfully.");
            }
            catch (Exception e)
            {
                Console.WriteLine("The following error has ocurred while restoring the database: \n" + e.Message);
                return 1;
            }

            return 0;
        }



        /// <summary>
        /// Removes a database backup
        /// </summary>
        /// <param name="options"></param>
        /// <returns></returns>
        private static int RemoveBackup(RemoveBackupOptions options)
        {
            try
            {
                DBMaintenance.DeleteBackup(options.Path);
                Console.WriteLine("The desired database backup was deleted successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine("The following error has ocurred while removing the database backup: \n" + ex.Message);
                return 1;
            }

            return 0;
        }

        /// <summary>
        /// Check if the database is up to date
        /// </summary>
        /// <param name="options"></param>
        /// <returns></returns>
        private static int DbStatus(DbStatusOptions options)
        {
            try
            {
                PersistentSupport sp = PersistentSupport.getPersistentSupport(string.IsNullOrEmpty(options.Schema) ? "0" : options.Schema );
                var reader = new DatabaseVersionReader(sp);

                int dbVersion = reader.GetDbVersionOrZero();
                int dbIndexVersion = reader.GetDbIndexVersion();
                int dbUpgradeVersion = reader.GetDbUpgradeVersion();

                int expectedDbVersion = Configuration.VersionDbGen;
                int expectedIndexVersion = Configuration.VersionIdxDbGen;

                Console.WriteLine($"Database version: {dbVersion}");
                Console.WriteLine($"Expected version: {expectedDbVersion}");

                Console.WriteLine($"Index version: {dbIndexVersion}");
                Console.WriteLine($"Expected index version: {expectedIndexVersion}");

                if (dbVersion == expectedDbVersion && dbIndexVersion >= expectedIndexVersion)
                {
                    Console.WriteLine("Database is up-to-date.");
                    return 0;
                }
                else
                {
                    Console.Error.WriteLine("Database version mismatch.");
                    return 2;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occurred while checking database status: \n" + ex.Message);
                return 1;
            }
        }
    }
}
