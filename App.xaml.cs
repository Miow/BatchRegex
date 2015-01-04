using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

using Mono.Options;
using System.Runtime.InteropServices;
using System.ComponentModel;


namespace BatchRegex
{
    public partial class App : Application
    {
        [DllImport("kernel32.dll", SetLastError = true, ExactSpelling = true)]
        private static extern bool AttachConsole(int processId);

        [DllImport("kernel32.dll", SetLastError = true, ExactSpelling = true)]
        private static extern bool FreeConsole();

        [STAThread]
        protected override void OnStartup(StartupEventArgs e)
        {

            AttachConsole(-1);


            if (e.Args != null && e.Args.Length > 0)
            {
                SimpleRegex Regex = new SimpleRegex();
                Regex.Pattern = "(.*)";
                Regex.Format = "$1";
                FolderList FilesContainer = new FolderList();


                OptionSet option_set = new OptionSet();
                option_set.Add("path=", v => { FilesContainer.AddDir(v); });
                option_set.Add("pattern=", v => { Regex.Pattern = v; });
                option_set.Add("format=", v => { Regex.Format = v; });

                try
                {
                    option_set.Parse(e.Args);
                }
                catch (OptionException)
                {
                    show_help("Error - usage is:", option_set);
                }


                if (FilesContainer.DirectoryList.Count == 0)
                {
                    show_help("Path not specified - usage is:", option_set);
                }
                else
                {
                    Console.WriteLine("\nPattern : " + Regex.Pattern);
                    Console.WriteLine("Format :  " + Regex.Format);

                    FilesContainer.ApplyRegex(Regex);
                    FilesContainer.RenameFiles();
                }


                //-path="C:\New Folder" -pattern="(.*)" -format="$1"
                FreeConsole();
                base.OnStartup(e);
            }
            else
            {
                base.OnStartup(e);
                new MainWindow().ShowDialog();
            }
            this.Shutdown();
        }


        public static void show_help(string message, OptionSet option_set)
        {
            Console.Error.WriteLine(message);
            option_set.WriteOptionDescriptions(Console.Error);
            Environment.Exit(-1);
        }  


        private void Application_Exit(object sender, ExitEventArgs e)
        {
            
        }

    }
}
