﻿using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using fuzzyLauncher.Engine;
using Shared;
using Shared.Base;

namespace fuzzyLauncher.SearchProviders
{



    [Export(typeof(SearchProvider))]
    class ActiveProcessesProvider : SearchProvider
    {
        readonly int currentSessionId = Process.GetCurrentProcess().SessionId;

        protected override List<SearchProviderResult> DoSearch(SearchQuery q)
        {

            var list = new List<SearchProviderResult>();


            foreach (var process in Process.GetProcesses().Where(t => t.SessionId == currentSessionId))
            {

                var procLower = process.ProcessName.ToLower();
                var query = q.QueryString;
                var searchLower = query.ToLower();

                if (procLower.Contains(searchLower) || searchLower.Contains(procLower) || query == "*")
                {

                    var result = new SearchProviderResult(this)
                    {
                        DisplayName = string.Format("{0}", process.ProcessName),
                        ProviderMetadata = process,
                        Priority = SearchProviderResult.PriorityLow,
                        Description = "<h3><img src='data:image/gif;base64,R0lGODdhMAAwAPAAAAAAAP///ywAAAAAMAAwAAAC8IyPqcvt3wCcDkiLc7C0qwyGHhSWpjQu5yqmCYsapyuvUUlvONmOZtfzgFzByTB10QgxOR0TqBQejhRNzOfkVJ+5YiUqrXF5Y5lKh/DeuNcP5yLWGsEbtLiOSpa/TPg7JpJHxyendzWTBfX0cxOnKPjgBzi4diinWGdkF8kjdfnycQZXZeYGejmJlZeGl9i2icVqaNVailT6F5iJ90m6mvuTS4OK05M0vDk0Q4XUtwvKOzrcd3iq9uisF81M1OIcR7lEewwcLp7tuNNkM3uNna3F2JQFo97Vriy/Xl4/f1cf5VWzXyym7PHhhx4dbgYKAAA7' />" + process.ProcessName + "</h3>"
                        // GroupName = "Kill"
                    };


                    result.SetEnterKeyAction((x) =>
                    {
                        var p = result.ProviderMetadata as Process;

                        if (p != null)
                            p.Kill();
                        return true;
                    });


                    try
                    {
                        if (!String.IsNullOrEmpty(process.MainModule.FileName))
                        {
                            result.SetIconFromFilePath(process.MainModule.FileName);
                        }

                    }
                    catch (Exception)
                    {

                        result.SetIcon(Properties.Resources.Application);
                    }
                    list.Add(result);
                }
            };


            return list;

        }

        public override string GetBaseName()
        {
            return "Process";
        }


    }
}
