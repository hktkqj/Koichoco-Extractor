using System.IO;

namespace Decrypt
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length == 0 || args.Length > 5)
            {
                Console.WriteLine("Usage: ");
                Console.WriteLine("       decrypt.exe -e <dat_file> -d <output_dir>");
                Console.WriteLine("       decrypt.exe -l <dat_file>");
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