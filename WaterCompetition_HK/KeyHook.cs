using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.Reflection;
using System.Threading;
using System.Windows.Forms;
using System.ComponentModel;
using System.Diagnostics;

namespace WaterCompetition_HK
{
    class KeyHook
    {
        //如果函数执行成功，返回值不为0。
        //如果函数执行失败，返回值为0。要得到扩展错误信息，调用GetLastError。
        [DllImport("user32.dll", SetLastError = true)]
        public static extern bool RegisterHotKey(
            IntPtr hWnd,                //要定义热键的窗口的句柄
            int id,                     //定义热键ID（不能与其它ID重复）           
            KeyModifiers fsModifiers,   //标识热键是否在按Alt、Ctrl、Shift、Windows等键时才会生效
            Keys vk                     //定义热键的内容
            );
        [DllImport("user32.dll", SetLastError = true)]
        public static extern bool UnregisterHotKey(
            IntPtr hWnd,                //要取消热键的窗口的句柄
            int id                      //要取消热键的ID
            );
        //定义了辅助键的名称（将数字转变为字符以便于记忆，也可去除此枚举而直接使用数值）
        [Flags()]
        public enum KeyModifiers
        {
            None = 0,
            Alt = 1,
            Ctrl = 2,
            Shift = 4,
            WindowsKey = 8
        }
    /* //每个结构体或者类的存储顺序布局形式，还存在Explicit,精确布局
     [StructLayout(LayoutKind.Sequential)]
     private class POINT//位置
     {
         public int x;
         public int y;
     }

     [StructLayout(LayoutKind.Sequential)]
     private class MouseHookStruct//鼠标钩子
     {
         public POINT pt;
         public int hwnd;
         public int wHitTestCode;
         public int dwExtraInfo;
     }

     [StructLayout(LayoutKind.Sequential)]
     private class MouseLLHookStruct//鼠标钩子
     {
         public POINT pt;
         public int mouseData;
         public int flags;
         public int time;
         public int dwExtraInfo;
     }

     [StructLayout(LayoutKind.Sequential)]
     private class KeyboardHookStruct//键盘钩子
     {
         public int vkCode;
         public int scanCode;
         public int flags;
         public int time;
         public int dwExtraInfo;
     }

     [DllImport("user32.dll", CharSet = CharSet.Auto,
        CallingConvention = CallingConvention.StdCall, SetLastError = true)]//调用动态库
     private static extern int SetWindowsHookEx(
         int idHook,
         HookProc lpfn,
         IntPtr hMod,
         int dwThreadId);
     //这个函数是这个dll中的进行重定义
     [DllImport("user32.dll", CharSet = CharSet.Auto,
         CallingConvention = CallingConvention.StdCall, SetLastError = true)]
     private static extern int UnhookWindowsHookEx(int idHook);

     [DllImport("user32.dll", CharSet = CharSet.Auto,
          CallingConvention = CallingConvention.StdCall)]
     private static extern int CallNextHookEx(
         int idHook,
         int nCode,
         int wParam,
         IntPtr lParam);

     private delegate int HookProc(int nCode, int wParam, IntPtr lParam);

     [DllImport("kernel32.dll")]
     public static extern IntPtr GetModuleHandle(string name);

     [DllImport("user32")]
     private static extern int ToAscii(
         int uVirtKey,
         int uScanCode,
         byte[] lpbKeyState,
         byte[] lpwTransKey,
         int fuState);

     [DllImport("user32")]
     private static extern int GetKeyboardState(byte[] pbKeyState);

     [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
     private static extern short GetKeyState(int vKey);

     private const int WH_MOUSE_LL = 14;
     private const int WH_KEYBOARD_LL = 13;
     private const int WH_MOUSE = 7;
     private const int WH_KEYBOARD = 2;
     private const int WM_MOUSEMOVE = 0x200;
     private const int WM_LBUTTONDOWN = 0x201;
     private const int WM_RBUTTONDOWN = 0x204;
     private const int WM_MBUTTONDOWN = 0x207;
     private const int WM_LBUTTONUP = 0x202;
     private const int WM_RBUTTONUP = 0x205;
     private const int WM_MBUTTONUP = 0x208;
     private const int WM_LBUTTONDBLCLK = 0x203;
     private const int WM_RBUTTONDBLCLK = 0x206;
     private const int WM_MBUTTONDBLCLK = 0x209;
     private const int WM_MOUSEWHEEL = 0x020A;

     private const int WM_KEYDOWN = 0x100;
     private const int WM_KEYUP = 0x101;
     private const int WM_SYSKEYDOWN = 0x104;
     private const int WM_SYSKEYUP = 0x105;

     private const byte VK_SHIFT = 0x10;
     private const byte VK_CAPITAL = 0x14;
     private const byte VK_NUMLOCK = 0x90;

     public KeyHook()//函数调用
     {
         Start();
     }

     public KeyHook(bool InstallMouseHook, bool InstallKeyboardHook)
     {
         Start(InstallMouseHook, InstallKeyboardHook);
     }
     ~KeyHook()//析构函数
     {
         Stop(true, true, false);
     }

     public event MouseEventHandler OnMouseActivity;
     public event KeyEventHandler KeyDown;
     public event KeyPressEventHandler KeyPress;
     public event KeyEventHandler KeyUp;

     private int hMouseHook = 0;
     private int hKeyboardHook = 0;

     private static HookProc MouseHookProcedure;
     private static HookProc KeyboardHookProcedure;

     public void Start()//启动
     {
         this.Start(true, true);
     }

     public void Start(bool InstallMouseHook, bool InstallKeyboardHook)
     {
         if (hMouseHook == 0 && InstallMouseHook)//鼠标检测
         {
             MouseHookProcedure = new HookProc(MouseHookProc);
             hMouseHook = SetWindowsHookEx(
                 WH_MOUSE_LL,
                 MouseHookProcedure,
                 GetModuleHandle(Process.GetCurrentProcess().MainModule.ModuleName),//设置窗体钩子
                 0);
             if (hMouseHook == 0)//检测不到
             {
                 int errorCode = Marshal.GetLastWin32Error();
                 Stop(true, false, false);
                 throw new Win32Exception(errorCode);
             }
         }

         if (hKeyboardHook == 0 && InstallKeyboardHook)//键盘检测
         {
             KeyboardHookProcedure = new HookProc(KeyboardHookProc);
             hKeyboardHook = SetWindowsHookEx(
                 WH_KEYBOARD_LL,
                 KeyboardHookProcedure,
                 GetModuleHandle(Process.GetCurrentProcess().MainModule.ModuleName),
                 0);
             if (hKeyboardHook == 0)
             {
                 int errorCode = Marshal.GetLastWin32Error();
                 Stop(false, true, false);
                 throw new Win32Exception(errorCode);
             }
         }
     }

     public void Stop()
     {
         this.Stop(true, true, true);
     }

     public void Stop(bool UninstallMouseHook, bool UninstallKeyboardHook, bool ThrowExceptions)
     {
         if (hMouseHook != 0 && UninstallMouseHook)
         {
             int retMouse = UnhookWindowsHookEx(hMouseHook);
             hMouseHook = 0;
             if (retMouse == 0 && ThrowExceptions)
             {
                 int errorCode = Marshal.GetLastWin32Error();
                 throw new Win32Exception(errorCode);
             }
         }

         if (hKeyboardHook != 0 && UninstallKeyboardHook)
         {
             int retKeyboard = UnhookWindowsHookEx(hKeyboardHook);
             hKeyboardHook = 0;
             if (retKeyboard == 0 && ThrowExceptions)
             {
                 int errorCode = Marshal.GetLastWin32Error();
                 throw new Win32Exception(errorCode);
             }
         }
     }

     private int MouseHookProc(int nCode, int wParam, IntPtr lParam)
     {
         if ((nCode >= 0) && (OnMouseActivity != null))
         {
             MouseLLHookStruct mouseHookStruct = (MouseLLHookStruct)Marshal.PtrToStructure(lParam, typeof(MouseLLHookStruct));

             MouseButtons button = MouseButtons.None;
             short mouseDelta = 0;
             switch (wParam)
             {
                 case WM_LBUTTONDOWN:
                     button = MouseButtons.Left;
                     break;
                 case WM_RBUTTONDOWN:
                     button = MouseButtons.Right;
                     break;
                 case WM_MOUSEWHEEL:
                     mouseDelta = (short)((mouseHookStruct.mouseData >> 16) & 0xffff);
                     break;
             }

             int clickCount = 0;
             if (button != MouseButtons.None)
                 if (wParam == WM_LBUTTONDBLCLK || wParam == WM_RBUTTONDBLCLK) clickCount = 2;
                 else clickCount = 1;

             MouseEventArgs e = new MouseEventArgs(
                                                button,
                                                clickCount,
                                                mouseHookStruct.pt.x,
                                                mouseHookStruct.pt.y,
                                                mouseDelta);
             OnMouseActivity(this, e);//触发事件
         }
         return CallNextHookEx(hMouseHook, nCode, wParam, lParam);
     }

     private int KeyboardHookProc(int nCode, Int32 wParam, IntPtr lParam)
     {
         bool handled = false;
         if ((nCode >= 0) && (KeyDown != null || KeyUp != null || KeyPress != null))
         {
             KeyboardHookStruct MyKeyboardHookStruct = (KeyboardHookStruct)Marshal.PtrToStructure(lParam, typeof(KeyboardHookStruct));
             if (KeyDown != null && (wParam == WM_KEYDOWN || wParam == WM_SYSKEYDOWN))
             {
                 Keys keyData = (Keys)MyKeyboardHookStruct.vkCode;
                 KeyEventArgs e = new KeyEventArgs(keyData);
                 KeyDown(this, e);
                 handled = handled || e.Handled;
             }

             if (KeyPress != null && wParam == WM_KEYDOWN)
             {
                 bool isDownShift = ((GetKeyState(VK_SHIFT) & 0x80) == 0x80 ? true : false);
                 bool isDownCapslock = (GetKeyState(VK_CAPITAL) != 0 ? true : false);

                 byte[] keyState = new byte[256];
                 GetKeyboardState(keyState);
                 byte[] inBuffer = new byte[2];
                 if (ToAscii(MyKeyboardHookStruct.vkCode,
                           MyKeyboardHookStruct.scanCode,
                           keyState,
                           inBuffer,
                           MyKeyboardHookStruct.flags) == 1)
                 {
                     char key = (char)inBuffer[0];
                     if ((isDownCapslock ^ isDownShift) && Char.IsLetter(key)) key = Char.ToUpper(key);
                     KeyPressEventArgs e = new KeyPressEventArgs(key);
                     KeyPress(this, e);
                     handled = handled || e.Handled;
                 }
             }

             if (KeyUp != null && (wParam == WM_KEYUP || wParam == WM_SYSKEYUP))
             {
                 Keys keyData = (Keys)MyKeyboardHookStruct.vkCode;
                 KeyEventArgs e = new KeyEventArgs(keyData);
                 KeyUp(this, e);
                 handled = handled || e.Handled;
             }

         }

         if (handled)
             return 1;
         else
             return CallNextHookEx(hKeyboardHook, nCode, wParam, lParam);
     }*/
}
}
