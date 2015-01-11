using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using subdown;
using subdown.Providers.OpenSubtitles;
using log4net;
using CommandLine;

namespace subup
{
    static class Program
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(Program));

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            for (var i = 0; i < args.Length; i++)
            {
                args[i] = args[i].Replace("\"\"", "\"");
            }
            var options = new SubtitleDownloaderOptions();


            bool optionsEnabled;
            {
                optionsEnabled = Parser.Default.ParseArgumentsStrict(args, options);
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
                    options.SearchDirectory = args.Length == 1 ? args[0] : Environment.CurrentDirectory;

                }

                Log.InfoFormat("Search Options:\r\n{0}", options);


            }
            else
            {
                Log.Info("Subtitle options could not be found");
            }
            Log.Info("Program terminated");

            #region DownloadSubtitles Application

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new SubdownForm(options));
            #endregion
        }
    }
}
