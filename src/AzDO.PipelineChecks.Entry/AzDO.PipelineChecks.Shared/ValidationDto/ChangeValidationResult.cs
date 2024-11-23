

namespace AzDO.PipelineChecks.Shared.ValidationDto
{
    public class ChangeValidationResult : ValidationResult
    {
        public string GetRowKey() => $"{this.DefinitionId}-{this.BuildId}";

        public static ChangeValidationResult CreateFrom(
            ValidationArguments validationArguments, bool isValid = false)
        {
            var validationResult = new ChangeValidationResult
            {
                IsValid = isValid,
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
