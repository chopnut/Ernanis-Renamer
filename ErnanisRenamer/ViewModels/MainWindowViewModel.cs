using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.IO;
using System.Windows.Forms;
using System.Diagnostics;
using System.ComponentModel; 

using Prism.Commands;
using Prism.Mvvm;

using Microsoft.Win32;
using Microsoft.Practices.Unity;

using ErnanisRenamer.Services;
using ErnanisRenamer.Models;

// for using the new file/folder select dialog
// codepack core and shell is needed for this to load
using Microsoft.WindowsAPICodePack.Dialogs;
using ErnanisRenamer.Views;

namespace ErnanisRenamer.ViewModels
{

    // Model View of the main window
    public class MainWindowViewModel: BindableBase
    {

        private StatusService _statServ;
        private RenameService _renServ;

        public MainWindowViewModel()
        {
       
            // Set unity container 

            UnityContainer unityContainer = (UnityContainer)System.Windows.Application.Current.Resources["UContainer"];

            unityContainer.RegisterType<OptionsModel>(new ContainerControlledLifetimeManager());
            unityContainer.RegisterType<RenameService>(new ContainerControlledLifetimeManager());

            // Instantiate objects now
            _option = unityContainer.Resolve<OptionsModel>();
            _statServ = unityContainer.Resolve<StatusService>();
            _renServ = unityContainer.Resolve<RenameService>();
            _renServ.SetOptions(_option); // only pass this as a copy not a reference;
          
            // Set actions for the view and just use lambda expression to return true regardless
            BrowseFolder = new DelegateCommand(BrowseExecute, ()=> true);
            RenameNow    = new DelegateCommand(RenExecute, ()=> true);

            // Call initialize(ON_INIT) for initialization just incase it was called from context menu
            // this call for everything status display, call to get folder etc.
            ProcessCode pc = ProcessCode.ON_INIT;
            InitializeAll(ref pc);



            _busy = false;
        }

        // Disable any buttons if renameservice is busy
        private bool _busy;
        public bool Busy
        {
            get { return !(_busy); }
            set { _busy = value; }
        }
        
        private OptionsModel _option;
        public OptionsModel Option
        {
            get { return _option; }
            set {  _option = value; }
        }

        // Used by the rename button to only activate when there are files selected
        private bool _IsSelectedFiles;
        public bool IsSelectedFiles
        {
            get { return _IsSelectedFiles; }
            set { SetProperty(ref _IsSelectedFiles, value); }
        }
        
        // Pop the browse multiple files dialog
        // and return the folder path they all live
        private string BrowseMultipleFilesPop(string paramFolder)
        {
            string tempFolder = paramFolder;
            List<string> sFiles = RenameTool.BrowseByMultipleFiles(tempFolder);
            if (sFiles.Count > 0)
            {
                // Set the folder path after selection

                tempFolder = Path.GetDirectoryName(sFiles.FirstOrDefault());
                _option.FilesSelected = sFiles;
            } 
            return tempFolder;
        }
        // Pop the dialog box to select a folder
        private string BrowseByFolderPop(string paramFolder)
        {
            string tempFolder = RenameTool.BrowseByFolder(paramFolder);
            Object tempObj = null;

            // If folder is empty string return zero list
            if (string.IsNullOrEmpty(tempFolder))
            {
                tempObj = new List<string>();
            }
            else // Otherwise get the files in folder
            {
                tempObj = RenameTool.SelectFilesInFolder(tempFolder, _option.FileTypes.GetFilters());

            }

            if (tempObj is List<string>)
            {
                _option.FilesSelected = tempObj as List<string>;
            }
            return tempFolder;
        }

        // Initialize All
        private void InitializeAll(ref ProcessCode pc)
        {
            ErrorCode enErr = ErrorCode.NO_ERROR;

            InitializeFilesSelected(ref enErr); // Pass an enum error if something goes wrong
            InitializeGUI(pc,enErr);
           
        }

