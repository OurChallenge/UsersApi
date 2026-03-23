FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
COPY ["Users.Api/Users.Api.csproj", "Users.Api/"]
COPY ["Users.Application/Users.Application.csproj", "Users.Application/"]
COPY ["Users.Domain/Users.Domain.csproj", "Users.Domain/"]
COPY ["Users.Infrastructure/Users.Infrastructure.csproj", "Users.Infrastructure/"]
RUN dotnet restore "Users.Api/Users.Api.csproj"
COPY . .
WORKDIR /src/Users.Api
RUN dotnet publish "Users.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:10.0
WORKDIR /app
EXPOSE 8080
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "Users.Api.dll"]
