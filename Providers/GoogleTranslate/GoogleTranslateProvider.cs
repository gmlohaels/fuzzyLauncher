using System;
using System.Collections.Generic;
using System.ComponentModel.Composition.Primitives;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shared;
using Shared.Base;
using System.ComponentModel.Composition;
using System.Net;
using GoogleTranslate.Properties;

namespace GoogleTranslate
{



    [Export(typeof(SearchProvider))]
    class GoogleTranslateProvider : SearchProvider
    {

        public GoogleTranslateProvider()
        {
            ImageWrapper.SetIcon(Resource.gti);
        }

        public static string TranslateText(string input, string languagePair)
        {
            string url = String.Format("http://www.google.com/translate_t?hl=en&ie=UTF8&text={0}&langpair={1}", input, languagePair);
            WebClient webClient = new WebClient { Encoding = Encoding.UTF8 };
            string result = webClient.DownloadString(url);
            result = result.Substring(result.IndexOf("<span title=\"") + "<span title=\"".Length);
            result = result.Substring(result.IndexOf(">") + 1);
            result = result.Substring(0, result.IndexOf("</span>"));
            return result.Trim();
        }




        protected override List<SearchProviderResult> DoSearch(string searchString)
        {

            var r = new List<SearchProviderResult>();

            var exactMatch = searchString.StartsWith("=>");


            if (exactMatch)
                searchString = searchString.Remove(0, 2);



            if (searchString.Length < 2)
                return null;

            if (exactMatch)
            {
                r.Add(new SearchProviderResult(this) { DisplayName = searchString, Description = "NOT LAZY", Priority = SearchProviderResult.PriorityExactMatch });
            }
            else
            {
                r.Add(new SearchProviderLazyResult(this, searchString, (x => searchString + " Lazy"), SearchProviderResult.PriorityUltraLow));
            }

            return r;
        }


    }
}
