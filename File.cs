using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace BatchRegex
{
    public class File
    {
        public File()
        {
            this.FileName = "";
            IsEnabled = true;
            ResetRegex();
        }
        public File(string fileName, Dir parent)
        {
            IsEnabled = true;
            ParentDirectory = parent;
            this.FileName = fileName;
            ResetRegex();
        }

        public Nullable<Boolean> IsEnabled
        {
            get;
            set;
        }
        private string _FileName;
        public string FileName
        {
            get
            {
                return _FileName;
            }
            set
            {
                _FileName = value;
                ResetRegex();
            }
        }
        public string RegexName
        {
            get;
            private set;
        }
        public Dir ParentDirectory
        {
            get;
            set;
        }


        // Reset the regex modified name to the current file name
        public void ResetRegex()
        {
            this.RegexName = this.FileName;
        }


        public void ApplyRegex(SimpleRegex regex)
        {
            this.RegexName = regex.Apply(this.FileName);
        }





        public RenameAction Rename()
        {
            if (IsEnabled == false)
            {
                return null;
            }


            try
            {
                System.IO.File.Move(ParentDirectory.Path + FileName, ParentDirectory.Path + RegexName);
                RenameAction returnValue = new RenameAction(ParentDirectory.Path + FileName, ParentDirectory.Path + RegexName);
                FileName = RegexName;
                return returnValue;
            }
            catch (System.IO.PathTooLongException)
            {
                Console.WriteLine("The new path is too long.");
            }
            catch (System.IO.DirectoryNotFoundException)
            {
                Console.WriteLine("The file was not found.");
            }
            catch (System.UnauthorizedAccessException)
            {
                Console.WriteLine("Access to the file has been denied.");
            }
            catch (System.NotSupportedException)
            {
                Console.WriteLine("The name's format is invalid.");
            }
            catch (System.ArgumentException)
            {
                Console.WriteLine("New name is empty.");
            }
            catch (System.IO.IOException)
            {
                Console.WriteLine("The source file was not found");
            }

            return null;
        }


    }
}
