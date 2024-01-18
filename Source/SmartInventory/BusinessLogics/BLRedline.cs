using DataAccess;
using Models;
using System.Collections.Generic;
using static DataAccess.DANetworkTicket;

namespace BusinessLogics
{
    public class BLRedline

    {
        public List<Redline> GetRedLine(RedlineFilter objRedLineFilter)
        {
            return new DARedline().GetRedLine(objRedLineFilter);
        }
        public RedlineMaster GetRedlineById(int task_system_id)
        {
            return new DARedline().GetRedlineById(task_system_id);
        }

        public List<Dictionary<string, string>> Get_RedlineEntity_History(RedlineFilter objRedlineFilter)
        {
            return new DARedline().Get_RedlineEntity_History(objRedlineFilter);
        }
        public RedlineMaster SaveRedline(RedlineMaster objRedlineMaster, int userId)
        {
            return new DARedline().SaveRedline(objRedlineMaster, userId);
        }
        public List<RedlineStatusMaster> getStatusDropdown()
        {
            return new DARedline().getStatusDropdown();
        }
        public RedlineAssignedUsers GetAssignedUsersById(int task_system_id, int assigned_user)
        {
            return DARedlineAssignedUsers.Instance.GetAssignedUsersById(task_system_id, assigned_user);
        }
    }
}