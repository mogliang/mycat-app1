# Pipelines


Pipeline | Function | Trigger
---------|----------|--------
Build App1 Image - CI | build application binary, make docker image and then push to Azure container registry (Acr). | checkin to */app1.fe* path
Deploy App1 Preprod - CD | deploy to preprod environment | checkin to */deploymemt/kustomize/environments/preprod-qliang-aks-dev* path
Build App1 Deployment - CI | create release spec, used by release | checkin to */deployment/kustomize* path

# Releases
## App1 Releaes rollout
Rollout all environments. triggered daily at 8:00AM, pick release spec with tags *'release' 'stage'*

