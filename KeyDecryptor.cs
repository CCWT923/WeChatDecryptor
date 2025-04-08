using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace WeChatDecryptor
{
    /// <summary>
    /// 获取微信的加密Key，仅用于3.x版本
    /// </summary>
    internal class KeyDecryptor
    {
        static byte?[] signature = new byte?[]
        {
            0x48, 0x8B, null, null, null, null, null, 0x48, 0x89
        };
        /// <summary>
        /// 从进程中读取
        /// </summary>
        /// <param name="processHandle"></param>
        /// <param name="address"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        byte[] ReadBytesFromProcess(IntPtr processHandle, IntPtr address, uint length)
        {
            byte[] buffer = new byte[length];
            WinApi.ReadProcessMemory(processHandle, address, buffer, length, out _);
            return buffer;
        }

        public static bool GetWeChatKey(IntPtr wechatHandle, out IntPtr keyAddress, out string key)
        {
            IntPtr curAddr = IntPtr.Zero;
            keyAddress = IntPtr.Zero;
            key = string.Empty;
            long maxAddress;
            if (Environment.Is64BitProcess)
            {
                maxAddress = 0x00007FFFFFFFFFFF; // 通常的 64 位用户空间上限
            }
            else
            {
                maxAddress = 0x7FFFFFFF; // 32 位最大地址
            }

            
            while ((long)curAddr < maxAddress)
            {
                WinApi.MEMORY_BASIC_INFORMATION mbi;
                if (WinApi.VirtualQueryEx(wechatHandle, curAddr, out mbi, (uint)Marshal.SizeOf(typeof(WinApi.MEMORY_BASIC_INFORMATION))) == 0)
                {
                    Trace.WriteLine($"VirtualQueryEx failed: {Marshal.GetLastWin32Error()}");
                    return false;
                }

                if (mbi.State == 0x1000 && (mbi.Protect & 0xF0) != 0)
                {
                    byte[] buffer = new byte[(int)mbi.RegionSize];
                    if (WinApi.ReadProcessMemory(wechatHandle, mbi.BaseAddress, buffer, (uint)buffer.Length, out _))
                    {
                        int offset = FindPattern(buffer, signature);
                        if (offset >= 0)
                        {
                            IntPtr keyAddr = mbi.BaseAddress + offset;
                            byte[] keyBuffer = new byte[32];
                            if (WinApi.ReadProcessMemory(wechatHandle, keyAddr, keyBuffer, 32, out _))
                            {
                                keyAddress = keyAddr;
                                key = BitConverter.ToString(keyBuffer).Replace("-", "");
                                Trace.WriteLine($"找到密钥！地址为：0x{keyAddr:X}，密钥：{BitConverter.ToString(keyBuffer).Replace("-", "")}");
                                return true;
                            }
                            break;
                        }
                    }
                }

                curAddr = new IntPtr((long)mbi.BaseAddress + (long)mbi.RegionSize);
            }
            return false;
        }

        static int FindPattern(byte[] buffer, byte?[] pattern)
        {
            for (int i = 0; i < buffer.Length - pattern.Length; i++)
            {
                bool matched = true;
                for (int j = 0; j < pattern.Length; j++)
                {
                    if (pattern[j].HasValue && buffer[i + j] != pattern[j])
                    {
                        matched = false;
                        break;
                    }
                }
                if (matched)
                    return i;
            }
            return -1;
        }

    }
}
