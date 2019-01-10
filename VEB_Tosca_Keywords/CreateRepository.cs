
/*
* Erstellt mit Eclipse.
* Benutzer: Nitja
* Erstellt am 14.12.2018
* Zuletzt modifiziert: 14.12.2018
*/

using System;
using System.Net;
using System.Collections.Generic;
using RestSharp;
using Tricentis.Automation.AutomationInstructions.TestActions;
using Tricentis.Automation.Creation;
using Tricentis.Automation.Engines;
using Tricentis.Automation.Engines.SpecialExecutionTasks;
using Tricentis.Automation.Engines.SpecialExecutionTasks.Attributes;
using Tricentis.Automation.AutomationInstructions.Configuration;

namespace veb {

	[SpecialExecutionTaskName("CreateRepository")]
	public class CreateRepository: SpecialExecutionTask {

		public CreateRepository(Validator validator) : base(validator) {}

		public override ActionResult Execute(ISpecialExecutionTaskTestAction testAction) {

			String endPoint = testAction.GetParameterAsInputValue("EndPoint", false).Value;
			String repoDescription = testAction.GetParameterAsInputValue("Description", false).Value;
			String repositoryName = testAction.GetParameterAsInputValue("Repository", false).Value;
			const String repoType = "InMemory";

			if (string.IsNullOrEmpty(endPoint)) {
				throw new ArgumentException("End Point is a mandatory parameter.");
			}
			else
			{
				if(!endPoint.EndsWith("/"))
				{
					endPoint += "/";
				}
			}

			if (string.IsNullOrEmpty(repositoryName)) {
				throw new ArgumentException("Repository Name is a mandatory parameter.");
			}

			var client = new RestClient(new Uri(endPoint + "configuration/repositories"));
			var request = new RestRequest(Method.POST);
			request.RequestFormat = DataFormat.Json;
			request.AddHeader("Content-Type", "application/json");
			request.AddBody(new {description = repoDescription, location = repositoryName, name = repositoryName, type = repoType});

			IRestResponse response = client.Execute(request);
			HttpStatusCode statusMessage = response.StatusCode;
			int statusCode = (int) statusMessage;

			return new PassedActionResult("Got response: " + statusCode + " " + statusMessage);

		}
	}
}
