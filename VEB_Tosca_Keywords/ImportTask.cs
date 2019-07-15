using System;
using ClosedXML.Excel;
using TCMigrationAPI;
using Tricentis.TCAddOns;

namespace SampleManualTestCaseConnector
{
    /// <summary>
    /// This class contains the business logic for the igration of Manual Test Cases.
    /// </summary>
    public class ImportTask
    {
        /// <summary>
        /// Definition object generates the migration metafile and required to access migration specific tasks from TCMigrationAPI.
        /// </summary>
        private ToscaObjectDefinition Definition;
        /// <summary>
        /// Builder object provides utility methods to create Tosca Business Objects with required parameters.
        /// </summary>
        private VEBToscaObjectBuilder Builder;

        /// <summary>
        /// Public Constructor
        /// </summary>
        /// <param name="definition">Definition Object</param>
        public ImportTask(ToscaObjectDefinition definition)
        {
            Definition = definition;
            //The 'Engine' parameter in ToscaObjectBuilder constructor is passed as null because we doen't need any specific engine for ManualTestCase.
            Builder = new VEBToscaObjectBuilder(Definition, null);
        }

        /// <summary>
        /// This method handles the migration of Manual TestCase
        /// </summary>
        /// <param name="filePath">File Path of the ManualTestCase Excel sheet.</param>
        public void ProcessManualTestCaseFile(string filePath, TCAddOnTaskContext taskContext)
        {
            //ClosedXML has been used here to read the excel files. The library can be found in Tosca Installation Directory at '%TRICENTIS_HOME%\ToscaCommander'.
            //Alternatively, Microsoft.Office.Interop.Excel library or other third-party library can also be used.
            XLWorkbook workBook = new XLWorkbook(filePath);

            int workSheetNumber = 0;
            foreach (IXLWorksheet sheet in workBook.Worksheets)
            {
                workSheetNumber++;
                // track worksheet number
                taskContext.ShowProgressInfo(workBook.Worksheets.Count, workSheetNumber, "Process worksheet: " + sheet.Name);
                
                IXLRange usedRange = sheet.RangeUsed();
                int testCaseFolderId = 0;
                int testCaseId = 0;
                int testStepId = 0;
                string testFolderName = "not resolved";
                string testCaseName = "not resolved";
                string testCaseDescription = "not resolved";
                string testStepName = "not resolved";

                // exported hp sheet will start in row = 5
                // example in row = 5
                // properties in row = 4

                // we only have to import "Import_Tests_QC"
                if (sheet.Name.Equals("Import_Tests_QC") || (workBook.Worksheets.Count == 1))
                {                   
                    for (int row = 1; row <= usedRange.RowCount(); row++)
                    {
                        //taskContext.ShowProgressInfo(usedRange.RowCount(), row, "Processed rows (Max: " + usedRange.RowCount() + ")");

                        // exported hp sheet has data starting at column = 2
                        for (int column = -5; column <= usedRange.ColumnCount(); column++)
                        {
                        	string cellValue = usedRange.Row(row).Cell(column).Value.ToString();
                        	//taskContext.ShowStatusInfo("Actual cell (Max: " + usedRange.ColumnCount() + ")" + cellValue + "( with subject: " + usedRange.Row(3).Cell(column).Value.ToString() + ")");
                            if (!string.IsNullOrEmpty(cellValue))
                            {
                                switch (usedRange.Row(3).Cell(column).Value.ToString())
                                {
                                	case "Subject":
                                		if(testFolderName == "not resolved" || !testFolderName.Equals(cellValue))
                        				{
											testFolderName = cellValue.Trim();
											//Creates TestFolder
                                        	testCaseFolderId = Builder.CreateFolder(testFolderName, FolderType.TestCases, Definition.TestCasesFolderId);
                        				}
                                		break;
                                    case "Test Name":
                                		if(!testCaseName.Equals(cellValue))
                                        {
                                			testCaseName = cellValue.Trim();
                                        	//Creates TestCase
                                        	testCaseId = Builder.CreateTestCase(testCaseName, testCaseDescription, testCaseFolderId);
                                        }
                                        break;
                                    case "Test Description":
                                        testCaseDescription = cellValue.Trim();
                                        break;
                                    case "Step Name":
                                        testStepName = cellValue.Trim();
                                        //Creates ManualTestStep
                                        testStepId = Builder.CreateManualTestStep(testStepName, testCaseId, null);
                                        break;
                                    case "Step Description":
                                        //Creates ManualTestStepValue with ActionMode as Input
                                        Builder.CreateManualTestStepValue(cellValue, testStepId, "DATA", ActionMode.Input.ToString(), null);
                                        break;
                                    case "Expected result":
                                        //Creates ManualTestStepValue with ActionMode as Verify
                                        Builder.CreateManualTestStepValue(cellValue, testStepId, "", ActionMode.Verify.ToString(), null);
                                        break;
                                }
                            }
                        }
                    }
                }
                else
                {
                    taskContext.ShowStatusInfo("Not in scope: " + sheet.Name);
                }
            }
        }
    }
}