/*
 * Created by SharpDevelop.
 * User: Nitja
 * Date: 08.04.2019
 * Time: 10:09
 */

using System;
using System.Linq;

namespace veb
{
	[SpecialExecutionTaskName("CalcWorkingDays")]
	public class CalcWorkingDays : SpecialExecutionTask
    {
		private double calcBusinessDays;
		
        public CalcWorkingDays(Validator validator) : base(validator)
        {
        }

        public override ActionResult Execute(ISpecialExecutionTaskTestAction testAction)
        {
            String paraCommand = testAction.GetParameterAsInputValue("Command", false).Value;
            if (string.IsNullOrEmpty(paraCommand))
            {
                throw new ArgumentException(string.Format("Es muss ein Command angegeben sein."));
            }

            DateTime paraStartDate = testAction.GetParameterAsInputValue("StartDate", false).Value;
            if (string.IsNullOrEmpty(paraStartDate))
            {
                throw new ArgumentException(string.Format("Es muss ein StartDate angegeben sein."));
            }

            DateTime paraEndDate = testAction.GetParameterAsInputValue("EndDate", false).Value;
            if (string.IsNullOrEmpty(paraEndDate))
            {
                throw new ArgumentException(string.Format("Es muss ein EndDate angegeben sein."));
            }

			calcBusinessDays = 1 + ((paraEndDate - paraStartDate).TotalDays * 5 - (paraStartDate.DayOfWeek - paraEndDate.DayOfWeek) * 2) / 7;
			
			if (paraEndDate.DayOfWeek == DayOfWeek.Saturday) calcBusinessDays--;
			if (paraStartDate.DayOfWeek == DayOfWeek.Sunday) calcBusinessDays--;

            testAction.SetResult(SpecialExecutionTaskResultState.Ok, "Calculated Business Days: " + calcBusinessDays);
        }
    }
}