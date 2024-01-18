using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataAccess.WFM;
using Models;
using Models.WFM;

namespace BusinessLogics
{
    public class BLJobOrder
    {

        public static List<VW_Route_Issue> GetJobOrderDetails(ViewJobOrderFilter viewJobOrderFilter, out int totalRecords)
        {
            return DAJobOrder.GetJobOrderDetails(viewJobOrderFilter, out totalRecords);
        }
        public static string GetViewRMJobOrderStatusCount(ViewAssignJobOrderFilter viewAssignJobOrderFilter)
        {
            return DAJobOrder.GetViewRMJobOrderStatusCount(viewAssignJobOrderFilter);
        }
        public static string GetViewJobOrderStatusCount(ViewAssignJobOrderFilter viewAssignJobOrderFilter)
        {
            return DAJobOrder.GetViewJobOrderStatusCount(viewAssignJobOrderFilter);
        }
        public static string GetViewRMJobOrderStatusTTCount(ViewAssignJobOrderFilter viewAssignJobOrderFilter)
        {
            return DAJobOrder.GetViewRMJobOrderStatusTTCount(viewAssignJobOrderFilter);
        }

        public static string GetViewJobOrderStatusCounttt(ViewAssignJobOrderFilter viewAssignJobOrderFilter)
        {
            return DAJobOrder.GetViewJobOrderStatusCounttt(viewAssignJobOrderFilter);
        }

        public static List<VW_Assign_Job_Order> GetAssignJobOrderDetails(ViewAssignJobOrderFilter viewAssignJobOrderFilter, out int totalRecords)
        {
            return DAJobOrder.GetAssignIssueRouteDetails(viewAssignJobOrderFilter, out totalRecords);
        }

        public static List<VW_Assign_Job_Order> GetAssignJobOrderDetailstt(ViewAssignJobOrderFilter viewAssignJobOrderFilter, out int totalRecords)
        {
            return DAJobOrder.GetAssignIssueRouteDetailstt(viewAssignJobOrderFilter, out totalRecords);
        }
        public static void GetAssignedRouteDetails(ViewAssignJobOrder objViewAssignedRoute, out int totalRecords, int isExport = 0, int isApi = 0)
        {
            DAJobOrder.GetAssignedRouteDetails(objViewAssignedRoute, out totalRecords, isExport, isApi);
        }
        public static List<Issue_Type_Master> GetIssueType()
        {
           
            return DAJobOrder.GetIssueType();
        }
        public static List<wfm_jo_type_master> GetJoType()
        {

            return DAJobOrder.GetJoType();
        }
        public static List<wfm_service_facility_master> GetServicesType()
        {

            return DAJobOrder.GetServicesType();
        }
        public static List<JoCategoryMaster> GetJoCategory()
        {
            return DAJobOrder.GetJoCategory();
        }

        public static JoCategoryMaster GetJoCategoryByCode(string Code)
        {
            return DAJobOrder.GetJoCategoryByCode(Code);
        }
        public static List<DropDownMaster> GetDropDownList(string ddType)
        {
            return DAJobOrder.GetDropDownList(ddType);
        }

        public static List<VW_Route_Issue> GetJobOrderDetailsRM(ViewJobOrderFilter viewJobOrderFilter, out int totalRecords)
        {
            return DAJobOrder.GetJobOrderDetailsRM(viewJobOrderFilter, out totalRecords);
        }

        public static List<VW_Route_Issue> GetttJobOrderDetailsRM(ViewJobOrderFilter viewJobOrderFilter, out int totalRecords)
        {
            return DAJobOrder.GetttJobOrderDetailsRM(viewJobOrderFilter, out totalRecords);
        }
        public static List<VW_Assign_Job_Order> GetAssignJobOrderDetailsRM(ViewAssignJobOrderFilter viewAssignJobOrderFilter, out int totalRecords)
        {
            return DAJobOrder.GetAssignIssueRouteDetailsRM(viewAssignJobOrderFilter, out totalRecords);
        }

        public static List<VW_Assign_Job_Order> GetAssignIssueRouteDetailsttRM(ViewAssignJobOrderFilter viewAssignJobOrderFilter, out int totalRecords)
        {
            return DAJobOrder.GetAssignIssueRouteDetailsttRM(viewAssignJobOrderFilter, out totalRecords);
        }


        public static List<DropDownMaster> GetDropDownListtt(string ddType)
        {
            return DAJobOrder.GetDropDownListtt(ddType);
        }
        public static List<VW_Route_Issue> GetJobOrderDetailstt(ViewJobOrderFilter viewJobOrderFilter, out int totalRecords)
        {
            return DAJobOrder.GetJobOrderDetailstt(viewJobOrderFilter, out totalRecords);
        }
        public static List<TT_Customer_Category> GetCustomerCategory()
        {

            return DAJobOrder.GetCustomerCategory();
        }
    
