using Article_Hub.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Article_Hub.Controllers
{

    [RoutePrefix("article")]
    public class ArticleController : ApiController
    {
        ArticleHubEntities db = new ArticleHubEntities();

        [HttpPost, Route("addNewArticle")]
        [CustomAuthenticationFilter]

        public HttpResponseMessage AddNewArticle([FromBody] article article)
        {
            try
            {
                article.publication_date = DateTime.Now;
                db.articles.Add(article);
                db.SaveChanges();
                return Request.CreateResponse(HttpStatusCode.OK, new { message = "Article Added Successfully" });
            }
            
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex);
            }
        }

        [HttpGet, Route("getAllArticle")]
        [CustomAuthenticationFilter]
        public async Task<HttpResponseMessage> GetAllArticle()
        {
            try
            {
                var result = await (from art in db.articles
                                    join cat in db.categories on art.categoryId equals cat.id
                                    select new
                                    {
                                        art.id,
                                        art.title,
                                        art.content,
                                        art.status,
                                        art.publication_date,
                                        categoryId = cat.id,
                                        categoryName = cat.name
                                    }).ToListAsync().ConfigureAwait(false);
                return Request.CreateResponse(HttpStatusCode.OK, result);
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex);
            }
        }

        [HttpGet, Route("getAllPublishedArticle")]
        public async Task<HttpResponseMessage> GetAllPublishedArticle()
        {
            try
            {
                var result = await (from art in db.articles
                                    join cat in db.categories on art.categoryId equals cat.id
                                    where art.status == "published"
                                    select new
                                    {
                                        art.id,
                                        art.title,
                                        art.content,
                                        art.status,
                                        art.publication_date,
                                        categoryId = cat.id,
                                        categoryName = cat.name
                                    }).ToListAsync().ConfigureAwait(false);
                return Request.CreateResponse(HttpStatusCode.OK, result);
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex);
            }
        }

        [HttpPost, Route("updateArticle")]
        [CustomAuthenticationFilter]

        public async Task<HttpResponseMessage> UpdateArticle([FromBody] article article)
        {
            try
            {
                var articleObj = await db.articles.FindAsync(article.id);
                if (articleObj == null)
                {
                    return Request.CreateResponse(HttpStatusCode.NotFound, new { message = "Article Id not found" });
                }
                articleObj.title = article.title;
                articleObj.content = article.content;
                articleObj.status = article.status;
                articleObj.categoryId = article.categoryId;
                articleObj.publication_date = DateTime.Now;
                article.status = article.status;
                db.Entry(articleObj).State = EntityState.Modified;
                await db.SaveChangesAsync().ConfigureAwait(false);
                return Request.CreateResponse(HttpStatusCode.OK, new { message = "Article Upadted Successfully" });
            }
            catch(Exception ex) 
            {
                return Request.CreateResponse(HttpStatusCode.OK, ex);
            }
        }

        [HttpGet, Route("deleteArticle/{id}")]
        [CustomAuthenticationFilter]
        public HttpResponseMessage DeleteArticle(int id)
        {
            try
            {
                article articleObj = db.articles.Find(id);
                if (articleObj == null)
                {
                    return Request.CreateResponse(HttpStatusCode.NotFound, new { message = "Article Id Not Found" });
                }
                db.articles.Remove(articleObj);
                db.SaveChanges();
                return Request.CreateResponse(HttpStatusCode.OK, new { message = "Article Deleted Successfully" });
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex);
            }
        
        }
    }
}
