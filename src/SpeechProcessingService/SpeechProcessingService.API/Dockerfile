FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base

WORKDIR /app
EXPOSE 8080
EXPOSE 8081

RUN apt-get update && apt-get install -y \
    libgomp1 \
    libstdc++6 \
    ffmpeg \
    && rm -rf /var/lib/apt/lists/*

COPY SpeechProcessingService.API/Certificates/russian_trusted_root_ca.cer /usr/local/share/ca-certificates/root.crt
COPY SpeechProcessingService.API/Certificates/russian_trusted_sub_ca.cer /usr/local/share/ca-certificates/sub.crt
RUN update-ca-certificates

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

COPY ["SpeechProcessingService.API/SpeechProcessingService.API.csproj", "SpeechProcessingService.API/"]
RUN dotnet restore "SpeechProcessingService.API/SpeechProcessingService.API.csproj"

COPY . .

WORKDIR "/src/SpeechProcessingService.API"
ARG BUILD_CONFIGURATION=Release
RUN dotnet build "SpeechProcessingService.API.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "SpeechProcessingService.API.csproj" -c $BUILD_CONFIGURATION -r linux-x64 --self-contained false -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

ENTRYPOINT ["dotnet", "SpeechProcessingService.API.dll"]