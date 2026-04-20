using System;
using System.IO;
using System.Runtime.InteropServices;

namespace CSGenio.framework
{
    /// <summary>
    /// Provides utility methods for safe file path handling to prevent Path Traversal attacks.
    /// </summary>
    public static class FilePathUtils
    {
        /// <summary>
        /// Combines the base directory and file name into a full path and ensures that the file resides 
        /// within the specified base directory. Also checks for Path Traversal attacks.
        /// </summary>
        /// <param name="baseDirectory">The expected base directory where the file should reside.</param>
        /// <param name="fileName">The name of the file to construct the path for.</param>
        /// <returns>The fully qualified file path if it is safe.</returns>
        /// <exception cref="ArgumentException">Thrown if the file name is null, empty, or contains invalid characters.</exception>
        /// <exception cref="UnauthorizedAccessException">Thrown if the resolved path is outside the base directory (Path Traversal).</exception>
        public static string GetSafeFilePath(string baseDirectory, string fileName)
        {
            // Validate input parameters
            if (string.IsNullOrWhiteSpace(baseDirectory))
            {
                throw new ArgumentException("Base directory cannot be null or empty.", nameof(baseDirectory));
            }

            // Ensure the file name is valid to prevent invalid characters from being used.
            if (!IsValidFileName(fileName))
            {
                throw new ArgumentException("The file name contains invalid characters.", nameof(fileName));
            }

            // Combine base directory and file name into full path.
            string filePath = Combine(baseDirectory, fileName);

            // Resolve the full path to remove any relative segments like "../".
            string fullFilePath = Path.GetFullPath(filePath);

            // Validate that the resolved path is within the expected base directory.
            if(!IsPathInsideDirectory(baseDirectory, fullFilePath))
            {
                throw new UnauthorizedAccessException("Potential Path Traversal attack detected.");
            }

            return fullFilePath;
        }

        /// <summary>
        /// Checks whether the provided file name is valid, i.e., it does not contains any invalid characters.
        /// </summary>
        /// <param name="fileName">The file name to validate.</param>
        /// <returns>True if the file name is valid, otherwise false.</returns>
        public static bool IsValidFileName(string fileName)
        {
            if(string.IsNullOrWhiteSpace(fileName)) return false;
            return fileName.IndexOfAny(Path.GetInvalidFileNameChars()) == -1;
        }

        /// <summary>
        /// Validates whether a resolved full path is inside the expected base directory.
        /// This function protects against Path Traversal attacks by ensuring that the file path does not "escape" the intended directory.
        /// </summary>
        /// <param name="basePath">The base directory expected to contain the file.</param>
        /// <param name="fullPath">The full path of the file to validate.</param>
        /// <returns>True if the file is inside the base directory; otherwise false.</returns>
        public static bool IsPathInsideDirectory(string basePath, string fullPath)
        {
            if (string.IsNullOrWhiteSpace(basePath) || string.IsNullOrWhiteSpace(fullPath))
                return false;

            // Normalize the paths by resolving them to their full paths and removing trailing directory separators.
            string normalizedBasePath = Path.GetFullPath(basePath).TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
            string normalizedFullPath = Path.GetFullPath(fullPath).TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);

            // Compare paths using appropriate case sensitivity based on the operating system
            StringComparison comparison = IsWindows() ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal;

            return normalizedFullPath.StartsWith(normalizedBasePath + Path.DirectorySeparatorChar, comparison);
        }

        /// <summary>
        /// Deletes the specified file if it exists within the base directory. 
        /// This function first ensures that the file path is safe and does not perform a Path Traversal.
        /// </summary>
        /// <param name="baseDirectory">The base directory where the file should be located.</param>
        /// <param name="fileName">The name of the file to delete.</param>
        /// <returns>True if the file was successfully deleted; false if the file did not exist or an error occurred.</returns>
        public static bool DeleteFileIfExists(string baseDirectory, string fileName)
        {
            try
            {
                // Get the fully qualified and save file path.
                string fullFilePath = GetSafeFilePath(baseDirectory, fileName);

                // If the file exists, delete it.
                if (File.Exists(fullFilePath))
                {
                    File.Delete(fullFilePath);
                    return true; 
                }
                return false;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// Combines an array of strings into a path.
        /// Recommended to use this function instead of string concatenation for paths.
        /// </summary>
        /// <param name="paths">An array of parts of the path.</param>
        /// <returns>The combined paths.</returns>
        /// <exception cref="System.ArgumentException">
        /// One of the strings in the array contains one or more of the invalid characters defined in System.IO.Path.GetInvalidPathChars.
        /// </exception>
        /// <exception cref="System.ArgumentNullException">One of the strings in the array is null</exception>
        public static string Combine(params string[] paths)
        {
            return Path.Combine(paths);
        }

        /// <summary>
        /// Determines whether the current operating system is Windows.
        /// </summary>
        /// <returns>True if the operating system is Windows; otherwise, false.</returns>
        private static bool IsWindows()
        {
            // In .NET Standard 2.0, use RuntimeInformation
            return RuntimeInformation.IsOSPlatform(OSPlatform.Windows);
        }
    }
}
