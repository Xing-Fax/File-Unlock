using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using IObitUnlocker.Wrapper;

namespace File_Unlock
{
    class Delete_Class
    {
        ///// <summary>
        ///// 释放到嵌入程序中的资源文件
        ///// </summary>
        ///// <param name="byDll">资源文件字节组</param>
        ///// <param name="file">释放文件路径</param>
        ///// <returns>文件是否释放成功</returns>
        //public static bool file(byte[] byDll, string file)
        //{
        //    using (FileStream fs = new FileStream(file, FileMode.Create))//开始写入文件流
        //        fs.Write(byDll, 0, byDll.Length);
        //    if (File.Exists(file))//检索文件是否正确被释放
        //        return true;
        //    else
        //        return false;
        //}

        ///// <summary>
        ///// 执行释放文件
        ///// </summary>
        ///// <param name="temp">释放文件路径</param>
        ///// <returns>释放全部成功释放</returns>
        //public static bool File_List(string temp)
        //{
        //    if (file(Properties.Resources.unlocker, temp + @"\File unlock library.exe"))
        //        return true;
        //    else
        //        return false;
        //}

        ///// <summary>
        ///// 调用其他控制台程序
        ///// </summary>
        ///// <param name="file">程序路径</param>
        ///// <param name="command">命令参数</param>
        ///// <returns>返回执行后输出结果</returns>
        //public static string Console(string file, string command)
        //{
        //    Process p = new Process();
        //    p.StartInfo.FileName = file;                //确定程序名
        //    p.StartInfo.Arguments = command;            //确定程式命令行
        //    p.StartInfo.UseShellExecute = false;        //Shell的使用
        //    p.StartInfo.RedirectStandardInput = true;   //重定向输入
        //    p.StartInfo.RedirectStandardOutput = true;  //重定向输出
        //    p.StartInfo.RedirectStandardError = true;   //重定向输出错误
        //    p.StartInfo.CreateNoWindow = true;          //设置置不显示示窗口
        //    p.Start();
        //    return p.StandardOutput.ReadToEnd();        //输出出流取得命令行结果果
        //}

        /// <summary>
        /// 执行CMD命令
        /// </summary>
        /// <param name="command">命令内容</param>
        /// <returns>返回执行后的输出结果</returns>
        public static string RunCmd(string command)
        {
            Process p = new Process();
            p.StartInfo.FileName = "cmd.exe";           //确定程序名
            p.StartInfo.Arguments = "/c " + command;    //确定程式命令行
            p.StartInfo.UseShellExecute = false;        //Shell的使用
            p.StartInfo.RedirectStandardInput = true;   //重定向输入
            p.StartInfo.RedirectStandardOutput = true;  //重定向输出
            p.StartInfo.RedirectStandardError = true;   //重定向输出错误
            p.StartInfo.CreateNoWindow = true;          //设置置不显示示窗口
            p.Start();
            return p.StandardOutput.ReadToEnd();        //输出出流取得命令行结果果
        }

        /// <summary>
        /// 通过更改所有权方式强制删除文件
        /// </summary>
        /// <param name="path">要删除的文件路径</param>
        /// <returns>是否删除成功</returns>
        public static bool Force_delete(string path)
        {
            RunCmd("takeown /F " + "\"" + path + "\"" + " /A");                     //将文件所有者设为管理员组
            RunCmd("icacls " + "\"" + path + "\"" + " /grant Administrators:F");    //取得文件所有控制权
            RunCmd("del " + "\"" + path + "\"");                                    //删除文件
            if (!File.Exists(path))                                                 //判断是否删除成功
                return true;
            return false;
        }

        /// <summary>
        /// 对文件进行解锁操作
        /// </summary>
        /// <param name="path"></param>
        /// <returns>是否执行成功</returns>
        public static bool Unlock_file(string Path_file)
        {
            try
            {
                //string temp = Environment.GetEnvironmentVariable("TMP"); //得到用户临时文件夹路径

                var file = Path.Combine(Directory.GetCurrentDirectory(), Path_file);

                IObitController.UnlockFile(file, FileOperation.Unlock);

                //Console(temp + @"\File unlock library.exe", "\"" + path + "\"");    //调用库文件进行解锁操作
                return true;
            }
            catch                                                        //遇到错误返回失败
            {
                return false;
            }
        }

        /// <summary>
        /// 读取流方式来复制文件
        /// </summary>
        /// <param name="soucrePath">原始文件路径</param>
        /// <param name="targetPath">复制目标文件路径</param>
        /// <returns>true复制成功,false复制失败</returns>
        public static bool CopyFile(string soucrePath, string targetPath)
        {
            try
            {
                //读取复制文件流
                using (FileStream fsRead = new FileStream(soucrePath, FileMode.Open, FileAccess.Read))
                {
                    //写入文件复制流
                    using (FileStream fsWrite = new FileStream(targetPath, FileMode.OpenOrCreate, FileAccess.Write))
                    {
                        byte[] buffer = new byte[1024 * 1024 * 2];                  //每次读取2M
                       
                        while (true)                                                //可能文件比较大，要循环读取，每次读取2M
                        {
                            int n = fsRead.Read(buffer, 0, buffer.Count());         //每次读取的数据    n：是每次读取到的实际数据大小
                            if (n == 0)                                             //如果n=0说明读取的数据为空，已经读取到最后了，跳出循环
                                break;
                            fsWrite.Write(buffer, 0, n);                            //写入每次读取的实际数据大小
                        }
                    }
                }
                return true;
            }
            catch
            {
                return false;
            }
        }

        [DllImport("kernel32.dll")]
        public static extern IntPtr _lopen(string lpPathName, int iReadWrite);

        [DllImport("kernel32.dll")]
        public static extern bool CloseHandle(IntPtr hObject);

        public const int OF_READWRITE = 2;
        public const int OF_SHARE_DENY_NONE = 0x40;
        public static  readonly IntPtr HFILE_ERROR = new IntPtr(-1);

        /// <summary>
        /// 判断文件是否被其他程序占用
        /// </summary>
        /// <param name="path"></param>
        /// <returns>true为占用,false为没有占用</returns>
        public static bool Judgment_status(string path)
        {
            IntPtr vHandle = _lopen(path, OF_READWRITE | OF_SHARE_DENY_NONE);
            if (vHandle == HFILE_ERROR)
            {   //占用
                return true;
            }
            CloseHandle(vHandle);
                //没有占用
            return false;
        }
    }
}
