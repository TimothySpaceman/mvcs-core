namespace Core.FileSystem;

/// <summary>
/// An abstraction layer for file system operations.
/// </summary>
public interface IFileSystemProvider
{
    /// <summary>
    /// Gets the primary directory separator character for paths in this file system provider.
    /// </summary>
    /// <value>
    /// The primary directory separator character (e.g., '\' for Windows, '/' for Unix-based systems).
    /// </value>
    char DirectorySeparatorChar { get; }

    /// <summary>
    /// Gets the alternative directory separator character for paths in this file system provider.
    /// </summary>
    /// <value>
    /// The alternative directory separator character that is also accepted by the file system.
    /// </value>
    char AltDirectorySeparatorChar { get; }

    /// <summary>
    /// Returns the absolute path for the specified path string.
    /// </summary>
    /// <param name="path">The file or directory for which to obtain absolute path information.</param>
    /// <returns>The fully qualified location of the path.</returns>
    string GetFullPath(string path);

    /// <summary>
    /// Returns the file name and extension of the specified path string.
    /// </summary>
    /// <param name="path">The path string from which to obtain the file name and extension.</param>
    /// <returns>
    /// The characters after the last directory separator character in the path. 
    /// If the last character is a directory separator, returns an empty string.
    /// </returns>
    string GetFileName(string path);

    /// <summary>
    /// Returns the file name of the specified path string without the extension.
    /// </summary>
    /// <param name="path">The path of the file.</param>
    /// <returns>
    /// The string returned by <see cref="GetFileName"/> minus the last period (.) 
    /// and all characters following it.
    /// </returns>
    string GetFileNameWithoutExtension(string path);

    /// <summary>
    /// Returns the extension of the specified path string.
    /// </summary>
    /// <param name="path">The path string from which to get the extension.</param>
    /// <returns>
    /// The extension of the specified path (including the period "."), or an empty string 
    /// if the path does not have extension information.
    /// </returns>
    string GetExtension(string path);

    /// <summary>
    /// Returns the directory information for the specified path string.
    /// </summary>
    /// <param name="path">The path of a file or directory.</param>
    /// <returns>
    /// Directory information for the path, or <c>null</c> if the path denotes a root directory 
    /// or is <c>null</c>.
    /// </returns>
    string? GetDirectoryName(string path);

    /// <summary>
    /// Combines an array of strings into a path.
    /// </summary>
    /// <param name="paths">An array of parts of the path.</param>
    /// <returns>The combined paths.</returns>
    string CombinePath(params string[] paths);

    /// <summary>
    /// Gets a value indicating whether the specified path string contains a root.
    /// </summary>
    /// <param name="path">The path to test.</param>
    /// <returns>
    /// <c>true</c> if the path contains a root; otherwise, <c>false</c>.
    /// </returns>
    bool IsPathRooted(string path);

    /// <summary>
    /// Determines whether the specified file exists.
    /// </summary>
    /// <param name="path">The file path to check.</param>
    /// <returns><c>true</c> if the file exists; otherwise, <c>false</c>.</returns>
    bool FileExists(string path);

    /// <summary>
    /// Creates a new file, writes the given byte array to the file, and then closes the file.
    /// If the target file already exists, it is overwritten.
    /// </summary>
    /// <param name="path">The file path to write to.</param>
    /// <param name="bytes">The bytes to write to the file.</param>
    void WriteAllBytes(string path, byte[] bytes);

    /// <summary>
    /// Opens a file, reads all bytes from the file, and then closes the file.
    /// </summary>
    /// <param name="path">The file path to read from.</param>
    /// <returns>A byte array containing the contents of the file.</returns>
    byte[] ReadAllBytes(string path);

    /// <summary>
    /// Deletes the specified file.
    /// </summary>
    /// <param name="path">The path of the file to be deleted.</param>
    void DeleteFile(string path);

