readonly AKS_APP_NAME=app1
readonly STORAGE_SHARE_NAME="app1fileshare"
readonly ACR_NAME=qliang
###################### Init variables below ########################
# Subscription
SUBSCRIPTION_NAME=<hidden>

# DNS
SITE_DNS=mycatstage

# AKS variable
USE_AKS=true
K8S_RESOURCE_GROUP=qliang-aks
K8S_NAME=qliang-dev-aks

# Storage variable
STORAGE_ACCOUNT=qliangaksapp1stage
STORAGE_RESOURCE_GROUP=qliang-aks
STORAGE_LOCATION=eastus

# AKS deployment variable
K8S_NAMESPACE=stageapp1

###################### Init connection #########################
if USE_AKS
then
    az account set --subscription $SUBSCRIPTION_NAME
    az aks get-credentials --resource-group $K8S_RESOURCE_GROUP --name $K8S_NAME
else
    echo "Please setup kubectl config manually !!!!!!!!!!!!!!!!!!!"
fi

###################### Setup file storage ##########################
# https://docs.microsoft.com/en-us/azure/aks/azure-files-volume

# Create a resource group
az group create --name $STORAGE_RESOURCE_GROUP --location $STORAGE_LOCATION

# Create a storage account
az storage account create -n $STORAGE_ACCOUNT -g $STORAGE_RESOURCE_GROUP -l $STORAGE_LOCATION --sku Standard_LRS

# Export the connection string as an environment variable, this is used when creating the Azure file share
export AZURE_STORAGE_CONNECTION_STRING=$(az storage account show-connection-string -n $STORAGE_ACCOUNT -g $STORAGE_RESOURCE_GROUP -o tsv)

# Create the file share
az storage share create --name $STORAGE_SHARE_NAME --connection-string $AZURE_STORAGE_CONNECTION_STRING

# Get storage account key
STORAGE_KEY=$(az storage account keys list --resource-group $STORAGE_RESOURCE_GROUP --account-name $STORAGE_ACCOUNT --query "[0].value" -o tsv)

# create K8S namespace
kubectl create namespace $K8S_NAMESPACE

# Create K8S secret
kubectl create secret generic app1storage-secret --from-literal=azurestorageaccountname=$STORAGE_ACCOUNT --from-literal=azurestorageaccountkey=$STORAGE_KEY -n $K8S_NAMESPACE


###################### setup VIP #########################
# https://docs.microsoft.com/en-us/azure/aks/static-ip
VIP_NAME="VIP_${K8S_NAMESPACE}_${AKS_APP_NAME}"

if $USE_AKS
then
    # if AKS, then need create VIP in its MC Resource group
    VIP_RESOURCE_GROUP=$(az aks list --query "[?name=='$K8S_NAME' && resourceGroup=='$K8S_RESOURCE_GROUP'].{rg:nodeResourceGroup}" --output tsv)
else
    VIP_RESOURCE_GROUP=$K8S_RESOURCE_GROUP
fi

# create vip
az network public-ip create --resource-group $VIP_RESOURCE_GROUP --name $VIP_NAME --sku Standard --allocation-method static
VIP_ADDRESS=$(az network public-ip show --resource-group $VIP_RESOURCE_GROUP --name $VIP_NAME --query ipAddress --output tsv)

# create DNS record
az network dns record-set a add-record --resource-group mc_qliang-aks_qliang-dev-aks_eastus --zone-name 2ae1f3c259e049cd9483.eastus.aksapp.io --record-set-name $SITE_DNS --ipv4-address $VIP_ADDRESS 

##################### setup acrpull ###########################
if [ $USE_AKS -eq 0 ]
then
    SERVICE_PRINCIPAL_NAME="${K8S_NAME}-sp"

    # Obtain the full registry ID
    ACR_REGISTRY_ID=$(az acr show --name $ACR_NAME --query "id" --output tsv)

    # Create the service principal with rights scoped to the registry.
    PASSWORD=$(az ad sp create-for-rbac --name $SERVICE_PRINCIPAL_NAME --scopes $ACR_REGISTRY_ID --role acrpull --query "password" --output tsv)
    USER_NAME=$(az ad sp list --display-name $SERVICE_PRINCIPAL_NAME --query "[].appId" --output tsv)

    # Create Acrpull secret
    REGISTRY_SECRET_NAME="${ACR_NAME}-acr-secret"
    REGISTRY_NAME="${ACR_NAME}.azurecr.io"
    kubectl create secret docker-registry $REGISTRY_SECRET_NAME --docker-server=$REGISTRY_NAME --docker-username=$USER_NAME --docker-password=$PASSWORD
fi

########################## Kustomize Setting ##########################

echo "Next step, please add patch in new environment's kustomization folder."
echo "public ip address: $VIP_ADDRESS"
echo "namespace: $K8S_NAMESPACE"