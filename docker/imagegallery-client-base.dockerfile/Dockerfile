# Build Stage
FROM microsoft/dotnet:2.2-sdk

# Declare constants
ENV NVM_VERSION v0.33.8
ENV NODE_VERSION v9.3.0
ENV NVM_DIR /usr/local/nvm

RUN rm /bin/sh && ln -s /bin/bash /bin/sh

RUN apt-get update && apt-get install -y \
    curl \
    tree \
&& rm -rf /var/lib/apt/lists/* \
&& curl -o- https://raw.githubusercontent.com/creationix/nvm/$NVM_VERSION/install.sh | bash

# install node and npm
RUN source $NVM_DIR/nvm.sh \
    && nvm install $NODE_VERSION \
    && nvm alias default $NODE_VERSION \
&& nvm use default

ENV NODE_PATH $NVM_DIR/$NODE_VERSION/lib/node_modules
ENV PATH $NVM_DIR/versions/node/$NODE_VERSION/bin:$PATH

RUN node -v
RUN npm -v

COPY .  /app
WORKDIR /app

RUN dotnet restore

WORKDIR /app/src/ImageGallery.Client
RUN npm install
