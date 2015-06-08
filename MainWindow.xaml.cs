using System;
using System.Windows;
using System.Windows.Input;

namespace fuzzyLauncher
{


    public partial class MainWindow : Window
    {

        private readonly AppCoreModel core;
        public MainWindow()
        {
            InitializeComponent();
            core = new AppCoreModel();
            DataContext = core;



        }

        private void SearchBox_KeyDown(object sender, KeyEventArgs e)
        {


            if (e.Key == Key.Escape)
            {
                if (String.IsNullOrEmpty(core.QueryString))
                    Environment.Exit(0);
                core.QueryString = "";
            }

            if (e.Key == Key.PageDown || e.Key == Key.PageUp)
            {
                var key = e.Key;
                var target = ResultList;
                var routedEvent = Keyboard.KeyDownEvent;

                target.RaiseEvent(new KeyEventArgs(Keyboard.PrimaryDevice, PresentationSource.FromVisual(target), 0, key) { RoutedEvent = routedEvent });
                e.Handled = true;
            }

            if (ResultList.Items.Count > 0)
            {
                if (e.Key == Key.Down)
                    ResultList.SelectedIndex++;
                if (e.Key == Key.Up)
                    ResultList.SelectedIndex--;


                
            }
            SearchBox.Focus();
            // (DataContext as AppCoreModel).Execute(null);
        }

        private void ResultList_OnPreviewKeyDown(object sender, KeyEventArgs e)
        {

        }


        private void ResultList_MouseUp(object sender, MouseButtonEventArgs e)
        {
            SearchBox.Focus();
        }

        private void Window_Activated(object sender, EventArgs e)
        {
            SearchBox.Focus();
        }

        private void Window_Deactivated(object sender, EventArgs e)
        {
            // Hide();
        }



    }
}