    /// <summary>
    /// Copies an existing file to a new file.
    /// </summary>
    /// <param name="sourcePath">The file to copy.</param>
    /// <param name="destinationPath">The path of the destination file.</param>
    /// <param name="overwrite">
    /// <c>true</c> if the destination file can be overwritten; otherwise, <c>false</c>. 
    /// Default is <c>false</c>.
    /// </param>
    void CopyFile(string sourcePath, string destinationPath, bool overwrite = false);

    /// <summary>
    /// Moves a specified file to a new location.
    /// </summary>
    /// <param name="sourcePath">The path of the file to move.</param>
    /// <param name="destinationPath">The new path for the file.</param>
    /// <param name="overwrite">
    /// <c>true</c> to overwrite the destination file if it already exists; otherwise, <c>false</c>. 
    /// Default is <c>false</c>.
    /// </param>
    void MoveFile(string sourcePath, string destinationPath, bool overwrite = false);

    /// <summary>
    /// Opens an existing file for reading.
    /// </summary>
    /// <param name="path">The file to be opened for reading.</param>
    /// <returns>A read-only <see cref="Stream"/> on the specified path.</returns>
    Stream OpenRead(string path);

    /// <summary>
    /// Opens an existing file or creates a new file for writing.
    /// </summary>
    /// <param name="path">The file to be opened for writing.</param>
    /// <returns>A writable <see cref="Stream"/> on the specified path.</returns>
    Stream OpenWrite(string path);

    /// <summary>
    /// Creates or overwrites a file in the specified path.
    /// </summary>
    /// <param name="path">The path and name of the file to create.</param>
    /// <returns>A writable <see cref="Stream"/> on the specified path.</returns>
    Stream Create(string path);

    /// <summary>
    /// Gets the date and time (UTC), that the file was last written at.
    /// </summary>
    /// <param name="path">The file for which to obtain modification date and time.</param>
    /// <returns>
    /// A <see cref="DateTime"/> structure set with the date and time that the file was last written at.
    /// </returns>
    DateTime? GetLastWriteTimeUtc(string path);

    /// <summary>
    /// Sets the date and time (UTC), that the file was last written at.
    /// </summary>
    /// <param name="path">The file for which to set modification date and time.</param>
    /// <param name="lastWriteTimeUtc">
    /// A <see cref="DateTime"/> containing the value to set for the last write date and time of the file.
    /// </param>
    void SetLastWriteTimeUtc(string path, DateTime lastWriteTimeUtc);

    /// <summary>
    /// Gets the size, in bytes, of the given file.
    /// </summary>
    /// <param name="path">The file for which to obtain size information.</param>
    /// <returns>The size of the file in bytes.</returns>
    long GetFileSize(string path);

    /// <summary>
    /// Asynchronously determines whether the specified file exists.
    /// </summary>
    /// <param name="path">The file path to check.</param>
    /// <param name="cancellationToken">
    /// A cancellation token for this operation.
    /// </param>
    /// <returns>
    /// A <see cref="Task"/> for this operation. The task result contains 
    /// <c>true</c> if the file exists; otherwise, <c>false</c>.
    /// </returns>
    Task<bool> FileExistsAsync(string path, CancellationToken cancellationToken = default);

    /// <summary>
    /// Asynchronously creates a new file, writes the specified byte array to the file, 
    /// and then closes the file. If the target file already exists, it is overwritten.
    /// </summary>
    /// <param name="path">The file path to write to.</param>
    /// <param name="bytes">The bytes to write to the file.</param>
    /// <param name="cancellationToken">
    /// A cancellation token for this operation.
    /// </param>
    /// <returns>A <see cref="Task"/> for this operation.</returns>
    Task WriteAllBytesAsync(string path, byte[] bytes, CancellationToken cancellationToken = default);

