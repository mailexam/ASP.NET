FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

WORKDIR /src

COPY ASP.NET.csproj ./
RUN dotnet restore

COPY . .
RUN dotnet publish -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:8.0

WORKDIR /app

COPY --from=build /app/publish .

ENV HTTP_HOST=0.0.0.0
ENV HTTP_PORT=8080

EXPOSE 8080

ENTRYPOINT ["dotnet", "ASP.NET.dll"]
