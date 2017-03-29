using System;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using WebMatrix.WebData;
using Web.Filters;
using Web.Models;

namespace Web.Controllers
{
  [Authorize]
  [InitialiseDatabase]
  public class AccountController : Controller
  {
    private SupercarModelContext db = new SupercarModelContext();

    //
    // GET: /Account/Login
    [AllowAnonymous]
    public ActionResult Login(string returnUrl)
    {
      ViewBag.ReturnUrl = returnUrl;
      return View();
    }

    //
    // POST: /Account/Login
    [HttpPost]
    [AllowAnonymous]
    public ActionResult Login(LoginModel model, string returnUrl)
    {
      if (ModelState.IsValid && WebSecurity.Login(model.Email, model.Password, true))
      {

        if (model.RememberMe)
        {
          var bytesToEncode = Encoding.UTF8.GetBytes(model.Password);
          var encodedPassword = Convert.ToBase64String(bytesToEncode);

          Response.Cookies.Add(new HttpCookie("Password", encodedPassword) {Expires = DateTime.Now.AddYears(1)});
          Response.Cookies.Add(new HttpCookie("Email", model.Email) {Expires = DateTime.Now.AddYears(1)});
        }
        else
        {
          Response.Cookies.Remove("Password");
          Response.Cookies.Remove("Email");
        }

        return RedirectToLocal(returnUrl);
      }

      ModelState.AddModelError("", "The email or password provided is incorrect.");
      return View(model);
    }

    //
    // POST: /Account/LogOff
    [HttpPost]
    public ActionResult LogOff()
    {
      WebSecurity.Logout();
      return RedirectToAction("Index", "Home");
    }

    //
    // GET: /Account/Register
    [AllowAnonymous]
    public ActionResult Register()
    {
      return View();
    }

    //
    // POST: /Account/Register
    [HttpPost]
    [AllowAnonymous]
    [ValidateInput(false)]
    public ActionResult Register(RegisterModel model)
    {
      if (ModelState.IsValid)
      {
        // Attempt to register the user
        try
        {
          WebSecurity.CreateUserAndAccount(model.Email, model.Password);
          WebSecurity.Login(model.Email, model.Password);

          var userProfile = db.UserProfiles.Single(u => u.Email == model.Email);
          userProfile.FirstName = model.FirstName;
          userProfile.LastName = model.LastName;
          userProfile.Password = model.Password;
          db.SaveChanges();

          var message = new MailMessage
            {
              Subject = "Your new account has been created",
              Body = string.Format("Welcome {0}!<br /><br />" +
                                   "Your new Supercar Showdown account has been created. Just so you don't forget it, your password is <strong>{1}</strong>",
                                   model.FirstName, model.Password),
              IsBodyHtml = true
            };
          message.To.Add(new MailAddress(model.Email));
            var smtpClient = new SmtpClient();
          smtpClient.Send(message);

          return RedirectToLocal("");
        }
        catch (MembershipCreateUserException e)
        {
          ModelState.AddModelError("", ErrorCodeToString(e.StatusCode));
        }
      }

      return View(model);
    }

    //
    // GET: /Account/UserProfile
    public ActionResult UserProfile(ManageMessageId? message)
    {
      ViewBag.StatusMessage = message == ManageMessageId.ChangeProfileSuccess ? "Your profile has been updated."
          : "";
      var profile = db.UserProfiles.SingleOrDefault(u => u.Email == User.Identity.Name);
      return View(profile);
    }

    //
    // POST: /Account/UserProfile
    [HttpPost]
    public ActionResult UserProfile(UserProfile model)
    {
      if (ModelState.IsValid)
      {
        var profile = db.UserProfiles.SingleOrDefault(u => u.Email == User.Identity.Name);
        if (TryUpdateModel(profile))
        {
          db.SaveChanges();
          return RedirectToAction("UserProfile", new { Message = ManageMessageId.ChangeProfileSuccess });
        }
      }

      return View(model);
    }

    //
    // GET: /Account/ChangePassword
    public ActionResult ChangePassword(ManageMessageId? message)
    {
      ViewBag.StatusMessage =
          message == ManageMessageId.ChangePasswordSuccess ? "Your password has been changed."
          : message == ManageMessageId.SetPasswordSuccess ? "Your password has been set."
          : "";
      ViewBag.ReturnUrl = Url.Action("ChangePassword");

      var userProfile = db.UserProfiles.SingleOrDefault(u => u.Email == User.Identity.Name);

      return View(new LocalPasswordModel { NewPassword = userProfile.Password, ConfirmPassword = userProfile.Password });
    }

