using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Freebie.Models;
using System.Data;
using Freebie.ViewModels;
using Freebie.Libs;
using System.Globalization;
using PagedList;

namespace Freebie.Areas.Backend.Controllers
{
    public class AdminConfigController : Controller
    {
        //
        // GET: /Backend/AdminConfig/
        //private EchoContext db = new EchoContext();

        [FreebieAuthorization(Url = "~/Backend/Authentication/Login", Type = "Backend")]
        public ActionResult BaseQuota()
        {
            using (var db = new EchoContext())
            {
                IEnumerable<Quota> quotas = db.Quotas.Where(x => x.Quota_Type_Cd.Equals("B"));
                return View(quotas.ToList());
            }     
        }

        [FreebieAuthorization(Url = "~/Backend/Authentication/Login", Type = "Backend")]
        public ActionResult UpdateBaseQuota()
        {
            using (var db = new EchoContext())
            {
                IEnumerable<Quota> quotas = db.Quotas.Where(x => x.Quota_Type_Cd.Equals("B"));
                return View(quotas.ToList());
            }
        }

        [FreebieAuthorization(Url = "~/Backend/Authentication/Login", Type = "Backend")]
        public ActionResult FreeTrialQuota()
        {
            using (var db = new EchoContext())
            {
                TrialQuota tq = new TrialQuota();

                var today = DateTime.Now.Date;
                AccountTrial today_acctt = db.AccountTrials.SingleOrDefault(x => x.Date.Equals(today));


                if (today_acctt == null)
                {
                    today_acctt = new AccountTrial();
                }

                AdminConfiguration ac = db.AdminConfigurations.SingleOrDefault();

                if (ac == null)
                {
                    ac = new AdminConfiguration();
                }

                tq.no_trial_used = today_acctt.No_Trial_Used;
                tq.no_trial_acc = today_acctt.No_Trial_Used_Acc;
                tq.trial_limit_total = ac.Trial_Limit_Total;
                tq.trial_dur_val = ac.Trial_Dur_Val;
                tq.trial_enable_flag = ac.Trial_Enable_Flag;

                return View(tq);
            }
            
        }

        [FreebieAuthorization(Url = "~/Backend/Authentication/Login", Type = "Backend")]
        [HttpPost]
        public ActionResult UpdateBaseQuota(string state)
        {
            using (var db = new EchoContext())
            {
                IEnumerable<Quota> quotas = db.Quotas.Where(x => x.Quota_Type_Cd.Equals("B"));
                string user_no = Session["User_No"].ToString();
                foreach (var item in quotas)
                {
                    string fv = Request.Form["Quota_Freq_Val_" + item.Quota_Cd];
                    string dv = Request.Form["Quota_Dur_Val_" + item.Quota_Cd];

                    try
                    {
                        item.Quota_Freq_Val = Convert.ToByte(fv);
                        item.Quota_Dur_Val = Convert.ToByte(dv);
                        item.Updated_By = user_no;
                        item.Updated_Dttm = DateTime.Now;
                        db.Entry(item).State = EntityState.Modified;
                    }
                    catch
                    {
                        return View(quotas);
                    }

                }
                db.SaveChanges();
                FreebieEvent.UserUpdateEvent(Permission.base_quota_page_id, "A04");
                return RedirectToAction("BaseQuota");
            }
            
        }

        [FreebieAuthorization(Url = "~/Backend/Authentication/Login", Type = "Backend")]
        public ActionResult UpdateFreeTrialQuota()
        {
            using (var db = new EchoContext())
            {
                TrialQuota tq = new TrialQuota();
                var today = DateTime.Now.Date;
                AccountTrial today_acctt = db.AccountTrials.SingleOrDefault(x => x.Date.Equals(today));

                if (today_acctt == null)
                {
                    today_acctt = new AccountTrial();
                }

                AdminConfiguration ac = db.AdminConfigurations.SingleOrDefault();

                if (ac == null)
                {
                    ac = new AdminConfiguration();
                }

                tq.no_trial_used = today_acctt.No_Trial_Used;
                tq.no_trial_acc = today_acctt.No_Trial_Used_Acc;
                tq.trial_limit_total = ac.Trial_Limit_Total;
                tq.trial_dur_val = ac.Trial_Dur_Val;
                tq.trial_enable_flag = ac.Trial_Enable_Flag;

                return View(tq);
            }
        }

        [FreebieAuthorization(Url = "~/Backend/Authentication/Login", Type = "Backend")]
        [HttpPost]
        public ActionResult UpdateFreeTrialQuota(TrialQuota tq)
        {
            using (var db = new EchoContext())
            {
                AdminConfiguration ac = db.AdminConfigurations.SingleOrDefault();
                string user_no = Session["User_No"].ToString();
                ac.Trial_Limit_Total = Convert.ToInt32(tq.trial_limit_total);
                ac.Trial_Dur_Val = Convert.ToInt32(tq.trial_dur_val);
                ac.Trial_Enable_Flag = Convert.ToBoolean(tq.trial_enable_flag);
                ac.Updated_By = user_no;
                ac.Updated_Dttm = DateTime.Now;

                db.Entry(ac).State = EntityState.Modified;
                db.SaveChanges();
                FreebieEvent.UserUpdateEvent(Permission.free_trial_page_id, "A04");
                return RedirectToAction("FreeTrialQuota");
            }       
        }

        [FreebieAuthorization(Url = "~/Backend/Authentication/Login", Type = "Backend")]
        public ActionResult ActivationLimit()
        {
            using (var db = new EchoContext())
            {
                ActivationLimit al = new ActivationLimit();

                var date = DateTime.Now.Date;

                AccountActivation aa = db.AccountActivations.Where(x => x.Date.Equals(date)).SingleOrDefault();

                if (aa == null)
                {
                    aa = new AccountActivation();
                }

                AdminConfiguration ac = db.AdminConfigurations.SingleOrDefault();

                if (ac == null)
                {
                    ac = new AdminConfiguration();
                }

                al.no_activation = aa.No_Activation;
                al.no_activation_pending = aa.No_Activation_Pending;
                al.no_activation_acc = aa.No_Activation_Acc;
                al.no_activation_limit_total = ac.No_Activation_Limit_Total;
                al.no_activation_limit_daily = ac.No_Activation_Limit_Daily;


                return View(al);
            
            }
            
        }

        [FreebieAuthorization(Url = "~/Backend/Authentication/Login", Type = "Backend")]
        public ActionResult UpdateActivationLimit()
        {
            using (var db = new EchoContext())
            {
                ActivationLimit al = new ActivationLimit();

                var date = DateTime.Now.Date;

                AccountActivation aa = db.AccountActivations.Where(x => x.Date.Equals(date)).SingleOrDefault();

                if (aa == null)
                {
                    aa = new AccountActivation();
                }

                AdminConfiguration ac = db.AdminConfigurations.SingleOrDefault();

                if (ac == null)
                {
                    ac = new AdminConfiguration();
                }

                al.no_activation = aa.No_Activation;
                al.no_activation_pending = aa.No_Activation_Pending;
                al.no_activation_acc = aa.No_Activation_Acc;
                al.no_activation_limit_total = ac.No_Activation_Limit_Total;
                al.no_activation_limit_daily = ac.No_Activation_Limit_Daily;


                return View(al);
            }
            
        }

        [FreebieAuthorization(Url = "~/Backend/Authentication/Login", Type = "Backend")]
        [HttpPost]
        public ActionResult UpdateActivationLimit(ActivationLimit al)
        {
            using (var db = new EchoContext())
            {
                AdminConfiguration ac = db.AdminConfigurations.SingleOrDefault();
                string user_no = Session["User_No"].ToString();
                ac.No_Activation_Limit_Total = Convert.ToInt32(al.no_activation_limit_total);
                ac.No_Activation_Limit_Daily = Convert.ToInt32(al.no_activation_limit_daily);
                ac.Updated_By = user_no;
                ac.Updated_Dttm = DateTime.Now;

                db.Entry(ac).State = EntityState.Modified;
                db.SaveChanges();
                FreebieEvent.UserUpdateEvent(Permission.activation_page_id, "A04");
                return RedirectToAction("ActivationLimit");
            }
            
        }

