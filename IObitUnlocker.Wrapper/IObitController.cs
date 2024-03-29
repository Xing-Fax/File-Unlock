﻿using System;
using System.IO;
using System.Runtime.InteropServices;

namespace IObitUnlocker.Wrapper
{
    public static class IObitController
    {
        static bool init = false;

        static string SysPath = Path.Combine(Directory.GetCurrentDirectory(), "IObitUnlocker.sys");

        static string DLLPath = Path.Combine(Directory.GetCurrentDirectory(), "IObitUnlocker.dll");

        public static void Init()
        {
            //写入文件
            File.WriteAllBytes(DLLPath, Properties.Resources.IObitUnlocker);

            File.WriteAllBytes(SysPath, Properties.Resources.IObitUnlockerSyS);

            //设置隐藏属性
            File.SetAttributes(DLLPath, FileAttributes.Hidden);

            File.SetAttributes(SysPath, FileAttributes.Hidden);

            init = true;
        }

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        delegate bool IObitDriverBase();

        [UnmanagedFunctionPointer(CallingConvention.StdCall, CharSet = CharSet.Unicode)]
        delegate int IObitDriverUnlockFile(string file, ref uint unk1, FileOperation deleteFile, int unk2, out int unk3);

        static int handle;                              //保存dll句柄
        public static bool DriverStart()
        {
            Init();
            handle = (int)Native.LoadLibraryEx(DLLPath, IntPtr.Zero, LoadLibraryFlags.None);
            var addr = Native.GetProcAddress((IntPtr)handle, "DriverStart");
            var DriverStart = Marshal.GetDelegateForFunctionPointer<IObitDriverBase>(addr);
            return DriverStart();
        }

        public static bool DriverStop()
        {
            var addr = Native.GetProcAddress((IntPtr)handle, "DriverStop");
            var DriverStop = Marshal.GetDelegateForFunctionPointer<IObitDriverBase>(addr);
            return DriverStop();
        }

        public static void DriverClose()
        {
            while (Native.FreeLibrary(handle) != 0);     //通过句柄释放dll
                                                         //删除文件
            if (File.Exists(SysPath))
                File.Delete(SysPath);
            if (File.Exists(DLLPath))
                File.Delete(DLLPath);
        }

        public static int UnlockFile(string file, FileOperation operation)
        {
            if (!init)
            {
                Init();
            }
            var dll = Native.LoadLibraryEx(DLLPath, IntPtr.Zero, LoadLibraryFlags.None);
            var addr = Native.GetProcAddress(dll, "DriverUnlockFile");
            var DriverUnlockFile = Marshal.GetDelegateForFunctionPointer<IObitDriverUnlockFile>(addr);
            uint flag = 0xc08b0000;
            return DriverUnlockFile(file, ref flag, operation, 0, out int unk3);
        }
    }
}
