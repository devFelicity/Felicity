#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY ["Felicity/Felicity.csproj", "Felicity/"]
RUN dotnet restore "Felicity/Felicity.csproj"
COPY . .
WORKDIR "/src/Felicity"
RUN dotnet build "Felicity.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Felicity.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Felicity.dll"]
