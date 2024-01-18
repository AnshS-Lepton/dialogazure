using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using Npgsql;
using System.Net.Mail;
using System.Net;
using Models.WFM;
using Utility;
using System.IO;

namespace WFMNotificationService
{
    public partial class NotificationService : ServiceBase
    {
        Timer timer1 = new Timer();
        DateTime _scheduleTime; // Unassign 08 :00 AM
        Timer timer2 = new Timer();//Ack

        Timer timer3 = new Timer();//Unassign jo manager
        DateTime _scheduleTime2; // Unassign

        Timer timer4 = new Timer();

        Timer timer5 = new Timer();

        Timer timer6 = new Timer();
        DateTime _scheduleTime3; // Unassign 08 :00 AM

        Timer timer7 = new Timer();//report timer order fulfilment
        DateTime _scheduleTime4;// 08:00 PM

        Timer timer8 = new Timer();//report timer trouble ticket
        DateTime _scheduleTime5;

        public NotificationService()
        {
            InitializeComponent();

            double unassignedtime = Convert.ToDouble(ConfigurationManager.AppSettings["ScheduleTimeUnassingned"]);
            _scheduleTime = DateTime.Today.AddDays(1).AddHours(unassignedtime);  //Unassigned // timer1
            double unassignedtimeJOmanager = Convert.ToDouble(ConfigurationManager.AppSettings["ScheduleTimeUnassingned"]);
            _scheduleTime2 = DateTime.Today.AddDays(1).AddHours(unassignedtimeJOmanager); //Unassigned Jo manager //time3
            double beforeonedaytocustomer = Convert.ToDouble(ConfigurationManager.AppSettings["ScheduleTimeBeforeonedaytoCustomer"]);
            _scheduleTime3 = DateTime.Today.AddDays(1).AddHours(beforeonedaytocustomer); // before one day to customer //timer6
            double reportmail = Convert.ToDouble(ConfigurationManager.AppSettings["ScheduleTimeReportsMail"]);
            _scheduleTime4 = DateTime.Today.AddDays(1).AddHours(reportmail);// report email at 8 pm //timer7 
            double reportmailtt = Convert.ToDouble(ConfigurationManager.AppSettings["ScheduleTimeReportsMailTT"]);
            _scheduleTime5 = DateTime.Today.AddDays(1).AddHours(reportmailtt);
        }

        protected override void OnStart(string[] args)
        {
            LogWriter.LogWrite("Service Started at: " + DateTime.Now);
            timer1.Elapsed += new ElapsedEventHandler(UnAssign);
            double Interval1 = Convert.ToDouble(ConfigurationManager.AppSettings["UnassginedTimerInterval"]);
            if (Interval1 > 0)
            {
                //timer1.Interval = 1000 * 60 * 5;
                timer1.Interval = Interval1;
                timer1.Enabled = true;
                LogWriter.LogWrite("Un-Assigned : Execution Schedule at : " + (_scheduleTime.AddMilliseconds(Interval1)).ToString());
            }

            timer2.Elapsed += new ElapsedEventHandler(Acknowledge);
            double Interval2 = Convert.ToDouble(ConfigurationManager.AppSettings["AcknowledgeTimerInterval"]);
            if (Interval2 > 0)
            {
                //timer2.Interval = 1000 * 60 * 8;
                timer2.Interval = Interval2;
                timer2.Enabled = true;
            }

            timer3.Elapsed += new ElapsedEventHandler(UnAssigned_JoManager);
            double Interval3 = Convert.ToDouble(ConfigurationManager.AppSettings["UnAssignedJoManagerTimerInterval"]);
            if (Interval3 > 0)
            {
                //timer3.Interval = 1000 * 60 * 10;
                timer3.Interval = Interval3;
                timer3.Enabled = true;
                LogWriter.LogWrite("Un-Assigned Jo Manager : Execution Schedule at : " + (_scheduleTime2.AddMilliseconds(Interval3)).ToString());
            }

            timer4.Elapsed += new ElapsedEventHandler(Delayed);
            double Interval4 = Convert.ToDouble(ConfigurationManager.AppSettings["TaskDelayedTimerInterval"]);
            if (Interval4 > 0)
            {
                //timer4.Interval = 1000 * 60 * 30;
                timer4.Interval = Interval4;
                timer4.Enabled = true;
            }

            timer5.Elapsed += new ElapsedEventHandler(NoTask);
            double Interval5 = Convert.ToDouble(ConfigurationManager.AppSettings["NoTaskTimerInterval"]);
            if (Interval5 > 0)
            {
                //timer5.Interval = 1000 * 60 * 60;
                timer5.Interval = Interval5;
                timer5.Enabled = true;
            }

            timer6.Elapsed += new ElapsedEventHandler(BeforeOneDay);
            double Interval6 = Convert.ToDouble(ConfigurationManager.AppSettings["BeforeOneDayTimerInterval"]);
            if (Interval6 > 0)
            {
                //timer6.Interval = 1000 * 60 * 12;
                timer6.Interval = Interval6;
                timer6.Enabled = true;
                LogWriter.LogWrite("BeforeOneDay: Execution Schedule at : " + (_scheduleTime3.AddMilliseconds(Interval6)).ToString());
            }

            timer7.Elapsed += new ElapsedEventHandler(ReportEmail);
            double Interval7 = Convert.ToDouble(ConfigurationManager.AppSettings["ReportEmailTimerInterval"]);
            if (Interval7 > 0)
            {
                //timer7.Interval = 1000 * 60 * 60;
                timer7.Interval = Interval7;
                timer7.Enabled = true;
                LogWriter.LogWrite("ReportEmail : Execution Schedule at : " + (_scheduleTime4.AddMilliseconds(Interval7)).ToString());
            }

            timer8.Elapsed += new ElapsedEventHandler(ReportEmailTT);
            double Interval8 = Convert.ToDouble(ConfigurationManager.AppSettings["ReportEmailTimerIntervalTT"]);
            if (Interval8 > 0)
            {
                //timer8.Interval = 1000 * 60 * 60;
                timer8.Interval = Interval8;
                timer8.Enabled = true;
                LogWriter.LogWrite("ReportEmail TT : Execution Schedule at : " + (_scheduleTime5.AddMilliseconds(Interval8)).ToString());
            }
        }

        protected override void OnStop()
        {
            timer1.Enabled = false;
            timer2.Enabled = false;
            timer3.Enabled = false;
            timer4.Enabled = false;
            timer5.Enabled = false;
            timer6.Enabled = false;
            timer7.Enabled = false;
            LogWriter.LogWrite("Service Stop at: " + DateTime.Now);
        }

