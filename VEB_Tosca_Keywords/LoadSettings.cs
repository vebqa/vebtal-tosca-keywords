/*
 * Erstellt mit SharpDevelop.
 * Benutzer: doerges
 * Datum: 13.02.2017
 * Zeit: 13:13
 * 
 * Sie können diese Vorlage unter Extras > Optionen > Codeerstellung > Standardheader ändern.
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
	[SpecialExecutionTaskName("LoadSettings")]
	public class LoadSettings : SpecialExecutionTask
	{
		#region Constants
		private const string BufferCollectionPath = "Engine.Buffer";
		#endregion
		
		#region Public Methods and Operators
		
		public LoadSettings(Validator validator)
			: base(validator)
		{
		}

		public override ActionResult Execute(ISpecialExecutionTaskTestAction testAction)
		{
			String paraSettingsFile = testAction.GetParameterAsInputValue("File", false).Value;
			if (string.IsNullOrEmpty(paraSettingsFile)) {
				throw new ArgumentException(string.Format("Es muss eine Settingsdatei (File) angegeben sein."));
			}
			
			if (!Collections.Instance.CollectionExists(BufferCollectionPath)) {
				throw new InvalidOperationException("Es existiert keine Buffer-Collection in den Einstellungen!");
			}
			
			XmlDocument doc = new XmlDocument();
   			doc.Load(paraSettingsFile);

			XmlNode SettingsListNode = doc.SelectSingleNode("/root");
			XmlNodeList SettingsNodeList = SettingsListNode.SelectNodes("CollectionEntry");
			
			foreach (XmlNode node in SettingsNodeList)
            {
				string key = node.Attributes.GetNamedItem("name").Value;
				string value = node.InnerText;
				Buffers.Instance.SetBuffer(key, value, false);
			}

			return new PassedActionResult("settings loaded from : " + paraSettingsFile);
		}
		
		#endregion
	}
}

