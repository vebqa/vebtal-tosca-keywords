﻿/*
 * Erstellt mit SharpDevelop.
 * Benutzer: doerges
 * Datum: 03.07.2017
 * Zeit: 09:45
 * 
 * Sie können diese Vorlage unter Extras > Optionen > Codeerstellung > Standardheader ändern.
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
using Tricentis.Automation.Engines;

namespace veb
{
	[SpecialExecutionTaskName("TN3270Execute")]
	public class TN3270Execute : SpecialExecutionTask
	{
		#region Public Methods and Operators

		public TN3270Execute(Validator validator)
			: base(validator)
		{
		}

		public override ActionResult Execute(ISpecialExecutionTaskTestAction testAction)
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
			var request = new RestRequest("msg/execute", Method.POST);
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
			
            // storedKey und storedValue beruecksichtigen
            if (JSONObj.ContainsKey("storedValue")) {
                	string storedValue = JSONObj["storedValue"];
                	string storedKey = JSONObj["storedKey"];
                	Buffers.Instance.SetBuffer(storedKey, storedValue, false);
            }
            
			if (rowCode == "0") {
				return new PassedActionResult("Got response: " + content);
			} else {
				return new  NotFoundFailedActionResult("Command failed: " + content + " for request: [command=" + paraCommand + "; target=" + paraTarget + "; value=" + paraValue + "]");
			}
		}

		#endregion
	}
}
