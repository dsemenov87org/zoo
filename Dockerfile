FROM microsoft/aspnetcore-build AS builder

WORKDIR /source

COPY ./src/ZooKeepers.Server/*.csproj ./ZooKeepers.Server/

RUN dotnet restore

COPY .src/ ./

RUN dotnet publish --output /app/ --configuration Release

FROM microsoft/aspnetcore

WORKDIR /app

COPY --from=builder /app .

ENTRYPOINT ["dotnet", "ZooKeepers.Server.dll"]