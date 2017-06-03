using ErnanisRenamer.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Windows;
using System.Text.RegularExpressions;
using System.ComponentModel;
using ErnanisRenamer.Views;
using Microsoft.Practices.Unity;


namespace ErnanisRenamer.Services
{
    public delegate void RenameNowDelegate(ProcessCode pc,ErrorCode ec=ErrorCode.NO_ERROR); 
    /* RenameNowDelegate is like a class but a function, you can set up parameter by adding it
     * to the declaration eg. RenameNowDelegate(string x, int i);
     * */
    public class RenameService
    {
        public event RenameNowDelegate renameNowEvent;

        private OptionsModel _option;
        private StatusService _statusService;
        private BackgroundWorker _bgworker;
        private ProgressWindow _wprogress;
      
        public RenameService(StatusService stat)
        {          
            _statusService = stat;
            renameNowEvent += _statusService.OnRenameNow;
            

            // Initialize background worker
            _bgworker = new BackgroundWorker();
            _bgworker.DoWork += _bgworker_DoWork;
            _bgworker.ProgressChanged += _bgworker_ProgressChanged;
            _bgworker.RunWorkerCompleted += _bgworker_RunWorkerCompleted;
            _bgworker.WorkerSupportsCancellation = true;
            _bgworker.WorkerReportsProgress = true;
             
        }
        // When DoWork completed, it will set the e.Result = any type for checking
        void _bgworker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Error!=null) // Theres an error
            {
                MessageBox.Show(e.Error.ToString(), "Error");
                return;
            } // No error
            bool eRes = false;
            if (e.Result != null)
            {
                eRes = (bool)e.Result;
            }
            if (eRes)
            {
                // Successful on renaming files
                _option.ProgressWindowStatus = string.Format("Successfully renamed {0} files.",_option.FilesSelected.Count);
                _option.ProgressWindowButtonLabel = "OK";
                renameNowEvent(ProcessCode.ON_RENAME_SUCCESS);

            }
           
        }

        // Any progress report called from the DoWork function is called in here.
        void _bgworker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            // throw new NotImplementedException();
        }
        // This is the main rename worker to do all the grun work

        void _bgworker_DoWork(object sender, DoWorkEventArgs e)
        {
            // Check if files selected
            int filecount = _option.FilesSelected.Count;

            if (filecount > 0)
            {

                // Initialize and get dates
                int day = DateTime.Now.Day;
                int month = DateTime.Now.Month;
                int year = DateTime.Now.Year;

                int increment = 1;
                float percentDone = 0;

                List<string> newList = new List<string>();

                foreach (string v in _option.FilesSelected)
                {
                    // Check first if theres any cancellation of action otherwise just keep going.
                    if (_bgworker.CancellationPending)
                    {
                         e.Cancel = true;
                        return;
                    }
                    string filename = Path.GetFileNameWithoutExtension(v);
                    string folder = Path.GetDirectoryName(v);
                    string ext = Path.GetExtension(v);
                    string newfilename = filename;
                    string prefix2 = _option.Prefix;
                    string suffix2 = _option.Suffix;


                    // Replace i option with increment and add 0 padding
                    int iPadNum = filecount.ToString().Length;
                    string sPad = increment.ToString().PadLeft(iPadNum,'0');
                    prefix2 = Regex.Replace(prefix2,@"\[i\]",sPad);
                    suffix2 = Regex.Replace(suffix2, @"\[i\]", sPad);
                         
                    // Replace date
                    prefix2 = Regex.Replace(prefix2, @"\[d\]", day.ToString());
                    prefix2 = Regex.Replace(prefix2, @"\[m\]", month.ToString());
                    prefix2 = Regex.Replace(prefix2, @"\[y\]", year.ToString());

                    suffix2 = Regex.Replace(suffix2, @"\[d\]", day.ToString());
                    suffix2 = Regex.Replace(suffix2, @"\[m\]", month.ToString());
                    suffix2 = Regex.Replace(suffix2, @"\[y\]", year.ToString());


                    // Set the progress window control
                    percentDone  =(float)increment / (float)filecount;
                    int totaldone = (int)Math.Ceiling(percentDone * 100);

                    _option.ProgressWindowPercentage = string.Format("{0}%", totaldone);
                    _option.ProgressWindowStatus = string.Format("Renaming {0}", newfilename);
                    _option.ProgressWindowWidth = (int)(percentDone * 170);

                    // If the prefix or suffix is the same as the first or last occurence remove them!
                    if (_option.RemovePrefixFirst)
                    {
                        string pmatch = string.Format("^{0}", prefix2);
                        newfilename = Regex.Replace(newfilename,pmatch,"");
                    }
                    if (_option.RemoveSuffixFirst)
                    {
                        string smatch = string.Format("{0}$", suffix2);
                        newfilename = Regex.Replace(newfilename, smatch, "");


                    }
                    // reconstruct newfilename
                    newfilename = string.Format("{0}{1}{2}", prefix2, newfilename, suffix2);

                    // Only remove file name if there is an [i] increment option in the suffix/prefix
                    if (_option.RemoveFilename)
                    {
                        newfilename = string.Format("{0}{1}", prefix2, suffix2);
                    }
                    
                    // Remove any unwanted character
                    newfilename = RenameTool.RemoveSpecialCharacters(newfilename);

                    // Lastly rename now
                    string fullpath = string.Format(@"{0}\{1}{2}",folder,newfilename,ext);

                    try
                    {
                        System.IO.File.Move(v, fullpath); // This will throw exception if file not found 
                        newList.Add(fullpath);

                        
                    }
                    catch (FileNotFoundException)
                    {
                        MessageBox.Show(string.Format("{0} does not exist. Skipping to the next one.",v),"Notification",MessageBoxButton.OK);
                    }
                    increment++;

                }

                // successfully renaming
                _option.FilesSelected = newList;
                e.Result = true;
                return;
            }

            // nothings been renamed
            e.Result = false;
            return;

        }

        // Ready to rename now the selected files
        public void RenameNow()
        {

            if (renameNowEvent != null)
            {
                renameNowEvent(ProcessCode.ON_RENAME_INIT);
            }
            // Renaming now so set busy to true for the MainWindowViewModel and OptionsModel
            _option.Busy = true;


            // Validation before renaming happens here
            // No renaming to happen if theres no suffix or prefix involved
            if (string.IsNullOrEmpty(_option.Prefix) && string.IsNullOrEmpty(_option.Suffix))
            {
                MessageBox.Show("Prefix and Suffix can't be empty!","Error",MessageBoxButton.OK);
                _option.Busy = false;
                return;
            }

            if (_option.RemoveFilename)
            {
                string[] apOptions = RenameTool.GetCapturedOptions(_option.Prefix);
                string[] asOptions = RenameTool.GetCapturedOptions(_option.Suffix);

                // Get all increment values using lambda select and where
                bool bSuffixi = RenameTool.IsContains(apOptions);
                bool bPrefixi = RenameTool.IsContains(asOptions);
                if (!bSuffixi && !bPrefixi)
                {
                    MessageBox.Show("Remove filename only works if you have [i] option set!", "Error", MessageBoxButton.OK);
                    _option.Busy = false;
                    return;
                }
            }



            // Done validating continue to rename now
            bool yes = RenameTool.AskToConfirm();
            if (yes && !_bgworker.IsBusy)
            {
                _bgworker.RunWorkerAsync();
                _wprogress = new ProgressWindow(_option,ref _bgworker);
                _wprogress.ShowDialog();
               
            }


            _option.Busy = false;
           

        }
        public void SetOptions(OptionsModel opt)
        {
            _option = opt;
            _statusService.Options = opt;
            
        }



    }
}
