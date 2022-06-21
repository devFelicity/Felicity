FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src

COPY *.sln .
COPY Felicity/*.csproj ./Felicity/
RUN dotnet restore

COPY Felicity/. ./Felicity/
WORKDIR /src/Felicity
RUN dotnet publish -c release -o /app --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:6.0
WORKDIR /app
COPY --from=build /app ./
ENV DOTNET_EnableDiagnostics=0
ENTRYPOINT ["dotnet", "Felicity.dll"]
