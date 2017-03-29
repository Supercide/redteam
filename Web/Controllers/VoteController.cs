using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Configuration;
using System.Web.Http;
using Web.Models;

namespace Web.Controllers
{
    public class VoteController : ApiController
    {
        private SupercarModelContext db = new SupercarModelContext();

        // POST api/Vote
        public HttpResponseMessage Post([FromBody] Vote vote)
        {
            if (!User.Identity.IsAuthenticated)
            {
                return Request.CreateResponse(HttpStatusCode.Forbidden);
            }

            var user = db.UserProfiles.First(u => u.Email == User.Identity.Name);
            vote.UserId = user.UserId;

            db.Votes.Add(vote);

            db.SaveChanges();

            return Request.CreateResponse(HttpStatusCode.Created);
        }
    }
}