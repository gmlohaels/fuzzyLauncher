﻿using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using fuzzyLauncher.Engine;
using fuzzyLauncher.Properties;
using Shared;
using Shared.Base;

namespace fuzzyLauncher.SearchProviders
{


    class BasicCommandsProvider : SearchProvider
    {
        private readonly SearchEngine searchEngine;




        private string reloadXlaunch = "reload launcher";

        public BasicCommandsProvider(SearchEngine searchEngine)
        {
            ImageWrapper.SetIcon(Resources.launcher);
            this.searchEngine = searchEngine;
        }


        private SearchProviderResult GenerateResult(string displayName, string description = "")
        {

            var result = new SearchProviderResult(this)
            {
                DisplayName = displayName,
                Description = description
            };
            result.SetIcon(Resources.cmd.ToBitmap());
            result.Path = result.DisplayName;
            return result;

        }

        protected override List<SearchProviderResult> DoSearch(SearchQuery q)
        {
            var query = q.QueryString.ToLower();

            var list = new List<SearchProviderResult>(0);

            if (query.StartsWith("prov") || query.StartsWith("rest"))
                list.Add(GenerateResult("Provider Stats", GetProvidersDescription()));



            if (query.StartsWith("rel") || query.StartsWith("rest"))
                list.Add(GenerateResult(reloadXlaunch));

            if (query.StartsWith("ex") || query.StartsWith("qu"))
            {
                var searchProviderResult = GenerateResult("exit").SetEnterKeyAction((a) =>
                {
                    Application.Current.Shutdown(0);
                    return true;

                });

                list.Add(searchProviderResult);
            }


            return list;
        }

        private string GetProvidersDescription()
        {
            var b = new StringBuilder();
            searchEngine.LoadedProviders.ForEach(p => b.Append(p + "<br>"));
            return b.ToString();
        }
    }
}
