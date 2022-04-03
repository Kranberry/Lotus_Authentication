﻿using Lotus_Authentication.API.ApiModels;
using Microsoft.AspNetCore.Mvc;

namespace Lotus_Authentication.API.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class AuthenticationController : ControllerBase
	{
		/// <summary>
		/// Authenticate a user by username AND/OR email
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
		/// <returns>A user object containing the users ID, email, Gender and Country ISO2, and Ban status of the user to your api key</returns>
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
			UserBanStatus? banStatus;
            try
            {
				user = DbHandler.GetUser(body.UserName, body.Email, body.Password);
				banStatus = DbHandler.GetUserBanStatus(user, apiKey);
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
				Country = user.CountryISO2,
				BanStatus = banStatus?.BanStatus.ToString(),
				BanLiftDate = banStatus?.BanLiftDate,
				BanReason = banStatus?.Reason
			};
			return returnedResult;
		}


		/// <summary>
		/// Ban or unban a user from your api key or widely for all your api keys
		/// </summary>
		/// <isActive>true</isActive>
		/// <method>POST</method>
		/// <route>api/authentication/user/ban</route>
		/// <header>
		///     <param name="apiKey" required="true">Your api key</param>
		/// </header>
		/// <body>
		///     <param name="userId" required="true">The id of the user. NOTE: Email address can be used instead. If both email and useId are provided, userId will be used</param>
		///     <param name="email" required="true">The email address of the user. NOTE: userId can be used instead. If both email and useId are provided, userId will be used</param>
		///     <param name="banStatus" required="true">true/false, true if ban, false if unban</param>
		///     <param name="wideBan">true/false, true to ban across all your api keys, false to unband across all your api keys</param>
		///     <param name="banLiftDate">Date and time for the ban to be lifted. By default 24 hours from ban date.</param>
		/// </body>
		/// <returns>200 OK or 400 Bad Request</returns>
		/// <results>
		///     <result status="200">Returned if your request was completed</result>
		///     <result status="400">Returned if your request is missing userId and Email</result>
		///     <result status="400">Returned if your request contained an invalid email or userId</result>
		/// </results>
		[HttpPost, Route("user/ban")]
		public ActionResult<dynamic> BanUser([FromHeader]string apiKey, [FromBody] BanUserControllerModel banStatusBody)
        {
			if (banStatusBody.UserId is null && banStatusBody.Email is null)
				return new BadRequestObjectResult("userId and Email cannot be null");


			bool succeded = DbHandler.BanUser(apiKey, banStatusBody.UserId, banStatusBody.Email, banStatusBody.BanStatus, banStatusBody.WideBan, banStatusBody.BanLiftDate, banStatusBody.Reason);
			
			return succeded ? new OkResult() : new BadRequestResult();
        }

		public class BanUserControllerModel
        {
			public int? UserId { get; set; }
			public string? Email { get; set; }
			public bool BanStatus { get; set; }
			public bool WideBan { get; set; }
			public string? Reason { get; set; }
			public DateTime? BanLiftDate { get; set; }
        }
	}
}
