using System.Diagnostics;
using System.IO.Hashing;
using System.Runtime.InteropServices;
using System.Text;
using Core.Config;
using Core.DI;
using Core.Storage;
using Core.Blobs;
using Core.Commits;
using Core.Diffing;
using Core.Events;
using Core.FileChanges;
using Core.FileSnapshots;
using Core.Refs;
using Core.Repositories;
using Core.WorkingDirectories;

Directory.SetCurrentDirectory("./repo-workdir");

var services = new ServiceContainer();

var configService = new ConfigService();
services.Use<IConfigService>(configService);

services.Use<IBlobStorageBackend, LocalBlobStorageBackend>();

services.Use<IBlobMetadataStore, BlobMetadataStore>();
services.Use<ICommitStore, CommitStore>();

services.Use<IBlobService, BlobService>();
services.Use<ICommitService, CommitService>();
services.Use<IRefStore, RefStore>();
services.Use<IDiffService, DiffService>();

configService.Set("repo.dir", "./repo/");
configService.Set("blob.dir", "blobs/");

var workingDirectory = services.Resolve<LocalWorkingDirectoryBuilder>().GetWorkingDirectory();
services.Use<IWorkingDirectory>(workingDirectory);

var repo = services.Resolve<Repository>();
repo.IgnoreRuleSet.ExcludeRules.Add("**/*.ign");

repo.OnCommit += OnCommit;
repo.OnCheckout += OnCheckout;

var dataDir = "../data";
var f1SourcePath = Path.Combine(dataDir, "f1.txt");
var f2SourcePath = Path.Combine(dataDir, "f2.txt");
var f3SourcePath = Path.Combine(dataDir, "f3.txt");
var f1Path = "f1.txt";
var f2Path = "f2.txt";
var f3Path = "subdir/f3.txt";

using (var fsR = File.OpenRead(f1SourcePath))
{
    using (var fsW = File.Create(f1Path))
    {
        fsR.CopyTo(fsW);
    }
}

repo.Commit("V1", repo.GetStatus());

using (var fsR = File.OpenRead(f2SourcePath))
{
    using (var fsW = File.Create(f2Path))
    {
        fsR.CopyTo(fsW);
    }
}

using (var fsR = File.OpenRead(f3SourcePath))
{
    using (var fsW = File.Create(f3Path))
    {
        fsR.CopyTo(fsW);
    }
}

repo.Commit("V2", repo.GetStatus());

File.Delete(f1Path);

repo.Commit("V3", repo.GetStatus());

File.Delete(f2Path);

repo.Commit("V4", repo.GetStatus());

File.Move(f3Path, f2Path);

repo.Commit("V5", repo.GetStatus());

File.Delete(f2Path);

repo.Commit("V6", repo.GetStatus());

var history = repo.GetCommitsHistory().ToArray();

repo.CheckoutCommit(history[2].Id);

var status1 = repo.GetStatus();

repo.CheckoutCommit(history[1].Id);

var status2 = repo.GetStatus();

repo.CheckoutCommit(history[4].Id);

Console.WriteLine("Done");

void OnCommit(object? sender, CommitEventArgs e)
{
    Console.WriteLine($"Commited {e.Commit.Id}!");
}

void OnCheckout(object? sender, CheckoutEventArgs e)
{
    Console.WriteLine($"Checked out {e.TargetId}!");
}

