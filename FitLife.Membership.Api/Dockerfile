FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY FitLife.Membership.slnx .
COPY FitLife.Membership.Api/FitLife.Membership.Api.csproj FitLife.Membership.Api/
COPY FitLife.Membership.Tests/FitLife.Membership.Tests.csproj FitLife.Membership.Tests/

RUN dotnet restore FitLife.Membership.slnx

COPY . .

RUN dotnet test FitLife.Membership.Tests/FitLife.Membership.Tests.csproj --no-restore

RUN dotnet publish FitLife.Membership.Api/FitLife.Membership.Api.csproj -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app

COPY --from=build /app/publish .

ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

ENTRYPOINT ["dotnet", "FitLife.Membership.Api.dll"]