    //
    // POST: /Account/ChangePassword
    [HttpPost]
    public ActionResult ChangePassword(LocalPasswordModel model)
    {
      ViewBag.ReturnUrl = Url.Action("ChangePassword");

      if (ModelState.IsValid)
      {
        // ChangePassword will throw an exception rather than return false in certain failure scenarios.
        bool changePasswordSucceeded;
        try
        {
          var token = WebSecurity.GeneratePasswordResetToken(User.Identity.Name);
          changePasswordSucceeded = WebSecurity.ResetPassword(token, model.NewPassword);

          var userProfile = db.UserProfiles.SingleOrDefault(u => u.Email == User.Identity.Name);
          userProfile.Password = model.NewPassword;
          db.SaveChanges();
        }
        catch (Exception)
        {
          changePasswordSucceeded = false;
        }

        if (changePasswordSucceeded)
        {
          return RedirectToAction("ChangePassword", new { Message = ManageMessageId.ChangePasswordSuccess });
        }
        else
        {
          ModelState.AddModelError("", "The current password is incorrect or the new password is invalid.");
        }
      }

      return View(model);
    }

    //
    // GET: /Account/ResetPassword
    [AllowAnonymous]
    public ActionResult ResetPassword()
    {
      return View();
    }

    //
    // GET: /Account/ResetPassword
    [HttpPost]
    [AllowAnonymous]
    public ActionResult ResetPassword(PasswordResetModel model)
    {
      var userProfile = db.UserProfiles.SingleOrDefault(u => u.Email == model.Email);

      if (userProfile != null)
      {
        const string chars = "abcdefghijklmnopqrstuvwxyz";
        var random = new Random();
        var newPassword = new string(Enumerable.Repeat(chars, 8).Select(s => s[random.Next(s.Length)]).ToArray());

        var token = WebSecurity.GeneratePasswordResetToken(model.Email);
        WebSecurity.ResetPassword(token, newPassword);

        userProfile.Password = newPassword;
        db.SaveChanges();

        var message = new MailMessage
        {
          Subject = "Your password has been reset",
          Body = string.Format("Dear {0}!<br /><br />" +
                               "Your password has been reset and it is now <strong>{1}</strong>",
                               userProfile.FirstName, newPassword),
          IsBodyHtml = true
        };
        message.To.Add(new MailAddress(userProfile.Email));
        var smtpClient = new SmtpClient();
        smtpClient.Send(message);

        return RedirectToAction("ResetPasswordComplete");
      }
      else
      {
        ModelState.AddModelError("", "The specified user does not exist.");
      }

      return View();
    }

    //
    // GET: /Account/ResetPasswordComplete
    [AllowAnonymous]
    public ActionResult ResetPasswordComplete()
    {
      return View();
    }

    #region Helpers

    private ActionResult RedirectToLocal(string returnUrl)
    {
      if (Url.IsLocalUrl(returnUrl))
      {
        return Redirect(string.Format("https://{0}{1}", Request.Url.Host, returnUrl));
      }
      else
      {
        return Redirect(Url.Action("Index", "Home", null, "https"));
      }
    }

    public enum ManageMessageId
    {
      ChangePasswordSuccess,
      SetPasswordSuccess,
      RemoveLoginSuccess,
      ChangeProfileSuccess
    }

    private static string ErrorCodeToString(MembershipCreateStatus createStatus)
    {
      switch (createStatus)
      {
        case MembershipCreateStatus.DuplicateUserName:
          return "Email already exists. Please enter a different address.";

        case MembershipCreateStatus.DuplicateEmail:
          return "That email address already exists. Please enter a different email address.";

        case MembershipCreateStatus.InvalidPassword:
          return "The password provided is invalid. Please enter a valid password value.";

        case MembershipCreateStatus.InvalidEmail:
          return "The email address provided is invalid. Please check the value and try again.";

        case MembershipCreateStatus.InvalidAnswer:
          return "The password retrieval answer provided is invalid. Please check the value and try again.";

        case MembershipCreateStatus.InvalidQuestion:
          return "The password retrieval question provided is invalid. Please check the value and try again.";

        case MembershipCreateStatus.InvalidUserName:
          return "The email provided is invalid. Please check the value and try again.";

        case MembershipCreateStatus.ProviderError:
          return "The authentication provider returned an error. Please verify your entry and try again. If the problem persists, please contact your system administrator.";

        case MembershipCreateStatus.UserRejected:
          return "The user creation request has been canceled. Please verify your entry and try again. If the problem persists, please contact your system administrator.";

        default:
          return "An unknown error occurred. Please verify your entry and try again. If the problem persists, please contact your system administrator.";
      }
    }

    #endregion
  }
}
