FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
ARG VERSION=0.0.0.0
RUN dotnet tool install -g dotnet-setversion
COPY ./src/ /src/
RUN setversion $VERSION
RUN dotnet restore "/src/Bazza/Bazza.csproj"
RUN dotnet build "/src/Bazza/Bazza.csproj" -c Release -o /app/build
RUN dotnet publish "/src/Bazza/Bazza.csproj" -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:5.0
WORKDIR /app
EXPOSE 80
EXPOSE 443
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "Bazza.dll"]