        private void InitializeUpdateSource()
        {
            if (_option.FilesSelected.Count() > 0)
            {
                _IsSelectedFiles = true;
            }
            else
            {
                _IsSelectedFiles = false;
            }
            RaisePropertyChanged("IsSelectedFiles"); // Call the property to update

        }

        // Initialize files selection coming from context menu
        private void InitializeFilesSelected(ref ErrorCode enErr)
        {
            
            if (_option.IsFromContextMenu)
            {
                int isDirOrDoesntExist = RenameTool.isDirectory(_option.FolderPath);
               // Get the files in folder and folder is 1
                if (_option.ArgumentProperty == "-folder" && isDirOrDoesntExist==1)
                {

                    var tempObj = RenameTool.SelectFilesInFolder(_option.FolderPath, _option.FileTypes.GetFilters());
                    if(tempObj is List<string>){
                        _option.FilesSelected = tempObj as List<string>;
                    }
                }
                // Get files from the temp file with list of path in it from context menu
                // int code for 0 means its a file and it exist
                else if (_option.ArgumentProperty == "-files" && isDirOrDoesntExist==0)
                {
                    // read the temp file and fill up the files selected
                    var lines = File.ReadLines(_option.FolderPath);
                    List<string> Files = new List<string>();
                    foreach (var line in lines)
                    {
                        Files.Add(line.ToString());
                    }
                    _option.FilesSelected = Files;

                    // must delete the file afterwards
                    // File.Delete(_option.FolderPath);
                }

                // int value is -1 File or folder doesnt exist 
                if (isDirOrDoesntExist < 0)
                {
                    enErr = ErrorCode.NO_FOLDER_EXIST;
                }

            }
            

        }
        // Initialize the interface looks and status messages
        private void InitializeGUI(ProcessCode pc,ErrorCode enErr)
        {
            int error = 0;
            switch (pc)
            {
                case ProcessCode.ON_INIT:
                    switch (enErr)
                    {
                        case ErrorCode.NO_FOLDER_EXIST:
                            _statServ.OnErrorNoFolderExist(_option.FolderPath, value => _option.Status = value, value => _option.Summary = value);
                            error++;
                            break;
                    }
                    break;
                case ProcessCode.ON_SELECT_BY_FOLDER:
                    break;
                case  ProcessCode.ON_SELECT_BY_MULTIPLE_FILES:
                    break;
                default:
                    break;
            }
            if (error ==0)
            {
                _statServ.OnSelectionFiles(_option.FilesSelected, value => _option.Status = value, value => _option.Summary = value);

            }
            InitializeUpdateSource();
              
        } 
        // PRISM Commands
        // *****************************************
        // Browse and Rename 
        // *****************************************

        public DelegateCommand BrowseFolder { get; set; }
        private void BrowseExecute()
        {
            // This is where you click on browse
            string initialFolder   = _option.FolderPath;
            _option.Busy = true;  // make busy while performing operations
            ErrorCode enErr = ErrorCode.NO_ERROR; // set error reference 
            

            //-----------------------------------------
            // Opening a dialog to select multiple files 
            // and return updated folderpath
            //-----------------------------------------

            
            if (_option.RenameSelectedFiles)
            {
                initialFolder = BrowseMultipleFilesPop(initialFolder);
                InitializeGUI(ProcessCode.ON_SELECT_BY_MULTIPLE_FILES,enErr);
 
            }
            //-----------------------------------------
            // Opening a dialog to select a folder
            // and return updated folderpath
            //-----------------------------------------

            else
            {
                initialFolder = BrowseByFolderPop(initialFolder);
                InitializeGUI(ProcessCode.ON_SELECT_BY_FOLDER,enErr);


            }
            _option.FolderPath = initialFolder;
            _option.Busy = false;

           
        }

        public DelegateCommand RenameNow { get; set; }
        private void RenExecute()
        {
           _renServ.RenameNow(); // to reference the address so it updates the busy property

        }

 
    }

}