        public static List<TT_Customer_Segment> GetCustomerSegment()
        {

            return DAJobOrder.GetCustomerSegment();
        }

        public static List<TT_category> Getttcatgeory()
        {

            return DAJobOrder.Getttcatgeory();
        }
        public static TT_category GetttcatgeoryByName(string name)
        {

            return DAJobOrder.GetttcatgeoryByName(name);
        }
        public static TT_Type GetttTypeByName(string name)
        {

            return DAJobOrder.GettTypeByName(name);
        }
        public static TT_Customer_Segment GettCustomerSegmentByName(string name)
        {

            return DAJobOrder.GettCustomerSegmentByName(name);
        }
        public static TT_Customer_Category GettCustomerCategoryByName(string name)
        {

            return DAJobOrder.GettCustomerCategoryByName(name);
        }
        

        public static List<TT_Type> Getttype(string type)
        {

            return DAJobOrder.Getttype(type);
        }
        public static GetUserPermissionArea GetUserPermissionArea(int  userId)
        {

            return DAJobOrder.GetUserPermissionArea(userId);
        }
        public static List<VW_Assign_Job_Order> GetAdditionalMaterial(ViewAssignJobOrderFilter viewAssignJobOrderFilter, out int totalRecords)
        {
            return DAJobOrder.GetAdditionalMaterial(viewAssignJobOrderFilter, out totalRecords);
        }

        public static GetUserPermissionArea GetAllPermissionArea(int userId)
        {
            return DAJobOrder.GetAllPermissionArea(userId);
        }
        public static string getprovince(string regionname)
        {
            return DAJobOrder.getprovince(regionname);
        }
        public static string getsubdistrict(string provincename)
        {
            return DAJobOrder.getsubdistrict(provincename);
        }
        public static charts getwfmdashboardjostatus(ViewAssignJobOrder viewAssignJobOrder)
        {
            return DAJobOrder.GetViewJobOrderStatusCountDashboard(viewAssignJobOrder);
        }
        public static charts getwfmdashboardjofacility(ViewAssignJobOrder viewAssignJobOrder)
        {
            return DAJobOrder.GetViewJobOrderfacilityCountDashboard(viewAssignJobOrder);
        }
        public static charts getwfmdashboardjoissuedesc(ViewAssignJobOrder viewAssignJobOrder)
        {
            return DAJobOrder.GetViewJobOrderissuedescCountDashboard(viewAssignJobOrder);
        }
        //public static charts getwfmdashboardjostateorprovince(ViewAssignJobOrderFilter viewAssignJobOrderFilter)
        //{
        //    return DAJobOrder.GetViewJobOrderstateorprovinceCountDashboard(viewAssignJobOrderFilter);
        //}
        public static charts getwfmdashboardjocity(ViewAssignJobOrder viewAssignJobOrder,string stateorprivince, IList<string> lstdistrictname)
        {
            return DAJobOrder.GetViewJobOrdercityCountDashboard(viewAssignJobOrder, stateorprivince,lstdistrictname);
        }
        public static charts getwfmdashboardjoassignto(ViewAssignJobOrder viewAssignJobOrder, string assignto)
        {
            return DAJobOrder.GetViewJobOrderassigntoCountDashboard(viewAssignJobOrder, assignto);
        }
        public static charts getwfmdashboardjostatusRM(ViewJobOrder viewJobOrder)
        {
            return DAJobOrder.GetViewJobOrderStatusCountDashboardRM(viewJobOrder);
        }
        public static charts getwfmdashboardjofacilityRM(ViewJobOrder viewJobOrder)
        {
            return DAJobOrder.GetViewJobOrderfacilityCountDashboardRM(viewJobOrder);
        }
        public static charts getwfmdashboardjoissuedescRM(ViewJobOrder viewJobOrder)
        {
            return DAJobOrder.GetViewJobOrderissuedescCountDashboardRM(viewJobOrder);
        }
        public static charts getwfmdashboardjocityRM(ViewJobOrder viewJobOrder, string stateorprivince, IList<string> lstdistrictname)
        {
            return DAJobOrder.GetViewJobOrdercityCountDashboardRM(viewJobOrder, stateorprivince, lstdistrictname);
        }
        public static charts getwfmdashboardjoassigntoRM(ViewJobOrder viewJobOrder, string assignto)
        {
            return DAJobOrder.GetViewJobOrderassigntoCountDashboardRM(viewJobOrder, assignto);
        }
    }
}
