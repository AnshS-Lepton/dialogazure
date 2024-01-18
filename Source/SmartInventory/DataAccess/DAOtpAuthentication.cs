using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataAccess.DBHelpers;
using Models.Admin;

namespace DataAccess
{
    public class DAOtpAuthentication : Repository<OTPAuthenticationSettings>
    {
        public OTPAuthenticationSettings getOtpConfigurationSetting(string applicationType)
        {
            try
            {
                if (!string.IsNullOrEmpty(applicationType))
                    return repo.Get(m => m.application_name.ToUpper() == applicationType.ToUpper());
                else
                    return null;
            }
            catch
            {
                throw;
            }
        }
        public OTPAuthenticationSettings updateOtpConfigurationSetting(OTPAuthenticationSettings obj, string applicationType)
        {
            try
            {
                var objOtpAuthentication = repo.Get(m => m.application_name == applicationType);
                if (obj.application_name != null)
                {
                    objOtpAuthentication.temp_lock_attempt = obj.temp_lock_attempt;
                    objOtpAuthentication.temp_lock_duration = obj.temp_lock_duration;
                    objOtpAuthentication.permanent_lock_attempt = obj.permanent_lock_attempt;
                    objOtpAuthentication.permanent_lock_duration = obj.permanent_lock_duration;
                    objOtpAuthentication.otp_resend_limit = obj.otp_resend_limit;
                    objOtpAuthentication.is_otp_enabled = obj.is_otp_enabled;
                    objOtpAuthentication.alert_message_timeout = obj.alert_message_timeout;
                    objOtpAuthentication.otp_min_value = obj.otp_min_value;
                    objOtpAuthentication.otp_max_value = obj.otp_max_value;
                    objOtpAuthentication.otp_expiry_time = obj.otp_expiry_time;
                    objOtpAuthentication.resend_otp_timer = obj.resend_otp_timer;

                    return repo.Update(objOtpAuthentication);
                }
                else
                {
                    return null;
                }
            }
            catch
            {
                throw;
            }
        }


    }
    public class DAEmailConfiguration : Repository<EmailEventTemplate>
    {
        public List<EmailEventTemplate> GetEmailTemplateDetail()
        {
            try
            {
                
                    return repo.GetAll().ToList();
                
                   
            }
            catch
            {
                throw;
            }
        }
        public EmailEventTemplate updateEmailConfigurationSetting(EmailEventTemplate obj)
        {
            try
            {
                var objEmailEventTemplate = repo.Get(m => m.trigger_event == obj.trigger_event);
                if (objEmailEventTemplate != null)
                {
                    objEmailEventTemplate.project_phase = obj.project_phase;
                    objEmailEventTemplate.trigger_event = obj.trigger_event;
                    objEmailEventTemplate.recipient_role = obj.recipient_role;
                    objEmailEventTemplate.recipient_list = obj.recipient_list;
                    objEmailEventTemplate.subject = obj.subject;
                    objEmailEventTemplate.template = obj.template;
                    objEmailEventTemplate.is_active = obj.is_active;
                    objEmailEventTemplate.remarks = obj.remarks;    
                    return repo.Update(objEmailEventTemplate);
                }
                else
                {
                    return null;
                }
            }
            catch
            {
                throw;
            }
        }
    }
  
}
