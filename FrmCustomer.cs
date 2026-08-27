using POS_System.Database;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SQLite;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace POS_System
{
    public partial class FrmCustomer : Form
    {
        public FrmCustomer()
        {
            InitializeComponent();
            LoadCustomers();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            ClearFields();
        }


        private void ClearFields()
        {
            txtCustomerID.Clear();
            txtCustomerName.Clear();
            txtContactNumber.Clear();
            txtAddress.Clear();

            txtCustomerName.Focus();
        }

        private void FrmCustomer_Load(object sender, EventArgs e)
        {

        }

        private void LoadCustomers()
        {
            try
            {
                using (SQLiteConnection connection =
                       DatabaseHelper.GetConnection())
                {
                    connection.Open();

                    string sql = @"
                SELECT
                    CustomerID,
                    CustomerName,
                    ContactNumber,
                    Address
                FROM Customers
                ORDER BY CustomerID DESC";

                    using (SQLiteCommand command =
                           new SQLiteCommand(sql, connection))
                    {
                        using (SQLiteDataAdapter adapter =
                               new SQLiteDataAdapter(command))
                        {
                            DataTable table = new DataTable();

                            adapter.Fill(table);

                            dgvCustomers.DataSource = table;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Unable to load customers.\n\n" +
                    ex.Message,
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            
            if (!ValidateCustomerInput())
            {
                return;
            }

            string customerName =
                txtCustomerName.Text.Trim();

            string contactNumber =
                txtContactNumber.Text.Trim();

            string address =
                txtAddress.Text.Trim();

            try
            {
                using (SQLiteConnection connection =
                       DatabaseHelper.GetConnection())
                {
                    connection.Open();

                    string sql = @"
                INSERT INTO Customers
                (
                    CustomerName,
                    ContactNumber,
                    Address
                )
                VALUES
                (
                    @customerName,
                    @contactNumber,
                    @address
                )";

                    using (SQLiteCommand command =
                           new SQLiteCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue(
                            "@customerName",
                            customerName);

                        command.Parameters.AddWithValue(
                            "@contactNumber",
                            contactNumber);

                        command.Parameters.AddWithValue(
                            "@address",
                            address);

                        command.ExecuteNonQuery();
                    }
                }

                MessageBox.Show(
                    "Customer added successfully.",
                    "Success",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);

                ClearFields();

                LoadCustomers();
            }
            catch (SQLiteException ex)
            {
                MessageBox.Show(
                    "Unable to add customer.\n\n" +
                    ex.Message,
                    "Database Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            
        }
    }

        private bool ValidateCustomerInput()
        {
            if (txtCustomerName.Text.Trim() == "")
            {
                MessageBox.Show(
                    "Please enter a customer name.",
                    "Validation",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);

                txtCustomerName.Focus();

                return false;
            }

            return true;
        }

        private void dgvCustomers_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            
            if (e.RowIndex < 0)
            {
                return;
            }

            DataGridViewRow row =
                dgvCustomers.Rows[e.RowIndex];

            txtCustomerID.Text =
                row.Cells["CustomerID"].Value?.ToString();

            txtCustomerName.Text =
                row.Cells["CustomerName"].Value?.ToString();

            txtContactNumber.Text =
                row.Cells["ContactNumber"].Value?.ToString();

            txtAddress.Text =
                row.Cells["Address"].Value?.ToString();
        
    }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            
            if (txtCustomerID.Text.Trim() == "")
            {
                MessageBox.Show(
                    "Please select a customer first.",
                    "Validation",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);

                return;
            }

            if (!ValidateCustomerInput())
            {
                return;
            }

            int customerID =
                Convert.ToInt32(txtCustomerID.Text);

            string customerName =
                txtCustomerName.Text.Trim();

            string contactNumber =
                txtContactNumber.Text.Trim();

            string address =
                txtAddress.Text.Trim();

            try
            {
                using (SQLiteConnection connection =
                       DatabaseHelper.GetConnection())
                {
                    connection.Open();

                    string sql = @"
                UPDATE Customers
                SET
                    CustomerName = @customerName,
                    ContactNumber = @contactNumber,
                    Address = @address
                WHERE CustomerID = @customerID";

                    using (SQLiteCommand command =
                           new SQLiteCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue(
                            "@customerName",
                            customerName);

                        command.Parameters.AddWithValue(
                            "@contactNumber",
                            contactNumber);

                        command.Parameters.AddWithValue(
                            "@address",
                            address);

                        command.Parameters.AddWithValue(
                            "@customerID",
                            customerID);

                        command.ExecuteNonQuery();
                    }
                }

                MessageBox.Show(
                    "Customer updated successfully.",
                    "Success",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);

                ClearFields();

                LoadCustomers();
            }
            catch (SQLiteException ex)
            {
                MessageBox.Show(
                    "Unable to update customer.\n\n" +
                    ex.Message,
                    "Database Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            
        }
    }

        private void btnDelete_Click(object sender, EventArgs e)
        {
           
            if (txtCustomerID.Text.Trim() == "")
            {
                MessageBox.Show(
                    "Please select a customer first.",
                    "Validation",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);

                return;
            }

            DialogResult result = MessageBox.Show(
                "Are you sure you want to delete this customer?",
                "Confirm Delete",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (result != DialogResult.Yes)
            {
                return;
            }

            int customerID =
                Convert.ToInt32(txtCustomerID.Text);

            try
            {
                using (SQLiteConnection connection =
                       DatabaseHelper.GetConnection())
                {
                    connection.Open();

                    string sql = @"
                DELETE FROM Customers
                WHERE CustomerID = @customerID";

                    using (SQLiteCommand command =
                           new SQLiteCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue(
                            "@customerID",
                            customerID);

                        command.ExecuteNonQuery();
                    }
                }

                MessageBox.Show(
                    "Customer deleted successfully.",
                    "Success",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);

                ClearFields();

                LoadCustomers();
            }
            catch (SQLiteException ex)
            {
                MessageBox.Show(
                    "Unable to delete customer.\n\n" +
                    "This customer may already have sales records.\n\n" +
                    ex.Message,
                    "Database Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            
        }
    }

        private void btnSearch_Click(object sender, EventArgs e)
        {
                    string searchText =
                txtSearch.Text.Trim();

            try
            {
                using (SQLiteConnection connection =
                       DatabaseHelper.GetConnection())
                {
                    connection.Open();

                    string sql = @"
                SELECT
                    CustomerID,
                    CustomerName,
                    ContactNumber,
                    Address
                FROM Customers
                WHERE CustomerName LIKE @search
                   OR ContactNumber LIKE @search
                   OR Address LIKE @search
                ORDER BY CustomerID DESC";

                    using (SQLiteCommand command =
                           new SQLiteCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue(
                            "@search",
                            "%" + searchText + "%");

                        using (SQLiteDataAdapter adapter =
                               new SQLiteDataAdapter(command))
                        {
                            DataTable table = new DataTable();

                            adapter.Fill(table);

                            dgvCustomers.DataSource = table;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Unable to search customers.\n\n" +
                    ex.Message,
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            
        }
    }

        private void btnShowAll_Click(object sender, EventArgs e)
        {
            txtSearch.Clear();

            LoadCustomers();
        }
    }
}
