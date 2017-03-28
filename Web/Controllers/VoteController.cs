using System.Data.SqlClient;
using System.Net;
using System.Net.Http;
using System.Web.Configuration;
using System.Web.Http;
using Web.Models;

namespace Web.Controllers
{
  public class VoteController : ApiController
  {
    // POST api/Vote
    public HttpResponseMessage Post([FromBody]Vote vote)
    {
      if (!User.Identity.IsAuthenticated)
      {
        return Request.CreateResponse(HttpStatusCode.Forbidden);
      }

      var connString = WebConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
      var sqlString = "INSERT INTO Vote(UserId, SupercarId, Comments) VALUES (" + vote.UserId + ", " + vote.SupercarId + ", '" + vote.Comments + "')";

      using (var conn = new SqlConnection(connString))
      {
        var command = new SqlCommand(sqlString, conn);
        command.Connection.Open();
        command.ExecuteNonQuery();
      }

      return Request.CreateResponse(HttpStatusCode.Created);
    }
  }
}