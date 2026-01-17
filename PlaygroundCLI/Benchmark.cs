// using System.IO.Hashing;
// using BenchmarkDotNet.Attributes;
// using Core.Blobs;
// using Core.Config;
// using Core.DI;
// using Core.FileChanges;
// using Core.FileSnapshots;
//
// namespace PlaygroundCLI;
//
// [MemoryDiagnoser]
// public class Benchmark
// {
//     public ServiceContainer services;
//     public IConfigService configService;
//
//     public BlobMetadataStore BlobMetadataStore;
//     public FileSnapshotStore fileSnapshotStore;
//     public FileChangeStore changesStore;
//
//     public string filePath;
//
//     [GlobalSetup]
//     public void Setup()
//     {
//         var baseDir = AppContext.BaseDirectory;
//
//         filePath = Path.Combine(baseDir, "data", "video.mp4");
//
//         Directory.CreateDirectory(Path.GetDirectoryName(filePath));
//         if (!File.Exists(filePath))
//         {
//             File.Copy("C:/projects/mvcs/MVCS/PlaygroundCLI/data/video.mp4", filePath);
//         }
//
//         services = new ServiceContainer();
//
//         configService = new ConfigService();
//         services.Use<IConfigService>(configService);
//
//         configService.Set("repo.dir", Path.Combine(baseDir, "repo"));
//         configService.Set("blob.dir", "blobs/");
//
//         services.Use<IBlobStorageBackend, LocalBlobStorageBackend>();
//
//         BlobMetadataStore = services.Resolve<BlobMetadataStore>();
//         fileSnapshotStore = services.Resolve<FileSnapshotStore>();
//         changesStore = services.Resolve<FileChangeStore>();
//     }
//
//     [Benchmark]
//     public void SimultaneousHashing()
//     {
//         var fs = File.OpenRead(filePath);
//         var blob = BlobMetadataStore.Add(fs);
//         fs.Close();
//     }
//
//     [Benchmark]
//     public void SeparateHashing()
//     {
//         var fs = File.OpenRead(filePath);
//
//         var hasher = new XxHash128();
//         hasher.Append(fs);
//         var hash = hasher.GetHashAndReset();
//         fs.Seek(0, SeekOrigin.Begin);
//
//         var ws = File.OpenWrite(Path.Combine(AppContext.BaseDirectory, "repo", "blobs",
//             $"{Convert.ToHexString(hash)}-alt"));
//         fs.CopyTo(ws);
//         fs.Close();
//         ws.Close();
//     }
// }