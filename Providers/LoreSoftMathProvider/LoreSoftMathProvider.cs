using System;
using System.Collections.Generic;
using System.ComponentModel.Composition.Primitives;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shared;
using System.ComponentModel.Composition;
using System.Globalization;
using LoreSoft.MathExpressions;
using Shared.Base;

namespace LoreSoftMathProvider
{
    [Export(typeof(SearchProvider))]
    public class LoreSoftMathProvider : SearchProvider
    {
        readonly string[] arr = new[] { "+", "-", "*", "/" };
        protected override List<SearchProviderResult> DoSearch(string searchString)
        {

            int priority = SearchProviderResult.PriorityUltraLow;
            var eval = new MathEvaluator();

         

            if (arr.Any(searchString.Contains))
            {
                priority = SearchProviderResult.PriorityHigh;
            }


            double result = eval.Evaluate(searchString);
            return ConstructSingleResult(result.ToString(CultureInfo.InvariantCulture), result.ToString(CultureInfo.InvariantCulture), priority);


        }
    }
}
