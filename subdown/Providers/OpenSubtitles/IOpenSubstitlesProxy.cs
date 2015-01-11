using System;
using CookComputing.XmlRpc;

namespace subdown.Providers.OpenSubtitles
{
    [XmlRpcUrl(OpenSubtitlesProvider.APIUrl)]
    public interface IOpenSubstitlesProxy : IXmlRpcProxy
    {
        /// <summary>Gets info about server</summary>
        /// <returns></returns>
        /// <example></example>
        /// <remarks></remarks>
        [XmlRpcMethod("ServerInfo")]
        Object ServerInfo();

        /*
             string LogOut( $token ) This will logout user (ends session id). Good call this function is before ending (closing) clients program.
             */
        /// <summary> Terminates session for provided token </summary>
        /// <returns></returns>
        /// <example></example>
        /// <remarks></remarks>
        [XmlRpcMethod("LogOut")]
        Object LogOut(string token);


        /// <summary>This will login user. This function should be called always when starting talking with server. </summary>
        /// <returns>It returns token (Session ID), which must be used in later communication.</returns>
        /// <example></example>
        /// <remarks>
        /// It returns token, which must be used in later communication.
        /// If user has no account, blank username and password should be OK.
        /// Note: when username and password is blank, status is 200 OK, because we want allow anonymous users too. 
        /// </remarks>
        /// <param name="language">use ISO639 2 letter code and later communication will be done in this language if applicable (error codes and so on)</param>
        /// <param name="username">optional</param>
        /// <param name="password">optional</param>
        /// <param name="useragent">cannot be empty string. For $useragent use your registered useragent, also provide version number - we need tracking version numbers of your program. 
        /// </param>
        [XmlRpcMethod("LogIn")]
        XmlRpcStruct LogIn(string username, string password, string language, string useragent);

        //    array CheckSubHash( $token, array($subhash, $subhash, ...) )
        [XmlRpcMethod("CheckSubHash")]
        Object CheckSubHash();


        /// <summary>Search for multiple subtitle files at once.</summary>
        /// <remarks>
        /// When nothing is found, 'data' is empty. 
        /// If SubLanguageId is empty, or have value 'all' - it search in every sublanguage, you can set it to multiple languages (e.g. 'eng,dut,cze'). 
        /// Tag is moviefilename or subtitlefilename, or releasename - currently we index more than 35.000.000 of tags.
        /// If you define MovieHash and MovieByteSize, then ImdbId and query in same array are ignored. 
        /// If you define ImdbId, then MovieHash, MovieByteSize and query is ignored. 
        /// If you define query, then MovieHash, MovieByteSize and ImdbId is ignored.
        /// Some data (IDSubMovieFile, MovieHash, MovieByteSize, MovieTimeMS) are set to 0, when searching by ImdbId or query
        /// Max results is set to 500 found items in total
        /// Season and Episode are not mandatory, when using query/imdb searching
        /// For perfect matches use MovieHash/MovieByteSize searching, for movie matches use tag/ImdbId searching, if you can not use any of them, use fulltext search (least accurate)
        /// If used this method in movie player and subtitles are found using ImdbId or query, should be nice to implement Automatic Uploading, so we get back movie hash of matching movie
        /// MatchedBy can be: MovieHash, ImdbId, tag, fulltext (so you know from where results comes from)
        /// Tip: you can combine your searches array as you want, so things like
        /// 
        /// array('query' => 'south park', 'season' => 1, 'episode' => 1, 'SubLanguageId'=>'all'),
        /// array('ImdbId' => '1129442', 'SubLanguageId' => 'eng'),
        /// array('query' => 'matrix', 'SubLanguageId' => 'cze,slo'),
        /// array('MovieHash' => '18379ac9af039390', 'MovieByteSize' => 366876694),
        /// array('tag' => 'heroess01e08.avi'), //you can add also query or ImdbId here
        /// are possible :)
        /// </remarks>
        /// <returns>Returns information about found subtitles. It is designed making multiple search at once.
        /// Note: SubLanguageId is the ISO639-3 language code
        /// </returns>

