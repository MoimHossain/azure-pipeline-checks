Payload structure

{
"Content-Type":"application/json", 
"PlanUrl": "$(system.CollectionUri)", 
"ProjectId": "$(system.TeamProjectId)", 
"HubName": "$(system.HostType)", 
"PlanId": "$(system.PlanId)", 
"JobId": "$(system.JobId)", 
"TimelineId": "$(system.TimelineId)", 
"TaskInstanceId": "$(system.TaskInstanceId)", 
"AuthToken": "$(system.AccessToken)",
"StageId": "$(system.StageId)",
"DefinitionId": "$(system.DefinitionId)",
"StageName": "$(system.StageName)",
"BuildId": "$(system.BuildId)",
"StageAttempt": "$(system.StageAttempt)",
"CheckStageAttempt": "$(checks.stageAttempt)",
"CheckStageId": "$(checks.stageId)"
}