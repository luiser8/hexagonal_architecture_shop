# hexagonal_architecture_shop
# add migration

dotnet ef migrations add Initial \
  --project HexagonalShop.Infrastructure/HexagonalShop.Infrastructure.csproj \
  --startup-project HexagonalShop.WebAPI/HexagonalShop.WebAPI.csproj \
  --context AppShopContext \
  --output-dir Persistence/Migrations \
  --verbose

  //20250908221114_Initial
  dotnet ef database update \
  --project HexagonalShop.Infrastructure/HexagonalShop.Infrastructure.csproj \
  --startup-project HexagonalShop.WebAPI/HexagonalShop.WebAPI.csproj \
  --context AppShopContext \
  --verbose

  dotnet ef database update 0 \
  --project HexagonalShop.Infrastructure/HexagonalShop.Infrastructure.csproj \
  --startup-project HexagonalShop.WebAPI/HexagonalShop.WebAPI.csproj \
  --context AppShopContext \
  --verbose

dotnet ef migrations remove \
  --project HexagonalShop.Infrastructure/HexagonalShop.Infrastructure.csproj \
  --startup-project HexagonalShop.WebAPI/HexagonalShop.WebAPI.csproj \
  --context AppShopContext \
  --verbose
