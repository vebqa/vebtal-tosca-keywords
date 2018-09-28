/*
 * Created by SharpDevelop.
 * User: doerges
 * Date: 04.11.2016
 * Time: 08:32
 * 
 * Dieses Keyword generiert eine Import Datei fuer die Verwendung z.B. im Selenium Kontext.
 */
using System;
using System.Net;
using System.Xml;
using System.Collections.Generic;

using Tricentis.Automation.AutomationInstructions.Configuration;
using Tricentis.Automation.Engines;
using Tricentis.Automation.AutomationInstructions.TestActions;
using Tricentis.Automation.Creation;
using Tricentis.Automation.Engines.SpecialExecutionTasks;
using Tricentis.Automation.Engines.SpecialExecutionTasks.Attributes;

using System.Linq;

namespace veb
{
	[SpecialExecutionTaskName("ResetSession")]
	public class ResetSession : SpecialExecutionTask
	{
		#region Public Methods and Operators
		
		public ResetSession(Validator validator)
			: base(validator)
		{
		}

		public override ActionResult Execute(ISpecialExecutionTaskTestAction testAction)
		{
			String paraTargetHost = testAction.GetParameterAsInputValue("Host", false).Value;
			if (string.IsNullOrEmpty(paraTargetHost)) {
				throw new ArgumentException(string.Format("Es muss ein Ziel (Host) angegeben sein."));
			}
			
			Uri resetRequest = new Uri("http://" + paraTargetHost + "/resetSession");
			
			GetResponse(resetRequest);
			
			return new PassedActionResult("Session reset on: " + paraTargetHost);
		}
		
		private void GetResponse(Uri uri)
		{
			WebClient wc = new WebClient();
			wc.OpenReadCompleted += (o, a) =>
			{
			};
			wc.OpenReadAsync(uri);
		}
		
		#endregion
	}
}

