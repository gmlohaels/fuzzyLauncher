using System;
using System.Collections.Generic;
using System.ComponentModel.Composition.Primitives;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.ComponentModel.Composition;
using Shared;
using Shared.Base;


namespace YandexMaps
{

    [Export(typeof(SearchProvider))]
    public class YandexMapsProvider : SearchProvider
    {
        public YandexMapsProvider()
        {
            ExactMatchSearchPrefix = "~";
            MinSymbolsToStartSearch = 4;
        }
        protected override List<SearchProviderResult> DoSearch(SearchQuery query)
        {
            var description = Resources.template.Replace("{{queryString}}", query.QueryString);
            return ConstructSingleResult("On Maps", description, query.ExactMatch ? query.SuggestedPriority : SearchProviderResult.PriorityUltraLow);
        }
    }
}
