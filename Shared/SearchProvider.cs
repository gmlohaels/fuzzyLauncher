using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Media;
using Shared.Base;

namespace Shared
{
    public abstract class SearchProvider
    {
        //
        public WpfImageWrapper ImageWrapper = new WpfImageWrapper();


        protected string ExactMatchSearchPrefix = String.Empty;
        protected int MinSymbolsToStartSearch;

        protected SearchProviderResult ConstructResult<T>(string customQuickName, string description, string displayName, string path, int launchCount = 0) where T : SearchProviderResult, new()
        {

            var result = new T() { Provider = this, CustomQuickName = customQuickName, Description = description, Path = path, DisplayName = displayName, LaunchCount = launchCount };
            result.OnException += OnException;
            return result;
        }




        protected List<SearchProviderResult> ConstructSingleResult(string name, string description = "", int priority = SearchProviderResult.PriorityNormal)
        {
            return new List<SearchProviderResult>
            {
                new SearchProviderResult(this) {DisplayName = name, Description = description, Priority = priority}
            };
        }

        private readonly AutoResetEvent searchEvent = new AutoResetEvent(false);

        private volatile SearchRequest lastRequest;


        public ImageSource ProviderIcon
        {
            get
            {
                return ImageWrapper.ImageExtractRoutine();
            }
        }

        public Color DisplayColor;



        public void Search(SearchRequest e)
        {

            if (!IsInitialized)
                return;

            lastRequest = e;
            searchEvent.Set(); //signal event
        }



        public class SearchResult
        {
            public int Priority;
            public List<SearchProviderResult> SearchResults;
            public SearchRequest SearchRequest;
            public Stopwatch SearchPerformance;

        }


        public class SearchQuery
        {
            public string RawQueryString;
            public int SuggestedPriority;
            public string QueryString;
            public bool ExactMatch;

        }


        protected abstract List<SearchProviderResult> DoSearch(SearchQuery query);






        public event EventHandler<Exception> ExecuteActionException;
        public event EventHandler<Exception> OnException;
        public event EventHandler<SearchResult> OnSearchCompleted;

        protected virtual void DoExecuteActionException(Exception e)
        {
            var handler = ExecuteActionException;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        private void SearchTask()
        {
            while (searchEvent.WaitOne())
            {

                SearchRequest searchRequest = lastRequest;

                if (searchRequest == null)
                    continue;

                string searchPattern = searchRequest.SearchPattern;

                if (String.IsNullOrEmpty(searchPattern)) continue;


                Stopwatch searchPerformance = Stopwatch.StartNew();
                List<SearchProviderResult> searchResult = null;
                try
                {
                    var query = GenerateQueryFromString(searchPattern);

                    if (query != null)
                        searchResult = DoSearch(query);
                }
                catch (Exception e)
                {
                    DoOnException(e);
                }
                finally
                {
                    searchPerformance.Stop();
                }


                if (searchResult != null && searchResult.Count > 0)
                {
                    DoOnSearchCompleted(new SearchResult() { SearchRequest = searchRequest, SearchPerformance = searchPerformance, SearchResults = searchResult });
                }


            }
        }

        private SearchQuery GenerateQueryFromString(string searchPattern)
        {


            string filtered = searchPattern;
            var exactMatch = searchPattern.StartsWith(ExactMatchSearchPrefix);


            if (exactMatch)
                filtered = filtered.Remove(0, ExactMatchSearchPrefix.Length);



            if (MinSymbolsToStartSearch != 0)
            {
                if (filtered.Length < MinSymbolsToStartSearch)
                    return null;
            }

            return new SearchQuery()
            {
                RawQueryString = searchPattern,
                QueryString = filtered,
                SuggestedPriority = exactMatch ? SearchProviderResult.PriorityExactMatch : SearchProviderResult.PriorityNormal,
                ExactMatch = exactMatch
            };
        }


        protected string nameSuffix;

        public string GetName()
        {

            if (String.IsNullOrEmpty(nameSuffix)) return "[" + GetBaseName() + "]";
            return String.Format("[" + GetBaseName() + "::" + nameSuffix + "]");
        }


        public virtual string GetBaseName()
        {
            return GetType().Name.Replace("Provider", "");
        }

        private Task searchTask;
        public bool IsInitialized { get; private set; }


        protected virtual void Initialize()
        {


        }



        public virtual void Load()
        {
            Task.Factory.StartNew(() =>
            {
                Initialize();
                searchTask = Task.Factory.StartNew(SearchTask, TaskCreationOptions.LongRunning);
                IsInitialized = true;
            });


        }


        public virtual void Unload()
        {

            //searchTask.Stop analog

        }

        protected virtual void DoOnException(Exception e)
        {
            var handler = OnException;
            if (handler != null) handler(this, e);
        }

        protected virtual void DoOnSearchCompleted(SearchResult s)
        {
            var handler = OnSearchCompleted;
            if (handler != null) handler(this, s);
        }

        public override string ToString()
        {
            return string.Format("Name: {0}, IsInitialized: {1}", GetName(), IsInitialized);
        }
    }
}
