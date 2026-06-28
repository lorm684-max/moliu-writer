using System;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.Web.WebView2.Core;
using Microsoft.Web.WebView2.WinForms;

namespace MoliuWriterDesktop
{
    internal static class Program
    {
        [STAThread]
        private static void Main()
        {
            ConfigureDpi();
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MoliuWindow());
        }

        private static void ConfigureDpi()
        {
            try
            {
                SetProcessDpiAwareness(2);
            }
            catch
            {
                try { SetProcessDPIAware(); } catch { }
            }
        }

        [DllImport("Shcore.dll")]
        private static extern int SetProcessDpiAwareness(int value);

        [DllImport("user32.dll")]
        private static extern bool SetProcessDPIAware();
    }

    internal sealed class MoliuWindow : Form
    {
        private const string AppTitle = "\u58a8\u6d41\u5199\u4f5c\u95f4";
        private readonly WebView2 webView;

        public MoliuWindow()
        {
            Text = AppTitle;
            StartPosition = FormStartPosition.CenterScreen;
            MinimumSize = new Size(980, 680);
            Size = new Size(1280, 860);
            BackColor = Color.FromArgb(6, 10, 14);
            Icon = SystemIcons.Application;
            AutoScaleMode = AutoScaleMode.Dpi;
            DoubleBuffered = true;

            webView = new WebView2
            {
                Dock = DockStyle.Fill,
                DefaultBackgroundColor = Color.FromArgb(6, 10, 14)
            };
            Controls.Add(webView);

            Load += async (_, __) => await LoadAppAsync();
        }

        private async Task LoadAppAsync()
        {
            string appDir = AppDomain.CurrentDomain.BaseDirectory;
            string indexPath = Path.Combine(appDir, "index.html");
            if (!File.Exists(indexPath))
            {
                MessageBox.Show("Missing index.html.", AppTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
                Close();
                return;
            }

            string userData = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "MoliuWriter",
                "WebView2");
            Directory.CreateDirectory(userData);

            var options = new CoreWebView2EnvironmentOptions(
                "--autoplay-policy=no-user-gesture-required " +
                "--enable-gpu " +
                "--enable-gpu-rasterization " +
                "--enable-zero-copy " +
                "--ignore-gpu-blocklist " +
                "--high-dpi-support=1 " +
                "--force-color-profile=srgb");

            CoreWebView2Environment env = await CoreWebView2Environment.CreateAsync(null, userData, options);
            await webView.EnsureCoreWebView2Async(env);

            webView.ZoomFactor = 1.0;
            webView.CoreWebView2.SetVirtualHostNameToFolderMapping(
                "moliu.local",
                appDir,
                CoreWebView2HostResourceAccessKind.Allow);

            webView.CoreWebView2.Settings.AreDefaultContextMenusEnabled = false;
            webView.CoreWebView2.Settings.AreDevToolsEnabled = true;
            webView.CoreWebView2.Settings.IsStatusBarEnabled = false;
            webView.CoreWebView2.Settings.IsZoomControlEnabled = true;
            webView.CoreWebView2.NewWindowRequested += (_, e) =>
            {
                e.Handled = true;
                if (!string.IsNullOrWhiteSpace(e.Uri)) webView.CoreWebView2.Navigate(e.Uri);
            };

            webView.CoreWebView2.Navigate("https://moliu.local/index.html");
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == Keys.F11)
            {
                ToggleFullscreen();
                return true;
            }
            if (keyData == Keys.Escape && FormBorderStyle == FormBorderStyle.None)
            {
                ToggleFullscreen();
                return true;
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }

        private void ToggleFullscreen()
        {
            if (FormBorderStyle == FormBorderStyle.None)
            {
                FormBorderStyle = FormBorderStyle.Sizable;
                WindowState = FormWindowState.Normal;
            }
            else
            {
                FormBorderStyle = FormBorderStyle.None;
                WindowState = FormWindowState.Maximized;
            }
        }
    }
}
