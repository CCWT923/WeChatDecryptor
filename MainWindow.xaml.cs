using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WeChatDecryptor
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Process? wechat = Process.GetProcessesByName("WeChat").FirstOrDefault();
            //没有找到微信进程
            if(wechat == null)
            {
                Tbx_Key.Text = "没有找到微信进程";
                return;
            }

            IntPtr wechatHandle = WinApi.OpenProcess(WinApi.PROCESS_VM_READ | WinApi.PROCESS_QUERY_INFORMATION, false, wechat.Id);
            if(wechatHandle == IntPtr.Zero)
            {
                Tbx_Key.Text = "无法打开微信进程";
                return;
            }

            bool found = KeyDecryptor.GetWeChatKey(wechatHandle, out IntPtr keyAddress, out string key);
            if (found)
            {
                Tbx_Key.Text = "Key: " + key;
            }
            WinApi.CloseHandle(wechatHandle);
        }
        
    }
}
