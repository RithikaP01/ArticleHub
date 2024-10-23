using Article_Hub.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Web.Http;

namespace Article_Hub.Controllers
{
    [RoutePrefix("appuser")]
    public class AppUserController : ApiController
    {
        ArticleHubEntities db = new ArticleHubEntities();

        [HttpPost, Route("login")]

        public HttpResponseMessage Login([FromBody] appuser appuser)
        {
            try
            {
                appuser userObj = db.appusers
                    .Where(u => (u.email == appuser.email && u.password == appuser.password)).FirstOrDefault();
                if (userObj != null) 
                { 
                    if(userObj.status == "true") 
                    { 
                        return Request.CreateResponse(HttpStatusCode.OK, new { token = TokenManagerClass.GenerateToken(userObj.email, userObj.isDeletable)});
                    }
                    else
                    {
                        return Request.CreateResponse(HttpStatusCode.Unauthorized, new { message = "Wait for Admin Approval" });
                    }
                }
                else
                {
                    return Request.CreateResponse(HttpStatusCode.Unauthorized, new { message = "Incorrect Email or Password" });
                }
            }
            catch(Exception e)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, e);
            }
        }

        [HttpPost, Route("addNewAppuser")]
        [CustomAuthenticationFilter]
        public HttpResponseMessage AddNewAppuser([FromBody] appuser appUser)
        {
            try
            {
                appuser userObj = db.appusers
                    .Where(u => u.email == appUser.email).FirstOrDefault();
                if(userObj == null)
                {
                    appUser.status = "false";
                    appUser.isDeletable = "true";
                    db.appusers.Add(appUser);
                    db.SaveChanges();
                    return Request.CreateResponse(HttpStatusCode.OK, new { message = "Successfully Registered"});

                } else
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest, new { message = "Email Already Exist" });
                }

            }
            catch (Exception e)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, e);
            }
        }

        [HttpGet, Route("getAllAppuser")]
        [CustomAuthenticationFilter]
        public HttpResponseMessage GetAllAppuser()
        {
            try
            {
                var token = Request.Headers.GetValues("authorization").First();
                tokenClaim tokenClaim = TokenManagerClass.ValidateToken(token);
                if(tokenClaim.isDeletable == "false")
                {
                    var result = db.appusers
                        .Where(u => u.isDeletable == "true")
                        .Select(u => new { u.id, u.name, u.email, u.status, u.isDeletable })
                        .ToList();
                    return Request.CreateResponse(HttpStatusCode.OK, result);
                }
                else
                {
                    var result = db.appusers
                        .Where(x => (x.isDeletable == "true" && x.email != tokenClaim.Email))
                        .Select(u => new { u.id, u.name, u.status, u.isDeletable})
                        .ToList();
                    return Request.CreateResponse(HttpStatusCode.OK, result);
                }

            }
            catch(Exception e)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, e);
            }
        }

        [HttpPost, Route("updateUserStatus")]
        [CustomAuthenticationFilter]

        public HttpResponseMessage UpdateUserStatus([FromBody] appuser appuser)
        {
            try
            {
                appuser userObj = db.appusers.FirstOrDefault(u => u.id == appuser.id && u.isDeletable == "true");
                if(userObj == null)
                {
                    return Request.CreateResponse(HttpStatusCode.NotFound, new { message = "User Id not found" });
                }
                userObj.status = appuser.status;
                db.Entry(userObj).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                return Request.CreateResponse(HttpStatusCode.OK, new { message = "User Updated Successfully" });
            }
            catch(Exception e)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, e);
            }
        }

        [HttpPost, Route("updateUser")]
        [CustomAuthenticationFilter]

        public HttpResponseMessage UpdateUser([FromBody] appuser appuser)
        {
            try
            {
                appuser userObj = db.appusers.FirstOrDefault(u => u.id == appuser.id && u.isDeletable == "true");
                if (userObj == null)
                {
                    return Request.CreateResponse(HttpStatusCode.NotFound, new { message = "User id does not found" });
                }
                userObj.status = appuser.name;
                userObj.email = appuser.email;
                db.Entry(userObj).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                return Request.CreateResponse(HttpStatusCode.OK, new { message = "User Updated Successfully" });
            }
            catch (Exception e)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, e);
            }
        }
    }
}