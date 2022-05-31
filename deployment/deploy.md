# setup new deploy environment
check bootstrap script

# deploy deployment manually
## build image
check app1.fe/dockerReadMe.md

## deploy
`kubectl apply -f deployment.yaml`

## check
`
kubectl describe deployment app1-deployment -n app1
kubectl get pod -n app1
kubectl logs <pod name>
kubectl get service -n app1
`

## ssh and check mount
```
kubectl exec --stdin --tty <pod name> -n app1 -- /bin/bash
cd /mnt/azure
ls
exit
```

## sometimes, you need a restart
`kubectl rollout restart deployment app1-deployment -n app1`

## get deployment status
`kubectl get deployment app1-deployment -n app1 -o=jsonpath='{.status}'`