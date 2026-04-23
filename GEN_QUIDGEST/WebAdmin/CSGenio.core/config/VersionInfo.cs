using System;

namespace CSGenio.framework
{
    /// <summary>
    /// Contains all version-related constants for the Genio generated system.
    /// Defined as partial class to allow to be easily modified by CI/CD processes if needed.
    /// </summary>
    public static partial class VersionInfo
    {

        /// <summary>
        /// Main application version number
        /// </summary>
        public static int Version { get; } = 2530;

        /// <summary>
        /// Version of the database
        /// CI/CD: Database schema version
        /// </summary>
        public const int DatabaseSchema = 2530;

        /// <summary>
        /// Version of the database indexes
        /// CI/CD: Database index version
        /// </summary>
        public const int DatabaseIndex = 30;

        /// <summary>
        /// Version of the latest version change routines
        /// </summary>
        public const int ChangeRoutines = 0;

        /// <summary>
        /// Version of the latest user settings format
        /// CI/CD: User settings schema version
        /// </summary>
        public const int UserSettings = 3;

        /// <summary>
        /// Genio generator version
        /// </summary>
        public const string GenioVersion = "382.64";

        /// <summary>
        /// Generated version. Is incremented each time there is a generation
        /// </summary>
        public const int Generation = 74;


        /// <summary>
        /// Default release version
        /// </summary>
        private const string Trackchanges = "0";

        /// <summary>
        /// CI/CD override for build number. Set via static constructor in partial class.
        /// </summary>
        internal static int? BuildCI = null;

        /// <summary>
        /// CI/CD override for release version. Set via static constructor in partial class.
        /// </summary>
        internal static string ReleaseCI = null;

        /// <summary>
        /// Solution build version.
        /// Uses the Generation version by default, but can be overridden by CI/CD
        /// </summary>
        public static int Build => BuildCI ?? Generation;

        /// <summary>
        /// Solution release version. Uses the trackchanges by default, but can be replaced by CI/CD
        /// </summary>
        public static string Release => ReleaseCI ?? Trackchanges;

        /// <summary>
        /// Version control number or commit hash
        /// </summary>
        public static readonly string VersionControl = "";

        /// <summary>
        /// Agregate versioning information string
        /// </summary>
        public static string GenAssemblyVersion
        {
            get
            {
                return string.Format("{0}.{1}.{2}.{3}", GenioVersion.Replace('.',','), DatabaseSchema, Release.Replace('.',','), Build);
            }
        }

        /// <summary>
        /// Date when the code was generated
        /// </summary>
        public static readonly DateTime GenerationDate = new DateTime(2026, 4, 23);

    }
}
