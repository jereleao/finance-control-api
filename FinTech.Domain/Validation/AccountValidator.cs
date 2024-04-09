using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using FinTech.Domain.Entities;
using FluentValidation;

namespace FinTech.Domain.Validation;

public class AccountValidator : AbstractValidator<Account>
{
    public AccountValidator()
    {
        RuleFor(e => e.Id).GreaterThanOrEqualTo(0);
        RuleFor(e => e.Name).NotEmpty();

        When(e => !string.IsNullOrWhiteSpace(e.Name), () =>
        {
            RuleFor(e => e.Name).Length(3, 50);
            RuleFor(e => e.Name).Must(name => !Utils.HasInvalidCharacters(name));
        });

        RuleFor(e => e.UserId).NotNull();

        When(e => e.ExpireDay.HasValue, () => RuleFor(e => e.ExpireDay).InclusiveBetween(1, 28));
        When(e => e.PaymentDay.HasValue, () => RuleFor(e => e.PaymentDay).InclusiveBetween(1, 28));

    }
}