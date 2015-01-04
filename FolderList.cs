using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BatchRegex
{
    public class FolderList
    {
        public FolderList()
        {
            DirectoryList = new List<Dir>();
            History = new Stack<Stack<RenameAction>>();
        }


        public List<Dir> DirectoryList
        {
            get;
            set;
        }
        private Stack<Stack<RenameAction>> History
        {
            get;
            set;
        }

        public void AddDir(string path)
        {
            if (!String.IsNullOrWhiteSpace(path))
            {
                if(path[path.Length - 1] != '\\')
                {
                    path = path + "\\";
                }

                foreach (Dir directory in DirectoryList)
                {
                    if (path == directory.Path)
                    {
                        return;
                    }
                }



                try
                {
                    DirectoryList.Add(new Dir(path));
                }
                catch (System.IO.DirectoryNotFoundException)
                {
                    Console.WriteLine("The specified folder could not be found.");
                }
                catch (System.IO.IOException)
                {
                    Console.WriteLine("The path is a file name.");
                }
                catch (Exception e)
                {
                    Console.WriteLine("The path is invalid:\n" + e.Message);
                }
            }
        }


        public void Clear()
        {
            DirectoryList.Clear();
        }


        public void UpdateAll()
        {
            foreach (Dir directory in DirectoryList)
            {
                try
                {
                    directory.Update();
                }
                catch (Exception e)
                {
                    Console.WriteLine("Error while updating \"" + directory.Path + "\":\n" + e.Message + ".");
                }
            }
        }

        public void ApplyRegex(SimpleRegex regex)
        {
            foreach (Dir directory in DirectoryList)
            {
                directory.ApplyRegex(regex);
            }
        }

        public void RenameFiles()
        {
            foreach (Dir directory in DirectoryList)
            {
                try
                {
                    Stack<RenameAction> lastAction = directory.RenameFiles();
                    History.Push(lastAction);
                }
                catch (Exception e)
                {
                    Console.WriteLine("Error while saving history \"" + directory.Path + "\":\n" + e.Message);
                }
            }
        }

        public void UndoLast()
        {
            Console.WriteLine("[Undo]");
            if (History.Count != 0)
            {
                foreach (RenameAction rename in History.Pop())
                {
                    rename.Undo();
                }
            }
        }

        public void UndoAll()
        {
            while (History.Count() != 0)
            {
                this.UndoLast();
            }
        }


    }
}
