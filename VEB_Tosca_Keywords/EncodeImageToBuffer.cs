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

	[SpecialExecutionTaskName("EncodeImageToBuffer")]
	public class EncodeImageToBuffer: SpecialExecutionTask {

		public EncodeImageToBuffer(Validator validator) : base(validator) {}

		public override ActionResult Execute(ISpecialExecutionTaskTestAction testAction) {
		
			String imageFile = testAction.GetParameterAsInputValue("ImageFile", false).Value;
			String bufferParameterName = testAction.GetParameterAsInputValue("BufferParamName", false).Value;
			const String repoType = "InMemory";

			if (string.IsNullOrEmpty(imageFile)) {
				throw new ArgumentException("Image file is a mandatory parameter.");
			}
			
			if (string.IsNullOrEmpty(bufferParameterName)) {
				throw new ArgumentException("Buffer Parameter Name is a mandatory parameter.");
			}
			
			byte[] imageArray = System.IO.File.ReadAllBytes(Image.FromFile(imageFile));
			string base64ImageRepresentation = Convert.ToBase64String(imageArray);

			return new PassedActionResult("Encoding Image successfully completed");

		}
	}
}
