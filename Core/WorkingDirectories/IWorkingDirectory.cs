using System.Collections.Immutable;
using Core.FileSnapshots;
using Core.Snapshots;

namespace Core.WorkingDirectories;

public interface IWorkingDirectory
{
    public Stream GetFileContent(string path);
    public void PutFileContent(string path, Stream content);
    public void DeleteFile(string path);
    public Snapshot GetCurrentSnapshot(IgnoreRuleSet? ignoreRules = null);
    public void ApplySnapshot(Snapshot snapshot, IgnoreRuleSet? ignoreRules = null);
}