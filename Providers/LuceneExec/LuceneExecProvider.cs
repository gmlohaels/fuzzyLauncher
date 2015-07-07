using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Windows.Media;
using Lucene.Net.Analysis;
using Lucene.Net.Documents;
using Lucene.Net.Index;
using Lucene.Net.QueryParsers;
using Lucene.Net.Search;
using Lucene.Net.Store;
using Lucene.Net.Util;
using Shared;
using Shared.Base;
using Shared.Helpers;
using Version = Lucene.Net.Util.Version;

namespace LuceneExec
{


    [Export(typeof(SearchProvider))]
    public class LuceneExecProvider : SearchProvider
    {


        private readonly RAMDirectory directory = new RAMDirectory();
        private readonly Analyzer analyzer = new KeywordAnalyzer();


        private readonly ConcurrentDictionary<string, Bitmap> iconCache = new ConcurrentDictionary<string, Bitmap>();
        public int maxResultSize = 50;



        private List<AppQuickId> LoadQuickAppList(string path, string mask)
        {

            var l = new ConcurrentBag<AppQuickId>();

            var fileList = System.IO.Directory.EnumerateFiles(path, mask, SearchOption.AllDirectories);

            Parallel.ForEach(
                fileList,
              file =>


          //  foreach (var file in fileList)
              {
                  try
                  {
                      var info = new DirectoryInfo(file).Parent;
                      var dirName = info;
                      var group = "";

                      if (info.Parent != null) group = info.Parent.Name.ToLower();
                      //TODO: log errors
                      var fileName = Path.GetFileName(file).ToLower();

                      if (!fileName.StartsWith("unins"))
                      {
                          var q = new AppQuickId()
                                      {
                                          GroupName = group,
                                          CustomQuickName = file.ToLower(),
                                          Description = dirName.ToString().ToLower(),
                                          DisplayName = fileName,
                                          Path = file.ToLower(),
                                      };

                          iconCache.TryAdd(q.Path, IconHelper.ExtractAssociatedBitmap(file));
                          l.Add(q);
                      }
                  }
                  catch (Exception e)
                  {
                      
                  }

              }
            );


            return l.ToList();
        }



        protected override void Initialize()
        {




            LuceneRamInit(LoadQuickAppList("c:\\Program Files (x86)\\", "*.exe"));
        }

        private void LuceneRamInit(IEnumerable<AppQuickId> quickAppList)
        {
            var writer = new IndexWriter(directory, analyzer, IndexWriter.MaxFieldLength.UNLIMITED);
            foreach (var toSearch in quickAppList)
            {
                var doc = new Document();

                doc.Add(new Field("path", toSearch.Path, Field.Store.YES, Field.Index.ANALYZED));
                doc.Add(new Field("description", toSearch.Description, Field.Store.YES, Field.Index.ANALYZED));
                doc.Add(new Field("groupName", toSearch.GroupName, Field.Store.YES, Field.Index.ANALYZED));

                // iconCache.Add(toSearch.Path, toSearch.DisplayImage);

                doc.Add(new Field("launchCount", toSearch.LaunchCount.ToString(), Field.Store.YES, Field.Index.ANALYZED));


                string exeName = "";
                try
                {
                    exeName = Path.GetFileNameWithoutExtension(toSearch.DisplayName);
                }
                catch (Exception)
                {
                    exeName = toSearch.DisplayName;
                }

                doc.Add(new Field("displayName", exeName, Field.Store.YES, Field.Index.ANALYZED));
                doc.Add(new Field("customQuickName", toSearch.CustomQuickName, Field.Store.YES, Field.Index.ANALYZED));


                // if(toSearch.Icon!=null)
                // doc.Add(new Field("icon",IconHelper.IconToBytes(toSearch.Icon),new Field.Store()));
                //   doc.Add(new Field());
                writer.AddDocument(doc);
            }
            writer.Optimize();
            writer.Dispose();


        }





        protected override List<SearchProviderResult> DoSearch(SearchQuery searchQuery)
        {


            string text = QueryParser.Escape(searchQuery.RawQueryString);

            var query = new QueryParser(Version.LUCENE_30, "displayName description customQuickName groupName", analyzer).Parse(
                    string.Format("displayName:{0}* or description:{0}* or displayName:{0}~ or customQuickName:{0}* or customQuickName:{0}~  or groupName:{0}* or groupName:{0} or path:{0}~ ", text));


            var searcher = new IndexSearcher(directory, true);

            var hits = searcher.Search(query, maxResultSize);


            var resultList = new List<SearchProviderResult>(hits.ScoreDocs.Length);

            foreach (var doc in hits.ScoreDocs)
            {
                var eDoc = searcher.Doc(doc.Doc);
                var path = eDoc.GetField("path").StringValue;
                var displayName = eDoc.GetField("displayName").StringValue;
                var desc = eDoc.GetField("description").StringValue;
                var customQuickName = eDoc.GetField("customQuickName").StringValue;

                // var iconBuffer = eDoc.GetBinaryValue("icon");


                Bitmap i;
                iconCache.TryGetValue(path, out i);


                var launchCount = Convert.ToInt32(eDoc.GetField("launchCount").StringValue);


                var result = ConstructResult<LuceneProviderResult>(customQuickName, desc, displayName, path, launchCount);

                result.SetEnterKeyAction((e) =>
                {
                    if (String.IsNullOrEmpty(e.Result.Path))
                        return false;
                    Process.Start(e.Result.Path, String.Empty);
                    return true;
                });

                if (i != null)
                {
                    result.SetIcon(i);
                }
                resultList.Add(result);
            }
            return resultList;
        }
    }
}
