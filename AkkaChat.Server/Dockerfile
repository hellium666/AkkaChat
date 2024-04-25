FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["AkkaChat.Server/AkkaChat.Server.csproj", "AkkaChat.Server/"]
RUN dotnet restore "AkkaChat.Server/AkkaChat.Server.csproj"
COPY . .
WORKDIR "/src/AkkaChat.Server"
RUN dotnet build "AkkaChat.Server.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "AkkaChat.Server.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "AkkaChat.Server.dll"]
