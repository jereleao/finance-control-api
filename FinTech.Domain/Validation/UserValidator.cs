using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using FinTech.Domain.Entities;
using FluentValidation;

namespace FinTech.Domain.Validation;

public class UserValidator : AbstractValidator<User>
{
    public UserValidator()
    {
        RuleFor(e => e.Id).GreaterThanOrEqualTo(0);
        RuleFor(e => e.Name).NotEmpty();

        When(e => !string.IsNullOrWhiteSpace(e.Name), () =>
        {
            RuleFor(e => e.Name).Length(3, 25);
            RuleFor(e => e.Name).Must(name => !Utils.HasInvalidCharacters(name));
        });

        RuleFor(e => e.Email).Matches(@"^([a-zA-Z0-9_\-\.]+)@((\[[0-9]{1,3}" +
         @"\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([a-zA-Z0-9\-]+\" +
         @".)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$");

        RuleFor(e => e.Password).NotEmpty();
        RuleFor(e => e.VerifiedEmail).NotNull();
    }
}