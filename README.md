# FitLife MembershipService

MembershipService er en microservice i FitLife-platformen, der håndterer medlemsprofiler, medlemsstatus og brugerpræferencer.

## Formål

Servicen har ansvar for funktioner relateret til medlemskab, herunder oprettelse af medlem, opdatering af profil, opdatering af præferencer, pause af medlemskab og opsigelse af medlemskab.

## Teknologier

- .NET 10 Web API
- xUnit
- FluentAssertions
- Docker
- Swagger/OpenAPI
- MongoDB Driver
- RabbitMQ Client
- Serilog

## Projektstruktur

```txt
Membership/
├── FitLife.Membership.Api/
├── FitLife.Membership.Tests/
├── MembershipService.sln
└── README.md