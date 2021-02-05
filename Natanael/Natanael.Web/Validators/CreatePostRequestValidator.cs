using FluentValidation;
using Natanael.Web.Contracts.V1.Requests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Natanael.Web.Validators
{
    public class CreatePostRequestValidator : AbstractValidator<CreatePostRequest>
    {
        public CreatePostRequestValidator()
        {
            RuleFor(a => a.Name)
                .NotEmpty()
                .Matches("^[a-zA-Z0-9 ]*$");
        }

    }
}
