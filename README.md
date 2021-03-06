# ImageGallery.Client

[![This image on DockerHub](https://img.shields.io/docker/pulls/stuartshay/imagegallery-client.svg)](https://hub.docker.com/r/stuartshay/imagegallery-client/)
[![Greenkeeper badge](https://badges.greenkeeper.io/stuartshay/ImageGallery.Client.svg)](https://greenkeeper.io/) [![Maintainability](https://api.codeclimate.com/v1/badges/3c9179479977132ebf3d/maintainability)](https://codeclimate.com/github/stuartshay/ImageGallery.Client/maintainability)

[![SonarCloud](http://sonar.navigatorglass.com:9000/api/project_badges/measure?project=ImageGalleryClient&metric=alert_status)](http://sonar.navigatorglass.com:9000/dashboard?id=ImageGalleryClient)
[![SonarCloud](http://sonar.navigatorglass.com:9000/api/project_badges/measure?project=ImageGalleryClient&metric=reliability_rating)](http://sonar.navigatorglass.com:9000/dashboard?id=ImageGalleryClient)
[![SonarCloud](http://sonar.navigatorglass.com:9000/api/project_badges/measure?project=ImageGalleryClient&metric=security_rating)](http://sonar.navigatorglass.com:9000/dashboard?id=ImageGalleryClient)
[![SonarCloud](http://sonar.navigatorglass.com:9000/api/project_badges/measure?project=ImageGalleryClient&metric=sqale_rating)](http://sonar.navigatorglass.com:9000/dashboard?id=ImageGalleryClient)


 Jenkins | Status  
------------ | -------------
Docker Base Image | [![Build Status](https://jenkins.navigatorglass.com/buildStatus/icon?job=ImageGallery-Auth/ImageGallery-Auth-base)](https://jenkins.navigatorglass.com/job/ImageGallery-Auth/job/ImageGallery-Auth-base/)
Docker Deployment Image  | [![Build Status](https://jenkins.navigatorglass.com/buildStatus/icon?job=ImageGallery-Auth/ImageGallery-Auth-build)](https://jenkins.navigatorglass.com/job/ImageGallery-Auth/job/ImageGallery-Auth-build/)
Docker Local Image | [![Build Status](https://jenkins.navigatorglass.com/buildStatus/icon?job=ImageGallery-Auth/ImageGallery-Auth-local)](https://jenkins.navigatorglass.com/job/ImageGallery-Auth/job/ImageGallery-Auth-local/)
Angular Lint Report | [![Build Status](https://jenkins.navigatorglass.com/buildStatus/icon?job=ImageGallery-Auth/ImageGallery-Auth-report-check)](https://jenkins.navigatorglass.com/job/ImageGallery-Auth/job/ImageGallery-Auth-report-check/)
C# SonarQube | [![Build Status](https://jenkins.navigatorglass.com/buildStatus/icon?job=ImageGallery-Auth/ImageGallery-Auth-sonarqube)](https://jenkins.navigatorglass.com/job/ImageGallery-Auth/job/ImageGallery-Auth-sonarqube/)
AppVeyor CI Build |[![Build status](https://ci.appveyor.com/api/projects/status/iub0tbs42d9ut0g7?svg=true)](https://ci.appveyor.com/project/StuartShay/imagegallery-client)

### Demo:
```
https://dev.informationcart.com
L: Claire P: password
```

### About:

Image Gallery Client is a .NET Core WebAPI/Angular Web Application, this project is a front end for managing and categorizing my large collection of New York City Landmarks and Attractions. 

The Collection can be viewed here  
```  
https://www.flickr.com/photos/stuartshay/
```

![](assets/web.png)


Photos are Submitted to the System with the assistance of custom Endpoints and Data sources to determine metadata about the photo.

![](assets/metadata.png)

![](assets/map.png)


### Project Structure
```
├── build   # Cake Build Configuration
|
├── docker  # Docker Build Files
│   └── *.dockerfile
|       └── .dockerignore
|       └── Dockerfile
|
├── e2e    # End to End Tests
│   │
│   └── ImageGallery.Client.Test.UI # Selenium
|
├── scripts    # Docker Image Scripts
│   └── *.sh
|
├── src
│   └── ImageGallery.Client # .NET Core Web
|       |
│       └── wwwroot # Angular Application
└── test
   │
   └── ImageGallery.Client.Test
```

### Prerequisites

```
Node v9.3.0
NET Core SDK 2.2.100
NET Core Runtime 2.2.0
```
### Install & Run

```
cd ImageGallery.Client
dotnet restore

cd src\ImageGallery.Client

npm install

npm run compile-app

dotnet run

http://localhost:8000/
```
#### Build Options 

```
Production: npm run compile-app-prod
Development: npm run compile-app
```
### Cake

Windows 

```
set-executionpolicy unrestricted

.\build.ps1
```

Linux/Mac
```
chmod +x build.sh
echo "export PATH=\"\$PATH:\$HOME/.dotnet/tools\"" >> ~/.bash_profile
source  ~/.bash_profile

dotnet tool install -g Cake.Tool
dotnet tool list -g
```

```
./build.sh
```

### SonarQube Testing 

```
 .\build.ps1 -target sonar
```
