using Lotus_Authentication.API.ApiModels;
using Microsoft.AspNetCore.Mvc;

namespace Lotus_Authentication.API.Controllers
{
    // api/Users
    //[Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        /* {
         *   "email": "example@gmail.com", // required
         *   "username": "example_user", // required
         *   "password": "C6B77501AF2051430FDCE1659E8A9582CCBA40CA", // SHA1 encrypted
         *   "first_name": "test", // optional
         *   "last_name": "testsson", // optional
         *   "gender": "1", // required
         *   "country_iso2": "SE", // required
         * } */
        [HttpPost, Route("api/users/newUser")]
        public async Task<ActionResult<User>> AddNewUser([FromHeader] string api_key, [FromBody] ApiUserModel body) // HttpRequest body
        {
            string[] mandatoryKeys = new string[] { nameof(body.Email), nameof(body.UserName), nameof(body.Password), nameof(body.CountryISO2) };
            // New method to check for req body requirements
            if (body.ArePropertiesNull(mandatoryKeys, out string? key))
                return new BadRequestObjectResult($"Mandatory property not found: '{key}'");

            if (!EmailValidator.IsValidEmail(body.Email))
                return new BadRequestObjectResult($"The property 'email' does not contain a valid email address");

            if (!ApiKey.IsValidApiKey(api_key))
                return StatusCode(403);

            string? badRequestMessage = body.UserName switch
            {
                string str when ( string.IsNullOrWhiteSpace(str) )  => "The property 'username' cannot be null or whitespace",
                string str when ( str.Contains(' ') )               => "The property 'username' cannot contain spaces", 
                string str when ( str.Length < 5 )                  => "The property 'username' cannot be shorter than 5 characters long",
                _                                                   => null
            };
            if(badRequestMessage is not null)
                return new BadRequestObjectResult(badRequestMessage);

            if (!SHA1Hash.IsValidSHA1(body.Password))
                return new BadRequestObjectResult($"The property 'password' is not a valid SHA1 checksum");

            body.Gender ??= Gender.Other;

            string? f = 5 < 10 ? "asd" : null;
            User? user = new(0, body.FirstName, body.LastName, body.Email, body.UserName, UserType.Regular, (Gender)body.Gender, body.CountryISO2, null, null, DateTime.Now, null, false);
            user.SetPassword(body.Password);

            user = await DbHandler.InsertUser(user, api_key);

            return user is not null ? user : new BadRequestObjectResult("User with this email or username already exists");
        }

        [HttpPut, Route("api/users/user/updateUser")]
        public ActionResult<User> UpdateUser([FromBody] ApiUserModel user)
        {
            // TODO: Update existing user via API
            /* {
             *   "email": "example@gmail.com", // required
             *   "username": "example_user", // required
             *   "password": "C6B77501AF2051430FDCE1659E8A9582CCBA40CA", // required, SHA1 encrypted
             *   "new_password": "C6B77501AF2051430FDCE1659E8A9582CCBA40CA", // optional, SHA1 encrypted
             *   "first_name": "test", // optional
             *   "last_name": "testsson", // optional
             *   "gender": "1", // optional
             *   "country_iso2": "SE", // optional
             * }
             */
            throw new NotImplementedException();
        }

        /// <summary>
        /// Permanently delete a user from the database
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        [HttpDelete("api/users/user/permanent-delete/{email}")]
        public async Task<ActionResult<string>> DeleteUser([FromHeader] string api_key, string email)
        {
            if (!ApiKey.IsValidApiKey(api_key))
                return new BadRequestResult();

            User user;
            ApiKey apiKey;
            try
            {
                user = DbHandler.GetUser(email);
                apiKey = DbHandler.GetApiKeyByApiKey(api_key);
            }
            catch (UserNotFoundException)
            {
                return new NotFoundResult();
            }
            catch (BadApiKeyReferenceException)
            {
                return new NotFoundObjectResult("Api key could not be found.");
            }

            if(await DbHandler.UserHasConnectionToApiKey(user.Id, apiKey.ApiKeyID))
            {
                DbHandler.PermanentDeleteUser(user.Id);
                return new OkResult();
            }

            return StatusCode(403);
        }

        /// <summary>
        /// Delete every reference connecting the user with incoming api key
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        [HttpDelete("api/users/user/remove/{email}")]
        public ActionResult<bool> DeleteUserApiReference([FromHeader] string api_key, string email)
        {
            if (string.IsNullOrWhiteSpace(api_key))
                return new BadRequestResult();
            if (!ApiKey.IsValidApiKey(api_key))
                return new BadRequestObjectResult("Invalid Api key");
            if (string.IsNullOrWhiteSpace(email))
                return new BadRequestObjectResult("email cannot be null or empty");

            User user;
            try
            {
                user = DbHandler.GetUser(email);
            }
            catch (UserNotFoundException)
            {
                return new NotFoundResult();
            }

            bool success = DbHandler.RemoveUserApiConnection(user.Id, api_key);

            if (!success)
                return new NotFoundResult();

            return Ok($"User has successfully been removed from your api key");
        }
    }
}
