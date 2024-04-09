using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using FinTech.Domain.Entities;
using FluentValidation;

namespace FinTech.Domain.Validation;

public class TransactionValidator : AbstractValidator<Transaction>
{
    public TransactionValidator()
    {
        RuleFor(e => e.Id).GreaterThanOrEqualTo(0);
        RuleFor(e => e.Value).GreaterThan(0);
        RuleFor(e => e.CurrencyStr).NotEmpty();
        RuleFor(e => e.FinalValue).GreaterThan(0);
        RuleFor(e => e.OperationId).NotEmpty();
        RuleFor(e => e.Date).NotNull();
        RuleFor(e => e.FromAccountId).NotEmpty();
        RuleFor(e => e.ToAccountId).NotEmpty();
}
}