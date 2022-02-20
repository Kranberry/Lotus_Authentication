using Lotus_Authentication.API.ApiModels;
using Microsoft.AspNetCore.Mvc;

namespace Lotus_Authentication.API.Controllers
{
    // api/Users
    //[Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        /// <summary>
        /// Add a new user to our database
        /// </summary>
        /// <isActive>true</isActive>
		/// <method>POST</method>
        /// <route>api/Users/newUser</route>
        /// <header>
        ///     <param name="apiKey" required="true">Your api key</param>
        /// </header>
        /// <body>
        ///     <param name="username" required="true">testsson</param>
        ///     <param name="email" required="true">test@testsson.se</param>
        ///     <param name="password" required="true">a94a8fe5ccb19ba61c4c0873d391e987982fbbd3</param>
        ///     <param name="country_iso2" required="true">SE</param>
        ///     <param name="first_name">Test</param>
        ///     <param name="last_name">Testsson</param>
        ///     <param name="gender">1</param>
        /// </body>
		/// <returns>A user object containing the users ID, email, Gender and Country ISO2</returns>
        /// <results>
        ///     <result status="200">Everything passed</result>
        ///     <result status="400">Any of the mandatory properties are null or empty</result>
        ///     <result status="400">The email is invalid</result>
        ///     <result status="400">The password is not a valid SHA1 checksum</result>
        ///     <result status="400">The username is invalid</result>
        ///     <result status="400">The username or email address already exists</result>
        ///     <result status="403">Unautorized</result>
        /// </results>
        [HttpPost, Route("api/users/newUser")]
        public async Task<ActionResult<dynamic>> AddNewUser([FromHeader] string apiKey, [FromBody] ApiUserModel body) // HttpRequest body
        {
            string[] mandatoryKeys = new string[] { nameof(body.Email), nameof(body.UserName), nameof(body.Password), nameof(body.CountryISO2) };
            if (body.ArePropertiesNull(mandatoryKeys, out string? key))
                return new BadRequestObjectResult($"Mandatory property not found: '{key}'");

            if (!EmailValidator.IsValidEmail(body.Email!))
                return new BadRequestObjectResult($"The property 'email' does not contain a valid email address");

            if (!ApiKey.IsValidApiKey(apiKey))
                return StatusCode(403);

            string badRequestMessage = body.UserName switch
            {
                string str when ( string.IsNullOrWhiteSpace(str) )  => "The property 'username' cannot be null or whitespace",
                string str when ( str.Contains(' ') )               => "The property 'username' cannot contain spaces", 
                string str when ( str.Length < 5 )                  => "The property 'username' cannot be shorter than 5 characters long",
                _                                                   => string.Empty
            };
            if(badRequestMessage != string.Empty)
                return new BadRequestObjectResult(badRequestMessage);

            if (!SHA1Hash.IsValidSHA1(body.Password!))
                return new BadRequestObjectResult($"The property 'password' is not a valid SHA1 checksum");

            body.Gender ??= Gender.Other;

            User user = new(0, body.FirstName, body.LastName, body.Email, body.UserName, UserType.Regular, (Gender)body.Gender, body.CountryISO2, null, null, DateTime.Now, null, false);
            user.SetPassword(body.Password);

            try
            {
                user = await DbHandler.InsertUser(user, apiKey);
            }
            catch(UserAlreadyExistsException)
            {
                return new BadRequestObjectResult("User with this email or username already exists");
            }

            var returnedResult = new
            {
                ID = user.Id,
                Email = user.Email,
                Gender = user.Gender.ToString(),
                Country = user.CountryISO2
            };

            return returnedResult;
        }

        /// <summary>
        /// Update an already existing user to the database
        /// </summary>
        /// <isActive>false</isActive>
		/// <method>PUT</method>
        /// <route>api/users/user/updateUser</route>
        /// <header>
        ///     <param name="apiKey" required="true">Your api key</param>
        /// </header>
        /// <body>
        /// 
        /// </body>
        /// <returns>Returns a user object newly created with the data sent in</returns>
        /// <results>
        /// 
        /// </results>
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
        /// Permanently delete a user from our database that your api key have been given rights to delete this user
        /// </summary>
        /// <isActive>true</isActive>
		/// <method>DELETE</method>
        /// <route>api/Users/permanent-delete/{userId}</route>
        /// <header>
        ///     <param name="apiKey" required="true">Your api key</param>
        /// </header>
        /// <query>
        ///     <param name="userId" required="true">The users ID</param>
        /// </query>
        /// <returns>OK result if user was deleted successfully</returns>
        /// <results>
        ///     <result status="200">Everything passed</result>
        ///     <result status="400">User with this id could not be found</result>
        ///     <result status="403">Unauthorized</result>
        /// </results>
        [HttpDelete("api/users/user/permanent-delete/{userId}")]
        public async Task<ActionResult> DeleteUser([FromHeader] string apiKey, int userId)
        {
            if (!ApiKey.IsValidApiKey(apiKey))
                return StatusCode(403);

            User user;
            ApiKey key;
            try
            {
                user = DbHandler.GetUser(userId);
                key = DbHandler.GetApiKeyByApiKey(apiKey);
            }
            catch (UserNotFoundException)
            {
                return new NotFoundResult();
            }
            catch (BadApiKeyReferenceException)
            {
                return StatusCode(403);
            }

            if(await DbHandler.UserHasConnectionToApiKey(user.Id, key.ApiKeyID))
            {
                DbHandler.PermanentDeleteUser(user.Id);
                return new OkResult();
            }

            return StatusCode(403);
        }

        /// <summary>
        /// Delete your connection and remove any rights towards a user originaly created by your api key or been given rights to
        /// </summary>
        /// <isActive>true</isActive>
		/// <method>DELETE</method>
        /// <route>api/Users/user/remove/{email}</route>
        /// <header>
        ///     <param name="apiKey" required="true">Your api key</param>
        /// </header>
        /// <query>
        ///     <param name="userId" required="true">The users ID</param>
        /// </query>
        /// <returns>OK result if the connection was successfully removed</returns>
        /// <results>
        ///     <result status="200">connection was successfully removed</result>
        ///     <result status="400">User with this id could not be found</result>
        ///     <result status="403">Unauthorized</result>
        /// </results>
        [HttpDelete("api/users/user/remove/{userId}")]
        public ActionResult DeleteUserApiReference([FromHeader] string apiKey, int userId)
        {
            if (string.IsNullOrWhiteSpace(apiKey) || !ApiKey.IsValidApiKey(apiKey))
                return StatusCode(403);
            if (userId > 0)
                return new BadRequestObjectResult("Ivalid userId");

            User user;
            try
            {
                user = DbHandler.GetUser(userId);
            }
            catch (UserNotFoundException)
            {
                return new NotFoundResult();
            }

            bool success = DbHandler.RemoveUserApiConnection(user.Id, apiKey);

            if (!success)
                return new NotFoundResult();

            return new OkResult();
        }
    }
}
