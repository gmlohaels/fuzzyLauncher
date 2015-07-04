using System.ComponentModel.Composition;
using System.Windows.Input;
using Shared.Base;

namespace fuzzyLauncher.Converters
{
    [Export(typeof(ISearchStringConverter))]
    public class ResultDescriptionToSearchBoxConverter : ISearchStringConverter
    {

        public KeyEventArgs KeyHandler;

        public bool CanHandleShortkey(KeyEventArgs e)
        {
            if (Keyboard.IsKeyDown(Key.LeftCtrl))
                return (e.Key == Key.W);


            return false;
        }

        public bool Convert(KeyEventArgs keys, SearchProviderResult selectedItem, ref string searchString)
        {

            if (selectedItem != null)
            {

                searchString = selectedItem.Description;
                return true;
            }
            return false;
        }
    }
}