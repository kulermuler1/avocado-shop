# Avocado Shop - Sklep Komputerowy

Sklep internetowy z komputerami i podzespo³ami komputerowymi. System umo¿liwia przegl¹danie produktów, sk³adanie zamówieñ, zarz¹dzanie koszykiem oraz panel administracyjny.

## Technologie

- ASP.NET Core MVC (.NET 9)
- Entity Framework Core
- ASP.NET Core Identity
- SQLite
- Bootstrap 5 + Bootstrap Icons

## Wymagania

- .NET 9 SDK
- Visual Studio 2022 / VS Code

## Instalacja i uruchomienie

1. Sklonuj repozytorium:
```bash
git clone https://github.com/kulermuler1/Sklep_internetowy.git
cd Avocado_shop
```

2. Przywróæ pakiety:
```bash
dotnet restore
```

3. Zastosuj migracje bazy danych:
```bash
dotnet ef database update
```

4. Uruchom aplikacjê:
```bash
dotnet run
```

5. Aplikacja bêdzie dostêpna pod adresem: `https://localhost:5001` lub `http://localhost:5000`

## Baza danych (SQLite)

Plik bazy danych: `Avocado_Shop.db` (tworzony automatycznie)

### Connection String
```json
"ConnectionStrings": {
  "AvocadoDBContextConnection": "Data Source=Avocado_Shop.db"
}
```

## Testowi u¿ytkownicy

Aplikacja automatycznie tworzy konta przy pierwszym uruchomieniu:

### Administrator
- **Email:** admin@local.test
- **Has³o:** Admin123
- **Uprawnienia:** Pe³ny dostêp - zarz¹dzanie produktami, kategoriami, u¿ytkownikami i zamówieniami

### Zwyk³y u¿ytkownik
Nowi u¿ytkownicy mog¹ siê zarejestrowaæ przez formularz rejestracji.
- **Uprawnienia:** Przegl¹danie produktów, sk³adanie zamówieñ, historia zamówieñ

## Funkcjonalnoœci

### Autoryzacja
- Rejestracja i logowanie u¿ytkowników
- Dwie role: **Admin** i **User**
- Admin: pe³ny dostêp do panelu administracyjnego
- User: przegl¹danie, koszyk, sk³adanie zamówieñ

### Produkty
- Przegl¹danie produktów z podzia³em na kategorie
- Szczegó³y produktu z opisem i cen¹
- Filtrowanie po kategoriach
- Dodawanie do koszyka

### Kategorie (hierarchia)
- Kategorie g³ówne: Gotowe zestawy, Podzespo³y, Peryferia, Monitory
- Podkategorie z przedzia³ami cenowymi lub typami produktów
- Przegl¹danie produktów w danej kategorii

### Koszyk
- Dodawanie/usuwanie produktów
- Zmiana iloœci
- Podsumowanie zamówienia
- Przejœcie do finalizacji

### Zamówienia
- Sk³adanie zamówieñ z danymi dostawy
- Historia zamówieñ u¿ytkownika
- Szczegó³y zamówienia

### Panel Administratora
- **Dashboard** - statystyki sklepu (u¿ytkownicy, zamówienia, produkty, przychód)
- **U¿ytkownicy** - lista u¿ytkowników z rolami, nadawanie/odbieranie uprawnieñ admina
- **Zamówienia** - wszystkie zamówienia, zmiana statusów (Nowe ? W realizacji ? Wys³ane ? Zakoñczone)
- **Produkty** - CRUD (dodawanie, edycja, usuwanie)
- **Kategorie** - CRUD (dodawanie, edycja, usuwanie)

### Formularze z walidacj¹
- **Rejestracja:** Email (wymagany), Has³o (min. 6 znaków, wielka litera, cyfra)
- **Logowanie:** Email, Has³o
- **Produkt:** Nazwa (wymagana), Opis, Cena, Stan magazynowy, Kategoria, URL obrazu
- **Zamówienie:** Imiê i nazwisko, Adres, Miasto, Kod pocztowy, Kraj

## Struktura bazy danych

### Encje

**Category (Kategoria)**
- Id, Name, Description, IconClass, MinPrice, MaxPrice, ParentCategoryId
- Relacje: wiele Products, wiele SubCategories (hierarchia)

**Product (Produkt)**
- Id, Name, Description, Price, Stock, ImageUrl, CategoryId
- Relacje: jedna Category, wiele OrderItems

**Order (Zamówienie)**
- Id, UserId, OrderDate, Status, TotalPrice
- Relacje: wiele OrderItems

**OrderItem (Pozycja zamówienia)**
- Id, OrderId, ProductId, Quantity, Price
- Relacje: jedno Order, jeden Product

**IdentityUser (U¿ytkownik)** - ASP.NET Core Identity
- Id, Email, UserName, PasswordHash, ...
- Relacje: wiele Orders

### Zwi¹zki miêdzy encjami

```
Category (1) ?? (N) Category (hierarchia - podkategorie)
Category (1) ?? (N) Product
IdentityUser (1) ?? (N) Order
Order (1) ?? (N) OrderItem
Product (1) ?? (N) OrderItem
```

## Struktura projektu

```
Avocado_shop/
??? Areas/
?   ??? Identity/               # Strony logowania/rejestracji
??? Controllers/
?   ??? AdminController.cs      # Panel admina
?   ??? CartController.cs       # Koszyk
?   ??? CategoriesController.cs # Kategorie
?   ??? CategoryAdminController.cs
?   ??? CheckoutController.cs   # Finalizacja zamówienia
?   ??? HomeController.cs       # Strona g³ówna
?   ??? OrdersController.cs     # Zamówienia u¿ytkownika
?   ??? ProductAdminController.cs
?   ??? ProductsController.cs   # Lista produktów
??? Data/
?   ??? DbInitializer.cs        # Seed danych
??? Models/
?   ??? Category.cs
?   ??? Product.cs
?   ??? Order.cs
?   ??? OrderItem.cs
??? Services/
?   ??? CartService.cs          # Obs³uga koszyka
?   ??? OrderService.cs         # Tworzenie zamówieñ
??? Views/
??? wwwroot/
??? Program.cs
??? appsettings.json
```

## Migracje

Jeœli migracje nie zosta³y zastosowane:
```bash
dotnet ef database update
```

## Autor

Projekt edukacyjny - Bazy Danych w Internecie i Aplikacjach.

## Licencja

Ikony: [Flaticon](https://www.flaticon.com/)
