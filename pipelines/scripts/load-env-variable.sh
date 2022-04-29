# https://github.com/mikefarah/yq#readme
wget https://github.com/mikefarah/yq/releases/download/v4.24.5/yq_linux_amd64 -O ~/yq && chmod +x ~/yq
kubeConnection=$(~/yq ".environments[] | select(.name==\"${DEPLOYENVIRONMENT}\") | .kubernetes.aksName" ${BUILD_SOURCESDIRECTORY}/deployment/kustomize/metadata.yaml)
echo "set pipeline variable: kubeConnection=${kubeConnection}"
echo "##vso[task.setvariable variable=kubeConnection;isoutput=true]$kubeConnection"
