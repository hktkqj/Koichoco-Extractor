using System;
using System.Runtime.CompilerServices;
using SkiaSharp;

namespace Decrypt
{
    public class CgCombine
    {
        public static Dictionary<string, string> CgPathDict = new();
        public static Dictionary<string, List<string>> CgList = new();
        private static readonly List<string> RequiredFolder = new(){"", "def", "evcg", "evcg_f"};

        public CgCombine(string outputFolder)
        {
            foreach (var folder in RequiredFolder)
            {
                string path = Path.Combine(outputFolder, folder);
                if (!Directory.Exists(path))
                {
                    Console.WriteLine($"Directory {path} does not exist.");
                    Console.WriteLine("Make sure system.dat, f_graphics.dat and graphics.dat are extracted to same folder.");
                    return;
                }
            }
            Console.WriteLine($"Collcting CG list(def/vcglist.csv) from {outputFolder}...");
            CgListHandler(Path.Combine(outputFolder, "def", "vcglist.csv"));

            Console.WriteLine($"\nCollecting CGs(evcg/) from {outputFolder}...");
            CgPathCollector(Path.Combine(outputFolder, "evcg"));
            CgPathCollector(Path.Combine(outputFolder, "evcg_f"));

            Console.WriteLine($"\nDone Initialize.");
        }
        public static SKBitmap? Combiner(List<string> FileList)
        {
            SKBitmap? combinedBitmap = null;
            foreach (var file in FileList)
            {
                if (!CgPathDict.ContainsKey(file))
                {
                    Console.WriteLine($"File {file} not found in CgPathDict.");
                    return null;
                }
                var filePath = CgPathDict[file];
                using var inputStream = new SKFileStream(filePath);
                using var bitmap = SKBitmap.Decode(inputStream);
                if (bitmap == null)
                {
                    Console.WriteLine($"Failed to decode {file}@{filePath}.");
                    return null;
                }
                if (combinedBitmap == null)
                {
                    combinedBitmap = new SKBitmap(bitmap.Width, bitmap.Height, bitmap.ColorType, bitmap.AlphaType);
                    using var canvas = new SKCanvas(combinedBitmap);
                    canvas.Clear(SKColors.Transparent);
                } 
                using (var canvas = new SKCanvas(combinedBitmap))
                {
                    canvas.DrawBitmap(bitmap, 0, 0);
                }
            }
            return combinedBitmap;
        }

        public static void CgListHandler(string cgListPath)
        {
            using var reader = new StreamReader(cgListPath);
            string? line;
            int Counter = 0;
            while ((line = reader.ReadLine()) != null)
            {
                // Split the line by whitespace and take the first part as the file name
                string[] parts = line.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                if (parts.Length > 1)
                {
                    string fileName = parts[0].ToLower() + ".png";
                    var filePaths = parts.Skip(1).ToArray().Select(name => name.ToLower() + ".webp").ToArray();
                    CgList[fileName] = new List<string>(filePaths);
                    Counter++;
                }
            }
            Console.WriteLine($"Total {Counter} CGs added from {cgListPath}.");
        }

        public static void CgPathCollector(string CgFolder)
        {
            // List all file in CgFolder
            string[] files = Directory.GetFiles(CgFolder, "*.webp", SearchOption.AllDirectories);
            var Counter = 0;
            foreach (string file in files)
            {
                string fileName = Path.GetFileName(file).ToLower();
                if (!CgPathDict.ContainsKey(fileName))
                {
                    CgPathDict[fileName] = Path.Combine(CgFolder, fileName);
                    Counter++;
                }
            }
            Console.WriteLine($"Collected {Counter} sprite(s) in {CgFolder}.");
        }
    }
}