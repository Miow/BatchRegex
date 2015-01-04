using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;


namespace BatchRegex
{
    public class SimpleRegex
    {
        public string Pattern
        {
            set;
            get;
        }
        public string Format
        {
            set;
            get;
        }

        public string Apply(string text)
        {
            if (String.IsNullOrEmpty(text) || String.IsNullOrEmpty(Pattern) || String.IsNullOrEmpty(Format))
                return "";

            try
            {
                return new Regex(Pattern, RegexOptions.None).Replace(text, Format, 1);
            }
            catch (ArgumentException e)
            {
                return "Invalid Pattern : " + e.Message;
            }
            catch (Exception)
            {
                return "";
            }
        }
    }
}
