# 恋爱与选举与巧克力（恋と選択とチョコレート）资源提取

## 文件结构

- **Decrypt**: 包含解密相关的代码和工具。
- **README.md**: 当前的项目说明文档。

## 使用方法

1. 确保已安装 .NET SDK（.NET 6.0）。
2. 打开命令行工具，导航到 `Decrypt` 文件夹。
3. 执行`dotnet add package SkiaSharp`来通过`NuGet`安装依赖的图形库。
4. 执行`dotnet build`命令以编译工具，编译的二进制可以在`bin\`文件夹下找到。

## 运行

- 查看资源文件中所有信息

```bash
./Decrype.exe -l <dat_file>
```

- 提取所有文件到指定目录

```bash
# 确保<output_dir>存在
./Decrypt -e <dat_file_path> -d <output_dir>
```

- 合并所有CG

```bash
# 确保已将system.dat, f_graphics.dat和graphics.dat提取至 <output_dir> 中
./Decrypt -c <output_dir>
```

## 注意事项

- 请勿将解密后的文件用于非法用途。
- 确保您拥有游戏的合法副本。

## 贡献

如果您对该项目有改进建议或发现问题，欢迎提交 Issue 或 Pull Request。
