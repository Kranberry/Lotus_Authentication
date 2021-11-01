using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Lotus_Authentication.API.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class AuthenticationController : ControllerBase
	{
		// GET api/<AuthenticationController>/5
		[HttpGet("{email}")]
		public string Get(string email, [FromBody] string body)
		{
			/*	Body contents
			 * email or username
			 * SHA1 hashed password
			 * 
			 * 
			 * 
			 */
			return email;
		}

		// POST api/<AuthenticationController>
		[HttpPost]
		public void Post([FromBody] string value)
		{
		}

		// PUT api/<AuthenticationController>/5
		[HttpPut("{id}")]
		public void Put(int id, [FromBody] string value)
		{
		}

		// DELETE api/<AuthenticationController>/5
		[HttpDelete("{id}")]
		public void Delete(int id)
		{
		}
	}
}
