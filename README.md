# ImageGallery.Client

[![This image on DockerHub](https://img.shields.io/docker/pulls/stuartshay/imagegallery-client.svg)](https://hub.docker.com/r/stuartshay/imagegallery-client/)
 [![dependencies Status](https://david-dm.org/stuartshay/ImageGallery.Client/status.svg)](https://david-dm.org/stuartshay/ImageGallery.Client) [![devDependencies Status](https://david-dm.org/stuartshay/ImageGallery.Client/dev-status.svg)](https://david-dm.org/stuartshay/ImageGallery.Client?type=dev) 


 Jenkins | Status  
------------ | -------------
Base Image (Auth) | [![Build Status](https://jenkins.navigatorglass.com/buildStatus/icon?job=ImageGallery-Auth/ImageGallery-Auth-base)](https://jenkins.navigatorglass.com/job/ImageGallery-Auth/job/ImageGallery-Auth-base/)
Application Image (Auth) | [![Build Status](https://jenkins.navigatorglass.com/buildStatus/icon?job=ImageGallery-Auth/ImageGallery-Auth-build)](https://jenkins.navigatorglass.com/job/ImageGallery-Auth/job/ImageGallery-Auth-build/)
Local Image (Auth) | [![Build Status](https://jenkins.navigatorglass.com/buildStatus/icon?job=ImageGallery-Auth/ImageGallery-Auth-local)](https://jenkins.navigatorglass.com/job/ImageGallery-Auth/job/ImageGallery-Auth-local/) [![Greenkeeper badge](https://badges.greenkeeper.io/stuartshay/ImageGallery.Client.svg)](https://greenkeeper.io/)

### Demo
```
https://dev.informationcart.com
L: Claire P: password
```


### Prerequisites

```
Node v9.3.0
NET Core 2.1
VS Code 1.19.1 or VS 2017 15.8.0 Preview 2
```

### Install

```
cd ImageGallery.Client
dotnet restore

cd src\ImageGallery.Client

npm install

npm run compile-app

dotnet run

http://localhost:5000/home
```

### Docker
```
docker build -t imagegallery-core-base

docker run -p 8080:44300 imagegallery-core-base
```


### Identity Server
https://auth.informationcart.com/
      
### API
https://imagegallery-api.informationcart.com/swagger

