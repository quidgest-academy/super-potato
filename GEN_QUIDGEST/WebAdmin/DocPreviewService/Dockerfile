FROM mcr.microsoft.com/dotnet/framework/aspnet:4.8 AS runtime
COPY ./publish /inetpub/wwwroot
RUN mkdir -p /inetpub/wwwroot/temp