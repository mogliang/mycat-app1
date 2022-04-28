echo start loading environment variables

cat $(Build.SourcesDirectory)/kustomize/metadata.json

kubeConnection=$(jq ".environments[] | select(.name==\"$(DeploymentEnvironment)\") | .kubernetes.aksName" $(Build.SourcesDirectory)/kustomize/metadata.json)

# set pipeline variable kubeConnection
echo "##vso[task.setvariable variable=kubeConnection;isreadonly=true]$kubeConnection"
