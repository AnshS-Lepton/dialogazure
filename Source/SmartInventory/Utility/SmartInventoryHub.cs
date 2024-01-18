using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Utility
{
    [HubName("SmartInventoryHub")]
    public class SmartInventoryHub : Hub
    {
        public string userName { get; set; }
        private readonly static ConnectionMapping<string> _connections = new ConnectionMapping<string>();

        private SmartInventoryHub()
        {

        }

        private static SmartInventoryHub objSmartInventoryHub = null;
        private static readonly object lockObject = new object();
        public static SmartInventoryHub Instance
        {
            get
            {
                lock (lockObject)
                {
                    if (objSmartInventoryHub == null)
                    {
                        objSmartInventoryHub = new SmartInventoryHub();
                    }
                }
                return objSmartInventoryHub;
            }
        }
        public void BroadCastUploadStatus(string status)
        {
            //return Clients.All.MessageReceiver(DateTimeHelper.Now);
            //new BLUploadSummary().BLUploadSummayEvent += RecieveNotification;
            var context = GlobalHost.ConnectionManager.GetHubContext<SmartInventoryHub>();
            context.Clients.All.MessageReceiver(status);



            //Clients.Client(userName).MessageReceiver(userName + ": " + status);

            //Clients.Caller.MessageReceiver(status);
            //Clients.All.MessageReceiver(status);
        }
        public void BroadCastInfo(NotificationOutPut objNotification)
        {

            //return Clients.All.MessageReceiver(DateTimeHelper.Now);
            //new BLUploadSummary().BLUploadSummayEvent += RecieveNotification;
            var context = GlobalHost.ConnectionManager.GetHubContext<SmartInventoryHub>();
            // if (objNotification.sendToAllUser)
            // {
            context.Clients.All.MessageReceiver(objNotification);
            //}
            //else
            //{
            //    context.Clients.Client(objNotification.connectionId).MessageReceiver(objNotification);
            //}

            //Clients.Client(userName).MessageReceiver(userName + ": " + status);

            //Clients.Caller.MessageReceiver(status);
            //Clients.All.MessageReceiver(status);

        }

        public void BroadCastPrintMapStatus(string status)
        {
            //return Clients.All.MessageReceiver(DateTimeHelper.Now);
            //new BLUploadSummary().BLUploadSummayEvent += RecieveNotification;
            var context = GlobalHost.ConnectionManager.GetHubContext<SmartInventoryHub>();
            context.Clients.All.MessageReceiver(status);



            //Clients.Client(userName).MessageReceiver(userName + ": " + status);

            //Clients.Caller.MessageReceiver(status);
            //Clients.All.MessageReceiver(status);
        }

        public override Task OnConnected()
        {
            string name = Context.User.Identity.Name;
            _connections.Add(name, Context.ConnectionId);
            return base.OnConnected();
        }

        public override Task OnDisconnected(bool stopCalled)
        {
            string name = Context.User.Identity.Name;
            _connections.Remove(name, Context.ConnectionId);
            return base.OnDisconnected(stopCalled);
        }

        //public void SendChatMessage(string who, string message)
        //{
        //    string name = Context.User.Identity.Name;
        //    foreach (var connectionId in _connections.GetConnections(who))
        //    {
        //        Clients.Client(connectionId).addChatMessage(name + ": " + message);
        //    }
        //}
    }

    public class ConnectionMapping<T>
    {
        private readonly Dictionary<T, HashSet<string>> _connections =
            new Dictionary<T, HashSet<string>>();

        public int Count
        {
            get
            {
                return _connections.Count;
            }
        }

        public void Add(T key, string connectionId)
        {
            lock (_connections)
            {
                HashSet<string> connections;
                if (!_connections.TryGetValue(key, out connections))
                {
                    connections = new HashSet<string>();
                    _connections.Add(key, connections);
                }

                lock (connections)
                {
                    connections.Add(connectionId);
                }
            }
        }

        public IEnumerable<string> GetConnections(T key)
        {
            HashSet<string> connections;
            if (_connections.TryGetValue(key, out connections))
            {
                return connections;
            }

            return Enumerable.Empty<string>();
        }

        public void Remove(T key, string connectionId)
        {
            lock (_connections)
            {
                HashSet<string> connections;
                if (!_connections.TryGetValue(key, out connections))
                {
                    return;
                }

                lock (connections)
                {
                    connections.Remove(connectionId);

                    if (connections.Count == 0)
                    {
                        _connections.Remove(key);
                    }
                }
            }
        }
    }
    public class NotificationOutPut
    {
        public string connectionId { get; set; }
        public string notificationType { get; set; }
        public string info { get; set; }
        public bool sendToAllUser { get; set; }
        public dynamic data { get; set; }
    }
}
