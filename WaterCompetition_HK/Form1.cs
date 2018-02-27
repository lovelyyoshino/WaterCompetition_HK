using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;

using System.Runtime.InteropServices;
namespace WaterCompetition_HK
{
    public partial class Form1 : Form
    {
        #region 各种初始定义

        private bool HkinitSDK = false;//初始化SDK
        /// <summary>
        /// m_lUserID:是否在登录
        /// m_lRealHandle：是否正在预览
        /// </summary>
        private Int32 m_lUserID = -1;
        private Int32 m_lRealHandle = -1;
        private uint iLastErr = 0;//序列号
        private string str;//错误信息
        private static bool state = false;
        #endregion

        public Form1()
        {
            InitializeComponent();
            HkinitSDK = CHCNetSDK.NET_DVR_Init();
            if (HkinitSDK == false)
            {
                MessageBox.Show("检查设备是否打开或者ip，用户名密码是否正确");
                return;
            }
            KeyHook.RegisterHotKey(Handle, 100, KeyHook.KeyModifiers.Shift, Keys.S);//注册快捷键
            KeyHook.RegisterHotKey(Handle, 101, KeyHook.KeyModifiers.Shift, Keys.V);//注册快捷键
            Thread WaterThread = new Thread(Start_View);//创建新线程
            WaterThread.Start();//开始
        }
        public void Start_View()//开始打开视频
        {
            btnLogin.Click += BtnLogin_Click;
            保存图片ToolStripMenuItem.Click += 保存图片ToolStripMenuItem_Click;
            控制摄像头ToolStripMenuItem.Click += 控制摄像头ToolStripMenuItem_Click;
            保存视频VToolStripMenuItem.Click += 保存视频VToolStripMenuItem_Click;
        }
        #region 视频保存
        private void 保存视频VToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Save_Video();
        }
        private void Save_Video()
        {
            string sVideoFileName = "Record_test.mp4";
            if (state == false)
            {
                if (!CHCNetSDK.NET_DVR_SaveRealData(m_lRealHandle, sVideoFileName))
                {
                    iLastErr = CHCNetSDK.NET_DVR_GetLastError();
                    str = "NET_DVR_SaveRealData failed, error code= " + iLastErr;
                    MessageBox.Show(str);
                    return;
                }
                else
                {
                    MessageBox.Show("开始录制");
                }
            }
            else
            {
                //停止录像 Stop recording
                if (!CHCNetSDK.NET_DVR_StopSaveRealData(m_lRealHandle))
                {
                    iLastErr = CHCNetSDK.NET_DVR_GetLastError();
                    str = "NET_DVR_StopSaveRealData failed, error code= " + iLastErr;
                    MessageBox.Show(str);
                    return;
                }
                else
                {
                    MessageBox.Show("结束录制");
                }
            }
        }
        #endregion
        #region 新窗体控制
        private void 控制摄像头ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Control control_hk=new Control();
            control_hk.m_lUserID = m_lUserID;
            control_hk.m_lChannel = 1;
            control_hk.m_lRealHandle = m_lRealHandle;
            control_hk.ShowDialog();
        }
        #endregion
        #region 快捷键检测
        //重写WndProc()方法，通过监视系统消息，来调用过程
        protected override void WndProc(ref Message m)
        {
            const int WM_HOTKEY = 0x0312;
            //按快捷键 
            switch (m.Msg)
            {
                case WM_HOTKEY:
                    switch (m.WParam.ToInt32())
                    {
                        case 100:    //按下的是Shift+S
                            SavePicture();
                            break;
                        case 101:    //按下的是Shift+V
                            Save_Video();
                            break;
                    }
                    break;
            }
            base.WndProc(ref m);
        }
        #endregion
        #region 保存图片
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void 保存图片ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SavePicture();
        }
        private void SavePicture()
        {
            
            string BMP_IMAGE = "BMP_test.bmp";
                if (CHCNetSDK.NET_DVR_CapturePicture(m_lRealHandle, BMP_IMAGE))
            {
                str = "保存路径为 " + BMP_IMAGE;
                MessageBox.Show(str);
            }
            else
            {
                iLastErr = CHCNetSDK.NET_DVR_GetLastError();//获取失败编码
                str = "NET_DVR_CapturePicture failed, error code= " + iLastErr;
                MessageBox.Show(str);
                return;
            }
        }
        #endregion
        #region 登录按钮
        private void BtnLogin_Click(object sender, EventArgs e)
        {
            if (textBoxIP.Text == "" || textBoxPort.Text == "" ||
                textBoxUserName.Text == "" || textBoxPassword.Text == "")
            {
                MessageBox.Show("请输入IP, 端口,用户名，密码!");
                return;
            }//查看是否补全
            if (m_lUserID < 0)//未进行登录
            {
                string DVRIPAddress = textBoxIP.Text; //设备IP地址或者域名
                Int16 DVRPortNumber = Int16.Parse(textBoxPort.Text);//设备服务端口号
                string DVRUserName = textBoxUserName.Text;//设备登录用户名
                string DVRPassword = textBoxPassword.Text;//设备登录密码

                CHCNetSDK.NET_DVR_DEVICEINFO_V30 DeviceInfo = new CHCNetSDK.NET_DVR_DEVICEINFO_V30();//设备

                //登录设备 Login the device
                m_lUserID = CHCNetSDK.NET_DVR_Login_V30(DVRIPAddress, DVRPortNumber, DVRUserName, DVRPassword, ref DeviceInfo);
                if (m_lUserID < 0)//尝试登陆失败
                {
                    iLastErr = CHCNetSDK.NET_DVR_GetLastError();
                    str = "NET_DVR_Login_V30 failed, error code= " + iLastErr; //登录失败，输出错误号
                    MessageBox.Show(str);
                    return;
                }
                else
                {
                    //登录成功
                    MessageBox.Show("Login Success!");
                    btnLogin.Text = "Logout";
                }

            }
            if (m_lRealHandle < 0)
            {
                CHCNetSDK.NET_DVR_PREVIEWINFO lpPreviewInfo = new CHCNetSDK.NET_DVR_PREVIEWINFO();
                lpPreviewInfo.hPlayWnd = RealPlayWnd.Handle;//预览窗口
                lpPreviewInfo.lChannel = Int16.Parse("1");//预te览的设备通道
                lpPreviewInfo.dwStreamType = 0;//码流类型：0-主码流，1-子码流，2-码流3，3-码流4，以此类推
                lpPreviewInfo.dwLinkMode = 0;//连接方式：0- TCP方式，1- UDP方式，2- 多播方式，3- RTP方式，4-RTP/RTSP，5-RSTP/HTTP 
                lpPreviewInfo.bBlocked = true; //0- 非阻塞取流，1- 阻塞取流
                lpPreviewInfo.dwDisplayBufNum = 15; //播放库播放缓冲区最大缓冲帧数

                //CHCNetSDK.REALDATACALLBACK RealData = new CHCNetSDK.REALDATACALLBACK(RealDataCallBack);//预览实时流回调函数
                IntPtr pUser = new IntPtr();//用户数据

                //打开预览
                m_lRealHandle = CHCNetSDK.NET_DVR_RealPlay_V40(m_lUserID, ref lpPreviewInfo, null/*RealData*/, pUser);
                if (m_lRealHandle < 0)//不能打开预览
                {
                    iLastErr = CHCNetSDK.NET_DVR_GetLastError();
                    str = "NET_DVR_RealPlay_V40 failed, error code= " + iLastErr; //预览失败，输出错误号
                    MessageBox.Show(str);
                    return;
                }
            }
        }
        #endregion
        #region 关闭清除数据
        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (m_lRealHandle >= 0)
            {
                CHCNetSDK.NET_DVR_StopRealPlay(m_lRealHandle);
            }
            if (m_lUserID >= 0)
            {
                CHCNetSDK.NET_DVR_Logout(m_lUserID);
            }
            if (HkinitSDK == true)
            {
                CHCNetSDK.NET_DVR_Cleanup();
            }
            //注销热键设定
            KeyHook.UnregisterHotKey(Handle, 100);
            KeyHook.UnregisterHotKey(Handle, 101);
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }
        #endregion
    }
}

