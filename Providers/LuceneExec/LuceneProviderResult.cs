using Shared.Base;

namespace LuceneExec
{
    public class LuceneProviderResult : SearchProviderResult
    {

        public override string Description
        {

            get { return string.Format("<h3>{0}</h3>", Path); }
        }
    }
}