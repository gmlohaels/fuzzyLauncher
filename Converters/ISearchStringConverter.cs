using System.Windows.Input;
using Shared;
using Shared.Base;

namespace fuzzyLauncher.Converters
{
    public interface ISearchStringConverter
    {
        bool CanHandleShortkey(KeyEventArgs e);
        bool Convert(KeyEventArgs keys, SearchProviderResult selectedItem, ref string searchString);
    }
}