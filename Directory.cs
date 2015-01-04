using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BatchRegex
{
    public class Dir
    {
        public Dir()
        {
            FileList = new List<File>();
        }
        public Dir(string newPath)
        {
            FileList = new List<File>();
            Path = newPath;
            Update();
        }


        private string _Path;
        public string Path
        {
            get
            {
                return this._Path;
            }
            set
            {
                this.Clear();
                this._Path = value;
                this.Update();
            }
        }


        public List<File> FileList
        {
            get;
            set;
        }
        private void AddFile(string fileName){
            File newFile = new File(fileName, this);
            this.FileList.Add(newFile);
        }

        public void Remove(File fileToRemove)
        {
            FileList.Remove(fileToRemove);
        }


        // Clear every file from the list
        public void Clear()
        {
            this.FileList.Clear();
        }

        // Add all the files from the directory specified in Path to the list
        public void Update() // Throws exceptions
        {
            this.Clear();
            string[] fileNamesList = System.IO.Directory.GetFiles(this.Path);

            foreach (string fileName in fileNamesList)
            {
                this.AddFile(System.IO.Path.GetFileName(fileName));
            }

        }

        // Apply the regex on every file
        public void ApplyRegex(SimpleRegex regex)
        {
            foreach (File file in this.FileList)
            {
                file.ApplyRegex(regex);
            }
        }

        // Rename the files
        public Stack<RenameAction> RenameFiles()
        {
            Console.WriteLine("[" + Path + "]");

            Stack<RenameAction> history = new Stack<RenameAction>();
            foreach (File file in this.FileList)
            {
                Console.WriteLine("    " + file.FileName + " ==> " + file.RegexName);
                RenameAction result = file.Rename();
                if (result != null)
                {
                    history.Push(result);
                }
            }

            return history;
        }


    }
}
