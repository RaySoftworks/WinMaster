using System;
using System.Net;
using System.Windows;
using System.Management;
using System.Diagnostics;
using System.Windows.Input;
using System.Windows.Threading;
using System.Security.Principal;
using System.Management.Automation;
using System.Runtime.InteropServices;
using System.Collections.ObjectModel;

namespace Typhoon
{
    public partial class Window2 : Window
    {
        private PerformanceCounter uptimeCounter;
        private DispatcherTimer timer;
        public Window2()

        {
            InitializeComponent();

            WinInfo();
            SetUsername();
            DisplayGPUInfo();
            DisplayCPUInfo();
            DisplayRAMInfo();
            DisplayDiskInfo();
            DisplayHostInfo();
            InitializeTimer();
            DisplayKernelInfo();
            InitializeUptimeCounter();

            INFO.Visibility = Visibility.Hidden;
            Credits.Visibility = Visibility.Hidden;
            RestartSys.Visibility = Visibility.Hidden;
        }

        private void InitializeUptimeCounter()
        {
            uptimeCounter = new PerformanceCounter("System", "System Up Time");
            uptimeCounter.CategoryName = "System";
            uptimeCounter.CounterName = "System Up Time";
        }

        private void InitializeTimer()
        {
            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Tick += Timer_Tick;
            timer.Start();
        }


        private void Timer_Tick(object sender, EventArgs e)
        {
            UpdateUptimeLabel();
        }

        private void UpdateUptimeLabel()
        {
            TimeSpan uptime = TimeSpan.FromSeconds(uptimeCounter.NextValue());
            lblUptime.Content = $"UPTIME: {uptime.Days} D, {uptime.Hours} H, {uptime.Minutes} MIN, {uptime.Seconds} S";
        }

