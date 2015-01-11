using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace subdown
{
    public delegate void SaveSubtitleDelegate(SubtitleDownloaderOptions options, List<ISubtitleResult> subtitles, string videoFilename);
    public interface ISubtitleProvider
    {
        void SetLogger(TextWriter logger);

        IEnumerable<ISubtitleResult> SearchSubtitles(IEnumerable<FileInfo> filenames, string language);

        IEnumerable<ISubtitleResult> DownloadSubtitles(IEnumerable<FileInfo> filenames, SubtitleDownloaderOptions options, SaveSubtitleDelegate saveSubtitle);

        string ProviderName { get; }

        bool SupportsDirectDownload { get; }

    }
}
