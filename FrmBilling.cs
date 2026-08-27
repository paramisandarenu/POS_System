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
    public partial class FrmBilling : Form
    {

        private class CartItem
        {
            public string SaleID { get; set; }

            public string SaleDate { get; set; }

            public int ProductID { get; set; }

            public string ProductName { get; set; }

            public decimal UnitPrice { get; set; }

            public int Quantity { get; set; }

            public decimal LineTotal
            {
                get
                {
                    return UnitPrice * Quantity;
                }
            }
        }

        private readonly List<CartItem> cartItems =
    new List<CartItem>();

        public FrmBilling()
        {
            InitializeComponent();
          
            dtpSaleDate.Value = DateTime.Now;

            lblSaleID.Text = "NEW";

            LoadCustomers();

            LoadProducts();

            SetupCartGrid();

            UpdateGrandTotal();
        }

        private void label3_Click(object sender, EventArgs e)
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
                    CustomerName
                FROM Customers
                ORDER BY CustomerName";

                    using (SQLiteCommand command =
                           new SQLiteCommand(sql, connection))
                    {
                        using (SQLiteDataAdapter adapter =
                               new SQLiteDataAdapter(command))
                        {
                            DataTable table =
                                new DataTable();

                            adapter.Fill(table);

                            cmbCustomer.DataSource = table;

                            cmbCustomer.DisplayMember =
                                "CustomerName";

                            cmbCustomer.ValueMember =
                                "CustomerID";

                            cmbCustomer.SelectedIndex = -1;
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
                    ProductID,
                    ProductName
                FROM Products
                WHERE StockQuantity > 0
                ORDER BY ProductName";

                    using (SQLiteCommand command =
                           new SQLiteCommand(sql, connection))
                    {
                        using (SQLiteDataAdapter adapter =
                               new SQLiteDataAdapter(command))
                        {
                            DataTable table =
                                new DataTable();

                            adapter.Fill(table);

                            cmbProduct.DataSource = table;

                            cmbProduct.DisplayMember =
                                "ProductName";

                            cmbProduct.ValueMember =
                                "ProductID";

                            cmbProduct.SelectedIndex = -1;
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

        private void SetupCartGrid()
        {
            dgvCart.AutoGenerateColumns = false;

            dgvCart.Columns.Clear();

            dgvCart.Columns.Add(
                new DataGridViewTextBoxColumn
                {
                    Name = "SaleID",
                    HeaderText = "Sale ID",
                    DataPropertyName = "SaleID"
                });

            dgvCart.Columns.Add(
                new DataGridViewTextBoxColumn
                {
                    Name = "SaleDate",
                    HeaderText = "Sale Date",
                    DataPropertyName = "SaleDate"
                });

            dgvCart.Columns.Add(
                new DataGridViewTextBoxColumn
                {
                    Name = "ProductName",
                    HeaderText = "Product",
                    DataPropertyName = "ProductName"
                });

            dgvCart.Columns.Add(
                new DataGridViewTextBoxColumn
                {
                    Name = "UnitPrice",
                    HeaderText = "Unit Price",
                    DataPropertyName = "UnitPrice"
                });

            dgvCart.Columns.Add(
                new DataGridViewTextBoxColumn
                {
                    Name = "Quantity",
                    HeaderText = "Quantity",
                    DataPropertyName = "Quantity"
                });

            dgvCart.Columns.Add(
                new DataGridViewTextBoxColumn
                {
                    Name = "LineTotal",
                    HeaderText = "Line Total",
                    DataPropertyName = "LineTotal"
                });

            dgvCart.DataSource = cartItems;
        }

        private void cmbProduct_SelectedIndexChanged(object sender, EventArgs e)
        {
            
            if (cmbProduct.SelectedIndex == -1 ||
                cmbProduct.SelectedValue == null)
            {
                txtUnitPrice.Clear();
                txtAvailableStock.Clear();

                return;
            }

            int productID;

            try
            {
                productID =
                    Convert.ToInt32(
                        cmbProduct.SelectedValue);
            }
            catch
            {
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
                    UnitPrice,
                    StockQuantity
                FROM Products
                WHERE ProductID = @productID";

                    using (SQLiteCommand command =
                           new SQLiteCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue(
                            "@productID",
                            productID);

                        using (SQLiteDataReader reader =
                               command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                txtUnitPrice.Text =
                                    Convert.ToDecimal(
                                        reader["UnitPrice"])
                                    .ToString("0.00");

                                txtAvailableStock.Text =
                                    reader["StockQuantity"]
                                    .ToString();
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Unable to load product details.\n\n" +
                    ex.Message,
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            
        }
    }

        private void btnAddToBill_Click(object sender, EventArgs e)
        {
            
            if (cmbProduct.SelectedIndex == -1)
            {
                MessageBox.Show(
                    "Please select a product.",
                    "Validation",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);

                return;
            }

            int quantity;

            if (!int.TryParse(
                    txtQuantity.Text.Trim(),
                    out quantity))
            {
                MessageBox.Show(
                    "Please enter a valid quantity.",
                    "Validation",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);

                txtQuantity.Focus();

                return;
            }

            if (quantity <= 0)
            {
                MessageBox.Show(
                    "Quantity must be greater than zero.",
                    "Validation",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);

                txtQuantity.Focus();

                return;
            }

            int productID =
                Convert.ToInt32(
                    cmbProduct.SelectedValue);

            string productName =
                cmbProduct.Text;

            decimal unitPrice =
                Convert.ToDecimal(
                    txtUnitPrice.Text);

            int availableStock =
                Convert.ToInt32(
                    txtAvailableStock.Text);

            int existingQuantity = 0;

            foreach (CartItem item in cartItems)
            {
                if (item.ProductID == productID)
                {
                    existingQuantity =
                        item.Quantity;

                    break;
                }
            }

            if (existingQuantity + quantity >
                availableStock)
            {
                MessageBox.Show(
                    "Quantity cannot exceed available stock.\n\n" +
                    "Available stock: " +
                    availableStock,
                    "Insufficient Stock",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);

                return;
            }

            CartItem existingItem = null;

            foreach (CartItem item in cartItems)
            {
                if (item.ProductID == productID)
                {
                    existingItem = item;

                    break;
                }
            }

            if (existingItem != null)
            {
                existingItem.Quantity += quantity;
            }
            else
            {
                cartItems.Add(
                    new CartItem
                    {
                        SaleID = "NEW",

                        SaleDate =
                            dtpSaleDate.Value.ToString(
                                "dd/MM/yyyy HH:mm"),

                        ProductID = productID,

                        ProductName = productName,

                        UnitPrice = unitPrice,

                        Quantity = quantity
                    });
            }

            RefreshCart();

            txtQuantity.Clear();

            txtQuantity.Focus();
        
    }

        private void RefreshCart()
        {
            dgvCart.DataSource = null;

            dgvCart.DataSource = cartItems;

            UpdateGrandTotal();
        }

        private decimal CalculateGrandTotal()
        {
            decimal total = 0;

            foreach (CartItem item in cartItems)
            {
                total += item.LineTotal;
            }

            return total;
        }

        private void UpdateGrandTotal()
        {
            lblGrandTotal.Text =
                CalculateGrandTotal()
                .ToString("0.00");
        }

        private void btnRemoveItem_Click(object sender, EventArgs e)
        {
            
            if (dgvCart.CurrentRow == null)
            {
                MessageBox.Show(
                    "Please select an item from the bill.",
                    "Validation",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);

                return;
            }

            int rowIndex =
                dgvCart.CurrentRow.Index;

            if (rowIndex < 0 ||
                rowIndex >= cartItems.Count)
            {
                return;
            }

            cartItems.RemoveAt(rowIndex);

            RefreshCart();
        
    }

        private void btnClearBill_Click(object sender, EventArgs e)
        {
            
            DialogResult result =
                MessageBox.Show(
                    "Are you sure you want to clear this bill?",
                    "Clear Bill",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question);

            if (result != DialogResult.Yes)
            {
                return;
            }

            ClearBill();
        }

        private void ClearBill()
        {
            cartItems.Clear();

            dgvCart.DataSource = null;

            dgvCart.DataSource = cartItems;

            cmbCustomer.SelectedIndex = -1;

            cmbProduct.SelectedIndex = -1;

            txtUnitPrice.Clear();

            txtAvailableStock.Clear();

            txtQuantity.Clear();

            dtpSaleDate.Value =
                DateTime.Now;

            lblSaleID.Text = "NEW";

            lblGrandTotal.Text = "0.00";
        }

        private void btnCompleteSale_Click(object sender, EventArgs e)
        {
            
            //VaLIDATE DATE
           

            DateTime saleDate =
                dtpSaleDate.Value;

            if (saleDate > DateTime.Now)
            {
                MessageBox.Show(
                    "Sale date cannot be in the future.",
                    "Validation",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);

                return;
            }

            
            //  VALIDATE CART
            

            if (cartItems.Count == 0)
            {
                MessageBox.Show(
                    "A bill cannot be completed without at least one item.",
                    "Validation",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);

                return;
            }

            
            // GET CUSTOMER
            // CUSTOMER IS OPTIONAL
            

            int? customerID = null;

            if (cmbCustomer.SelectedIndex != -1 &&
                cmbCustomer.SelectedValue != null)
            {
                try
                {
                    customerID =
                        Convert.ToInt32(
                            cmbCustomer.SelectedValue);
                }
                catch
                {
                    customerID = null;
                }
            }

            
            //  CALCULATE TOTAL
            
            decimal totalAmount =
                CalculateGrandTotal();

            
            //  CONFIRM
            

            DialogResult result =
                MessageBox.Show(
                    "Complete this sale?\n\n" +
                    "Date: " +
                    saleDate.ToString(
                        "dd/MM/yyyy HH:mm") +
                    "\nGrand Total: " +
                    totalAmount.ToString("0.00"),
                    "Confirm Sale",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question);

            if (result != DialogResult.Yes)
            {
                return;
            }

            try
            {
                using (SQLiteConnection connection =
                       DatabaseHelper.GetConnection())
                {
                    connection.Open();

                    using (SQLiteTransaction transaction =
                           connection.BeginTransaction())
                    {
                        try
                        {
                            
                            //  INSERT SALE
                           

                            string saleSQL = @"
                        INSERT INTO Sales
                        (
                            SaleDate,
                            CustomerID,
                            TotalAmount
                        )
                        VALUES
                        (
                            @saleDate,
                            @customerID,
                            @totalAmount
                        );

                        SELECT last_insert_rowid();";

                            long saleID;

                            using (SQLiteCommand command =
                                   new SQLiteCommand(
                                       saleSQL,
                                       connection,
                                       transaction))
                            {
                                command.Parameters.AddWithValue(
                                    "@saleDate",
                                    saleDate.ToString(
                                        "yyyy-MM-dd HH:mm:ss"));

                                if (customerID.HasValue)
                                {
                                    command.Parameters.AddWithValue(
                                        "@customerID",
                                        customerID.Value);
                                }
                                else
                                {
                                    command.Parameters.AddWithValue(
                                        "@customerID",
                                        DBNull.Value);
                                }

                                command.Parameters.AddWithValue(
                                    "@totalAmount",
                                    totalAmount);

                                saleID =
                                    Convert.ToInt64(
                                        command.ExecuteScalar());
                            }

                            
                            //  SAVE SALE ITEMS
                            

                            foreach (CartItem item in cartItems)
                            {
                                // Get current stock

                                string stockSQL = @"
                            SELECT StockQuantity
                            FROM Products
                            WHERE ProductID = @productID";

                                int currentStock;

                                using (SQLiteCommand command =
                                       new SQLiteCommand(
                                           stockSQL,
                                           connection,
                                           transaction))
                                {
                                    command.Parameters.AddWithValue(
                                        "@productID",
                                        item.ProductID);

                                    object stock =
                                        command.ExecuteScalar();

                                    if (stock == null)
                                    {
                                        throw new Exception(
                                            "Product does not exist.");
                                    }

                                    currentStock =
                                        Convert.ToInt32(stock);
                                }

                                // Check quantity

                                if (item.Quantity <= 0)
                                {
                                    throw new Exception(
                                        "Quantity must be greater than zero.");
                                }

                                // Check stock

                                if (item.Quantity > currentStock)
                                {
                                    throw new Exception(
                                        "Insufficient stock for " +
                                        item.ProductName +
                                        ".\n\n" +
                                        "Available stock: " +
                                        currentStock +
                                        "\nRequested: " +
                                        item.Quantity);
                                }

                                // Save SaleItem

                                string itemSQL = @"
                            INSERT INTO SaleItems
                            (
                                SaleID,
                                ProductID,
                                Quantity,
                                UnitPrice,
                                LineTotal
                            )
                            VALUES
                            (
                                @saleID,
                                @productID,
                                @quantity,
                                @unitPrice,
                                @lineTotal
                            )";

                                using (SQLiteCommand command =
                                       new SQLiteCommand(
                                           itemSQL,
                                           connection,
                                           transaction))
                                {
                                    command.Parameters.AddWithValue(
                                        "@saleID",
                                        saleID);

                                    command.Parameters.AddWithValue(
                                        "@productID",
                                        item.ProductID);

                                    command.Parameters.AddWithValue(
                                        "@quantity",
                                        item.Quantity);

                                    command.Parameters.AddWithValue(
                                        "@unitPrice",
                                        item.UnitPrice);

                                    command.Parameters.AddWithValue(
                                        "@lineTotal",
                                        item.LineTotal);

                                    command.ExecuteNonQuery();
                                }

                                
                                //  REDUCE STOCK
                                

                                string updateStockSQL = @"
                            UPDATE Products
                            SET StockQuantity =
                                StockQuantity - @quantity
                            WHERE ProductID = @productID";

                                using (SQLiteCommand command =
                                       new SQLiteCommand(
                                           updateStockSQL,
                                           connection,
                                           transaction))
                                {
                                    command.Parameters.AddWithValue(
                                        "@quantity",
                                        item.Quantity);

                                    command.Parameters.AddWithValue(
                                        "@productID",
                                        item.ProductID);

                                    command.ExecuteNonQuery();
                                }
                            }

                           
                            //  COMMIT
                            
                            transaction.Commit();

                            // Display actual Sale ID
                            lblSaleID.Text =
                                saleID.ToString();

                            // Update Sale ID in the grid
                            foreach (CartItem item in cartItems)
                            {
                                item.SaleID =
                                    saleID.ToString();
                            }

                            RefreshCart();
                        }
                        catch
                        {
                            transaction.Rollback();

                            throw;
                        }
                    }
                }

                MessageBox.Show(
                    "Sale completed successfully.\n\n" +
                    "Sale ID: " +
                    lblSaleID.Text +
                    "\nSale Date: " +
                    saleDate.ToString(
                        "dd/MM/yyyy HH:mm") +
                    "\nGrand Total: " +
                    totalAmount.ToString("0.00"),
                    "Sale Completed",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);

                // Reload products because stock changed
                LoadProducts();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "The sale could not be completed.\n\n" +
                    ex.Message,
                    "Sale Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        
    }

        private void txtAvailableStock_TextChanged(object sender, EventArgs e)
        {

        }

        private void FrmBilling_Load(object sender, EventArgs e)
        {

        }
    }
}
