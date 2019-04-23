/*
 * Created by SharpDevelop.
 * User: Nitja
 * Date: 16.04.2019
 * Time: 13:00
 */

using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using Tricentis.Automation.Engines;
using Tricentis.Automation.Creation;
using Tricentis.Automation.Execution.Results;
using Tricentis.Automation.Engines.SpecialExecutionTasks;
using Tricentis.Automation.AutomationInstructions.TestActions;
using Tricentis.Automation.AutomationInstructions.Configuration;
using Tricentis.Automation.Engines.SpecialExecutionTasks.Attributes;

namespace veb
{

	[SpecialExecutionTaskName("IdentifyFileWithPattern")]
	public class IdentifyFileWithPattern : SpecialExecutionTask {
	
        public IdentifyFileWithPattern (Validator validator) : base(validator) {}

        public override ActionResult Execute(ISpecialExecutionTaskTestAction testAction) {
        
        String filePath = testAction.GetParameterAsInputValue("Path", false).Value;
        String fileNamePattern = testAction.GetParameterAsInputValue("Pattern", false).Value;
        String bufferName = testAction.GetParameterAsInputValue("Buffer", false).Value;
        
        	if (string.IsNullOrEmpty(filePath))
            {
                throw new ArgumentException(string.Format("File Path is required."));
            }
            			
			if (string.IsNullOrEmpty(fileNamePattern))
            {
                throw new ArgumentException(string.Format("File Name Pattern is required."));
            }
            
            if (string.IsNullOrEmpty(bufferName))
            {
                throw new ArgumentException(string.Format("Buffer Name is required."));
            }
					
			string[] matches = Directory.GetFiles(filePath, fileNamePattern);
			
			int arrLength = matches.Length;
			
			if(arrLength == 0) {
				return new NotFoundFailedActionResult("No matching file name was found for the given pattern.");
			} else if(arrLength > 1) {
				return new NotFoundFailedActionResult("More than 1 file found for the given pattern.");
			} else {
				Buffers.Instance.SetBuffer(bufferName, matches[0], false);
				return new PassedActionResult("File name saved successfully");
			}
            
        }
    }
}