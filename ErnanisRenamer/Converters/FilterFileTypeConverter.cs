using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows;
using ErnanisRenamer.Models;
using ErnanisRenamer.Converters;
using System.Diagnostics;
using Microsoft.Practices.Unity;


namespace ErnanisRenamer.Converters
{
    public class FilterFileTypeConverter: IValueConverter
    {
        // Convert from source to UI(Filetypesmodel to UI as string)
        // Converts the hashset string to a readable string with comma delimeter
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
           //  throw new NotImplementedException();
            Type fileType = typeof(FileTypesModel);
            string strVal = "";
            

            if (value.GetType() == fileType) // check type first if its a FileTypesModel
            {
                var obj = (FileTypesModel)value;
                strVal = obj.ConvertToString();
               
                

            }
            //  Debug.WriteLine("Get Type: " + value.GetType()+ " VS "+fileType);
            if (string.IsNullOrEmpty(strVal) && string.IsNullOrWhiteSpace(strVal))
            {
                strVal = "";
            }
            else
            {
                strVal = strVal.Trim();
            }
            return strVal;
        }
        // Convert back from UI to source (UI as string to FileTypesModel)
        // Must return as FileTypeModel from value(string type)
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {

            UnityContainer unityContainer = (UnityContainer)Application.Current.Resources["UContainer"];
            FileTypesModel tempFileType = unityContainer.Resolve<FileTypesModel>();

            if (value.GetType() == typeof(string))
            {
                string tmpValue = (string)value;
                if (!String.IsNullOrEmpty(tmpValue) || !String.IsNullOrWhiteSpace(tmpValue))
                {
                    var tempArray = value.ToString().Split(',');
                    foreach (string strType in tempArray)
                    {
                        tempFileType.Add(strType);
                    }

                    tempFileType.OnUpdateSetTypes(ProcessCode.ON_UPDATE_FILTER,ErrorCode.NO_ERROR);// populate the secondary include exclude list
                    Debug.WriteLine("Debug from FilterTypeConverter: "+tempFileType.GetFilters().Count);
                    
                }

 

            }
            return tempFileType;
        }
    }
}
