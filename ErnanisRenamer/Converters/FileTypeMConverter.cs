using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace ErnanisRenamer.Converters
{
    public class FileTypeMConverter: IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            bool isChecked = (bool)values[0]; // this is value from element cbSelectFiles
            bool busy = false; 
            
            // this is the value from busy property dependency value
            // Always check if value is bool, cause sometimes it will return DependencyProperty.UnSetValue
            // if other components still needs to be loaded.
            if (values[1] is bool)
            {
                busy = (bool)values[1];
            }
            return !isChecked || !busy; // either one must return opposite of the value

        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
