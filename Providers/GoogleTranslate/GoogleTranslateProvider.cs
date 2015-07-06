using System;
using System.Collections.Generic;
using System.ComponentModel.Composition.Primitives;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shared;
using Shared.Base;
using System.ComponentModel.Composition;
using System.IO;
using System.Net;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using GoogleTranslate.Properties;

namespace GoogleTranslate
{



    // [Export(typeof(SearchProvider))]
    class GoogleTranslateProvider : SearchProvider
    {

        public GoogleTranslateProvider()
        {
            ImageWrapper.SetIcon(Resource.gti);
        }

        public static string TranslateText(string input, string languagePair)
        {
            string strUrl = "https://translate.yandex.net/api/v1.5/tr.json/translate?";
            strUrl += "key=trnsl.1.1.20150624T044522Z.904fd838812ba867.80102829f6e81c4e6644da5342680bc8dbeff88f";
            strUrl += "&lang=en-ru";
            strUrl += "&text=" + input;
            WebClient wc = new WebClient { Encoding = Encoding.UTF8 };
            string strJson = wc.DownloadString(strUrl);


            var s = new DataContractJsonSerializer(typeof(YandexAnswer));
            var ms = new MemoryStream(Encoding.UTF8.GetBytes(strJson));
            YandexAnswer answer = (YandexAnswer)s.ReadObject(ms);
            return string.Join("<br/>", answer.text);



        }




        protected override List<SearchProviderResult> DoSearch(SearchQuery s)
        {
            var query = s.RawQueryString;
            var r = new List<SearchProviderResult>();


            var exactMatch = query.StartsWith("=>");


            if (exactMatch)
                query = query.Remove(0, 2);



            if (query.Length < 2)
                return null;

            if (exactMatch)
            {
                r.Add(new SearchProviderResult(this) { DisplayName = query, Description = TranslateText(query, "r"), Priority = SearchProviderResult.PriorityExactMatch });
            }
            else
            {
                r.Add(new SearchProviderLazyResult(this, query, (x => TranslateText(query, "")), SearchProviderResult.PriorityUltraLow));
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
