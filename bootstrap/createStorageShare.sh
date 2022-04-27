

# https://docs.microsoft.com/en-us/azure/aks/azure-files-volume
# Change these four parameters as needed for your own environment
AKS_PERS_STORAGE_ACCOUNT_NAME=qliangaksapp1
AKS_PERS_RESOURCE_GROUP=qliang-aks
AKS_PERS_LOCATION=eastus
AKS_PERS_SHARE_NAME=qliangaksapp1share

AKS_KUBECONFIG_PATH=
AKS_NAMESPACE=app1

###################### Setup file storage #########################

# Create a resource group
az group create --name $AKS_PERS_RESOURCE_GROUP --location $AKS_PERS_LOCATION

# Create a storage account
az storage account create -n $AKS_PERS_STORAGE_ACCOUNT_NAME -g $AKS_PERS_RESOURCE_GROUP -l $AKS_PERS_LOCATION --sku Standard_LRS

# Export the connection string as an environment variable, this is used when creating the Azure file share
export AZURE_STORAGE_CONNECTION_STRING=$(az storage account show-connection-string -n $AKS_PERS_STORAGE_ACCOUNT_NAME -g $AKS_PERS_RESOURCE_GROUP -o tsv)

# Create the file share
az storage share create -n $AKS_PERS_SHARE_NAME --connection-string $AZURE_STORAGE_CONNECTION_STRING

# Get storage account key
STORAGE_KEY=$(az storage account keys list --resource-group $AKS_PERS_RESOURCE_GROUP --account-name $AKS_PERS_STORAGE_ACCOUNT_NAME --query "[0].value" -o tsv)

# Create AKS secret
kubectl create secret generic app1storage-secret --from-literal=azurestorageaccountname=$AKS_PERS_STORAGE_ACCOUNT_NAME --from-literal=azurestorageaccountkey=$STORAGE_KEY -n $AKS_NAMESPACE


# setup VIP
# https://docs.microsoft.com/en-us/azure/aks/static-ip
az network public-ip create --resource-group MC_qliang-aks_qliang-dev-aks_eastus --name app1-lb --sku Standard --allocation-method static
VIP=$(az network public-ip show --resource-group MC_qliang-aks_qliang-dev-aks_eastus --name app1-lb --query ipAddress --output tsv)
