using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using FinTech.Domain.Entities;
using FluentValidation;

namespace FinTech.Domain.Validation;

public class CategoryValidator : AbstractValidator<Category>
{
    public CategoryValidator()
    {
        RuleFor(e => e.Id).GreaterThanOrEqualTo(0);
        RuleFor(e => e.Name).NotEmpty();

        When(e => !string.IsNullOrWhiteSpace(e.Name), () =>
        {
            RuleFor(e => e.Name).Length(3, 50);
            RuleFor(e => e.Name).Must(name => !Utils.HasInvalidCharacters(name));
        });

        RuleFor(e => e.CategoryGroupId).NotEmpty();
    }
}