        /// <param name="token">Session ID</param>
        ///<param name="searchOptions">
        /// {
        ///     {  'SubLanguageId' => $SubLanguageId, 
        ///         'MovieHash' => $MovieHash, 
        ///         'MovieByteSize' => $moviesize, 
        ///         ImdbId => $ImdbId, 
        ///         query => 'movie name', 
        ///         "season" => 'season number', 
        ///         "episode" => 'episode number', 
        ///         'tag' => tag 
        ///     },
        ///     {...}
        /// }</param>
        ///<example>
        /// 
        ///      [data] => Array
        ///        (
        ///            [0] => Array
        ///                (
        ///                    [MatchedBy] => MovieHash
        ///                    [IDSubMovieFile] => 865
        ///                    [MovieHash] => d745cd88e9798509
        ///                    [MovieByteSize] => 734058496
        ///                    [MovieTimeMS] => 0
        ///                    [IDSubtitleFile] => 1118
        ///                    [SubFileName] => Al sur de Granada (SPA).srt
        ///                    [SubActualCD] => 1
        ///                    [SubSize] => 15019
        ///                    [SubHash] => 0cb51bf4a5266a9aee42a2d8c7ab6793
        ///                    [IDSubtitle] => 905
        ///                    [UserID] => 0
        ///                    [SubLanguageID] => spa
        ///                    [SubFormat] => srt
        ///                    [SubSumCD] => 1
        ///                    [SubAuthorComment] => 
        ///                    [SubAddDate] => 2005-06-15 20:05:35
        ///                    [SubBad] => 1
        ///                    [SubRating] => 4.5
        ///                    [SubDownloadsCnt] => 216
        ///                    [MovieReleaseName] => ss
        ///                    [IDMovie] => 11517
        ///                    [IDMovieImdb] => 349076
        ///                    [MovieName] => Al sur de Granada
        ///                    [MovieNameEng] => South from Granada
        ///                    [MovieYear] => 2003
        ///                    [MovieImdbRating] => 6.4
        ///                    [SubFeatured] => 0
        ///                    [UserNickName] => 
        ///                    [ISO639] => es
        ///                    [LanguageName] => Spanish
        ///                    [SubComments] => 1
        ///                    [SubHearingImpaired] => 0
        ///                    [UserRank] => 
        ///                    [SeriesSeason] => 
        ///                    [SeriesEpisode] => 
        ///                    [MovieKind] => movie
        ///                    [QueryParameters] => Array
        ///                        (
        ///                            [SubLanguageId] => all
        ///                            [MovieHash] => d745cd88e9798509
        ///                            [MovieByteSize] => 734058496
        ///                        )
        ///
        ///                    [QueryNumber] => 0
        ///                    [SubDownloadLink] => http://dl.opensubtitles.local/en/download/filead/1118.gz
        ///                    [ZipDownloadLink] => http://dl.opensubtitles.local/en/download/subad/905
        ///                    [SubtitlesLink] => http://www.opensubtitles.local/en/subtitles/905/al-sur-de-granada-es
        ///                )
        ///
        ///            [1] => Array
        ///...
        ///
        /// </example>
        [XmlRpcMethod("SearchSubtitles")]
        XmlRpcStruct SearchSubtitles(string token, Object searchOptions);

        /// <summary>Finds the IDs for subtitle files specified by their hash value</summary>
        /// <example>
        /// [status] => 200 OK
        ///        [data] => Array
        ///            (
        ///                [a9672c89bc3f5438f820f06bab708067] => 1      // id for hash a9672c89bc3f5438f820f06bab708067
        ///                [0ca1f1e42cfb58c1345e149f98ac3aec] => 3      // id for hash 0ca1f1e42cfb58c1345e149f98ac3aec
        ///                [11111111111111111111111111111111] => 0      // id for invalid hash 11111111111111111111111111111111
        ///            )
        ///        [seconds] => 0.009
        ///</example>
        /// <remarks>array CheckSubHash( $token, array($subhash, $subhash, ...) )</remarks>
        /// <returns>This method returns !IDSubtitleFile, if Subtitle Hash exists in database. If not exists, it returns '0'.</returns>
        /// <param name="token">Session ID for the established connection. Received upon call to LogIn method</param>
        /// <param name="subhashes">Array of strings containing subtitle hashes</param>
        [XmlRpcMethod("CheckSubHash")]
        Object CheckSubHash(string token, object subhashes);


