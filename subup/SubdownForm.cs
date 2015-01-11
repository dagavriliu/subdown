using subdown;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using subdown.Providers.OpenSubtitles;
using log4net;

namespace subup
{
    public partial class SubdownForm : Form
    {
        private readonly static ILog Log = LogManager.GetLogger(typeof(SubdownForm));

        public string SearchPath { get; set; }
        public SubtitleDownloaderOptions Options { get; set; }
        public SubdownForm(SubtitleDownloaderOptions options)
        {
            Options = options;
            InitializeComponent();
            DownloadSubtitles();
        }

        public void DownloadSubtitles()
        {
            var subtitleProviders = new[] { new OpenSubtitlesProvider() };
            foreach (var provider in subtitleProviders)
            {
                try
                {
                    var downloader = new SubtitleDownloader(provider);
                    

                    var results = downloader.SearchSubtitles(Options).ToArray();
                    cbxSubtitleList.DisplayMember = "SubtitleFileName";
                    cbxSubtitleList.Items.AddRange(results);
                }
                catch (Exception exception)
                {
                    Log.Error(String.Format("Provider {0} threw an exception.", provider.ProviderName), exception);
                }
            }
        }

        private void btnChoosePath_Click(object sender, EventArgs e)
        {
            openFileDialog1.ShowDialog();
        }
    }
}
