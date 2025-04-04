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
                .NotEmpty()
                .MaximumLength(100);

            RuleFor(x => x.LastName)
                .NotEmpty()
                .MaximumLength(100);

            RuleFor(x => x.Email)
                .NotEmpty()
                .EmailAddress()
                .MaximumLength(255)
                /*.MustAsync(async (email, cancellation) => !await _userRepository.ExistsByEmailAsync(email))*/
                .WithMessage("A user with this email already exists.");

            RuleFor(x => x.PhoneNumber)
                .NotEmpty()
                .Length(9)
                .Custom((s, context) => _userRepository.ExistsByPhoneNumber(s));
        }
    }
}