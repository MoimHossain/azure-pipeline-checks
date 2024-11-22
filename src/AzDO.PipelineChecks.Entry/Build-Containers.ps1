clear
# define a string variable in powershell and assign the value to "0.0.4"
$containerTag = "v0.0.2"

Write-Host "Building Containers with $containerTag tag"
Write-Host "========================================"


Write-Host "Building AzDO.PipelineChecks.Entry container"
docker build -t moimhossain/azdo-checkv2-entry:$containerTag -f ./AzDO.PipelineChecks.Entry/Dockerfile .
docker push moimhossain/azdo-checkv2-entry:$containerTag
az containerapp update --resource-group ING-CHECK-V2 --name check-entry --image moimhossain/azdo-checkv2-entry:$containerTag



Write-Host "Building AzDO.Pipelines.WorkItemValidation container"
docker build -t moimhossain/azdo-checkv2-workitem-validation:$containerTag -f ./AzDO.Pipelines.WorkItemValidation/Dockerfile .
docker push moimhossain/azdo-checkv2-workitem-validation:$containerTag
az containerapp update --resource-group ING-CHECK-V2 --name workitem-validation --image moimhossain/azdo-checkv2-workitem-validation:$containerTag



