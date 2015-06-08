using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Media;
using fuzzyLauncher.Base;
using fuzzyLauncher.Engine;

namespace fuzzyLauncher.SearchProviders
{
    public abstract class SearchProvider
    {

        public readonly SearchEngine Engine;
        private readonly AutoResetEvent searchEvent = new AutoResetEvent(false);


        private readonly ConcurrentStack<SearchEngine.SearchRequest> searchBag =
            new ConcurrentStack<SearchEngine.SearchRequest>();


        public Color DisplayColor;
        private readonly string nameSuffix;


        protected SearchProvider(SearchEngine engine, String suffixName = "")
        {
            Engine = engine;
            nameSuffix = suffixName;
            Engine.OnSearchPatternChanged += Engine_OnSearchPatternChanged;

        }

        private void Engine_OnSearchPatternChanged(object sender, SearchEngine.SearchRequest e)
        {
            searchBag.Push(e);
            searchEvent.Set(); //signal event
        }







        public abstract List<SearchProviderResult> DoSearch(string searchString);



        public event EventHandler<Exception> ExecuteActionException;

        protected virtual void DoExecuteActionException(Exception e)
        {
            var handler = this.ExecuteActionException;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        private void SearchTask()
        {
            while (searchEvent.WaitOne())
            {
                SearchEngine.SearchRequest searchRequest;

                while (this.searchBag.TryPop(out searchRequest))
                {

                    string searchPattern = searchRequest.SearchPattern;

                    if (String.IsNullOrEmpty(searchPattern)) continue;


                    Stopwatch searchPerformance = Stopwatch.StartNew();
                    List<SearchProviderResult> searchResult = null;
                    try
                    {
                        searchResult = DoSearch(searchPattern);
                    }
                    catch (Exception e)
                    {
                        Engine.DoOnSearchProviderException(this, e);
                    }
                    finally
                    {
                        searchPerformance.Stop();
                    }



                    if (searchResult != null && searchResult.Count > 0)
                    {
                        Engine.DoProviderCompleteSearch(this, searchRequest, searchResult, searchPerformance);
                    }

                }
            }
        }





        public string GetName()
        {

            if (String.IsNullOrEmpty(nameSuffix)) return "[" + GetBaseName() + "]";


            return String.Format("[" + GetBaseName() + "::" + nameSuffix + "]");
        }


        public abstract string GetBaseName();

        private Task searchTask;

        public virtual void Load()
        {
            searchTask = Task.Factory.StartNew(SearchTask, TaskCreationOptions.LongRunning);
        }


        public virtual void Unload()
        {

            //searchTask.Stop analog

        }
    }
}
