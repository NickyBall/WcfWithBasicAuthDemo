using System;
using System.IdentityModel.Selectors;
using System.IdentityModel.Tokens;

namespace WcfService
{
    public class CustomUserNamePasswordValidator : UserNamePasswordValidator
    {
        public override void Validate(string userName, string password)
        {
            // Write your own username and password validation logic here.

            // Sample logic below.
            if (string.IsNullOrWhiteSpace(userName) || string.IsNullOrWhiteSpace(password))
            {
                throw new ArgumentNullException("Username and password are mandatory");
            }

            if (!userName.Equals("validusername") || !password.Equals("validPassword"))
            {
                throw new SecurityTokenValidationException("Username or password is invalid.");
            }
        }
    }
}