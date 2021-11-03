using Microsoft.AspNetCore.Mvc;
using Lotus_Authentication.API.ApiModels;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Lotus_Authentication.API.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class AuthenticationController : ControllerBase
	{
		// GET api/<AuthenticationController>/user
		[HttpGet, Route("user")]
		public ActionResult<User> Get([FromBody] User body)
		{
			/* {
			 *	  "username": "testsson",	// optional, but only if email is not supplied
			 *	  "email": "testsson@email.com",	// optional, but only if username is not supplied
			 *	  "password": "C6B77501AF2051430FDCE1659E8A9582CCBA40CA"	// Required SHA1 validated
			 * }
			 */
			if(string.IsNullOrWhiteSpace(body.UserName) && string.IsNullOrWhiteSpace(body.Email))
				return BadRequest("The 'email' and 'username' fields can not be empty");
			if (!SHA1Hash.IsValidSHA1(body.Password))
                return BadRequest("The 'password' field is not a valid SHA1 checksum");

			if (!EmailValidator.IsValidEmail(body.Email))
				return BadRequest("Input email is not a valid email address");

            return body;
		}
	}
}
