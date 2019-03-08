/*
 * Benutzer: Nitja
 * Datum: 07.03.2019
 * Zeit: 16:30
 */
using System;
using RestSharp;
using RestSharp.Deserializers;
using System.Collections.Generic;
using Tricentis.Automation.AutomationInstructions.TestActions;
using Tricentis.Automation.Creation;
using Tricentis.Automation.Engines;
using Tricentis.Automation.Engines.SpecialExecutionTasks;
using Tricentis.Automation.Engines.SpecialExecutionTasks.Attributes;
using Tricentis.Automation.AutomationInstructions.Configuration;
using Tricentis.Automation.Execution.Results;

namespace veb
{
	[SpecialExecutionTaskName("TNTestData")]
	public class TNTestData : SpecialExecutionTaskEnhanced
	{
		#region Public Methods and Operators

		public TNTestData(Validator validator) : base(validator)
		{
		}

		public override void ExecuteTask(ISpecialExecutionTaskTestAction testAction)
		{
			String paraCommand = testAction.GetParameterAsInputValue("Command", false).Value;
			if (string.IsNullOrEmpty(paraCommand)) {
				throw new ArgumentException(string.Format("Es muss ein Command angegeben sein."));
			}
			
			String paraTarget = testAction.GetParameterAsInputValue("Target", false).Value;
			if (string.IsNullOrEmpty(paraTarget)) {
				throw new ArgumentException(string.Format("Es muss ein Target angegeben sein."));
			}

			String paraValue = testAction.GetParameterAsInputValue("Value", false).Value;
			if (string.IsNullOrEmpty(paraValue)) {
				throw new ArgumentException(string.Format("Es muss ein Value angegeben sein."));
			}

			// rest server address + port can be configured via buffer settings.
			// key should be: LOCAL_VEBTAL_PORT
			String outServer;
			bool adrServer = Buffers.Instance.TryGetBuffer("LOCAL_VEBTAL_SERVER", out outServer);
			if (string.IsNullOrEmpty(outServer)) {
				outServer = "http://127.0.0.1"; // fallback to default
			}

			// key should be: LOCAL_VEBTAL_PORT
			String outPort;
			bool port = Buffers.Instance.TryGetBuffer("LOCAL_VEBTAL_PORT", out outPort);
			if (string.IsNullOrEmpty(outPort)) {
				outPort = "84"; // fallback to default
			}
			
			var client = new RestClient(outServer + ":" + outPort);
			var request = new RestRequest("/td", Method.POST);
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
 			RestSharp.Deserializers.JsonDeserializer deserial = new JsonDeserializer();
            var JSONObj = deserial.Deserialize<Dictionary<string, string>>(response); 
            string rowCode = JSONObj["code"];
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
               	testAction.SetResult(SpecialExecutionTaskResultState.Ok, "The execution of the command was successfully processed.");
			} else {
				testAction.SetResult(SpecialExecutionTaskResultState.Failed, rowMsg);}
		}

		#endregion
	}
}

