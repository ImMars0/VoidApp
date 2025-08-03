using System.Text.RegularExpressions;
using user.Models;
using Void.Repositories;

namespace Void.Services
{
    public class AuthenticationService
    {
        private readonly UserRepository _userRepository;

        public AuthenticationService(UserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public void Register(string username, string password, string confirmPassword, string email)
        {
            ValidatePassword(password, confirmPassword);
            ValidateEmail(email);

            if (_userRepository.UserExists(username))
                throw new ArgumentException("Username already exists");


            var user = new User
            {
                UserName = username,
                Password = password,
                Email = email
            };

            _userRepository.AddUser(user);
        }

        public bool Login(string username, string password)
        {
            return _userRepository.ValidateUser(username, password);
        }

        private void ValidatePassword(string password, string confirmPassword)
        {
            var errors = new List<string>();

            if (password.Length < 6)
                errors.Add("Password must be at least 6 characters");
            if (!Regex.IsMatch(password, @"[A-Z]"))
                errors.Add("Password must contain at least one uppercase letter");
            if (!Regex.IsMatch(password, @"[a-z]"))
                errors.Add("Password must contain at least one lowercase letter");
            if (!Regex.IsMatch(password, @"[0-9]"))
                errors.Add("Password must contain at least one digit");
            if (password != confirmPassword)
                errors.Add("Passwords don't match");

            if (errors.Any())
                throw new ArgumentException(string.Join(Environment.NewLine, errors));
        }

        private void ValidateEmail(string email)
        {

            string pattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
            if (string.IsNullOrEmpty(email) || !Regex.IsMatch(email, pattern))
                throw new ArgumentException("Invalid email format");




        }
    }
}