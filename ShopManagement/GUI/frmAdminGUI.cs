using Coffee.BUS;
using Coffee.DTO;
using System;
using System.Collections.Generic;
using System.Data;
using System.Windows.Forms;

namespace ShopManagement.GUI
{
    public partial class frmAdminGUI : Form
    {
        public frmAdminGUI()
        {
            InitializeComponent();
            StatisticalAMounth();
            FoodShow();
            BindingFoodData();
            AccountLoad();
            BindingAccountData();
            GetTableList();
            GetCategoryList();
        }
        #region Methods

        void FoodShow() => dtgrvFoodShow.DataSource = FoodBUS.Instance.GetFoodList();
        void BindingFoodData()
        {
            if (dtgrvFoodShow.SelectedRows.Count > 0)
            {
                tbxFoodId.DataBindings.Clear();
                tbxFoodName.DataBindings.Clear();
                tbxPrice.DataBindings.Clear();
                tbxFoodId.DataBindings.Add(new Binding("Text", dtgrvFoodShow.DataSource, "Mã món"));
                tbxFoodName.DataBindings.Add(new Binding("Text", dtgrvFoodShow.DataSource, "Tên món"));
                tbxPrice.DataBindings.Add(new Binding("Text", dtgrvFoodShow.DataSource, "Giá"));
                var categorylist = CatergoryBUS.Instance.GetcategoryList();
                cbxCatergory.DataSource = categorylist;
                var selectedcategory = dtgrvFoodShow.SelectedCells[0].OwningRow.Cells["Danh mục"].Value;
                foreach (CategoryDTO item in categorylist)
                {
                    if (string.Compare(selectedcategory.ToString(), item.Name) == 0)
                        cbxCatergory.SelectedItem = item;

                }
            }
            cbxCatergory.DisplayMember = "Name";
        }
        void BindingTableData()
        {
            if(dtgrvTable.SelectedRows.Count > 0)
            {
                tbxTableId.DataBindings.Clear();
                tbxTableName.DataBindings.Clear();
                cbxTableStatus.DataBindings.Clear();
                tbxTableId.DataBindings.Add(new Binding("Text", dtgrvTable.DataSource, "Id", true, DataSourceUpdateMode.Never));
                tbxTableName.DataBindings.Add(new Binding("Text", dtgrvTable.DataSource, "Name", true, DataSourceUpdateMode.Never));
                cbxTableStatus.DataSource = new List<string> {"Trống", "Có người"};
                if (string.Compare(dtgrvTable.SelectedCells[0].OwningRow.Cells[2].Value.ToString(), "Trống") == 0)
                    cbxTableStatus.SelectedIndex = 0;
                else
                    cbxTableStatus.SelectedIndex = 1;
                cbxTableStatus.DisplayMember = "Status";
            }
        }
        void BindingCategoryData()
        {
            if (dtgrvCategory.SelectedRows.Count > 0)
            {
                tbxCategoryId.DataBindings.Clear();
                tbxCategoryName.DataBindings.Clear();
                tbxCategoryId.DataBindings.Add(new Binding("Text", dtgrvCategory.DataSource, "Id", true, DataSourceUpdateMode.Never));
                tbxCategoryName.DataBindings.Add(new Binding("Text", dtgrvCategory.DataSource, "Name", true, DataSourceUpdateMode.Never));
                cbxTableStatus.DisplayMember = "Status";
            }
        }
        void AccountLoad()
        {
            dtgrvAccount.DataSource = AccountBUS.Instances.GetAccountList();
        }
        void BindingAccountData()
        {

            if (dtgrvAccount.SelectedRows.Count > 0)
            {
                tbxName.DataBindings.Clear();
                tbxNickname.DataBindings.Clear();
                tbxName.DataBindings.Add(new Binding("Text", dtgrvAccount.DataSource, "Tên"));
                tbxNickname.DataBindings.Add(new Binding("Text", dtgrvAccount.DataSource, "Biệt danh"));
                List<string> type = new List<string>() { "User", "Admin" };
                cbxType.DataSource = type;
                var selectedaccount = dtgrvAccount.SelectedCells[0].OwningRow.Cells["Loại tài khoản"].Value;
                switch (selectedaccount)
                {
                    case true: cbxType.SelectedIndex = 1; break;
                    default: cbxType.SelectedIndex = 0; break;
                }
            }
            cbxCatergory.DisplayMember = "status";
        }
        void StatisticalAMounth()
        {
            DateTime now = DateTime.Now;
            dtpkFrom.Value = new DateTime(now.Year, now.Month, 1);
            dtpkTo.Value = dtpkFrom.Value.AddMonths(1).AddDays(-1);
            var datefirst = dtpkFrom.Value.ToString("yyyy-MM-dd 00:00:00.000");
            var datelast = dtpkTo.Value.ToString("yyyy-MM-dd 23:59:59.999");
            dtgrvStatisticalShow.DataSource = GetStatisticalData(datefirst, datelast);
        }
        DataTable GetStatisticalData(string datein, string dateout) => BillBUS.Instances.GetstatisticalData(datein, dateout);
        void GetTableList() => dtgrvTable.DataSource = TableBUS.Instance.GetTableList();
        void GetCategoryList() => dtgrvCategory.DataSource = CatergoryBUS.Instance.GetcategoryList();

