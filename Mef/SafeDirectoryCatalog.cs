using System;
using System.Collections.Generic;
using System.ComponentModel.Composition.Hosting;
using System.ComponentModel.Composition.Primitives;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace fuzzyLauncher.Mef
{

    public class SafeDirectoryCatalog : ComposablePartCatalog
    {
        public readonly AggregateCatalog Catalog;

        public SafeDirectoryCatalog(string directory)
        {


            try
            {


                var files = Directory.EnumerateFiles(directory, "*.dll", SearchOption.AllDirectories);

                Catalog = new AggregateCatalog();
                Catalog.Catalogs.Add(new AssemblyCatalog((this.GetType()).Assembly));

                Parallel.ForEach(files,
                    file =>
                    {
                        try
                        {
                            var asmCat = new AssemblyCatalog(file);

                            //Force MEF to load the plugin and figure out if there are any exports
                            // good assemblies will not throw the RTLE exception and can be added to the catalog
                            if (asmCat.Parts.ToList().Count > 0)
                                Catalog.Catalogs.Add(asmCat);
                        }
                        catch (ReflectionTypeLoadException)
                        {
                        }
                        catch (BadImageFormatException)
                        {
                        }
                    });

            }
            catch (Exception)
            {

            }
        }
    }

}