# Monitoring

## Runner


## Log

[Query container log from Log Analytics](https://ms.portal.azure.com#@72f988bf-86f1-41af-91ab-2d7cd011db47/blade/Microsoft_Azure_Monitoring_Logs/LogsBlade/resourceId/%2Fsubscriptions%2Fb9e38f20-7c9c-4497-a25d-1a0c5eef2108%2FresourceGroups%2FDefaultResourceGroup-EUS%2Fproviders%2FMicrosoft.OperationalInsights%2Fworkspaces%2Fqliang-aks-log/source/LogsBlade.AnalyticsShareLinkToQuery/q/H4sIAAAAAAAAA12OQQvCMAyF7%252FsVue3Si2fpSUWKIiK7S2zD7HRJ6Tpl4I%252B3A3XqKby8vPflSgmscELPFEHDpj%252FRXpzhG3GSOBQPuJ8pEuywpS6gJdAaSgxhVmbP%252BS55tgkW7w6zVJMYQ%252FPiI7dS50wjnuHi2WnPI3TCC3%252F35NMQpaHcDpVvaU15jYmcAtNiTa9RYf1HVJBBK05xUL9%252FVUPI5vFAnfTRknHFE5J58U%252F%252FAAAA/timespan/P1D)

```
let container = KubePodInventory
| where Namespace == 'app1'
| distinct ContainerID, ContainerName;
ContainerLog
| join kind=inner container on ContainerID
| project  TimeGenerated, Image, ImageTag, ContainerName, LogEntry, ContainerID, Type, _ResourceId
```