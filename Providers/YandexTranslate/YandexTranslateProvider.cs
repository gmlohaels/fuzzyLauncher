using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using System.Net;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Text.RegularExpressions;
using Shared;
using Shared.Base;

namespace YandexTranslate
{



    [Export(typeof(SearchProvider))]
    class YandexTranslateProvider : SearchProvider
    {
        private static string apiKey = "trnsl.1.1.20150624T044522Z.904fd838812ba867.80102829f6e81c4e6644da5342680bc8dbeff88f";
        public YandexTranslateProvider()
        {
            ImageWrapper.SetIcon(Resource.y2);
            ExactMatchSearchPrefix = "=>";
            MinSymbolsToStartSearch = 2;
        }

        public static string TranslateText(string input, string direction)
        {
            string strUrl = "https://translate.yandex.net/api/v1.5/tr.json/translate?";
            strUrl += "key=" + apiKey;
            strUrl += "&lang=" + direction;
            strUrl += "&text=" + input;
            WebClient wc = new WebClient { Encoding = Encoding.UTF8 };
            string strJson = wc.DownloadString(strUrl);

            var s = new DataContractJsonSerializer(typeof(YandexAnswer));
            var ms = new MemoryStream(Encoding.UTF8.GetBytes(strJson));
            YandexAnswer answer = (YandexAnswer)s.ReadObject(ms);
            return string.Join("<br/>", answer.text);



        }



        protected override List<SearchProviderResult> DoSearch(SearchQuery q)
        {

            var r = new List<SearchProviderResult>();
            var query = q.QueryString;


            string direction = "en-ru";

            if (Regex.IsMatch(query, @"\p{IsCyrillic}"))
            {
                direction = "ru-en";
            }

            if (q.ExactMatch)
            {
                r.Add(new SearchProviderResult(this) { DisplayName = TranslateText(query, direction), Description = query, Priority = SearchProviderResult.PriorityExactMatch });
            }
            else
            {
                r.Add(new SearchProviderLazyResult(this, query, (x => TranslateText(query, direction)), SearchProviderResult.PriorityLow));
            }

            return r;
        }


    }

    [DataContract]
    internal class YandexAnswer
    {
        [DataMember]
        public int code { get; set; }
        [DataMember]
        public string lang { get; set; }
        [DataMember]
        public List<string> text { get; set; }
    }
}
