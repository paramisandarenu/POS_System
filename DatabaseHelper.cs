using System;
using System.Data.SQLite;
using System.IO;

namespace POS_System.Database
{
    internal static class DatabaseHelper
    {
        private static readonly string DatabasePath =
            Path.Combine(
                AppDomain.CurrentDomain.BaseDirectory,
                "POS_System.db");

        private static readonly string ConnectionString =
            $"Data Source={DatabasePath};Version=3;";

        public static SQLiteConnection GetConnection()
        {
            return new SQLiteConnection(ConnectionString);
        }
    }
}