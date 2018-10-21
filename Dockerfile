FROM microsoft/dotnet:2.1-sdk AS builder

WORKDIR /sourcedocekr 

COPY ./src/ZooKeepers.Server/*.csproj ./ZooKeepers.Server/

RUN dotnet restore ./ZooKeepers.Server

COPY ./src/ ./

RUN dotnet publish --output /app/ --configuration Release

FROM microsoft/dotnet:2.1-aspnetcore-runtime

WORKDIR /app

COPY --from=builder /app .

ENTRYPOINT ["dotnet", "ZooKeepers.Server.dll"]