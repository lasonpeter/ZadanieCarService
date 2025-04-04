using FluentValidation;
using WebApplication1.Models;
using WebApplication1.Repositories;

namespace WebApplication1.Validators
{
    public class UserValidator : AbstractValidator<User>
    {
        private readonly IUserRepository _userRepository;
        private readonly ILogger<UserValidator> _logger; // Add logger

        public UserValidator(IUserRepository userRepository,ILogger<UserValidator> logger)
        {
            _logger = logger;
            _userRepository = userRepository;

            RuleFor(x => x.FirstName)
                .NotEmpty().
                MinimumLength(3)
                .MaximumLength(100);

            RuleFor(x => x.LastName)
                .NotEmpty()
                .MinimumLength(3)
                .MaximumLength(100);
            RuleFor(x => x.Email)
                .NotEmpty()
                .EmailAddress()
                .MaximumLength(255)
                .WithMessage("A user with this email already exists.")
                .Custom((email, context) => _userRepository.ExistsByEmail(email));
                

            RuleFor(x => x.PhoneNumber)
                .NotEmpty()
                .Length(9)
                .Matches("^[0-9]+$").WithMessage("The phoneNumber field must contain only numbers.")
                .Custom((s, context) => _userRepository.ExistsByPhoneNumber(s));
        }
    }
}