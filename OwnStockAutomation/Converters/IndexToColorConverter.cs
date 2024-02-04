using System.Collections.Generic;
using System;
using System.Globalization;
using System.Linq;
using static System.Runtime.InteropServices.JavaScript.JSType;


namespace MyOwnStockAutomation.Converters
{
  
        public class IndexToColorConverter : IValueConverter
        {
            public Color EvenColor { get; set; }

            public Color OddColor { get; set; }

            public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
            {
                var collectionView = parameter as CollectionView;

                return collectionView.ItemsSource.Cast<object>().ToList().IndexOf(value) % 2 == 0 ? EvenColor : OddColor;
            }

            public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            {
            throw new NotImplementedException();
            }
      
    }
    
}
