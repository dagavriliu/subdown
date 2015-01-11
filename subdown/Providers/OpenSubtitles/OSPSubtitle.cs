using System;
using System.IO;

namespace subdown.Providers.OpenSubtitles
{
    [Serializable]
    public class OSPSubtitle : ISubtitleResult
    {
        public string MatchedBy;
        public string IDSubMovieFile;
        public string MovieHash;
        public string MovieByteSize;
        public string MovieTimeMS;
        public string IDSubtitleFile;

        public string SubFileName;
        public string SubtitleFileName { get { return SubFileName; } set { SubFileName = value; } }

        public string SubActualCD;
        public string SubSize;
        public string SubHash;
        public string IDSubtitle;
        public string UserID;
        public string SubLanguageID;

        public string SubFormat;
        public string SubtitleFormat { get { return SubFormat; } set { SubFormat = value; } }
        public string LocalFilename { get; set; }
        public FileInfo VideoFileInfo { get; set; }

        public string SubSumCD;
        public string SubAuthorComment;
        public string SubAddDate;
        public string SubBad;
        public string SubRating;
        public string SubDownloadsCnt;
        public string MovieReleaseName;
        public string MovieFPS;
        public string IDMovie;
        public string IDMovieImdb;
        public string MovieName;
        public string MovieNameEng;
        public string MovieYear;
        public string MovieImdbRating;
        public string SubFeatured;
        public string UserNickName;
        public string ISO639;
        public string LanguageName;
        public string SubComments;
        public string SubHearingImpaired;
        public string UserRank;
        public string SeriesSeason;
        public string SeriesEpisode;
        public string MovieKind;
        public string SubHD;
        public string SeriesIMDBParent;
        public string QueryNumber;
        public string SubDownloadLink;
        public string ZipDownloadLink;
        public string SubtitlesLink;

        public int NameFit;

        public string SubtitleEncoded { get; set; }
        public string SubtitleDecoded { get; set; }
        public string Id { get { return IDSubtitle; } set { IDSubtitle = value; } }


    }
}
