using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using fuzzyLauncher.Base;
using fuzzyLauncher.Engine;
using fuzzyLauncher.Helpers;

namespace fuzzyLauncher.SearchProviders
{


    public delegate void MtaMethod();



    class ActiveProcessesProvider : SearchProvider
    {
        public ActiveProcessesProvider(SearchEngine engine, string suffixName = "")
            : base(engine, suffixName)
        {
        }

        public override List<SearchProviderResult> DoSearch(string searchString)
        {

            var list = new List<SearchProviderResult>();


            var currentSessionId = Process.GetCurrentProcess().SessionId;

            var processlist = (from c in Process.GetProcesses() where c.SessionId == currentSessionId select c).ToArray();


            foreach (var process in processlist)
            {

                var procLower = process.ProcessName.ToLower();
                var searchLower = searchString.ToLower();




                if (procLower.Contains(searchLower) || searchLower.Contains(procLower) || searchString == "*")
                {

                    var result = new SearchProviderResult(this, DisplayColor, KillProcess)
                    {
                        DisplayName = string.Format("{0}", process.ProcessName),
                        ProviderMetadata = process,
                        // GroupName = "Kill"
                    };



                    try
                    {



                        if (process.MainModule != null && !String.IsNullOrEmpty(process.MainModule.FileName))
                        {
                            var sysicon = IconHelper.ExtractAssociatedIconEx(process.MainModule.FileName);
                            result.ImageExtractRoutine = () => sysicon.ToBitmapSource();
                            //result.DisplayImage = sysicon.ToBitmapSource();
                        }

                    }
                    catch (Exception ex)
                    {
                        // result.DisplayImage = Properties.Resources.app.ToBitmap();
                    }

                    list.Add(result);
                }
            }

            return list;


        }

        private void KillProcess(SearchEngine searchEngine, KeyEventArgs keyEventArgs, SearchProviderResult result, string args)
        {
            return;

            // if (keyEventArgs.KeyCode != Keys.Enter) return;

            try
            {

                var process = result.ProviderMetadata as Process;

                if (process != null)
                    process.Kill();


            }
            catch (Exception e)
            {
                DoExecuteActionException(e);
            }

        }

        public override string GetBaseName()
        {
            return "Process";
        }


    }
}
