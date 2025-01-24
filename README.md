# ProductMVCApp
Iztsrādē tika lietots Visual Studio 2022 un SQL Server Management Studio

1. Instalē .NET 8.0 savā IDE
2. Lejupielādē projektu un atver to savā IDE
3. Atver Package Manager Console un ieraksti "dotnet restore"
4. appsettings.json failā nomaini "DefaultConnection" virknē "Server" uz savu datu bāzes servera nosaukumu (noklusējuma ir "(localdb)\\MSSQLLocalDB")
5. Uzspied ar labo peles klikšķi uz "TotalPriceVatCalcTest" un uzspied "Unload Project", ja tas jau nav izdarīts
6. Package Manager Console ieraksti "Add-Migration x" (x vietā var likt jebko)
7. Package Manager Console ieraksti "Update-Database"
8. Palaid projektu
