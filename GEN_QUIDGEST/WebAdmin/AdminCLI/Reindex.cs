using CommandLine;
using ExecuteQueryCore;
using DbAdmin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace AdminCLI
{
    [Verb("reindex", HelpText = "Reindexes a database")]
    class ReindexOptions
    {
        [Option('u', "username", Required = true, HelpText = "Database instance username")]
        public string Username { get; set; }

        [Option('p', "password", Required = true, HelpText = "Database instance password")]
        public string Password { get; set; }

        [Option('f', "full", HelpText = "Make a full reindexation")]
        public bool Full { get; set; }

        [Option("category", HelpText = "Reindex per category")]
        public string Category { get; set; }

        [Option("single-script", HelpText = "Specify a single script to reindex")]
        public string SingleScript { get; set; }

        [Option("multi-script", HelpText = "Specify multiple scripts to reindex")]
        public IEnumerable<string> MultiScript { get; set; }
        
        [Option('y',"year", HelpText = "Specify the DataSystem (if not specified the default is used)")]
        public string Year { get; set; }
    }

    [Verb("list-reindex-scripts", HelpText = "Lists all the reindexation scripts")]
    class ListReindexScriptsOptions
    {
        [Option('c', "category", HelpText = "Reindex scripts category")]
        public string Category { get; set; }
    }

    partial class AdminCLI
    {
        /// <summary>
        /// Reindexes a database from a solution
        /// </summary>
        /// <param name="options"></param>
        /// <returns></returns>
        private static int Reindex(ReindexOptions options)
        {
            ManualResetEvent Wait = new ManualResetEvent(false);

            RdxParamUpgradeSchema RdxItem = null;

            ChangedEventHandler rdxEvent = (sender, eventArgs, status) =>
            {
                double percStatus = status.Percentage();
                RdxItem.Progress = status;
                if (status.State == RdxProgressStatus.RUNNING)
                {
                    string curr_script = "Unknown Script...";
                    if (!string.IsNullOrEmpty(status.ActualScript))
                        curr_script = " Running script " + status.ActualScript + "...";
                    else if (percStatus == 0)
                        curr_script = "Starting...";
                    else if (percStatus == 100)
                        curr_script = "Done :)";
                    
                    Console.WriteLine(string.Format("{0}% - {1}", percStatus.ToString("0"), curr_script));
                    return;
                }

                if (status.State == RdxProgressStatus.SUCCESS)
                    Console.WriteLine("Done :)");
                else if (status.State == RdxProgressStatus.ERROR)
                {
                    //There was an error, show it                    
                    Console.Error.WriteLine($"An error ocurred on script {status.ActualScript}:\n{status.Message}");
                }
                else if (status.State == RdxProgressStatus.CANCELLED)
                    Console.WriteLine("Operation Cancelled");
                
                Wait.Set();
            };

            RdxItem = dBMaintenance.StartReindexation(
                options.Username,
                options.Password, 
                options.SingleScript,
                options.MultiScript.ToList(),
                options.Category,
                options.Full,
                rdxEvent,
                new CancellationToken(),
                options.Year
            );

            Wait.WaitOne(); //Wait for the reindexation to finish
            //In case there was an error
            if(RdxItem != null && RdxItem.Progress.State == RdxProgressStatus.ERROR)
                return 1;
            else
                return 0;
        }

        /// <summary>
        /// Reindexes a database from a solution
        /// </summary>
        /// <param name="options"></param>
        /// <returns></returns>
        private static int ListReindexScripts(ListReindexScriptsOptions options)
        {
            ReindexOrder rdxOrd = dBMaintenance.GetReindexScripts();

            foreach(ReIndexGroup group in rdxOrd.Reindexgroups)
            {
                if((!string.IsNullOrEmpty(options.Category) && group.Name.ToLower() ==  options.Category.ToLower()) || string.IsNullOrEmpty(options.Category))
                {
                    Console.WriteLine($"{group.Name}:");

                    foreach(string script in group.GroupItems)
                    {
                        Console.WriteLine($"   - {script}");
                    }

                    //Leave line in between
                    Console.WriteLine("");
                }                
            }

            return 0;
        }
    }
}
