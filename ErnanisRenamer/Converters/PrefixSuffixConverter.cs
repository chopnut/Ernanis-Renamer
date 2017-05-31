using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using ErnanisRenamer.Models;

namespace ErnanisRenamer.Converters
{
    class PrefixSuffixConverter :IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            string presuff = (string)value;
            return RenameTool.RemoveSpecialCharacters(presuff);
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            string presuff = (string)value;
            return RenameTool.RemoveSpecialCharacters(presuff);
        }
    }
}
