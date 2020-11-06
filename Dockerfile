FROM mcr.microsoft.com/dotnet/core/runtime:3.1 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/core/sdk:3.1 AS build
WORKDIR /src
COPY ["JackTheStudent.csproj", "./"]
RUN dotnet restore "JackTheStudent.csproj"
COPY . .
WORKDIR "/src/."
RUN dotnet build "JackTheStudent.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "JackTheStudent.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "JackTheStudent.dll"]
