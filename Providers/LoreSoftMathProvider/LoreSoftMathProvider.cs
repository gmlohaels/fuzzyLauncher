using System;
using System.Collections.Generic;
using System.ComponentModel.Composition.Primitives;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.ComponentModel.Composition;
using System.Globalization;
using LoreSoft.MathExpressions;
using Shared;
using Shared.Base;


namespace LoreSoftMathProvider
{
    [Export(typeof(SearchProvider))]
    public class LoreSoftMathProvider : SearchProvider
    {
        readonly string[] arr = new[] { "+", "-", "*", "/" };
        protected override List<SearchProviderResult> DoSearch(SearchQuery query)
        {

            int priority = SearchProviderResult.PriorityUltraLow;
            var eval = new MathEvaluator();



            if (arr.Any(query.RawQueryString.Contains))
            {
                priority = SearchProviderResult.PriorityHigh;
            }


            double result = eval.Evaluate(query.RawQueryString);
            return ConstructSingleResult(result.ToString(CultureInfo.InvariantCulture), result.ToString(CultureInfo.InvariantCulture), priority);


        }
    }
}
