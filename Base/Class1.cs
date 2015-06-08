using System;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using fuzzyLauncher.Engine;
using fuzzyLauncher.SearchProviders;

namespace fuzzyLauncher.Base
{

    public class AppQuickId
    {
        public string Path { get; set; }
        public string DisplayName { get; set; }
        public string Description { get; set; }
        public string CustomQuickName { get; set; }

        public Image Icon { get; set; }
        public string GroupName { get; set; }
        public int LaunchCount { get; set; }
    }




    public class SearchProviderResult : AppQuickId
    {


        public object Tag;

        public Func<ImageSource> ImageExtractRoutine;

        public ImageSource DisplayImage
        {
            get
            {
                if (ImageExtractRoutine != null)
                    return ImageExtractRoutine();
                return null;

            }
        }

        public object ProviderMetadata;


        public string ProviderName
        {
            get { return Provider.GetName(); }
        }

        public override string ToString()
        {
            // return "SUPER SPACY NAME ha ha";
            return DisplayName;

        }


        public readonly SearchProvider Provider;
        public readonly SearchEngine.DisplayPriority Priority;


        public readonly Color DisplayColor;




        private readonly Action<SearchEngine, KeyEventArgs, SearchProviderResult, string> execRoutine;


        public SearchProviderResult(SearchProvider provider, Color resultColor, Action<SearchEngine, KeyEventArgs, SearchProviderResult, string> execRoutine = null, SearchEngine.DisplayPriority priority = SearchEngine.DisplayPriority.DontCare)
        {
            this.Provider = provider;
            this.Priority = priority;
            DisplayColor = resultColor;
            this.execRoutine = execRoutine;

        }

        public void FireAction(SearchEngine engine, KeyEventArgs keys, string args)
        {
            if (execRoutine != null)
                execRoutine(engine, keys, this, args);
        }
    }

}
