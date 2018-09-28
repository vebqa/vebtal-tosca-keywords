/*
 * Erstellt mit SharpDevelop.
 * Benutzer: doerges
 * Datum: 30.01.2017
 * Zeit: 15:31
 * 
 * Sie können diese Vorlage unter Extras > Optionen > Codeerstellung > Standardheader ändern.
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
	[SpecialExecutionTaskName("CheckMessage")]
	public class CheckMessage : SpecialExecutionTask
	{
		#region Public Methods and Operators
		
		public CheckMessage(Validator validator)
			: base(validator)
		{
		}

		public override ActionResult Execute(ISpecialExecutionTaskTestAction testAction)
		{
			String paraTargetHost = testAction.GetParameterAsInputValue("Host", false).Value;
			if (string.IsNullOrEmpty(paraTargetHost)) {
				throw new ArgumentException(string.Format("Es muss ein Ziel (Host) angegeben sein."));
			}
			
			string paraMessage = testAction.GetParameterAsInputValue("Message", false).Value;
			if (string.IsNullOrEmpty(paraMessage)) {
				throw new ArgumentException(string.Format("Es muss eine Message angegeben sein."));
			}
			
			string paraApi = testAction.GetParameterAsInputValue("API", false).Value;
			if (string.IsNullOrEmpty(paraApi)) {
				throw new ArgumentException(string.Format("Es muss eine API angegeben sein."));
			}
			
			
			Uri checkRequest = new Uri(string.Format("http://{0}/checkMessage/?api={1}", paraTargetHost, paraApi));
			
			GetResponse(checkRequest);
			
			return new PassedActionResult("Check result for request " + checkRequest);
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

