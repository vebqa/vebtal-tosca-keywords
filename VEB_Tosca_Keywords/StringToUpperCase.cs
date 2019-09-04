/*
 * Created by Eclipse
 * User: Nitja
 * Date: 13.08.2019
 */

using System;
using System.IO;
using System.Collections.Generic;
using Tricentis.Automation.Engines;
using Tricentis.Automation.Creation;
using Tricentis.Automation.Execution.Results;
using Tricentis.Automation.Engines.SpecialExecutionTasks;
using Tricentis.Automation.AutomationInstructions.TestActions;
using Tricentis.Automation.AutomationInstructions.Configuration;
using Tricentis.Automation.Engines.SpecialExecutionTasks.Attributes;

namespace veb
{

	[SpecialExecutionTaskName("StringToUpperCase")]
	public class StringToUpperCase : SpecialExecutionTask {
	
        public StringToUpperCase (Validator validator) : base(validator) {}

        public override ActionResult Execute(ISpecialExecutionTaskTestAction testAction) {
        
			String inputStr = testAction.GetParameterAsInputValue("InputString", false).Value;
                     
			if (string.IsNullOrEmpty(inputStr))
            {
                throw new ArgumentException(string.Format("Please provide input string to be converted to uppercase!"));
            }
            			
            string resultString = inputStr.ToUpper();
            
			return new PassedActionResult(resultString);
            
        }
    }
}