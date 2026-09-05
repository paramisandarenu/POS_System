# POS System

A desktop Point of Sale (POS) System developed using C# Windows Forms and a database. The system is designed to manage products, categories, customers, sales, billing, and stock efficiently.

## 📌 Project Overview

The POS System provides the main functions required for a small retail business. Users can manage products and categories, maintain customer information, create sales bills, and view sales history.

The system also validates product quantities and available stock before completing a sale.

## 🚀 Features

### Welcome Screen
- Professional welcome interface
- Navigation to the POS system
- Exit option

### Dashboard
- Main navigation menu
- Access to all major system modules

### Category Management
- Add categories
- Update categories
- Delete categories
- View category information

### Product Management
- Add products
- Update products
- Delete products
- View product information
- Manage product prices and stock

### Customer Management
- Add customer information
- Update customer information
- Delete customer information
- View customer records

### Billing / Sales
- Search and select products
- Display product details
- Enter quantity
- Validate available stock
- Add products to the bill/cart
- Remove products from the bill/cart
- Calculate subtotal
- Calculate grand total
- Generate a unique Sale ID
- Record sale date
- Save completed sales
- Reduce stock after completing a sale

### Sales History
- View completed sales
- View sale information
- View purchased items and quantities
- View sale totals

## ✅ Validation

The system includes the following validation rules:

- Quantity must be greater than zero.
- Quantity cannot exceed available stock.
- A bill cannot be completed without at least one item.
- Required information must be entered before saving records.

## 🛠️ Technologies Used

- C#
- Windows Forms
- .NET Framework
- SQLite / System.Data.SQLite
- Visual Studio
- GitHub

## 🗄️ Database

The system uses a database to store information such as:

- Categories
- Products
- Customers
- Sales
- Sale items
- Stock information

## 📂 Project Structure

POS_System/
│
├── POS_System.sln
│
├── POS_System/
│   ├── Forms
│   ├── Database
│   ├── Images
│   ├── Properties
│   ├── Program.cs
│   ├── App.config
│   └── POS_System.csproj
│
├── Screenshots/
├── PPT/
└── Documentation/

🖥️ User Interface

The system contains the following main screens:

Welcome Screen
Dashboard
Category Management
Product Management
Customer Management
Billing / Sales
Sales History

The interface uses consistent buttons, readable labels, appropriate spacing, and simple navigation.

📸 Screenshots

Screenshots of the system are available in the Screenshots folder.

📋 Optional Features

The following features are not currently included:

Login authentication
Receipt preview
Bill printing
Discount calculation
Tax calculation
Low-stock alert
Dashboard statistics
Dark mode
Product image support
Advanced search filters

🎯 Purpose

This project was developed as an academic POS system project to demonstrate practical knowledge of C# Windows Forms, database management, CRUD operations, validation, billing, and stock management.

👩‍💻 Developer

Parami Sandarenu

📄 License

This project was developed for educational purposes.
