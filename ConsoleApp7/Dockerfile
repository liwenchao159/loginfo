FROM microsoft/dotnet:2.0-runtime AS base
WORKDIR /app

FROM microsoft/dotnet:2.0-sdk AS build
WORKDIR /src
COPY Centaline.Fyq.LogAnalyze.sln ./
COPY ConsoleApp7/Centaline.Fyq.LogAnalyze.csproj ConsoleApp7/
COPY Config/Centaline.Fyq.LogAnalyze.Config.csproj Config/
COPY ElasticSearch/Centaline.Fyq.LogAnalyze.ElasticSearch.csproj ElasticSearch/
COPY Centaline.Fyq.LogAnalyze.Mode/Centaline.Fyq.LogAnalyze.Mode.csproj Centaline.Fyq.LogAnalyze.Mode/
RUN dotnet restore -nowarn:msb3202,nu1503
COPY . .
WORKDIR /src/ConsoleApp7
RUN dotnet build -c Release -o /app

FROM build AS publish
RUN dotnet publish -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "Centaline.Fyq.LogAnalyze.dll"]
