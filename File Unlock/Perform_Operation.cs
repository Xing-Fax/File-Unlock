using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace File_Unlock
{
    class Perform_Operation
    {

        private static void Log(string str)
        {
            new Thread(() =>//异步调用
            {
                Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Normal,
                    new Action(() =>
                    {
                        window.日志.Text += "\n[" + DateTime.Now.ToLongTimeString().ToString() + "]: " + str;
                        window.框.ScrollToVerticalOffset(window.框.ExtentHeight);
                    }));
            }).Start();

        }
        static ArrayList Number_file;
        static string Out_file;
        static MainWindow window;
        private static void Background_execution(object sender, DoWorkEventArgs e)
        {

            for (int i = 0;i < Number_file.Count;i ++)
            {
                if(File .Exists (Number_file[i].ToString()))
                {
                    bool Fail = false;                                              //决定是否解锁成功true为失败
                                                                                    //解锁文件操作
                    if (Delete_Class.Judgment_status(Number_file[i].ToString()))    //判断文件是否被锁定
                    {
                        if (Delete_Class.Unlock_file(Number_file[i].ToString()))    //进行解锁文件
                        {
                            Log("解锁文件：" + Path.GetFileName(Number_file[i].ToString()) + "   成功！");
                            Fail = true;
                        }
                        else
                        {
                            Log("解锁文件：" + Path.GetFileName(Number_file[i].ToString()) + "   失败！");
                        }
                    }
                    else
                    {
                        Log("文件：" + Path.GetFileName(Number_file[i].ToString()) + "   没有锁定！");
                        Fail = true;
                    }

                    if ((e.Argument.ToString() == "3" || e.Argument.ToString() == "4") && Fail == true)
                    {

                        if (Delete_Class.CopyFile(Number_file[i].ToString(), Out_file + @"\" + Path.GetFileName(Number_file[i].ToString())))
                        {
                            Log("文件：" + Path.GetFileName(Number_file[i].ToString()) + "   复制成功！");
                            Fail = false;
                        }
                        else
                        {
                            Log("文件：" + Path.GetFileName(Number_file[i].ToString()) + "   复制失败！");
                        }
                    }

                    //删除文件操作
                    if (e.Argument.ToString() == "2" || e.Argument.ToString() == "3" && Fail == false)
                    {
                        Delete_Class.RunCmd("del " + "\"" + Number_file[i].ToString() + "\"");

                        if (File .Exists(Number_file[i].ToString()))//判断文件是删除成功，否则采用强制删除
                        {
                            if (Delete_Class.Force_delete(Number_file[i].ToString()))
                            {
                                Log("文件：" + Path.GetFileName(Number_file[i].ToString()) + "   强制删除成功！");
                            }
                            else
                            {
                                Log("文件：" + Path.GetFileName(Number_file[i].ToString()) + "   强制删除失败！");
                            }
                        }
                        else
                        {
                            Log("文件：" + Path.GetFileName(Number_file[i].ToString()) + "   删除成功！");
                        }
                    }
                }
                else
                {
                    Log("文件：" + Path.GetFileName(Number_file[i].ToString()) + "   不存在！");
                }
            }

            Log("一共" + Number_file.Count.ToString() + " 个文件执行完毕！");
            new Thread(() =>//异步调用
            {
                Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Normal,
                    new Action(() =>
                    {
                        window.进度条.IsIndeterminate = false;
                    }));
            }).Start();
        }
           

        /// <summary>
        /// 开始执行操作
        /// </summary>
        /// <param name="Number_file">文件列表</param>
        /// <param name="Out_file">输出目录</param>
        /// <param name="main">窗体对象</param>
        public static void Operation(ArrayList Number_file0,string Out_file0,MainWindow main)
        {
            Number_file = Number_file0;
            Out_file = Out_file0;
            window = main;

            if((bool)main.操作3.IsChecked || (bool)main.操作4.IsChecked)
                if (!File.Exists(Out_file))             //判断输出路径是否存在
                {
                    Directory.CreateDirectory(Out_file);//创建新路径
                }
            int temp = 0;
            if ((bool)main.操作1.IsChecked)
                temp = 1;

            if ((bool)main.操作2.IsChecked)
                temp = 2;

            if ((bool)main.操作3.IsChecked)
                temp = 3;

            if ((bool)main.操作4.IsChecked)
                temp = 4;

            main.进度条.IsIndeterminate = true;
            using (BackgroundWorker bw = new BackgroundWorker())
            {
                bw.DoWork += new DoWorkEventHandler(Background_execution);
                bw.RunWorkerAsync(temp.ToString());
            }
        }
    }
}
