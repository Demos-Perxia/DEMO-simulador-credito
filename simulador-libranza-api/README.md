# Cuota Clara API

API .NET 8 por capas para el simulador mock de libranza. No persiste solicitudes ni usa servicios externos.

## Requisitos

- .NET SDK 10.0 o superior.

## Comandos

```powershell
dotnet restore CuotaClara.sln
dotnet build CuotaClara.sln
dotnet run --project src/CuotaClara.Api
dotnet test CuotaClara.sln
```

La API expone `GET /api/v1/catalogs/activities`, `GET /api/v1/catalogs/activities/{activityId}/agreements` y `POST /api/v1/credit-simulations`. En desarrollo publica OpenAPI en `GET /swagger/v1/swagger.json` y Swagger UI en `/swagger`.

`POST /api/v1/credit-simulations` recibe `mode` (`AMOUNT` o `INSTALLMENT_CAPACITY`), `activityId`, `agreementId`, `monthlyIncomeCop`, `payrollDeductionsCop` y exactamente uno de `requestedAmountCop` o `maximumInstallmentCop`.

La política mock configurable en `appsettings.json` usa 18 % EA, COP, plazos 60/72/96/108/120, rango de 1.000.000 a 100.000.000 COP y capacidad máxima del 40 % del ingreso disponible. Los errores controlados devuelven `application/problem+json`. CORS permite explícitamente los orígenes locales configurados.
