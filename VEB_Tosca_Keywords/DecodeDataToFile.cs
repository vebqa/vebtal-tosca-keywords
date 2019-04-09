﻿
/*
* Erstellt mit Eclipse.
* Benutzer: Nitja
* Erstellt am 30.01.2019
* Zuletzt modifiziert: 30.01.2019
*/

using System;
using System.Net;
using System.Collections.Generic;
using Tricentis.Automation.AutomationInstructions.TestActions;
using Tricentis.Automation.Creation;
using Tricentis.Automation.Engines;
using Tricentis.Automation.Engines.SpecialExecutionTasks;
using Tricentis.Automation.Engines.SpecialExecutionTasks.Attributes;
using Tricentis.Automation.AutomationInstructions.Configuration;

namespace veb {

	[SpecialExecutionTaskName("DecodeDataToFile")]
	public class DecodeDataToFile: SpecialExecutionTask {

		public DecodeDataToFile(Validator validator) : base(validator) {}

		public override ActionResult Execute(ISpecialExecutionTaskTestAction testAction) {
		
			String base64ImageRepresentation = testAction.GetParameterAsInputValue("Payload", false).Value;
			String targetFile = testAction.GetParameterAsInputValue("TargetFile", false).Value;

			if (targetFile != null && base64ImageRepresentation != null) {
				System.IO.File.WriteAllBytes(targetFile, Convert.FromBase64String(base64ImageRepresentation));
			} else {
				throw new ArgumentException();
			}
			
			return new PassedActionResult("Decoding File successfully completed");
			
		}
	}
}