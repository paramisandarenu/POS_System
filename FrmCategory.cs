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
using POS_System.Database;

namespace POS_System
{
    public partial class FrmCategory : Form
    {
        public FrmCategory()
        {
            InitializeComponent();
            LoadCategories();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            ClearFields();
        }

        private void ClearFields()
        {
            txtCategoryID.Clear();
            txtCategoryName.Clear();
            txtDescription.Clear();

            txtCategoryName.Focus();
        }

        private void LoadCategories()
        {
            try
            {
                using (SQLiteConnection connection = DatabaseHelper.GetConnection())
                {
                    connection.Open();

                    string sql = @"SELECT CategoryID, CategoryName, Description
                                    FROM Categories ORDER BY CategoryID DESC";

                    using (SQLiteCommand command = new SQLiteCommand(sql, connection))
                    {
                        using (SQLiteDataAdapter adapter = new SQLiteDataAdapter(command))
                        {
                            DataTable table = new DataTable();

                            adapter.Fill(table);
                            dgvCategories.DataSource = table;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Unable to load categories. \n\n" + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            
            string categoryName =
                txtCategoryName.Text.Trim();

            string description =
                txtDescription.Text.Trim();

            if (categoryName == "")
            {
                MessageBox.Show(
                    "Please enter a category name.",
                    "Validation",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);

                txtCategoryName.Focus();

                return;
            }

            try
            {
                using (SQLiteConnection connection =
                       DatabaseHelper.GetConnection())
                {
                    connection.Open();

                    string sql = @"
                INSERT INTO Categories
                (
                    CategoryName,
                    Description
                )
                VALUES
                (
                    @categoryName,
                    @description
                )";

                    using (SQLiteCommand command =
                           new SQLiteCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue(
                            "@categoryName",
                            categoryName);

                        command.Parameters.AddWithValue(
                            "@description",
                            description);

                        command.ExecuteNonQuery();
                    }
                }

                MessageBox.Show(
                    "Category added successfully.",
                    "Success",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);

                ClearFields();

                LoadCategories();
            }
            catch (SQLiteException ex)
            {
                MessageBox.Show(
                    "Unable to add category.\n\n" +
                    ex.Message,
                    "Database Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            
        }
    }

        private void dgvCategories_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            
            if (e.RowIndex < 0)
            {
                return;
            }

            DataGridViewRow row =
                dgvCategories.Rows[e.RowIndex];

            txtCategoryID.Text =
                row.Cells["CategoryID"].Value?.ToString();

            txtCategoryName.Text =
                row.Cells["CategoryName"].Value?.ToString();

            txtDescription.Text =
                row.Cells["Description"].Value?.ToString();
        
    }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            
            if (txtCategoryID.Text.Trim() == "")
            {
                MessageBox.Show(
                    "Please select a category first.",
                    "Validation",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);

                return;
            }

            string categoryName =
                txtCategoryName.Text.Trim();

            string description =
                txtDescription.Text.Trim();

            if (categoryName == "")
            {
                MessageBox.Show(
                    "Please enter a category name.",
                    "Validation",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);

                txtCategoryName.Focus();

                return;
            }

            int categoryID =
                Convert.ToInt32(txtCategoryID.Text);

            try
            {
                using (SQLiteConnection connection =
                       DatabaseHelper.GetConnection())
                {
                    connection.Open();

                    string sql = @"
                UPDATE Categories
                SET
                    CategoryName = @categoryName,
                    Description = @description
                WHERE CategoryID = @categoryID";

                    using (SQLiteCommand command =
                           new SQLiteCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue(
                            "@categoryName",
                            categoryName);

                        command.Parameters.AddWithValue(
                            "@description",
                            description);

                        command.Parameters.AddWithValue(
                            "@categoryID",
                            categoryID);

                        command.ExecuteNonQuery();
                    }
                }

                MessageBox.Show(
                    "Category updated successfully.",
                    "Success",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);

                ClearFields();

                LoadCategories();
            }
            catch (SQLiteException ex)
            {
                MessageBox.Show(
                    "Unable to update category.\n\n" +
                    ex.Message,
                    "Database Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            
        }
    }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            
            if (txtCategoryID.Text.Trim() == "")
            {
                MessageBox.Show(
                    "Please select a category first.",
                    "Validation",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);

                return;
            }

            DialogResult result = MessageBox.Show(
                "Are you sure you want to delete this category?",
                "Confirm Delete",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (result != DialogResult.Yes)
            {
                return;
            }

            int categoryID =
                Convert.ToInt32(txtCategoryID.Text);

            try
            {
                using (SQLiteConnection connection =
                       DatabaseHelper.GetConnection())
                {
                    connection.Open();

                    string sql = @"
                DELETE FROM Categories
                WHERE CategoryID = @categoryID";

                    using (SQLiteCommand command =
                           new SQLiteCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue(
                            "@categoryID",
                            categoryID);

                        command.ExecuteNonQuery();
                    }
                }

                MessageBox.Show(
                    "Category deleted successfully.",
                    "Success",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);

                ClearFields();

                LoadCategories();
            }
            catch (SQLiteException ex)
            {
                MessageBox.Show(
                    "Unable to delete category.\n\n" +
                    "If this category is being used by a product, " +
                    "delete or update those products first.\n\n" +
                    ex.Message,
                    "Database Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            
        }
    }
    }
}