// using (var fs = File.OpenRead(Path.Combine(dataDir, f2Path)))
// {
//     f2Snapshot = FileSnapshotFactory.CreateSnapshot(
//         f2Path,
//         BlobMetadataFactory.CreateMetadata(fs).Id,
//         File.GetLastWriteTimeUtc(f2Path)
//     );
// }
//
// using (var fs = File.OpenRead(Path.Combine(dataDir, f3Path)))
// {
//     f3Snapshot = FileSnapshotFactory.CreateSnapshot(
//         "subdir2/" + f3Path,
//         BlobMetadataFactory.CreateMetadata(fs).Id,
//         File.GetLastWriteTimeUtc(f3Path)
//     );
// }
//
//
// var fChange1 = new FileChange(null, f1Snapshot);
// var fChange2 = new FileChange(null, f2Snapshot);
// var fChange3 = new FileChange(null, f3Snapshot);
// var fChange4 = new FileChange(f1Snapshot, null);
// var fChange5 = new FileChange(f2Snapshot, null);
// var fChange6 = new FileChange(f3Snapshot, null);
//
// var commitBuilder = new CommitBuilder();
// var commit1 = commitBuilder.AddMessage("V1").AddFileChange(fChange1).GetCommit();
// var commit2 = commitBuilder.Reset().AddParentId(commit1.Id).AddMessage("V2").AddFileChange(fChange2)
//     .AddFileChange(fChange3).GetCommit();
// var commit3 = commitBuilder.Reset().AddParentId(commit2.Id).AddMessage("V3").AddFileChange(fChange4).GetCommit();
// var commit4 = commitBuilder.Reset().AddParentId(commit3.Id).AddMessage("V4").AddFileChange(fChange5).GetCommit();
// var commit5 = commitBuilder.Reset().AddParentId(commit4.Id).AddMessage("V5").AddFileChange(fChange6).GetCommit();
//
// commitService.AddCommit(commit1);
// commitService.AddCommit(commit2);
// commitService.AddCommit(commit3);
// commitService.AddCommit(commit4);
// commitService.AddCommit(commit5);
//
// var snapshot1 = commitService.GetSnapshotForCommit(commit1.Id);
// var snapshot2 = commitService.GetSnapshotForCommit(commit2.Id);
// var snapshot3 = commitService.GetSnapshotForCommit(commit3.Id);
// var snapshot4 = commitService.GetSnapshotForCommit(commit4.Id);
// var snapshot5 = commitService.GetSnapshotForCommit(commit5.Id);
//
// var currentFiles = workingDirectory.GetCurrentSnapshot(rules);

// workingDirectory.ApplySnapshot(snapshot1, rules);
// workingDirectory.ApplySnapshot(snapshot2, rules);
// workingDirectory.ApplySnapshot(snapshot3, rules);
// workingDirectory.ApplySnapshot(snapshot4, rules);
// workingDirectory.ApplySnapshot(snapshot5, rules);
// Console.WriteLine("Done");
//
// var diff1 = diffService.DiffSnapshots(snapshot1, snapshot2);
// var diff2 = diffService.DiffSnapshots(snapshot1, snapshot3);
// var diff3 = diffService.DiffSnapshots(workingDirectory.GetCurrentSnapshot(rules), snapshot2);

//
// var all = commitService.GetCommitsChain(commit5.Id);
// var first4 = commitService.GetCommitsChain(commit4.Id);
// var last3 = commitService.GetCommitsChain(commit5.Id, commit3.Id);
// var mid3 = commitService.GetCommitsChain(commit4.Id, commit2.Id);
//
// var files1 = commitService.GetSnapshotForCommit(commit1.Id);
// var files2 = commitService.GetSnapshotForCommit(commit2.Id);
// var files3 = commitService.GetSnapshotForCommit(commit3.Id);
// var files4 = commitService.GetSnapshotForCommit(commit4.Id);
// var files5 = commitService.GetSnapshotForCommit(commit5.Id);
//
// Console.WriteLine("Done!");
// using BenchmarkDotNet.Running;
// using PlaygroundCLI;
//
// BenchmarkRunner.Run<Benchmark>();

