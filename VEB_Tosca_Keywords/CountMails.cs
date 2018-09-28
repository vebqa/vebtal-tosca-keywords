/*
 * Created by SharpDevelop.
 * User: doerges
 * Date: 10.01.2017
 * Time: 15:27
 * 
 * Implementierung des Keyword "CountMails". Es wird die Anzahl der in einer Mailbox enthaltenen Messages ohne weiteren Text zurueckgegeben.
 */

using System;
using System.Collections.Generic;
using System.Linq;
using OpenPop.Mime;
using OpenPop.Pop3;
using OpenPop.Pop3.Exceptions;
using Tricentis.Automation.Execution.Results.Factory;
using Message = OpenPop.Mime.Message;

using Tricentis.Automation.AutomationInstructions.TestActions;
using Tricentis.Automation.Creation;
using Tricentis.Automation.Engines;
using Tricentis.Automation.Engines.SpecialExecutionTasks;
using Tricentis.Automation.Engines.SpecialExecutionTasks.Attributes;

namespace veb
{
	[SpecialExecutionTaskName("CountMails")]
	public class CountMails : SpecialExecutionTask
	{
		// POP Client aus OpenPop
		private Pop3Client pop3Client;
		
		// Liste der eingegangenen Messages
		private Dictionary<int, Message> messages;
		
		public CountMails(Validator validator)
			: base(validator)
		{
		}

		public override ActionResult Execute(ISpecialExecutionTaskTestAction testAction)
		{
			// Host auf dem der POP3 Service laeuft
			String host = testAction.GetParameterAsInputValue("host", false).Value;
			if (string.IsNullOrEmpty(host)) {
				throw new ArgumentException(string.Format("Es muss ein POP3 Host angegeben sein."));
			}
			
			// Port fuer den POP Service
			String strPort = testAction.GetParameterAsInputValue("port", false).Value;
			if (string.IsNullOrEmpty(strPort)) {
				throw new ArgumentException(string.Format("Es muss ein POP3 Port angegeben sein."));
			}
			int port = int.Parse(strPort);

			// User Name			
			String user = testAction.GetParameterAsInputValue("user", false).Value;
			if (string.IsNullOrEmpty(user)) {
				throw new ArgumentException(string.Format("Es muss ein User angegeben werden."));
			}

			// Password			
			String password = testAction.GetParameterAsInputValue("password", false).Value;
			if (string.IsNullOrEmpty(password)) {
				throw new ArgumentException(string.Format("Es muss ein Passwort angegeben werden."));
			}

			pop3Client = new Pop3Client();
            messages = new Dictionary<int, Message>();
			int count = 0;
			
			try
			{
				if (pop3Client.Connected)
					pop3Client.Disconnect();
				pop3Client.Connect(host, port, false);
				pop3Client.Authenticate(user, password);
				count = pop3Client.GetMessageCount();
			}
			catch (InvalidLoginException e)
			{
				return new UnknownFailedActionResult("Login failed: " + e.Message);
			}
			catch (PopServerNotFoundException e)
			{
				return new UnknownFailedActionResult("The server could not be found:" + e.Message);
			}
			catch (PopServerLockedException e)
			{
				return new UnknownFailedActionResult("The mailbox is locked. It might be in use or under maintenance. Are you connected elsewhere: " + e.Message);
			}
			catch (LoginDelayException e)
			{
				return new UnknownFailedActionResult("Login not allowed. Server enforces delay between logins. Have you connected recently: " + e.Message);
			}
			catch (Exception e)
			{
				return new PassedActionResult("Error occurred retrieving mail: " + e.Message);
			}
			finally
			{
				pop3Client.Disconnect();
			}
			
			return new PassedActionResult(Convert.ToString(count));
		}
	}
}

