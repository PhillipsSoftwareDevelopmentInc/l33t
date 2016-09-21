FROM microsoft/dotnet:1.0.0-preview2-sdk
MAINTAINER Matt Phillips matt@phillipssoftware.co

COPY . /initializer
WORKDIR /initializer

RUN dotnet restore -v Verbose
RUN dotnet build

#CMD ["dotnet","run"]