        private void UnAssign(object source, ElapsedEventArgs e)
        {
            try
            {
                LogWriter.LogWrite("Unassign1 executed at : " + DateTime.Now);
                if (DateTime.Now > _scheduleTime)
                {
                    LogWriter.LogWrite("Unassign1 executed at : " + DateTime.Now);
                    //this.NotificationACK("fn_wfm_unassign_notification");
                    Unassign1();
                    _scheduleTime = _scheduleTime.AddDays(1);
                    LogWriter.LogWrite("Next Execution Schedule at : " + _scheduleTime.ToString());
                }
                //else
                //{
                //    LogWriter.LogWrite("Unassign1 will be executed at : " + _scheduleTime);
                //}
            }
            catch (Exception ex)
            {
                LogWriter.LogWrite("Unassign1 not executed, Error in Processing : " + ex.Message);
            }
        }

        private void Acknowledge(object source, ElapsedEventArgs e)
        {
            try
            {
                LogWriter.LogWrite("Acknowledge executed at : " + DateTime.Now);
                // this.NotificationACK("fn_wfm_ack_notification");
                Acknowledge();
            }
            catch (Exception ex)
            {
                LogWriter.LogWrite("Acknowledge not executed, Error in Processing : " + ex.Message);
            }
        }

        private void UnAssigned_JoManager(object source, ElapsedEventArgs e)
        {
            try
            {
                LogWriter.LogWrite("Unassign2 executed at : " + DateTime.Now);
                if (DateTime.Now > _scheduleTime2)
                {
                    LogWriter.LogWrite("Unassign2 executed at : " + DateTime.Now);
                    Unassign2();
                    _scheduleTime2 = _scheduleTime2.AddDays(1);
                    LogWriter.LogWrite("Next Execution Schedule at : " + _scheduleTime2.ToString());
                }
                //else
                //{
                //    LogWriter.LogWrite("Unassign2 will be executed at : " + _scheduleTime2);
                //}
            }
            catch (Exception ex)
            {
                LogWriter.LogWrite("Unassign2 not executed, Error in Processing : " + ex.Message);
            }
        }

        private void Delayed(object source, ElapsedEventArgs e)
        {
            try
            {
                LogWriter.LogWrite("Delayed executed at : " + DateTime.Now);
                // this.NotificationACK("fn_wfm_Delayed_notification");
                Delayed();
            }
            catch (Exception ex)
            {
                LogWriter.LogWrite("Delayed not executed, Error in Processing : " + ex.Message);
            }
        }

        private void NoTask(object source, ElapsedEventArgs e)
        {
            try
            {
                LogWriter.LogWrite("NoTask executed at : " + DateTime.Now);
                //this.NotificationACK("fn_wfm_totask_notification_jomanager");
                NoTask();
            }
            catch (Exception ex)
            {
                LogWriter.LogWrite("NoTask not executed, Error in Processing : " + ex.Message);
            }
        }

        private void BeforeOneDay(object source, ElapsedEventArgs e)
        {
            try
            {
                if (DateTime.Now > _scheduleTime3)
                {
                    LogWriter.LogWrite("********************************************************************************");
                    LogWriter.LogWrite("BeforeOneDay executed at : " + DateTime.Now);
                    //this.NotificationACK("fn_wfm_unassign_notification");
                    this.BeforeOneDay();
                    _scheduleTime3 = _scheduleTime3.AddDays(1);
                    LogWriter.LogWrite("Next Execution Schedule at : " + _scheduleTime3.ToString());
                    LogWriter.LogWrite("********************************************************************************");
                }
                //else
                //{
                //    LogWriter.LogWrite("BeforeOneDay will be executed at : " + _scheduleTime3);
                //}
            }
            catch (Exception ex)
            {
                LogWriter.LogWrite("BeforeOneDay not executed, Error in Processing : " + ex);
            }
        }

        private void ReportEmail(object sender, ElapsedEventArgs e)
        {
            try
            {
                double Interval = Convert.ToDouble(ConfigurationManager.AppSettings["IntervalHoursReportMail"]);
                if (DateTime.Now > _scheduleTime4)
                {
                    LogWriter.LogWrite("********************************************************************************");
                    LogWriter.LogWrite("Report Email order fulfilment executed at : " + DateTime.Now);
                    ReportEmail();
                    _scheduleTime4 = _scheduleTime4.AddDays(Interval);
                    LogWriter.LogWrite("Next Execution Schedule at : " + _scheduleTime4.ToString());
                    LogWriter.LogWrite("********************************************************************************");
                }
            }
            catch (Exception ex)
            {
                LogWriter.LogWrite("ReportEmail not executed, Error in Processing : " + ex.Message);
            }
        }

        private void ReportEmailTT(object sender, ElapsedEventArgs e)
        {
            try
            {
                double Interval = Convert.ToDouble(ConfigurationManager.AppSettings["IntervalHoursReportMailTT"]);
                if (DateTime.Now > _scheduleTime5)
                {
                    LogWriter.LogWrite("********************************************************************************");
                    LogWriter.LogWrite("Report Email Trouble Ticket executed at : " + DateTime.Now);
                    ReportEmailTT();
                    _scheduleTime5 = _scheduleTime5.AddHours(Interval);
                    LogWriter.LogWrite("Next Execution Schedule at : " + _scheduleTime5.ToString());
                    LogWriter.LogWrite("********************************************************************************");
                }
            }
            catch (Exception ex)
            {
                LogWriter.LogWrite("ReportEmail TT not executed, Error in Processing : " + ex.Message);
            }
        }

