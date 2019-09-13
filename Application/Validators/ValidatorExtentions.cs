using FluentValidation;

namespace Application.Validators
{
    public static class ValidatorExtentions
    {
        // Custom password validator fluentValidation
        public static IRuleBuilder<T, string> Password<T>(this IRuleBuilder<T, string> ruileBuilder)
        {
            var options = ruileBuilder.NotEmpty()
                    .MinimumLength(6).WithMessage("Password must be 6 characters long")
                    .Matches("[A-Z]").WithMessage("Password must contain 1 uppercase letter")
                    .Matches("[a-z]").WithMessage("Password must contain 1 lowercase letter")
                    .Matches("[^a-zA-Z0-9]").WithMessage("Password must contain 1 special character")
                    .Matches("[0-9]").WithMessage("Password must contain 1 number");

            return options;
        }
    }
}