FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG VERSION=0.0.0.0
RUN dotnet tool install -g dotnet-setversion
ENV PATH="${PATH}:/root/.dotnet/tools"
COPY ./src/ /src/
WORKDIR /src/Bazza
RUN setversion $VERSION
RUN dotnet publish "/src/Bazza/Bazza.csproj" -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:9.0
WORKDIR /app
EXPOSE 80
ENV ASPNETCORE_HTTP_PORTS=80
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "Bazza.dll"]