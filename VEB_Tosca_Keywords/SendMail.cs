/*
 * Created by SharpDevelop.
 * User: doerges
 * Date: 09.01.2017
 * Time: 11:10
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */

using System;
using System.Net.Mail;

using Tricentis.Automation.AutomationInstructions.TestActions;
using Tricentis.Automation.Creation;
using Tricentis.Automation.Engines;
using Tricentis.Automation.Engines.SpecialExecutionTasks;
using Tricentis.Automation.Engines.SpecialExecutionTasks.Attributes;

namespace veb
{
	[SpecialExecutionTaskName("SendMail")]
	public class SendMail : SpecialExecutionTask
	{
		public SendMail(Validator validator)
			: base(validator)
		{
		}

		public override ActionResult Execute(ISpecialExecutionTaskTestAction testAction)
		{
			String relay = testAction.GetParameterAsInputValue("relay", false).Value;
			if (string.IsNullOrEmpty(relay)) {
				throw new ArgumentException(string.Format("Es muss ein SMTP Relay angegeben sein."));
			}
			
			String sendTo = testAction.GetParameterAsInputValue("to", false).Value;
			if (string.IsNullOrEmpty(sendTo)) {
				throw new ArgumentException(string.Format("Es muss ein Empfänger angegeben werden."));
			}
			
			String sendFrom = testAction.GetParameterAsInputValue("from", false).Value;
			if (string.IsNullOrEmpty(sendFrom)) {
				throw new ArgumentException(string.Format("Es muss ein Absender angegeben werden."));
			}

			String msgSubject = testAction.GetParameterAsInputValue("subject", false).Value;
			if (string.IsNullOrEmpty(msgSubject)) {
				throw new ArgumentException(string.Format("Es muss ein Betreff angegeben sein."));
			}

			String msgBody = testAction.GetParameterAsInputValue("body", false).Value;
			if (string.IsNullOrEmpty(msgBody)) {
				throw new ArgumentException(string.Format("Es muss ein Text (Body) angegeben sein."));
			}
			
			MailMessage message = new MailMessage(sendFrom, sendTo);
			message.Subject = msgSubject;
			message.Body = @msgBody;
			
			using (SmtpClient client = new SmtpClient(relay)) {
				// client.UseDefaultCredentials = true;

				try {
					client.Send(message);
					return new PassedActionResult("Message successfully sent.");
				}
				catch (Exception ex) {
					return new NotFoundFailedActionResult("Exception caught in CreateTestMessage2(): {0}",
					                                      ex.ToString() );
				} finally {
					
				}
			}
		}
	}
}