        [FreebieAuthorization(Url = "~/Backend/Authentication/Login", Type = "Backend")]
        public ActionResult SpecialQuotas(int? page)
        {
            using (var db = new EchoContext())
            {
                CultureInfo us = System.Globalization.CultureInfo.GetCultureInfo("en-US");
                DateTime today = DateTime.Now;
                today = new DateTime(today.Year, today.Month, today.Day, us.Calendar);
                DateTime From_Start_Dttm = new DateTime((today.Year), 1, 1);
                DateTime To_Start_Dttm = new DateTime((today.Year), 1, 1);
                DateTime From_End_Dttm = new DateTime((today.Year), 12, 31);
                DateTime To_End_Dttm = new DateTime((today.Year), 12, 31);

                IEnumerable<Quota> special_quotas = db.Quotas.Where(x => x.Quota_Type_Cd.Equals("S")).Where(x => !x.Status.Equals("Deleted")).Take(20).OrderByDescending(x => x.Created_Dttm);
                ViewBag.Name = "";
                ViewBag.From_Start_Date = From_Start_Dttm.ToString("dd/MM/yyyy", us);
                ViewBag.From_End_Date = From_End_Dttm.ToString("dd/MM/yyyy", us);
                ViewBag.To_Start_Date = To_Start_Dttm.ToString("dd/MM/yyyy", us);
                ViewBag.To_End_Date = To_End_Dttm.ToString("dd/MM/yyyy", us);
                ViewBag.Status_Cd = Statuses(null);
                var pageNumber = page ?? 1;
                special_quotas = special_quotas.ToList().ToPagedList(pageNumber, 20);
                ViewBag.PageQuotas = special_quotas;
                ViewBag.pageNumber = pageNumber;
                ViewBag.pageSize = 20;
                return View(special_quotas);
            }
           
        }

        [FreebieAuthorization(Url = "~/Backend/Authentication/Login", Type = "Backend")]
        [HttpPost]
        public ActionResult SpecialQuotas(int? page, string state)
        {
            string name = Request.Form["Quota_Desc"];
            string status_cd = Request.Form["Status_Cd"];
            DateTime today = DateTime.Now;
            CultureInfo us = System.Globalization.CultureInfo.GetCultureInfo("en-US");
            DateTime from_start_date = today;
            DateTime from_end_date = today;
            DateTime to_end_date = today;
            DateTime to_start_date = today;

            if (string.IsNullOrWhiteSpace(Request.Form["From_Start_Dttm"]))
            {
                from_start_date = new DateTime(today.Year, 1, 1, us.Calendar);
            }
            else
            {
                try
                {
                    from_start_date = DateTime.ParseExact(Request.Form["From_Start_Dttm"], @"d/M/yyyy", System.Globalization.CultureInfo.InvariantCulture);

                }
                catch (Exception)
                {
                    from_start_date = new DateTime((today.Year), 1, 1, us.Calendar);
                }

            }

            if (string.IsNullOrWhiteSpace(Request.Form["From_End_Dttm"]))
            {
                from_end_date = new DateTime((today.Year), 12, 31, us.Calendar);
            }
            else
            {
                try
                {
                    from_end_date = DateTime.ParseExact(Request.Form["From_End_Dttm"], @"d/M/yyyy", System.Globalization.CultureInfo.InvariantCulture);

                }
                catch (Exception)
                {
                    from_end_date = new DateTime((today.Year), 12, 31, us.Calendar);
                }

            }

            if (string.IsNullOrWhiteSpace(Request.Form["To_Start_Dttm"]))
            {
                to_start_date = new DateTime(today.Year, 1, 1, us.Calendar);
            }
            else
            {
                try
                {
                    to_start_date = DateTime.ParseExact(Request.Form["To_Start_Dttm"], @"d/M/yyyy", System.Globalization.CultureInfo.InvariantCulture);

                }
                catch (Exception)
                {
                    to_start_date = new DateTime((today.Year), 1, 1, us.Calendar);
                }

            }

            if (string.IsNullOrWhiteSpace(Request.Form["To_End_Dttm"]))
            {
                to_end_date = new DateTime((today.Year), 12, 31, us.Calendar);
            }
            else
            {
                try
                {
                    to_end_date = DateTime.ParseExact(Request.Form["To_End_Dttm"], @"d/M/yyyy", System.Globalization.CultureInfo.InvariantCulture);

                }
                catch (Exception)
                {
                    to_end_date = new DateTime((today.Year), 12, 31, us.Calendar);
                }

            }

            using (var db = new EchoContext())
            {
                IEnumerable<Quota> special_quotas = db.Quotas.Where(x => x.Quota_Type_Cd.Equals("S")).Where(x => !x.Status.Equals("Deleted")); //.Where(x => x.Quota_Type_Cd.Equals("S")).Where(x => x.Start_Dttm >= start_date).Where(x => x.End_Dttm <= end_date);

                special_quotas = special_quotas.Where(x => x.Start_Dttm >= from_start_date).Where(x => x.Start_Dttm <= from_end_date);
                special_quotas = special_quotas.Where(x => x.End_Dttm >= to_start_date).Where(x => x.End_Dttm <= to_end_date);

                if (!string.IsNullOrWhiteSpace(name))
                {
                    special_quotas = special_quotas.Where(x => (x.Quota_Desc.ToLower().Contains(name.ToLower())));
                }

                if (!string.IsNullOrWhiteSpace(status_cd))
                {
                    special_quotas = special_quotas.Where(x => x.Status.ToString().Trim().Equals(status_cd));
                }


                ViewBag.Name = name;
                ViewBag.From_Start_Date = from_start_date.ToString("dd/MM/yyyy", us);
                ViewBag.From_End_Date = from_end_date.ToString("dd/MM/yyyy", us);
                ViewBag.To_Start_Date = to_start_date.ToString("dd/MM/yyyy", us);
                ViewBag.To_End_Date = to_end_date.ToString("dd/MM/yyyy", us);
                ViewBag.Status_Cd = Statuses(Request.Form["Status_Cd"]);
                var pageNumber = page ?? 1;
                int count = (special_quotas.Count() > 0) ? special_quotas.Count() : 1;
                special_quotas = special_quotas.OrderByDescending(x => x.Created_Dttm).ToList().ToPagedList(pageNumber, count);
                ViewBag.PageQuotas = special_quotas;
                ViewBag.pageNumber = pageNumber;
                ViewBag.pageSize = special_quotas.Count();
                return View(special_quotas);
            }

        }

        [FreebieAuthorization(Url = "~/Backend/Authentication/Login", Type = "Backend")]
        public ActionResult NewSpecialQuota()
        {
            using (var db = new EchoContext())
            {
                Quota sp_quota = new Quota();
                return View(sp_quota);
            }
        }

