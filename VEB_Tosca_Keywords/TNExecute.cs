/*
 * Benutzer: doerges
 * Datum: 03.07.2017
 * Zeit: 09:45
 */
using System;
using RestSharp;
using RestSharp.Deserializers;
using System.Collections.Generic;
using Tricentis.Automation.AutomationInstructions.TestActions;
using Tricentis.Automation.Creation;
using Tricentis.Automation.Engines.SpecialExecutionTasks;
using Tricentis.Automation.Engines.SpecialExecutionTasks.Attributes;
using Tricentis.Automation.AutomationInstructions.Configuration;
using Tricentis.Automation.Execution.Results;

namespace veb
{
	[SpecialExecutionTaskName("TNExecute")]
	public class TNExecute : SpecialExecutionTaskEnhanced
	{
		#region Public Methods and Operators

		public TNExecute(Validator validator)
			: base(validator)
		{
		}

		public override void ExecuteTask(ISpecialExecutionTaskTestAction testAction)
		{
			String paraCommand = testAction.GetParameterAsInputValue("Command", false).Value;
			if (string.IsNullOrEmpty(paraCommand)) {
				throw new ArgumentException(string.Format("Es muss ein Command angegeben sein."));
			}

			String paraTarget = testAction.GetParameterAsInputValue("Target", false).Value;
			// Wenn Target null, dann leer uebergeben.
			if (string.IsNullOrEmpty(paraTarget)) {
				paraTarget = "";
			}

			String paraValue = testAction.GetParameterAsInputValue("Value", false).Value;
			// Wenn Value null, dann leer uebergeben.
			if (string.IsNullOrEmpty(paraValue)) {
				paraValue = "";
			}

			// Rest Service konfigurierbar machen?
			var client = new RestClient("http://127.0.0.1:84");
			var request = new RestRequest("tn5250/execute", Method.POST);
			request.RequestFormat = DataFormat.Json;
			request.AddHeader("Content-Type", "application/json");
			request.AddHeader("Accept", "application/json");
			request.AddBody(new { 
				command = paraCommand, 
				target = paraTarget, 
				value = paraValue
				});

			IRestResponse response = client.Execute(request);
			var content = response.Content;

			// Deserialisieren
 			RestSharp.Deserializers.JsonDeserializer deserial= new JsonDeserializer();
            var JSONObj = deserial.Deserialize<Dictionary<string, string>>(response); 
            string rowCode =  JSONObj["code"];
            string rowMsg = "";
            if (JSONObj.ContainsKey("message"))
            {
                rowMsg = JSONObj["message"];
            }
			
            // storedKey und storedValue beruecksichtigen
            if (JSONObj.ContainsKey("storedValue")) {
                	string storedValue = JSONObj["storedValue"];
                	string storedKey = JSONObj["storedKey"];
                	Buffers.Instance.SetBuffer(storedKey, storedValue, false);
            }
            
			if (rowCode == "0") {
				// return new PassedActionResult("Got response: " + content);
               	testAction.SetResult(SpecialExecutionTaskResultState.Ok, "The execution of the command was successfully processed.");
			} else {
				// Im Falle eines Fehlers und damit eines Abbruchs muessen wir die Telnet Session aufraeumen?
				// var requestClean = new RestRequest("selenese/close", Method.POST);
				// requestClean.RequestFormat = DataFormat.Json;
				// requestClean.AddHeader("Content-Type", "application/json");
				// requestClean.AddHeader("Accept", "application/json");
				// requestClean.AddBody(new { 
				//	command = "close", 
				//	target = "this", 
				//	value = "null"
				//	});
				// IRestResponse responseClean = client.Execute(requestClean);

				// return new  VerifyFailedActionResult("expected", "real");
				testAction.SetResult(
                    SpecialExecutionTaskResultState.Failed,
                    rowMsg);
			}
		}

		#endregion
	}
}

