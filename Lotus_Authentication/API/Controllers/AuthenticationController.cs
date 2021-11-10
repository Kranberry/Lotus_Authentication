using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Lotus_Authentication.API.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class AuthenticationController : ControllerBase
	{
		// GET api/<AuthenticationController>/user
		[HttpGet, Route("user")]
		public ActionResult<User> Get([FromBody] Dictionary<string, string> body)
		{
#warning Test this after new user creation has been made
			/* {
			 *	  "username": "testsson",	// optional, but only if email is not supplied
			 *	  "email": "testsson@email.com",	// optional, but only if username is not supplied
			 *	  "password": "C6B77501AF2051430FDCE1659E8A9582CCBA40CA"	// Required SHA1 validated
			 * }
			 */
			body.TryGetValue("userName", out string? userName);
			body.TryGetValue("email", out string? email);
			body.TryGetValue("password", out string? password);
			if(email is not null && !EmailValidator.IsValidEmail(email))
				return new BadRequestObjectResult($"{email} is not a valid email address");
			if (string.IsNullOrWhiteSpace(password))
				return new BadRequestObjectResult("password cannot be null or empty");
			if(SHA1Hash.IsValidSHA1(password))
				return new BadRequestObjectResult("password must be sent in as a SHA1 checksum");

			User? user = DbHandler.GetUser(userName, email, password);

			if (user is null)
				return new BadRequestResult();

			return user;
		}
	}
}
