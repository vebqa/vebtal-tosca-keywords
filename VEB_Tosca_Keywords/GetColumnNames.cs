/*
 * Created by SharpDevelop
 * User: Nitja
 * Date: 03.06.2019
 */

using System;
using System.IO;
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

	[SpecialExecutionTaskName("GetColumnNames")]
	public class GetColumnNames : SpecialExecutionTask {
	
        public GetColumnNames (Validator validator) : base(validator) {}

        public override ActionResult Execute(ISpecialExecutionTaskTestAction testAction) {
        
			String filePath = testAction.GetParameterAsInputValue("FilePath", false).Value;
            String rowNumber = testAction.GetParameterAsInputValue("RowNumber", false).Value;
                     
			if (string.IsNullOrEmpty(filePath))
            {
                throw new ArgumentException(string.Format("Text file path is required."));
            }
            			
			if (string.IsNullOrEmpty(rowNumber))
            {
                throw new ArgumentException(string.Format("Row number with column names is required."));
            }
            
            int i = 0;
            int row = Int32.Parse(rowNumber);
            int[] columnSize = {14,3,4,7,7,3,3,3,10,3,6,2,5,8,6,10,13,9,8,3,16,25,8};
            
            string resultString = "Column Names: ";
            string[] lines = File.ReadAllLines(filePath);
                    
            foreach (string line in lines)
            {
            	if(line.Equals(lines[row-1]) && i == 0)
            	{
            		string columnNames = line.Substring(1);
            		int columnLength = 0;
            		
            		for(int x = 0; x < columnSize.Length; x++)
            		{
						columnLength = columnSize[x];
            			resultString += columnNames.Substring(i, columnLength + 1).Trim() + ", ";		
            			i = i + columnLength + 1;
            		}
            	}
            }
            
			return new PassedActionResult(resultString);
            
        }
    }
}