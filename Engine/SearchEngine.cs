using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using fuzzyLauncher.SearchProviders;
using Shared;

namespace fuzzyLauncher.Engine
{


    public class SearchEngine
    {



        [ImportMany]
        private readonly List<SearchProvider> providerList = new List<SearchProvider>();



        public List<SearchProvider> LoadedProviders
        {
            get { return providerList.ToList(); }
        }



        public long SearchTime { get; private set; }
        public int SearchRequestNumber;

        public delegate void ProviderCompleteSearch(SearchEngine sender, SearchProvider p, SearchProvider.SearchResult r);

        public event ProviderCompleteSearch OnProviderCompleteSearch;


        public virtual void DoProviderCompleteSearch(object p, SearchProvider.SearchResult r)
        {

            if (r.SearchRequest.RequestNumber != SearchRequestNumber)
                return;


            lock (syncObject)
            {

                SearchTime += r.SearchPerformance.ElapsedMilliseconds;
                var handler = OnProviderCompleteSearch;
                if (handler != null)
                {

                    if (r.SearchRequest.RequestNumber == SearchRequestNumber)
                        handler(this, p as SearchProvider, r);
                }
            }
        }


        private readonly object syncObject = new object();

        private string searchPattern;

        public string SearchPattern
        {
            get
            {
                return searchPattern;
            }
            private set
            {
                searchPattern = value;
                SetupNewSearch(searchPattern);
            }
        }

        private void SetupNewSearch(string pattern)
        {
            SearchTime = 0;

            Stopwatch sw = new Stopwatch();
            sw.Start();

            Interlocked.Increment(ref SearchRequestNumber);
            var searchRequest = new SearchRequest { RequestNumber = SearchRequestNumber, SearchPattern = pattern };
            providerList.ForEach(t => t.Search(searchRequest));

            sw.Stop();


            if (sw.ElapsedMilliseconds > 50)
            {

            }

        }


        public SearchEngine()
        {
            searchPattern = String.Empty;
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


        public void ComposingFinished()
        {
            var provider = new BasicCommandsProvider(this);



            providerList.Add(provider);


            providerList.ForEach(t =>
            {
                t.OnSearchCompleted += DoProviderCompleteSearch;
                t.Load();

            });
        }
    }
}
