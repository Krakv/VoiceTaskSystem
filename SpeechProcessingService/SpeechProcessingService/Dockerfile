# See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

# This stage is used when running from VS in fast mode (Default for Debug configuration)
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base

ARG GIGACHAT_ROOT_PATH=./SpeechProcessingService/Certificates/russian_trusted_root_ca.cer
ARG GIGACHAT_SUB_PATH=./SpeechProcessingService/Certificates/russian_trusted_sub_ca.cer

RUN echo "GIGACHAT_ROOT_PATH: ${GIGACHAT_ROOT_PATH}"
RUN echo "GIGACHAT_SUB_PATH: ${GIGACHAT_SUB_PATH}"

COPY ${GIGACHAT_ROOT_PATH} /usr/local/share/ca-certificates/root.crt
COPY ${GIGACHAT_SUB_PATH} /usr/local/share/ca-certificates/sub.crt
RUN update-ca-certificates

USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

# This stage is used to build the service project
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["SpeechProcessingService/SpeechProcessingService.csproj", "SpeechProcessingService/"]
RUN dotnet restore "./SpeechProcessingService/SpeechProcessingService.csproj"
COPY . .
WORKDIR "/src/SpeechProcessingService"
RUN dotnet build "./SpeechProcessingService.csproj" -c $BUILD_CONFIGURATION -o /app/build

# This stage is used to publish the service project to be copied to the final stage
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./SpeechProcessingService.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# This stage is used in production or when running from VS in regular mode (Default when not using the Debug configuration)
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "SpeechProcessingService.dll"]