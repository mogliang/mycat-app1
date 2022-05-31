## create image
`docker build -t app1.fe-image:v0.2 --build-arg VERSION=1 -f dockerfile .`

## show image
`docker image ls`

## run
`docker run -d -p 8088:8011 --name app1.fe app1.fe-image:v0.2` 

## stop
`docker container stop app1.fe`

## remove container
`docker container rm app1.fe`

## push to acr
https://docs.microsoft.com/en-us/azure/container-registry/container-registry-get-started-docker-cli?tabs=azure-cli
```
docker login qliang.azurecr.io -u 00000000-0000-0000-0000-000000000000 -p <get token by 'az acr login -n qliang --expose-token'>
docker tag app1.fe-image:v0.4 qliang.azurecr.io/app1.fe-image:v0.4
docker push qliang.azurecr.io/app1.fe-image:v0.2
```
