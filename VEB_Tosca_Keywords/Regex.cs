
/*
* Erstellt mit Eclipse.
* Benutzer: Nitja
* Erstellt am 01.02.2019
* Zuletzt modifiziert: 01.02.2019
*/

using System;
using System.Collections.Generic;
using Tricentis.Automation.AutomationInstructions.TestActions;
using Tricentis.Automation.Creation;
using Tricentis.Automation.Engines;
using Tricentis.Automation.Engines.SpecialExecutionTasks;
using Tricentis.Automation.Engines.SpecialExecutionTasks.Attributes;
using Tricentis.Automation.AutomationInstructions.Configuration;

namespace veb
{
	[SpecialExecutionTaskName("Regex")]
	public class Regex : SpecialExecutionTask
	{
		#region Public Methods and Operators

		public Regex(Validator validator) : base(validator)
		{
		}

		public override ActionResult Execute(ISpecialExecutionTaskTestAction testAction)
		{
			String paraSource = testAction.GetParameterAsInputValue("BufferSource", false).Value;
			String paraTarget = testAction.GetParameterAsInputValue("BufferTarget", false).Value;
			String regularExpression = testAction.GetParameterAsInputValue("RegularExpression", false).Value;
		}
	}
}
