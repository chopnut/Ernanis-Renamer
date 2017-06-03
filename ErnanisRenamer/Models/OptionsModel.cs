using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ErnanisRenamer.Services;
using Microsoft.Practices.Unity;
using System.Diagnostics;
using System.IO;

namespace ErnanisRenamer.Models
{
    // Using Prism here
    public class OptionsModel : BindableBase
    {
        // Initialize all options here
        public OptionsModel(RenameService rs)
        {
            // Set defaults
            _fileTypes = new FileTypesModel(rs);
            // set defaults for file types
            _fileTypes.Add("*.*");

            _prefix = "";
            _suffix = "";
            _removeFilename = _renameSelectedFiles = _busy = false;
            _removePrefixFirst = _removeSuffixFirst = true;
            _progressWindowButtonLabel = "CANCEL";

            _filesSelected = new List<string>();
            _folderPath = GetFolderPathInit();

        }
        // Default to NULL , -files or -folder this is used when renamertool is called 
        // via context menu as eg. renamer.exe -files c:path\paths.tmp or -folder c:\folderpath\
        public string ArgumentProperty { get; set; }
        
        // Only get path to folder on initialization
        // default to documents

        public string GetFolderPathInit()
        {
            // Get folder path if the argument is set
            string[] args = Environment.GetCommandLineArgs();
            string folder = null;

            if (args.Length>2)
            {
                // If argument is not empty
                // always start with index 1 as the first index is always the running application
                ArgumentProperty = args[1];
                folder = args[2];
                IsFromContextMenu = true;

            }
            else
            {
                // its empty so use my document as default folder
                if (string.IsNullOrEmpty(folder) || !Directory.Exists(folder))
                {
                    // if empty get my documents as default or does not exist
                    folder = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                }
            
                IsFromContextMenu = false;
            }
            return folder;

        }


        public bool IsFromContextMenu { get; set; }

        // ******************************************
        // This are the properties of Progress Windows Model
        private string _progressWindowStatus; // The Message Status
        public string ProgressWindowStatus
        {
            get { return (_progressWindowStatus); }
            set { SetProperty(ref _progressWindowStatus, value); }
        }

        private string _progressWindowPercentage;
        public string ProgressWindowPercentage
        {
            get { return _progressWindowPercentage; }
            set { SetProperty(ref _progressWindowPercentage, value); }
        }

        private int _progressWindowBarWidth;
        public int ProgressWindowWidth
        {
            get { return _progressWindowBarWidth; }
            set { SetProperty(ref _progressWindowBarWidth, value); }
        }

        private string _progressWindowButtonLabel;
        public string ProgressWindowButtonLabel
        {
            get { return _progressWindowButtonLabel; }
            set { SetProperty(ref _progressWindowButtonLabel, value); }
        }
        // ******************************************
        // Store the files selected to be renamed
        private List<string> _filesSelected;
        public List<string> FilesSelected
        {
            get { return _filesSelected; }
            set { SetProperty(ref _filesSelected, value); }
        }

        private bool _busy;
        public bool Busy
        {
            get { return !(_busy); }
            set { SetProperty(ref _busy, value); }
        }
        private FileTypesModel _fileTypes;
        public FileTypesModel FileTypes
        {
            get { return _fileTypes; }
            set { SetProperty(ref _fileTypes,value); }
        }

        private string _folderPath;
        public string FolderPath
        {
            get { return _folderPath; }
            set { SetProperty(ref _folderPath,value); }
        }

        private string _prefix;
        public string Prefix
        {
            get { return _prefix; }
            set { SetProperty(ref _prefix, value); }
        }

        private string _suffix;
        public string Suffix
        {
            get { return _suffix; }
            set { SetProperty(ref _suffix, value); }

        }

        private bool _removePrefixFirst;
        public bool RemovePrefixFirst
        {
            get { return _removePrefixFirst; }
            set { SetProperty(ref _removePrefixFirst, value); }

        }

        private bool _removeSuffixFirst;
        public bool RemoveSuffixFirst
        {
            get { return _removeSuffixFirst; }
            set { SetProperty(ref _removeSuffixFirst, value); }

        }

        private bool _removeFilename;
        public bool RemoveFilename
        {
            get { return _removeFilename; }
            set { SetProperty(ref _removeFilename, value); }

        }
        // whether to rename selected files or select folder
        private bool _renameSelectedFiles;
        public bool RenameSelectedFiles
        {
            get { return _renameSelectedFiles; }
            set { SetProperty(ref _renameSelectedFiles, value); }

        }
        // Message and status
        private string _status;
        public string Status
        {
            get { return _status; }
            set { SetProperty(ref _status, value); }
        }

        private string _summary;
        public string Summary       
        {
            get { return _summary; }
            set { SetProperty(ref _summary, value); }
        }
        
    }
}
