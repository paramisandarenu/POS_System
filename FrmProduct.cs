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
    public partial class FrmProduct : Form
    {
        public FrmProduct()
        {
            InitializeComponent();
            LoadCategories();
            LoadProducts();
        }

        private void FrmProduct_Load(object sender, EventArgs e)
        {

        }

        private void LoadCategories()
        {
            try
            {
                using (SQLiteConnection connection =
                       DatabaseHelper.GetConnection())
                {
                    connection.Open();

                    string sql = @"
                SELECT
                    CategoryID,
                    CategoryName
                FROM Categories
                ORDER BY CategoryName";

                    using (SQLiteCommand command =
                           new SQLiteCommand(sql, connection))
                    {
                        using (SQLiteDataReader reader =
                               command.ExecuteReader())
                        {
                            DataTable table = new DataTable();

                            table.Load(reader);

                            cmbCategory.DataSource = table;
                            cmbCategory.DisplayMember = "CategoryName";
                            cmbCategory.ValueMember = "CategoryID";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Unable to load categories.\n\n" +
                    ex.Message,
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private void LoadProducts()
        {
            try
            {
                using (SQLiteConnection connection =
                       DatabaseHelper.GetConnection())
                {
                    connection.Open();

                    string sql = @"
                SELECT
                    p.ProductID,
                    p.ProductName,
                    p.CategoryID,
                    c.CategoryName,
                    p.UnitPrice,
                    p.StockQuantity
                FROM Products p
                INNER JOIN Categories c
                    ON p.CategoryID = c.CategoryID
                ORDER BY p.ProductID DESC";

                    using (SQLiteCommand command =
                           new SQLiteCommand(sql, connection))
                    {
                        using (SQLiteDataAdapter adapter =
                               new SQLiteDataAdapter(command))
                        {
                            DataTable table = new DataTable();

                            adapter.Fill(table);

                            dgvProducts.DataSource = table;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Unable to load products.\n\n" +
                    ex.Message,
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            
            if (!ValidateProductInput())
            {
                return;
            }

            string productName =
                txtProductName.Text.Trim();

            int categoryID =
                Convert.ToInt32(cmbCategory.SelectedValue);

            decimal unitPrice =
                Convert.ToDecimal(txtUnitPrice.Text);

            int stockQuantity =
                Convert.ToInt32(txtStockQuantity.Text);

            try
            {
                using (SQLiteConnection connection =
                       DatabaseHelper.GetConnection())
                {
                    connection.Open();

                    string sql = @"
                INSERT INTO Products
                (
                    ProductName,
                    CategoryID,
                    UnitPrice,
                    StockQuantity
                )
                VALUES
                (
                    @productName,
                    @categoryID,
                    @unitPrice,
                    @stockQuantity
                )";

                    using (SQLiteCommand command =
                           new SQLiteCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue(
                            "@productName",
                            productName);

                        command.Parameters.AddWithValue(
                            "@categoryID",
                            categoryID);

                        command.Parameters.AddWithValue(
                            "@unitPrice",
                            unitPrice);

                        command.Parameters.AddWithValue(
                            "@stockQuantity",
                            stockQuantity);

                        command.ExecuteNonQuery();
                    }
                }

                MessageBox.Show(
                    "Product added successfully.",
                    "Success",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);

                ClearFields();
                LoadProducts();
            }
            catch (SQLiteException ex)
            {
                MessageBox.Show(
                    "Unable to add product.\n\n" +
                    ex.Message,
                    "Database Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            
        }
    }

        private bool ValidateProductInput()
        {
            if (txtProductName.Text.Trim() == "")
            {
                MessageBox.Show(
                    "Please enter a product name.",
                    "Validation",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);

                txtProductName.Focus();

                return false;
            }

            if (cmbCategory.SelectedIndex == -1)
            {
                MessageBox.Show(
                    "Please select a category.",
                    "Validation",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);

                cmbCategory.Focus();

                return false;
            }

            decimal unitPrice;

            if (!decimal.TryParse(
                    txtUnitPrice.Text.Trim(),
                    out unitPrice))
            {
                MessageBox.Show(
                    "Please enter a valid unit price.",
                    "Validation",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);

                txtUnitPrice.Focus();

                return false;
            }

            if (unitPrice <= 0)
            {
                MessageBox.Show(
                    "Unit price must be greater than zero.",
                    "Validation",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);

                txtUnitPrice.Focus();

                return false;
            }

            int stockQuantity;

            if (!int.TryParse(
                    txtStockQuantity.Text.Trim(),
                    out stockQuantity))
            {
                MessageBox.Show(
                    "Please enter a valid stock quantity.",
                    "Validation",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);

                txtStockQuantity.Focus();

                return false;
            }

            if (stockQuantity < 0)
            {
                MessageBox.Show(
                    "Stock quantity cannot be negative.",
                    "Validation",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);

                txtStockQuantity.Focus();

                return false;
            }

            return true;
        }

        private void dgvProducts_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            
            if (e.RowIndex < 0)
            {
                return;
            }

            DataGridViewRow row =
                dgvProducts.Rows[e.RowIndex];

            txtProductID.Text =
                row.Cells["ProductID"].Value?.ToString();

            txtProductName.Text =
                row.Cells["ProductName"].Value?.ToString();

            txtUnitPrice.Text =
                row.Cells["UnitPrice"].Value?.ToString();

            txtStockQuantity.Text =
                row.Cells["StockQuantity"].Value?.ToString();

            if (row.Cells["CategoryID"].Value != null)
            {
                cmbCategory.SelectedValue =
                    row.Cells["CategoryID"].Value;
            
        }
    }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            
            if (txtProductID.Text.Trim() == "")
            {
                MessageBox.Show(
                    "Please select a product first.",
                    "Validation",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);

                return;
            }

            if (!ValidateProductInput())
            {
                return;
            }

            int productID =
                Convert.ToInt32(txtProductID.Text);

            string productName =
                txtProductName.Text.Trim();

            int categoryID =
                Convert.ToInt32(cmbCategory.SelectedValue);

            decimal unitPrice =
                Convert.ToDecimal(txtUnitPrice.Text);

            int stockQuantity =
                Convert.ToInt32(txtStockQuantity.Text);

            try
            {
                using (SQLiteConnection connection =
                       DatabaseHelper.GetConnection())
                {
                    connection.Open();

                    string sql = @"
                UPDATE Products
                SET
                    ProductName = @productName,
                    CategoryID = @categoryID,
                    UnitPrice = @unitPrice,
                    StockQuantity = @stockQuantity
                WHERE ProductID = @productID";

                    using (SQLiteCommand command =
                           new SQLiteCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue(
                            "@productName",
                            productName);

                        command.Parameters.AddWithValue(
                            "@categoryID",
                            categoryID);

                        command.Parameters.AddWithValue(
                            "@unitPrice",
                            unitPrice);

                        command.Parameters.AddWithValue(
                            "@stockQuantity",
                            stockQuantity);

                        command.Parameters.AddWithValue(
                            "@productID",
                            productID);

                        command.ExecuteNonQuery();
                    }
                }

                MessageBox.Show(
                    "Product updated successfully.",
                    "Success",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);

                ClearFields();
                LoadProducts();
            }
            catch (SQLiteException ex)
            {
                MessageBox.Show(
                    "Unable to update product.\n\n" +
                    ex.Message,
                    "Database Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            
        }
    }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            
            if (txtProductID.Text.Trim() == "")
            {
                MessageBox.Show(
                    "Please select a product first.",
                    "Validation",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);

                return;
            }

            DialogResult result = MessageBox.Show(
                "Are you sure you want to delete this product?",
                "Confirm Delete",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (result != DialogResult.Yes)
            {
                return;
            }

            int productID =
                Convert.ToInt32(txtProductID.Text);

            try
            {
                using (SQLiteConnection connection =
                       DatabaseHelper.GetConnection())
                {
                    connection.Open();

                    string sql = @"
                DELETE FROM Products
                WHERE ProductID = @productID";

                    using (SQLiteCommand command =
                           new SQLiteCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue(
                            "@productID",
                            productID);

                        command.ExecuteNonQuery();
                    }
                }

                MessageBox.Show(
                    "Product deleted successfully.",
                    "Success",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);

                ClearFields();
                LoadProducts();
            }
            catch (SQLiteException ex)
            {
                MessageBox.Show(
                    "Unable to delete product.\n\n" +
                    ex.Message,
                    "Database Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            
        }
    }

        private void btnClear_Click(object sender, EventArgs e)
        {
            ClearFields();
        }

        private void ClearFields()
        {
            txtProductID.Clear();
            txtProductName.Clear();
            txtUnitPrice.Clear();
            txtStockQuantity.Clear();

            if (cmbCategory.Items.Count > 0)
            {
                cmbCategory.SelectedIndex = -1;
            }

            txtProductName.Focus();
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
                    p.ProductID,
                    p.ProductName,
                    p.CategoryID,
                    c.CategoryName,
                    p.UnitPrice,
                    p.StockQuantity
                FROM Products p
                INNER JOIN Categories c
                    ON p.CategoryID = c.CategoryID
                WHERE p.ProductName LIKE @search
                   OR c.CategoryName LIKE @search
                ORDER BY p.ProductID DESC";

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

                            dgvProducts.DataSource = table;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Unable to search products.\n\n" +
                    ex.Message,
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            
        }
    }

        private void btnShowAll_Click(object sender, EventArgs e)
        {
            txtSearch.Clear();

            LoadProducts();
        }

        private void lblProductID_Click(object sender, EventArgs e)
        {

        }

        private void lblProductName_Click(object sender, EventArgs e)
        {

        }

        private void lblCategory_Click(object sender, EventArgs e)
        {

        }

        private void txtUnitPrice_TextChanged(object sender, EventArgs e)
        {

        }

        private void lblSearch_Click(object sender, EventArgs e)
        {

        }
    }
}
