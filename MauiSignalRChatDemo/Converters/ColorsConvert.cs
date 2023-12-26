using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MauiSignalRChatDemo
{
    public class ColorChangedConverter : IValueConverter
    {
        public ColorChangedConverter()
        {
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {

            if ((int)value == 1)
            {
                return Colors.Red;
            }
            else if ((int)value == 2)
            {
                return Colors.Blue;
            }
            else
            {
                return Colors.Green;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
