using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace WeChatDecryptor
{
    internal class WinApi
    {
        /// <summary>
        /// 需要使用 ReadProcessMemory 读取进程中的内存。
        /// </summary>
        public const int PROCESS_VM_READ = 0x0010;
        /// <summary>
        /// 检索有关进程的某些信息
        /// </summary>
        public const int PROCESS_QUERY_INFORMATION = 0x0400;
        /// <summary>
        /// 打开进程对象
        /// </summary>
        /// <param name="dwDesiredAccess"></param>
        /// <param name="bInheritHandle"></param>
        /// <param name="dwProcessId"></param>
        /// <returns></returns>
        /// <see cref="https://learn.microsoft.com/zh-cn/windows/win32/api/processthreadsapi/nf-processthreadsapi-openprocess"/>
        [DllImport("kernel32.dll")]
        public static extern IntPtr OpenProcess(uint dwDesiredAccess, bool bInheritHandle, int dwProcessId);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="hProcess"></param>
        /// <param name="lpBaseAddress"></param>
        /// <param name="lpBuffer"></param>
        /// <param name="dwSize"></param>
        /// <param name="lpNumberOfBytesRead"></param>
        /// <returns></returns>
        /// <see cref="https://learn.microsoft.com/zh-cn/windows/win32/api/memoryapi/nf-memoryapi-readprocessmemory"/>
        [DllImport("kernel32.dll")]
        public static extern bool ReadProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, byte[] lpBuffer, uint dwSize, out IntPtr lpNumberOfBytesRead);

        /// <summary>
        /// 检索有关指定进程的虚拟地址空间中的页范围的信息
        /// <see cref="https://learn.microsoft.com/zh-cn/windows/win32/api/memoryapi/nf-memoryapi-virtualqueryex"/>
        /// </summary>
        /// <param name="hProcess"></param>
        /// <param name="lpAddress"></param>
        /// <param name="lpBuffer"></param>
        /// <param name="dwLength"></param>
        /// <returns></returns>
        [DllImport("kernel32.dll")]
        public static extern int VirtualQueryEx(IntPtr hProcess, IntPtr lpAddress, out MEMORY_BASIC_INFORMATION lpBuffer, uint dwLength);


        /// <summary>
        /// 关闭句柄
        /// </summary>
        /// <param name="hObject"></param>
        /// <returns></returns>
        [DllImport("kernel32.dll")]
        public static extern bool CloseHandle(IntPtr hObject);

        /// <summary>
        /// 包含有关进程虚拟地址空间中的页面范围信息的结构体
        /// <see cref="https://learn.microsoft.com/zh-cn/windows/win32/api/winnt/ns-winnt-memory_basic_information"/>
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        public struct MEMORY_BASIC_INFORMATION
        {
            public IntPtr BaseAddress;
            public IntPtr AllocationBase;
            public uint AllocationProtect;
            public IntPtr RegionSize;
            public uint State;
            public uint Protect;
            public uint Type;
        }
    }
}
