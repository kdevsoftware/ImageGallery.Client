## ImageGallery Docker Build Scripts

[![This image on DockerHub](https://img.shields.io/docker/pulls/stuartshay/imagegallery-client.svg)](https://hub.docker.com/r/stuartshay/imagegallery-client/)

 Image | Size
------------ | -------------
Base  2.1.18 |[![](https://images.microbadger.com/badges/image/stuartshay/imagegallery-client:2.1.18-build-auth.svg)](https://microbadger.com/images/stuartshay/imagegallery-client:2.1.18-build-auth "Get your own image badge on microbadger.com")





```
├── docker  # Docker Build Files
|   |
│   └── imagegallery-client-base.dockerfile
|   |
│   └── imagegallery-client-build.dockerfile
|   |
│   └── imagegallery-client-local.dockerfile
|   |
│   └── imagegallery-client-web.tests.dockerfile
|   |
│   └── rancher  # Rancher Stack Deployment
```

Inspect Image 
```
docker run -i -t --entrypoint /bin/bash <IMAGEID>  
``` 
