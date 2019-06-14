/*
 * Created by SharpDevelop
 * User: Nitja
 * Date: 04.06.2019
 */

using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Tricentis.Automation.Engines;
using Tricentis.Automation.Creation;
using Tricentis.Automation.Execution.Results;
using Tricentis.Automation.Engines.SpecialExecutionTasks;
using Tricentis.Automation.AutomationInstructions.TestActions;
using Tricentis.Automation.AutomationInstructions.Configuration;
using Tricentis.Automation.Engines.SpecialExecutionTasks.Attributes;

namespace veb
{

	[SpecialExecutionTaskName("GetColumnValue")]
	public class GetColumnValue : SpecialExecutionTask
	{
	
		public GetColumnValue(Validator validator)
			: base(validator)
		{
		}

		public override ActionResult Execute(ISpecialExecutionTaskTestAction testAction)
		{
			String filePath = testAction.GetParameterAsInputValue("FilePath", false).Value;
			String refColName = testAction.GetParameterAsInputValue("RefColName", false).Value;
			String textToFind = testAction.GetParameterAsInputValue("TextInRefCol", false).Value;
			String resColName = testAction.GetParameterAsInputValue("ResColName", false).Value;
                     
			if (string.IsNullOrEmpty(filePath)) {
				throw new ArgumentException(string.Format("Text file path is required."));
			}
			
			if (string.IsNullOrEmpty(refColName)) {
				throw new ArgumentException(string.Format("Reference column name is required."));
			}
			
			if (string.IsNullOrEmpty(textToFind)) {
				throw new ArgumentException(string.Format("Search string is required."));
			}
			
			if (string.IsNullOrEmpty(resColName)) {
				throw new ArgumentException(string.Format("Result column name is required."));
			}
            
			string[] allRows = File.ReadAllLines(filePath);

			int i = 0;
			int refColNameIndex = 0;
			int resColNameIndex = 0;
			int[] fixedLength = {14,3,4,7,7,3,3,3,10,3,6,2,5,8,6,10,11,11,8,3,16,25,8};
			string rowValue = null;
			string resultString = null;

			List<string> colNames = new List<string>();
            
			if (colNames.Count == 0) {
				rowValue = allRows[4].Substring(1);
				
				//Find Column Names and add to list
				foreach (int length in fixedLength) {
					colNames.Add(rowValue.Substring(i, length + 1).Trim());
					i = i + length + 1;
				}
				i = 0;
            	
				//Find the index of the reference column
				try {
					refColNameIndex = colNames.FindIndex(a => a.Contains(refColName));
					resColNameIndex = colNames.FindIndex(b => b.Contains(resColName));
				} catch {
					throw new ArgumentNullException(string.Format("Given reference/result column name (RefColumnName/ResColumnName) not found."));
				}
			}
			
			for (int x = 0; x < allRows.Count(); x++) {
				rowValue = allRows[x].Substring(1);
            	
				//Do only for rows which match the given regular expression
				if (!string.IsNullOrEmpty(rowValue) && Regex.IsMatch(rowValue, "^ [0-9]")) {
					resultString = null;
					
					//Build a string with the spilt column values
					for (int y = 0; y < fixedLength.Length; y++) {
						resultString += rowValue.Substring(i, fixedLength[y] + 1).Trim() + "|";
						i = i + fixedLength[y] + 1;
					}
					i = 0;
					
					//Check if the reference value exists and find the required value
					if(resultString.IndexOf(textToFind, StringComparison.Ordinal) != -1) {
						string[] columnValues = resultString.Split('|');
						return new PassedActionResult(columnValues[resColNameIndex]);
					}
				}
			}
			return new NotFoundFailedActionResult("Given text was not found!");
		}
	}
}