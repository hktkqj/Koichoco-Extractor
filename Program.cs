using System.IO;
using System.Threading.Tasks;
using SkiaSharp;

namespace Decrypt
{
    class Program
    {
        static void Combine(string IncludePath)
        {
            var CgCombine = new CgCombine(IncludePath);
            var Counter = 0;
            var FailedEntry = new Dictionary<string, List<string>>();
            IncludePath = Path.Combine(IncludePath, "CombinedCG");
            if (!Directory.Exists(IncludePath))
            {
                Directory.CreateDirectory(IncludePath);
            }
            Parallel.ForEach(CgCombine.CgList, cg =>
            {
                var exportFilename = cg.Key;
                Console.WriteLine($"Combining {exportFilename}({cg.Value.Count} files)...{Counter}/{CgCombine.CgList.Count}");
                var BitmapData = CgCombine.Combiner(cg.Value);
                if (BitmapData != null)
                {
                    Counter++;
                    var outputPath = Path.Combine(IncludePath, exportFilename);
                    using var outputStream = new FileStream(outputPath, FileMode.Create, FileAccess.Write);
                    BitmapData.Encode(outputStream, SKEncodedImageFormat.Png, 100);
                }
                else
                {
                    Console.WriteLine($"Failed to combine {exportFilename}.");
                    FailedEntry.Add(exportFilename, cg.Value);
                }
            });
            Console.WriteLine($"Total {Counter} of {CgCombine.CgList.Count} CGs combined, saving to {IncludePath}/.");
            Console.WriteLine($"Failed to combine {FailedEntry.Count} CGs:");
            foreach (var entry in FailedEntry)
            {
                Console.WriteLine($"  Failed to combine {entry.Key} from {string.Join(", ", entry.Value)}.");
            }
        }
        static void Main(string[] args)
        {
            if (args.Length == 0 || args.Length > 5)
            {
                Console.WriteLine("Usage: ");
                Console.WriteLine("       decrypt.exe -e <dat_file> -d <dir> // Extract files from dat file.");
                Console.WriteLine("       decrypt.exe -l <dat_file>          // List files in dat file.");
                Console.WriteLine("       decrypt.exe -c <dir>               // Make sure system.dat, f_graphics.dat, ");
                Console.WriteLine("                                             graphics.dat are extracted to same folder.");
                return;
            }
            string datFile;
            string outputDir = "";
            bool showOnly;
            if (args[0] == "-l" && args.Length == 2)
            {
                showOnly = true;
                datFile = args[1];
            }
            else if (args[0] == "-e" && args[2] == "-d" && args.Length == 4)
            {
                showOnly = false;
                datFile = args[1];
                outputDir = args[3];
            }
            else if (args[0] == "-c" && args.Length == 2)
            {
                outputDir = args[1];
                Combine(outputDir);
                return;
            }
            else
            {
                Console.WriteLine("Invalid arguments.");
                return;
            }
            var FileHandler = new PRead(datFile);
            if (showOnly)
            {
                FileHandler.ShowFileEntries();
                return;
            } else {
                if (!Directory.Exists(outputDir))
                {
                    Console.WriteLine("Output directory does not exist.");
                    return;
                } else {
                    var ExtractCount = 0;
                    Console.WriteLine($"Extracting {FileHandler.TableIndex.Count} files to {outputDir}...");
                    foreach (var entry in FileHandler.TableIndex)
                    {
                        string filePath = Path.Combine(outputDir, entry.Key);
                        Directory.CreateDirectory(Path.GetDirectoryName(filePath) ?? string.Empty);
                        using var outputStream = new FileStream(filePath, FileMode.Create, FileAccess.Write);
                        Console.Write($"Extracting {entry.Key} to {filePath}...");
                        var binary = FileHandler.Data(entry.Key);
                        if (binary != null)
                        {
                            outputStream.Write(binary, 0, binary.Length);
                            outputStream.Close();
                            ExtractCount++;
                            Console.WriteLine("DONE");
                        }
                        else
                        {
                            Console.WriteLine("FAILED");
                        }
                    }
                    Console.WriteLine($"Extracted {ExtractCount} files successfully.({FileHandler.TableIndex.Count - ExtractCount} failed)");
                }
            }
        }
    }
}