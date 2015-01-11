using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mime;
using System.Text;
using CommandLine;
using CommandLine.Text;
using log4net;
using subdown.Providers.OpenSubtitles;

namespace subdown
{
    /// <summary>
    /// Specifies how the DownloadDirectory should be resolved
    /// </summary>
    public enum SubtitleDownloadType
    {
        /// <summary>
        /// The DownloadDirectory is relative to each video file searched
        /// </summary>
        RelativeToFile,
        /// <summary>
        /// The DownloadDirectory is relative to the SearchDirectory
        /// </summary>
        RelativeToDirectory,
        /// <summary>
        /// The DownloadDirectory is static
        /// </summary>
        Static
    }

    public class SubtitleDownloaderOptions
    {
        [OptionArray('f', "files", DefaultValue = new string[] { }, HelpText = "Filenames", Required = false)]
        public string[] Filenames { get; set; }

        [Option('s', "searchDirectory", DefaultValue = "", HelpText = "Directory to search for files")]
        public string SearchDirectory { get; set; }

        [Option('t', "downloadType", DefaultValue = SubtitleDownloadType.RelativeToDirectory, HelpText = "Specifies how the DownloadDirectory should be resolved. Values: {RelativeToFile, RelativeToDirectory, Static}")]
        public SubtitleDownloadType DownloadType { get; set; }

        [Option('d', "donwloadDir", DefaultValue = "subs", HelpText = "Directory to download subtitles")]
        public string DownloadDirectory { get; set; }

        [Option('l', "language", DefaultValue = "eng", HelpText = "Subtitle language. Defaults to \"eng\"")]
        public string Language { get; set; }

        [Option("listFiles", DefaultValue = false, HelpText = "Just show the list of files")]
        public bool List { get; set; }

        /// <summary>
        /// The name of the Subtitle provider that will be used. Defaults to "osp", OpenSubtitleProvider.
        /// </summary>
        [Option('p', "provider", DefaultValue = "osp", HelpText = "The name of the Subtitle Provider")]
        public string SubtitleProviderName { get; set; }

        /// <summary>
        /// The maximum number of subtitles downloaded for a file. This includes the files already found at the specified subtitle download location
        /// </summary>
        [Option("subsPerFile", DefaultValue = 3, HelpText = "The maximum number of subtitles per file")]
        public int MaximumSubtitlesPerFile { get; set; }

        /// <summary>
        /// Subtitle downloads can sometimes be grouped together to increase performance. When this is possible, how may should be downloaded together ?
        /// </summary>
        [Option("chunkSize", DefaultValue = 10, HelpText = "Subtitles can be downloaded in bulk. Specify how many should be grouped together.")]
        public int DownloadChunkSize { get; set; }

        /// <summary>
        /// It's only common sense to wait a little, usually 
        /// </summary>
        [Option("waitBetweenChunks", DefaultValue = 200, HelpText = "Milliseconds to wait between chunk downloads.")]
        public int TimeToWaitBetweenChunks { set; get; }

        [Option("waitBetweenSubtitles", DefaultValue = 200, HelpText = "Milliseconds to wait between subtitle downloads.")]
        public int TimeToWaitBetweenSubtitles { get; set; }

        /// <summary>
        /// Subtitles can be obtained via APIs or direct downloads. This option specifies how a subtitle provider implementation should obtain the subtitles
        /// </summary>
        [Option("forceDirectDownload", DefaultValue = false, HelpText = "Subtitles can be obtained via APIs or direct downloads. This option specifies how a subtitle provider implementation should obtain the subtitles.")]
        public bool ForceDirectDownload { get; set; }

        /// <summary>
        /// Strictly for debugging purposes 
        /// </summary>
        [Option('e', "enable", DefaultValue = true, HelpText = "Strictly for debugging purposes, this flag needs to be enabled for the options to be valid")]
        public bool Enable { get; set; }

        [Option('r', "recursiveSearch", DefaultValue = false, HelpText = "Search for video files recursively.")]
        public bool Recurse { get; set; }

        private static readonly ILog Log = LogManager.GetLogger(typeof(SubtitleDownloaderOptions));

        public bool Validate()
        {
            bool isValid = Enable;

            if (Filenames != null && Filenames.Length > 0 && Filenames.Any(filename => !File.Exists(filename)))
            {
                isValid = false;
            }
            if (!String.IsNullOrWhiteSpace(SearchDirectory) && Directory.Exists(SearchDirectory) == false)
            {
                isValid = false;
            }

            if (!isValid)
            {
                Log.Warn("subdown options could not be validated");
                Log.Warn("subdown options: " + this);
            }

            return isValid;
        }

        [HelpOption]
        public string GetUsage()
        {
            var help = new HelpText
            {
                Heading = new HeadingInfo("subdown", "1.0"),
                Copyright = new CopyrightInfo("Dan", DateTime.Now.Year),
                AdditionalNewLineAfterOption = true,
                AddDashesToOption = true,
                MaximumDisplayWidth = 80
            };
            help.AddPreOptionsLine("Usage: subdown");
            help.AddOptions(this);
            return help;

        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            if (Filenames != null && Filenames.Length > 0)
            {
                sb.AppendLine("filenames: " + Filenames.Aggregate("", (e, i) => e + i + " "));
            }
            if (!String.IsNullOrWhiteSpace(SearchDirectory))
            {
                sb.AppendLine("search directory: " + SearchDirectory);
            }
            if (!String.IsNullOrWhiteSpace(DownloadDirectory))
            {
                sb.AppendLine("download directory: " + DownloadDirectory);
            }
            if (!String.IsNullOrWhiteSpace(Language))
            {
                sb.AppendLine("language: " + Language);
            }
            if (!String.IsNullOrWhiteSpace(SubtitleProviderName))
            {
                sb.AppendLine("provider: " + SubtitleProviderName);
            }

            sb.AppendLine(String.Format("DownloadChunkSize {0}", DownloadChunkSize));
            sb.AppendLine(String.Format("DownloadType {0}", DownloadType));
            sb.AppendLine(String.Format("Enable: {0}", Enable));
            sb.AppendLine(String.Format("ForceDirectDownload: {0}", ForceDirectDownload));
            sb.AppendLine(String.Format("MaximumSubtitlesPerFile: {0}", MaximumSubtitlesPerFile));
            sb.AppendLine(String.Format("SubtitleProviderName: {0}", SubtitleProviderName));
            sb.AppendLine(String.Format("TimeToWaitBetweenChunks: {0}", TimeToWaitBetweenChunks));
            sb.AppendLine(String.Format("TimeToWaitBetweenSubtitles: {0}", TimeToWaitBetweenSubtitles));

            return sb.ToString();
        }

        private static SubtitleDownloaderOptions _default;

        public static SubtitleDownloaderOptions Default
        {
            get
            {
                if (_default == null)
                {
                    _default = new SubtitleDownloaderOptions
                                   {
                                       DownloadChunkSize = 10,
                                       DownloadDirectory = "subs",
                                       DownloadType = SubtitleDownloadType.RelativeToDirectory,
                                       Filenames = new[] { "" },
                                       ForceDirectDownload = false,
                                       Language = "eng",
                                       MaximumSubtitlesPerFile = 3,
                                       SearchDirectory = "",
                                       SubtitleProviderName = "osp",
                                       TimeToWaitBetweenChunks = 200,
                                       TimeToWaitBetweenSubtitles = 200
                                   };

                }
                return _default;

            }
        }

        public readonly string[] VideoExtensions = SubtitleDownloader.VideoFileExtensions;


    }
}