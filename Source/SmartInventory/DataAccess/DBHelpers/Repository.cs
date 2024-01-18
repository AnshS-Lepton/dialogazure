using DataAccess.Contracts;
using DataAccess.DBContext;
namespace DataAccess.DBHelpers
{
    //Repository
    public abstract class Repository<T> where T : class
    {
        protected string connetionString { get; set; }
        public delegate void NotificationEventHandlerBusiness(dynamic data);
        public event NotificationEventHandlerBusiness DataUploaderNotificationHandler;

        private  IGenericRepository<T> _repo;
        protected  IGenericRepository<T> repo
        {
            get
            {
                _repo = null;
                _repo = new GenericRepository<T>(new MainContext(connetionString));
                return _repo;
            }
        }        
        private IGenericRepository<T> _repo_routing;
        protected IGenericRepository<T> repo_routing
        {
            get
            {
                _repo_routing = null;
                _repo_routing = new GenericRepository<T>(new RoutingContext());
                return _repo_routing;
            }
        }
        public void NotifyUpdatedStatus(dynamic dynamic)
        {
            if (DataUploaderNotificationHandler != null)
                DataUploaderNotificationHandler.Invoke(dynamic);
        }                
       
    }    
}
