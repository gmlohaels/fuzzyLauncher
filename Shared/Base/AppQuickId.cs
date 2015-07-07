using System;
using System.Drawing;
using System.Windows.Media;
using Shared.Helpers;

namespace Shared.Base
{

    public class WpfImageWrapper
    {
        public WpfImageWrapper()
        {
            ImageExtractRoutine = () => null;
        }
        public void SetIcon(Bitmap sysicon)
        {
            ImageExtractRoutine = sysicon.ToBitmapSource;
        }

        public void SetIcon(ImageSource source)
        {
            ImageExtractRoutine = () => source;
        }

        public void SetIconFromFilePath(string filePath)
        {
            var sysicon = IconHelper.ExtractAssociatedIconEx(filePath);
            SetIcon(sysicon);
        }

        public Func<ImageSource> ImageExtractRoutine;

    }

    public class AppQuickId : WpfImageWrapper
    {
        public virtual string Path { get; set; }




        public virtual string DisplayName { get; set; }
        public virtual string Description { get; set; }
        public string CustomQuickName { get; set; }

        //  public virtual ImageSource Icon { get; set; }
        public string GroupName { get; set; }
        public int LaunchCount { get; set; }




        public ImageSource DisplayImage
        {
            get { return ImageExtractRoutine(); }
        }




    }
}
