using System;
using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;

namespace ErnanisRenamer.Models
{
    public class FilterType
    {
        public FilterType()
        {
            isExcluded = false;
        }
        

        public string Content { get; set; }
        public bool isExcluded { get; set; }

        // Check if the filename is matched against the content
        public int IsMatched(string filename)
        {
            try
            {

                string pat = WildCardToRegEx();
                Match result = Regex.Match(filename, pat, RegexOptions.IgnoreCase);
                if (result.Success)
                {
                    // If its excluded return -1
                    if (isExcluded)
                    {
                        return -1;
                    }
                    // Debug.WriteLine("Matched:  " + pat + " -> " + Content + "(" + filename + ")=" + result.Success);
                    return 1;

                }
                // Debug.WriteLine("Matched:  " + pat + " -> " + Content + "(" + filename + ")=" + result.Success);
            }
            catch (Exception e)
            {
                Debug.WriteLine("Exception o(FilterType): m(IsMatched) - " +e.Message);
            }

            return 0;
           
        }
        // Convert regular expression using wild cards
        // and escape literal symbols with slashes
        public string WildCardToRegEx()
        {
            string removeWhiteSpace = Content.Replace(" ", String.Empty);
            string pattern = Regex.Escape(removeWhiteSpace)

                .Replace(@"\*", ".*")
                .Replace(@"\?", ".")+"$";
            return pattern;

        }
    }
}
