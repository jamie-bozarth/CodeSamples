using CNTRNHibernateLibrary;
using ICS_NHibernate;
using Jenzabar.Common;
using Jenzabar.Portal.Framework;
using Jenzabar.Portal.Framework.Facade;
using Jenzabar.Portal.Framework.NHibernateFWK;
using Jenzabar.Portal.Framework.Web.UI;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

namespace CNTRCovidForm.Views
{
    public partial class Default_View : PortletViewBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (IsFirstLoad)
            {
                List<CNTR_CovidForm_Rec> user_Daily_Forms = new JICSBaseFacade<CNTR_CovidForm_Rec>().GetQuery().Where(x => x.UserID == PortalUser.Current.ID && x.SubmittedDate.Date == DateTime.Today).ToList();
                if (user_Daily_Forms.Count == 0)
                {
                    InitializeForm();
                }
                else
                {
                    CNTR_CovidForm_Rec covid_Form_Submission = user_Daily_Forms[0];
                    SetLabels(covid_Form_Submission);
                    if (covid_Form_Submission.Symptomatic)
                    {
                        blocked();
                    }
                    else if (covid_Form_Submission.Quarantined)
                    {
                        quarantined();
                    }
                    else
                    {
                        cleared();
                    }
                }
            }
        }

        private void quarantined()
        {
            lbl_Clearance.Text = "Not Cleared For Campus";
            lbl_Clearance.CssClass = "text-danger";
            lbl_Symptoms.Text = @"Quarantine Reported
                                <br>
                                <br>
                                <ol>
                                    <li class='cntr-clearance'>
                                        Call 911 if you begin to experience severe symptoms: Trouble breathing, Persistent chest pain or pressure, Confusion, Bluish lips or face, An inability to wake up or stay awake
                                    </li>
                                    <li class='cntr-clearance'>
                                        Do not report to work or attend classes, meetings, events, or any other on-campus activity until the end of your quarantine period.
                                    </li>
                                    <li class='cntr-clearance'>
                                        Continue to follow guidance from the contact tracer.  Call your contact tracer or your healthcare provider if you have further questions.
                                    </li>
                                    <li class='cntr-clearance'>
                                        For further guidance on COVID-19, go to the CDC website
                                        <br><div class='cntr-center'><a href = 'https://www.cdc.gov/coronavirus/2019-ncov/if-you-are-sick/steps-when-sick.html' target = '_blank' rel ='noopener'>CDC Info</a>
                                        </ div>
                                        <br><br>
                                     </li>
                                   </ol>";

            completion_Display.Attributes["class"] = "container text-center bg-danger";
            main_Display.Visible = false;
            success_Display.Visible = false;
            completion_Display.Visible = true;
            div_Warning.Visible = false;
            getUserIMG();
            AppendReportingLinks();
        }

        private string GetUserName()
        {
            if (PortalUser.Current.PreferredName != null)
                return PortalUser.Current.LastName + ", " + PortalUser.Current.PreferredName;
            else
                return PortalUser.Current.LastName + ", " + PortalUser.Current.FirstName;
        }

        public void blocked()
        {
            lbl_Clearance.Text = "Not Cleared For Campus";
            lbl_Clearance.CssClass = "text-danger";
            lbl_Symptoms.Text = @"One or More Symptoms Reported
                                <br>
                                <br>
                                <ol>
                                    <li class='cntr-clearance'>
                                        Call 911 if you are experiencing severe symptoms: Trouble breathing, Persistent chest pain or pressure, Confusion, Bluish lips or face, An inability to wake up or stay awake
                                    </li>
                                    <li class='cntr-clearance'>
                                        Your symptoms may be related to COVID-19.
                                    </li>
                                    <li class='cntr-clearance'>
                                        Do not report to work or attend classes, meetings, events, or any other on-campus activity.
                                    </li>
                                    <li class='cntr-clearance'>
                                         " + ExtraNotes() + @"
                                    </li>
                                    <li class='cntr-clearance'>
                                        For further guidance on COVID-19, go to the CDC website
                                        <br><div class='cntr-center'><a href = 'https://www.cdc.gov/coronavirus/2019-ncov/if-you-are-sick/steps-when-sick.html' target = '_blank' rel ='noopener'>CDC Info</a>
                                        </ div>
                                        <br><br>
                                     </li>
                                   </ol>";

            completion_Display.Attributes["class"] = "container text-center bg-danger";
            main_Display.Visible = false;
            success_Display.Visible = false;
            completion_Display.Visible = true;
            div_Warning.Visible = false;
            getUserIMG();
            AppendReportingLinks();
        }

        private void AppendReportingLinks()
        {
            if (PortalUser.Current.IsMemberOf(PortalGroup.Staff) || PortalUser.Current.IsMemberOf(PortalGroup.Faculty))
            {
                Reporting_Row.Visible = true;
            }
            return;
        }

        private string ExtraNotes()
        {
            string return_String = " ";
            if (PortalUser.Current.IsMemberOf(PortalGroup.Students))
            {
                return_String = @"Call the Centre Student Health office at 859-238-5530 to schedule an appointment for testing and to discuss appropriate self-quarantine steps. 
                                    <br>You must be cleared by Student Health in order to resume campus activities.";
            }
            else if (PortalUser.Current.IsMemberOf(PortalGroup.Staff) || PortalUser.Current.IsMemberOf(PortalGroup.Faculty))
            {
                return_String = @"Contact your primary health care provider, a telehealth service, or seek a medical evaluation at a local facility. You should be tested for COVID-19 within the next 36 hours.
                                    <br>In Danville, you can be tested at Ephraim McDowell Walk-in & Primary Care 1541 Lebanon Road 859-936-8350; 
                                    <br>First Care 1591 Hustonville Road  859-724-3057; or Integrity Extended Healthcare 606-303-4389;
                                    <br>Self-quarantine at home while awaiting test results and as long as you are experiencing symptoms. ";
            }
            return return_String;
        }

        public void cleared()
        {
            lbl_Clearance.Text = "Cleared For Campus";
            lbl_Clearance.CssClass = "text-success";
            lbl_Symptoms.Text = "No Symptoms Reported";
            completion_Display.Attributes["class"] = "container text-center bg-success";
            main_Display.Visible = false;
            success_Display.Visible = false;
            completion_Display.Visible = true;
            div_Warning.Visible = false;
            getUserIMG();
            AppendReportingLinks();
        }

        public void InitializeForm()
        {
            success_Display.Visible = false;
            completion_Display.Visible = false;
            main_Display.Visible = true;
            div_Warning.Visible = false;
            Reporting_Row.Visible = false;
            hf_Temp.Value = "false";
            hf_Chills.Value = "false";
            hf_Cough.Value = "false";
            hf_Breath.Value = "false";
            hf_Fatigue.Value = "false";
            hf_MusclePain.Value = "false";
            hf_Headache.Value = "false";
            hf_SmellOrTaste.Value = "false";
            hf_Throat.Value = "false";
            hf_Congestion.Value = "false";
            hf_Nausea.Value = "false";
            hf_Diarrhea.Value = "false";
            hf_Quarantined.Value = "false";
            lbl_Name.Text = GetUserName();
            lbl_Date.Text = DateTime.Now.ToString("dddd, dd MMMM yyyy");
            if (PortalUser.Current.IsMemberOf(PortalGroup.Students))
            {
                cntr_Header.Text = "Everyone must take their temperature with a thermometer and complete this form every day.  If you are living on campus, please complete this form before leaving your residence.  If you are not living on campus, today please complete this form as soon as possible to ensure accurate reporting.";
            }
            else if (PortalUser.Current.IsMemberOf(PortalGroup.Staff) || PortalUser.Current.IsMemberOf(PortalGroup.Faculty))
            {
                cntr_Header.Text = "Everyone must take their temperature with a thermometer and complete this form every day. If you are coming to campus today, please complete this form before or as soon as you arrive on campus. If you are not coming to campus today, please complete this form as soon as possible to ensure accurate reporting.";
            }
        }
        public void getUserIMG()
        {
            ICS_Portal_User_Facade userFacade = new ICS_Portal_User_Facade();
            ICS_Portal_User attendee = userFacade.GetByID(PortalUser.Current.ID);
            int intAttendeeID = Int32.Parse(attendee.HostID.TrimStart('0'));
            SimpleProfileFacade profileFacade = new SimpleProfileFacade();
            SimpleProfile attendeeProfile = profileFacade.getProfileByID(intAttendeeID);
            string imageUrl;
            //Get the attendee's picture and make it an image
            byte[] pictureByteArray = attendeeProfile.Photo;
            if (pictureByteArray != null)
            {
                imageUrl = "data:image/jpg;base64," + Convert.ToBase64String(pictureByteArray);
                div_Image.InnerHtml = "<img src=" + imageUrl + ">";
            }
            else
            {
                imageUrl = "/ICS/icsfs/mm/logo_stacked_black.jpg?target=69535bc7-f944-433c-ae2c-54b00d4cfcc1";
                div_Image.InnerHtml = "<img src=" + imageUrl + " width='193' height='150'>";
            }
            return;
        }
        public void SetLabels(CNTR_CovidForm_Rec covid_Form_Submission)
        {
            lbl_completion_Date.Text = covid_Form_Submission.SubmittedDate.ToString("dddd, dd MMMM yyyy");
            lbl_completion_Name.Text = GetUserName();
        }
        public CNTR_CovidForm_Rec CreateSubmission()
        {
            CNTR_CovidForm_Rec covid_Form_Submission = new CNTR_CovidForm_Rec();
            try
            {
                covid_Form_Submission.SubmissionID = Guid.NewGuid();
                covid_Form_Submission.UserID = PortalUser.Current.ID;
                covid_Form_Submission.CentreID = PortalUser.Current.HostID.TrimStart('0');
                covid_Form_Submission.SubmittedDate = DateTime.Now;
                covid_Form_Submission.Temp = bool.Parse(hf_Temp.Value);
                covid_Form_Submission.Chills = bool.Parse(hf_Chills.Value);
                covid_Form_Submission.Cough = bool.Parse(hf_Cough.Value);
                covid_Form_Submission.ShortnessOfBreath = bool.Parse(hf_Breath.Value);
                covid_Form_Submission.Fatigue = bool.Parse(hf_Fatigue.Value);
                covid_Form_Submission.MusclePain = bool.Parse(hf_MusclePain.Value);
                covid_Form_Submission.Headache = bool.Parse(hf_Headache.Value);
                covid_Form_Submission.SmellOrTaste = bool.Parse(hf_SmellOrTaste.Value);
                covid_Form_Submission.SoreThroat = bool.Parse(hf_Throat.Value);
                covid_Form_Submission.Congestion = bool.Parse(hf_Congestion.Value);
                covid_Form_Submission.Nausea = bool.Parse(hf_Nausea.Value);
                covid_Form_Submission.Diarrhea = bool.Parse(hf_Diarrhea.Value);
                covid_Form_Submission.OnCampus = bool.Parse(hf_OnCampus.Value);
                covid_Form_Submission.Quarantined = bool.Parse(hf_Quarantined.Value);
                covid_Form_Submission.Symptomatic = CheckSubmission(covid_Form_Submission);
            }
            catch (Exception e)
            {
                EventLog.WriteEntry("ICSNET", "Error: " + e, EventLogEntryType.Information);
            }
            return covid_Form_Submission;

        }
        private bool CheckSubmission(CNTR_CovidForm_Rec covid_Form_Submission)
        {
            List<bool> form_Questions = new List<bool>() {
                //Possible Symptoms
                covid_Form_Submission.Temp,
                covid_Form_Submission.Chills,
                covid_Form_Submission.Cough,
                covid_Form_Submission.ShortnessOfBreath,
                covid_Form_Submission.Fatigue,
                covid_Form_Submission.MusclePain,
                covid_Form_Submission.Headache,
                covid_Form_Submission.SmellOrTaste,
                covid_Form_Submission.SoreThroat,
                covid_Form_Submission.Congestion,
                covid_Form_Submission.Nausea,
                covid_Form_Submission.Diarrhea
            };
            foreach (bool form_Question in form_Questions)
            {
                if (form_Question)
                    return true;
            }
            return false;
        }

        protected void Submit_Click(object sender, EventArgs e)
        {
            //EventLog.WriteEntry("ICSNET", "Clicked Submit", EventLogEntryType.Information);
            if (hf_Saveable.Value != "false")
            {
                CNTR_CovidForm_Rec covid_Form_Submission = CreateSubmission();
                CNTR_CovidForm_Rec_Facade covidForm_Rec_Facade = new CNTR_CovidForm_Rec_Facade();
                covidForm_Rec_Facade.Add(covid_Form_Submission);
                SetLabels(covid_Form_Submission);
                if (covid_Form_Submission.Symptomatic)
                {
                    blocked();
                }
                else if (covid_Form_Submission.Quarantined)
                {
                    quarantined();
                }
                else
                {
                    //EventLog.WriteEntry("ICSNET", "Cleared user", EventLogEntryType.Information);
                    cleared();
                }
            }
            else
            {
                div_Warning.Visible = true;
            }
        }

        protected void ResubmitButton_Click(object sender, EventArgs e)
        {
            CNTR_CovidForm_Rec_Facade covidForm_Rec_Facade = new CNTR_CovidForm_Rec_Facade();
            List<CNTR_CovidForm_Rec> user_Daily_Forms = new JICSBaseFacade<CNTR_CovidForm_Rec>().GetQuery().Where(x => x.UserID == PortalUser.Current.ID && x.SubmittedDate.Date == DateTime.Today).OrderBy(x => x.SubmittedDate).ToList();
            //EventLog.WriteEntry("ICSNET", "Made it to resubmit!", EventLogEntryType.Information);
            if (user_Daily_Forms.Count > 0)
            {
                //EventLog.WriteEntry("ICSNET", "About to Delete Submission:" + user_Daily_Forms[0].SubmissionID, EventLogEntryType.Information);
                covidForm_Rec_Facade.Remove(user_Daily_Forms[0]);
            }
            InitializeForm();
        }
    }
}