        [FreebieAuthorization(Url = "~/Backend/Authentication/Login", Type = "Backend")]
        [HttpPost]
        public ActionResult NewSpecialQuota(Quota quota)
        {
            Quota sp_quota = new Quota();
            #region validations

                if (string.IsNullOrWhiteSpace(Request.Form["Quota_Desc"]))
                {
                    ModelState.AddModelError("Quota_Desc", System.Configuration.ConfigurationManager.AppSettings["SpecialQuota02"]);
                }

                if (string.IsNullOrWhiteSpace(Request.Form["Start_Dttm"]))
                {
                    ModelState.AddModelError("Start_Dttm", System.Configuration.ConfigurationManager.AppSettings["SpecialQuota03"]);
                }
                

                if (string.IsNullOrWhiteSpace(Request.Form["End_Dttm"]))
                {
                    ModelState.AddModelError("End_Dttm", System.Configuration.ConfigurationManager.AppSettings["SpecialQuota04"]);
                }

                if (!string.IsNullOrWhiteSpace(Request.Form["Start_Dttm"]) && !string.IsNullOrWhiteSpace(Request.Form["End_Dttm"]))
                {
                    try
                    {
                        DateTime today = DateTime.Now;

                        DateTime start_date = DateTime.ParseExact(Request.Form["Start_Dttm"], @"d/M/yyyy", System.Globalization.CultureInfo.InvariantCulture);
                        DateTime end_date = DateTime.ParseExact(Request.Form["End_Dttm"], @"d/M/yyyy", System.Globalization.CultureInfo.InvariantCulture);
                        if (end_date.Date < today.Date)
                        {
                            ModelState.AddModelError("End_Dttm", System.Configuration.ConfigurationManager.AppSettings["SpecialQuota10"]);
                        }
                        else
                        {
                            if (start_date > end_date)
                            {
                                ModelState.AddModelError("End_Dttm", System.Configuration.ConfigurationManager.AppSettings["SpecialQuota08"]);
                            }
                            else
                            {
                                ModelState.Remove("Start_Dttm");
                                ModelState.Remove("End_Dttm");
                            }
                        }
                    }
                    catch (Exception) {
                        ModelState.AddModelError("End_Dttm", System.Configuration.ConfigurationManager.AppSettings["SpecialQuota04"]);
                    }
                }

                if (string.IsNullOrWhiteSpace(Request.Form["Quota_Freq_Val"]))
                {
                    ModelState.AddModelError("Quota_Freq_Val", System.Configuration.ConfigurationManager.AppSettings["SpecialQuota05"]);
                }
                else {
                    if (Convert.ToInt16(Request.Form["Quota_Freq_Val"]) < 0) 
                    {
                        ModelState.AddModelError("Quota_Freq_Val", System.Configuration.ConfigurationManager.AppSettings["SpecialQuota07"]);
                    }
                }

                if (string.IsNullOrWhiteSpace(Request.Form["Quota_Dur_Val"]))
                {
                    ModelState.AddModelError("Quota_Dur_Val", System.Configuration.ConfigurationManager.AppSettings["SpecialQuota06"]);
                }
                else
                {
                    if (Convert.ToInt16(Request.Form["Quota_Dur_Val"]) < 0)
                    {
                        ModelState.AddModelError("Quota_Freq_Val", System.Configuration.ConfigurationManager.AppSettings["SpecialQuota07"]);
                    }
                }

            #endregion

                using (var db = new EchoContext())
                {
                    if (ModelState.IsValid)
                    {
                        try
                        {
                            DateTime today = DateTime.Now;

                            DateTime start_date = DateTime.ParseExact(Request.Form["Start_Dttm"], @"d/M/yyyy", System.Globalization.CultureInfo.InvariantCulture);
                            DateTime end_date = DateTime.ParseExact(Request.Form["End_Dttm"], @"d/M/yyyy", System.Globalization.CultureInfo.InvariantCulture);

                            IList<Quota> sequence = db.Quotas.ToList();
                            int length = sequence.Count;

                            string new_quota_cd = "Q" + (length + 1).ToString("D4");
                            sp_quota.Quota_Cd = new_quota_cd;
                            sp_quota.Quota_Type_Cd = "S";
                            sp_quota.Quota_Desc = quota.Quota_Desc;
                            sp_quota.Start_Dttm = start_date; 
                            sp_quota.End_Dttm = end_date;
                            sp_quota.Quota_Freq_Val = quota.Quota_Freq_Val;
                            sp_quota.Quota_Dur_Val = quota.Quota_Dur_Val;
                            sp_quota.Created_By = Session["User_No"].ToString();
                            sp_quota.Created_Dttm = today;
                            sp_quota.Updated_By = Session["User_No"].ToString();
                            sp_quota.Updated_Dttm = today;


                            if (today < sp_quota.Start_Dttm)
                            {
                                sp_quota.Status = FreebieStatus.QuotaDeployed();
                            }

                            if (today >= sp_quota.Start_Dttm && today <= sp_quota.End_Dttm)
                            {
                                sp_quota.Status = FreebieStatus.QuotaActive();
                            }

                            if (today > sp_quota.End_Dttm)
                            {
                                sp_quota.Status = FreebieStatus.QuotaEnded();
                            }


                            db.Quotas.Add(sp_quota);

                            db.SaveChanges();
                            return RedirectToAction("SpecialQuotas");
                        }
                        catch(Exception e)
                        {
                            ModelState.AddModelError("Quota_Cd", System.Configuration.ConfigurationManager.AppSettings["SpecialQuota09"]);
                        }

                       
                    }
                    else
                    {
                       
                    }

                    return View(sp_quota);
                }
        }

        [FreebieAuthorization(Url = "~/Backend/Authentication/Login", Type = "Backend")]
        public ActionResult ViewSpecialQuota(string quota_cd)
        {
            using (var db = new EchoContext())
            {
                Quota sp_quota = db.Quotas.SingleOrDefault(x => x.Quota_Cd.Equals(quota_cd));
                if (sp_quota.Start_Dttm != null)
                {
                    DateTime start_date = Convert.ToDateTime(sp_quota.Start_Dttm);
                    sp_quota.Start_Dttm = Convert.ToDateTime((start_date.Year) + "-" + start_date.Month + "-" + start_date.Day);
                }

                if (sp_quota.End_Dttm != null)
                {
                    DateTime end_date = Convert.ToDateTime(sp_quota.End_Dttm);
                    sp_quota.End_Dttm = Convert.ToDateTime((end_date.Year) + "-" + end_date.Month + "-" + end_date.Day);
                }


                #region Statuses
                    List<SelectListItem> statuses = new List<SelectListItem>();

                    var default_status = new SelectListItem()
                    {
                        Text = sp_quota.Status,
                        Value = sp_quota.Status,
                        Selected = true
                    };
                    statuses.Add(default_status);
                    // deployed
                    if (sp_quota.Status.Equals(FreebieStatus.QuotaDeployed()))
                    {
                        var canceled_status = new SelectListItem()
                        {
                            Text = FreebieStatus.QuotaCanceled(),
                            Value = FreebieStatus.QuotaCanceled(),
                            Selected = false
                        };
                        statuses.Add(canceled_status);
                    }

                    // active
                    if (sp_quota.Status.Equals(FreebieStatus.QuotaActive()))
                    {
                        var paused_status = new SelectListItem()
                        {
                            Text = FreebieStatus.QuotaPaused(),
                            Value = FreebieStatus.QuotaPaused(),
                            Selected = false
                        };
                        statuses.Add(paused_status);

                        var canceled_status = new SelectListItem()
                        {
                            Text = FreebieStatus.QuotaCanceled(),
                            Value = FreebieStatus.QuotaCanceled(),
                            Selected = false
                        };
                        statuses.Add(canceled_status);
                    }

                    //  paused
                    if (sp_quota.Status.Equals(FreebieStatus.QuotaPaused()))
                    {
                        var active_status = new SelectListItem()
                        {
                            Text = FreebieStatus.QuotaActive(),
                            Value = FreebieStatus.QuotaActive(),
                            Selected = false
                        };
                        statuses.Add(active_status);

                        var canceled_status = new SelectListItem()
                        {
                            Text = FreebieStatus.QuotaCanceled(),
                            Value = FreebieStatus.QuotaCanceled(),
                            Selected = false
                        };
                        statuses.Add(canceled_status);
                    }

                    ViewBag.Status_Cd = statuses;
                #endregion

                return View(sp_quota);
            }
        }

        [FreebieAuthorization(Url = "~/Backend/Authentication/Login", Type = "Backend")]
        public ActionResult EditSpecialQuota(string edit_quota_cd)
        {
            using (var db = new EchoContext())
            {
                Quota sp_quota = db.Quotas.SingleOrDefault(x => x.Quota_Cd.Equals(edit_quota_cd));
                //DateTime start_date = DateTime.ParseExact(Request.Form["Start_Dttm"], @"d/M/yyyy", System.Globalization.CultureInfo.InvariantCulture);
                //DateTime end_date = DateTime.ParseExact(Request.Form["End_Dttm"], @"d/M/yyyy", System.Globalization.CultureInfo.InvariantCulture);
                if (sp_quota.Start_Dttm != null)
                {
                    DateTime start_date = Convert.ToDateTime(sp_quota.Start_Dttm);
                    sp_quota.Start_Dttm = Convert.ToDateTime((start_date.Year) + "-" + start_date.Month + "-" + start_date.Day);
                }

                if (sp_quota.End_Dttm != null)
                {
                    DateTime end_date = Convert.ToDateTime(sp_quota.End_Dttm);
                    sp_quota.End_Dttm = Convert.ToDateTime((end_date.Year) + "-" + end_date.Month + "-" + end_date.Day);
                }
                

                #region Statuses
                    List<SelectListItem> statuses = new List<SelectListItem>();

                    var default_status = new SelectListItem()
                    {
                        Text = sp_quota.Status,
                        Value = sp_quota.Status,
                        Selected = true
                    };
                    statuses.Add(default_status);
                    // deployed
                    if (sp_quota.Status.Equals(FreebieStatus.QuotaDeployed()))
                    {
                        var canceled_status = new SelectListItem()
                        {
                            Text = FreebieStatus.QuotaCanceled(),
                            Value = FreebieStatus.QuotaCanceled(),
                            Selected = false
                        };
                        statuses.Add(canceled_status);
                    }

                    // active
                    if (sp_quota.Status.Equals(FreebieStatus.QuotaActive()))
                    {
                        var paused_status = new SelectListItem()
                        {
                            Text = FreebieStatus.QuotaPaused(),
                            Value = FreebieStatus.QuotaPaused(),
                            Selected = false
                        };
                        statuses.Add(paused_status);

                        var canceled_status = new SelectListItem()
                        {
                            Text = FreebieStatus.QuotaCanceled(),
                            Value = FreebieStatus.QuotaCanceled(),
                            Selected = false
                        };
                        statuses.Add(canceled_status);
                    }

                    //  paused
                    if (sp_quota.Status.Equals(FreebieStatus.QuotaPaused()))
                    {
                        var active_status = new SelectListItem()
                        {
                            Text = FreebieStatus.QuotaActive(),
                            Value = FreebieStatus.QuotaActive(),
                            Selected = false
                        };
                        statuses.Add(active_status);

                        var canceled_status = new SelectListItem()
                        {
                            Text = FreebieStatus.QuotaCanceled(),
                            Value = FreebieStatus.QuotaCanceled(),
                            Selected = false
                        };
                        statuses.Add(canceled_status);
                    }

                    ViewBag.Status_Cd = statuses;
                #endregion
                
                return View(sp_quota);
            }
        
        }


