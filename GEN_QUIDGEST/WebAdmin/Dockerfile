# Stage 1: Build the Vue.js frontend
FROM node:22-alpine AS frontend-build
WORKDIR /app

# Install pnpm
RUN npm install -g pnpm

# Copy only package files first to leverage caching
COPY ClientApp/package.json ./

# Install dependencies (this layer will be cached unless package files change)
RUN pnpm install

COPY ClientApp/src ./src
COPY ClientApp/public ./public
COPY ClientApp/plugins ./plugins
COPY ClientApp/tsconfig.json ./
COPY ClientApp/vite.config.ts ./
COPY ClientApp/svgstore.config.ts ./
COPY ClientApp/index.html ./

# Build the frontend
ENV NODE_OPTIONS=--max_old_space_size=10240
RUN pnpm run build

# Stage 2: Build the .NET backend
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS backend-build
WORKDIR /src

# Copy only project files first to leverage caching
COPY *.sln .
COPY Administration/*.csproj ./Administration/
COPY ClientApp/*.esproj ./ClientApp/
COPY AdminCLI/*.csproj ./AdminCLI/
COPY DbAdmin/*.csproj ./DbAdmin/
COPY GenioServer/*.csproj ./GenioServer/
COPY Quidgest.Persistence/*.csproj ./Quidgest.Persistence/
COPY CSGenio.framework/*.csproj ./CSGenio.framework/
COPY CSGenio.core/*.csproj ./CSGenio.core/
COPY CSGenio.reporting/*.csproj ./CSGenio.reporting/

# Restore NuGet packages (this layer will be cached unless project files change)
RUN dotnet restore Administration/Administration.csproj 
RUN dotnet restore AdminCLI

# Copy the rest of the source code
COPY Administration/ ./Administration/
COPY AdminCLI/ ./AdminCLI/
COPY DbAdmin/ ./DbAdmin/
COPY GenioServer/ ./GenioServer/
COPY Quidgest.Persistence/ ./Quidgest.Persistence/
COPY CSGenio.framework/ ./CSGenio.framework/
COPY CSGenio.core/ ./CSGenio.core/
COPY CSGenio.reporting/ ./CSGenio.reporting/

# Build the application
RUN dotnet publish Administration/Administration.csproj -c Release -o /app/publish-linux -p:SkipClientApp=true -p:PublishProfile=linux
RUN dotnet publish AdminCLI/AdminCLI.csproj -c Release -o /app/publish-linux -p:PublishProfile=linux

# Stage 3: Run
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080

# Fix for export utils in Genio applications
RUN apt update && apt install libgdiplus -y

# Copy the published backend
COPY --from=backend-build /app/publish-linux/ .

# Copy the built frontend
COPY --from=frontend-build /app/dist/ ./wwwroot

# Set the entry point
ENTRYPOINT ["dotnet", "Administration.dll"]