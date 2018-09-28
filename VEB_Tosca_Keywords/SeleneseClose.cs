/*
 * Erstellt mit SharpDevelop.
 * Benutzer: doerges
 * Datum: 17.05.2017
 * Zeit: 13:18
 * 
 * Sie können diese Vorlage unter Extras > Optionen > Codeerstellung > Standardheader ändern.
 */

using System;
using RestSharp;
using Tricentis.Automation.AutomationInstructions.TestActions;
using Tricentis.Automation.Creation;
using Tricentis.Automation.Engines;
using Tricentis.Automation.Engines.SpecialExecutionTasks;
using Tricentis.Automation.Engines.SpecialExecutionTasks.Attributes;

namespace veb
{
	[SpecialExecutionTaskName("SeleneseClose")]
	public class SeleneseClose : SpecialExecutionTask
	{
		#region Public Methods and Operators
		
		public SeleneseClose(Validator validator)
			: base(validator)
		{
		}

		public override ActionResult Execute(ISpecialExecutionTaskTestAction testAction)
		{
			// Rest Service konfigurierbar machen?
			var client = new RestClient("http://127.0.0.1:84");
			var request = new RestRequest("selenese/close", Method.POST);
			request.RequestFormat = DataFormat.Json;
			request.AddHeader("Content-Type", "application/json");
			request.AddHeader("Accept", "application/json");
			request.AddBody(new { 
			                	command = "close", 
			                	target = "this", 
			                	value = "null"
			                });
			
			IRestResponse response = client.Execute(request);
            var content = response.Content;
            
			return new PassedActionResult("Got response: " + content);
		}
	
		#endregion
	}
}

