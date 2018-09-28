/*
 * Created by SharpDevelop.
 * User: doerges
 * Date: 09.01.2017
 * Time: 11:54
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using OpenPop.Mime;
using OpenPop.Pop3;
using OpenPop.Pop3.Exceptions;
using OpenPop.Mime.Header;
using Message = OpenPop.Mime.Message;

using Tricentis.Automation.AutomationInstructions.TestActions;
using Tricentis.Automation.Creation;
using Tricentis.Automation.Engines;
using Tricentis.Automation.Engines.SpecialExecutionTasks;
using Tricentis.Automation.Engines.SpecialExecutionTasks.Attributes;

namespace veb
{
	[SpecialExecutionTaskName("ListMails")]
	public class ListMails : SpecialExecutionTask
	{
		private Pop3Client pop3Client;
		private Dictionary<int, Message> messages;
		private String logMessages;
		
		public ListMails(Validator validator)
			: base(validator)
		{
		}

		public override ActionResult Execute(ISpecialExecutionTaskTestAction testAction)
		{
			String host = testAction.GetParameterAsInputValue("host", false).Value;
			if (string.IsNullOrEmpty(host)) {
				throw new ArgumentException(string.Format("Es muss ein POP3 Host angegeben sein."));
			}
			
			String strPort = testAction.GetParameterAsInputValue("port", false).Value;
			if (string.IsNullOrEmpty(strPort)) {
				throw new ArgumentException(string.Format("Es muss ein POP3 Port angegeben sein."));
			}
						
			int port = int.Parse(strPort);
			
			String user = testAction.GetParameterAsInputValue("user", false).Value;
			if (string.IsNullOrEmpty(user)) {
				throw new ArgumentException(string.Format("Es muss ein User angegeben werden."));
			}
			
			String password = testAction.GetParameterAsInputValue("password", false).Value;
			if (string.IsNullOrEmpty(password)) {
				throw new ArgumentException(string.Format("Es muss ein Passwort angegeben werden."));
			}


			pop3Client = new Pop3Client();
            messages = new Dictionary<int, Message>();
            logMessages = "";
			int success = 0;
			
			try
			{
				if (pop3Client.Connected)
					pop3Client.Disconnect();
				pop3Client.Connect(host, port, false);
				pop3Client.Authenticate(user, password);
				int count = pop3Client.GetMessageCount();
				messages.Clear();
				int fail = 0;
				for (int i = count; i >= 1; i -= 1)
				{
					try
					{
						// Application.DoEvents();
						string body;
						Message message = pop3Client.GetMessage(i);
						MessageHeader headers = pop3Client.GetMessageHeaders(i);
						string subject = headers.Subject;
						
						logMessages = logMessages + "- " + i + " -" + subject + "\n";
						
						MessagePart plainTextPart = message.FindFirstPlainTextVersion();
						if (plainTextPart != null)
						{
							// The message had a text/plain version - show that one
							body = plainTextPart.GetBodyAsText();
						}
						else
						{
							// Try to find a body to show in some of the other text versions
							List<MessagePart> textVersions = message.FindAllTextVersions();
							if (textVersions.Count >= 1)
								body = textVersions[0].GetBodyAsText();
							else
								body = "<<OpenPop>> Cannot find a text version body in this message to show <<OpenPop>>";
						}
						// Build up the attachment list
						List<MessagePart> attachments = message.FindAllAttachments();
						foreach (MessagePart attachment in attachments)
						{ }
						
						// Add the message to the dictionary from the messageNumber to the Message
						messages.Add(i, message);
						
						
						success++;
					}
					catch (Exception e)
					{
						fail++;
					}
				}
			}
			catch (InvalidLoginException)
			{
				//MessageBox.Show(this, "The server did not accept the user credentials!", "POP3 Server Authentication");
			}
			catch (PopServerNotFoundException)
			{
				//MessageBox.Show(this, "The server could not be found", "POP3 Retrieval");
			}
			catch (PopServerLockedException)
			{
				//MessageBox.Show(this, "The mailbox is locked. It might be in use or under maintenance. Are you connected elsewhere?", "POP3 Account Locked");
			}
			catch (LoginDelayException)
			{
				//MessageBox.Show(this, "Login not allowed. Server enforces delay between logins. Have you connected recently?", "POP3 Account Login Delay");
			}
			catch (Exception e)
			{
				return new PassedActionResult("Error occurred retrieving mail: " + e.Message);
			}
			finally
			{
				pop3Client.Disconnect();
			}
			
			return new PassedActionResult("Mails found: " + success + "\n" + logMessages);
		}
	}
}
