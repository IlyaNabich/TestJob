
FROM mcr.microsoft.com/dotnet/sdk:10.0

WORKDIR /src

EXPOSE 8080

ENTRYPOINT ["dotnet", "run", "--urls", "http://+:8080", "--no-launch-profile"]
