# LuoguPaintboardPro

2021 版洛谷冬日画板工具, 支持彩色图片, 多账号, 自修复

使用 .Net Core 打造

## 快速使用说明

需要先安装 [.Net Core SDK](https://dotnet.microsoft.com/download)

```sh
git clone https://github.com/Duanyll/LuoguPaintboardPro
cd LuoguPaintboardPro/LuoguPaintboardPro
dotnet build
dotnet run
```

将图片处理为 32 色图(使用Floyd–Steinberg_dithering算法)：

```sh
dotnet run genpic picture.png
```
生成 `preview.png` 和 `data.txt`

开始绘制:

```sh
dotnet run draw x y 
```

用 x y 指定左上角坐标

## cookie 格式

`cookie.txt` 文件：

```
__client_id=abcdefghijklmnopqrstuvwxyz1234567890abcd; _uid=48256
__client_id=abcdefghijklmnopqrstuvwxyz1234567890abcd; _uid=xxxxx
__client_id=abcdefghijklmnopqrstuvwxyz1234567890abcd; _uid=xxxxx
__client_id=abcdefghijklmnopqrstuvwxyz1234567890abcd; _uid=xxxxx
__client_id=abcdefghijklmnopqrstuvwxyz1234567890abcd; _uid=xxxxx
```

有多余 cookie 不影响，但必须有 `__client_id` 和 `_uid`

## 命令行参数

```
Usage: LuoguPaintboardPro [options] [command]

Options:
  -h|--help     Show help information
  -v|--version  Show version information

Commands:
  draw    在指定坐标绘制图片, 并监视保护.
  genpic  将彩色图片处理为 32 色图.

Use "LuoguPaintboardPro [command] --help" for more information about a command.

Usage: LuoguPaintboardPro genpic [arguments] [options]

Arguments:
  inputFile     输入的 png 图像.
  [outputFile]  输出文件.

Options:
  -h|--help  Show help information

Usage: LuoguPaintboardPro draw [arguments] [options]

Arguments:
  xpos    绘制图片位置的 x 坐标
  ypos    绘制图片位置的 Y 坐标
  image   由 genpic 生成的代表图片的 txt
  cookie  包含要使用的 cookie 的 txt 文件, 一行一个 cookie.

Options:
  -h|--help  Show help information
```

## 说明


1. 可以多开，只需要多开终端，并且单独指定 `data.txt` 和 `cookie.txt`
2. 切换按顺序绘制和随机绘制: 修改[此处](https://github.com/Duanyll/LuoguPaintboardPro/blob/ecd0663450c04b868dc98e15e9cfb1eb46ac1d84/LuoguPaintboardPro/PointToDraw.cs#L30)
3. mac 使用：需要 `brew install mono-libgdiplus`
