#### Base Stage
FROM microsoft/dotnet:2.1.402-sdk AS base

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

RUN npm install -g gulp  
RUN gulp -v

#### Package Stage
FROM base AS package

COPY .  /app
WORKDIR /app

RUN dotnet restore /p:RestoreUseSkipNonexistentTargets="false"

WORKDIR /app/src/ImageGallery.Client
RUN npm install 

####  Publish 
FROM package AS publish
COPY --from=base /app /app 

WORKDIR /app/src/ImageGallery.Client

RUN node node_modules/webpack/bin/webpack.js --config webpack.config.js --env.prod
RUN npm run compile-app
RUN dotnet publish -o /publish -c Release -f netcoreapp2.1 -r debian.9-x64

#### RELEASE 
FROM microsoft/dotnet:2.1.5-aspnetcore-runtime AS release

COPY --from=publish /app/scripts .

RUN apt-get update && apt-get install -y \
        curl \
        dos2unix \
        dnsutils \ 
        iputils-ping \ 
		telnet \
        tree \
    && rm -rf /var/lib/apt/lists/* \
    && curl https://raw.githubusercontent.com/vishnubob/wait-for-it/master/wait-for-it.sh > /wait_for_it.sh \
    && chmod +x /*.sh

WORKDIR /publish
COPY --from=publish /publish .

# Set environment variables
ENV ASPNETCORE_URLS http://*:44600
ENV ASPNETCORE_ENVIRONMENT Docker

EXPOSE 44600

#HEALTHCHECK CMD curl --fail http://localhost:44600/api/Diagnostics/status || exit 1

#CMD ["/bin/bash"]
ENTRYPOINT ["/entrypoint.sh"]
