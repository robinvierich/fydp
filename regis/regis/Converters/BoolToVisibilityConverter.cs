using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;

namespace PlaybackGenerator.Converters
{
    public class BoolToVisibilityConverter : IValueConverter
    {

        public object Convert(object value)
        {
            bool val = System.Convert.ToBoolean(value);

            if (val)
                return System.Windows.Visibility.Visible;
            else
                return System.Windows.Visibility.Collapsed;
        }

        public object ConvertBack(object value)
        {
            if ((bool)value)
                return 0;
            else
                return 1; 
        }
    }
}
