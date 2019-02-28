/*
 * Erstellt mit SharpDevelop.
 * Benutzer: doerges
 * Datum: 11.09.2017
 * Zeit: 14:35
 * 
 * Sie können diese Vorlage unter Extras > Optionen > Codeerstellung > Standardheader ändern.
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

namespace veb
{
	[SpecialExecutionTaskName("PDFExecute")]
	public class PDFExecute : SpecialExecutionTask
	{
		#region Public Methods and Operators

		public PDFExecute(Validator validator)
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
			var request = new RestRequest("pdf/execute", Method.POST);
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

