using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SQLite;

namespace POS_System.Database
{
    
        internal static class DatabaseInitializer
        {
            public static void InitializeDatabase()
            {
                using (SQLiteConnection connection =
                       DatabaseHelper.GetConnection())
                {
                    connection.Open();

                    string sql = @"
                    PRAGMA foreign_keys = ON;

                    CREATE TABLE IF NOT EXISTS Categories
                    (
                        CategoryID INTEGER PRIMARY KEY AUTOINCREMENT,
                        CategoryName TEXT NOT NULL UNIQUE,
                        Description TEXT
                    );

                    CREATE TABLE IF NOT EXISTS Products
                    (
                        ProductID INTEGER PRIMARY KEY AUTOINCREMENT,
                        ProductName TEXT NOT NULL,
                        CategoryID INTEGER NOT NULL,
                        UnitPrice REAL NOT NULL,
                        StockQuantity INTEGER NOT NULL,

                        FOREIGN KEY (CategoryID)
                            REFERENCES Categories(CategoryID)
                    );

                    CREATE TABLE IF NOT EXISTS Customers
                    (
                        CustomerID INTEGER PRIMARY KEY AUTOINCREMENT,
                        CustomerName TEXT NOT NULL,
                        ContactNumber TEXT,
                        Address TEXT
                    );

                    CREATE TABLE IF NOT EXISTS Sales
                    (
                        SaleID INTEGER PRIMARY KEY AUTOINCREMENT,
                        SaleDate TEXT NOT NULL,
                        CustomerID INTEGER,
                        TotalAmount REAL NOT NULL,

                        FOREIGN KEY (CustomerID)
                            REFERENCES Customers(CustomerID)
                    );

                    CREATE TABLE IF NOT EXISTS SaleItems
                    (
                        SaleItemID INTEGER PRIMARY KEY AUTOINCREMENT,
                        SaleID INTEGER NOT NULL,
                        ProductID INTEGER NOT NULL,
                        Quantity INTEGER NOT NULL,
                        UnitPrice REAL NOT NULL,
                        LineTotal REAL NOT NULL,

                        FOREIGN KEY (SaleID)
                            REFERENCES Sales(SaleID),

                        FOREIGN KEY (ProductID)
                            REFERENCES Products(ProductID)
                    );
                ";

                    using (SQLiteCommand command =
                           new SQLiteCommand(sql, connection))
                    {
                        command.ExecuteNonQuery();
                    }
                }
            }
        }
    
}
