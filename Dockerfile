FROM mcr.microsoft.com/dotnet/core/runtime:3.1 AS base
RUN apt-get update -y
RUN apt-get install -y tzdata
WORKDIR /app

FROM mcr.microsoft.com/dotnet/core/sdk:3.1 AS build
WORKDIR /src
COPY ["JackTheStudent.csproj", "./"]
RUN dotnet nuget add source https://nuget.emzi0767.com/api/v3/index.json -n dsharp
RUN dotnet restore "JackTheStudent.csproj"
COPY . .
WORKDIR "/src/."
RUN dotnet build "JackTheStudent.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "JackTheStudent.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENV TZ Europe/Berlin
ENTRYPOINT ["dotnet", "JackTheStudent.dll"]
