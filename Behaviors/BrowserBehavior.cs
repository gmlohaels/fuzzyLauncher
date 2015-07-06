using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace fuzzyLauncher.Behaviors
{
    public class BrowserBehavior
    {
        public static readonly DependencyProperty HtmlProperty = DependencyProperty.RegisterAttached(
                "Html",
                typeof(string),
                typeof(BrowserBehavior),
                new FrameworkPropertyMetadata(OnHtmlChanged));

        [AttachedPropertyBrowsableForType(typeof(WebBrowser))]
        public static string GetHtml(WebBrowser d)
        {
            return (string)d.GetValue(HtmlProperty);
        }

        public static void SetHtml(WebBrowser d, string value)
        {



            d.SetValue(HtmlProperty, value);
        }

        static void OnHtmlChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            WebBrowser webBrowser = dependencyObject as WebBrowser;
            if (webBrowser != null)
            {
                var value = e.NewValue as string ?? "&nbsp;";
                //<meta  /> 
                value = "<meta charset=\"utf-8\" http-equiv=\"X-UA-Compatible\" content=\"IE=edge\" /> " + value + "\r\n\r\n<script>window.onerror = function (msg, url, line) { return true; }; void (0);</script>";
                webBrowser.NavigateToString(value);
            }
        }
    }
}
