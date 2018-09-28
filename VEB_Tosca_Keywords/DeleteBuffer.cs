using System;
using System.Collections.Generic;

using Tricentis.Automation.AutomationInstructions.Configuration;
using Tricentis.Automation.AutomationInstructions.TestActions;
using Tricentis.Automation.Creation;
using Tricentis.Automation.Engines.SpecialExecutionTasks;
using Tricentis.Automation.Engines.SpecialExecutionTasks.Attributes;
using Tricentis.Automation.Execution.Results;

namespace Tricentis.Automation.SpecialExecutionTasks.BufferOperations {
    [SpecialExecutionTaskName("DeleteBuffer")]
    public class DeleteBuffer : SpecialExecutionTaskEnhanced {
        #region Constants

        private const string BufferCollectionPath = "Engine.Buffer";

        #endregion

        #region Constructors and Destructors

        public DeleteBuffer(Validator validator)
            : base(validator) {
        }

        #endregion

        #region Public Methods and Operators

        public override void ExecuteTask(ISpecialExecutionTaskTestAction testAction) {
        	if (!Collections.Instance.CollectionExists(BufferCollectionPath)) {
                throw new InvalidOperationException("No Buffer-Collection is available in the settings!");
            }
            foreach (
                KeyValuePair<string, string> buffer in Collections.Instance.GetCollectionEntries(BufferCollectionPath)) {
                if (!string.IsNullOrEmpty(buffer.Value)) {
        			if ((!buffer.Key.StartsWith("LOCAL_")) && (!buffer.Key.StartsWith("ENV")) && (!buffer.Key.StartsWith("TOOLS_"))) {
                    	Buffers.Instance.SetBuffer(buffer.Key, string.Empty, false);
        			}
                }
            }
            testAction.SetResult(SpecialExecutionTaskResultState.Ok, "Successfully cleared all Buffers!");
        }

        #endregion
    }
}