using Coffee.BUS;
using Coffee.DTO;
using System;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;

namespace ShopManagement.GUI
{
    public partial class frmManagerGUI : Form
    {
        public frmManagerGUI(AccountDTO account)
        {
            InitializeComponent();
            Account = account;
            LoadTableList();
            LoadCategoryList();
            LoadTableListForCombobox(cbxChangeTable);
        }
        #region Method
        void LoadTableList()
        {
            flpTable.Controls.Clear();
            var tablelist = TableBUS.Instance.GetTableList();
            foreach (var table in tablelist)
            {
                Button btn = new Button() { Width = 128, Height = 90 };
                btn.Text = table.Name + Environment.NewLine + table.Status;
                switch (table.Status)
                {
                    case "Trống":
                        btn.BackColor = Color.ForestGreen;
                        break;
                    default:
                        btn.BackColor = Color.Red;
                        break;
                }
                flpTable.Controls.Add(btn);
                btn.Click += Btn_Click;
                btn.Tag = table;
            }
        }
        private AccountDTO _account;
        public AccountDTO Account
        {
            get => _account;
            set
            {
                _account = value;
                CheckAdminAccount(Account.Status);
            }
        }
        void LoadCategoryList()
        {
            cbxCategory.DataSource = CatergoryBUS.Instance.GetcategoryList();
            cbxCategory.DisplayMember = "Name";
        }
        void LoadFoodList(int categoryId)
        {
            cbxFood.DataSource = FoodBUS.Instance.GetFoodList(categoryId);
            cbxFood.DisplayMember = "Name";
        }
        void ShowBill(int tableId)
        {
            lvBill.Items.Clear();
            totalmoney = 0;
            var listMenu = MenuBUS.Instance.GetMenuList(tableId);
            foreach (var item in listMenu)
            {
                ListViewItem listView = new ListViewItem(item.Name);
                listView.SubItems.Add(item.Count.ToString());
                listView.SubItems.Add(item.Price.ToString());
                listView.SubItems.Add(item.Money.ToString());
                totalmoney += item.Money;
                lvBill.Items.Add(listView);
            }
            tbxTotalMoney.Text = totalmoney.ToString("c", new CultureInfo("vi-VN"));
        }
        void CheckAdminAccount(int accounttye) => QuantriToolStripMenuItem.Enabled = accounttye == 1;
        void LoadTableListForCombobox(ComboBox cbx)
        {
            cbx.DataSource = TableBUS.Instance.GetTableList();
            cbx.DisplayMember = "Name";
        }
        private static int totalmoney;
        #endregion

        #region Event

