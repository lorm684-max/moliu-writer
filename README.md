# 墨流写作间

一个本地沉浸式写作小程序。界面围绕 3D 悬浮空间、液态玻璃、中文书写氛围和环境音设计，适合写随笔、短文、片段灵感。

## 当前内容

- `app/`：网页版本程序，可以直接打开 `index.html` 体验。
- `app/audio/`：氛围音效资源。
- `desktop/`：Windows 独立桌面版外壳源码，基于 Microsoft Edge WebView2。
- `scripts/`：辅助脚本。

## 直接运行

进入 `app` 文件夹，双击 `index.html` 即可打开。

如果需要独立桌面窗口，可以使用之前打包出的 `MoliuWriter.exe`，或者按下面的方式重新构建。

## 构建 Windows 桌面版

1. 确认电脑已安装 Microsoft Edge WebView2 Runtime。
2. 运行 `scripts/build-windows.ps1`。
3. 构建结果会生成在 `dist/` 文件夹。

如果脚本提示找不到 WebView2 组件，需要先安装 WebView2 Runtime 或把 `Microsoft.Web.WebView2.Core.dll`、`Microsoft.Web.WebView2.WinForms.dll`、`WebView2Loader.dll` 放到脚本能找到的位置。

## GitHub 发布建议

建议仓库只保存源码和资源，不直接提交 `.exe`、`.zip`、视频成品等大文件。安装包可以之后放到 GitHub Releases。

