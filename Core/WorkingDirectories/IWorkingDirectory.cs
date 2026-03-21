using System.Collections.Immutable;
using Core.FileSnapshots;
using Core.Snapshots;

namespace Core.WorkingDirectories;

public interface IWorkingDirectory
{
    bool IsIgnored(string relativePath, IgnoreRuleSet? ignoreRules = null);
    
    public bool HasFile(string path);
    public Task<Stream?> GetFileContentAsync(string path, CancellationToken cancellationToken = default);
    public Task PutFileContentAsync(string path, Stream content, CancellationToken cancellationToken = default);
    public Task DeleteFileAsync(string path, CancellationToken cancellationToken = default);

    public Task<Snapshot> GetCurrentSnapshotAsync(
        IgnoreRuleSet? ignoreRules = null,
        CancellationToken cancellationToken = default
    );

    public Task ApplySnapshotAsync(
        Snapshot snapshot,
        IgnoreRuleSet? ignoreRules = null,
        CancellationToken cancellationToken = default
    );
}