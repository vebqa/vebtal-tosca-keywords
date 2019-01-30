﻿﻿
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

	[SpecialExecutionTaskName("EncodeImageToBuffer")]
	public class EncodeImageToBuffer: SpecialExecutionTask {

		public EncodeImageToBuffer(Validator validator) : base(validator) {}

		public override ActionResult Execute(ISpecialExecutionTaskTestAction testAction) {
		
			String sourceFile = testAction.GetParameterAsInputValue("ImageFile", false).Value;
			String bufferParameterName = testAction.GetParameterAsInputValue("BufferParamName", false).Value;

			if (sourceFile != null && bufferParameterName != null) {
				string base64ImageRepresentation = Convert.ToBase64String(System.IO.File.ReadAllBytes(sourceFile));
				Buffers.Instance.SetBuffer(bufferParameterName, base64ImageRepresentation, false);
			} else {
				throw new ArgumentException();
			}
			
			return new PassedActionResult("Encoding Image successfully completed");

		}
	}
}