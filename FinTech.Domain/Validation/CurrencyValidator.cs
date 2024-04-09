using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using FinTech.Domain.Entities;
using FluentValidation;

namespace FinTech.Domain.Validation;

public class CurrencyValidator : AbstractValidator<Currency>
{
    public CurrencyValidator()
    {
        RuleFor(e => e.Id).Length(3);
        RuleFor(e => e.CurrentRatio).NotEmpty();
    }
}