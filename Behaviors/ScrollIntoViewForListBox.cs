using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interactivity;
using System.Windows.Media;

namespace fuzzyLauncher.Behaviors
{
    public class ScrollIntoViewForListBox : Behavior<ListBox>
    {
        /// <summary>
        ///  When Behavior is attached
        /// </summary>
        protected override void OnAttached()
        {
            base.OnAttached();
            AssociatedObject.SelectionChanged += AssociatedObject_SelectionChanged;


        }



        /// <summary>
        /// On Selection Changed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void AssociatedObject_SelectionChanged(object sender,
                                               SelectionChangedEventArgs e)
        {


            if (sender is ListBox)
            {
                ListBox listBox = (sender as ListBox);

                listBox.Dispatcher.BeginInvoke(
                    (Action)(() =>
                    {
                        listBox.UpdateLayout();
                        if (listBox.SelectedItem != null)
                        {
                            listBox.ScrollIntoView(listBox.SelectedItem);
                        }

                        if (listBox.SelectedItems == null || listBox.SelectedIndex == 0)
                        {
                            (((VisualTreeHelper.GetChild(listBox, 0) as Border).Child) as ScrollViewer).ScrollToVerticalOffset(0);

                        }
                    }));


            }
        }
        /// <summary>
        /// When behavior is detached
        /// </summary>
        protected override void OnDetaching()
        {
            base.OnDetaching();
            AssociatedObject.SelectionChanged -=
                AssociatedObject_SelectionChanged;

        }
    }
}
