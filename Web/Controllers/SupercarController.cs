using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Web.Mvc;
using Web.Filters;
using Web.Models;
using Web.ViewModels;

namespace Web.Controllers
{
    [InitialiseDatabase]
    public class SupercarController : Controller
    {
        private SupercarModelContext db = new SupercarModelContext();

        public ActionResult Index(int id = 0)
        {
            var supercar = db.Supercars
                             .Include("Make")
                             .Include("Votes")
                             .Include("Votes.User")
                             .SingleOrDefault(s => s.SupercarId == id);

            if (supercar == null)
            {
                return HttpNotFound();
            }

            ViewBag.TotalVotes = (decimal)db.Votes.Count();

            if (User.Identity.IsAuthenticated)
            {
                var profile = db.UserProfiles.SingleOrDefault(u => u.Email == User.Identity.Name);
                if (profile != null)
                {
                    ViewBag.FirstName = profile.FirstName;
                    ViewBag.LastName = profile.LastName;
                }
            }

            return View(supercar);
        }

        public ActionResult Leaderboard(string orderBy = "votes", bool asc = true)
        {
            try
            {
                var supercars = db.Supercars.SqlQuery("SELECT * FROM Supercar").ToList();

                supercars = asc
                    ? supercars.OrderBy(orderBy == "votes" ? "SupercarId" : orderBy).ToList()
                    : supercars.OrderByDescending(orderBy == "votes" ? "SupercarId" : orderBy).ToList();


                if (orderBy == "votes")
                {
                    supercars = supercars.OrderByDescending(s => s.Votes.Count()).ToList();
                }

                var leaderboard = supercars.Select(s => new Leaderboard
                {
                    SupercarId = s.SupercarId,
                    Make = s.Make.Name,
                    Model = s.Model,
                    PowerKw = s.PowerKw,
                    TorqueNm = s.TorqueNm,
                    ZeroToOneHundredKmInSecs = s.ZeroToOneHundredKmInSecs,
                    TopSpeedKm = s.TopSpeedKm,
                    Votes = s.Votes.Count()
                });

                return View(leaderboard);
            }
            catch (SqlException)
            {
                return View("LeaderBoardError");
            }
        }
    }

    public static class LinqExtensions
    {
        private static PropertyInfo GetPropertyInfo(Type objType, string name)
        {
            var properties = objType.GetProperties();
            var matchedProperty = properties.FirstOrDefault(p => p.Name == name);
            if (matchedProperty == null)
                throw new ArgumentException("name");

            return matchedProperty;
        }
        private static LambdaExpression GetOrderExpression(Type objType, PropertyInfo pi)
        {
            var paramExpr = Expression.Parameter(objType);
            var propAccess = Expression.PropertyOrField(paramExpr, pi.Name);
            var expr = Expression.Lambda(propAccess, paramExpr);
            return expr;
        }

        public static IEnumerable<T> OrderBy<T>(this IEnumerable<T> query, string name)
        {
            var propInfo = GetPropertyInfo(typeof(T), name);
            var expr = GetOrderExpression(typeof(T), propInfo);

            var method = typeof(Enumerable).GetMethods().FirstOrDefault(m => m.Name == "OrderBy" && m.GetParameters().Length == 2);
            var genericMethod = method.MakeGenericMethod(typeof(T), propInfo.PropertyType);
            return (IEnumerable<T>)genericMethod.Invoke(null, new object[] { query, expr.Compile() });
        }

        public static IEnumerable<T> OrderByDescending<T>(this IEnumerable<T> query, string name)
        {
            var propInfo = GetPropertyInfo(typeof(T), name);
            var expr = GetOrderExpression(typeof(T), propInfo);

            var method = typeof(Enumerable).GetMethods().FirstOrDefault(m => m.Name == "OrderByDescending" && m.GetParameters().Length == 2);
            var genericMethod = method.MakeGenericMethod(typeof(T), propInfo.PropertyType);
            return (IEnumerable<T>)genericMethod.Invoke(null, new object[] { query, expr.Compile() });
        }

        public static IQueryable<T> OrderBy<T>(this IQueryable<T> query, string name)
        {
            var propInfo = GetPropertyInfo(typeof(T), name);
            var expr = GetOrderExpression(typeof(T), propInfo);

            var method = typeof(Queryable).GetMethods().FirstOrDefault(m => m.Name == "OrderBy" && m.GetParameters().Length == 2);
            var genericMethod = method.MakeGenericMethod(typeof(T), propInfo.PropertyType);
            return (IQueryable<T>)genericMethod.Invoke(null, new object[] { query, expr });
        }
    }
}
