FROM microsoft/dotnet:1.0.0-preview2-sdk
MAINTAINER Matt Phillips matt@phillipssoftware.co

COPY ./project.json /web/project.json

WORKDIR /web
RUN dotnet restore -v Verbose

COPY . /web
WORKDIR /web


RUN dotnet build

EXPOSE 5000/tcp
#CMD ["dotnet","run","--server.urls=http://0.0.0.0:5000"]
