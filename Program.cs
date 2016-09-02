using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Purifier
{
    internal class Program
    {
        //private static string temp_dir = Path.Combine(Path.GetTempPath(), "Purifier");

        private static string EscapePath(string path)
        {
            return "\"" + Regex.Replace(path, @"(\\+)$", @"$1$1") + "\"";
        }

        //private static void ExtractEmbeddedResource(string outputDir, string resourceLocation, string file)
        //{
        //    try {
        //        var filePath = Path.Combine(outputDir, file);
        //        byte[] resource;
        //        using (Stream stream = System.Reflection.Assembly.GetExecutingAssembly().GetManifestResourceStream(resourceLocation + @"." + file)) {
        //            using (MemoryStream a = new MemoryStream()) {
        //                stream.CopyTo(a);
        //                resource = a.ToArray();
        //            }
        //        }
        //        if (File.Exists(filePath)) {
        //            var b = File.ReadAllBytes(filePath);
        //            if (resource.SequenceEqual(b)) {
        //                return;
        //            }
        //        }
        //        File.WriteAllBytes(filePath, resource);

        //    } catch (Exception) { }
        //}

        private static int Main(string[] args)
        {
            if (args.Length < 1)
            {
                return 1;
            }
            var path = String.Join(" ", args);
            if (!File.Exists(path) && !Directory.Exists(path))
            {
                return 1;
            }
            //System.IO.Directory.CreateDirectory(temp_dir);
            //ExtractEmbeddedResource(temp_dir, "Purifier", "gifsicle.exe");
            //ExtractEmbeddedResource(temp_dir, "Purifier", "optipng.exe");
            //ExtractEmbeddedResource(temp_dir, "Purifier", "mozjpeg.exe");
            if (Directory.Exists(path))
            {
                var all_files = Directory.GetFiles(path, "*.*", SearchOption.AllDirectories);

                var gif_files = all_files.Where(file => file.EndsWith(".gif", StringComparison.OrdinalIgnoreCase)).ToArray();
                var all_jpegs = all_files.Where(file => file.EndsWith(".jpg", StringComparison.OrdinalIgnoreCase) || file.EndsWith(".jpeg", StringComparison.OrdinalIgnoreCase)).ToArray();
                var png_files = all_files.Where(file => file.EndsWith(".png", StringComparison.OrdinalIgnoreCase)).ToArray();

                int filesToOptimize = gif_files.Length + png_files.Length + all_jpegs.Length;

                var currentFile = 0;
                Parallel.For(0, gif_files.ToArray().Length, (i) =>
                {
                    OptimizeGif(gif_files[i]);
                    currentFile++;
                    Console.WriteLine(currentFile + "/" + filesToOptimize + " | " + Math.Round((currentFile / (float)filesToOptimize) * 100, 2) + "%");
                });
                Parallel.For(0, png_files.ToArray().Length, (i) =>
                {
                    OptimizePng(png_files[i]);
                    currentFile++;
                    Console.WriteLine(currentFile + "/" + filesToOptimize + " | " + Math.Round((currentFile / (float)filesToOptimize) * 100, 2) + "%");
                });
                Parallel.For(0, all_jpegs.ToArray().Length, (i) =>
                {
                    OptimizeJpeg(all_jpegs[i]);
                    currentFile++;
                    Console.WriteLine(currentFile + "/" + filesToOptimize + " | " + Math.Round((currentFile / (float)filesToOptimize) * 100, 2) + "%");
                });
            }
            else if (path.ToLower().EndsWith(".jpg", true, null) || path.ToLower().EndsWith(".jpeg", true, null))
            {
                OptimizeJpeg(path);
            }
            else if (path.ToLower().EndsWith(".png", true, null))
            {
                OptimizePng(path);
            }
            else if (path.ToLower().EndsWith(".gif", true, null))
            {
                OptimizeGif(path);
            }
            return 0;
        }

        private static void OptimizeGif(string file_path)
        {
            using (Process the_process = new Process())
            {
                the_process.StartInfo = new ProcessStartInfo()
                {
                    WindowStyle = ProcessWindowStyle.Hidden,
                    ErrorDialog = false,
                    UseShellExecute = true,
                };
                the_process.StartInfo.FileName = "gifsicle.exe";
                var tempFileName = Path.GetTempFileName();
                the_process.StartInfo.Arguments = "-O3 -o " + EscapePath(tempFileName) + " " + EscapePath(file_path);
                Console.WriteLine(@"Optimizing " + file_path + @".");
                the_process.Start();
                the_process.WaitForExit();
                FileInfo tempFileInfo = new FileInfo(tempFileName);
                FileInfo originalFileInfo = new FileInfo(file_path);
                if (tempFileInfo.Length < originalFileInfo.Length && tempFileInfo.Length != 0)
                {
                    File.Delete(file_path);
                    File.Move(tempFileName, file_path);
                }
            }
            return;
        }
        private static void OptimizePng(string file_path)
        {
            using (Process the_process = new Process())
            {
                the_process.StartInfo = new ProcessStartInfo()
                {
                    WindowStyle = ProcessWindowStyle.Hidden,
                    ErrorDialog = false,
                    UseShellExecute = true,
                };
                file_path = EscapePath(file_path);
                the_process.StartInfo.FileName = "optipng.exe";
                the_process.StartInfo.Arguments = "-clobber -fix -quiet -strip all " + file_path;
                Console.WriteLine(@"Optimizing " + file_path + @".");
                the_process.Start();
                the_process.WaitForExit();
            }
            return;
        }
        private static void OptimizeJpeg(string file_path)
        {
            using (Process the_process = new Process())
            {
                the_process.StartInfo = new ProcessStartInfo()
                {
                    WindowStyle = ProcessWindowStyle.Hidden,
                    ErrorDialog = false,
                    UseShellExecute = true,
                };
                file_path = EscapePath(file_path);
                the_process.StartInfo.FileName = "jpegtran.exe";
                the_process.StartInfo.Arguments = "-copy none -outfile " + file_path + " " + file_path;
                Console.WriteLine(@"Optimizing " + file_path + @".");
                the_process.Start();
                the_process.WaitForExit();
            }
            return;
        }
    }
}