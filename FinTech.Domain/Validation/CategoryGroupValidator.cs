using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using FinTech.Domain.Entities;
using FluentValidation;

namespace FinTech.Domain.Validation;

public class CategoryGroupValidator : AbstractValidator<CategoryGroup>
{
    public CategoryGroupValidator()
    {
        RuleFor(e => e.Id).GreaterThanOrEqualTo(0);
        RuleFor(e => e.Name).NotEmpty();

        When(e => !string.IsNullOrWhiteSpace(e.Name), () =>
        {
            RuleFor(e => e.Name).Length(3, 50);
            RuleFor(e => e.Name).Must(name => !Utils.HasInvalidCharacters(name));
        });

        When(e => !string.IsNullOrWhiteSpace(e.Description), () =>
        {
            RuleFor(e => e.Description).Length(10, 255);
            RuleFor(e => e.Description).Must(description => !Utils.HasInvalidCharacters(description));
        });

    }
}