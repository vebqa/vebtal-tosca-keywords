/*
 * Created by SharpDevelop.
 * User: doerges
 * Date: 04.11.2016
 * Time: 08:32
 * 
 * Dieses Keyword generiert eine Import Datei fuer die Verwendung z.B. im Selenium Kontext.
 */
using System;
using System.Xml;
using System.Collections.Generic;

using Tricentis.Automation.AutomationInstructions.Configuration;
using Tricentis.Automation.Engines;
using Tricentis.Automation.AutomationInstructions.TestActions;
using Tricentis.Automation.Creation;
using Tricentis.Automation.Engines.SpecialExecutionTasks;
using Tricentis.Automation.Engines.SpecialExecutionTasks.Attributes;

namespace veb
{
	[SpecialExecutionTaskName("GenerateExchange")]
	public class GenerateExchange : SpecialExecutionTask
	{
		#region Constants
		private const string BufferCollectionPath = "Engine.Buffer";
		#endregion
		
		#region Public Methods and Operators
		
		public GenerateExchange(Validator validator)
			: base(validator)
		{
		}

		public override ActionResult Execute(ISpecialExecutionTaskTestAction testAction)
		{
			String paraExchangeFile = testAction.GetParameterAsInputValue("File", false).Value;
			if (string.IsNullOrEmpty(paraExchangeFile)) {
				throw new ArgumentException(string.Format("Es muss eine Exchangedatei (File) angegeben sein."));
			}
			
			if (!Collections.Instance.CollectionExists(BufferCollectionPath)) {
				throw new InvalidOperationException("Es existiert keine Buffer-Collection in den Einstellungen!");
			}
			
			XmlDocument doc = new XmlDocument();

			XmlDeclaration xmlDeclaration = doc.CreateXmlDeclaration("1.0", "UTF-8", null);
			XmlElement root = doc.DocumentElement;
			doc.InsertBefore(xmlDeclaration, root);

			XmlElement bodyElement = doc.CreateElement(string.Empty, "exchange", string.Empty);
			doc.AppendChild(bodyElement);

			XmlElement varElement = doc.CreateElement(string.Empty, "variables", string.Empty);
			bodyElement.AppendChild(varElement);

			foreach (
				
				KeyValuePair<string, string> buffer in Collections.Instance.GetCollectionEntries(BufferCollectionPath)) {
				if (!string.IsNullOrEmpty(buffer.Value) && !buffer.Key.StartsWith("#")) {
					XmlElement varKey = doc.CreateElement(string.Empty, buffer.Key, string.Empty);
					XmlText varValue;
					if (buffer.Key == "QAPass") {
						varValue = doc.CreateTextNode("****");
						varKey.AppendChild(varValue);
					} else {
						varValue = doc.CreateTextNode(buffer.Value);
						varKey.AppendChild(varValue);
					}
					varElement.AppendChild(varKey);
				}
			}

			doc.Save(paraExchangeFile);
			
			return new PassedActionResult("Exchange generated to: " + paraExchangeFile);
		}
		
		#endregion
	}
}

