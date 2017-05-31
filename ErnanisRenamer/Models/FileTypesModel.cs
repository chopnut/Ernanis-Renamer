using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using ErnanisRenamer.Services;
using Microsoft.Practices.Unity;

namespace ErnanisRenamer.Models
{
    public class FileTypesModel
    {
        // Initialize types hashset dont make duplicates of value
        // any added value if duplicate is found its removed.
        // This is the raw and gets displayed in the UI
        private HashSet<string> _fileTypes = new HashSet<string>();

        // FilterType the one that holds the text and other option and the one to match against
        private List<FilterType> _filters = new List<FilterType>();

        public FileTypesModel(RenameService rs)
        {
            if (rs != null)
            {
                // Add the method when RenameNowEvent is triggered so to update the filefilterstring
                rs.renameNowEvent += OnUpdateSetTypes;
            }


        }

        public List<FilterType> GetFilters()

        {
            OnUpdateSetTypes(ProcessCode.ON_UPDATE_FILTER, ErrorCode.NO_ERROR);
            return _filters;
        }
 

        // Add string file type
        public bool Add(string type)
        {

            return (_fileTypes.Add(type));
        }
        // Get all collection of file types
        public HashSet<string> GetTypes()
        {
            return _fileTypes;
        }
        // Convert the collection to string delimited by comma
        public string ConvertToString()
        {
            return (string.Join(",",_fileTypes.ToArray()));
        }

        // When all types has lost focus included and excluded
        public void OnUpdateSetTypes(ProcessCode pc, ErrorCode ec)
        {

            _filters.Clear();
            foreach (string stype in _fileTypes)
            {
                char firstChar  = stype[0];

                /*
                 * eg: (*.*) all files, (*text*.*) any files with text occurence anywhere
                 *     (*.pdf) any pdf files (-.txt) no text files allowed (-filename.txt) exclude filename.txt
                 *     Note: using * asterisk with nothing before it will cause an exception when matched
                 * */
                FilterType ft = new FilterType();
               
                if (firstChar == '-')
                {
                    ft.isExcluded = true;
                    ft.Content = stype.Substring(1); // get rid of the minus(-) at the beginning
                   
                }
                else
                {
                    ft.Content = stype;
                 
                }

                _filters.Add(ft);

               
            }
            

        }

    }
}
