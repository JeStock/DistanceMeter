namespace CTeleport.DistanceMeter.Api.Validators
{
    using System.Text.RegularExpressions;
    using Application.Constants;
    using Domain.Models;
    using FluentValidation;

    internal class IataCoupleValidator : AbstractValidator<IataCouple>
    {
        public IataCoupleValidator()
        {
            RuleFor(couple => couple.FirstIata)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .Must(BeValidIataCode)
                .WithMessage(ErrorMessages.InputValidationErrorMessage);
            
            RuleFor(couple => couple.SecondIata)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .Must(BeValidIataCode)
                .WithMessage(ErrorMessages.InputValidationErrorMessage);

            RuleFor(couple => couple)
                .Must(couple => couple.FirstIata != couple.SecondIata)
                .WithMessage(ErrorMessages.InputValidationSameIataErrorMessage);
        }

        private static bool BeValidIataCode(string iata)
        {
            return Regex.IsMatch(iata, ValidationConstants.ValidIataCode);
        }
    }
}