using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WaterCompetition_HK
{
    public partial class Control : Form
    {
        #region 各类声明调用
        public int m_lUserID = -1;
        public int m_lRealHandle = -1;
        public int m_lChannel = -1;
        private bool bAuto = false;
        #endregion
        public Control()
        {
            InitializeComponent();
            comboBoxSpeed.SelectedIndex = 3;
        }
        private void btnLeft_MouseDown(object sender, MouseEventArgs e)
        {
                CHCNetSDK.NET_DVR_PTZControlWithSpeed(m_lRealHandle, CHCNetSDK.PAN_LEFT, 0, (uint)comboBoxSpeed.SelectedIndex + 1);
        }

        private void btnLeft_MouseUp(object sender, MouseEventArgs e)
        {
                CHCNetSDK.NET_DVR_PTZControlWithSpeed(m_lRealHandle, CHCNetSDK.PAN_LEFT, 1, (uint)comboBoxSpeed.SelectedIndex + 1);
        }

        private void btnUp_MouseDown(object sender, MouseEventArgs e)
        {
                CHCNetSDK.NET_DVR_PTZControlWithSpeed(m_lRealHandle, CHCNetSDK.TILT_UP, 0, (uint)comboBoxSpeed.SelectedIndex + 1);
        }

        private void btnUp_MouseUp(object sender, MouseEventArgs e)
        {
                CHCNetSDK.NET_DVR_PTZControlWithSpeed(m_lRealHandle, CHCNetSDK.TILT_UP, 1, (uint)comboBoxSpeed.SelectedIndex + 1);
        }

        private void btnRight_MouseDown(object sender, MouseEventArgs e)
        {
                CHCNetSDK.NET_DVR_PTZControlWithSpeed(m_lRealHandle, CHCNetSDK.PAN_RIGHT, 0, (uint)comboBoxSpeed.SelectedIndex + 1);
        }

        private void btnRight_MouseUp(object sender, MouseEventArgs e)
        {
                CHCNetSDK.NET_DVR_PTZControlWithSpeed(m_lRealHandle, CHCNetSDK.PAN_RIGHT, 1, (uint)comboBoxSpeed.SelectedIndex + 1);
        }

        private void btnDown_MouseDown(object sender, MouseEventArgs e)
        {
                CHCNetSDK.NET_DVR_PTZControlWithSpeed(m_lRealHandle, CHCNetSDK.TILT_DOWN, 0, (uint)comboBoxSpeed.SelectedIndex + 1);
        }

        private void btnDown_MouseUp(object sender, MouseEventArgs e)
        {
                CHCNetSDK.NET_DVR_PTZControlWithSpeed(m_lRealHandle, CHCNetSDK.TILT_DOWN, 1, (uint)comboBoxSpeed.SelectedIndex + 1);
        }
        private void btnAuto_Click(object sender, EventArgs e)
        {
            if (!bAuto)
            {
                CHCNetSDK.NET_DVR_PTZControlWithSpeed(m_lRealHandle, CHCNetSDK.PAN_AUTO, 0, (uint)comboBoxSpeed.SelectedIndex + 1);
                btnAuto.Text = "Stop";
                bAuto = true;
            }
            else
            {
                CHCNetSDK.NET_DVR_PTZControlWithSpeed(m_lRealHandle, CHCNetSDK.PAN_AUTO, 1, (uint)comboBoxSpeed.SelectedIndex + 1);
                btnAuto.Text = "Auto";
                bAuto = false;
            }
        }
    }
}