        private void DisplayCPUInfo()
        {
            try
            {
                ManagementObjectSearcher searcher = new ManagementObjectSearcher("SELECT * FROM Win32_Processor");
                ManagementObjectCollection collection = searcher.Get();

                foreach (ManagementObject obj in collection)
                {
                    string processorName = obj["Name"].ToString();
                    string numberOfCores = obj["NumberOfCores"].ToString();
                    string numberOfLogicalProcessors = obj["NumberOfLogicalProcessors"].ToString();
                    string architecture = obj["Architecture"].ToString();
                    string maxClockSpeed = obj["MaxClockSpeed"].ToString();
                    cpuInfoLabel.Content = $"CPU: {processorName}\nCores: {numberOfCores}\nLogical Processors: {numberOfLogicalProcessors}\nArchitecture: {architecture}\nMax Clock Speed: {maxClockSpeed} MHz";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Can't Download CPU Info: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void WinInfo()
        {
            string windowsEdition = GetWindowsEdition();
            lblWindowsInfo.Content = $"WIN EDITION: {windowsEdition}";
        }

        private string GetWindowsEdition()
        {
            try
            {
                string edition = "Unknown";

                using (var key = Microsoft.Win32.Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Windows NT\CurrentVersion"))
                {
                    if (key != null)
                    {
                        edition = key.GetValue("EditionID") as string;
                    }
                }

                return edition;
            }
            catch (Exception ex)
            {
                return $"Error: {ex.Message}";
            }
        }


        private void DisplayRAMInfo()
        {
            try
            {
                ManagementObjectSearcher searcher = new ManagementObjectSearcher("SELECT * FROM Win32_ComputerSystem");
                ManagementObjectCollection collection = searcher.Get();

                foreach (ManagementObject obj in collection)
                {
                    ulong totalRAMBytes = Convert.ToUInt64(obj["TotalPhysicalMemory"]);
                    ulong totalRAMMegabytes = totalRAMBytes / (1024 * 1024);
                    ramInfoLabel.Content = $"RAM: {totalRAMMegabytes} MB";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Can't Download Ram Info: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void DisplayKernelInfo()
        {
            string kernelVersion = GetKernelVersion();
            lblKernelVersion.Content = $"KERNEL: {kernelVersion}";
        }

        private string GetKernelVersion()
        {
            try
            {
                var osInfo = new OSVERSIONINFOEX();
                osInfo.dwOSVersionInfoSize = Marshal.SizeOf(typeof(OSVERSIONINFOEX));
                if (GetVersionEx(ref osInfo) != 0)
                {
                    return $"{osInfo.dwMajorVersion}.{osInfo.dwMinorVersion} Build {osInfo.dwBuildNumber}";
                }
                else
                {
                    return "Can't Download Kernel Info";
                }
            }
            catch (Exception ex)
            {
                return $"Error: {ex.Message}";
            }

        }

        [StructLayout(LayoutKind.Sequential)]
        private struct OSVERSIONINFOEX
        {
            public int dwOSVersionInfoSize;
            public int dwMajorVersion;
            public int dwMinorVersion;
            public int dwBuildNumber;
            public int dwPlatformId;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
            public string szCSDVersion;
            public ushort wServicePackMajor;
            public ushort wServicePackMinor;
            public ushort wSuiteMask;
            public byte wProductType;
            public byte wReserved;
        }

        [DllImport("kernel32.dll")]
        private static extern int GetVersionEx(ref OSVERSIONINFOEX osInfo);

        private void DisplayHostInfo()
        {
            string hostName = Environment.MachineName;
            string hostInfo = $"HOST: {hostName}";
            lblHostInfo.Content = hostInfo;
        }

        private void DisplayDiskInfo()
        {
            try
            {
                ManagementObjectSearcher searcher = new ManagementObjectSearcher("SELECT * FROM Win32_DiskDrive");
                ManagementObjectCollection collection = searcher.Get();

                foreach (ManagementObject obj in collection)
                {
                    string diskModel = obj["Model"].ToString();
                    ulong diskSizeBytes = Convert.ToUInt64(obj["Size"]);
                    ulong diskSizeGigabytes = diskSizeBytes / (1024 * 1024 * 1024);
                    diskInfoLabel.Content = $"DISK: {diskModel}\n SIZE: {diskSizeGigabytes} GB";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Can't Download Disk Info: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void DisplayGPUInfo()
        {
            try
            {
                ManagementObjectSearcher searcher = new ManagementObjectSearcher("SELECT * FROM Win32_VideoController");
                ManagementObjectCollection collection = searcher.Get();

                foreach (ManagementObject obj in collection)
                {
                    string gpuName = obj["Name"].ToString();
                    string adapterRAM = obj["AdapterRAM"].ToString();
                    string driverVersion = obj["DriverVersion"].ToString();

                    object videoModeDescriptionObject = obj["VideoModeDescription"];
                    string videoModeDescription = (videoModeDescriptionObject != null) ? videoModeDescriptionObject.ToString() : "Brak dostępnych informacji o trybie wideo";

                    gpuInfoLabel.Content = $"GPU: {gpuName}\nAdapter Ram: {adapterRAM} KB\nDriver Version: {driverVersion}\nVideo Mode Desc: {videoModeDescription}";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Can't Download GPU Information: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void SetUsername()
        {
            WindowsIdentity currentUser = WindowsIdentity.GetCurrent();
            if (currentUser != null)
            {
                string username = currentUser.Name;
                if (username.Contains("\\"))
                {
                    username = username.Split('\\')[1];
                }

                usernameLabel.Content = "User: " + username;
            }
            else
            {
                usernameLabel.Content = "Can't Show UserName.";
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Environment.Exit(0);
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            if (Credits.Visibility == Visibility.Hidden)
            {
                Credits.Visibility = Visibility.Visible;
            }
            else
            {
                Credits.Visibility = Visibility.Hidden;

            }
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            Credits.Visibility = Visibility.Hidden;
        }

        private void TopBorder_MouseDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            if (INFO.Visibility == Visibility.Hidden)
            {
                INFO.Visibility = Visibility.Visible;
                RootBorder.Visibility = Visibility.Hidden;
            }
            else
            {
                INFO.Visibility = Visibility.Hidden;

            }
        }

        private void Button_Click_4(object sender, RoutedEventArgs e)
        {
            if (RootBorder.Visibility == Visibility.Hidden)
            {
                INFO.Visibility = Visibility.Hidden;
                RootBorder.Visibility = Visibility.Visible;
            }
            else
            {
                INFO.Visibility = Visibility.Visible;

            }
        }

        private void Button_Click_5(object sender, RoutedEventArgs e)
        {
            string scriptUrl = "https://massgrave.dev/get";

            try
            {
                using (WebClient client = new WebClient())
                {
                    string scriptContent = client.DownloadString(scriptUrl);
                    RunPowerShellScript(scriptContent);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An Error: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void RunPowerShellScript(string scriptContent)
        {
            try
            {
                using (PowerShell PowerShellInstance = PowerShell.Create())
                {
                    PowerShellInstance.AddScript(scriptContent);
                    Collection<PSObject> result = PowerShellInstance.Invoke();

                    foreach (PSObject outputItem in result)
                    {
                        if (outputItem != null)
                        {
                            Console.WriteLine(outputItem.BaseObject.ToString());
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An Erroc Accured Opening PowerShell: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Button_Click_7(object sender, RoutedEventArgs e)
        {
            Process.Start("Powershell.exe", "utils\\wtbbc.ps1");
        }

        private void Button_Click_6(object sender, RoutedEventArgs e)
        {
            Process.Start("utils\\phv2\\ProcessHacker.exe");
        }

        private void Button_Click_8(object sender, RoutedEventArgs e)
        {
            Process.Start("utils\\rufus-4.4p.exe");
        }

        private void Button_Click_9(object sender, RoutedEventArgs e)
        {
            Process.Start("utils\\dc\\dControl.exe");
        }

        private void Button_Click_10(object sender, RoutedEventArgs e)
        {
            Process.Start("Powershell.exe" ,"utils\\scoop.ps1");
        }

        private void Button_Click_11(object sender, RoutedEventArgs e)
        {
            Process.Start("shutdown", "/r /t 0");
        }

        private void Button_Click_12(object sender, RoutedEventArgs e)
        {
            if (RestartSys.Visibility == Visibility.Hidden)
            {
                RestartSys.Visibility = Visibility.Visible;
                RootBorder.Visibility = Visibility.Hidden;
            }
            else
            {
                RestartSys.Visibility = Visibility.Hidden;

            }
        }

        private void Button_Click_13(object sender, RoutedEventArgs e)
        {
            if (RootBorder.Visibility == Visibility.Hidden)
            {
                RestartSys.Visibility = Visibility.Hidden;
                RootBorder.Visibility = Visibility.Visible;
            }
            else
            {
                RestartSys.Visibility = Visibility.Visible;

            }
        }
    }   
}