// using System.Diagnostics;
// using System.IO.Hashing;
// using System.Text;
// using Core.Config;
// using Core.DI;
// using Core.Storage;
// using Core.Blobs;
// using Core.FileChanges;
// using Core.FileSnapshots;
//
// var services = new ServiceContainer();
//
// var configService = new ConfigService();
// services.Use<IConfigService>(configService);
// services.Use<IBlobStorageBackend, LocalBlobStorageBackend>();
//
// configService.Set("repo.dir", "./repo/");
// configService.Set("blob.dir", "blobs/");
//
// var blobStore = services.Resolve<BlobStore>();
// var fileSnapshotStore = services.Resolve<FileSnapshotStore>();
// var changesStore = services.Resolve<FileChangeStore>();
//
// Stopwatch sw = new Stopwatch();
//
//
// Console.WriteLine("Testing simultaneous hashing and saving...");
// sw.Start();
// var fs = File.OpenRead("./data/video.mp4");
// var blob = blobStore.Add(fs);
// fs.Close();
// sw.Stop();
// Console.WriteLine($"Hash: {blob.Id}");
// Console.WriteLine($"Elapsed time: {sw.ElapsedMilliseconds}ms");
//
//
// Console.WriteLine("Testing separate hashing and saving...");
// sw.Restart();
// fs = File.OpenRead("./data/video.mp4");
//
// var hasher = new XxHash128();
// hasher.Append(fs);
// var hash = hasher.GetHashAndReset();
// fs.Seek(0, SeekOrigin.Begin);
//
// var ws = File.OpenWrite($"./repo/blobs/{Convert.ToHexString(hash)}-alt");
// fs.CopyTo(ws);
// fs.Close();
// ws.Close();
//
// sw.Stop();
// Console.WriteLine($"Hash: {Convert.ToHexString(hash)}");
// Console.WriteLine($"Elapsed time: {sw.ElapsedMilliseconds}ms");

//
// void ProcessFile(string filePath)
// {
//     var sw = new Stopwatch();
//     sw.Start();
//
//     var fileStream = File.OpenRead(filePath);
//     var blob = blobStore.Add(fileStream);
//     fileStream.Close();
//     Console.WriteLine($"Elapsed time: {sw.ElapsedMilliseconds}ms (blob for {filePath})");
//
//     fileSnapshotStore.Add(filePath, blob.Id, File.GetLastWriteTimeUtc(filePath));
//     Console.WriteLine($"Elapsed time: {sw.ElapsedMilliseconds}ms (file snapshot for {filePath})");
//
//     sw.Stop();
// }
//
// Console.WriteLine(blob1.Id == blob2.Id);
// Console.WriteLine(blob1 == blob2);
// Console.WriteLine(fileSnapshot1.Id == fileSnapshot2.Id);
// Console.WriteLine(fileSnapshot1 == fileSnapshot2);
//
// var hasher = new XxHash128();
// hasher.Append(File.OpenRead("./Program.cs"));
// Console.WriteLine($"Manual Hash: {Convert.ToHexString(hasher.GetHashAndReset())}");

// var store = services.Resolve<BlobStore>();
//
// var sw = new Stopwatch();
// sw.Start();
// store.Add(File.OpenRead("./data/Piano.wav"));
// sw.Stop();
// Console.WriteLine(sw.ElapsedMilliseconds);


// store.Add(File.OpenRead("./Program.cs"));
// store.Add(File.OpenRead("./Program.cs"));
// var blob = store.Add(File.OpenRead("./Program.cs"));
//
// var metadata = store.Get(blob.Id);
// Console.WriteLine($"Id: {metadata.Id}");
// Console.WriteLine($"Length: {metadata.Length}");
// Console.WriteLine($"Hash: {Convert.ToBase64String(metadata.Hash)}");
//
// using (var reader = new StreamReader(store.GetContent(metadata.Id), Encoding.UTF8, leaveOpen: true))
// {
//     var result = reader.ReadToEnd();
//     Console.WriteLine(result);
// }
//
// using (var reader = new StreamReader(store.GetContent(metadata.Id), Encoding.UTF8, leaveOpen: true))
// {
//     var result = reader.ReadToEnd();
//     Console.WriteLine(result);
// }
//
// using (var reader = new StreamReader(store.GetContent(metadata.Id), Encoding.UTF8, leaveOpen: true))
// {
//     var result = reader.ReadToEnd();
//     Console.WriteLine(result);
// }