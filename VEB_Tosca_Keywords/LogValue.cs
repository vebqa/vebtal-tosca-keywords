/*
 * Created by SharpDevelop.
 * User: Doerges
 * Date: 03.11.2016
 * Time: 13:36
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;

using Tricentis.Automation.AutomationInstructions.TestActions;
using Tricentis.Automation.Creation;
using Tricentis.Automation.Engines;
using Tricentis.Automation.Engines.SpecialExecutionTasks;
using Tricentis.Automation.Engines.SpecialExecutionTasks.Attributes;

namespace veb
{
	[SpecialExecutionTaskName("LogValue")]
	public class LogValue : SpecialExecutionTask
	{
		public LogValue(Validator validator)
			: base(validator)
		{
		}

		public override ActionResult Execute(ISpecialExecutionTaskTestAction testAction)
		{
			String paraBuffer = testAction.GetParameterAsInputValue("Value", false).Value;
			if (string.IsNullOrEmpty(paraBuffer)) {
				throw new ArgumentException(string.Format("Es muss ein Buffer angegeben sein."));
			}
	    
			return new PassedActionResult(paraBuffer);
		}
	}
}
