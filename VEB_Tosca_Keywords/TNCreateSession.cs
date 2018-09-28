/*
 * Erstellt mit SharpDevelop.
 * Benutzer: doerges
 * Datum: 30.06.2017
 * Zeit: 10:28
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
	[SpecialExecutionTaskName("TNCreateSession")]
	public class TNCreateSession : SpecialExecutionTask
	{
		#region Public Methods and Operators

		public TNCreateSession(Validator validator)
			: base(validator)
		{
		}

		public override ActionResult Execute(ISpecialExecutionTaskTestAction testAction)
		{
			String paraHost = testAction.GetParameterAsInputValue("Host", false).Value;
			if (string.IsNullOrEmpty(paraHost)) {
				throw new ArgumentException(string.Format("Es muss ein Host angegeben sein."));
			}

			String paraPort = testAction.GetParameterAsInputValue("Port", false).Value;
// Wenn Target null, dann leer uebergeben.
			if (string.IsNullOrEmpty(paraPort)) {
				paraPort = "992";
			}

			String paraCP = testAction.GetParameterAsInputValue("CodePage", false).Value;
// Wenn Value null, dann leer uebergeben.
			if (string.IsNullOrEmpty(paraCP)) {
				paraCP = "1141";
			}

			String paraSSL = testAction.GetParameterAsInputValue("SSLType", false).Value;
// Wenn Value null, dann leer uebergeben.
			if (string.IsNullOrEmpty(paraSSL)) {
				paraSSL = "SSLv3";
			}


// Rest Service konfigurierbar machen?
			var client = new RestClient("http://127.0.0.1:84");
			var request = new RestRequest("telenese/createsession", Method.POST);
			request.RequestFormat = DataFormat.Json;
			request.AddHeader("Content-Type", "application/json");
			request.AddHeader("Accept", "application/json");
			request.AddBody(new { 
				host = paraHost, 
				port = paraPort, 
				codepage = paraCP,
				ssltype = paraSSL
				});

			IRestResponse response = client.Execute(request);
			var content = response.Content;

			// Deserialisieren
 			RestSharp.Deserializers.JsonDeserializer deserial= new JsonDeserializer();
            var JSONObj = deserial.Deserialize<Dictionary<string, string>>(response); 
            string rowCode =  JSONObj["code"];			
			
			if (rowCode == "0") {
				return new PassedActionResult("Got response: " + content);
            } else {
				return new  NotFoundFailedActionResult("Command failed: " + content + " for request: [host=" + paraHost + "; port=" + paraPort + "; codepage=" + paraCP + "; ssltype=" + paraSSL + "]");
			} // fi
		} // etucexe
		#endregion
	}
}
