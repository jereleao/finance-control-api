using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using FinTech.Domain.Entities;
using FluentValidation;

namespace FinTech.Domain.Validation;

public class OperationValidator : AbstractValidator<Operation>
{
    public OperationValidator()
    {
        RuleFor(e => e.Id).GreaterThanOrEqualTo(0);
        RuleFor(e => e.Description).NotEmpty();

        When(e => !string.IsNullOrWhiteSpace(e.Description), () =>
        {
            RuleFor(e => e.Description).Length(3, 100);
            RuleFor(e => e.Description).Must(name => !Utils.HasInvalidCharacters(name));
        });

        RuleFor(e => e.CategoryId).NotEmpty();
        RuleFor(e => e.IsForecast).NotNull();
        RuleFor(e => e.CreatedBy).NotNull();
        RuleFor(e=> e.CreatedAt).NotNull();
    }
}