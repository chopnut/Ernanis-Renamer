using Prism.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ErnanisRenamer.AttachedProperties
{
    public class WebBrowserBehaviour
    {

        public static void Wui_click(object ob, RoutedEventArgs args)
        {
            UIElement ui = (UIElement)ob;
            MessageBox.Show("Attached Property: " + GetHtml(ui));
            
        }
        public static string GetHtml(DependencyObject obj)
        {
            return (string)obj.GetValue(HtmlProperty);
        }

        public static void SetHtml(DependencyObject obj, string value)
        {
            obj.SetValue(HtmlProperty, value);
        }

        // Using a DependencyProperty as the backing store for Html.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty HtmlProperty =
            DependencyProperty.RegisterAttached("Html", typeof(string), typeof(WebBrowserBehaviour), new PropertyMetadata());

        
    }
}
