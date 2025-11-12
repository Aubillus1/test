using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace Actualizar.Utils.Clases
{
    public class StepFillConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            double val = System.Convert.ToDouble(value);
            int step = System.Convert.ToInt32(parameter);

            if (val > step) return (SolidColorBrush)Application.Current.FindResource("BarraCargaColor1"); // completado
            if (val == step) return (SolidColorBrush)Application.Current.FindResource("BarraCargaColor1"); // actual
            return (SolidColorBrush)Application.Current.FindResource("BarraCargaColor2"); // pendiente
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => null;
    }

    public class StepTextConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            double val = System.Convert.ToDouble(value);
            int step = System.Convert.ToInt32(parameter);
            return val >= step ? (SolidColorBrush)Application.Current.FindResource("BarraCargaColor2") : (SolidColorBrush)Application.Current.FindResource("BarraCargaColor1");
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => null;
    }

    public class LineFillConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            double val = System.Convert.ToDouble(value);
            int step = System.Convert.ToInt32(parameter);
            return val > step ? (SolidColorBrush)Application.Current.FindResource("BarraCargaColor1") : (SolidColorBrush)Application.Current.FindResource("BarraCargaColor2");
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => null;
    }

    public class StepOutlineVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            double val = System.Convert.ToDouble(value);
            int step = System.Convert.ToInt32(parameter);
            return val == step ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => null;
    }
}
