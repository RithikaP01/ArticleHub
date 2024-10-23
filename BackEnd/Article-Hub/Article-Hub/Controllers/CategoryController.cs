using Article_Hub.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Article_Hub.Controllers
{
    [RoutePrefix("category")]
    public class CategoryController : ApiController
    {
        ArticleHubEntities db = new ArticleHubEntities();

        [HttpPost,Route("addNewCategory")]
        [CustomAuthenticationFilter]
        public HttpResponseMessage AddNewCategory([FromBody] category category)
        {
            try
            {
                db.categories.Add(category);
                db.SaveChanges();
                return Request.CreateResponse(HttpStatusCode.OK, new { message = "Category Added Successfully" });
            }
            catch(Exception ex) 
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex);
            }
        }

        [HttpGet, Route("getAllCategory")]
        [CustomAuthenticationFilter]
        public HttpResponseMessage GetAllCategory()
        {
            try
            {
                var categories = db.categories.OrderBy(category => category.name).ToList();
                return Request.CreateResponse(HttpStatusCode.OK, categories);
            }
            catch(Exception e)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, e);
            }
        }

        [HttpPost, Route("updateCategory")]
        [CustomAuthenticationFilter]
        public HttpResponseMessage UpdateCategory([FromBody] category category)
        {
            try
            {
                category categoryObj = db.categories.Find(category.id);
                if (categoryObj == null)
                {
                    return Request.CreateResponse(HttpStatusCode.NotFound, new { message = "Category Id  Not Found" });
                }
                categoryObj.name = category.name;
                db.Entry(categoryObj).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                return Request.CreateResponse(HttpStatusCode.OK, new { message = "Category Updated Successfully" });
            }
            catch(Exception e)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, e);
            }
        }

        [HttpGet, Route("deleteCategory/{id}")]
        [CustomAuthenticationFilter]
        public HttpResponseMessage DeleteCategory(int id)
        {
            try
            {
                category categoryObj = db.categories.Find(id);
                if (categoryObj == null)
                {
                    return Request.CreateResponse(HttpStatusCode.NotFound, new { message = "Category Id Not Found" });
                }
                db.categories.Remove(categoryObj);
                db.SaveChanges();
                return Request.CreateResponse(HttpStatusCode.OK, new { message = "Category Deleted Successfully" });
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex);
            }

        }
    }
}
