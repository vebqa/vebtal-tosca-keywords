using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AS.IBAN.Helper;
using AS.IBAN.Manager;
using AS.IBAN.Model;
using Tricentis.Automation.AutomationInstructions.TestActions;
using Tricentis.Automation.Creation;
using Tricentis.Automation.Engines;
using Tricentis.Automation.Engines.SpecialExecutionTasks;
using Tricentis.Automation.Execution.Results;
using Tricentis.Automation.AutomationInstructions.Configuration;

namespace veb
{
    class GenerateIban : SpecialExecutionTask
    {
        private IbanBic iban;
        private IIbanManager _manager;

        public GenerateIban(Validator validator) : base(validator)
        {
        }

        public override ActionResult Execute(ISpecialExecutionTaskTestAction testAction)
        {
            String bankIdent = testAction.GetParameterAsInputValue("bankIdent", false).Value;
            if (string.IsNullOrEmpty(bankIdent))
            {
                throw new ArgumentException(string.Format("Es muss eine Bank identifikation angegeben sein."));
            }

            String accountNumber = testAction.GetParameterAsInputValue("accountNumber", false).Value;
            if (string.IsNullOrEmpty(accountNumber))
            {
                throw new ArgumentException(string.Format("Es muss eine Kontonummer angegeben sein."));
            }

            String buffer = testAction.GetParameterAsInputValue("buffer", false).Value;
            if (string.IsNullOrEmpty(buffer))
            {
                throw new ArgumentException(string.Format("Es muss ein Puffer angegeben werden in welche die IBAN gespeichert werden soll."));
            }

            // onyl support DE so far
            ECountry countryCode = ECountry.DE;
            // DE46 | 5054 0028 | 0420 0861 00

            _manager = ContainerBootstrapper.Resolve<IIbanManager>(countryCode.ToString());
            iban = _manager.GenerateIban(countryCode, bankIdent, accountNumber);

            Buffers.Instance.SetBuffer(buffer, iban.IBAN.IBAN, false);

            testAction.SetResult(SpecialExecutionTaskResultState.Ok, "IBAN generated and saved to buffer.");

            throw new NotImplementedException();
        }

    }
}
