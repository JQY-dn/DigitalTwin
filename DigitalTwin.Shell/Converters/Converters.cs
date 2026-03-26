using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace DigitalTwin.Shell.Converters
{
    // bool → Visibility
    public class BoolToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type t, object p, CultureInfo c)
            => value is true ? Visibility.Visible : Visibility.Collapsed;

        public object ConvertBack(object value, Type t, object p, CultureInfo c)
            => value is Visibility.Visible;
    }

    // 枚举 → bool（RadioButton / Tab 选中状态）
    // ConverterParameter 传枚举值名称字符串，如 "Dashboard"
    public class EnumToBoolConverter : IValueConverter
    {
        public object Convert(object value, Type t, object p, CultureInfo c)
            => value?.ToString() == p?.ToString();

        public object ConvertBack(object value, Type t, object p, CultureInfo c)
            => value is true ? p?.ToString() : Binding.DoNothing;
    }
}
