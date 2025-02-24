using Application.Commands;
using FluentValidation;

namespace Application.FluentValidations;

public class RefreshTokenCommandValidator : AbstractValidator<RefreshTokenCommand>
{
    public RefreshTokenCommandValidator()
    {
        RuleFor(x => x.RefreshToken).NotEmpty()
            .WithMessage("Refresh token is required");
    }
}