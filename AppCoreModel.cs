using System;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Runtime.CompilerServices;
using System.Windows.Data;
using fuzzyLauncher.Annotations;
using fuzzyLauncher.Engine;
using fuzzyLauncher.Mef;
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

        public SearchEngine SearchEngine = new SearchEngine();
        private readonly FastObservableCollection<SearchProviderResult> rawResultList;


        public ICollectionView ResultList { get; private set; }


        public AppCoreModel()
        {
            rawResultList = new FastObservableCollection<SearchProviderResult>();


            var safeCatalog = new SafeDirectoryCatalog("Providers");
            var container = new CompositionContainer(safeCatalog.Catalog);

            container.ComposeParts(SearchEngine);


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
            //Application.Current.Dispatcher.Invoke(DispatcherPriority.Normal,
            //    (Action)delegate()
            //{
            //     OnPropertyChanged("ResultList");
            //       OnPropertyChanged("SelectedItem");
            //});
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
    }
}