        #endregion

        #region Events
        public AccountDTO account;
        public event EventHandler frmClosed;
        private event EventHandler insertevent;
        public event EventHandler Insertevent
        {
            add { insertevent += value; }
            remove { insertevent -= value; }
        }
        private event EventHandler deleteevent;
        public event EventHandler Deleteevent
        {
            add { deleteevent += value; }
            remove { deleteevent -= value; }
        }
        private event EventHandler editevent;
        public event EventHandler Editevent
        {
            add { editevent += value; }
            remove { editevent -= value; }
        }

        private void btnStatistical_Click(object sender, EventArgs e)
        {
            var datein = dtpkFrom.Value.ToString("yyyy-MM-dd 00:00:00.000");
            var dateout = dtpkTo.Value.ToString("yyyy-MM-dd 23:59:59.999");
            dtgrvStatisticalShow.DataSource = GetStatisticalData(datein, dateout);
        }
        private void btnView_Click(object sender, EventArgs e)
        {
            FoodShow();
        }
        private void dtgrvFoodShow_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            BindingFoodData();
        }
        private void btnAdd_Click(object sender, EventArgs e)
        {
            int categoryid = (cbxCatergory.SelectedItem as CategoryDTO).Id;
            string name = tbxFoodName.Text;
            int price = Convert.ToInt32(tbxPrice.Text);
            var result = FoodBUS.Instance.AddFood(categoryid, name, price);
            if (result == 0)
                MessageBox.Show("Thêm món thất bại, liên hệ admin Cross - 8165", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
            else
            {
                MessageBox.Show("Thêm món thành công", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                FoodShow();
                insertevent?.Invoke(this, new EventArgs());
            }
        }
        private void btnDel_Click(object sender, EventArgs e)
        {
            int foodid = Convert.ToInt32(tbxFoodId.Text);
            BillInforBUS.Instances.DelBillInfor(foodid);
            var resultf = FoodBUS.Instance.DelFood(foodid);
            if (resultf != 0)
            {
                MessageBox.Show("Xóa món thành công", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                FoodShow();
                deleteevent?.Invoke(this, new EventArgs());
            }
            else
                MessageBox.Show("Xóa món thất bại, liên hệ admin Cross - 8165", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
        private void btnEdit_Click(object sender, EventArgs e)
        {
            int categoryid = (cbxCatergory.SelectedItem as CategoryDTO).Id;
            string name = tbxFoodName.Text;
            int price = Convert.ToInt32(tbxPrice.Text);
            int foodid = Convert.ToInt32(tbxFoodId.Text);
            var result = FoodBUS.Instance.EditFood(categoryid, name, price, foodid);
            if (result == 0)
                MessageBox.Show("Cập nhật thất bại, liên hệ admin Cross - 8165", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
            else
            {
                MessageBox.Show("Cập nhật thành công", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                FoodShow();
                editevent?.Invoke(this, new EventArgs());
            }
        }
        private void btnSearch_Click(object sender, EventArgs e)
        {
            dtgrvFoodShow.DataSource = FoodBUS.Instance.GetFoodlistByName(tbxSearch.Text);
            dtgrvFoodShow.CellClick -= dtgrvFoodShow_CellClick;
        }
        private void dtgrvAccount_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            BindingAccountData();
        }
        private void btnViewTaikhoan_Click(object sender, EventArgs e)
        {
            AccountLoad();
        }
        private void btnAddTaikhoan_Click(object sender, EventArgs e)
        {
            var type = Convert.ToByte(cbxType.SelectedItem.ToString() == "Admin" ? 1 : 0);
            var result = AccountBUS.Instances.AddAccount(tbxName.Text, tbxNickname.Text, type);
            if (result == 0)
                MessageBox.Show("Thêm thành viên thất bại, liên hệ admin Cross - 8165", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
            else
            {
                MessageBox.Show("Thêm thành viên thành công", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                AccountLoad();
            }
        }
        private void btnEditTaikhoan_Click(object sender, EventArgs e)
        {
            var type = Convert.ToByte(cbxType.SelectedItem.ToString() == "Admin" ? 1 : 0);
            var result = AccountBUS.Instances.EditAccount(tbxName.Text, tbxNickname.Text, type);
            if (result == 0)
                MessageBox.Show("Cập nhật thành viên thất bại, liên hệ admin Cross - 8165", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
            else
            {
                MessageBox.Show("Cập nhật viên thành công", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                AccountLoad();
            }
        }
        private void btnDelTaikhoan_Click(object sender, EventArgs e)
        {
            if (account.Name.Equals(tbxName.Text))
            {
                MessageBox.Show("Bạn không thể xóa chính mình", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (MessageBox.Show("Bạn đang xóa thành viên, đúng chứ ?", "Thông báo", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                var result = AccountBUS.Instances.DelAccount(tbxName.Text);
                if (result == 0)
                    MessageBox.Show("Xóa thành viên thất bại, liên hệ admin Cross - 8165", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                else
                {
                    MessageBox.Show("Xóa thành viên thành công", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    AccountLoad();
                }
            }

        }
        private void btnResetPassTaikhoan_Click(object sender, EventArgs e)
        {
            var result = AccountBUS.Instances.ResetPassword(tbxName.Text);
            if (result == 0)
                MessageBox.Show("Đặt lại mật khẩu thất bại, liên hệ admin Cross - 8165", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
            else
                MessageBox.Show("Đặt lại mật khẩu thành công", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        private void btnTableView_Click(object sender, EventArgs e) => TableBUS.Instance.GetTableList();
        private void dtgrvTable_CellClick(object sender, DataGridViewCellEventArgs e) =>  BindingTableData();
        private void btnTableAdd_Click(object sender, EventArgs e)
        {
            if(string.IsNullOrEmpty(tbxTableName.Text))
            {
                MessageBox.Show("Thêm bàn thất bại, vui lòng điền tên bàn", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            var result = TableBUS.Instance.AddTable(tbxTableName.Text);
            if (result > 0)
                MessageBox.Show("Thêm bàn thành công", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
            else
                MessageBox.Show("Thêm bàn thất bại, vui lòng kiểm tra lại thông tin", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
        private void btnTableDel_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(tbxTableId.Text))
            {
                MessageBox.Show("Xóa bàn thất bại, vui lòng chọn bàn cần xóa", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            var result = TableBUS.Instance.DelTable(int.Parse(tbxTableId.Text));
            if (result > 0)
                MessageBox.Show("Xóa bàn thành công", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
            else
                MessageBox.Show("Xóa bàn thất bại, vui lòng kiểm tra lại thông tin", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);

        }
        private void btnTableEdit_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(tbxTableId.Text))
            {
                MessageBox.Show("Sửa bàn thất bại, vui lòng chọn bàn cần sửa", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            var table = new TableDTO(int.Parse(tbxTableId.Text), tbxTableName.Text, cbxTableStatus.SelectedValue.ToString());
            var result = TableBUS.Instance.EditTable(table);
            if (result > 0)
                MessageBox.Show("Sửa bàn thành công", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
            else
                MessageBox.Show("Sửa bàn thất bại, vui lòng kiểm tra lại thông tin", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
        private void frmAdminGUI_FormClosed(object sender, FormClosedEventArgs e)
        {
            frmClosed?.Invoke(sender, e);
        }
        private void btnCategoryView_Click(object sender, EventArgs e) => GetCategoryList();
        private void btnCategoryAdd_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(tbxCategoryName.Text))
            {
                MessageBox.Show("Thêm Danh mục thất bại, vui lòng điền tên Danh mục", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            var result = CatergoryBUS.Instance.AddCategory(tbxCategoryName.Text);
            if (result > 0)
                MessageBox.Show("Thêm Danh mục thành công", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
            else
                MessageBox.Show("Thêm Danh mục thất bại, vui lòng kiểm tra lại thông tin", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
        private void btnCategoryDel_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(tbxCategoryId.Text))
            {
                MessageBox.Show("Xóa Danh mục thất bại, vui lòng chọn Danh mục cần xóa", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            var result = CatergoryBUS.Instance.DelCategory(int.Parse(tbxCategoryId.Text));
            if (result > 0)
                MessageBox.Show("Xóa Danh mục thành công", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
            else
                MessageBox.Show("Xóa Danh mục thất bại, vui lòng kiểm tra lại thông tin", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
        private void btnCategoryEdit_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(tbxCategoryId.Text))
            {
                MessageBox.Show("Sửa Danh mục thất bại, vui lòng chọn Danh mục cần sửa", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            var category = new CategoryDTO(int.Parse(tbxCategoryId.Text), tbxCategoryName.Text);
            var result = CatergoryBUS.Instance.EditCategory(category);
            if (result > 0)
                MessageBox.Show("Sửa Danh mục thành công", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
            else
                MessageBox.Show("Sửa Danh mục thất bại, vui lòng kiểm tra lại thông tin", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
        private void dtgrvCategory_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            BindingCategoryData();
        }
        #endregion


    }
}
