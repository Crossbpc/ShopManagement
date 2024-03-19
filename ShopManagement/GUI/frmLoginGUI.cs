using Coffee.BUS;
using Coffee.DTO;
using System;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace ShopManagement.GUI
{
    public partial class frmLoginGUI : Form
    {
        public frmLoginGUI()
        {
            InitializeComponent();
            SetButtunlogin();
        }

        #region Event

        private void frmLoginGUI_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (MessageBox.Show("Bạn chắc chắn muốn thoát?", "Thông báo", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
                e.Cancel = true;
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        private void btnLogin_Click(object sender, EventArgs e)
        {
            if (CheckLoginLogic(tbxUser.Text, tbxPassword.Text))
            {
                frmManagerGUI manager = new frmManagerGUI(account);
                manager.Text = "Manager - " + account.NickName;
                this.Hide();
                manager.ShowDialog();
                this.Show();
            }
        }
        private void tbxUser_TextChanged(object sender, EventArgs e)
        {
            SetButtunlogin();
        }

        private void tbxPassword_TextChanged(object sender, EventArgs e)
        {
            SetButtunlogin();
        }
        private void btnRegister_Click(object sender, EventArgs e)
        {
            frmRegisterGUI register = new frmRegisterGUI();
            this.Hide();
            register.ShowDialog();
            this.Show();
        }

        #region Event Drag and move form
        public const int WM_NCLBUTTONDOWN = 0xA1;
        public const int HTCAPTION = 0x2;
        [DllImport("User32.dll")]
        public static extern bool ReleaseCapture();
        [DllImport("User32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);
        #endregion
        private void tableLayoutPanel1_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                ReleaseCapture();
                SendMessage(Handle, WM_NCLBUTTONDOWN, HTCAPTION, 0);
            }
        }

        #endregion


        #region Method

        bool CheckLoginLogic(string username, string password)
        {
            var obj = AccountBUS.Instances.GetAccount(username, password);
            if (obj is AccountDTO)
            {
                account = (AccountDTO)obj;
                return true;
            }
            else
            {
                if (obj is AccountError.WrongPassword)
                {
                    MessageBox.Show("Sai mật khẩu, vui lòng nhập lại !", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    tbxPassword.Clear();
                    tbxPassword.Focus();
                    return false;
                }
                else if (obj is AccountError.AccountNotFount)
                {
                    MessageBox.Show("Tài khoản không tồn tại, vui lòng nhập lại !", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }
                else
                {
                    MessageBox.Show("Lỗi chưa xác định, vui lòng liên hệ admin: Cross - 8165 !", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }
            }
        }
        void SetButtunlogin()
        {
            if (tbxUser.Text.Count() > 2 && tbxPassword.Text.Count() > 2)
                btnLogin.Enabled = true;
            else
                btnLogin.Enabled = false;
        }
        private AccountDTO account;


        #endregion
    }
}
