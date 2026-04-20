# Stage 1: Build the Vue.js frontend
FROM node:22-alpine AS frontend-build
WORKDIR /app

# Install pnpm
RUN npm install -g pnpm

# Copy only package files first to leverage caching
COPY ClientApp/package.json ClientApp/pnpm-workspace.yaml ./
COPY ClientApp/apps/geniovue/package.json ./apps/geniovue/
COPY ClientApp/packages/clientapp/package.json ./packages/clientapp/

# Install dependencies (this layer will be cached unless package files change)
RUN pnpm install

# Copy the rest of the frontend files
COPY ClientApp/apps/geniovue ./apps/geniovue
COPY ClientApp/packages/clientapp/ ./packages/clientapp
COPY ClientApp/tsconfig.json ./
COPY ClientApp/vitest.config.ts ./

# Build the frontend
ENV NODE_OPTIONS=--max_old_space_size=10240
RUN pnpm run build

# Stage 2: Build the .NET backend
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS backend-build
WORKDIR /src

# Copy only project files first to leverage caching
COPY *.sln .
COPY GenioMVC/*.csproj ./GenioMVC/
COPY GenioServer/*.csproj ./GenioServer/
COPY Quidgest.Persistence/*.csproj ./Quidgest.Persistence/
COPY CSGenio.framework/*.csproj ./CSGenio.framework/
COPY CSGenio.core/*.csproj ./CSGenio.core/
COPY CSGenio.reporting/*.csproj ./CSGenio.reporting/
COPY DocPreviewService/*.csproj ./DocPreviewService/
COPY QCodeAnalysis/*.csproj ./QCodeAnalysis/

# Restore NuGet packages (this layer will be cached unless project files change)
RUN dotnet restore GenioMVC

# Copy the rest of the source code
COPY GenioMVC/ ./GenioMVC/
COPY GenioServer/ ./GenioServer/
COPY Quidgest.Persistence/ ./Quidgest.Persistence/
COPY CSGenio.framework/ ./CSGenio.framework/
COPY CSGenio.core/ ./CSGenio.core/
COPY CSGenio.reporting/ ./CSGenio.reporting/
COPY DocPreviewService/ ./DocPreviewService/
COPY QCodeAnalysis/ ./QCodeAnalysis/

# Build the application
RUN dotnet publish GenioMVC/GenioMVC.csproj -c Release -o /app/publish-linux -p:SkipClientApp=true -p:PublishProfile=linux

# Stage 3: Final image
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
EXPOSE 8080

# Fix for export utils in Genio applications
RUN apt update && apt install libgdiplus -y

# Copy the published backend
COPY --from=backend-build /app/publish-linux/ .

# Copy the built frontend
COPY --from=frontend-build /app/apps/geniovue/dist/ ./wwwroot

# Set the entry point
ENTRYPOINT ["dotnet", "GenioMVC.dll"]
