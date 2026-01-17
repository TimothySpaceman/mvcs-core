// using System.Diagnostics;
// using System.IO.Hashing;
//
// if (args.Length < 1)
// {
//     Console.Error.WriteLine("File path is not provided");
//     return;
// }
//
// var filePath = args[0];
// if (!File.Exists(filePath))
// {
//     Console.Error.WriteLine("File does not exist");
//     return;
// }
//
// Console.WriteLine("Computing hashes for \"{0}\"...\n", filePath);
//
// var results = new List<HashStats>();
//
// results.AddRange(BenchmarkHash(filePath, new XxHash3(), 10));
// results.AddRange(BenchmarkHash(filePath, new XxHash32(), 10));
// results.AddRange(BenchmarkHash(filePath, new XxHash64(), 10));
// results.AddRange(BenchmarkHash(filePath, new XxHash128(), 10));
//
// const int algoWidth = 10;
// const int elapsedWidth = 10;
// Console.WriteLine("Stats for hashing \"{0}\":", filePath);
// Console.WriteLine($"|{"Algo",-algoWidth}|{"Time (ms)",-elapsedWidth}|Hash");
//
// foreach (var stats in results)
// {
//     var hashString = Convert.ToBase64String(stats.Hash);
//     Console.WriteLine($"|{stats.Algo,-algoWidth}|{stats.TimeTaken,-elapsedWidth}|{hashString}");
// }
//
// return;
//
//
// HashStats ComputeHash(string path, NonCryptographicHashAlgorithm hashAlgo)
// {
//     var sw = new Stopwatch();
//     sw.Start();
//     using var fs = File.OpenRead(path);
//     hashAlgo.Append(fs);
//     var hash = hashAlgo.GetHashAndReset();
//     sw.Stop();
//     return new HashStats(path, hashAlgo.GetType().Name, hash, sw.ElapsedMilliseconds);
// }
//
// List<HashStats> BenchmarkHash(string path, NonCryptographicHashAlgorithm hashAlgo, int iterations)
// {
//     var benchmarks = new List<HashStats>();
//     for (var i = 0; i < iterations; i++)
//     {
//         benchmarks.Add(ComputeHash(path, hashAlgo));
//     }
//
//     return benchmarks;
// }
//
// record HashStats(string FilePath, string Algo, byte[] Hash, long TimeTaken);