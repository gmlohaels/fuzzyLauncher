using System;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Threading;
using fuzzyLauncher.Annotations;
using fuzzyLauncher.Base;
using fuzzyLauncher.Engine;
using fuzzyLauncher.SearchProviders;

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
            SearchEngine.RegisterSearchProvider(new ActiveProcessesProvider(SearchEngine));
            ResultList = CollectionViewSource.GetDefaultView(rawResultList);

            ResultList.GroupDescriptions.Add(new PropertyGroupDescription("ProviderName"));
            ResultList.GroupDescriptions.Add(new PropertyGroupDescription("GroupName"));
            ResultList.SortDescriptions.Add(new SortDescription("ProviderAndGroup", ListSortDirection.Ascending));
            SearchEngine.OnProviderCompleteSearch += SearchEngine_OnProviderCompleteSearch;

        }

        void SearchEngine_OnProviderCompleteSearch(SearchEngine sender, SearchProvider p, SearchEngine.SearchRequest searchRequest, System.Collections.Generic.List<Base.SearchProviderResult> searchResult, System.Diagnostics.Stopwatch searchPerformance)
        {



            rawResultList.AddItems(searchResult);

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
            get { return selectedItem; }
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