using System;
using System.CodeDom.Compiler;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using CookComputing.XmlRpc;
using Microsoft.Win32;
using log4net;
using subdown.Providers.OpenSubtitles;

namespace subdown
{
    class Program
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(Program));

        public static void WriteSubtitles(string[] subtitleData, string[] filenames, string dir)
        {
            for (int i = 0; i < subtitleData.Length; i++)
            {
                string filename = dir + "/" + filenames[i];
                using (var sw = new StreamWriter(filename) { AutoFlush = true })
                {
                    sw.WriteLine(subtitleData[i]);

                }
            }
        }

        public static SubtitleDownloaderOptions DefaultOptions = new SubtitleDownloaderOptions { SearchDirectory = "subs", Language = "eng", SubtitleProviderName = "osp" };

        public static SubtitleDownloaderOptions DebuggingOptions()
        {
            var options = new SubtitleDownloaderOptions
                              {
                                  Enable = true,
                                  ForceDirectDownload = true,
                                  DownloadChunkSize = 10,
                                  MaximumSubtitlesPerFile = 3,
                                  DownloadDirectory = "subs",
                                  DownloadType = SubtitleDownloadType.RelativeToFile,
                                  Language = "eng",
                                  SearchDirectory = @"D:\media\video\MASH.Complet.Series.DVDrip-No.Grp\Season 2\",

                              };
            return options;
        }

        static Program()
        {
            log4net.Config.XmlConfigurator.Configure();
        }

        static void Main(string[] args)
        {
             Log.Info("Starting subdownload");

            var options = new SubtitleDownloaderOptions();
            var subtitleProviders = new[] { new OpenSubtitlesProvider() };

            bool optionsEnabled;
            if (Debugger.IsAttached)
            {
                options = DebuggingOptions();
                optionsEnabled = true;
            }
            else
            {
                optionsEnabled = CommandLine.Parser.Default.ParseArgumentsStrict(args, options);
                options.Enable = true;
            }
            // we should check that our options are valid
            optionsEnabled &= options.Validate();

            options.ForceDirectDownload = false;
            options.DownloadChunkSize = 10;
            options.DownloadDirectory = "subs";
            options.MaximumSubtitlesPerFile = 3;

            if (optionsEnabled)
            {
                if (String.IsNullOrWhiteSpace(options.SearchDirectory))
                {
                    options.SearchDirectory = Environment.CurrentDirectory;
                }

                Log.InfoFormat("Search Options:\r\n{0}", options);

                foreach (var provider in subtitleProviders)
                {
                    try
                    {
                        var downloader = new SubtitleDownloader(provider);
                        downloader.DownloadSubtitles(options);
                    }
                    catch (Exception exception)
                    {
                        Log.Error(String.Format("Provider {0} threw an exception.", provider.ProviderName), exception);
                    }

                }
            }
            else
            {
                Log.Info("Subtitle options could not be found");
            }
            Log.Info("Program terminated");

        }

        //}

    }
}