        public void Unassign1()
        {
            string TemplatePath = Convert.ToString(ConfigurationManager.AppSettings["TemplatePath"]);
            string Subject = string.Empty;
            string TemplateName = string.Empty;
            string MsgBody = string.Empty;

            DataSet DsTemplate = getNotification("wfm_notification_template where Upper(type)='UNASSIGN1'");

            if (DsTemplate.Tables.Count > 0 && DsTemplate.Tables[0].Rows.Count > 0)
            {
                foreach (DataRow dr in DsTemplate.Tables[0].Rows)
                {
                    Subject = Convert.ToString(dr["subject"]);
                    TemplateName = Convert.ToString(dr["html_template"]);
                    string HtmlTemplate = Path.Combine(TemplatePath, TemplateName);
                    if (File.Exists(HtmlTemplate))
                    {
                        MsgBody = File.ReadAllText(HtmlTemplate, Encoding.UTF8);
                    }
                    else
                    {
                        LogWriter.LogWrite("Template file not found" + TemplateName);
                    }
                }
            }
            else
            {
                LogWriter.LogWrite("Template configurations not found");
                return;
            }

            DataSet ds = getNotification("vw_wfm_get_unassign1_not");
            if (ds.Tables.Count > 0)
            {
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    //int task_tracking_id = Convert.ToInt32(dr["task_tracking_id"]);
                    //int issue_id = Convert.ToInt32(dr["task_tracking_id"]);
                    //int frt_id = Convert.ToInt32(dr["frt_id"]);
                    //string hpsmid = Convert.ToString(dr["hpsmid"]);
                    DateTime appointment_date = Convert.ToDateTime(dr["appointment_date"]);
                    int manager_id = Convert.ToInt32(dr["manager_id"]);
                    int reporting_manager = Convert.ToInt32(dr["reporting_manager"]);
                    int jo_manager = Convert.ToInt32(dr["jo_manager"]);
                    string email = MiscHelper.Decrypt(Convert.ToString(dr["email"]));
                    string notification_type = Convert.ToString(dr["notification_type"]);
                    string user_name = Convert.ToString(dr["notification_type"]);
                    string reccount = Convert.ToString(dr["reccount"]);
                    string message = MsgBody.Replace("#reccount", reccount);

                    LogWriter.LogWrite("Template Name: " + TemplateName);
                    LogWriter.LogWrite("Emails configured: " + email);

                    int id = InsertNotification(notification_type, appointment_date, manager_id, user_name, reporting_manager, jo_manager, message, email);

                    if (id > 0)
                    {
                        if (!string.IsNullOrEmpty(email))
                        {
                            LogWriter.LogWrite("Sending email");
                            bool isMailsent = sendMail(email, Subject, message);
                            if (isMailsent)
                            {
                                LogWriter.LogWrite("Email sent successfully");
                                updateemailstatus(1, id);
                            }
                            else
                            {
                                LogWriter.LogWrite("Unable to send mail");
                            }
                        }
                    }
                }
            }
            else
            {
                LogWriter.LogWrite("Result not found");
            }
        }