        private void Btn_Click(object sender, EventArgs e)
        {
            lvBill.Items.Clear();
            tbxTotalMoney.Clear();
            lvBill.Tag = (sender as Button).Tag;
            var tableId = ((sender as Button).Tag as TableDTO).Id;
            ShowBill(tableId);
        }
        private void cbxCategory_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                tbxPrice.DataBindings.Clear();
                ComboBox cb = sender as ComboBox;
                int categoryId = (cb.SelectedItem as CategoryDTO).Id;
                LoadFoodList(categoryId);
                tbxPrice.DataBindings.Add(new Binding("Text", cbxFood.DataSource, "Price"));
            }
            catch { }
        }
        private void btnAddFood_Click(object sender, EventArgs e)
        {
            if ((lvBill.Tag as TableDTO) is null)
            {
                MessageBox.Show("Chưa có bàn được chọn để thêm món", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            int tableid = (lvBill.Tag as TableDTO).Id;
            var billid = BillBUS.Instances.GetBillId(tableid);
            var foodid = (cbxFood.SelectedItem as FoodDTO).Id;
            var count = (int)nmrCount.Value;
            if (billid == -1)
            {
                if (count < 1)
                {
                    MessageBox.Show("Thêm món thất bại, chưa có món để bớt", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                var rowbill = BillBUS.Instances.InsertBill(tableid);
                var rowbillinfor = BillInforBUS.Instances.InsertBillInfor(BillBUS.Instances.GetMaxBillId(), foodid, count);
                if (rowbillinfor > 0)
                    MessageBox.Show("Thêm món thành công", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                var rowbillinfor = BillInforBUS.Instances.InsertBillInfor(billid, foodid, count);
                if (rowbillinfor > 0)
                    MessageBox.Show("Thêm món thành công", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                else
                    MessageBox.Show("Thêm món thất bại, không có món để bớt", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            ShowBill(tableid);
            LoadTableList();
        }
        private void btnCheckOut_Click(object sender, EventArgs e)
        {
            var table = lvBill.Tag as TableDTO;
            var discount = (int)nmrDisscount.Value;
            var finalmoney = totalmoney - (totalmoney * discount) / 100;
            var billid = BillBUS.Instances.GetBillId(table.Id);
            if (billid > 0)
            {
                if (MessageBox.Show($"Bạn chắc chắn thanh toán hóa đơn cho {table.Name}?\nTổng tiền sau giảm giá {nmrDisscount.Value}% ({(totalmoney * discount) / 100} đồng) là\n {finalmoney} đồng", "Thông báo", MessageBoxButtons.OKCancel, MessageBoxIcon.Information) == DialogResult.OK)
                {
                    var rowbill = BillBUS.Instances.CheckOut(billid, discount, finalmoney);
                    if (rowbill > 0)
                        MessageBox.Show("Thanh toán thành công", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    else
                        MessageBox.Show("Thanh toán thất bại", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);

                    LoadTableList();
                    ShowBill(table.Id);
                }
            }
            else
                MessageBox.Show("Thanh toán thất bại, bàn chưa có hóa đơn", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);

        }
        private void btnChangeTable_Click(object sender, EventArgs e)
        {
            var firsttable = lvBill.Tag as TableDTO;
            var secondtable = cbxChangeTable.SelectedValue as TableDTO;
            if (BillBUS.Instances.GetBillId(firsttable.Id) == -1 && BillBUS.Instances.GetBillId(secondtable.Id) == -1)
            {
                MessageBox.Show($"Bạn không thể chuyển bàn trống cho nhau", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            else
            {
                if (MessageBox.Show($"Bạn chắc chắn muốn chuyển từ {firsttable.Name} sang {secondtable.Name}?", "Thông báo", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    TableBUS.Instance.ChangeTable(firsttable.Id, secondtable.Id);
                    LoadTableList();
                }
            }
        }
        private void QuantriToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmAdminGUI admin = new frmAdminGUI();
            admin.Insertevent += Admin_Insertevent;
            admin.Editevent += Admin_Editevent;
            admin.Deleteevent += Admin_Deleteevent;
            admin.frmClosed += ReloadTable_Event;
            this.Hide();
            admin.account = _account;
            admin.ShowDialog();
            this.Show();
        }
        private void Admin_Deleteevent(object sender, EventArgs e)
        {
            LoadCategoryList();
            ShowBill((lvBill.Tag as TableDTO).Id);
            LoadTableList();
        }
        private void Admin_Editevent(object sender, EventArgs e)
        {
            LoadCategoryList();
            ShowBill((lvBill.Tag as TableDTO).Id);
        }
        private void Admin_Insertevent(object sender, EventArgs e)
        {
            LoadCategoryList();
            ShowBill((lvBill.Tag as TableDTO).Id);
        }
        private void đăngXuấtToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        private void thôngTinToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var acount = acc == null ? _account : acc;
            frmAccountProfileGUI profile = new frmAccountProfileGUI(acount);
            profile.AccountBackEvent += Profile_AccountBackEvent;
            this.Hide();
            profile.ShowDialog();
            this.Show();
        }
        private void Profile_AccountBackEvent(object sender, AccountfromProfile e)
        {
            this.Text = "Manager - " + e.Account.NickName;
            acc = e.Account;
        }
        private AccountDTO acc;
        private void thêmMónToolStripMenuItem_Click(object sender, EventArgs e)
        {
            btnAddFood_Click(sender, e);
        }
        private void thanhToánToolStripMenuItem_Click(object sender, EventArgs e)
        {
            btnCheckOut_Click(sender, e);
        }
        private void ReloadTable_Event(object sender, EventArgs e)
        {
            LoadTableList();
            LoadCategoryList();
        }
        #endregion
    }
}
