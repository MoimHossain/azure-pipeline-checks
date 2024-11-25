

namespace AzDO.PipelineChecks.Shared
{
    public class Constants
    {
        public class Dapr
        {
            public const string PubSub_Entry = "azdo.pipeline.check.entry";
            public const string PubSub_ValidationOutcome = "azdo.pipeline.check.outcome";
            public const string Topic_RequestReceived = "entryrequests";
            public const string Topic_RequestEvaluated = "requestevaluated";

            public class Sub
            {
                public const string WorkItem = "azdo.pipeline.check.workitem.validation";
                public const string Change = "azdo.pipeline.check.change.validation";
                public const string OutcomeAggragator = "azdo.pipeline.check.outcome.consumer";
            }

            public class State
            {
                public const string WorkItemValidations = "azdo.pipeline.workitem.validations";
                public const string ChangeValidations = "azdo.pipeline.change.validations";
                public const string CheckOutcomes = "azdo.pipeline.check.outcomestore";
                public const string Leases = "azdo.pipeline.check.leasestore";
            }
        }

        public class MicroServices
        {
            public const string WorkItemValidation = "workitem-validation";
            public const string ChangeValidation = "change-validation";
            public const string Entry = "check-entry";
        }
    }
}