        [FreebieAuthorization(Url = "~/Backend/Authentication/Login", Type = "Backend")]
        [HttpPost]
        public ActionResult EditSpecialQuota(Quota quota)
        {
            #region validations
                //if (string.IsNullOrWhiteSpace(Request.Form["Quota_Cd"]))
                //{
                //    ModelState.AddModelError("Quota_Cd", System.Configuration.ConfigurationManager.AppSettings["SpecialQuota01"]);
                //}

                if (string.IsNullOrWhiteSpace(Request.Form["Quota_Desc"]))
                {
                    ModelState.AddModelError("Quota_Desc", System.Configuration.ConfigurationManager.AppSettings["SpecialQuota02"]);
                }

                if (string.IsNullOrWhiteSpace(Request.Form["Start_Dttm"]))
                {
                    ModelState.AddModelError("Start_Dttm", System.Configuration.ConfigurationManager.AppSettings["SpecialQuota03"]);
                }


                if (string.IsNullOrWhiteSpace(Request.Form["End_Dttm"]))
                {
                    ModelState.AddModelError("End_Dttm", System.Configuration.ConfigurationManager.AppSettings["SpecialQuota04"]);
                }

                if (!string.IsNullOrWhiteSpace(Request.Form["Start_Dttm"]) && !string.IsNullOrWhiteSpace(Request.Form["End_Dttm"]))
                {
                    try
                    {
                        DateTime today = DateTime.Now;
                        DateTime start_date = DateTime.ParseExact(Request.Form["Start_Dttm"], @"d/M/yyyy", System.Globalization.CultureInfo.InvariantCulture);
                        DateTime end_date = DateTime.ParseExact(Request.Form["End_Dttm"], @"d/M/yyyy", System.Globalization.CultureInfo.InvariantCulture);

                        if (end_date.Date < today.Date)
                        {
                            ModelState.AddModelError("End_Dttm", System.Configuration.ConfigurationManager.AppSettings["SpecialQuota10"]);
                        }
                        else
                        {
                            if (start_date > end_date)
                            {
                                ModelState.AddModelError("End_Dttm", System.Configuration.ConfigurationManager.AppSettings["SpecialQuota08"]);
                            }
                            else
                            {
                                ModelState.Remove("Start_Dttm");
                                ModelState.Remove("End_Dttm");
                            }
                        }
                    }
                    catch (Exception)
                    {
                        ModelState.AddModelError("End_Dttm", System.Configuration.ConfigurationManager.AppSettings["SpecialQuota04"]);
                    }
                }

                if (string.IsNullOrWhiteSpace(Request.Form["Quota_Freq_Val"]))
                {
                    ModelState.AddModelError("Quota_Freq_Val", System.Configuration.ConfigurationManager.AppSettings["SpecialQuota05"]);
                }
                else
                {
                    if (Convert.ToInt16(Request.Form["Quota_Freq_Val"]) < 0)
                    {
                        ModelState.AddModelError("Quota_Freq_Val", System.Configuration.ConfigurationManager.AppSettings["SpecialQuota07"]);
                    }
                }

                if (string.IsNullOrWhiteSpace(Request.Form["Quota_Dur_Val"]))
                {
                    ModelState.AddModelError("Quota_Dur_Val", System.Configuration.ConfigurationManager.AppSettings["SpecialQuota06"]);
                }
                else
                {
                    if (Convert.ToInt16(Request.Form["Quota_Dur_Val"]) < 0)
                    {
                        ModelState.AddModelError("Quota_Freq_Val", System.Configuration.ConfigurationManager.AppSettings["SpecialQuota07"]);
                    }
                }

            #endregion
            using (var db = new EchoContext())
            {
                string quota_cd = Request.Form["Edited_Quota_Cd"].ToString();
                Quota sp_quota = db.Quotas.SingleOrDefault(x => x.Quota_Cd.Equals(quota_cd));
                if (ModelState.IsValid)
                {
                    try
                    {
                        DateTime today = DateTime.Now;
                        //CultureInfo th = System.Globalization.CultureInfo.GetCultureInfo("th-TH");
                        DateTime start_date = DateTime.ParseExact(Request.Form["Start_Dttm"], @"d/M/yyyy", System.Globalization.CultureInfo.InvariantCulture);
                        DateTime end_date = DateTime.ParseExact(Request.Form["End_Dttm"], @"d/M/yyyy", System.Globalization.CultureInfo.InvariantCulture);

                        sp_quota.Quota_Type_Cd = "S";
                        sp_quota.Quota_Desc = quota.Quota_Desc;
                        sp_quota.Start_Dttm = start_date; 
                        sp_quota.End_Dttm = end_date; 
                        sp_quota.Quota_Freq_Val = quota.Quota_Freq_Val;
                        sp_quota.Quota_Dur_Val = quota.Quota_Dur_Val;
                        sp_quota.Updated_By = Session["User_No"].ToString();
                        sp_quota.Updated_Dttm = today;
                        sp_quota.Status = Request.Form["Status_Cd"];



                        db.Entry(sp_quota).State = EntityState.Modified;

                        db.SaveChanges();
                        return RedirectToAction("SpecialQuotas");
                    }
                    catch (Exception)
                    {
                        ModelState.AddModelError("Quota_Cd", System.Configuration.ConfigurationManager.AppSettings["SpecialQuota09"]);
                    }
                }


                #region Statuses
                List<SelectListItem> statuses = new List<SelectListItem>();

                var default_status = new SelectListItem()
                {
                    Text = sp_quota.Status,
                    Value = sp_quota.Status,
                    Selected = true
                };
                statuses.Add(default_status);
                // deployed
                if (sp_quota.Status.Equals(FreebieStatus.QuotaDeployed()))
                {
                    var canceled_status = new SelectListItem()
                    {
                        Text = FreebieStatus.QuotaCanceled(),
                        Value = FreebieStatus.QuotaCanceled(),
                        Selected = false
                    };
                    statuses.Add(canceled_status);
                }

                // active
                if (sp_quota.Status.Equals(FreebieStatus.QuotaActive()))
                {
                    var paused_status = new SelectListItem()
                    {
                        Text = FreebieStatus.QuotaPaused(),
                        Value = FreebieStatus.QuotaPaused(),
                        Selected = false
                    };
                    statuses.Add(paused_status);

                    var canceled_status = new SelectListItem()
                    {
                        Text = FreebieStatus.QuotaCanceled(),
                        Value = FreebieStatus.QuotaCanceled(),
                        Selected = false
                    };
                    statuses.Add(canceled_status);
                }

                //  paused
                if (sp_quota.Status.Equals(FreebieStatus.QuotaPaused()))
                {
                    var active_status = new SelectListItem()
                    {
                        Text = FreebieStatus.QuotaActive(),
                        Value = FreebieStatus.QuotaActive(),
                        Selected = false
                    };
                    statuses.Add(active_status);

                    var canceled_status = new SelectListItem()
                    {
                        Text = FreebieStatus.QuotaCanceled(),
                        Value = FreebieStatus.QuotaCanceled(),
                        Selected = false
                    };
                    statuses.Add(canceled_status);
                }

                ViewBag.Status_Cd = statuses;
                #endregion

                return View(sp_quota);
            }
        }

