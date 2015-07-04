using System.ComponentModel.Composition.Primitives;
using System.Windows.Input;
using Shared.Helpers;
using System.ComponentModel.Composition;
using Shared;
using Shared.Base;

namespace fuzzyLauncher.Converters
{
    [Export(typeof(ISearchStringConverter))]
    public class EngToCyrylicTwoWaysConverter : ISearchStringConverter
    {

        public KeyEventArgs KeyHandler;

        public bool CanHandleShortkey(KeyEventArgs e)
        {
            if (Keyboard.IsKeyDown(Key.LeftCtrl))
                return (e.Key == Key.R);
            return false;
        }

        public bool Convert(KeyEventArgs keys, SearchProviderResult selectedItem, ref string searchString)
        {
            searchString = CyrillicHelper.ConvertLayout(searchString);

            return true;
        }

    }
}