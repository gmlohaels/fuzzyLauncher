using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Threading;
using Shared;
using Shared.Base;

namespace Dummy
{

    [Export(typeof(SearchProvider))]
    public class DummySearchProvider : SearchProvider
    {


        public DummySearchProvider()
        {
            ImageWrapper.SetIcon(Resource.cvetik);
        }

        protected override void Initialize()
        {


            Thread.Sleep(5000);
            int x = 1;

        }

        protected override List<SearchProviderResult> DoSearch(string searchString)
        {

            Thread.Sleep(1000);
            return ConstructSingleResult("dummy", "dummy descr", SearchProviderResult.PriorityAlwaysLast);
        }
    }
}
