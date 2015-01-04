using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BatchRegex
{
    public class RenameAction
    {
        public RenameAction(string oldName, string newName)
        {
            OldName = oldName;
            NewName = newName;
        }


        public string OldName
        {
            get;
            set;
        }
        public string NewName
        {
            get;
            set;
        }

        public void Undo()
        {
            try
            {
                System.IO.File.Move(NewName, OldName);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }

            Console.WriteLine("    " + NewName + " ==> " + OldName);
        }
    }
}
