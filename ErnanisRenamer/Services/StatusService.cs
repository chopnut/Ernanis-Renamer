using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using ErnanisRenamer.Models;
using System.IO;

namespace ErnanisRenamer.Services
{
    public class StatusService
    {
        public StatusService(){
        }
        private OptionsModel _options;
        public OptionsModel Options
        {
            get { return _options; }
            set { _options = value; }
        }

        // After a rename update the status and summary
        public void OnRenameNow(ProcessCode pc,ErrorCode ec)
        {
            if (pc == ProcessCode.ON_RENAME_SUCCESS && _options != null)
            {
                int iFileCount = _options.FilesSelected.Count;
                _options.Summary = string.Format("{0} files has been renamed, do you want to rename them again?", iFileCount);
                _options.Status = OnStatusFilesSelected(_options.FilesSelected);
            }
        }
        private string OnStatusFilesSelected(List<string> files)
        {
            if (files.Count > 0)
            {
                StringBuilder statMsg = new StringBuilder();

                foreach (string str in files)
                {
                    string fname = Path.GetFileName(str);
                    statMsg.AppendLine(fname);
                }
                return statMsg.ToString();
            } return "";
        }
        public void OnSelectionFiles(List<string> files , Action<string> setStatus, Action<string> setSummary)
        {
            if (files.Count > 0)
            {
                string sumMsg = string.Format("{0} files to be rename.", files.Count);
                string statMsg = OnStatusFilesSelected(files);

                setSummary(sumMsg);
                setStatus(statMsg);

            }
            else
            {
                setSummary("No files selected or folder empty.");
                setStatus("");
            }

        }


        public void OnErrorNoFolderExist(string sFolderPath, Action<string> setStatus, Action<string> setSummary)
        {
           // throw new NotImplementedException();
            setStatus(sFolderPath);
            setSummary("No folder/file exist");
        }
    }
}
