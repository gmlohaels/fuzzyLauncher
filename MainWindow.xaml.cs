using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using Shared.Base;

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


        readonly List<Key> modifiers = new List<Key> { Key.LeftAlt, Key.RightAlt, Key.LeftCtrl, Key.RightCtrl };

        public bool IsProviderKey(KeyEventArgs e)
        {

            if (modifiers.Any(Keyboard.IsKeyDown))
            {
                return true;
            }


            if (e.Key == Key.Enter)
                return true;


            return false;
        }

        private void SearchBox_KeyDown(object sender, KeyEventArgs e)
        {


            if (e.Key == Key.Escape)
            {
                if (String.IsNullOrEmpty(core.QueryString))
                    Hide();
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
                if (e.Key == Key.Up && ResultList.SelectedIndex > 0)
                    ResultList.SelectedIndex--;
            }
            SearchBox.Focus();

        }




        private void ResultList_MouseUp(object sender, MouseButtonEventArgs e)
        {
            SearchBox.Focus();
        }

        private void Window_Activated(object sender, EventArgs e)
        {
            Activate();
            Focus();
            core.QueryString = "";
            SearchBox.Focus();
        }




        private void SearchBoxOnKeyUp(object sender, KeyEventArgs e)
        {
            if (IsProviderKey(e))
            {

                //if (core.SelectedItem == null)
                //    core.SelectedItem = ResultList.Items[0] as SearchProviderResult;

                if (core.SelectedItem != null)
                {
                    core.SelectedItem.GotKeyboardEvent(sender, e);
                    e.Handled = true;
                }
            }
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







    }
}
