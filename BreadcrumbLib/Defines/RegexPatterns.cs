
namespace BreadcrumbLib.Defines
{
    /// <summary>
    /// Store a number of Regex patterns.
    /// </summary>
    public static class RegexPatterns
    {
        //http://regexlib.com/Search.aspx?k=file%20path

        /// <summary>
        /// For validate Filesystem path, e.g. c:\temp
        /// </summary>
        public static string FileSystemPattern = "^(([a-zA-Z]\\:)|(\\\\))(\\\\{1}|((\\\\{1})[^\\\\]([^/:*?<>|\"]*))+)$";

        /// <summary>
        /// For santize a filename.
        /// </summary>
        public static string SantizeFileNamePattern = "[\\/:*?\"<>|]*";

        /// <summary>
        /// For parsing a mask string to file name and extension (e.g. a*.zip to a and zip)
        /// </summary>
        public static string ParseFileNameAndExtPattern = @"(?<name>[^\.]*)[\.]?(?<ext>.*)?";

        /// <summary>
        /// For validate a relative path, e.g. temp\\Bug
        /// </summary>
        public static string RelativePathPattern = "^([^\\:*?<>\"|]*\\)*([^\\:*?<>\"|]*)$";

        /// <summary>
        /// Match a Guid, e.g. 031E4825-7B94-4DC3-B131-E946B44C8DD5
        /// </summary>
        public static string GuidPattern = "^([A-Z0-9]{8}-[A-Z0-9]{4}-[A-Z0-9]{4}-[A-Z0-9]{4}-[A-Z0-9]{12})$";

        /// <summary>
        /// For validate GUID path, e.g. ::{031E4825-7B94-4DC3-B131-E946B44C8DD5}\Documents.library-ms
        /// </summary>
        public static string FileSystemGuidPattern = "^(::{[A-Z0-9]{8}-[A-Z0-9]{4}-[A-Z0-9]{4}-[A-Z0-9]{4}-[A-Z0-9]{12}})(\\.*)?$";

        /// <summary>
        /// For validate and parse COFE directory volume path, e.g. {ListerKey}\C:\Temp
        /// </summary>
        public static string ParseDirectoryListerLink = @"^({(?<key>[\w ^}]*)}[\\/]?)?(?<path>.*)$";

        /// <summary>
        /// For parsing parameter string, e.g. login=xyz&password=abc
        /// </summary>        
        public static string ParseAlphaParamStringPattern = @"([&]?(?<key>[\w]*)=(?<value>[\w]*))";

        /// <summary>
        /// For parsing parameter string, e.g. login=xyz&password=abc
        /// </summary>        
        public static string ParseParamStringPattern = @"([&]?(?<key>[^&^=]*)=(?<value>[^&^=]*))";

        /// <summary>
        /// For two-level parameter string, e.g. any=(file=*.cs&file=*.txt)&root={temp}\(aaa)&this=that
        ///    key  value
        /// => any	file=*.cs&file=*.txt
        /// => root {temp}\(aaa)
        /// => this that
        /// </summary>        
        public static string ParseParamStringPattern2 = @"[&]?(?<key>[^&=]*)=(([(](?<value>[^)]*)[)])|(?<value>[^&=]*))";

        /// <summary>
        /// For parsing next token in a Filter string 
        /// e.g. root:{temp} subdir:true filetype:cs OR filetype:txt 
        /// for lising cs and txt file in temp or it's subdirectory.
        /// Result value should trim the " character.
        /// </summary>
        public static string ParseParamStringNextTokenPattern =
            @"(((?<key>[-]?[\w]*):)?(?<value>[^\s^']{1,}|'[^']*'))".Replace("'", "\"");


        public static string UsernamePattern = @"^(?=.*\d)(?=.*[a-z])(?=.*[A-Z]).{8,10}$";

        /// <summary>
        /// Parse work name(Library::WorkName(Parameter), e.g. COFEDB::FindExpireEntryWork(abcde)
        /// (^(?<lib>[^:()]*))::(?<type>[^:()]*)\((?<param>[^:()]*)\)"
        /// </summary>
        public static string ParseWorkNamePattern = "(^(?<lib>[^:()]*))::(?<type>[^:()]*)\\((?<param>[^:()]*)\\)";

        /// <summary>
        /// Parse a http(or https) path (http://localhost:8080/abc/cde?def=ghi)
        /// => host = http://localhost:8080
        /// => path = /abc/cde
        /// => option = def=ghi
        /// </summary>
        public static string ParseNetPathPattern = @"(?<host>[\w]{1,2}tp[s]?://[^/]*)(?<path>[^?]*)([?]?)(?<option>\S*)";

        /// <summary>
        /// For listing entries in ICompressionWrapper.
        /// </summary>
        public static string CompressionListPattern = "^(?<parent>{0})[\\\\]?(?<name>[^\\\\]*)(?<trail>[\\\\]?.*)$";
        //"^(?<parent>{0})[\\\\](?<name>[^\\\\]*)(?<trail>[\\\\]?.*)$";
        //"^(?<parent>{0})(?<name>[^\\\\]*)(?<trail>[\\\\]?.*)$"; //Bugged.
        //^(?<parent>JKL)[\\](?<name>[^\\]*)(?<trail>[\\]?.*)$ works, unslashed.

        public static string NumberPattern = "^[-]?[0-9][0-9]*$";

        public static string jsonPropertyValuePattern = "^[\\s]*\"(?<key>[\\w]*)\"\\s*:\\s*(\"?)(?<value>[^,\"]*)(\"?)\\s*,?";
        //^[\s]*"(?<key>[\w]*)"\s*:\s*("?)(?<value>[^,"]*)("?)\s*,
        public static string jsonListStartPattern = "^[\\s]*\"(?<key>[\\w]*)\"[\\s]*:[\\s]*[[](?<contents>[^]]*)]";
        //^[\s]*"(?<key>[\w]*)"\s*:\s*[[](?<contents>[^]]*)]

        /// <summary>
        /// For ExifPropertyProvider, extract "10/290" from "0.0344827586206897 (10/290)"
        /// </summary>
        public static string ExtractValueInBracketPattern = @"^(?<value1>[^(^\s]*) \((?<value2>[\d/]*)\)";

        /// <summary>
        /// For parsing variable with or without an array index accesser (e.g. this[123]).
        /// </summary>
        public static string ParseArrayCounterPattern = "(?<variable>[^\\[]*)(\\[(?<counter>[\\d]*)\\])?";
    }
}
