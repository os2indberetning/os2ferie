using System.Linq;
using System.Web.Http;
using System.Web.OData;
using System.Web.OData.Query;
using Core.DomainModel;
using Core.DomainServices;

namespace OS2Indberetning.Controllers
{
    public class MailNotificationsController : BaseController<MailNotificationSchedule>
    {
        public MailNotificationsController(IGenericRepository<MailNotificationSchedule> repo, IGenericRepository<Person> personRepo) : base(repo, personRepo){}
        
        //GET: odata/MailNotificationSchedules
        [EnableQuery]
        public IQueryable<MailNotificationSchedule> Get(ODataQueryOptions<MailNotificationSchedule> queryOptions)
        {
            var res =  GetQueryable(queryOptions);
            return res;
        }

        //GET: odata/MailNotificationSchedules(5)
        public IQueryable<MailNotificationSchedule> Get([FromODataUri] int key, ODataQueryOptions<MailNotificationSchedule> queryOptions)
        {
            return GetQueryable(key, queryOptions);
        }

        //PUT: odata/MailNotificationSchedules(5)
        public new IHttpActionResult Put([FromODataUri] int key, Delta<MailNotificationSchedule> delta)
        {
            return base.Put(key, delta);
        }

        //POST: odata/MailNotificationSchedules
        [EnableQuery]
        public new IHttpActionResult Post(MailNotificationSchedule MailNotificationSchedule)
        {
            return CurrentUser.IsAdmin ? base.Post(MailNotificationSchedule) : Unauthorized();
        }

        //PATCH: odata/MailNotificationSchedules(5)
        [EnableQuery]
        [AcceptVerbs("PATCH", "MERGE")]
        public new IHttpActionResult Patch([FromODataUri] int key, Delta<MailNotificationSchedule> delta)
        {
            return CurrentUser.IsAdmin ? base.Patch(key, delta) : Unauthorized();
        }

        //DELETE: odata/MailNotificationSchedules(5)
        public new IHttpActionResult Delete([FromODataUri] int key)
        {
            return CurrentUser.IsAdmin ? base.Delete(key) : Unauthorized();
        }
    }
}