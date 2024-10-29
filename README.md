# Project: File Bundler for Large Document Projects

## Overview
This CLI project is a command-line tool designed to take multiple files in various formats from a larger project and consolidate them into a single text file. It’s useful for combining extensive documentation or source code files into one organized output file. 

## Features
- **Specify Output**: Choose an output file to bundle all selected files.
- **Language Filtering**: Filter files by specific file extensions (languages) or choose all files.
- **Optional Metadata**: Add notes with file paths and specify the author in the output.
- **Sorting**: Organize files by name or type before bundling.
- **Empty Line Removal**: Remove blank lines for a more compact result.
  
## Requirements
- .NET SDK 6.0 or higher

## Usage

The project has two main commands:

1. **`bundle`**  
   This command takes multiple options to customize the bundling process.

   - `--output` or `--o`: Required. Specify the output file.
   - `--language` or `--l`: Required. Specify file extensions (e.g., `cs, txt`), or use "all" for all types.
   - `--note` or `--n`: Optional. Include a note with the original file path.
   - `--sort` or `--s`: Optional. Sort files by `name` or `type`.
   - `--remove-empty-line` or `--rel`: Optional. Remove empty lines from files.
   - `--author` or `--a`: Optional. Specify the author name in the output.

2. **`create-rsp`**  
   This command assists in creating a `.rsp` file for bundling with custom options.

### Example Command

```bash
dotnet run bundle --output "output.txt" --language "cs,txt" --note --sort "name" --remove-empty-line --author "John Doe"
```

### This Command Will:

- **Combine** all `.cs` and `.txt` files.
- **Include** file paths as notes.
- **Sort** by file name.
- **Remove** empty lines.
- **Include** "John Doe" as the author.

### Error Handling
The program handles common errors, such as invalid paths and unauthorized access, and provides helpful messages for troubleshooting.

