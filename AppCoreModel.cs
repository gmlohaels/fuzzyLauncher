using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Windows.Data;
using System.Windows.Input;
using fuzzyLauncher.Annotations;
using fuzzyLauncher.Converters;
using fuzzyLauncher.Engine;
using fuzzyLauncher.Mef;
using fuzzyLauncher.Utilitary;
using Shared;
using Shared.Base;

namespace fuzzyLauncher
{
    public class AppCoreModel : INotifyPropertyChanged
    {
        protected bool SetProperty<T>(ref T storage, T value, [CallerMemberName] String propertyName = null)
        {
            if (Equals(storage, value))
            {
                return false;
            }

            storage = value;
            OnPropertyChanged(propertyName);
            return true;
        }



        [ImportMany]
        private readonly List<ISearchStringConverter> converters = new List<ISearchStringConverter>();



        public SearchEngine SearchEngine = new SearchEngine();
        private readonly FastObservableCollection<SearchProviderResult> rawResultList;


        public ICollectionView ResultList { get; private set; }


        public AppCoreModel()
        {
            rawResultList = new FastObservableCollection<SearchProviderResult>();


            var safeCatalog = new SafeDirectoryCatalog("Providers");
            var container = new CompositionContainer(safeCatalog.Catalog);

            container.ComposeParts(SearchEngine);
            container.ComposeParts(this);


            SearchEngine.OnProviderCompleteSearch += SearchEngine_OnProviderCompleteSearch;



            ResultList = CollectionViewSource.GetDefaultView(rawResultList);
            ResultList.GroupDescriptions.Add(new PropertyGroupDescription("ProviderName"));
            //  ResultList.GroupDescriptions.Add(new PropertyGroupDescription("GroupName"));
            ResultList.SortDescriptions.Add(new SortDescription("Priority", ListSortDirection.Descending));





            SearchEngine.ComposingFinished();

        }




        void SearchEngine_OnProviderCompleteSearch(SearchEngine sender, SearchProvider p, SearchProvider.SearchResult result)
        {



            rawResultList.AddItems(result.SearchResults);


            //      if (SelectedItem == null)
            //Application.Current.Dispatcher.Invoke(DispatcherPriority.Normal,
            //    (Action)delegate()
            //{
            //     OnPropertyChanged("ResultList");
            //       OnPropertyChanged("SelectedItem");
            //});
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


        private string queryString;

        public string QueryString
        {
            get
            {
                return queryString;
            }
            set
            {
                //  SelectedItem = null;
                rawResultList.Clear();
                queryString = value;
                SearchEngine.SetSearchPattern(value);
                OnPropertyChanged();

            }
        }



        SearchProviderResult selectedItem;
        public SearchProviderResult SelectedItem
        {
            get
            {
                return selectedItem;
            }
            set { SetProperty(ref selectedItem, value); }
        }



        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            var handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }



        public bool ProcessCompatibleConverter(object sender, KeyEventArgs e)
        {

            var converter = GetCompatibleConverterFor(e);

            if (converter != null)
            {
                string ss = QueryString;
                e.Handled = converter.Convert(e, SelectedItem, ref ss);
                QueryString = ss;
                return true;
                
            }
            return false;

        }

        public bool ProcessQueryKeyEvent(object sender, KeyEventArgs e)
        {

            if (IsProviderKey(e))
            {
                if (SelectedItem != null)
                {
                    return SelectedItem.GotKeyboardEvent(sender, e);
                }
            }
            return false;

        }

        private ISearchStringConverter GetCompatibleConverterFor(KeyEventArgs e)
        {
            return converters.FirstOrDefault(t => t.CanHandleShortkey(e));
        }
    }
}