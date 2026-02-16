Avocado Shop – Computer Store
An online store for computers and computer components. The system allows browsing products, placing orders, managing the shopping cart, and includes an administration panel.

Technologies
ASP.NET Core MVC (.NET 9)

Entity Framework Core

ASP.NET Core Identity

SQLite

Bootstrap 5 + Bootstrap Icons

Requirements
.NET 9 SDK

Visual Studio 2022 / VS Code

Installation and running
Clone the repository:

bash
git clone https://github.com/kulermuler1/Sklep_internetowy.git
cd Avocado_shop
Restore packages:

bash
dotnet restore
Apply database migrations:

bash
dotnet ef database update
Run the application:

bash
dotnet run
The application will be available at: https://localhost:5001 or http://localhost:5000.

Database (SQLite)
Database file: Avocado_Shop.db (created automatically)

Connection string
json
"ConnectionStrings": {
  "AvocadoDBContextConnection": "Data Source=Avocado_Shop.db"
}
Test users
The application automatically creates accounts on first run:

Administrator
Email: admin@local.test

Password: Admin123

Permissions: Full access – manage products, categories, users, and orders

Regular user
New users can register using the registration form.

Permissions: Browse products, place orders, view order history

Features
Authentication and authorization
User registration and login

Two roles: Admin and User

Admin: full access to the admin panel

User: browsing, cart, placing orders

Products
Browsing products by category

Product details with description and price

Filtering by categories

Adding products to the cart

Categories (hierarchy)
Main categories: Pre‑built PCs, Components, Peripherals, Monitors

Subcategories with price ranges or product types

Browsing products within a given category

Cart
Adding and removing products

Changing quantities

Order summary

Proceeding to checkout

Orders
Placing orders with delivery details

User order history

Order details

Admin panel
Dashboard – store statistics (users, orders, products, revenue)

Users – list of users with roles, granting/revoking admin rights

Orders – all orders, changing statuses (New → Processing → Shipped → Completed)

Products – full CRUD (create, read, update, delete)

Categories – full CRUD (create, read, update, delete)

Forms with validation
Registration: Email (required), password (min. 6 characters, uppercase letter, digit)

Login: Email, password

Product: Name (required), description, price, stock, category, image URL

Order: First and last name, address, city, postal code, country

Database structure
Entities
Category

Id, Name, Description, IconClass, MinPrice, MaxPrice, ParentCategoryId

Relations: many Products, many SubCategories (hierarchy)

Product

Id, Name, Description, Price, Stock, ImageUrl, CategoryId

Relations: one Category, many OrderItems

Order

Id, UserId, OrderDate, Status, TotalPrice

Relations: many OrderItems

OrderItem

Id, OrderId, ProductId, Quantity, Price

Relations: one Order, one Product

IdentityUser (User) – ASP.NET Core Identity

Id, Email, UserName, PasswordHash, ...

Relations: many Orders

Relationships between entities
text
Category (1) → (N) Category (hierarchy – subcategories)
Category (1) → (N) Product
IdentityUser (1) → (N) Order
Order (1) → (N) OrderItem
Product (1) → (N) OrderItem
Project structure
text
Avocado_shop/
├── Areas/
│   └── Identity/                # Login/registration pages
├── Controllers/
│   ├── AdminController.cs       # Admin panel
│   ├── CartController.cs        # Cart
│   ├── CategoriesController.cs  # Categories
│   ├── CategoryAdminController.cs
│   ├── CheckoutController.cs    # Checkout
│   ├── HomeController.cs        # Home page
│   ├── OrdersController.cs      # User orders
│   ├── ProductAdminController.cs
│   └── ProductsController.cs    # Product list
├── Data/
│   └── DbInitializer.cs         # Data seeding
├── Models/
│   ├── Category.cs
│   ├── Product.cs
│   ├── Order.cs
│   └── OrderItem.cs
├── Services/
│   ├── CartService.cs           # Cart handling
│   └── OrderService.cs          # Order creation
├── Views/
├── wwwroot/
├── Program.cs
└── appsettings.json
Migrations
If migrations have not been applied yet:

bash
dotnet ef database update
Author
Educational project – Databases in the Internet and Applications course.

License
Icons: Flaticon