        /// <summary>This method returns best matching !MovieImdbID, MovieName, MovieYear, if available for each $MovieHash</summary>
        /// <returns></returns>
        /// <remarks> 
        /// See also CheckMovieHash2(). 
        /// Read more about Movie Identification. 
        /// Note: method accepts only first 200 hashes to avoid database load, not processed hashes are included in "not_processed"
        /// </remarks>
        /// <param name="token">Session ID</param>
        /// <param name="movieHashesArray">Array of strings containing Movie hashes</param>
        /// <example>
        /// [status] => 200 OK
        ///    [data] => Array
        ///        (
        ///            [dab462412773581c] => Array
        ///                (
        ///                    [MovieHash] => dab462412773581c
        ///                    [MovieImdbID] => 133152
        ///                    [MovieName] => Planet of the Apes
        ///                    [MovieYear] => 2001
        ///                    [MovieKind] => movie
        ///                    [SeriesSeason] => 
        ///                    [SeriesEpisode] =>  
        ///               )
        ///
        ///            [ae34f157eefc093c] => Array
        ///                (
        ///                    [MovieHash] => ae34f157eefc093c
        ///                    [MovieImdbID] => 288477
        ///                    [MovieName] => Ghost Ship
        ///                    [MovieYear] => 2002
        ///                    [MovieKind] => movie
        ///                    [SeriesSeason] => 
        ///                    [SeriesEpisode] => 
        ///                )
        ///
        ///            [abcdefg123211222] => Array
        ///                (
        ///                )
        ///
        ///        )
        ///
        ///    [not_processed] => Array
        ///        (
        ///        )
        ///
        ///    [seconds] => 0.133
        /// </example>
        [XmlRpcMethod("CheckMovieHash")]
        XmlRpcStruct CheckMovieHash(string token, object movieHashesArray);

        /// <param name="subFileIdArray"> Array of Subtitle File IDs </param>
        /// <param name="token">Session ID</param>
        /// <returns>
        /// [status] => HTTP Status of request
        /// [data] => array of {[idsubtitlefile], [BASE64 encoded -> gzipped subtitle file]}
        /// </returns>
        /// <remarks></remarks>
        /// <summary>
        /// Returns BASE64 encoded gzipped IDSubtitleFile(s). You need to BASE64 decode and ungzip 'data' to get its contents.
        /// </summary>
        /// <example>
        ///    [status] => 200 OK
        ///    [data] => Array
        ///        (
        ///            [0] => Array
        ///                (
        ///                    [idsubtitlefile] => 10
        ///                    [data] => MQ0KMDA6MDA6MzgsMzAwIC0tPiAwMDowMDo0MSwwMDA...
        ///                )
        ///            [1] => Array
        ///                (
        ///                    [idsubtitlefile] => 20
        ///                    [data] => MQ0KMDA6MDA6MjYsMjgzIC0tPiAwMD...
        ///                )
        ///    [seconds] => 0.397
        ///
        /// </example>
        [XmlRpcMethod("DownloadSubtitles")]
        XmlRpcStruct DownloadSubtitles(string token, object subFileIdArray);

        /*
             array NoOperation( $token )

This function should be called each 15 minutes after last request to xmlrpc. It is used for not expiring current session. It also returns if current $token is registered.

Example output when token is registered:

    [status] => 200 OK
    [seconds] => 0.055
When it is not registered, in client should be called LogIn() again. Example of response:

    [status] => 406 No session
    [seconds] => 0.061
Fields explanation: All field are self-explained.
             */

        [XmlRpcMethod("NoOperation")]
        XmlRpcStruct NoOperation(string token);
    }
}

