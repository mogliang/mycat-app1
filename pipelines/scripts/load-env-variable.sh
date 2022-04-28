# https://github.com/mikefarah/yq#readme
wget https://github.com/mikefarah/yq/releases/download/v4.24.5/yq_linux_amd64 -O ~/yq &&\
    chmod +x ~/yq

cat $(Build.SourcesDirectory)/kustomize/metadata.yaml

kubeConnection=$(~/yq ".environments[] | select(.name==\"$(DeploymentEnvironment)\") | .kubernetes.aksName" $(Build.SourcesDirectory)/kustomize/metadata.yaml)

# set pipeline variable kubeConnection
echo "##vso[task.setvariable variable=kubeConnection;isreadonly=true]$kubeConnection"
