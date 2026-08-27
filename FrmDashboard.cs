using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace POS_System
{
    public partial class FrmDashboard : Form
    {
        public FrmDashboard()
        {
            InitializeComponent();
        }

        private void btnCategories_Click(object sender, EventArgs e)
        {
            this.Hide();

            using (FrmCategory form = new FrmCategory())
            {
                form.ShowDialog();
            }

            this.Show();
        }

        private void btnProducts_Click(object sender, EventArgs e)
        {
            this.Hide();

            using (FrmProduct form = new FrmProduct())
            {
                form.ShowDialog();
            }

            this.Show();
        }

        private void btnCustomers_Click(object sender, EventArgs e)
        {
            this.Hide();

            using (FrmCustomer form = new FrmCustomer())
            {
                form.ShowDialog();
            }

            this.Show();
        }

        private void btnBilling_Click(object sender, EventArgs e)
        {
            this.Hide();

            using (FrmBilling form = new FrmBilling())
            {
                form.ShowDialog();
            }

            this.Show();
        }

        private void btnSalesHistory_Click(object sender, EventArgs e)
        {
            this.Hide();

            using (FrmSaleHistory form = new FrmSaleHistory())
            {
                form.ShowDialog();
            }

            this.Show();
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show("Are you sure you want to exit?", "Exit", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result == DialogResult.Yes)
            {
                MessageBox.Show("Thank you for using the POS System.", "Goodbye");

                Application.Exit();
            }
        }
    }
}
