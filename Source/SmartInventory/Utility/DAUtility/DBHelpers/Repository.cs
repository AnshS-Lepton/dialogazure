using Utility.DAUtility.Contracts;
using Utility.DAUtility.DBContext;
namespace Utility.DAUtility.DBHelpers
{
    //Repository
    public abstract class Repository<T> where T : class
    {

        public delegate void NotificationEventHandlerBusiness(dynamic data);
        public event NotificationEventHandlerBusiness DataUploaderNotificationHandler;

        private  IGenericsRepository<T> _repo;
        protected  IGenericsRepository<T> repo
        {
            get
            {
                _repo = null;
                _repo = new GenericRepository<T>(new MainContext());
                return _repo;
            }
        }

        private IGenericsRepository<T> _repo_routing;
        protected IGenericsRepository<T> repo_routing
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
