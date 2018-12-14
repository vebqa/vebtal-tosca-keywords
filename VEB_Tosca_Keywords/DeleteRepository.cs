
/*
* Erstellt mit SharpDevelop.
* Benutzer: doerges
* Erstellt am 14.12.2018
* Zuletzt modifiziert: 14.12.2018
* 
* Sie können diese Vorlage unter Extras > Optionen > Codeerstellung > Standardheader ändern.
*/

using System;
using System.Net;
using System.Collections.Generic;
using RestSharp;
using RestSharp.Deserializers;
using Tricentis.Automation.AutomationInstructions.TestActions;
using Tricentis.Automation.Creation;
using Tricentis.Automation.Engines;
using Tricentis.Automation.Engines.SpecialExecutionTasks;
using Tricentis.Automation.Engines.SpecialExecutionTasks.Attributes;
using Tricentis.Automation.AutomationInstructions.Configuration;

namespace veb {

	[SpecialExecutionTaskName("DeleteRepository")]
	public class deleteRepository: SpecialExecutionTask {

		public deleteRepository(Validator validator) : base(validator) {}

		public override ActionResult Execute(ISpecialExecutionTaskTestAction testAction) {

			String endPoint = testAction.GetParameterAsInputValue("EndPoint", false).Value;
			String repositoryName = testAction.GetParameterAsInputValue("Repository", false).Value;

			if (string.IsNullOrEmpty(endPoint)) {
				throw new ArgumentException(string.Format("Es muss ein EndPoint angegeben sein."));
			}
			else
			{
				if(!endPoint.EndsWith("/"))
				{
					endPoint += "/";
				}
			}

			if (string.IsNullOrEmpty(repositoryName)) {
				throw new ArgumentException(string.Format("Es muss ein Repository angegeben sein."));
			}

			var client = new RestClient(endPoint + "configuration/repositories/" + repositoryName);
			var request = new RestRequest("", Method.DELETE);

			IRestResponse response = client.Execute(request);
			HttpStatusCode statusMessage = response.StatusCode;
			int statusCode = (int) statusMessage;

			return new PassedActionResult("Got response: " + statusCode + " " + statusMessage);

		}
	}
}
