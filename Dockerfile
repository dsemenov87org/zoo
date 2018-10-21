FROM microsoft/aspnetcore

WORKDIR /app

COPY /out/ .

ENTRYPOINT ["dotnet", "ZooKeepers.Server.dll"]