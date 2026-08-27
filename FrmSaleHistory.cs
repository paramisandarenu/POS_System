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
    public partial class FrmSaleHistory : Form
    {
        public FrmSaleHistory()
        {
            InitializeComponent();
            dtpFromDate.Value = DateTime.Today;
            dtpToDate.Value = DateTime.Today;

            LoadSales();
        }

        private void FrmSaleHistory_Load(object sender, EventArgs e)
        {

        }

        private void LoadSales()
        {
            try
            {
                using (SQLiteConnection connection =
                       DatabaseHelper.GetConnection())
                {
                    connection.Open();

                    string sql = @"
                SELECT
                    s.SaleID,
                    s.SaleDate,
                    c.CustomerName,
                    s.TotalAmount
                FROM Sales s
                LEFT JOIN Customers c
                    ON s.CustomerID = c.CustomerID
                ORDER BY s.SaleID DESC";

                    using (SQLiteCommand command =
                           new SQLiteCommand(sql, connection))
                    {
                        using (SQLiteDataAdapter adapter =
                               new SQLiteDataAdapter(command))
                        {
                            DataTable table = new DataTable();

                            adapter.Fill(table);

                            dgvSales.DataSource = table;
                        }
                    }
                }

                FormatSalesGrid();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Unable to load sales history.\n\n" +
                    ex.Message,
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private void FormatSalesGrid()
        {
            if (dgvSales.Columns.Contains("SaleID"))
            {
                dgvSales.Columns["SaleID"]
                    .HeaderText = "Sale ID";
            }

            if (dgvSales.Columns.Contains("SaleDate"))
            {
                dgvSales.Columns["SaleDate"]
                    .HeaderText = "Date";
            }

            if (dgvSales.Columns.Contains("CustomerName"))
            {
                dgvSales.Columns["CustomerName"]
                    .HeaderText = "Customer";
            }

            if (dgvSales.Columns.Contains("TotalAmount"))
            {
                dgvSales.Columns["TotalAmount"]
                    .HeaderText = "Total Amount";
            }
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            
            DateTime fromDate =
                dtpFromDate.Value.Date;

            DateTime toDate =
                dtpToDate.Value.Date;

            if (fromDate > toDate)
            {
                MessageBox.Show(
                    "From Date cannot be after To Date.",
                    "Validation",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);

                return;
            }

            try
            {
                using (SQLiteConnection connection =
                       DatabaseHelper.GetConnection())
                {
                    connection.Open();

                    string sql = @"
                SELECT
                    s.SaleID,
                    s.SaleDate,
                    c.CustomerName,
                    s.TotalAmount
                FROM Sales s
                LEFT JOIN Customers c
                    ON s.CustomerID = c.CustomerID
                WHERE date(s.SaleDate)
                      BETWEEN @fromDate AND @toDate
                ORDER BY s.SaleID DESC";

                    using (SQLiteCommand command =
                           new SQLiteCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue(
                            "@fromDate",
                            fromDate.ToString("yyyy-MM-dd"));

                        command.Parameters.AddWithValue(
                            "@toDate",
                            toDate.ToString("yyyy-MM-dd"));

                        using (SQLiteDataAdapter adapter =
                               new SQLiteDataAdapter(command))
                        {
                            DataTable table =
                                new DataTable();

                            adapter.Fill(table);

                            dgvSales.DataSource = table;
                        }
                    }
                }

                FormatSalesGrid();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Unable to search sales.\n\n" +
                    ex.Message,
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            
        }
    }

        private void btnShowAll_Click(object sender, EventArgs e)
        {
            LoadSales();
        }

        private void dgvSales_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            
            if (e.RowIndex < 0)
            {
                return;
            }

            DataGridViewRow row =
                dgvSales.Rows[e.RowIndex];

            if (row.Cells["SaleID"].Value == null)
            {
                return;
            }

            int saleID =
                Convert.ToInt32(
                    row.Cells["SaleID"].Value);

            if (row.Cells["TotalAmount"].Value != null)
            {
                lblSelectedTotal.Text =
                    Convert.ToDecimal(
                        row.Cells["TotalAmount"].Value)
                    .ToString("0.00");
            }

            LoadSaleDetails(saleID);
        }

        private void LoadSaleDetails(int saleID)
        {
            try
            {
                using (SQLiteConnection connection =
                       DatabaseHelper.GetConnection())
                {
                    connection.Open();

                    string sql = @"
                SELECT
                    p.ProductName,
                    si.Quantity,
                    si.UnitPrice,
                    si.LineTotal
                FROM SaleItems si
                INNER JOIN Products p
                    ON si.ProductID = p.ProductID
                WHERE si.SaleID = @saleID
                ORDER BY si.SaleItemID";

                    using (SQLiteCommand command =
                           new SQLiteCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue(
                            "@saleID",
                            saleID);

                        using (SQLiteDataAdapter adapter =
                               new SQLiteDataAdapter(command))
                        {
                            DataTable table =
                                new DataTable();

                            adapter.Fill(table);

                            dgvSaleDetails.DataSource =
                                table;
                        }
                    }
                }

                FormatSaleDetailsGrid();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Unable to load sale details.\n\n" +
                    ex.Message,
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private void FormatSaleDetailsGrid()
        {
            if (dgvSaleDetails.Columns.Contains(
                    "ProductName"))
            {
                dgvSaleDetails.Columns["ProductName"]
                    .HeaderText = "Product";
            }

            if (dgvSaleDetails.Columns.Contains(
                    "Quantity"))
            {
                dgvSaleDetails.Columns["Quantity"]
                    .HeaderText = "Quantity";
            }

            if (dgvSaleDetails.Columns.Contains(
                    "UnitPrice"))
            {
                dgvSaleDetails.Columns["UnitPrice"]
                    .HeaderText = "Unit Price";
            }

            if (dgvSaleDetails.Columns.Contains(
                    "LineTotal"))
            {
                dgvSaleDetails.Columns["LineTotal"]
                    .HeaderText = "Line Total";
            }
        }

        private void btnViewDetails_Click(object sender, EventArgs e)
        {
            
            if (dgvSales.CurrentRow == null)
            {
                MessageBox.Show(
                    "Please select a sale first.",
                    "Validation",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);

                return;
            }

            DataGridViewRow row =
                dgvSales.CurrentRow;

            if (row.Cells["SaleID"].Value == null)
            {
                return;
            }

            int saleID =
                Convert.ToInt32(
                    row.Cells["SaleID"].Value);

            LoadSaleDetails(saleID);

            if (row.Cells["TotalAmount"].Value != null)
            {
                lblSelectedTotal.Text =
                    Convert.ToDecimal(
                        row.Cells["TotalAmount"].Value)
                    .ToString("0.00");
            }
        }

        private void lblToDate_Click(object sender, EventArgs e)
        {

        }
    }
}
