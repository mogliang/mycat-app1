readonly AKS_APP_NAME=app1
readonly STORAGE_SHARE_NAME="app1fileshare"

###################### Init variables below ########################
# Subscription
SUBSCRIPTION_NAME=EdgeZone_Fiji_StorageService_Dev

# DNS
SITE_DNS=preprod-mycat

# AKS variable
AKS_RESOURCE_GROUP=qliang-aks
AKS_NAME=qliang-dev-aks

# Storage variable
STORAGE_ACCOUNT=qliangaksapp12
STORAGE_RESOURCE_GROUP=qliang-aks
STORAGE_LOCATION=eastus

# AKS deployment variable
AKS_NAMESPACE=preprodapp1

###################### Init connection #########################
az account set --subscription $SUBSCRIPTION_NAME
az aks get-credentials --resource-group $AKS_RESOURCE_GROUP --name $AKS_NAME


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

# create AKS namespace
kubectl create namespace $AKS_NAMESPACE

# Create AKS secret
kubectl create secret generic app1storage-secret --from-literal=azurestorageaccountname=$STORAGE_ACCOUNT --from-literal=azurestorageaccountkey=$STORAGE_KEY -n $AKS_NAMESPACE


###################### setup VIP #########################
# https://docs.microsoft.com/en-us/azure/aks/static-ip
VIP_NAME="VIP_${AKS_NAMESPACE}_${AKS_APP_NAME}"
AKS_MC_RESOURCE_GROUP=$(az aks list --query "[?name=='$AKS_NAME' && resourceGroup=='$AKS_RESOURCE_GROUP'].{rg:nodeResourceGroup}" --output tsv)

# create vip
az network public-ip create --resource-group $AKS_MC_RESOURCE_GROUP --name $VIP_NAME --sku Standard --allocation-method static
VIP_ADDRESS=$(az network public-ip show --resource-group $AKS_MC_RESOURCE_GROUP --name $VIP_NAME --query ipAddress --output tsv)

# create DNS record
az network dns record-set a add-record --resource-group mc_qliang-aks_qliang-dev-aks_eastus --zone-name 2ae1f3c259e049cd9483.eastus.aksapp.io --record-set-name $SITE_DNS --ipv4-address $VIP_ADDRESS 

########################## Kustomize Setting ##########################

echo "Next step, please add patch in new environment's kustomization folder."
echo "public ip address: $VIP_ADDRESS"
echo "namespace: $AKS_NAMESPACE"