        [FreebieAuthorization(Url = "~/Backend/Authentication/Login", Type = "Backend")]
        public ActionResult RemoveSpecialQuota(string quota_cd)
        {
            string role_cd = (Session["Role"] == null) ? "" : Session["Role"].ToString();
            bool has_permission = role_cd.Equals("AM");

            if (has_permission)
            {
                using (var db = new EchoContext())
                {
                    Quota remove_quota = db.Quotas.SingleOrDefault(x => x.Quota_Cd.Equals(quota_cd));
                    if (remove_quota != null)
                    {
                        remove_quota.Status = FreebieStatus.QuotaDeleted();
                        db.Entry(remove_quota).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                }
            }
            return RedirectToAction("SpecialQuotas");
        }

        [FreebieAuthorization(Url = "~/Backend/Authentication/Login", Type = "Backend")]
        public ActionResult PromotionCodes(int? page)
        { 
            using (var db = new EchoContext())
            {
                CultureInfo us = System.Globalization.CultureInfo.GetCultureInfo("en-US");
                DateTime today = DateTime.Now;
                today = new DateTime(today.Year, today.Month, today.Day, us.Calendar);
                DateTime From_Start_Dttm = new DateTime((today.Year), 1, 1);
                DateTime To_Start_Dttm = new DateTime((today.Year), 1, 1);
                DateTime From_End_Dttm = new DateTime((today.Year), 12, 31);
                DateTime To_End_Dttm = new DateTime((today.Year), 12, 31);

                IEnumerable<PromotionCode> promotions = db.PromotionCodes.Where(x => !x.Status.Equals("Deleted")).Take(20).OrderByDescending(x => x.Created_Dttm);
                ViewBag.Name = "";
                ViewBag.From_Start_Date = From_Start_Dttm.ToString("dd/MM/yyyy", us);
                ViewBag.From_End_Date = From_End_Dttm.ToString("dd/MM/yyyy", us);
                ViewBag.To_Start_Date = To_Start_Dttm.ToString("dd/MM/yyyy", us);
                ViewBag.To_End_Date = To_End_Dttm.ToString("dd/MM/yyyy", us);
                ViewBag.Status_Cd = Statuses(null);
                var pageNumber = page ?? 1;
                promotions = promotions.ToList().ToPagedList(pageNumber, 20);
                ViewBag.PagePromotions = promotions;
                ViewBag.pageNumber = pageNumber;
                ViewBag.pageSize = 20;
                return View(promotions);
            }
        }

        [FreebieAuthorization(Url = "~/Backend/Authentication/Login", Type = "Backend")]
        [HttpPost]
        public ActionResult PromotionCodes(int? page, string state)
        {
            string name = Request.Form["Promotion_Code_Value"];
            string status_cd = Request.Form["Status_Cd"];
            CultureInfo th = System.Globalization.CultureInfo.GetCultureInfo("th-TH");
            CultureInfo us = System.Globalization.CultureInfo.GetCultureInfo("en-US");
            DateTime today = DateTime.Now;
            DateTime from_start_date = today;
            DateTime from_end_date = today;
            DateTime to_end_date = today;
            DateTime to_start_date = today;

            if (string.IsNullOrWhiteSpace(Request.Form["From_Start_Dttm"]))
            {
                from_start_date = new DateTime(today.Year, 1, 1, us.Calendar);
            }
            else
            {
                try
                {
                    from_start_date = DateTime.ParseExact(Request.Form["From_Start_Dttm"], @"d/M/yyyy", System.Globalization.CultureInfo.InvariantCulture);

                }
                catch (Exception)
                {
                    from_start_date = new DateTime((today.Year), 1, 1, us.Calendar);
                }

            }

            if (string.IsNullOrWhiteSpace(Request.Form["From_End_Dttm"]))
            {
                from_end_date = new DateTime((today.Year), 12, 31, us.Calendar);
            }
            else
            {
                try
                {
                    from_end_date = DateTime.ParseExact(Request.Form["From_End_Dttm"], @"d/M/yyyy", System.Globalization.CultureInfo.InvariantCulture);

                }
                catch (Exception)
                {
                    from_end_date = new DateTime((today.Year), 12, 31, us.Calendar);
                }

            }

            if (string.IsNullOrWhiteSpace(Request.Form["To_Start_Dttm"]))
            {
                to_start_date = new DateTime(today.Year, 1, 1, us.Calendar);
            }
            else
            {
                try
                {
                    to_start_date = DateTime.ParseExact(Request.Form["To_Start_Dttm"], @"d/M/yyyy", System.Globalization.CultureInfo.InvariantCulture);

                }
                catch (Exception)
                {
                    to_start_date = new DateTime((today.Year), 1, 1, us.Calendar);
                }

            }

            if (string.IsNullOrWhiteSpace(Request.Form["To_End_Dttm"]))
            {
                to_end_date = new DateTime((today.Year), 12, 31, us.Calendar);
            }
            else
            {
                try
                {
                    to_end_date = DateTime.ParseExact(Request.Form["To_End_Dttm"], @"d/M/yyyy", System.Globalization.CultureInfo.InvariantCulture);

                }
                catch (Exception)
                {
                    to_end_date = new DateTime((today.Year), 12, 31, us.Calendar);
                }

            }

            using (var db = new EchoContext())
            {
                IEnumerable<PromotionCode> promotions = db.PromotionCodes.Where(x => !x.Status.Equals("Deleted")).OrderByDescending(x => x.Created_Dttm); //.Where(x => x.Start_Dttm >= start_date).Where(x => x.End_Dttm <= end_date);

                promotions = promotions.Where(x => x.Start_Dttm >= from_start_date).Where(x => x.Start_Dttm <= from_end_date);
                promotions = promotions.Where(x => x.End_Dttm >= to_start_date).Where(x => x.End_Dttm <= to_end_date);

                if (!string.IsNullOrWhiteSpace(name))
                {
                    promotions = promotions.Where(x => (x.Promotion_Code_Value.ToLower().Contains(name.ToLower())));
                }

                if (!string.IsNullOrWhiteSpace(status_cd))
                {
                    promotions = promotions.Where(x => x.Status.ToString().Trim().Equals(status_cd));
                }


                ViewBag.Name = name;
                ViewBag.From_Start_Date = from_start_date.ToString("dd/MM/yyyy", us);
                ViewBag.From_End_Date = from_end_date.ToString("dd/MM/yyyy", us);
                ViewBag.To_Start_Date = to_start_date.ToString("dd/MM/yyyy", us);
                ViewBag.To_End_Date = to_end_date.ToString("dd/MM/yyyy", us);

                ViewBag.Status_Cd = Statuses(Request.Form["Status_Cd"]);
                var pageNumber = page ?? 1;
                int count = (promotions.Count() > 0) ? promotions.Count() : 1;
                promotions = promotions.OrderByDescending(x => x.Created_Dttm).ToList().ToPagedList(pageNumber, count);
                ViewBag.PagePromotions = promotions;
                ViewBag.pageNumber = pageNumber;
                ViewBag.pageSize = promotions.Count();
                return View(promotions);
            }

        }

        [FreebieAuthorization(Url = "~/Backend/Authentication/Login", Type = "Backend")]
        public ActionResult NewPromotionCode()
        {
            using (var db = new EchoContext())
            {
                PromotionCode promotion = new PromotionCode();

                #region SpecialQuotas
                    List<SelectListItem> special_quotas = new List<SelectListItem>();

                    var blank_item = new SelectListItem()
                    {
                        Text = "-",
                        Value = "",
                        Selected = true
                    };
                    special_quotas.Add(blank_item);
                    var tmp_status_arrs = new string[3];
                    tmp_status_arrs[0] = "Active";
                    tmp_status_arrs[1] = "Deployed";
                    tmp_status_arrs[2] = "Paused";
                    IEnumerable<Quota> active_items = db.Quotas.Where(x => x.Quota_Type_Cd.Equals("S")).Where(x => tmp_status_arrs.Contains(x.Status)).OrderBy(x => x.Quota_Cd);

                    foreach (var active_item in active_items.ToList())
                    {
                        var item = new SelectListItem()
                        {
                            Text = active_item.Quota_Cd,
                            Value = active_item.Quota_Cd,
                            Selected = false
                        };
                        special_quotas.Add(item);
                    }
                    ViewBag.Special_Quota_Cd = special_quotas;
                #endregion
                
                return View(promotion);
            }
        }

        [FreebieAuthorization(Url = "~/Backend/Authentication/Login", Type = "Backend")]
        [HttpPost]
        public ActionResult NewPromotionCode(PromotionCode promotion)
        {
            PromotionCode new_promotion = new PromotionCode();
            #region validations
                if (string.IsNullOrWhiteSpace(Request.Form["Promotion_Code_Value"]))
                {
                    ModelState.AddModelError("Promotion_Code_Value", System.Configuration.ConfigurationManager.AppSettings["PromotionCode01"]);
                }

                if (string.IsNullOrWhiteSpace(Request.Form["Promotion_Desc"]))
                {
                    ModelState.AddModelError("Promotion_Desc", System.Configuration.ConfigurationManager.AppSettings["PromotionCode02"]);
                }

                if (string.IsNullOrWhiteSpace(Request.Form["Start_Dttm"]))
                {
                    ModelState.AddModelError("Start_Dttm", System.Configuration.ConfigurationManager.AppSettings["PromotionCode03"]);
                }


                if (string.IsNullOrWhiteSpace(Request.Form["End_Dttm"]))
                {
                    ModelState.AddModelError("End_Dttm", System.Configuration.ConfigurationManager.AppSettings["PromotionCode04"]);
                }

                if (!string.IsNullOrWhiteSpace(Request.Form["Start_Dttm"]) && !string.IsNullOrWhiteSpace(Request.Form["End_Dttm"]))
                {
                    try
                    {
                        DateTime today = DateTime.Now;
                        DateTime start_date = DateTime.ParseExact(Request.Form["Start_Dttm"], @"d/M/yyyy", System.Globalization.CultureInfo.InvariantCulture);
                        DateTime end_date = DateTime.ParseExact(Request.Form["End_Dttm"], @"d/M/yyyy", System.Globalization.CultureInfo.InvariantCulture);

                        if (end_date.Date < today.Date)
                        {
                            ModelState.AddModelError("End_Dttm", System.Configuration.ConfigurationManager.AppSettings["PromotionCode09"]);
                        }
                        else
                        {
                            if (start_date > end_date)
                            {
                                ModelState.AddModelError("End_Dttm", System.Configuration.ConfigurationManager.AppSettings["SpecialQuota08"]);
                            }
                            else
                            {
                                ModelState.Remove("Start_Dttm");
                                ModelState.Remove("End_Dttm");
                            }
                        }
                    }
                    catch (Exception)
                    {
                        ModelState.AddModelError("End_Dttm", System.Configuration.ConfigurationManager.AppSettings["PromotionCode04"]);
                    }
                }

                if (string.IsNullOrWhiteSpace(Request.Form["Online_Message"]))
                {
                    ModelState.AddModelError("Online_Message", System.Configuration.ConfigurationManager.AppSettings["PromotionCode06"]);
                }
            #endregion

            using (var db = new EchoContext())
            {
                if (ModelState.IsValid)
                {
                    try {
                        DateTime today = DateTime.Now;
                        DateTime start_date = DateTime.ParseExact(Request.Form["Start_Dttm"], @"d/M/yyyy", System.Globalization.CultureInfo.InvariantCulture);
                        DateTime end_date = DateTime.ParseExact(Request.Form["End_Dttm"], @"d/M/yyyy", System.Globalization.CultureInfo.InvariantCulture);
                       
                        IList<PromotionCode> sequence = db.PromotionCodes.ToList();
                        var length = sequence.Count;
                        string promotion_cd = "PM" + (length + 1).ToString("D4");
                       
                        new_promotion.Promotion_Cd = promotion_cd;
                        new_promotion.Promotion_Code_Value = promotion.Promotion_Code_Value;
                        new_promotion.No_of_Target = promotion.No_of_Target;
                        new_promotion.Promotion_Desc = promotion.Promotion_Desc;
                        new_promotion.Special_Quota_Cd = promotion.Special_Quota_Cd;
                        new_promotion.Online_Message = promotion.Online_Message;
                        new_promotion.Start_Dttm = start_date;
                        new_promotion.End_Dttm = end_date;
                        new_promotion.Created_By = Session["User_No"].ToString();
                        new_promotion.Created_Dttm = today;
                        new_promotion.Updated_By = Session["User_No"].ToString();
                        new_promotion.Updated_Dttm = today;


                        if (today < new_promotion.Start_Dttm)
                        {
                            new_promotion.Status = FreebieStatus.QuotaDeployed();
                        }

                        if (today >= new_promotion.Start_Dttm && today <= new_promotion.End_Dttm)
                        {
                            new_promotion.Status = FreebieStatus.QuotaActive();
                        }

                        if (today > new_promotion.End_Dttm)
                        {
                            new_promotion.Status = FreebieStatus.QuotaEnded();
                        }


                        db.PromotionCodes.Add(new_promotion);

                        db.SaveChanges();
                        return RedirectToAction("PromotionCodes");
                    }
                    catch (Exception err)
                    {
                        ModelState.AddModelError("Promotion_Cd", System.Configuration.ConfigurationManager.AppSettings["PromotionCode07"]);
                        FreebieEvent.AddError(err, Permission.promotion_code_page_id);
                    }
                }

                #region SpecialQuotas
                    List<SelectListItem> special_quotas = new List<SelectListItem>();

                    var blank_item = new SelectListItem()
                    {
                        Text = "-",
                        Value = "",
                        Selected = true
                    };
                    special_quotas.Add(blank_item);

                    var tmp_status_arrs = new string[3];
                    tmp_status_arrs[0] = "Active";
                    tmp_status_arrs[1] = "Deployed";
                    tmp_status_arrs[2] = "Paused";
                    IEnumerable<Quota> active_items = db.Quotas.Where(x => x.Quota_Type_Cd.Equals("S")).Where(x => tmp_status_arrs.Contains(x.Status)).OrderBy(x => x.Quota_Cd);

                    foreach (var active_item in active_items.ToList())
                    {
                        var item = new SelectListItem()
                        {
                            Text = active_item.Quota_Cd,
                            Value = active_item.Quota_Cd,
                            Selected = false
                        };
                        special_quotas.Add(item);
                    }
                    ViewBag.Special_Quota_Cd = special_quotas;
                #endregion
                return View(new_promotion);
            }
        }

        [FreebieAuthorization(Url = "~/Backend/Authentication/Login", Type = "Backend")]
        public ActionResult ViewPromotionCode(string promotion_cd)
        {
            using (var db = new EchoContext())
            {
                PromotionCode promotion = db.PromotionCodes.SingleOrDefault(x => x.Promotion_Cd.Equals(promotion_cd));

                if (promotion.Start_Dttm != null)
                {
                    DateTime start_date = Convert.ToDateTime(promotion.Start_Dttm);
                    promotion.Start_Dttm = Convert.ToDateTime((start_date.Year) + "-" + start_date.Month + "-" + start_date.Day);
                }

                if (promotion.End_Dttm != null)
                {
                    DateTime end_date = Convert.ToDateTime(promotion.End_Dttm);
                    promotion.End_Dttm = Convert.ToDateTime((end_date.Year) + "-" + end_date.Month + "-" + end_date.Day);
                }

                #region Statuses
                    List<SelectListItem> statuses = new List<SelectListItem>();

                    var default_status = new SelectListItem()
                    {
                        Text = promotion.Status,
                        Value = promotion.Status,
                        Selected = true
                    };
                    statuses.Add(default_status);
                    // deployed
                    if (promotion.Status.Equals(FreebieStatus.QuotaDeployed()))
                    {
                        var canceled_status = new SelectListItem()
                        {
                            Text = FreebieStatus.QuotaCanceled(),
                            Value = FreebieStatus.QuotaCanceled(),
                            Selected = false
                        };
                        statuses.Add(canceled_status);
                    }

                    // active
                    if (promotion.Status.Equals(FreebieStatus.QuotaActive()))
                    {
                        var paused_status = new SelectListItem()
                        {
                            Text = FreebieStatus.QuotaPaused(),
                            Value = FreebieStatus.QuotaPaused(),
                            Selected = false
                        };
                        statuses.Add(paused_status);

                        var canceled_status = new SelectListItem()
                        {
                            Text = FreebieStatus.QuotaCanceled(),
                            Value = FreebieStatus.QuotaCanceled(),
                            Selected = false
                        };
                        statuses.Add(canceled_status);
                    }

                    //  paused
                    if (promotion.Status.Equals(FreebieStatus.QuotaPaused()))
                    {
                        var active_status = new SelectListItem()
                        {
                            Text = FreebieStatus.QuotaActive(),
                            Value = FreebieStatus.QuotaActive(),
                            Selected = false
                        };
                        statuses.Add(active_status);

                        var canceled_status = new SelectListItem()
                        {
                            Text = FreebieStatus.QuotaCanceled(),
                            Value = FreebieStatus.QuotaCanceled(),
                            Selected = false
                        };
                        statuses.Add(canceled_status);
                    }

                    ViewBag.Status_Cd = statuses;
                #endregion

                #region SpecialQuotas
                    List<SelectListItem> special_quotas = new List<SelectListItem>();

                    var blank_item = new SelectListItem()
                    {
                        Text = "-",
                        Value = "",
                        Selected = string.IsNullOrWhiteSpace(promotion.Special_Quota_Cd)
                    };
                    special_quotas.Add(blank_item);

                    var tmp_status_arrs = new string[3];
                    tmp_status_arrs[0] = "Active";
                    tmp_status_arrs[1] = "Deployed";
                    tmp_status_arrs[2] = "Paused";
                    IEnumerable<Quota> active_items = db.Quotas.Where(x => x.Quota_Type_Cd.Equals("S")).Where(x => tmp_status_arrs.Contains(x.Status)).OrderBy(x => x.Quota_Cd);

                    foreach (var active_item in active_items.ToList())
                    {
                        var item = new SelectListItem()
                        {
                            Text = active_item.Quota_Cd,
                            Value = active_item.Quota_Cd,
                            Selected = string.IsNullOrWhiteSpace(promotion.Special_Quota_Cd) ? false : promotion.Special_Quota_Cd.Equals(active_item.Quota_Cd)
                        };
                        special_quotas.Add(item);
                    }
                    ViewBag.Special_Quota_Cd = special_quotas;
                #endregion

                return View(promotion);
            }
        }

        [FreebieAuthorization(Url = "~/Backend/Authentication/Login", Type = "Backend")]
        public ActionResult EditPromotionCode(string edit_promotion_cd)
        {
            using (var db = new EchoContext())
            {
                PromotionCode promotion = db.PromotionCodes.SingleOrDefault(x => x.Promotion_Cd.Equals(edit_promotion_cd));

                if (promotion.Start_Dttm != null)
                {
                    DateTime start_date = Convert.ToDateTime(promotion.Start_Dttm);
                    promotion.Start_Dttm = Convert.ToDateTime((start_date.Year) + "-" + start_date.Month + "-" + start_date.Day);
                }

                if (promotion.End_Dttm != null)
                {
                    DateTime end_date = Convert.ToDateTime(promotion.End_Dttm);
                    promotion.End_Dttm = Convert.ToDateTime((end_date.Year) + "-" + end_date.Month + "-" + end_date.Day);
                }


                #region Statuses
                List<SelectListItem> statuses = new List<SelectListItem>();

                    var default_status = new SelectListItem()
                    {
                        Text = promotion.Status,
                        Value = promotion.Status,
                        Selected = true
                    };
                    statuses.Add(default_status);
                    // deployed
                    if (promotion.Status.Equals(FreebieStatus.QuotaDeployed()))
                    {
                        var canceled_status = new SelectListItem()
                        {
                            Text = FreebieStatus.QuotaCanceled(),
                            Value = FreebieStatus.QuotaCanceled(),
                            Selected = false
                        };
                        statuses.Add(canceled_status);
                    }

                    // active
                    if (promotion.Status.Equals(FreebieStatus.QuotaActive()))
                    {
                        var paused_status = new SelectListItem()
                        {
                            Text = FreebieStatus.QuotaPaused(),
                            Value = FreebieStatus.QuotaPaused(),
                            Selected = false
                        };
                        statuses.Add(paused_status);

                        var canceled_status = new SelectListItem()
                        {
                            Text = FreebieStatus.QuotaCanceled(),
                            Value = FreebieStatus.QuotaCanceled(),
                            Selected = false
                        };
                        statuses.Add(canceled_status);
                    }

                    //  paused
                    if (promotion.Status.Equals(FreebieStatus.QuotaPaused()))
                    {
                        var active_status = new SelectListItem()
                        {
                            Text = FreebieStatus.QuotaActive(),
                            Value = FreebieStatus.QuotaActive(),
                            Selected = false
                        };
                        statuses.Add(active_status);

                        var canceled_status = new SelectListItem()
                        {
                            Text = FreebieStatus.QuotaCanceled(),
                            Value = FreebieStatus.QuotaCanceled(),
                            Selected = false
                        };
                        statuses.Add(canceled_status);
                    }

                    ViewBag.Status_Cd = statuses;
                #endregion

                #region SpecialQuotas
                    List<SelectListItem> special_quotas = new List<SelectListItem>();

                    var blank_item = new SelectListItem()
                    {
                        Text = "-",
                        Value = "",
                        Selected = string.IsNullOrWhiteSpace(promotion.Special_Quota_Cd)
                    };
                    special_quotas.Add(blank_item);

                    var tmp_status_arrs = new string[3];
                    tmp_status_arrs[0] = "Active";
                    tmp_status_arrs[1] = "Deployed";
                    tmp_status_arrs[2] = "Paused";
                    IEnumerable<Quota> active_items = db.Quotas.Where(x => x.Quota_Type_Cd.Equals("S")).Where(x => tmp_status_arrs.Contains(x.Status)).OrderBy(x => x.Quota_Cd);

                    foreach (var active_item in active_items.ToList())
                    {
                        var item = new SelectListItem()
                        {
                            Text = active_item.Quota_Cd,
                            Value = active_item.Quota_Cd,
                            Selected = string.IsNullOrWhiteSpace(promotion.Special_Quota_Cd) ? false : promotion.Special_Quota_Cd.Equals(active_item.Quota_Cd)
                        };
                        special_quotas.Add(item);
                    }
                    ViewBag.Special_Quota_Cd = special_quotas;
                #endregion

                return View(promotion);
            }

        }

        [FreebieAuthorization(Url = "~/Backend/Authentication/Login", Type = "Backend")]
        [HttpPost]
        public ActionResult EditPromotionCode(PromotionCode promotion)
        {
            #region validations
                if (string.IsNullOrWhiteSpace(Request.Form["Promotion_Code_Value"]))
                {
                    ModelState.AddModelError("Promotion_Code_Value", System.Configuration.ConfigurationManager.AppSettings["PromotionCode01"]);
                }

                if (string.IsNullOrWhiteSpace(Request.Form["Promotion_Desc"]))
                {
                    ModelState.AddModelError("Promotion_Desc", System.Configuration.ConfigurationManager.AppSettings["PromotionCode02"]);
                }

                if (string.IsNullOrWhiteSpace(Request.Form["Start_Dttm"]))
                {
                    ModelState.AddModelError("Start_Dttm", System.Configuration.ConfigurationManager.AppSettings["PromotionCode03"]);
                }


                if (string.IsNullOrWhiteSpace(Request.Form["End_Dttm"]))
                {
                    ModelState.AddModelError("End_Dttm", System.Configuration.ConfigurationManager.AppSettings["PromotionCode04"]);
                }

                if (!string.IsNullOrWhiteSpace(Request.Form["Start_Dttm"]) && !string.IsNullOrWhiteSpace(Request.Form["End_Dttm"]))
                {
                    try
                    {
                        DateTime today = DateTime.Now;
                        DateTime start_date = DateTime.ParseExact(Request.Form["Start_Dttm"], @"d/M/yyyy", System.Globalization.CultureInfo.InvariantCulture);
                        DateTime end_date = DateTime.ParseExact(Request.Form["End_Dttm"], @"d/M/yyyy", System.Globalization.CultureInfo.InvariantCulture);

                        if (end_date.Date < today.Date)
                        {
                            ModelState.AddModelError("End_Dttm", System.Configuration.ConfigurationManager.AppSettings["PromotionCode09"]);
                        }
                        else
                        {
                            if (start_date > end_date)
                            {
                                ModelState.AddModelError("End_Dttm", System.Configuration.ConfigurationManager.AppSettings["SpecialQuota08"]);
                            }
                            else
                            {
                                ModelState.Remove("Start_Dttm");
                                ModelState.Remove("End_Dttm");
                            }
                        }
                    }
                    catch (Exception)
                    {
                        ModelState.AddModelError("End_Dttm", System.Configuration.ConfigurationManager.AppSettings["PromotionCode04"]);
                    }
                }

                if (string.IsNullOrWhiteSpace(Request.Form["Online_Message"]))
                {
                    ModelState.AddModelError("Online_Message", System.Configuration.ConfigurationManager.AppSettings["PromotionCode06"]);
                }
            #endregion

            using (var db = new EchoContext())
            {
                string promotion_cd = Request.Form["Edited_Promotion_Cd"].ToString();
                PromotionCode edit_promotion = db.PromotionCodes.SingleOrDefault(x => x.Promotion_Cd.Equals(promotion_cd));
                if (ModelState.IsValid)
                {
                    try
                    {
                        DateTime today = DateTime.Now;
                        DateTime start_date = DateTime.ParseExact(Request.Form["Start_Dttm"], @"d/M/yyyy", System.Globalization.CultureInfo.InvariantCulture);
                        DateTime end_date = DateTime.ParseExact(Request.Form["End_Dttm"], @"d/M/yyyy", System.Globalization.CultureInfo.InvariantCulture);
                        
                       
                        edit_promotion.Promotion_Code_Value = promotion.Promotion_Code_Value;
                        edit_promotion.No_of_Target = promotion.No_of_Target;
                        edit_promotion.Promotion_Desc = promotion.Promotion_Desc;
                        edit_promotion.Special_Quota_Cd = promotion.Special_Quota_Cd;
                        edit_promotion.Online_Message = promotion.Online_Message;
                        edit_promotion.Start_Dttm = start_date;
                        edit_promotion.End_Dttm = end_date;
                        edit_promotion.Status = Request.Form["Status_Cd"];
                        edit_promotion.Updated_By = Session["User_No"].ToString();
                        edit_promotion.Updated_Dttm = today;

                        db.Entry(edit_promotion).State = EntityState.Modified;

                        db.SaveChanges();
                        return RedirectToAction("PromotionCodes");
                    }
                    catch (Exception)
                    {
                        ModelState.AddModelError("Promotion_Cd", System.Configuration.ConfigurationManager.AppSettings["PromotionCode07"]);
                    }
                }


                #region Statuses
                    List<SelectListItem> statuses = new List<SelectListItem>();

                    var default_status = new SelectListItem()
                    {
                        Text = edit_promotion.Status,
                        Value = edit_promotion.Status,
                        Selected = true
                    };
                    statuses.Add(default_status);
                    // deployed
                    if (edit_promotion.Status.Equals(FreebieStatus.QuotaDeployed()))
                    {
                        var canceled_status = new SelectListItem()
                        {
                            Text = FreebieStatus.QuotaCanceled(),
                            Value = FreebieStatus.QuotaCanceled(),
                            Selected = false
                        };
                        statuses.Add(canceled_status);
                    }

                    // active
                    if (edit_promotion.Status.Equals(FreebieStatus.QuotaActive()))
                    {
                        var paused_status = new SelectListItem()
                        {
                            Text = FreebieStatus.QuotaPaused(),
                            Value = FreebieStatus.QuotaPaused(),
                            Selected = false
                        };
                        statuses.Add(paused_status);

                        var canceled_status = new SelectListItem()
                        {
                            Text = FreebieStatus.QuotaCanceled(),
                            Value = FreebieStatus.QuotaCanceled(),
                            Selected = false
                        };
                        statuses.Add(canceled_status);
                    }

                    //  paused
                    if (edit_promotion.Status.Equals(FreebieStatus.QuotaPaused()))
                    {
                        var active_status = new SelectListItem()
                        {
                            Text = FreebieStatus.QuotaActive(),
                            Value = FreebieStatus.QuotaActive(),
                            Selected = false
                        };
                        statuses.Add(active_status);

                        var canceled_status = new SelectListItem()
                        {
                            Text = FreebieStatus.QuotaCanceled(),
                            Value = FreebieStatus.QuotaCanceled(),
                            Selected = false
                        };
                        statuses.Add(canceled_status);
                    }

                    ViewBag.Status_Cd = statuses;
                #endregion

                #region SpecialQuotas
                    List<SelectListItem> special_quotas = new List<SelectListItem>();

                    var blank_item = new SelectListItem()
                    {
                        Text = "-",
                        Value = "",
                        Selected = string.IsNullOrWhiteSpace(promotion.Special_Quota_Cd)
                    };
                    special_quotas.Add(blank_item);

                    var tmp_status_arrs = new string[3];
                    tmp_status_arrs[0] = "Active";
                    tmp_status_arrs[1] = "Deployed";
                    tmp_status_arrs[2] = "Paused";
                    IEnumerable<Quota> active_items = db.Quotas.Where(x => x.Quota_Type_Cd.Equals("S")).Where(x => tmp_status_arrs.Contains(x.Status)).OrderBy(x => x.Quota_Cd);

                    foreach (var active_item in active_items.ToList())
                    {
                        var item = new SelectListItem()
                        {
                            Text = active_item.Quota_Cd,
                            Value = active_item.Quota_Cd,
                            Selected = string.IsNullOrWhiteSpace(promotion.Special_Quota_Cd) ? false : promotion.Special_Quota_Cd.Equals(active_item.Quota_Cd)
                        };
                        special_quotas.Add(item);
                    }
                    ViewBag.Special_Quota_Cd = special_quotas;
                #endregion

                return View(edit_promotion);
            }
        }

        [FreebieAuthorization(Url = "~/Backend/Authentication/Login", Type = "Backend")]
        public ActionResult RemovePromotionCode(string promotion_cd)
        {
            string role_cd = (Session["Role"] == null) ? "" : Session["Role"].ToString();
            bool has_permission = role_cd.Equals("AM");

            if (has_permission)
            {
                using (var db = new EchoContext())
                {
                    PromotionCode remove_promotion = db.PromotionCodes.SingleOrDefault(x => x.Promotion_Cd.Equals(promotion_cd));
                    if (remove_promotion != null)
                    {
                        //db.PromotionCodes.Remove(remove_promotion);
                        remove_promotion.Status = FreebieStatus.QuotaDeleted();
                        db.Entry(remove_promotion).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                }
            }
            return RedirectToAction("PromotionCodes");
        }

        private List<SelectListItem> Statuses(string selected_item)
        {
            if (string.IsNullOrWhiteSpace(selected_item))
            {
                selected_item = "";
            }
            List<SelectListItem> statuses = new List<SelectListItem>();
            string deployed_str = FreebieStatus.QuotaDeployed();
            string active_str = FreebieStatus.QuotaActive();
            string paused_str = FreebieStatus.QuotaPaused();
            string canceled_str = FreebieStatus.QuotaCanceled();
            string ended_str = FreebieStatus.QuotaEnded();
            string deleted_str = FreebieStatus.QuotaDeleted();

            var default_item = new SelectListItem()
            {
                Text = "-",
                Value = "",
                Selected = selected_item.Equals("")
            };
            var deployed_item = new SelectListItem()
            {
                Text = FreebieStatus.QuotaDeployed(),
                Value = FreebieStatus.QuotaDeployed(),
                Selected = selected_item.Equals(deployed_str)
            };
            var active_item = new SelectListItem()
            {
                Text = FreebieStatus.QuotaActive(),
                Value = FreebieStatus.QuotaActive(),
                Selected = selected_item.Equals(active_str)
            };
            var paused_item = new SelectListItem()
            {
                Text = FreebieStatus.QuotaPaused(),
                Value = FreebieStatus.QuotaPaused(),
                Selected = selected_item.Equals(paused_str)
            };
            var canceled_item = new SelectListItem()
            {
                Text = FreebieStatus.QuotaCanceled(),
                Value = FreebieStatus.QuotaCanceled(),
                Selected = selected_item.Equals(canceled_str)
            };
            var ended_item = new SelectListItem()
            {
                Text = FreebieStatus.QuotaEnded(),
                Value = FreebieStatus.QuotaEnded(),
                Selected = selected_item.Equals(ended_str)
            };

            //var deleted_item = new SelectListItem()
            //{
            //    Text = FreebieStatus.QuotaDeleted(),
            //    Value = FreebieStatus.QuotaDeleted(),
            //    Selected = selected_item.Equals(deleted_str)
            //};

            statuses.Add(default_item);
            statuses.Add(deployed_item);
            statuses.Add(active_item);
            statuses.Add(paused_item);
            statuses.Add(canceled_item);
            statuses.Add(ended_item);
            //statuses.Add(deleted_item);
            return statuses;
        }
    }
}
