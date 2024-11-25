

namespace AzDO.PipelineChecks.Shared.ValidationDto
{
    public class WorkItemValidationResult : ValidationResult
    {
        public string GetRowKey() => $"{this.BuildId}";

        public static WorkItemValidationResult CreateFrom(
            ValidationArguments validationArguments, bool isValid = false)
        {
            var validationResult = new WorkItemValidationResult
            {
                IsValid = isValid,
                CheckComputation = CheckResultCompuationKind.Computed,
                CreationTime = DateTime.UtcNow,
                ProjectId = validationArguments.ProjectId,
                BuildId = validationArguments.BuildId,
                DefinitionId = validationArguments.DefinitionId,
                StageName = validationArguments.StageName,
                JobId = validationArguments.JobId,                
                PlanId = validationArguments.PlanId,
                StageId = validationArguments.StageId,
                CheckStageId = validationArguments.CheckStageId
            };
            return validationResult;
        }
    }
}
