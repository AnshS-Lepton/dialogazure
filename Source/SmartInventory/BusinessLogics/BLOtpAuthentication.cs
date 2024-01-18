using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataAccess;
using Models.Admin;

namespace BusinessLogics
{
    public class BLOtpAuthentication
    {
        DAOtpAuthentication DAOtpAuthentication = new DAOtpAuthentication();
        public OTPAuthenticationSettings getOtpConfigurationSetting(string applicationType)
        {
            return DAOtpAuthentication.getOtpConfigurationSetting(applicationType);
        }
        public OTPAuthenticationSettings updateOtpConfigurationSetting(OTPAuthenticationSettings obj,string app)
        {
            return DAOtpAuthentication.updateOtpConfigurationSetting(obj, app);
        }
        public List<EmailEventTemplate> GetEmailTemplateDetail()
        {
            DAEmailConfiguration objDAEmailConfiguration = new DAEmailConfiguration();
            return objDAEmailConfiguration.GetEmailTemplateDetail();
        }
        public EmailEventTemplate updateEmailConfigurationSetting(EmailEventTemplate obj)
        {
            DAEmailConfiguration objDAEmailConfiguration = new DAEmailConfiguration();
            return objDAEmailConfiguration.updateEmailConfigurationSetting(obj);
        }
       
    }
}
