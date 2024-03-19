using Coffee.BUS;
using Coffee.DTO;
using System;
using System.Linq;
using System.Windows.Forms;

namespace ShopManagement.GUI
{
    public partial class frmAccountProfileGUI : Form
    {
        private AccountDTO _account;
        public frmAccountProfileGUI(AccountDTO account)
        {
            InitializeComponent();
            _account = account;
            AccountUpToFormLoad();
            SetUpdateButton();
        }

        #region Mothods
        void AccountUpToFormLoad()
        {
            tbxUser.Text = _account.Name;
            tbxAlias.Text = _account.NickName;
            tbxAlias.Focus();
        }
        void SetUpdateButton()
        {
            if (tbxPass.Text.Count() > 2)
                btnUpdate.Enabled = true;
            else
                btnUpdate.Enabled = false;
        }
        #endregion

        #region Events
        private void btnUpdate_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(tbxPass.Text))
                MessageBox.Show("Mật khẩu Không thể bỏ trống, thử lại !", "Thông báo lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            if (string.Compare(tbxReNewPass.Text, tbxNewpass.Text) != 0)
                MessageBox.Show("Mật khẩu mới không trùng khớp nhau, thử lại !", "Thông báo lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            else
            {
                var resurt = AccountBUS.Instances.EditAccountProFile(tbxUser.Text, tbxPass.Text, tbxNewpass.Text, tbxAlias.Text);
                switch (resurt)
                {
                    case 1:
                        {
                            var pass = tbxNewpass.Text == "" ? tbxPass.Text : tbxNewpass.Text;
                            MessageBox.Show("Cập nhật thành công", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            accountBackEvent?.Invoke(this, new AccountfromProfile((AccountDTO)AccountBUS.Instances.GetAccount(tbxUser.Text, pass)));
                            tbxPass.Clear();
                            tbxNewpass.Clear();
                            tbxReNewPass.Clear();
                            tbxAlias.Focus();
                            break;
                        }
                    default:
                        MessageBox.Show("Sai mật khẩu, thử lại !", "Thông báo lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        break;
                }
            }
        }
        private void tbxPass_TextChanged(object sender, EventArgs e)
        {
            SetUpdateButton();
        }
        private void tbxNewpass_TextChanged(object sender, EventArgs e)
        {
            SetUpdateButton();
        }
        private void tbxReNewPass_TextChanged(object sender, EventArgs e)
        {
            SetUpdateButton();
        }
        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        private event EventHandler<AccountfromProfile> accountBackEvent;
        public event EventHandler<AccountfromProfile> AccountBackEvent
        {
            add { accountBackEvent += value; }
            remove { accountBackEvent -= value; }
        }
        #endregion
    }
    public class AccountfromProfile
    {
        public AccountfromProfile(AccountDTO account)
        {
            this.Account = account;
        }
        private AccountDTO account;

        public AccountDTO Account { get => account; set => account = value; }
    }
}
