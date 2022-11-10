FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["DictionaryBot/DictionaryBot.csproj", "DictionaryBot/"]
COPY ["DatabaseAccess/DatabaseAccess.csproj", "DatabaseAccess/"]
COPY ["DictionaryApiAccess/DictionaryApiAccess.csproj", "DictionaryApiAccess/"]
RUN dotnet restore "DictionaryBot/DictionaryBot.csproj"
COPY . .
WORKDIR "/src/DictionaryBot"
RUN dotnet build "DictionaryBot.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "DictionaryBot.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "DictionaryBot.dll"]