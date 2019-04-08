/*
 * Created by SharpDevelop.
 * User: Nitja
 * Date: 08.04.2019
 * Time: 10:09
 */

using System;
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
	[SpecialExecutionTaskName("CalcWorkingDays")]
	public class CalcWorkingDays : SpecialExecutionTask {
	
		private double calcBusinessDays;
		
        public CalcWorkingDays(Validator validator) : base(validator) {}

        public override ActionResult Execute(ISpecialExecutionTaskTestAction testAction)   {
        
			DateTime paraStartDate, paraEndDate;
			
			if (string.IsNullOrEmpty(testAction.GetParameterAsInputValue("StartDate", false).Value))
            {
                throw new ArgumentException(string.Format("Es muss ein Start Datum angegeben sein."));
            }
			
			if (string.IsNullOrEmpty(testAction.GetParameterAsInputValue("EndDate", false).Value))
            {
                throw new ArgumentException(string.Format("Es muss ein End Datum angegeben sein."));
            }
            
		 	paraStartDate = DateTime.ParseExact(testAction.GetParameterAsInputValue("StartDate", false).Value, "dd-MM-yyyy", System.Globalization.CultureInfo.InvariantCulture);
           	paraEndDate = DateTime.ParseExact(testAction.GetParameterAsInputValue("EndDate", false).Value, "dd-MM-yyyy", System.Globalization.CultureInfo.InvariantCulture);
			          
			calcBusinessDays = 1 + ((paraEndDate - paraStartDate).TotalDays * 5 - (paraStartDate.DayOfWeek - paraEndDate.DayOfWeek) * 2) / 7;
			if (paraEndDate.DayOfWeek == DayOfWeek.Saturday) calcBusinessDays--;
			if (paraStartDate.DayOfWeek == DayOfWeek.Sunday) calcBusinessDays--;
			
            return new PassedActionResult("Calculated Business Days: " + calcBusinessDays);
            
        }
    }
}