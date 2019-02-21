FROM microsoft/dotnet:2.1-aspnetcore-runtime AS base
WORKDIR /app
EXPOSE 80

FROM microsoft/dotnet:2.1-sdk AS build
COPY Softeq.NetKit.Chat.Web/Softeq.NetKit.Chat.Web.csproj Softeq.NetKit.Chat.Web/
RUN dotnet restore Softeq.NetKit.Chat.Web/Softeq.NetKit.Chat.Web.csproj
COPY . .
WORKDIR /Softeq.NetKit.Chat.Web
RUN dotnet build Softeq.NetKit.Chat.Web.csproj -c Debug -o /app

FROM build AS publish
RUN dotnet publish Softeq.NetKit.Chat.Web.csproj -c Debug -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "Softeq.NetKit.Chat.Web.dll"]