        public void Acknowledge()
        {
            string TemplatePath = Convert.ToString(ConfigurationManager.AppSettings["TemplatePath"]);
            string TemplateName = string.Empty;
            string Subject = string.Empty;
            string MsgBody = string.Empty;

            DataSet DsTemplate = getNotification("wfm_notification_template where Upper(type)='ACKNOWLEDGE'");

            if (DsTemplate.Tables.Count > 0 && DsTemplate.Tables[0].Rows.Count > 0)
            {
                foreach (DataRow dr in DsTemplate.Tables[0].Rows)
                {
                    Subject = Convert.ToString(dr["subject"]);
                    TemplateName = Convert.ToString(dr["html_template"]);
                    string HtmlTemplate = Path.Combine(TemplatePath, TemplateName);
                    if (File.Exists(HtmlTemplate))
                    {
                        MsgBody = File.ReadAllText(HtmlTemplate, Encoding.UTF8);
                    }
                    else
                    {
                        LogWriter.LogWrite("Template file not found" + TemplateName);
                    }
                }
            }
            else
            {
                LogWriter.LogWrite("Template configurations not found");
                return;
            }

            DataSet ds = getNotification("vw_wfm_get_ack_not");
            if (ds.Tables.Count > 0)
            {
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    DateTime appointment_date = Convert.ToDateTime(dr["appointment_date"]);
                    int task_tracking_id = Convert.ToInt32(dr["task_tracking_id"]);
                    int issue_id = Convert.ToInt32(dr["issue_id"]);
                    int frt_id = Convert.ToInt32(dr["frt_id"]);
                    int manager_id = Convert.ToInt32(dr["manager_id"]);
                    int jo_manager = Convert.ToInt32(dr["jo_manager"]);
                    int reporting_manager = Convert.ToInt32(dr["reporting_manager"]);
                    string email = MiscHelper.Decrypt(Convert.ToString(dr["email"]));
                    string hpsmid = Convert.ToString(dr["hpsmid"]);
                    string notification_type = Convert.ToString(dr["notification_type"]);
                    string user_name = Convert.ToString(dr["user_name"]);
                    string message = MsgBody.Replace("#jobid", hpsmid).Replace("#username", user_name);

                    LogWriter.LogWrite("Template Name: " + TemplateName);
                    LogWriter.LogWrite("Emails configured: " + email);

                    int id = InsertNotification(notification_type, appointment_date, manager_id, user_name, reporting_manager, jo_manager, message, email, frt_id, hpsmid, task_tracking_id, issue_id);
                    if (id > 0)
                    {
                        if (!string.IsNullOrEmpty(email))
                        {
                            LogWriter.LogWrite("Sending email");
                            var isMailsent = sendMail(email, Subject, message);
                            updateemailstatus(isMailsent == true ? 1 : 0, id);
                            //update  mail sent
                        }
                    }
                }
            }
            else
            {
                LogWriter.LogWrite("Result not found");
            }
        }

        public void Unassign2()
        {
            string TemplatePath = Convert.ToString(ConfigurationManager.AppSettings["TemplatePath"]);
            string Subject = string.Empty;
            string TemplateName = string.Empty;
            string MsgBody = string.Empty;

            DataSet DsTemplate = getNotification("wfm_notification_template where Upper(type)='UNASSIGN2'");

            if (DsTemplate.Tables.Count > 0 && DsTemplate.Tables[0].Rows.Count > 0)
            {
                foreach (DataRow dr in DsTemplate.Tables[0].Rows)
                {
                    Subject = Convert.ToString(dr["subject"]);
                    TemplateName = Convert.ToString(dr["html_template"]);
                    string HtmlTemplate = Path.Combine(TemplatePath, TemplateName);
                    if (File.Exists(HtmlTemplate))
                    {
                        MsgBody = File.ReadAllText(HtmlTemplate, Encoding.UTF8);
                    }
                    else
                    {
                        LogWriter.LogWrite("Template file not found" + TemplateName);
                    }
                }
            }
            else
            {
                LogWriter.LogWrite("Template configurations not found");
                return;
            }

            DataSet ds = getNotification("vw_wfm_get_unassign1_notjomanager");
            if (ds.Tables.Count > 0)
            {
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    //int frt_id = Convert.ToInt32(dr["frt_id"]);
                    //int task_tracking_id = Convert.ToInt32(dr["task_tracking_id"]);
                    //int issue_id = Convert.ToInt32(dr["task_tracking_id"]);
                    //string hpsmid = Convert.ToString(dr["hpsmid"]);
                    DateTime appointment_date = Convert.ToDateTime(dr["appointment_date"]);
                    int manager_id = Convert.ToInt32(dr["manager_id"]);
                    int reporting_manager = Convert.ToInt32(dr["reporting_manager"]);
                    int jo_manager = Convert.ToInt32(dr["jo_manager"]);
                    string email = MiscHelper.Decrypt(Convert.ToString(dr["email"]));
                    string notification_type = Convert.ToString(dr["notification_type"]);
                    string user_name = Convert.ToString(dr["user_name"]);
                    string reccount = Convert.ToString(dr["reccount"]);
                    string message = MsgBody.Replace("#reccount", reccount).Replace("#username", user_name);

                    LogWriter.LogWrite("Template Name: " + TemplateName);
                    LogWriter.LogWrite("Emails configured: " + email);

                    int id = InsertNotification(notification_type, appointment_date, manager_id, user_name, reporting_manager, jo_manager, message, email);


                    if (id > 0)
                    {
                        if (!string.IsNullOrEmpty(email))
                        {
                            LogWriter.LogWrite("Sending email");
                            var isMailsent = sendMail(email, Subject, message);
                            updateemailstatus(isMailsent == true ? 1 : 0, id);
                            //update  mail sent
                        }
                    }
                }
            }
            else
            {
                LogWriter.LogWrite("Result not found");
            }
        }

        public void Delayed()
        {
            string TemplatePath = Convert.ToString(ConfigurationManager.AppSettings["TemplatePath"]);
            string Subject = string.Empty;
            string TemplateName = string.Empty;
            string MsgBody = string.Empty;

            DataSet DsTemplate = getNotification("wfm_notification_template where Upper(type)='DELAYED'");

            if (DsTemplate.Tables.Count > 0 && DsTemplate.Tables[0].Rows.Count > 0)
            {
                foreach (DataRow dr in DsTemplate.Tables[0].Rows)
                {
                    Subject = Convert.ToString(dr["subject"]);
                    TemplateName = Convert.ToString(dr["html_template"]);
                    string HtmlTemplate = Path.Combine(TemplatePath, TemplateName);
                    if (File.Exists(HtmlTemplate))
                    {
                        MsgBody = File.ReadAllText(HtmlTemplate, Encoding.UTF8);
                    }
                    else
                    {
                        LogWriter.LogWrite("Template file not found" + TemplateName);
                    }
                }
            }
            else
            {
                LogWriter.LogWrite("Template configurations not found");
                return;
            }

            DataSet ds = getNotification("vw_wfm_get_Delayed_notification");
            if (ds.Tables.Count > 0)
            {
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    DateTime appointment_date = Convert.ToDateTime(dr["appointment_date"]);
                    int task_tracking_id = Convert.ToInt32(dr["task_tracking_id"]);
                    int issue_id = Convert.ToInt32(dr["issue_id"]);
                    int frt_id = Convert.ToInt32(dr["frt_id"]);
                    int manager_id = Convert.ToInt32(dr["manager_id"]);
                    int reporting_manager = Convert.ToInt32(dr["reporting_manager"]);
                    int jo_manager = Convert.ToInt32(dr["jo_manager"]);
                    string email = MiscHelper.Decrypt(Convert.ToString(dr["email"]));
                    string hpsmid = Convert.ToString(dr["hpsmid"]);
                    string notification_type = Convert.ToString(dr["notification_type"]);
                    string user_name = Convert.ToString(dr["user_name"]);
                    string message = MsgBody.Replace("#jobid", hpsmid).Replace("#username", user_name);

                    LogWriter.LogWrite("Template Name: " + TemplateName);
                    LogWriter.LogWrite("Emails configured: " + email);

                    int id = InsertNotification(notification_type, appointment_date, manager_id, user_name, reporting_manager, jo_manager, message, email, frt_id, hpsmid, task_tracking_id, issue_id);


                    if (id > 0)
                    {
                        if (!string.IsNullOrEmpty(email))
                        {
                            LogWriter.LogWrite("Sending email");
                            var isMailsent = sendMail(email, Subject, message);
                            updateemailstatus(isMailsent == true ? 1 : 0, id);
                            //update  mail sent
                        }
                    }
                }
            }
            else
            {
                LogWriter.LogWrite("Result not found");
            }

        }

        public void NoTask()
        {
            string TemplatePath = Convert.ToString(ConfigurationManager.AppSettings["TemplatePath"]);
            string Subject = string.Empty;
            string TemplateName = string.Empty;
            string MsgBody = string.Empty;

            DataSet DsTemplate = getNotification("wfm_notification_template where Upper(type)='NOTASK'");

            if (DsTemplate.Tables.Count > 0 && DsTemplate.Tables[0].Rows.Count > 0)
            {
                foreach (DataRow dr in DsTemplate.Tables[0].Rows)
                {
                    Subject = Convert.ToString(dr["subject"]);
                    TemplateName = Convert.ToString(dr["html_template"]);
                    string HtmlTemplate = Path.Combine(TemplatePath, TemplateName);
                    if (File.Exists(HtmlTemplate))
                    {
                        MsgBody = File.ReadAllText(HtmlTemplate, Encoding.UTF8);
                    }
                    else
                    {
                        LogWriter.LogWrite("Template file not found" + TemplateName);
                    }
                }
            }
            else
            {
                LogWriter.LogWrite("Template configurations not found");
                return;
            }

            DataSet ds = getNotification("vw_wfm_get_notask_notification");
            if (ds.Tables.Count > 0)
            {

                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    DateTime appointment_date = Convert.ToDateTime(dr["appointment_date"]);
                    int task_tracking_id = Convert.ToInt32(dr["task_tracking_id"]);
                    int issue_id = Convert.ToInt32(dr["issue_id"]);
                    int frt_id = 0;
                    int manager_id = Convert.ToInt32(dr["manager_id"]);
                    int reporting_manager = Convert.ToInt32(dr["reporting_manager"]);
                    int jo_manager = Convert.ToInt32(dr["jo_manager"]);
                    string email = MiscHelper.Decrypt(Convert.ToString(dr["email"]));
                    string notification_type = Convert.ToString(dr["notification_type"]);
                    string user_name = Convert.ToString(dr["user_name"]);
                    string hpsmid = "";
                    string message = MsgBody.Replace("#username", user_name);

                    LogWriter.LogWrite("Template Name: " + TemplateName);
                    LogWriter.LogWrite("Emails configured: " + email);

                    int id = InsertNotification(notification_type, appointment_date, manager_id, user_name, reporting_manager, jo_manager, message, email, frt_id, hpsmid, task_tracking_id, issue_id);

                    if (id > 0)
                    {
                        if (!string.IsNullOrEmpty(email))
                        {
                            LogWriter.LogWrite("Sending email");
                            var isMailsent = sendMail(email, Subject, message);
                            updateemailstatus(isMailsent == true ? 1 : 0, id);
                            //update  mail sent
                        }
                    }
                }
            }
            else
            {
                LogWriter.LogWrite("Result not found");
            }
        }

        public void BeforeOneDay()
        {
            string TemplatePath = Convert.ToString(ConfigurationManager.AppSettings["TemplatePath"]);
            string Subject = string.Empty;
            string TemplateName = string.Empty;
            string MsgBody = string.Empty;
            DataSet DsTemplate = getNotification("wfm_notification_template where Upper(type)='BEFORE_ONE_DAY'");
            if (DsTemplate.Tables.Count > 0 && DsTemplate.Tables[0].Rows.Count > 0)
            {
                foreach (DataRow dr in DsTemplate.Tables[0].Rows)
                {
                    Subject = Convert.ToString(dr["subject"]);
                    TemplateName = Convert.ToString(dr["html_template"]);
                    string HtmlTemplate = Path.Combine(TemplatePath, TemplateName);
                    if (File.Exists(HtmlTemplate))
                    {
                        MsgBody = File.ReadAllText(HtmlTemplate, Encoding.UTF8);
                    }
                    else
                    {
                        LogWriter.LogWrite("Template file not found" + TemplateName);
                    }
                }
            }
            else
            {
                LogWriter.LogWrite("Template configurations not found");
                return;
            }
            DataSet ds = getNotification("vw_wfm_get_before_one_day");
            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                DateTime appointment_date = Convert.ToDateTime(dr["appointment_date"]);
                string hpsmid = Convert.ToString(dr["hpsmid"]);
                string notification_type = Convert.ToString(dr["notification_type"]);
                string account_no = Convert.ToString(dr["account_number"]);
                string slot_time = Convert.ToString(dr["slot_time"]);
                string email = MiscHelper.Decrypt(Convert.ToString(dr["email"]));
                string mobileno = Convert.ToString(dr["mobileno"]);
                string message = MsgBody.Replace("#date",appointment_date.ToString("dd-MMM-yyyy")).Replace("#account", account_no);
                LogWriter.LogWrite("Template Name: " + TemplateName);
                LogWriter.LogWrite("jobOrderId: " + hpsmid);
                LogWriter.LogWrite("appointment_date: " + appointment_date);
                LogWriter.LogWrite("notification_type: " + notification_type);
                LogWriter.LogWrite("account_no: " + account_no);
                LogWriter.LogWrite("slot_time: " + slot_time);
                LogWriter.LogWrite("email: " + email);
                LogWriter.LogWrite("mobileno: " + mobileno);
                var isMailsent = sendMail(email, Subject, message);
                var notification = InsertCustomerNotification(hpsmid, appointment_date, isMailsent == true ? 1 : 0, DateTime.Now, "", notification_type);
                if (notification == 0)
                {
                    LogWriter.LogWrite("Email not sent");
                }
                if (!string.IsNullOrEmpty(mobileno))
                {
                    string smsremark = "";
                    int sentsms = SMS.SendSms(ref smsremark, mobileno, message);
                    UpdateCustomerNotification(sentsms, DateTime.Now, smsremark, hpsmid);
                    if (sentsms == 0)
                    {
                        LogWriter.LogWrite("SMS not sent");
                    }
                }
            }
        }

        public void ReportEmail()
        {
            try
            {
                int ReportDays = Convert.ToInt32(ConfigurationManager.AppSettings["ReportSpan"]);
                string LocalPath = Convert.ToString(ConfigurationManager.AppSettings["ReportPath"]);
                string TemplatePath = Convert.ToString(ConfigurationManager.AppSettings["TemplatePath"]);
                string ExcelFileName = "";

                DateTime toDate = DateTime.Now;
                DateTime fromDate = toDate.AddDays(-ReportDays);

                string strFromDate = MiscHelper.FormatDate(fromDate.ToString());
                string strToDate = MiscHelper.FormatDate(toDate.ToString());

                DataSet DsTemplate = getNotification("wfm_notification_template");

                // got html template file name ,subject , report name,emails to send report 

                if (DsTemplate.Tables.Count > 0 && DsTemplate.Tables[0].Rows.Count > 0)
                {
                    foreach (DataRow dr in DsTemplate.Tables[0].Rows)
                    {
                        string ReportType = Convert.ToString(dr["type"]);
                        string Emails = Convert.ToString(dr["email"]);
                        string Subject = Convert.ToString(dr["subject"]);
                        Subject = Subject + " " + DateTime.Now.ToString("dd-MM-yyyy");
                        string TemplateName = Convert.ToString(dr["html_template"]);
                        string HtmlTemplate = Path.Combine(TemplatePath, TemplateName);
                        string MsgBody = "";

                        switch (ReportType.ToUpper())
                        {
                            case "JOB ORDER REPORT":
                                if (File.Exists(HtmlTemplate))
                                {
                                    LogWriter.LogWrite("Template Name: " + TemplateName);
                                    LogWriter.LogWrite("Start date: " + strFromDate);
                                    LogWriter.LogWrite("End date: " + strToDate);
                                    LogWriter.LogWrite("Emails configured: " + Emails);

                                    MsgBody = File.ReadAllText(HtmlTemplate, Encoding.UTF8);
                                    MsgBody = MsgBody.Replace("#FromDate", strFromDate);
                                    MsgBody = MsgBody.Replace("#ToDate", strToDate);

                                    DataSet DsJor = getNotification("fn_wfm_get_jo_report('" + strFromDate + "','" + strToDate + "',1)");// 1 for order fulfilment
                                    if (DsJor.Tables.Count > 0 && DsJor.Tables[0].Rows.Count > 0)
                                    {
                                        ExcelFileName = string.Format("{0}{1}.xlsx", ReportType.Replace(' ', '_'), DateTime.Now.ToString("_MM-dd-yy-HHmmss"));
                                        LogWriter.LogWrite("File Name : " + ExcelFileName);
                                        LogWriter.LogWrite("Number of Records found : " + DsJor.Tables[0].Rows.Count);
                                        ExcelFileName = Path.Combine(LocalPath, ExcelFileName);
                                        ExcelFileName = commonUtil.DataTableToExcel("xlsx", DsJor.Tables[0], ExcelFileName);
                                        LogWriter.LogWrite("File saved successfully");
                                        LogWriter.LogWrite("Sending email");
                                        bool isMailSent = sendMail(Emails, Subject, MsgBody, ExcelFileName);
                                        if (isMailSent)
                                        {
                                            LogWriter.LogWrite("Email sent successfully");
                                            File.Delete(ExcelFileName);
                                            LogWriter.LogWrite("Excel File deleted successfully");
                                        }
                                        else
                                        {
                                            LogWriter.LogWrite("Unable to send mail");
                                        }

                                    }
                                    else
                                    {
                                        LogWriter.LogWrite("Report data not available.");
                                    }
                                }
                                else
                                {
                                    LogWriter.LogWrite("Template file not found" + TemplateName);
                                }
                                break;
                            case "ADDITIONAL MATERIAL REPORT":
                                if (File.Exists(HtmlTemplate))
                                {
                                    LogWriter.LogWrite("Template Name: " + TemplateName);
                                    LogWriter.LogWrite("Start date: " + strFromDate);
                                    LogWriter.LogWrite("End date: " + strToDate);
                                    LogWriter.LogWrite("Emails configured: " + Emails);

                                    MsgBody = File.ReadAllText(HtmlTemplate, Encoding.UTF8);
                                    MsgBody = MsgBody.Replace("#FromDate", strFromDate);
                                    MsgBody = MsgBody.Replace("#ToDate", strToDate);

                                    DataSet DsAdr = getNotification("fn_wfm_get_additional_material_report('" + strFromDate + "','" + strToDate + "',1)");
                                    if (DsAdr.Tables.Count > 0 && DsAdr.Tables[0].Rows.Count > 0)
                                    {
                                        ExcelFileName = string.Format("{0}{1}.xlsx", ReportType.Replace(' ', '_'), DateTime.Now.ToString("_MM-dd-yy-HHmmss"));
                                        LogWriter.LogWrite("File Name : " + ExcelFileName);
                                        LogWriter.LogWrite("Number of Records found : " + DsAdr.Tables[0].Rows.Count);
                                        ExcelFileName = Path.Combine(LocalPath, ExcelFileName);
                                        ExcelFileName = commonUtil.DataTableToExcel("xlsx", DsAdr.Tables[0], ExcelFileName);
                                        LogWriter.LogWrite("File saved successfully");
                                        LogWriter.LogWrite("Sending email");
                                        bool isMailSent = sendMail(Emails, Subject, MsgBody, ExcelFileName);
                                        if (isMailSent)
                                        {
                                            LogWriter.LogWrite("Email sent successfully");
                                            File.Delete(ExcelFileName);
                                            LogWriter.LogWrite("Excel File deleted successfully");
                                        }
                                        else
                                        {
                                            LogWriter.LogWrite("Unable to send mail");
                                        }
                                    }
                                    else
                                    {
                                        LogWriter.LogWrite("Report data not available.");
                                    }
                                }
                                else
                                {
                                    LogWriter.LogWrite("Template file not found" + TemplateName);
                                }
                                break;
                            default:
                                //do nothing
                                break;
                        }
                    }
                }
                else
                {
                    LogWriter.LogWrite("Template configurations not found");
                }
            }
            catch
            {
                throw;
            }
        }

        public void ReportEmailTT()
        {
            try
            {
                int ReportDays = Convert.ToInt32(ConfigurationManager.AppSettings["ReportSpan"]);
                string LocalPath = Convert.ToString(ConfigurationManager.AppSettings["ReportPath"]);
                string TemplatePath = Convert.ToString(ConfigurationManager.AppSettings["TemplatePath"]);
                string ExcelFileName = "";

                DateTime toDate = DateTime.Now;
                DateTime fromDate = toDate.AddDays(-ReportDays);

                string strFromDate = MiscHelper.FormatDate(fromDate.ToString());
                string strToDate = MiscHelper.FormatDate(toDate.ToString());

                DataSet DsTemplate = getNotification("wfm_notification_template");

                // got html template file name ,subject , report name,emails to send report 

                if (DsTemplate.Tables.Count > 0 && DsTemplate.Tables[0].Rows.Count > 0)
                {
                    foreach (DataRow dr in DsTemplate.Tables[0].Rows)
                    {
                        string ReportType = Convert.ToString(dr["type"]);
                        string Emails = Convert.ToString(dr["email"]);
                        string Subject = Convert.ToString(dr["subject"]);
                        Subject = Subject + " " + DateTime.Now.ToString("dd-MM-yyyy");
                        string TemplateName = Convert.ToString(dr["html_template"]);
                        string HtmlTemplate = Path.Combine(TemplatePath, TemplateName);
                        string MsgBody = "";

                        switch (ReportType.ToUpper())
                        {
                            case "TT JOB ORDER REPORT":
                                if (File.Exists(HtmlTemplate))
                                {
                                    LogWriter.LogWrite("Template Name: " + TemplateName);
                                    LogWriter.LogWrite("Start date: " + strFromDate);
                                    LogWriter.LogWrite("End date: " + strToDate);
                                    LogWriter.LogWrite("Emails configured: " + Emails);

                                    MsgBody = File.ReadAllText(HtmlTemplate, Encoding.UTF8);
                                    MsgBody = MsgBody.Replace("#FromDate", strFromDate);
                                    MsgBody = MsgBody.Replace("#ToDate", strToDate);

                                    DataSet DsTTJor = getNotification("fn_wfm_get_tt_jo_report('" + strFromDate + "','" + strToDate + "',4)");// 4 for trouble ticket
                                    if (DsTTJor.Tables.Count > 0 && DsTTJor.Tables[0].Rows.Count > 0)
                                    {
                                        ExcelFileName = string.Format("{0}{1}.xlsx", ReportType.Replace(' ', '_'), DateTime.Now.ToString("_MM-dd-yy-HHmmss"));
                                        LogWriter.LogWrite("File Name : " + ExcelFileName);
                                        LogWriter.LogWrite("Number of Records found : " + DsTTJor.Tables[0].Rows.Count);
                                        ExcelFileName = Path.Combine(LocalPath, ExcelFileName);
                                        ExcelFileName = commonUtil.DataTableToExcel("xlsx", DsTTJor.Tables[0], ExcelFileName);
                                        LogWriter.LogWrite("File saved successfully");
                                        LogWriter.LogWrite("Sending email");
                                        bool isMailSent = sendMail(Emails, Subject, MsgBody, ExcelFileName);
                                        if (isMailSent)
                                        {
                                            LogWriter.LogWrite("Email sent successfully");
                                            File.Delete(ExcelFileName);
                                            LogWriter.LogWrite("Excel File deleted successfully");
                                        }
                                        else
                                        {
                                            LogWriter.LogWrite("Unable to send mail");
                                        }
                                    }
                                    else
                                    {
                                        LogWriter.LogWrite("Report data not available.");
                                    }
                                }
                                else
                                {
                                    LogWriter.LogWrite("Template file not found" + TemplateName);
                                }
                                break;
                            case "TT ADDITIONAL MATERIAL REPORT":
                                if (File.Exists(HtmlTemplate))
                                {
                                    LogWriter.LogWrite("Template Name: " + TemplateName);
                                    LogWriter.LogWrite("Start date: " + strFromDate);
                                    LogWriter.LogWrite("End date: " + strToDate);
                                    LogWriter.LogWrite("Emails configured: " + Emails);

                                    MsgBody = File.ReadAllText(HtmlTemplate, Encoding.UTF8);
                                    MsgBody = MsgBody.Replace("#FromDate", strFromDate);
                                    MsgBody = MsgBody.Replace("#ToDate", strToDate);

                                    DataSet DsttAdr = getNotification("fn_wfm_get_tt_additional_material_report('" + strFromDate + "','" + strToDate + "',4)");

                                    if (DsttAdr.Tables.Count > 0 && DsttAdr.Tables[0].Rows.Count > 0)
                                    {
                                        ExcelFileName = string.Format("{0}{1}.xlsx", ReportType.Replace(' ', '_'), DateTime.Now.ToString("_MM-dd-yy-HHmmss"));
                                        LogWriter.LogWrite("File Name : " + ExcelFileName);
                                        LogWriter.LogWrite("Number of Records found : " + DsttAdr.Tables[0].Rows.Count);
                                        ExcelFileName = Path.Combine(LocalPath, ExcelFileName);
                                        ExcelFileName = commonUtil.DataTableToExcel("xlsx", DsttAdr.Tables[0], ExcelFileName);
                                        LogWriter.LogWrite("File saved successfully");
                                        LogWriter.LogWrite("Sending email");
                                        bool isMailSent = sendMail(Emails, Subject, MsgBody, ExcelFileName);
                                        if (isMailSent)
                                        {
                                            LogWriter.LogWrite("Email sent successfully");
                                            File.Delete(ExcelFileName);
                                            LogWriter.LogWrite("Excel File deleted successfully");
                                        }
                                        else
                                        {
                                            LogWriter.LogWrite("Unable to send mail");
                                        }
                                    }
                                    else
                                    {
                                        LogWriter.LogWrite("Report data not available.");
                                    }
                                }
                                else
                                {
                                    LogWriter.LogWrite("Template file not found" + TemplateName);
                                }
                                break;
                            default:
                                //do nothing
                                break;
                        }
                    }
                }
                else
                {
                    LogWriter.LogWrite("Template configurations not found");
                }
            }
            catch
            {
                throw;
            }
        }

        public DataSet getNotification(string fn_name)
        {
            string connectionString = ConfigurationManager.AppSettings["constr"];
            DataSet dataSet = new DataSet();
            using (NpgsqlConnection _con = new NpgsqlConnection(connectionString))
            {
                try
                {
                    NpgsqlCommand cmd = new NpgsqlCommand();
                    cmd.Connection = _con;
                    _con.Open();
                    cmd.CommandType = CommandType.Text;
                    cmd.CommandText = "select * from public." + fn_name;
                    // LogWriter.WriteToLog(cmd.CommandText, 1);
                    NpgsqlDataAdapter dataAdapter = new NpgsqlDataAdapter(cmd);
                    //dataSet.Reset();
                    dataAdapter.Fill(dataSet);
                    _con.Close();
                }
                catch (Exception ex)
                {
                    LogWriter.LogWrite("Error in fetching customers, Error msg: " + ex.Message);
                }

            }
            return dataSet;
        }

        public void NotificationACK(string ProcedureName)
        {
            string connectionString = ConfigurationSettings.AppSettings["constr"];
            NpgsqlConnection _con = new NpgsqlConnection(connectionString);
            NpgsqlCommand cmd = new NpgsqlCommand();
            cmd.Connection = _con;
            NpgsqlTransaction npgsqlTrans = null;
            npgsqlTrans = null;
            _con.Open();
            npgsqlTrans = _con.BeginTransaction();
            cmd.Transaction = npgsqlTrans;
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = ProcedureName;
            cmd.Parameters.Clear();
            try
            {
                var str = cmd.ExecuteNonQuery();
                npgsqlTrans.Commit();
                LogWriter.LogWrite("Procedure commited at : " + DateTime.Now);
            }
            catch (Exception ex)
            {
                npgsqlTrans.Rollback();
                LogWriter.LogWrite("connect. Exp : " + ex.Message + ". Inner Msg :" + (ex.InnerException != null ? ex.InnerException.Message : ""));
            }
            finally
            {
                npgsqlTrans.Dispose();
                _con.Close();
            }
        }

        public bool sendMail(string To, string Subject, string Message)
        {
            bool isMailSent = false;
            try
            {
                int Port = Convert.ToInt32(ConfigurationManager.AppSettings["Port"]);
                string From = ConfigurationManager.AppSettings["From"];
                string Password = ConfigurationManager.AppSettings["Password"];
                string Host = ConfigurationManager.AppSettings["Host"];
                string Body = Message;
                try
                {
                    MailMessage message = new MailMessage();
                    SmtpClient smtp = new SmtpClient();
                    message.From = new MailAddress(From, "Converge");
                    message.To.Add(new MailAddress(To));
                    message.Subject = Subject;
                    message.IsBodyHtml = true;
                    message.Body = Body;
                    smtp.Port = Port;
                    smtp.Host = Host;
                    smtp.EnableSsl = false;
                    smtp.UseDefaultCredentials = false;
                    smtp.Credentials = new NetworkCredential(From, Password);
                    smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
                    smtp.Send(message);
                    isMailSent = true;
                    LogWriter.LogWrite("Email send succussfully to :  " + To + " on " + DateTime.Now);
                }
                catch (Exception ex)
                {
                    LogWriter.LogWrite("Email inner catch executed at : " + DateTime.Now + " error:" + ex.Message);
                }

            }
            catch (Exception ex)
            {
                LogWriter.LogWrite("Email outer catch executed at : " + DateTime.Now + " error:" + ex.Message);
            }
            return isMailSent;
        }

        public bool sendMail(string To, string Subject, string Message, string AttachmentPath)
        {
            bool isMailSent = false;
            try
            {
                int Port = Convert.ToInt32(ConfigurationManager.AppSettings["Port"]);
                string From = ConfigurationManager.AppSettings["From"];
                string Password = ConfigurationManager.AppSettings["Password"];
                string Host = ConfigurationManager.AppSettings["Host"];
                string Body = Message;
                try
                {
                    MailMessage message = new MailMessage();
                    SmtpClient smtp = new SmtpClient();
                    message.From = new MailAddress(From, "Converge");
                    string[] Multi = To.Split(',');
                    foreach (string s in Multi)
                    {
                        message.To.Add(new MailAddress(s));
                    }
                    message.Subject = Subject;
                    message.IsBodyHtml = true;
                    message.Body = Body;
                    message.Attachments.Add(new Attachment(AttachmentPath));
                    smtp.Port = Port;
                    smtp.Host = Host;
                    smtp.EnableSsl = false;
                    smtp.UseDefaultCredentials = false;
                    smtp.Credentials = new NetworkCredential(From, Password);
                    smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
                    smtp.Send(message);
                    isMailSent = true;
                    for (int i = 0; i < message.Attachments.Count; i++)
                    {
                        message.Attachments[i].Dispose();
                    }
                    message.Attachments.Clear();
                    message.Attachments.Dispose();
                    message.Dispose();
                    message = null;
                }
                catch (Exception ex)
                {
                    LogWriter.LogWrite("Email inner catch exception :" + ex.Message);
                }

            }
            catch (Exception ex)
            {
                LogWriter.LogWrite("Email outer catch exception:" + ex.Message);
            }
            return isMailSent;
        }

        public int InsertNotification(string notification_type, DateTime appointment_date, int manager_id, string user_name, int reporting_manager, int jo_manager, string message, string email, int frt_id = 0, string hpsmid = "", int task_tracking_id = 0, int issue_id = 0)
        {
            int id = 0;
            string connectionString = ConfigurationManager.AppSettings["constr"];
            DataSet dataSet = new DataSet();
            using (NpgsqlConnection _con = new NpgsqlConnection(connectionString))
            {
                try
                {
                    string pgcommand = "";
                    NpgsqlCommand cmd = new NpgsqlCommand();
                    cmd.Connection = _con;
                    cmd.CommandType = CommandType.Text;
                    _con.Open();
                    pgcommand = "insert into wfm_notification(notification_type,appointment_date,manager_id,user_name,reporting_manager,jo_manager,message,email_id,frt_id,hpsmid,task_tracking_id,issue_id) values (@notification_type,@appointment_date,@manager_id,@user_name,@reporting_manager,@jo_manager,@message,@email_id,@frt_id,@hpsmid,@task_tracking_id,@issue_id) returning id";
                    cmd.CommandText = pgcommand;
                    cmd.Parameters.AddWithValue("@task_tracking_id", task_tracking_id);
                    cmd.Parameters.AddWithValue("@issue_id", issue_id);
                    cmd.Parameters.AddWithValue("@notification_type", notification_type);
                    cmd.Parameters.AddWithValue("@appointment_date", appointment_date);
                    cmd.Parameters.AddWithValue("@manager_id", manager_id);
                    cmd.Parameters.AddWithValue("@user_name", user_name);
                    cmd.Parameters.AddWithValue("@reporting_manager", reporting_manager);
                    cmd.Parameters.AddWithValue("@jo_manager", jo_manager);
                    cmd.Parameters.AddWithValue("@message", message);
                    cmd.Parameters.AddWithValue("@email_id", email);
                    cmd.Parameters.AddWithValue("@frt_id", frt_id);
                    cmd.Parameters.AddWithValue("@hpsmid", hpsmid);
                    id = (int)cmd.ExecuteScalar();
                    _con.Close();
                }
                catch (Exception ex)
                {
                    LogWriter.LogWrite("Error InsertLog: " + ex.Message);
                }
                return id;
            }
        }

        public int InsertCustomerNotification(string joborderid, DateTime appointment_date, int emailstatus, DateTime emaildeliverytime, string emailremark, string type)
        {
            int id = 0;
            string connectionString = ConfigurationManager.AppSettings["constr"];
            DataSet dataSet = new DataSet();
            using (NpgsqlConnection _con = new NpgsqlConnection(connectionString))
            {
                try
                {
                    string pgcommand = "";
                    NpgsqlCommand cmd = new NpgsqlCommand();
                    cmd.Connection = _con;
                    cmd.CommandType = CommandType.Text;
                    _con.Open();
                    pgcommand = "insert into wfm_email_sms_log(joborderid,appointment_date,emailstatus,emaildeliverytime,emailremark,type) values (@joborderid,@appointment_date,joborderid,appointment_date,emailstatus,emaildeliverytime,emailremark,type,emailstatus,emaildeliverytime,emailremark,type) returning id";
                    cmd.CommandText = pgcommand;
                    cmd.Parameters.AddWithValue("@joborderid", joborderid);
                    cmd.Parameters.AddWithValue("@appointment_date", appointment_date);
                    cmd.Parameters.AddWithValue("@emailstatus", emailstatus);
                    cmd.Parameters.AddWithValue("@emaildeliverytime", emaildeliverytime);
                    cmd.Parameters.AddWithValue("@emailremark", emailremark);
                    cmd.Parameters.AddWithValue("@type", type);
                    id = (int)cmd.ExecuteScalar();
                    _con.Close();
                }
                catch (Exception ex)
                {
                    LogWriter.LogWrite("Error InsertLog: " + ex.Message);
                }
                return id;
            }
        }

        public int UpdateCustomerNotification(int smsstatus, DateTime smsdeliverytime, string smsremark, string joborderid)
        {
            int id = 0;
            string connectionString = ConfigurationManager.AppSettings["constr"];
            DataSet dataSet = new DataSet();
            using (NpgsqlConnection _con = new NpgsqlConnection(connectionString))
            {
                try
                {
                    string pgcommand = "";
                    NpgsqlCommand cmd = new NpgsqlCommand();
                    cmd.Connection = _con;
                    cmd.CommandType = CommandType.Text;
                    _con.Open();
                    pgcommand = "update wfm_email_sms_log set smsstatus=@smsstatus,smsdeliverytime=@smsdeliverytime,smsremark=@smsremark,joborderid=@joborderid";
                    cmd.CommandText = pgcommand;
                    cmd.Parameters.AddWithValue("@smsstatus", smsstatus);
                    cmd.Parameters.AddWithValue("@smsdeliverytime", smsdeliverytime);
                    cmd.Parameters.AddWithValue("@smsremark", smsremark);
                    cmd.Parameters.AddWithValue("@joborderid", joborderid);
                    id = (int)cmd.ExecuteScalar();
                    _con.Close();
                }
                catch (Exception ex)
                {
                    LogWriter.LogWrite("Error InsertLog: " + ex.Message);
                }
                return id;
            }
        }

        public void updateemailstatus(int status, int id)
        {
            string connectionString = ConfigurationManager.AppSettings["constr"];
            DataSet dataSet = new DataSet();
            using (NpgsqlConnection _con = new NpgsqlConnection(connectionString))
            {
                try
                {
                    string pgcommand = "update wfm_notification set emailstatus=@status where id=@id";
                    NpgsqlCommand cmd = new NpgsqlCommand();
                    cmd.Connection = _con;
                    _con.Open();
                    cmd.CommandType = CommandType.Text;
                    cmd.CommandText = pgcommand;
                    cmd.Parameters.AddWithValue("@status", status);
                    cmd.Parameters.AddWithValue("@id", id);
                    cmd.ExecuteNonQuery();

                    _con.Close();
                }
                catch (Exception ex)
                {
                    LogWriter.LogWrite("Error in updating Normal email status, Error msg: " + ex.Message);
                }

            }

        }
    }
}

