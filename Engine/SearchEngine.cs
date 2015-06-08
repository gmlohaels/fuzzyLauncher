using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows.Input;
using fuzzyLauncher.Base;
using fuzzyLauncher.SearchProviders;

namespace fuzzyLauncher.Engine
{

    public class SearchEngine
    {


        public enum DisplayPriority
        {
            DontCare,
            TopMost,
            Highest,
            Normal,
            Lowest,
        }

        public class SearchRequest
        {
            public int RequestNumber;
            public string SearchPattern;
        }

        private readonly Dictionary<string, SearchProvider> providerList = new Dictionary<string, SearchProvider>();



        public long SearchTime { get; private set; }


        public int SearchRequestNumber { get; private set; }




        public void ClearSearchResults()
        {
            SearchTime = 0;

        }


        public delegate void ProviderCompleteSearch(SearchEngine sender, SearchProvider p, SearchRequest searchRequest, List<SearchProviderResult> searchResult, Stopwatch searchPerformance);

        public event ProviderCompleteSearch OnProviderCompleteSearch;




        public virtual void DoProviderCompleteSearch(SearchProvider p, SearchRequest searchRequest, List<SearchProviderResult> searchResult, Stopwatch searchPerformance)
        {

            lock (syncObject)
            {

                //  if (!CacheResults.ContainsKey(searchRequest.SearchPattern))
                //      CacheResults.Add(searchRequest.SearchPattern, searchResult);


                SearchTime += searchPerformance.ElapsedMilliseconds;
                ProviderCompleteSearch handler = OnProviderCompleteSearch;
                if (handler != null)
                {

                    if (searchRequest.RequestNumber == SearchRequestNumber)
                        handler(this, p, searchRequest, searchResult, searchPerformance);
                }
            }
        }


        public event EventHandler<Exception> OnSearchProviderException;


        public virtual void DoOnSearchProviderException(SearchProvider p, Exception e)
        {
            var handler = OnSearchProviderException;
            if (handler != null)
            {
                handler(p, e);
            }
        }

        public event EventHandler<SearchRequest> OnSearchPatternChanged;




        protected virtual void DoSearchPatternChanged(SearchEngine sender, SearchRequest newSearchRequest)
        {
            var handler = OnSearchPatternChanged;
            if (handler != null)
            {
                handler(sender, newSearchRequest);
            }
        }


        private readonly object syncObject = new object();


        private string searchPattern;




        private void SearchCompleteFromCache(string request)
        {
            if (CacheResults.ContainsKey(request))
            {

                var handler = OnProviderCompleteSearch;
                if (handler != null)
                {
                    handler(this, null, new SearchRequest() { RequestNumber = SearchRequestNumber, SearchPattern = request }, CacheResults[request], new Stopwatch());
                }

            }
        }

        public string SearchPattern
        {
            get
            {
                string s = searchPattern;
                return s;

            }
            private set
            {

                lock (syncObject)
                {
                    SearchRequestNumber++;
                    searchPattern = value;
                    ClearSearchResults();

                    //  if (!CacheResults.ContainsKey(SearchPattern))
                    {
                        DoSearchPatternChanged(this, new SearchRequest() { RequestNumber = SearchRequestNumber, SearchPattern = searchPattern });
                    }
                    //   else
                    {
                        //  SearchCompleteFromCache(SearchPattern);
                    }
                }


            }
        }



        public SearchEngine()
        {
            searchPattern = "";
        }

        public void RegisterSearchProvider(SearchProvider provider)
        {

            if (providerList.ContainsKey(provider.GetName()))
                throw new ArgumentException("Already exists");
            providerList.Add(provider.GetName(), provider);
            provider.Load();

        }



        public void EmptySearchPattern()
        {
            SearchRequestNumber++;
        }

        public void SetSearchPattern(string newSearchPattern)
        {
            if (String.IsNullOrEmpty(newSearchPattern))
            {
                EmptySearchPattern();
                return;
            }

            SearchPattern = newSearchPattern;

        }

        //public ConcurrentDictionary<string, SearchProviderResult> ItemInfo = new ConcurrentDictionary<string, SearchProviderResult>();
        public Dictionary<string, List<SearchProviderResult>> CacheResults = new Dictionary<string, List<SearchProviderResult>>();

        public void ProcessItem(KeyEventArgs keyEventArgs, SearchProviderResult item, string args)
        {

            if (item != null)
            {
                item.FireAction(this, keyEventArgs, args);
            }
        }
    }
}
