/*
 * Created by SharpDevelop.
 * User: doerges
 * Date: 16.11.2016
 * Time: 13:17
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.IO;

using Tricentis.Automation.AutomationInstructions.TestActions;
using Tricentis.Automation.Creation;
using Tricentis.Automation.Engines;
using Tricentis.Automation.Engines.SpecialExecutionTasks;
using Tricentis.Automation.Engines.SpecialExecutionTasks.Attributes;


namespace veb
{
[SpecialExecutionTaskName("ClearDirectory")]
public class ClearDirectory : SpecialExecutionTask
	{
		public ClearDirectory(Validator validator)
			: base(validator)
		{
		}
		
		public override ActionResult Execute(ISpecialExecutionTaskTestAction testAction)
		{
			String paraBuffer = testAction.GetParameterAsInputValue("Directory", false).Value;
			if (string.IsNullOrEmpty(paraBuffer)) {
				throw new ArgumentException(string.Format("Es muss ein Verzeichnis angegeben sein."));
			}
			
			int countFiles = 0;
			
			if(Directory.Exists(paraBuffer)) {
				string [] fileEntries = Directory.GetFiles(paraBuffer);
				foreach(string fileName in fileEntries) {
					File.Delete(fileName);
					countFiles++;
				}
			} else {
				throw new ArgumentException(string.Format("Es wurde kein Verzeichnis angegeben."));
			}
				
			return new PassedActionResult(string.Format("Removed files: {0}", countFiles));
		}		
	}
}