    /// <summary>
    /// Asynchronously opens a file, reads all bytes from the file, and then closes the file.
    /// </summary>
    /// <param name="path">The file path to read from.</param>
    /// <param name="cancellationToken">
    /// A cancellation token for this operation.
    /// </param>
    /// <returns>
    /// A <see cref="Task"/> for this operation. The task result contains  
    /// a byte array with the contents of the file.
    /// </returns>
    Task<byte[]> ReadAllBytesAsync(string path, CancellationToken cancellationToken = default);

    /// <summary>
    /// Asynchronously deletes the specified file.
    /// </summary>
    /// <param name="path">The path of the file to be deleted.</param>
    /// <param name="cancellationToken">
    /// A cancellation token for this operation.
    /// </param>
    /// <returns>A <see cref="Task"/> for this operation.</returns>
    Task DeleteFileAsync(string path, CancellationToken cancellationToken = default);

    /// <summary>
    /// Asynchronously copies an existing file to a new file.
    /// </summary>
    /// <param name="sourcePath">The file to copy.</param>
    /// <param name="destinationPath">The path of the destination file.</param>
    /// <param name="overwrite">
    /// <c>true</c> if the destination file can be overwritten; otherwise, <c>false</c>. 
    /// Default is <c>false</c>.
    /// </param>
    /// <param name="cancellationToken">
    /// A cancellation token for this operation.
    /// </param>
    /// <returns>A <see cref="Task"/> for this operation.</returns>
    Task CopyFileAsync(string sourcePath, string destinationPath, bool overwrite = false,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Asynchronously moves a specified file to a new location.
    /// </summary>
    /// <param name="sourcePath">The path of the file to move.</param>
    /// <param name="destinationPath">The new path for the file.</param>
    /// <param name="overwrite">
    /// <c>true</c> if the destination file can be overwritten; otherwise, <c>false</c>. 
    /// Default is <c>false</c>.
    /// </param>
    /// <param name="cancellationToken">
    /// A cancellation token for this operation.
    /// </param>
    /// <returns>A <see cref="Task"/> for this operation.</returns>
    Task MoveFileAsync(string sourcePath, string destinationPath, bool overwrite = false,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Asynchronously opens an existing file for reading.
    /// </summary>
    /// <param name="path">The file to be opened for reading.</param>
    /// <param name="cancellationToken">
    /// A cancellation token for this operation.
    /// </param>
    /// <returns>
    /// A <see cref="Task"/> for this operation. The task result contains  
    /// a read-only <see cref="Stream"/> on the specified path.
    /// </returns>
    Task<Stream> OpenReadAsync(string path, CancellationToken cancellationToken = default);

    /// <summary>
    /// Asynchronously opens an existing file or creates a new file for writing.
    /// </summary>
    /// <param name="path">The file to be opened for writing.</param>
    /// <param name="cancellationToken">
    /// A cancellation token for this operation.
    /// </param>
    /// <returns>
    /// A <see cref="Task"/> for this operation. The task result contains  
    /// a writable <see cref="Stream"/> on the specified path.
    /// </returns>
    Task<Stream> OpenWriteAsync(string path, CancellationToken cancellationToken = default);

    /// <summary>
    /// Asynchronously creates or overwrites a file in the specified path.
    /// </summary>
    /// <param name="path">The path and name of the file to create.</param>
    /// <param name="cancellationToken">
    /// A cancellation token for this operation.
    /// </param>
    /// <returns>
    /// A <see cref="Task"/> for this operation. The task result contains  
    /// a writable <see cref="Stream"/> on the specified path.
    /// </returns>
    Task<Stream> CreateAsync(string path, CancellationToken cancellationToken = default);

    /// <summary>
    /// Determines whether the specified directory exists.
    /// </summary>
    /// <param name="path">The path to test.</param>
    /// <returns><c>true</c> if the directory exists; otherwise, <c>false</c>.</returns>
    bool DirectoryExists(string path);

    /// <summary>
    /// Creates directory and all subdirectories in the specified path unless they already exist.
    /// </summary>
    /// <param name="path">The directory path to create.</param>
    void CreateDirectory(string path);

    /// <summary>
    /// Deletes the specified directory.
    /// </summary>
    /// <param name="path">The path of the directory to remove.</param>
    /// <param name="recursive">
    /// <c>true</c> to remove all subdirectories and files in the specified directory; 
    /// otherwise, <c>false</c>. Default is <c>false</c>.
    /// </param>
    void DeleteDirectory(string path, bool recursive = false);

    /// <summary>
    /// Moves a directory and its contents to a new location.
    /// </summary>
    /// <param name="sourcePath">The path of the directory to move.</param>
    /// <param name="destinationPath">The path to the new location for the directory.</param>
    void MoveDirectory(string sourcePath, string destinationPath);

    /// <summary>
    /// Returns an enumerable collection of file paths in a specified path.
    /// </summary>
    /// <param name="path">The directory to search.</param>
    /// <param name="searchPattern">
    /// The search string to match against the names of files. Default is "*" (all files).
    /// </param>
    /// <param name="searchOption">
    /// One of the <see cref="SearchOption"/> values that specifies whether the search operation should include 
    /// only the current directory or all subdirectories. Default is <see cref="SearchOption.TopDirectoryOnly"/>.
    /// </param>
    /// <returns>An enumerable collection of file paths.</returns>
    IEnumerable<string> EnumerateFiles(string path, string searchPattern = "*",
        SearchOption searchOption = SearchOption.TopDirectoryOnly);

    /// <summary>
    /// Returns an enumerable collection of directory paths in a specified path.
    /// </summary>
    /// <param name="path">The directory to search.</param>
    /// <param name="searchPattern">
    /// The search string to match against the names of directories. Default is "*" (all directories).
    /// </param>
    /// <param name="searchOption">
    /// One of the <see cref="SearchOption"/> values that specifies whether the search operation should include 
    /// only the current directory or all subdirectories. Default is <see cref="SearchOption.TopDirectoryOnly"/>.
    /// </param>
    /// <returns>An enumerable collection of directory paths.</returns>
    IEnumerable<string> EnumerateDirectories(string path, string searchPattern = "*",
        SearchOption searchOption = SearchOption.TopDirectoryOnly);

    /// <summary>
    /// Returns an enumerable collection of file system entry paths (files and directories) in a specified path.
    /// </summary>
    /// <param name="path">The directory to search.</param>
    /// <param name="searchPattern">
    /// The search string to match against the names of file system entries. Default is "*" (all entries).
    /// </param>
    /// <param name="searchOption">
    /// One of the <see cref="SearchOption"/> values that specifies whether the search operation should include 
    /// only the current directory or all subdirectories. Default is <see cref="SearchOption.TopDirectoryOnly"/>.
    /// </param>
    /// <returns>An enumerable collection of file system entry paths.</returns>
    IEnumerable<string> EnumerateEntries(string path, string searchPattern = "*",
        SearchOption searchOption = SearchOption.TopDirectoryOnly);

    /// <summary>
    /// Asynchronously determines whether the specified directory exists.
    /// </summary>
    /// <param name="path">The path to test.</param>
    /// <param name="cancellationToken">
    /// A cancellation token for this operation.
    /// </param>
    /// <returns>
    /// A <see cref="Task"/> for this operation. The task result contains 
    /// <c>true</c> if the directory exists; otherwise, <c>false</c>.
    /// </returns>
    Task<bool> DirectoryExistsAsync(string path, CancellationToken cancellationToken = default);

    /// <summary>
    /// Asynchronously creates all directories and subdirectories in the specified path 
    /// unless they already exist.
    /// </summary>
    /// <param name="path">The directory path to create.</param>
    /// <param name="cancellationToken">
    /// A cancellation token for this operation.
    /// </param>
    /// <returns>A <see cref="Task"/> for this operation.</returns>
    Task CreateDirectoryAsync(string path, CancellationToken cancellationToken = default);

    /// <summary>
    /// Asynchronously deletes the specified directory.
    /// </summary>
    /// <param name="path">The path of the directory to remove.</param>
    /// <param name="recursive">
    /// <c>true</c> to remove directories, subdirectories, and files in the path; 
    /// otherwise, <c>false</c>. Default is <c>false</c>.
    /// </param>
    /// <param name="cancellationToken">
    /// A cancellation token for this operation.
    /// </param>
    /// <returns>A <see cref="Task"/> for this operation.</returns>
    Task DeleteDirectoryAsync(string path, bool recursive = false, CancellationToken cancellationToken = default);

    /// <summary>
    /// Asynchronously moves a directory and its contents to a new location.
    /// </summary>
    /// <param name="sourcePath">The path of the directory to move.</param>
    /// <param name="destinationPath">The path to the new location for the directory.</param>
    /// <param name="cancellationToken">
    /// A cancellation token for this operation.
    /// </param>
    /// <returns>A <see cref="Task"/> for this operation.</returns>
    Task MoveDirectoryAsync(string sourcePath, string destinationPath, CancellationToken cancellationToken = default);

    /// <summary>
    /// Returns an asynchronous enumerable collection of file paths in a specified path.
    /// </summary>
    /// <param name="path">The directory to search.</param>
    /// <param name="searchPattern">
    /// The search string to match against the names of files. Default is "*" (all files).
    /// </param>
    /// <param name="searchOption">
    /// One of the <see cref="SearchOption"/> values that specifies whether the search operation should include 
    /// only the current directory or all subdirectories. Default is <see cref="SearchOption.TopDirectoryOnly"/>.
    /// </param>
    /// <param name="cancellationToken">
    /// A cancellation token for this operation.
    /// </param>
    /// <returns>An asynchronous enumerable collection of file paths.</returns>
    IAsyncEnumerable<string> EnumerateFilesAsync(string path, string searchPattern = "*",
        SearchOption searchOption = SearchOption.TopDirectoryOnly, CancellationToken cancellationToken = default);

    /// <summary>
    /// Returns an asynchronous enumerable collection of directory paths in a specified path.
    /// </summary>
    /// <param name="path">The directory to search.</param>
    /// <param name="searchPattern">
    /// The search string to match against the names of directories. Default is "*" (all directories).
    /// </param>
    /// <param name="searchOption">
    /// One of the <see cref="SearchOption"/> values that specifies whether the search operation should include 
    /// only the current directory or all subdirectories. Default is <see cref="SearchOption.TopDirectoryOnly"/>.
    /// </param>
    /// <param name="cancellationToken">
    /// A cancellation token for this operation.
    /// </param>
    /// <returns>An asynchronous enumerable collection of directory paths.</returns>
    IAsyncEnumerable<string> EnumerateDirectoriesAsync(string path, string searchPattern = "*",
        SearchOption searchOption = SearchOption.TopDirectoryOnly, CancellationToken cancellationToken = default);

    /// <summary>
    /// Returns an asynchronous enumerable collection of file system entry paths (files and directories) 
    /// in a specified path.
    /// </summary>
    /// <param name="path">The directory to search.</param>
    /// <param name="searchPattern">
    /// The search string to match against the names of file system entries. Default is "*" (all entries).
    /// </param>
    /// <param name="searchOption">
    /// One of the <see cref="SearchOption"/> values that specifies whether the search operation should include 
    /// only the current directory or all subdirectories. Default is <see cref="SearchOption.TopDirectoryOnly"/>.
    /// </param>
    /// <param name="cancellationToken">
    /// A cancellation token for this operation.
    /// </param>
    /// <returns>An asynchronous enumerable collection of file system entry paths.</returns>
    IAsyncEnumerable<string> EnumerateEntriesAsync(string path, string searchPattern = "*",
        SearchOption searchOption = SearchOption.TopDirectoryOnly, CancellationToken cancellationToken = default);
}