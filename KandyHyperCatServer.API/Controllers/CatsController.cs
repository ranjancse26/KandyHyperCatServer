using System.Collections.Generic;
using System.Web.Http;
using System.Configuration;
using System.Linq;
using NHyperCat;
using Raven.Client.Document;
using Raven.Client.Linq;

namespace KandyHyperCatServer.API.Controllers
{
    [System.Web.Http.RoutePrefix("api/cats")]
    public class CatsController : ApiController
    {
        private DocumentStore GetDocumentStore()
        {
            var documentStore= new DocumentStore
            {
                DefaultDatabase = "Hypercat",
                Url = ConfigurationManager.AppSettings["RavenDBConnectionUrl"]
            };
            documentStore.Initialize();
            return documentStore;
        }

        private IRavenQueryable<HyperCatCatalouge> GetQuery(string catalougeName)
        {
            var documentStore = GetDocumentStore();
            using (var session = documentStore.OpenSession())
            {
                var hyperCatCatalougeQuery =
                    session.Query<HyperCatCatalouge>()
                        .Where(x => x.Name == catalougeName);
                return hyperCatCatalougeQuery;
            }
        }

        [System.Web.Http.HttpGet]
        public Catalogue Get(string id)
        {
            var hyperCatCatalouge = GetQuery(id).FirstOrDefault();
            if (hyperCatCatalouge != null)
                return hyperCatCatalouge.Catalogue;

            return new Catalogue
            {
                Items = new List<Item>(),
                CatalogueMetaData = new List<CatalogueMetaData>()
            };
        }

        [System.Web.Http.HttpPost]
        public IHttpActionResult Post([FromUri] string catalougeName,
                                      [FromBody] Catalogue catalouge)
        {
            var documentStore = GetDocumentStore();
            using (var session = documentStore.OpenSession())
            {
                session.Store(new HyperCatCatalouge
                {
                    Catalogue = catalouge,
                    Name = catalougeName
                });
                session.SaveChanges();
            }
            return Ok();
        }

        [System.Web.Http.HttpDelete]
        public IHttpActionResult Delete([FromUri] string id)
        {
            var documentStore = GetDocumentStore();
            using (var session = documentStore.OpenSession())
            {
                var hyperCatCatalouge = session.Query<HyperCatCatalouge>()
                           .FirstOrDefault(x => x.Name == id);
                if (hyperCatCatalouge != null)
                {
                    session.Delete(hyperCatCatalouge);
                    session.SaveChanges();
                }
            }
            return Ok();
        }
    }
}