using Models;
using System.Web;
namespace SmartInventory.Helper
{
    public class AuthorizationHelper
    {
        public AuthorizeMessage IsAuthrozedForEntityCreation()
        {
            AuthorizeMessage objAuthorizeMessage= new AuthorizeMessage();
            if(((User)HttpContext.Current.Session["userDetail"]).role_id == 3)
            {
                objAuthorizeMessage.status = false;
                objAuthorizeMessage.message = "You are not authorized to create entity!";               
            }
            else
            {
                objAuthorizeMessage.status = true;
            }
            return objAuthorizeMessage;

        }
    }
}