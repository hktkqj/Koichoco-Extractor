# Koichoco (恋と選択とチョコレート) extractor

## Project Overview

This project is designed to handle the decryption of game data for "Koi to Senkyo to Chocolate" ("Love, Election, and Chocolate"). It includes tools and scripts to help users decrypt and read game files.

## File Structure

- **Decrypt**: Contains decryption-related code and tools.
- **README_EN.md**: This project documentation in English.

## Compile Instructions

1. Ensure that .NET SDK (.NET 6.0) is installed.
2. Open a command-line tool and navigate to the `Decrypt` folder.
3. Run `dotnet add package SkiaSharp` to install `SkiaSharp` from `NuGet`.
4. Run `dotnet build` to build and execute the decryption tool, then run executable from `bin\` folder.

## Run

- To list all file entry from .dat file:

```bash
./Decrype.exe -l <dat_file>
```

- To extract file to specific folder:

```bash
# Create <output_dir> before extract
./Decrypt -e <dat_file_path> -d <output_dir>
```

- Combine all CGs

```bash
# Make sure system.dat, f_graphics.dat and graphics.dat are extracted to same folder.
./Decrypt -c <output_dir>
```

## Notes

- Do not use the decrypted files for illegal purposes.
- Ensure you own a legitimate copy of the game.

## Contribution

If you have suggestions for improvements or encounter issues, feel free to submit an Issue or a Pull Request.
