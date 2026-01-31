# Sử dụng image .NET 8 SDK để build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy file csproj và restore dependencies
COPY ["PCM.API.csproj", "./"]
RUN dotnet restore "PCM.API.csproj"

# Copy toàn bộ source code và build
COPY . .
RUN dotnet publish "PCM.API.csproj" -c Release -o /app/publish

# Sử dụng image .NET 8 ASP.NET để chạy (nhẹ hơn SDK)
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=build /app/publish .

# Cấu hình port cho Render (Render dùng biến môi trường PORT)
ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

ENTRYPOINT ["dotnet", "PCM.API.dll"]