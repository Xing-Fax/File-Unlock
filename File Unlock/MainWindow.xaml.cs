using IObitUnlocker.Wrapper;
using Microsoft.Win32;
using Microsoft.WindowsAPICodePack.Dialogs;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace File_Unlock
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {

        static string Out_file = string.Empty;
        public MainWindow()
        {
            InitializeComponent();

            if (check.Document_verification() != true)
            {
                MessageBox.Show("签名校验失败,程序可能被篡改,轻击确定以退出程序", "警告");
                Environment.Exit(0);
            }

            日志.Text += "[" + DateTime.Now.ToLongTimeString().ToString() + "]: 程序启动... Copyright © xcz 2021";
            Log("释放驱动文件...");
            IObitController.DriverStart(); //加载驱动
            Log("程序已经准备就绪!");
        }

        private void 开始_Click(object sender, RoutedEventArgs e)
        {
            if(App.Number_file.Count != 0)
            {
                if ((bool)操作3.IsChecked || (bool)操作4.IsChecked)
                {
                    if (Out_file != string.Empty)
                    {
                        Perform_Operation.Operation(App.Number_file, Out_file, this);
                    }
                    else
                    {
                        Log("请选择目标文件夹！");
                    }
                }
                else
                {
                    Perform_Operation.Operation(App.Number_file, Out_file, this);
                }
            }
            else
            {
                Log("请添加要操作的文件！");
            }
        }

        public void Log(string str)
        {
            日志.Text += "\n[" + DateTime.Now.ToLongTimeString().ToString() + "]: " + str;
            框.ScrollToVerticalOffset(框.ExtentHeight);
        }

        private void 标题_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed) { DragMove(); }
        }

        private void 关闭_Click(object sender, RoutedEventArgs e)
        {
            IObitController.DriverStop();  //释放驱动
            IObitController.DriverClose(); //释放资源
            Environment.Exit(0);           //关闭程序
        }

        private void 最小化_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        private void 关于_Click(object sender, RoutedEventArgs e)
        {
            BeginStoryboard((Storyboard)FindResource("打开"));
        }

        private void ToggleButton_Click(object sender, RoutedEventArgs e)
        {
            this.Topmost = (bool)前端显示.IsChecked;
        }

        private void 选择路径_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var dialog = new CommonOpenFileDialog();
                dialog.IsFolderPicker = true;
                CommonFileDialogResult result = dialog.ShowDialog();
                if (dialog.FileName != "")
                {
                    输出路径.Text = dialog.FileName;
                    Out_file = dialog.FileName;
                    日志.Text += "\n[" + DateTime.Now.ToLongTimeString().ToString() + "]: 重新定向输出文件夹：" + dialog.FileName;
                }
            }
            catch { }
        }

        private void 添加文件_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "全部文件|*.*";
            ofd.Multiselect = true;
            if (ofd.ShowDialog(this) == true)
            {
                foreach (string file in ofd.FileNames)
                {
                    if (App.Number_file.Contains(file) == false)//排除同类项
                    {
                        App.Number_file.Add(file);//将元素添加到数组末尾
                        文件列表.Items.Add(file);
                        日志.Text += "\n[" + DateTime.Now.ToLongTimeString().ToString() + "]: 已添加文件：" + file;
                        框.ScrollToVerticalOffset(框.ExtentHeight);
                    }
                    else
                    {
                        日志.Text += "\n[" + DateTime.Now.ToLongTimeString().ToString() + "]: 添加失败!原因：已存在此文件";
                    }
                }
            }
            文件下拉框.Header = "已选择" + App.Number_file.Count + "个对象";
        }

        private void 打开目录_Click(object sender, RoutedEventArgs e)
        {
            if(Out_file != string.Empty)
            {
                if (Directory.Exists(Out_file))                     //判断输出路径是否存在
                {
                    Delete_Class.RunCmd("explorer " + Out_file + @"\");
                }
                else                                            //如果不在重新创建
                {
                    Log("重新创建输出目录...");
                    Directory.CreateDirectory(Out_file);//创建新路径
                    Delete_Class.RunCmd("explorer " + Out_file + @"\");
                }
            }
            else
            {
                Log("您还没有选择输出目录！");
            }
        }

        private void 清空对象_Click(object sender, RoutedEventArgs e)
        {
            App.Number_file.Clear();
            文件列表.Items.Clear();
            文件下拉框.Header = "已选择0个对象";
            日志.Text += "\n[" + DateTime.Now.ToLongTimeString().ToString() + "]: 已清空文件列表";
        }

        private void 清除日志_Click(object sender, RoutedEventArgs e)
        {
            日志.Text = "[" + DateTime.Now.ToLongTimeString().ToString() + "]: 已经清空日志...";
        }

        private void RadioButton_Checked(object sender, RoutedEventArgs e)
        {
            输出路径.IsEnabled = false;
        }

        private void RadioButton_Checked_1(object sender, RoutedEventArgs e)
        {
            输出路径.IsEnabled = false;
        }

        private void RadioButton_Checked_2(object sender, RoutedEventArgs e)
        {
            输出路径.IsEnabled = true;
        }

        private void RadioButton_Checked_3(object sender, RoutedEventArgs e)
        {
            输出路径.IsEnabled = true;
        }

        private void 关闭关于_Click(object sender, RoutedEventArgs e)
        {
            BeginStoryboard((Storyboard)FindResource("关闭"));
        }

        private void 图标_MouseUp(object sender, MouseButtonEventArgs e)
        {
            Process.Start("https://github.com/xingchuanzhen/File-Unlock");
        }

        private void Window_DragEnter(object sender, DragEventArgs e)
        {
            拖入文件.Visibility = Visibility.Visible;
        }

        private void Window_DragLeave(object sender, DragEventArgs e)
        {
            拖入文件.Visibility = Visibility.Collapsed;
        }

        private void 拖入文件_Drop(object sender, DragEventArgs e)
        {
            拖入文件.Visibility = Visibility.Collapsed;
            string[] filePath = (string[])e.Data.GetData(DataFormats.FileDrop);
            for (int i = 0; i < filePath.Length; i++)
            {
                if (App.Number_file.Contains(filePath[i]) == false)//排除同类项
                {
                    App.Number_file.Add(filePath[i]);//将元素添加到数组末尾
                    文件列表.Items.Add(filePath[i]);
                    日志.Text += "\n[" + DateTime.Now.ToLongTimeString().ToString() + "]: 已添加文件：" + filePath[i];
                }
                else
                {
                    日志.Text += "\n[" + DateTime.Now.ToLongTimeString().ToString() + "]: 添加失败!原因：已存在此文件";
                }
            }
            文件下拉框.Header = "已选择" + App.Number_file.Count + "个对象";
        }
    }
}
