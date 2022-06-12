#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/runtime:6.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["FelicityOne/FelicityOne.csproj", "FelicityOne/"]
RUN dotnet restore "FelicityOne/FelicityOne.csproj"
COPY . .
WORKDIR "/src/FelicityOne"
RUN dotnet build "FelicityOne.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "FelicityOne.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENV DOTNET_EnableDiagnostics=0
ENTRYPOINT ["dotnet", "FelicityOne.dll"]
