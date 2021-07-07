using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace moyu
{
    public partial class MoyuForn : Form
    {
        private const int ALT = 0xA4;
        private const int EXTENDEDKEY = 0x1;
        private const int KEYUP = 0x2;
        private const uint Restore = 9;

        [DllImport("user32.dll")]
        private static extern bool SetForegroundWindow(IntPtr hWnd);

        [DllImport("user32.dll")]
        private static extern void keybd_event(byte bVk, byte bScan, uint dwFlags, int dwExtraInfo);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool IsIconic(IntPtr hWnd);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool IsWindow(IntPtr hWnd);

        [DllImport("user32.dll")]
        private static extern int ShowWindow(IntPtr hWnd, uint Msg);

        [DllImport("user32.dll")]
        private static extern IntPtr GetForegroundWindow();

        public static void ActivateWindow(IntPtr mainWindowHandle)
        {
            //check if already has focus
            if (mainWindowHandle == GetForegroundWindow()) return;

            //check if window is minimized
            if (IsIconic(mainWindowHandle))
            {
                ShowWindow(mainWindowHandle, Restore);
            }

            // Simulate a key press
            keybd_event((byte)ALT, 0x45, EXTENDEDKEY | 0, 0);

            //SetForegroundWindow(mainWindowHandle);

            // Simulate a key release
            keybd_event((byte)ALT, 0x45, EXTENDEDKEY | KEYUP, 0);

            SetForegroundWindow(mainWindowHandle);
        }

        public MoyuForn()
        {
            InitializeComponent();
        }

        private IntPtr ProcessMainWindowHandle { get; set; }
        private List<string> ProcessTitles {get; set; }

        private void GetProcessHandle()
        {
            Process[] processList = Process.GetProcesses();
            foreach (var title in ProcessTitles)
            {
                foreach (Process p in processList)
                {
                    if (string.IsNullOrEmpty(p.MainWindowTitle))
                    {
                        continue;
                    }
                    if (p.MainWindowTitle.Contains(title))
                    {
                        ProcessMainWindowHandle = p.MainWindowHandle;
                        return;
                    }
                }
            }
            

            throw new Exception($"{ProcessTitles} not found");
        }

        private void GetProcessTitle()
        {
            string configPath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "processes.txt");
            ProcessTitles = new List<string>();
            if (File.Exists(configPath))
            {
                var lines = File.ReadAllLines(configPath);
                foreach (var line in lines)
                {
                    var lineTrimed = line.Trim();
                    if (lineTrimed.StartsWith("#"))
                    {
                        continue;
                    }
                    if (!string.IsNullOrEmpty(lineTrimed))
                    {
                        ProcessTitles.Add(lineTrimed);
                    }
                }
            }
            if (ProcessTitles.Count == 0)
            {
                throw new Exception("process list not found");
            }
        }

        private Point GetStartLocation()
        {
            var bounds = Screen.PrimaryScreen.Bounds;
            var x = bounds.Width - this.ClientSize.Width;
            var y = bounds.Height / 2 - this.ClientSize.Height / 2;
            return new Point { X = x, Y = y };
        }

        private Size CalculateSize()
        {
            var bounds = Screen.PrimaryScreen.Bounds;
            int w = bounds.Width / 40;
            int h = bounds.Height / 5;
            return new System.Drawing.Size(w, h);
        }

        private void test()
        {
            Process[] processList = Process.GetProcesses();
            foreach (Process p in processList)
            {
                if (string.IsNullOrEmpty(p.MainWindowTitle))
                {
                    continue;
                }
                Console.WriteLine(p.MainWindowTitle);
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            test();
            GetProcessTitle();
            GetProcessHandle();
            this.ClientSize = CalculateSize();
            this.Location = GetStartLocation();
        }

        private void Form1_MouseLeave(object sender, EventArgs e)
        {

        }

        private void Form1_MouseEnter(object sender, EventArgs e)
        {
            try
            {
                var x = this.Location;
                if (!IsWindow(ProcessMainWindowHandle))
                {
                    GetProcessHandle();
                }
                ActivateWindow(ProcessMainWindowHandle);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            
        }
    }
}
