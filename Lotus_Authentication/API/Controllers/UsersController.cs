using Microsoft.AspNetCore.Mvc;

namespace Lotus_Authentication.API.Controllers
{
    // api/Users
    //[Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {

        [HttpGet, Route("api/users/getUser/test")]
        public ActionResult<User> TestMethod([FromBody] Dictionary<string, string> body)
        {
            User user = DbHandler.GetUser(body["email"]);
            return new OkObjectResult(user);
        }

        [HttpPost, Route("api/users/newUser")]
        public ActionResult<User> AddNewUser([FromBody] User user)
        {
            // TODO: Add new user via API
            /* {
             *   "email": "example@gmail.com", // required
             *   "username": "example_user", // required
             *   "password": "C6B77501AF2051430FDCE1659E8A9582CCBA40CA", // SHA1 encrypted
             *   "first_name": "test", // optional
             *   "last_name": "testsson", // optional
             *   "gender": "1", // required
             *   "country": "sweden", // required
             * }
             */
            throw new NotImplementedException();
        }

        [HttpPut, Route("api/users/user/updateUser")]
        public ActionResult<User> UpdateUser([FromBody] User user)
        {
            // TODO: Update existing user via API
            /* {
             *   "email": "example@gmail.com", // required
             *   "username": "example_user", // required
             *   "old_password": "C6B77501AF2051430FDCE1659E8A9582CCBA40CA", // required, SHA1 encrypted
             *   "new_password": "C6B77501AF2051430FDCE1659E8A9582CCBA40CA", // required, SHA1 encrypted
             *   "first_name": "test", // optional
             *   "last_name": "testsson", // optional
             *   "gender": "1", // required
             *   "country": "sweden", // required
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
        public async Task<ActionResult<string>> DeleteUser(string email)
        {
            // TODO: permanentyly delete user via API
            // email is required
            // Fetch api key from header
            throw new NotImplementedException();
        }

        /// <summary>
        /// Delete every reference connecting the user with incoming api key
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        [HttpDelete("api/users/user/remove/{email}")]
        public ActionResult<User> DeleteUserApiReference(string email)
        {
            // TODO: Delete api reference from existing user via API
            // fetch Api key from header
            // email is required
            throw new NotImplementedException();
        }
    }
}
