using System;
using System.IO;
using System.Diagnostics;
using System.Threading;

using Tricentis.Automation.AutomationInstructions.TestActions;
using Tricentis.Automation.Creation;
using Tricentis.Automation.Engines;
using Tricentis.Automation.Engines.SpecialExecutionTasks;
using Tricentis.Automation.Engines.SpecialExecutionTasks.Attributes;

namespace veb
{
	[SpecialExecutionTaskName("WaitForFile")]
	public class WaitForFile : SpecialExecutionTask
	{
		public WaitForFile(Validator validator)
			: base(validator)
		{
		}

		public override ActionResult Execute(ISpecialExecutionTaskTestAction testAction)
		{
			String file = testAction.GetParameterAsInputValue("File", false).Value;
			if (string.IsNullOrEmpty(file)) {
				throw new ArgumentException(string.Format("Es muss eine Datei angegeben sein."));
			}
	    
			String paraTimeOut = testAction.GetParameterAsInputValue("TimeOut", false).Value;
			if (string.IsNullOrEmpty(paraTimeOut)) {
				throw new ArgumentException(string.Format("TimeOut muss gesetzt sein."));
			}
			int timeOut;
			bool isNumeric = int.TryParse(paraTimeOut, out timeOut);
			if (!isNumeric) {
				throw new ArgumentException(string.Format("TimeOut muss numerisch sein."));
			}
	    
			String paraInterval = testAction.GetParameterAsInputValue("Interval", false).Value;
			if (string.IsNullOrEmpty(paraInterval)) {
				throw new ArgumentException(string.Format("Es muss ein Interval gesetzt sein."));
			}
			int interval;
			isNumeric = int.TryParse(paraInterval, out interval);
			if (!isNumeric) {
				throw new ArgumentException(string.Format("Das Inervall muss numerisch sein."));
			}

			bool isFinished = false;
			bool isFound = false;
			long foundAfter = 666;
			
			var stopwatch = new Stopwatch();
			stopwatch.Start();
			
			while (!isFinished) {
				if (File.Exists(file)) {
					foundAfter = stopwatch.ElapsedMilliseconds;
					stopwatch.Stop();
					isFound = true;
					isFinished = true;
				}
				if (stopwatch.Elapsed > TimeSpan.FromMilliseconds(timeOut)) {
					isFinished = true;
				}
				if (!isFinished) {
					Thread.Sleep(interval);
				}
			}

			// Abschlusswartezeit?
			Thread.Sleep(2000);
			
			if (isFound) {
				return new PassedActionResult("File " + file + " found after: " + foundAfter + " ms.");
			} else {
				return new NotFoundFailedActionResult("File " + file + " not found.");
			}
		}
	}
}