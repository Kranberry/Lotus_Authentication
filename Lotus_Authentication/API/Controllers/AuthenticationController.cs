using Microsoft.AspNetCore.Mvc;

namespace Lotus_Authentication.API.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class AuthenticationController : ControllerBase
	{
		// GET api/<AuthenticationController>/user
		[HttpGet, Route("user")]
		public ActionResult<dynamic> Get([FromBody] Dictionary<string, string> body, [FromHeader]string apiKey)
		{
            // TODO: Validate the apiKey
            try
            {
				DbHandler.GetApiKeyByApiKey(apiKey);
			}
            catch (BadApiKeyReferenceException)
            {
				return StatusCode(403);
			}

			body.TryGetValue("userName", out string? userName);
			body.TryGetValue("email", out string? email);
			body.TryGetValue("password", out string? password);
			if (userName is null && email is null)
				return new BadRequestObjectResult($"username and email cannot be null");
			if (email is not null && !EmailValidator.IsValidEmail(email))
				return new BadRequestObjectResult($"{email} is not a valid email address");
			if (string.IsNullOrWhiteSpace(password))
				return new BadRequestObjectResult("password cannot be null or empty");
			if(!SHA1Hash.IsValidSHA1(password))
				return new BadRequestObjectResult("password must be sent in as a SHA1 checksum");

			User? user;
            try
            {
				user = DbHandler.GetUser(userName, email, password);
			}
			catch(UserNotFoundException)
            {
				return new BadRequestObjectResult("Invalid username, email or password");
			}

			if (user is null)
				return new BadRequestResult();

			var returnedresult = new
			{
				ID = user.Id,
				Email = user.Email,
				Gender = user.Gender.ToString(),
				Country = user.CountryISO2
			};
			return returnedresult;
		}
	}
}
