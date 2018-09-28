/*
 * User: doerges
 * Date: 09.01.2017
 * Time: 11:54
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
	[SpecialExecutionTaskName("CheckMail")]
	public class CheckMail : SpecialExecutionTask
	{
		private Pop3Client pop3Client;
		private Dictionary<int, Message> messages;
		private String logMessages;
		
		public CheckMail(Validator validator)
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

			string expectedSubject = testAction.GetParameterAsInputValue("expectedSubject", false).Value;
			string expectedBody = testAction.GetParameterAsInputValue("expectedBody", false).Value;
			
			pop3Client = new Pop3Client();
            messages = new Dictionary<int, Message>();
            logMessages = "";
			int success = 0;
			
			string body = "";
			string subject = "";
			
			try
			{
				if (pop3Client.Connected)
					pop3Client.Disconnect();
				pop3Client.Connect(host, port, false);
				pop3Client.Authenticate(user, password);
				int count = pop3Client.GetMessageCount();
				
				if (count > 1) {
					return new  NotFoundFailedActionResult("Command failed: There is more than one email for this user. Clear mailbox before testing!");
				}
				
				if (count == 0) {
					return new  NotFoundFailedActionResult("Command failed: There is no email waiting for this user!");
				}
				
				messages.Clear();
				int fail = 0;
				for (int i = count; i >= 1; i -= 1)
				{
					try
					{
						// Application.DoEvents();
						Message message = pop3Client.GetMessage(i);
						MessageHeader headers = pop3Client.GetMessageHeaders(i);
						subject = headers.Subject;
						
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
			catch (InvalidLoginException e)
			{
                return new VerifyFailedActionResult("Expected user mailbox: " + user + " with password: " + password,  e.Message);
            }
			catch (PopServerNotFoundException e)
			{
                return new UnknownFailedActionResult("The server could not be found" + e.Message);
            }
			catch (PopServerLockedException e)
			{
                return new UnknownFailedActionResult("The mailbox is locked. It might be in use or under maintenance. Are you connected elsewhere?" + e.Message);
            }
			catch (LoginDelayException e)
			{
                return new UnknownFailedActionResult("Login not allowed. Server enforces delay between logins. Have you connected recently?" + e.Message);
            }
			catch (Exception e)
			{
				return new UnknownFailedActionResult("Error occurred retrieving mail: " + e.Message);
			}
			finally
			{
				pop3Client.Disconnect();
			}
			
			if (body.Contains(expectedBody) && (subject.Contains(expectedSubject))) {
				return new VerifyPassedActionResult("Subject:" + expectedSubject + "\n\rBody:" + expectedBody, "Subject: " + subject + "\n\nBody: " + body);
			} else {
                string resultMessage = "";
                if (body.Contains(expectedBody))
                {
                    resultMessage = "Body is correct.";
                } else
                {
                    resultMessage = "Body is not correct.";
                }
                if (subject.Contains(expectedSubject))
                {
                    resultMessage = resultMessage + " Subject is correct.";
                } else
                {
                    resultMessage = resultMessage + " Subject is not correct.";
                }
				return new VerifyFailedActionResult(resultMessage + "\n\r" + "Subject:" + expectedSubject + "\n\rBody:" + expectedBody, "\n\rSubject:\n\r" + subject + "\n\rBody:" + body);
			}
		}
	}
}
