## Install Azure File CSI
https://github.com/kubernetes-sigs/azurefile-csi-driver/blob/master/docs/install-csi-driver-v1.17.0.md

```
curl -skSL https://raw.githubusercontent.com/kubernetes-sigs/azurefile-csi-driver/v1.17.0/deploy/install-driver.sh | bash -s v1.17.0 --
```
or
```
repo="https://raw.githubusercontent.com/kubernetes-sigs/azurefile-csi-driver/v1.17.0/deploy"
echo "Installing Azure File CSI driver, version: $ver ..."
kubectl apply -f $repo/rbac-csi-azurefile-controller.yaml
kubectl apply -f $repo/rbac-csi-azurefile-node.yaml
kubectl apply -f $repo/csi-azurefile-controller.yaml
kubectl apply -f $repo/csi-azurefile-driver.yaml
kubectl apply -f $repo/csi-azurefile-node.yaml
kubectl apply -f $repo/csi-azurefile-node-windows.yaml
echo 'Azure File CSI driver installed successfully.'
```
