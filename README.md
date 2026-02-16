# Avocado Shop - Computer Store

An online store for computers and computer components. The system allows browsing products, placing orders, managing the shopping cart, and includes an admin panel.

## Technologies

- ASP.NET Core MVC (.NET 9)
- Entity Framework Core
- ASP.NET Core Identity
- SQLite
- Bootstrap 5 + Bootstrap Icons

## Requirements

- .NET 9 SDK
- Visual Studio 2022 / VS Code

## Installation & Setup

1. Clone the repository:
```bash
git clone https://github.com/kulermuler1/Sklep_internetowy.git
cd Avocado_shop
```

2. Restore packages:
```bash
dotnet restore
```

3. Apply database migrations:
```bash
dotnet ef database update
```

4. Run the application:
```bash
dotnet run
```

5. The application will be available at: `https://localhost:5001` or `http://localhost:5000`

## Database (SQLite)

Database file: `Avocado_Shop.db` (created automatically)

### Connection String
```json
"ConnectionStrings": {
  "AvocadoDBContextConnection": "Data Source=Avocado_Shop.db"
}
```

## Test Users

The application automatically creates accounts on first launch:

### Administrator
- **Email:** admin@local.test
- **Password:** Admin123
- **Permissions:** Full access - manage products, categories, users, and orders

### Regular User
New users can register via the registration form.
- **Permissions:** Browse products, place orders, view order history

## Features

### Authorization
- User registration and login
- Two roles: **Admin** and **User**
- Admin: full access to the admin panel
- User: browsing, cart, placing orders

### Products
- Browse products divided by categories
- Product detail page with description and price
- Filter by category
- Add to cart

### Categories (hierarchy)
- Main categories: Ready-made Sets, Components, Peripherals, Monitors
- Subcategories with price ranges or product types
- Browse products within a given category

### Shopping Cart
- Add/remove products
- Change quantities
- Order summary
- Proceed to checkout

### Orders
- Place orders with delivery details
- User order history
- Order details view

### Admin Panel
- **Dashboard** - store statistics (users, orders, products, revenue)
- **Users** - user list with roles, grant/revoke admin permissions
- **Orders** - all orders, change statuses (New → In Progress → Shipped → Completed)
- **Products** - CRUD (add, edit, delete)
- **Categories** - CRUD (add, edit, delete)

### Forms with Validation
- **Registration:** Email (required), Password (min. 6 characters, uppercase letter, digit)
- **Login:** Email, Password
- **Product:** Name (required), Description, Price, Stock, Category, Image URL
- **Order:** Full name, Address, City, Postal code, Country

## Database Structure

### Entities

**Category**
- Id, Name, Description, IconClass, MinPrice, MaxPrice, ParentCategoryId
- Relations: many Products, many SubCategories (hierarchy)

**Product**
- Id, Name, Description, Price, Stock, ImageUrl, CategoryId
- Relations: one Category, many OrderItems

**Order**
- Id, UserId, OrderDate, Status, TotalPrice
- Relations: many OrderItems

**OrderItem**
- Id, OrderId, ProductId, Quantity, Price
- Relations: one Order, one Product

**IdentityUser** - ASP.NET Core Identity
- Id, Email, UserName, PasswordHash, ...
- Relations: many Orders

### Entity Relationships

```
Category (1) ── (N) Category (hierarchy - subcategories)
Category (1) ── (N) Product
IdentityUser (1) ── (N) Order
Order (1) ── (N) OrderItem
Product (1) ── (N) OrderItem
```

## Project Structure

```
Avocado_shop/
├── Areas/
│   └── Identity/               # Login/registration pages
├── Controllers/
│   ├── AdminController.cs      # Admin panel
│   ├── CartController.cs       # Shopping cart
│   ├── CategoriesController.cs # Categories
│   ├── CategoryAdminController.cs
│   ├── CheckoutController.cs   # Order checkout
│   ├── HomeController.cs       # Home page
│   ├── OrdersController.cs     # User orders
│   ├── ProductAdminController.cs
│   └── ProductsController.cs   # Product listing
├── Data/
│   └── DbInitializer.cs        # Seed data
├── Models/
│   ├── Category.cs
│   ├── Product.cs
│   ├── Order.cs
│   └── OrderItem.cs
├── Services/
│   ├── CartService.cs          # Cart handling
│   └── OrderService.cs         # Order creation
├── Views/
├── wwwroot/
├── Program.cs
└── appsettings.json
```

## Migrations

If migrations have not been applied:
```bash
dotnet ef database update
```

## Author

Educational project - Internet and Application Databases.

## License

Icons: [Flaticon](https://www.flaticon.com/)
