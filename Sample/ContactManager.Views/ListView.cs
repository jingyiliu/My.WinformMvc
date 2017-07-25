using System;
using System.Collections.Generic;
using System.Windows.Forms;
using ContactManager.Utils;
using ContactManager.Views.Model;
using ContactManager.Views.Utils;
using My.WinformMvc;

namespace ContactManager.Views
{
    [MvcView("ListView")]
    public partial class ListView : BaseView, IView<List<ContactModel>>
    {
        public ListView()
        {
            InitializeComponent();
        }

        public void BindModel(List<ContactModel> model)
        {
            Logger.Log("ListView.BindModel");
            contactDataGridView.DataSource = model;
        }

        private void btClose_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (contactDataGridView.SelectedRows.Count == 0)
            {
                MessageBox.Show("Please select a row first!", "Information");
                return;
            }

            var contact = contactDataGridView.CurrentRow.DataBoundItem as ContactModel;
            InvokeAction("Delete", contact);
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            OpenView(Constant.EditController, new ContactModel{ IsEdit = false });
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            if (contactDataGridView.SelectedRows.Count == 0)
            {
                MessageBox.Show("Please select a row first!", "Information");
                return;
            }
            var contact = contactDataGridView.CurrentRow.DataBoundItem as ContactModel;
            contact.IsEdit = true;
            OpenView(Constant.EditController, contact);
        }
    }
}
