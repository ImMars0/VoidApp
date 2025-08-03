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

            if (password != confirmPassword)
                throw new ArgumentException("Passwords don't match");


            ValidatePassword(password);
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

        private void ValidatePassword(string password)
        {
            if (password.Length < 6)
                throw new ArgumentException("Password must be at least 6 characters");
            if (!Regex.IsMatch(password, @"[A-Z]"))
                throw new ArgumentException("Password must contain at least one uppercase letter");
            if (!Regex.IsMatch(password, @"[a-z]"))
                throw new ArgumentException("Password must contain at least one lowercase letter");
            if (!Regex.IsMatch(password, @"[0-9]"))
                throw new ArgumentException("Password must contain at least one digit");
        }

        private void ValidateEmail(string email)
        {

            string pattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
            if (string.IsNullOrEmpty(email) || !Regex.IsMatch(email, pattern))
                throw new ArgumentException("Invalid email format");




        }
    }
}