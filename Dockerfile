FROM mcr.microsoft.com/dotnet/core/aspnet:3.1 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/core/sdk:3.1 AS build
COPY ./src/ /src/
RUN dotnet restore "/src/Adliance.Website/Adliance.Website.csproj"
RUN dotnet build "/src/Adliance.Website/Adliance.Website.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "/src/Adliance.Website/Adliance.Website.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Adliance.Website.dll"]