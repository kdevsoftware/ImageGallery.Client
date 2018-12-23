# Build Stage
FROM stuartshay/imagegallery-client:2.1.6-base-auth AS build-env

# Declare constants
ENV NVM_VERSION v0.33.8
ENV NODE_VERSION v9.3.0
ENV NVM_DIR /usr/local/nvm

ARG BUILD_NUMBER=0
ENV BUILD_NUMBER ${BUILD_NUMBER}

ARG LOGGLY_TOKEN=0
ENV LOGGLY_TOKEN ${LOGGLY_TOKEN}

# Install Packages 
RUN apt-get update && apt-get install -y \
    curl

RUN curl --silent -o- https://raw.githubusercontent.com/creationix/nvm/${NVM_VERSION}/install.sh | bash

# install node and npm
RUN source $NVM_DIR/nvm.sh \
    && nvm install $NODE_VERSION \
    && nvm alias default $NODE_VERSION \
&& nvm use default

ENV NODE_PATH $NVM_DIR/$NODE_VERSION/lib/node_modules
ENV PATH $NVM_DIR/versions/node/$NODE_VERSION/bin:$PATH

RUN node -v
RUN npm -v

RUN npm install -g gulp \ 

RUN gulp -v

# Copy Source
COPY src /app/src
COPY ImageGallery.Client.sln /app/ImageGallery.Client.sln

WORKDIR /app/src/ImageGallery.Client

RUN export PATH="$PATH:/root/.dotnet/tools"
RUN dotnet tool install --global dotnet-setversion --version 2.0.0 
RUN dotnet tool list -g
#RUN dotnet setversion 2.1.2.${BUILD_NUMBER};

RUN dotnet publish -o /publish -c Release -f netcoreapp2.1 -r debian.9-x64

# Runtime Image Stage
FROM microsoft/dotnet:2.1.5-aspnetcore-runtime

WORKDIR /publish
COPY --from=build-env /publish .

# Set environment variables
ENV ASPNETCORE_URLS http://*:44600
ENV ASPNETCORE_ENVIRONMENT Staging

EXPOSE 44600

ENTRYPOINT ["dotnet", "ImageGallery.Client.dll"]
