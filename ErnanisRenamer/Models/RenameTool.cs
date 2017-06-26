using Microsoft.WindowsAPICodePack.Dialogs;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Linq;
using System.Reflection;
using System.Diagnostics;
using System.Text;
using System.Text.RegularExpressions;

namespace ErnanisRenamer.Models
{
    class RenameTool
    {
        // return the selected folder
        public static string BrowseByFolder(string initialFolder = null)
        {
            var dlg = new CommonOpenFileDialog();
            dlg.Title = "Select a folder";
            dlg.IsFolderPicker = true;

            dlg.InitialDirectory = initialFolder;
            dlg.AddToMostRecentlyUsedList = false;
            dlg.AllowNonFileSystemItems = false; // turn back to false
            dlg.Multiselect = false;
            dlg.DefaultDirectory = "";
            dlg.EnsureFileExists = true;
            dlg.EnsurePathExists = true;
            dlg.EnsureReadOnly = false;
            dlg.EnsureValidNames = true;
            dlg.ShowPlacesList = true;

            if (dlg.ShowDialog() != CommonFileDialogResult.Ok)
            {
                return ""; // Just return empty string upon cancel
            }
            return (dlg.FileName);
        }
        // If true is directory otherwise a file
        // 1 = directory 0 = file -1 = doesnt exist
        public static int isDirectory(string filepath)
        {
            try
            {;
            FileAttributes attr = File.GetAttributes(filepath);

                if ((attr & FileAttributes.Directory) == FileAttributes.Directory)
                {

                    return 1;// a directory
                }

                return 0; // a file
            }catch(Exception){
                return -1; // does not exist
            }

        }
        // Return file paths in a list
        public static List<string> BrowseByMultipleFiles(string folderPath = null)
        {
            Microsoft.Win32.OpenFileDialog ofd = new Microsoft.Win32.OpenFileDialog();
            ofd.Title = "Select files to rename";
            ofd.InitialDirectory = folderPath;
            ofd.RestoreDirectory = true;
            ofd.Multiselect = true;
            ofd.Filter = "All files | *.*";

            if (ofd.ShowDialog() == true)
            {
                string[] filenames = ofd.FileNames;
                return filenames.ToList<string>();

            }
            return new List<string>();

        }
        // Select the path according to the folder path return null if the directory doesnt exist or 
        // an error access 
        public static object SelectFilesInFolder(string folderpath,IEnumerable<object> filterObject = null)
        {

            if (!Directory.Exists(folderpath))
            {
                return null;
            }
            List<string> files = new List<string>();
            try
            {

                files = Directory.GetFiles(folderpath).ToList(); // Get all files first
                files = SelectFilesFromListOnFilter(files,filterObject); // Then filter them all using IsMatched() as default
            }
            catch (Exception e)
            {
                string message = string.Format("Exception Error in Selectfilesinfolder: {0}\nProgram is shutting down.", e.Message);
                System.Windows.MessageBox.Show(message);
                return null;
            }finally{

            }
            return files;
        }
        /// <summary>
        /// Returns list of selected files according to filter generated from collection objects
        /// </summary>
        /// <param name="files">List of string to filter</param>
        /// <param name="filterObjects">List of objects that filters</param>
        /// <param name="mname">Name of the method to call upon to for boolean expression </param>
        /// <returns>List of string</returns>
       
        public static List<string> SelectFilesFromListOnFilter(List<string> files, IEnumerable<object> filterObjects = null, string mname = "IsMatched")
        {

            // Debug.WriteLine("MyMethodname: " + mname);
            // Use linq for list to filter filename selected based on filters

            List<string> tempFiles = files.Where(x => {
                string fname = Path.GetFileName(x);

                // Do not include desktop.ini
                if(fname.ToLower() == "desktop.ini")
                {
                    return false;
                }
                try
                {
                    Type valType = x.GetType();

                    // You have to go through all otherwise you will miss other entry that may have 
                    // - as excluded and may turn out not included in the folder
                    int[] counter = new int[filterObjects.Count()];
                    int i = 0;
                    foreach (var obj in filterObjects)
                    {
                       Type t = obj.GetType();
                       MethodInfo caller = t.GetMethod(mname);
                       
                                                    
                        // this is how to turn single object to an array as parameter
                        // this will also call the method according to the methodName from the object FilterType 
                       int b = (int) caller.Invoke(obj, new object[]{ fname });
                       if (b<0) return false;
                       counter[i] = b;
                       i++;
                      
                    }
                    // If the index is found return true;
                    int z = Array.FindIndex(counter, element => element == 1);
                    if (z>=0)
                    {
                        return true;
                    }

                }
                catch (Exception e)
                {
                    MessageBox.Show("Exception: SelectFilesFromListOnFilter() - "+e.Message+"\n"+e.StackTrace+"\nThe program is shutting down.","Error", 
                        MessageBoxButton.OK, 
                        MessageBoxImage.Error);
                    Application.Current.Shutdown();
                }
                // No matter what return false for safety so it doesnt rename any files in folder
                return false;
            }).ToList();

            return tempFiles;
        }
        // Remove special characters 
        public static string RemoveSpecialCharacters(string str)
        {
            StringBuilder sb = new StringBuilder();
            foreach (char c in str)
            {
                if ((c >= '0' && c <= '9') || 
                    (c >= 'A' && c <= 'Z') || 
                    (c >= 'a' && c <= 'z') || 
                    c == '.' || c == '_' || c== '-' || c==' ' || c=='[' || c==']')
                {
                    sb.Append(c);
                }

            }
            return sb.ToString();
        }
        // if string array contains incremental
        public static bool IsContains(string[] option, string search = "i")
        {
            string[] aIncSuff = option.Select((value, index) => value.ToLower()).Where(v => v.Contains(search)).ToArray();
            if (aIncSuff.Count() > 0)
            {
                return true;
            }
            return false;
        }
        // Get options surrounded by square brackets
        public static string[] GetCapturedOptions(string text, string regularExp = @"\[(.*?)\]")
        {
            MatchCollection match = Regex.Matches(text, regularExp);
            string[] capturedOptionsPref = new string[match.Count];

            if (match.Count > 0)
            {
                int i = 0;
                foreach (Match m in match)
                {
                    capturedOptionsPref[i] = m.Groups[1].Value;
                    i++;
                }
            }
            return capturedOptionsPref;
        }
        // Ask to confirm
        public static bool AskToConfirm()
        {
            MessageBoxResult areyousure = MessageBox.Show("Are you sure you want to continue renaming selected files?", "Rename Confirmation", MessageBoxButton.YesNo);
            if (areyousure == MessageBoxResult.Yes)
            {
                return true;
            }
            return false;
        }
    }
}
