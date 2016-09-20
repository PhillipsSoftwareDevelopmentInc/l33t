FROM microsoft/dotnet:1.0.0-preview2-sdk
MAINTAINER Matt Phillips matt@phillipssoftware.co

COPY . /dotnetapp
WORKDIR /dotnetapp

RUN dotnet restore -v Verbose
RUN dotnet build

CMD ["dotnet","run"]