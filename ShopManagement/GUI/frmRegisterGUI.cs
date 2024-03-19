using Coffee.BUS;
using Coffee.DTO;
using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace ShopManagement.GUI
{
    public partial class frmRegisterGUI : Form
    {
        public frmRegisterGUI()
        {
            InitializeComponent();
            SetButton();
        }

        #region Events
        private void tbxUser_TextChanged(object sender, EventArgs e)
        {
            SetButton();
        }
        private void tbxPassword_TextChanged(object sender, EventArgs e)
        {
            SetButton();
        }
        private void tbxRePassword_TextChanged(object sender, EventArgs e)
        {
            SetButton();
        }
        private void btnRegister_Click(object sender, EventArgs e)
        {
            var username = tbxUser.Text;
            var password = tbxPassword.Text;
            var nickname = tbxNickname.Text;
            var checkpassword = password == tbxRePassword.Text;
            if (checkpassword == false)
            {
                MessageBox.Show("Mật khẩu không trùng khớp !!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                tbxPassword.Clear();
                tbxRePassword.Clear();
                tbxPassword.Focus();
                return;
            }
            var checkaccount = AccountBUS.Instances.GetAccount(username, password);
            if ((AccountError)checkaccount == AccountError.AccountNotFount)
            {
                if (AccountBUS.Instances.RegisterAccount(username, password, nickname) == 1)
                {
                    this.Close();
                    MessageBox.Show("Đăng ký thành công !!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            else
                MessageBox.Show("Đăng ký thất bại, tài khoản đã tồn tại, vui lòng thử lại !!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
        private void tbxUser_Click(object sender, EventArgs e)
        {
            tbxUser.Clear();
        }
        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        #endregion

        #region Method
        void SetButton()
        {
            if (tbxUser.Text.Count() > 2 && tbxPassword.Text.Count() > 2 && tbxRePassword.Text.Count() > 2)
                btnRegister.Enabled = true;
        }
        #endregion
    }
}
