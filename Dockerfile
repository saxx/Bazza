FROM mcr.microsoft.com/dotnet/sdk:7.0 AS setversion
ARG VERSION=0.0.0.0
RUN dotnet tool install -g dotnet-setversion
ENV PATH="${PATH}:/root/.dotnet/tools"
COPY ./src/ /src/
WORKDIR /src/Bazza
RUN setversion $VERSION

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
COPY --from=setversion /src/ /src/
RUN dotnet restore "/src/Bazza/Bazza.csproj"
RUN dotnet build "/src/Bazza/Bazza.csproj" -c Release -o /app/build
RUN dotnet publish "/src/Bazza/Bazza.csproj" -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
EXPOSE 80
ENV ASPNETCORE_HTTP_PORTS=80
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "Bazza.dll"]