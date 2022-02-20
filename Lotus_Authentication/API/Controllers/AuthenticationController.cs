using Lotus_Authentication.API.ApiModels;
using Microsoft.AspNetCore.Mvc;

namespace Lotus_Authentication.API.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class AuthenticationController : ControllerBase
	{
		/// <summary>
		/// Authenticate a user by username OR/AND email
		/// </summary>
		/// <isActive>true</isActive>
		/// <method>GET</method>
		/// <route>api/authentication/user</route>
		/// <header>
		///     <param name="apiKey" required="true">Your api key</param>
		/// </header>
		/// <body>
		///     <param name="email">The email address of the user</param>
		///     <param name="userName">The username of the user)</param>
		///     <param name="password">The SHA1 hashed password of the user</param>
		/// </body>
		/// <returns>A user object containing the users ID, email, Gender and Country ISO2</returns>
		/// <results>
		///     <result status="200">Authentication passed</result>
		///     <result status="400">username and email cannot be left empty</result>
		///     <result status="400">password cannot be left empty</result>
		///     <result status="400">password must be a valid SHA1 checksum</result>
		///     <result status="404">User could not be authenticated. Invalid username, email or password</result>
		///     <result status="403">Unauthorized</result>
		/// </results>
		[HttpGet, Route("user")]
		public ActionResult<dynamic> Authentication([FromBody] ApiUserModel body, [FromHeader]string apiKey)
		{
            try
            {
				DbHandler.GetApiKeyByApiKey(apiKey);
			}
            catch (BadApiKeyReferenceException)
            {
				return StatusCode(403);
			}

			if (string.IsNullOrWhiteSpace(body.UserName) && string.IsNullOrWhiteSpace(body.Email))
				return new BadRequestObjectResult($"username and email cannot be null");
			if (body.Email is not null && !EmailValidator.IsValidEmail(body.Email))
				return new BadRequestObjectResult($"{body.Email} is not a valid email address");
			if (string.IsNullOrWhiteSpace(body.Password))
				return new BadRequestObjectResult("password cannot be null or empty");
			if(!SHA1Hash.IsValidSHA1(body.Password))
				return new BadRequestObjectResult("password must be sent in as a SHA1 checksum");

			User? user;
            try
            {
				user = DbHandler.GetUser(body.UserName, body.Email, body.Password);
			}
			catch(UserNotFoundException)
            {
				return new NotFoundObjectResult("Invalid username, email or password");
			}

			if (user is null)
				return new NotFoundResult();

			var returnedResult = new
			{
				ID = user.Id,
				Email = user.Email,
				Gender = user.Gender.ToString(),
				Country = user.CountryISO2
			};
			return returnedResult;
		}
	}
}
