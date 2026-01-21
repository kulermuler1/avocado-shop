using Avocado_shop.Areas.Identity.Data;
using Avocado_shop.Models;
using Microsoft.AspNetCore.Identity;

namespace Avocado_shop.Data
{
    public static class DbInitializer
    {
        public static async Task SeedAsync(IServiceProvider serviceProvider, IConfiguration config)
        {
            var userManager = serviceProvider.GetRequiredService<UserManager<IdentityUser>>();
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var db = serviceProvider.GetRequiredService<AvocadoDBContext>();

            // Seed roles
            string[] roles = new[] { "Admin", "User" };
            foreach (var r in roles)
            {
                if (!await roleManager.RoleExistsAsync(r))
                    await roleManager.CreateAsync(new IdentityRole(r));
            }

            // Seed admin user
            var adminEmail = config["Seed:AdminEmail"];
            var adminPass = config["Seed:AdminPassword"];
            if (!string.IsNullOrEmpty(adminEmail) && !string.IsNullOrEmpty(adminPass))
            {
                var admin = await userManager.FindByEmailAsync(adminEmail);
                if (admin == null)
                {
                    admin = new IdentityUser 
                    { 
                        UserName = adminEmail, 
                        Email = adminEmail, 
                        EmailConfirmed = true 
                    };
                    var res = await userManager.CreateAsync(admin, adminPass);
                    if (res.Succeeded) await userManager.AddToRoleAsync(admin, "Admin");
                }
            }

            // Seed categories and products if empty
            if (!db.Categories.Any())
            {
                // ========== G£ÓWNE KATEGORIE ==========

                var zestawy = new Category
                {
                    Name = "Gotowe zestawy komputerowe",
                    Description = "Kompletne zestawy PC gotowe do pracy i grania",
                    IconClass = "bi-pc-display"
                };

                var podzespoly = new Category
                {
                    Name = "Podzespo³y komputerowe",
                    Description = "Procesory, karty graficzne, p³yty g³ówne i pamiêæ RAM",
                    IconClass = "bi-cpu"
                };

                var peryferia = new Category
                {
                    Name = "Peryferia",
                    Description = "Myszki, klawiatury i s³uchawki",
                    IconClass = "bi-mouse"
                };

                var monitory = new Category
                {
                    Name = "Monitory",
                    Description = "Monitory gamingowe i do pracy",
                    IconClass = "bi-display"
                };

                db.Categories.AddRange(zestawy, podzespoly, peryferia, monitory);
                await db.SaveChangesAsync();

                // ========== PODKATEGORIE - ZESTAWY (wg ceny) ==========
                var zestawyDo3000 = new Category
                {
                    Name = "Do 3000 z³",
                    Description = "Bud¿etowe zestawy komputerowe",
                    ParentCategoryId = zestawy.Id,
                    MaxPrice = 3000
                };
                var zestawy3000do5000 = new Category
                {
                    Name = "3000 - 5000 z³",
                    Description = "Zestawy ze œredniej pó³ki",
                    ParentCategoryId = zestawy.Id,
                    MinPrice = 3000,
                    MaxPrice = 5000
                };
                var zestawyPowyzej5000 = new Category
                {
                    Name = "Powy¿ej 5000 z³",
                    Description = "Wydajne zestawy gamingowe i profesjonalne",
                    ParentCategoryId = zestawy.Id,
                    MinPrice = 5000
                };

                // ========== PODKATEGORIE - PODZESPO£Y ==========
                var procesory = new Category
                {
                    Name = "Procesory",
                    Description = "Procesory Intel i AMD",
                    ParentCategoryId = podzespoly.Id,
                    IconClass = "bi-cpu-fill"
                };
                var kartyGraficzne = new Category
                {
                    Name = "Karty graficzne",
                    Description = "Karty graficzne NVIDIA i AMD",
                    ParentCategoryId = podzespoly.Id,
                    IconClass = "bi-gpu-card"
                };
                var plytyGlowne = new Category
                {
                    Name = "P³yty g³ówne",
                    Description = "P³yty g³ówne do procesorów Intel i AMD",
                    ParentCategoryId = podzespoly.Id,
                    IconClass = "bi-motherboard"
                };
                var pamiecRam = new Category
                {
                    Name = "Pamiêæ RAM",
                    Description = "Pamiêci DDR4 i DDR5",
                    ParentCategoryId = podzespoly.Id,
                    IconClass = "bi-memory"
                };

                // ========== PODKATEGORIE - PERYFERIA ==========
                var myszki = new Category
                {
                    Name = "Myszki",
                    Description = "Myszki gamingowe i biurowe",
                    ParentCategoryId = peryferia.Id,
                    IconClass = "bi-mouse-fill"
                };
                var klawiatury = new Category
                {
                    Name = "Klawiatury",
                    Description = "Klawiatury mechaniczne i membranowe",
                    ParentCategoryId = peryferia.Id,
                    IconClass = "bi-keyboard"
                };
                var sluchawki = new Category
                {
                    Name = "S³uchawki",
                    Description = "S³uchawki gamingowe i muzyczne",
                    ParentCategoryId = peryferia.Id,
                    IconClass = "bi-headphones"
                };

                // ========== PODKATEGORIE - MONITORY (wg ceny) ==========
                var monitoryDo800 = new Category
                {
                    Name = "Do 800 z³",
                    Description = "Bud¿etowe monitory",
                    ParentCategoryId = monitory.Id,
                    MaxPrice = 800
                };
                var monitory800do1500 = new Category
                {
                    Name = "800 - 1500 z³",
                    Description = "Monitory ze œredniej pó³ki",
                    ParentCategoryId = monitory.Id,
                    MinPrice = 800,
                    MaxPrice = 1500
                };
                var monitoryPowyzej1500 = new Category
                {
                    Name = "Powy¿ej 1500 z³",
                    Description = "Monitory premium",
                    ParentCategoryId = monitory.Id,
                    MinPrice = 1500
                };

                db.Categories.AddRange(
                    zestawyDo3000, zestawy3000do5000, zestawyPowyzej5000,
                    procesory, kartyGraficzne, plytyGlowne, pamiecRam,
                    myszki, klawiatury, sluchawki,
                    monitoryDo800, monitory800do1500, monitoryPowyzej1500
                );
                await db.SaveChangesAsync();

                // ========== PRODUKTY (bez obrazków - bêd¹ placeholdery) ==========
                db.Products.AddRange(
                    // Zestawy komputerowe
                    new Product
                    {
                        Name = "Zestaw PC Office Basic",
                        Description = "Komputer do pracy biurowej. Intel Core i3, 8GB RAM, SSD 256GB. Idealny do dokumentów, przegl¹dania internetu i podstawowych zadañ.",
                        Price = 1999.00m,
                        Stock = 5,
                        CategoryId = zestawyDo3000.Id
                    },
                    new Product
                    {
                        Name = "Zestaw PC Gaming Entry",
                        Description = "Bud¿etowy komputer gamingowy. AMD Ryzen 3, GTX 1650, 16GB RAM, SSD 512GB. Gry w Full HD na œrednich ustawieniach.",
                        Price = 2799.00m,
                        Stock = 4,
                        CategoryId = zestawyDo3000.Id
                    },
                    new Product
                    {
                        Name = "Zestaw PC Gaming Mid",
                        Description = "Komputer gamingowy œredniej klasy. AMD Ryzen 5 5600X, RTX 4060, 16GB RAM, SSD 512GB. P³ynna rozgrywka w Full HD/QHD.",
                        Price = 4299.00m,
                        Stock = 3,
                        CategoryId = zestawy3000do5000.Id
                    },
                    new Product
                    {
                        Name = "Zestaw PC Creator",
                        Description = "Komputer do pracy kreatywnej. Intel Core i5-13400F, RTX 4060 Ti, 32GB RAM, SSD 1TB. Monta¿ wideo, grafika 3D.",
                        Price = 4899.00m,
                        Stock = 2,
                        CategoryId = zestawy3000do5000.Id
                    },
                    new Product
                    {
                        Name = "Zestaw PC Gaming Pro",
                        Description = "Wydajny komputer gamingowy. Intel Core i7-13700KF, RTX 4070 Super, 32GB DDR5, SSD 1TB NVMe. 4K gaming ready.",
                        Price = 6999.00m,
                        Stock = 2,
                        CategoryId = zestawyPowyzej5000.Id
                    },
                    new Product
                    {
                        Name = "Zestaw PC Extreme",
                        Description = "Topowy komputer gamingowy. AMD Ryzen 9 7950X, RTX 4080 Super, 64GB DDR5, SSD 2TB NVMe. Bez kompromisów.",
                        Price = 12999.00m,
                        Stock = 1,
                        CategoryId = zestawyPowyzej5000.Id
                    },

                    // Procesory
                    new Product
                    {
                        Name = "Intel Core i5-13400F",
                        Description = "Procesor 10 rdzeni (6P+4E), 16 w¹tków, 2.5-4.6 GHz, 20MB Cache. Socket LGA1700. Œwietny stosunek ceny do wydajnoœci.",
                        Price = 749.00m,
                        Stock = 15,
                        CategoryId = procesory.Id
                    },
                    new Product
                    {
                        Name = "AMD Ryzen 5 7600X",
                        Description = "Procesor 6 rdzeni, 12 w¹tków, 4.7-5.3 GHz, 32MB Cache. Socket AM5. Architektura Zen 4, PCIe 5.0.",
                        Price = 899.00m,
                        Stock = 10,
                        CategoryId = procesory.Id
                    },
                    new Product
                    {
                        Name = "Intel Core i7-13700KF",
                        Description = "Procesor 16 rdzeni (8P+8E), 24 w¹tki, 3.4-5.4 GHz, 30MB Cache. Socket LGA1700. Odblokowany mno¿nik.",
                        Price = 1599.00m,
                        Stock = 8,
                        CategoryId = procesory.Id
                    },
                    new Product
                    {
                        Name = "AMD Ryzen 7 7800X3D",
                        Description = "Procesor 8 rdzeni, 16 w¹tków, 4.2-5.0 GHz, 96MB Cache 3D V-Cache. Najlepszy procesor do gier.",
                        Price = 1799.00m,
                        Stock = 5,
                        CategoryId = procesory.Id
                    },

                    // Karty graficzne
                    new Product
                    {
                        Name = "NVIDIA GeForce RTX 4060",
                        Description = "8GB GDDR6, Ray Tracing, DLSS 3, 2460 MHz Boost. Wydajna karta do grania w Full HD i QHD.",
                        Price = 1399.00m,
                        Stock = 8,
                        CategoryId = kartyGraficzne.Id
                    },
                    new Product
                    {
                        Name = "AMD Radeon RX 7600",
                        Description = "8GB GDDR6, FSR 3, 2655 MHz Boost. Konkurencyjna wydajnoœæ w Full HD gaming.",
                        Price = 1199.00m,
                        Stock = 6,
                        CategoryId = kartyGraficzne.Id
                    },
                    new Product
                    {
                        Name = "NVIDIA GeForce RTX 4070 Super",
                        Description = "12GB GDDR6X, Ray Tracing, DLSS 3, 2480 MHz Boost. Œwietna do QHD i 4K gaming.",
                        Price = 2699.00m,
                        Stock = 4,
                        CategoryId = kartyGraficzne.Id
                    },
                    new Product
                    {
                        Name = "AMD Radeon RX 7800 XT",
                        Description = "16GB GDDR6, FSR 3, 2430 MHz Boost. Topowa wydajnoœæ w QHD gaming.",
                        Price = 2199.00m,
                        Stock = 5,
                        CategoryId = kartyGraficzne.Id
                    },

                    // P³yty g³ówne
                    new Product
                    {
                        Name = "MSI MAG B650 TOMAHAWK WiFi",
                        Description = "P³yta g³ówna AMD AM5, DDR5, PCIe 5.0, WiFi 6E, 2.5G LAN. ATX. Solidna podstawa dla Ryzen 7000.",
                        Price = 899.00m,
                        Stock = 7,
                        CategoryId = plytyGlowne.Id
                    },
                    new Product
                    {
                        Name = "ASUS ROG STRIX B760-F Gaming WiFi",
                        Description = "P³yta g³ówna Intel LGA1700, DDR5, PCIe 5.0, WiFi 6, 2.5G LAN. ATX. Premium dla Intel 12/13/14 gen.",
                        Price = 999.00m,
                        Stock = 5,
                        CategoryId = plytyGlowne.Id
                    },
                    new Product
                    {
                        Name = "Gigabyte B550 AORUS Elite V2",
                        Description = "P³yta g³ówna AMD AM4, DDR4, PCIe 4.0, 2.5G LAN. ATX. Œwietna dla Ryzen 5000.",
                        Price = 549.00m,
                        Stock = 10,
                        CategoryId = plytyGlowne.Id
                    },

                    // Pamiêæ RAM
                    new Product
                    {
                        Name = "Kingston Fury Beast 32GB DDR5",
                        Description = "2x16GB, 5200MHz, CL40, XMP 3.0. Szybka pamiêæ DDR5 dla nowych platform.",
                        Price = 449.00m,
                        Stock = 20,
                        CategoryId = pamiecRam.Id
                    },
                    new Product
                    {
                        Name = "G.Skill Trident Z5 RGB 32GB DDR5",
                        Description = "2x16GB, 6000MHz, CL36, XMP 3.0, RGB. Premium pamiêæ DDR5 z podœwietleniem.",
                        Price = 649.00m,
                        Stock = 12,
                        CategoryId = pamiecRam.Id
                    },
                    new Product
                    {
                        Name = "Corsair Vengeance 16GB DDR4",
                        Description = "2x8GB, 3200MHz, CL16, XMP 2.0. Sprawdzona pamiêæ DDR4 w dobrej cenie.",
                        Price = 189.00m,
                        Stock = 30,
                        CategoryId = pamiecRam.Id
                    },

                    // Myszki
                    new Product
                    {
                        Name = "Logitech G502 X Plus",
                        Description = "Bezprzewodowa myszka gamingowa, sensor HERO 25K, 25600 DPI, LIGHTSPEED, RGB. 106g.",
                        Price = 549.00m,
                        Stock = 15,
                        CategoryId = myszki.Id
                    },
                    new Product
                    {
                        Name = "Razer DeathAdder V3",
                        Description = "Ergonomiczna myszka gamingowa, sensor Focus Pro 30K, 30000 DPI, 90 mln klikniêæ. 59g.",
                        Price = 349.00m,
                        Stock = 20,
                        CategoryId = myszki.Id
                    },
                    new Product
                    {
                        Name = "SteelSeries Rival 3",
                        Description = "Bud¿etowa myszka gamingowa, sensor TrueMove Core, 8500 DPI, RGB. 77g. Œwietna na start.",
                        Price = 129.00m,
                        Stock = 25,
                        CategoryId = myszki.Id
                    },

                    // Klawiatury
                    new Product
                    {
                        Name = "SteelSeries Apex Pro TKL",
                        Description = "Klawiatura mechaniczna TKL, regulowane switche OmniPoint 2.0, OLED ekran, RGB. Premium.",
                        Price = 799.00m,
                        Stock = 8,
                        CategoryId = klawiatury.Id
                    },
                    new Product
                    {
                        Name = "Razer BlackWidow V4",
                        Description = "Klawiatura mechaniczna, switche Razer Green, RGB Chroma, multimedia dial. Full-size.",
                        Price = 699.00m,
                        Stock = 10,
                        CategoryId = klawiatury.Id
                    },
                    new Product
                    {
                        Name = "Logitech G413 SE",
                        Description = "Klawiatura mechaniczna, switche taktyczne, bia³e podœwietlenie. Solidna i prosta.",
                        Price = 279.00m,
                        Stock = 15,
                        CategoryId = klawiatury.Id
                    },

                    // S³uchawki
                    new Product
                    {
                        Name = "HyperX Cloud III Wireless",
                        Description = "Bezprzewodowe s³uchawki gamingowe, DTS Headphone:X, 120h baterii, mikrofon. Komfort.",
                        Price = 649.00m,
                        Stock = 12,
                        CategoryId = sluchawki.Id
                    },
                    new Product
                    {
                        Name = "SteelSeries Arctis Nova 7",
                        Description = "Bezprzewodowe s³uchawki gamingowe, 360° Spatial Audio, 38h baterii, dual wireless.",
                        Price = 799.00m,
                        Stock = 8,
                        CategoryId = sluchawki.Id
                    },
                    new Product
                    {
                        Name = "Razer Kraken X",
                        Description = "Przewodowe s³uchawki gamingowe, 7.1 Surround, ultralekkie 250g. Bud¿etowa opcja.",
                        Price = 199.00m,
                        Stock = 20,
                        CategoryId = sluchawki.Id
                    },

                    // Monitory
                    new Product
                    {
                        Name = "AOC 24G2SPU",
                        Description = "Monitor 23.8\", IPS, Full HD, 165Hz, 1ms, FreeSync Premium. Œwietny do gier w bud¿ecie.",
                        Price = 599.00m,
                        Stock = 12,
                        CategoryId = monitoryDo800.Id
                    },
                    new Product
                    {
                        Name = "iiyama G-Master G2470HSU",
                        Description = "Monitor 23.8\", IPS, Full HD, 165Hz, 0.8ms, FreeSync Premium. Solidny i tani.",
                        Price = 549.00m,
                        Stock = 15,
                        CategoryId = monitoryDo800.Id
                    },
                    new Product
                    {
                        Name = "LG 27GP850P-B",
                        Description = "Monitor 27\", Nano IPS, QHD 2560x1440, 180Hz, 1ms, G-Sync Compatible. Œwietne kolory.",
                        Price = 1299.00m,
                        Stock = 6,
                        CategoryId = monitory800do1500.Id
                    },
                    new Product
                    {
                        Name = "ASUS VG27AQ1A",
                        Description = "Monitor 27\", IPS, QHD 2560x1440, 170Hz, 1ms, G-Sync Compatible, HDR10.",
                        Price = 1099.00m,
                        Stock = 8,
                        CategoryId = monitory800do1500.Id
                    },
                    new Product
                    {
                        Name = "Samsung Odyssey G7 32\"",
                        Description = "Monitor 32\", VA 1000R, QHD, 240Hz, 1ms, G-Sync/FreeSync Premium Pro. Zakrzywiony.",
                        Price = 1899.00m,
                        Stock = 4,
                        CategoryId = monitoryPowyzej1500.Id
                    },
                    new Product
                    {
                        Name = "LG 27GR95QE-B OLED",
                        Description = "Monitor 27\", OLED, QHD, 240Hz, 0.03ms, G-Sync Compatible. Perfekcyjna czerñ i kolory.",
                        Price = 3499.00m,
                        Stock = 2,
                        CategoryId = monitoryPowyzej1500.Id
                    }
                );
                await db.SaveChangesAsync();
            }
        }
    }
}
