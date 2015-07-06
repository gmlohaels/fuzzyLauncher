using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using fuzzyLauncher.Utilitary;
using fuzzyLauncher.Windows;
using Shared.Base;
using Shared.Helpers;

namespace fuzzyLauncher
{


    public partial class MainWindow : Window
    {

        private readonly AppCoreModel core;
        public MainWindow()
        {
            InitializeComponent();
            core = (AppCoreModel)DataContext;
            CompositionTarget.Rendering += CheckForNonActive;
            var hotKey = new HotKey(Key.Space, KeyModifier.Alt, OnHotKeyHandler);
        }

        private void CheckForNonActive(object sender, EventArgs e)
        {
            if (!IsActive && IsVisible)
                Hide();

        }

        private void OnHotKeyHandler(HotKey obj)
        {
            Show();
        }




        private void SearchBox_KeyDown(object sender, KeyEventArgs e)
        {
            //            FancyBalloon balloon = new FancyBalloon();

            //show balloon and close it after 4 seconds



            if (e.Key == Key.Escape)
            {
                if (String.IsNullOrEmpty(core.QueryString))
                    Hide();
                core.QueryString = "";
                //Environment.Exit(0);

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
                if (e.Key == Key.Up && ResultList.SelectedIndex > 0)
                    ResultList.SelectedIndex--;
            }


            if (core.ProcessCompatibleConverter(sender, e))
            {
                SearchBox.CaretIndex = Int32.MaxValue;

            }
            else
            {
                if (core.ProcessQueryKeyEvent(sender, e))
                {
                    Hide();
                }
            }

            SearchBox.Focus();
        }

        private void CenterWindowOnScreen()
        {
            double screenWidth = System.Windows.SystemParameters.PrimaryScreenWidth;
            double screenHeight = System.Windows.SystemParameters.PrimaryScreenHeight;
            double windowWidth = this.Width;
            double windowHeight = this.Height;
            this.Left = (screenWidth / 2) - (windowWidth / 2);
            this.Top = (screenHeight / 2) - (windowHeight / 2);
        }


        private void ResultList_MouseUp(object sender, MouseButtonEventArgs e)
        {
            SearchBox.Focus();
        }

        private void Window_Activated(object sender, EventArgs e)
        {
            Activate();
            CenterWindowOnScreen();
            Focus();
            core.QueryString = "";
            SearchBox.Focus();
        }






        private void SearchBoxOnKeyUp(object sender, KeyEventArgs e)
        {

        }



        private void Window_KeyDown(object sender, KeyEventArgs e)
        {

            if (e.Key == Key.Tab)
            {
                SearchBox.Focus();
                e.Handled = true;
            }

        }

        private void ResultList_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            MessageBox.Show("DBLCLICK");
        }


        private void ActivateSettingsForm(object sender, RoutedEventArgs e)
        {
            var ms = new MainSettings();

            ms.ShowDialog();
        }
    }
}
