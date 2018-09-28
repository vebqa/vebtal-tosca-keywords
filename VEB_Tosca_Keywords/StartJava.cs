/*
 * Created by SharpDevelop.
 * User: doerges
 * Date: 10.11.2016
 * Time: 17:11
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading;

using Tricentis.Automation.AutomationInstructions.Dynamic.Values;
using Tricentis.Automation.AutomationInstructions.TestActions;
using Tricentis.Automation.Creation;
using Tricentis.Automation.Engines;
using Tricentis.Automation.Engines.SpecialExecutionTasks;
using Tricentis.Automation.Engines.SpecialExecutionTasks.Attributes;
using Tricentis.Automation.Execution.Results;

namespace WaitForFile {
[SpecialExecutionTaskName("StartProgram")]
	/// <summary>
	/// Description of StartJava.
	/// </summary>
	public class StartJava : SpecialExecutionTaskEnhanced
	{
        #region Constants

        private const string Argument = "Argument";

        private const string Arguments = "Arguments";

        private const string Directory = "Directory";

        private const string Path = "Path";

        private const string ProcessExitCode = "ExitCode";

        private const string StandardOutputFile = "StandardOutputFile";

        private const string TimeoutForExit = "TimeoutForExit";

        private const string WaitForExit = "WaitForExit";

        #endregion

        #region Constructors and Destructors

        public StartJava(Validator validator)
            : base(validator) {
        }

        #endregion

        #region Public Methods and Operators

        public override void ExecuteTask(ISpecialExecutionTaskTestAction testAction) {
            IParameter pathParameter = testAction.GetParameter(Path);
            string processArguments = null, expandedPath = null;
            ProcessStartInfo processStartInfo = null;
            try {
                processStartInfo = CreateProcessStartInfo(
                        testAction,
                        pathParameter,
                        out processArguments,
                        out expandedPath);
            }
            catch (FileNotFoundException e) {
                testAction.SetResultForParameter(pathParameter, new UnknownFailedActionResult("Cannot find the given file'.", e.ToString(), expandedPath ?? string.Empty));
                return;
            }
            IParameter exitParameter = testAction.GetParameter(WaitForExit, true, new[] { ActionMode.Select });
            if (exitParameter != null) {
                StartProgramWithWaitForExit(
                    testAction,
                    exitParameter,
                    processStartInfo,
                    pathParameter,
                    expandedPath,
                    processArguments);
            }
            else {
                StartProgramWithoutWaitingForExit(
                    testAction,
                    processStartInfo,
                    expandedPath,
                    processArguments,
                    pathParameter);
            }
        }

        #endregion

        #region Methods

        protected static ProcessStartInfo CreateProcessStartInfo(
            ISpecialExecutionTaskTestAction testAction,
            IParameter pathParameter,
            out string processArguments,
            out string expandedPath) {
            IInputValue path = pathParameter.GetAsInputValue();
            IInputValue directoryValue = testAction.GetParameterAsInputValue(Directory, true);
            IParameter argumentsParameter = testAction.GetParameter(Arguments, true);
            processArguments = string.Empty;
            if (string.IsNullOrEmpty(path.Value)) {
                throw new ArgumentException("Mandatory parameter '{Path}' not set.");
            }
            string directory = GetDirectory(directoryValue);
            if (argumentsParameter != null) {
                processArguments = ParseArguments(argumentsParameter);
            }
            expandedPath = Environment.ExpandEnvironmentVariables(path.Value);
            ProcessStartInfo processStartInfo = new ProcessStartInfo(expandedPath, processArguments);
            if (!string.IsNullOrEmpty(directory)) {
                processStartInfo.WorkingDirectory = directory;
            }
            return processStartInfo;
        }

        private static string CreateMessageForStartError(
            string expandedPath,
            string processArguments,
            Exception exception) {
            return
                string.Format(
                    "Failed while trying to start:\nPath: {0}\r\nArguments: {1}\r\n{2} occured with Message '{3}'. See 'Details' for Stacktrace",
                    expandedPath,
                    processArguments,
                    exception.GetType().Name,
                    exception.Message);
        }

        private static bool GetAutoAddDoubleQuotes(IParameter argumentsParameter) {
            String autoAddDoubleQuotesParam = GetConfigurationParameter(argumentsParameter, "AutoAddDoubleQuotes");
            bool autoAddDoubleQuotes = true;
            if (autoAddDoubleQuotesParam != null) {
                if (!bool.TryParse(autoAddDoubleQuotesParam, out autoAddDoubleQuotes)) {
                    throw new InvalidOperationException(
                        "Please use either 'True' or 'False' for the ConfigurationParameter 'AutoAddDoubleQuotes'");
                }
            }
            
            // Aunahmen 
            // return false;
            return autoAddDoubleQuotes;
        }

        private static String GetConfigurationParameter(IParameter parameter, String nameOfConfigurationParameter) {
            List<IModuleAttributeXParameter> xparams =
                new List<IModuleAttributeXParameter>(parameter.XParameters.ConfigurationParameters);
            String valueOfXParam;
            if (!xparams.TryGetSingleXParameterValue(nameOfConfigurationParameter, out valueOfXParam)) {
                return null;
            }
            return valueOfXParam;
        }

        private static string GetDirectory(IInputValue directoryValue) {
            string directory = null;
            if (directoryValue != null) {
                directory = Environment.ExpandEnvironmentVariables(directoryValue.Value);
            }
            return directory;
        }

        private static string ParseArguments(IParameter argumentsParameter) {
            bool autoAddDoubleQuotes = GetAutoAddDoubleQuotes(argumentsParameter);
            IEnumerable<IParameter> arguments = argumentsParameter.GetChildParameters(Argument);
            const string Doublequotes = "\"";
            List<String> argumentValues = new List<string>();
            foreach (IParameter argument in arguments) {
                IInputValue processArgument = argument.GetAsInputValue();
                string value = processArgument.Value;
                value = Environment.ExpandEnvironmentVariables(value);
                if (autoAddDoubleQuotes && value.Contains(" ")
                    && !(value.StartsWith(Doublequotes) && value.EndsWith(Doublequotes))) {
                    value = Doublequotes + value + Doublequotes;
                }
                argumentValues.Add(value);
            }
            return String.Join(" ", argumentValues.ToArray());
        }
        
        private static String StartProcessWithWaitForExit(
            ProcessStartInfo processStartInfo,
            bool redirectStdOut,
            int timeout,
            out int exitCode,
            out bool wasTimeout) {
            StringBuilder output = new StringBuilder();
            Process process = new Process();
            processStartInfo.UseShellExecute = false;
            if (redirectStdOut) {
                processStartInfo.RedirectStandardOutput = true;
            }
            process.StartInfo = processStartInfo;
            exitCode = -1;
            wasTimeout = false;
            using (AutoResetEvent outputWaitHandle = new AutoResetEvent(false)) {
                DataReceivedEventHandler action = (sender, e) => {
                    if (e.Data == null) {
                        // ReSharper disable once AccessToDisposedClosure
                        outputWaitHandle.Set();
                    } else {
                        output.AppendLine(e.Data);
                    }
                };
                if (redirectStdOut) {
                    process.OutputDataReceived += action;
                }
                try {
                    process.Start();
                }
                catch (Win32Exception) {
                    if (string.IsNullOrEmpty(processStartInfo.WorkingDirectory)) throw;
                    processStartInfo.FileName = System.IO.Path.Combine(processStartInfo.WorkingDirectory,
                                                                       processStartInfo.FileName);
                    process.Start();
                }
                if (redirectStdOut) {
                    process.BeginOutputReadLine();
                }
                if (process.WaitForExit(timeout) && (!redirectStdOut || outputWaitHandle.WaitOne(timeout))) {
                    exitCode = process.ExitCode;
                }
                else {
                    wasTimeout = true;
                }
                if (redirectStdOut) {
                    process.OutputDataReceived -= action;
                }
            }
            return output.ToString();
        }

        private static void StartProgramWithoutWaitingForExit(
            ISpecialExecutionTaskTestAction testAction,
            ProcessStartInfo processStartInfo,
            string expandedPath,
            string processArguments,
            IParameter pathParameter) {
            try {
                Process.Start(processStartInfo);
                testAction.SetResult(
                    SpecialExecutionTaskResultState.Ok,
                    "Started: " + expandedPath + " with arguments: " + processArguments);
            }
            catch (Win32Exception e) {
                testAction.SetResultForParameter(
                    pathParameter,
                    SpecialExecutionTaskResultState.Failed,
                    CreateMessageForStartError(expandedPath, processArguments, e),
                    e.StackTrace,
                    "");
            }
        }

        private void StartProgramWithWaitForExit(
            ISpecialExecutionTaskTestAction testAction,
            IParameter exitParameter,
            ProcessStartInfo processStartInfo,
            IParameter pathParameter,
            string expandedPath,
            string processArguments) {
            if (exitParameter.GetAsInputValue().Verify("True", exitParameter.Operator) is FailedActionResult) {
                throw new InvalidOperationException(
                    "Please use 'True' as value for " + exitParameter.Name
                    + ". If you don't want to wait for exit of the application, please delete the node. The items below won't work without waiting for exit.");
            }
            IParameter stdoutParameter = exitParameter.GetChildParameter(StandardOutputFile, true);
            IParameter exitCodeParameter = exitParameter.GetChildParameter(
                ProcessExitCode,
                true,
                new[] { ActionMode.Buffer, ActionMode.Verify });
            IInputValue timeoutForExitValue = exitParameter.GetChildParameterAsInputValue(TimeoutForExit, true);

            int timeoutForExit;
            if (timeoutForExitValue != null) {
                if (!int.TryParse(timeoutForExitValue.Value, out timeoutForExit)) {
                    throw new InvalidOperationException(
                        "Please use a valid integer value for the Parameter 'TimeoutForExit'");
                }
                timeoutForExit = timeoutForExit * 1000;
            }
            else {
                timeoutForExit = int.MaxValue;
            }
            string output;
            try {
                int exitCode;
                bool wasTimeout;
                Stopwatch stopwatch = new Stopwatch();
                stopwatch.Start();
                output = StartProcessWithWaitForExit(
                    processStartInfo,
                    stdoutParameter != null,
                    timeoutForExit,
                    out exitCode,
                    out wasTimeout);
                stopwatch.Stop();
                testAction.SetResult(
                    SpecialExecutionTaskResultState.Ok,
                    string.Format("Started '{0}' with arguments '{1}'", expandedPath, processArguments));
                if (wasTimeout) {
                    testAction.SetResultForParameter(
                        pathParameter,
                        SpecialExecutionTaskResultState.Failed,
                        string.Format(
                            "Timeout occured, process didn't stop within the timeout of {0} seconds!",
                            timeoutForExit / 1000));
                }
                else {
                    testAction.SetResultForParameter(
                        pathParameter,
                        SpecialExecutionTaskResultState.Ok,
                        string.Format(
                            "Process stopped after '{0}' minutes.",
                            Math.Round(stopwatch.Elapsed.TotalMinutes)));
                    if (exitCodeParameter != null) {
                        HandleActualValue(testAction, exitCodeParameter, exitCode);
                    }
                }
            }
            catch (Win32Exception e) {
                testAction.SetResultForParameter(
                    pathParameter,
                    SpecialExecutionTaskResultState.Failed,
                    CreateMessageForStartError(expandedPath, processArguments, e),
                    e.StackTrace,
                    "");
                return;
            }
            if (stdoutParameter != null && output != null) {
                String filePath = Environment.ExpandEnvironmentVariables(stdoutParameter.GetAsInputValue().Value);
                File.WriteAllText(filePath, output);
                testAction.SetResultForParameter(
                    stdoutParameter,
                    new PassedActionResult(
                        "The Standard-Output was saved inside the given file. See 'Details'-column for the path.",
                        filePath,
                        String.Empty));
            }
        }

        #endregion
    }
}
