using CNTRDesignObjects.Iter_ables;
using CNTRDesignObjects.Object_ables.Forms;
using CNTRNHibernateLibrary;
using ICS_NHibernate;
using Jenzabar.Portal.Framework.NHibernateFWK;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CNTRDesignObjects.Object_ables.Flows
{
    [Serializable]

    public class Study_Abroad_Flow : Flow_Form_able
    {
        public Study_Abroad_Flow() {
            setFormable(new Study_Abroad_Form());
            Flow_ID = new Guid("DA166616-5796-4795-B0AF-4ED844937ECA");
            Name = "Study Abroad Flow";
            Index = new List<string> { "Parrent_Flow", "Flow_ID" };
            Index_Value = new List<object> { new Guid(), Flow_ID };
            facade = new Flow_Rec();
            
            
        }
        public override void SetSubmission(JCF_Submission jcf_Submission)
        {
            
            submission = jcf_Submission;
            double score = 0;
            ICS_Portal_User_Facade user_Facade = new ICS_Portal_User_Facade();
            ICS_Portal_User user = (ICS_Portal_User)user_Facade.GetByIdWhere(submission.UserID, "ID")[0];
            List<Involvement> involvements = new InvolvementFacade().getAllInvolvementsByStudentID(System.Convert.ToInt32(user.HostID));
            StuAcad_Facade stuAcad_Facade = new StuAcad_Facade();
            List<Abstract_Iter_able> stu_acad_records = stuAcad_Facade.GetByIdWhere(Int32.Parse(user.HostID), "id");
            string cum_gpa = "Error, No GPA Found";
            float pass_Hours = 0 / 1;
            string abroad_Status = "";
            StuStatRecord stuStatRecord = (StuStatRecord)(new StuStatRecord_Facade().GetByIdWhere(Int32.Parse(user.HostID), "ID").First());

            if (stuStatRecord != null)
            {
                cum_gpa = stuStatRecord.CumulativeGPA.ToString();
                score += ((stuStatRecord.CumulativeGPA / 4) * 5);
            }


            //foreach (StuAcad record in stu_acad_records)
            //{
            //    if (record.cum_pass_hrs > pass_Hours)
            //    {
            //        cum_gpa = record.cum_gpa.ToString();
            //        pass_Hours = record.cum_pass_hrs;
            //    }
            //}

            ProgEnrRecFacade progEnrRecFacade = new ProgEnrRecFacade();
            ProgEnrRec progEnrRec = progEnrRecFacade.GetById(Int32.Parse(user.HostID.TrimStart('0')));
            foreach (Involvement involvement in involvements)
            {
                if (involvement.InvlCode.EdiInvl == "OCS")
                {
                    if (abroad_Status != "")
                        abroad_Status += ", ";
                    abroad_Status += involvement.InvlCode.Descr;
                }
            }

            string StuClassification = "";
            switch (progEnrRec.Class)
            {
                
                case "FR":
                    StuClassification = "Sophmore";
                    score += 2;
                    break;
                case "SO":
                    StuClassification = "Junior";
                    score += 3;
                    break;
                case "JR":
                    StuClassification = "Senior";
                    score += 4;
                    if (abroad_Status != "")
                        score++;
                    break;
                case "SR":
                    StuClassification = "Senior";
                    score += 4;
                    if (abroad_Status != "")
                        score++;
                    break;
            }


            if (abroad_Status == "")
                abroad_Status = "Student has not studied abroad before.";
            

            single_Header_Items = new Dictionary<string, object>(){
                {"Student ID",user.FirstName+" "+user.LastName+" | "+user.Email},
                {"Program Preference 1:", ((JCF_Answer)new JCF_Answer_Facade().GetByIdWhere(new List<object>(){submission.SubmissionID, new Guid("3DD8962A-805D-4972-91BB-11E834E2506C") },new List<string>(){"SubmissionID","ItemID"})[0]).AnswerValue},
                {"Program Preference 2:",((JCF_Answer)new JCF_Answer_Facade().GetByIdWhere(new List<object>(){submission.SubmissionID, new Guid("8D988A93-0192-4CC2-A8BD-A9FC5F91F9F2") },new List<string>(){"SubmissionID","ItemID"})[0]).AnswerValue},
                {"Program Preference 3:",((JCF_Answer)new JCF_Answer_Facade().GetByIdWhere(new List<object>(){submission.SubmissionID, new Guid("5F121850-3B2A-44AA-8D9B-FD9BC21818AC") },new List<string>(){"SubmissionID","ItemID"})[0]).AnswerValue},
                {"Study Abroad Status:", abroad_Status},
                {"Classification:", StuClassification},
                {"Student GPA:", cum_gpa }
            };
            single_Footer_Items = new Dictionary<string, object>(){
                {"Student ID",user.FirstName+" "+user.LastName+" | "+user.Email},
                {"Program Preference 1:", ((JCF_Answer)new JCF_Answer_Facade().GetByIdWhere(new List<object>(){submission.SubmissionID, new Guid("3DD8962A-805D-4972-91BB-11E834E2506C") },new List<string>(){"SubmissionID","ItemID"})[0]).AnswerValue},
                {"Program Preference 2:",((JCF_Answer)new JCF_Answer_Facade().GetByIdWhere(new List<object>(){submission.SubmissionID, new Guid("8D988A93-0192-4CC2-A8BD-A9FC5F91F9F2") },new List<string>(){"SubmissionID","ItemID"})[0]).AnswerValue},
                {"Program Preference 3:",((JCF_Answer)new JCF_Answer_Facade().GetByIdWhere(new List<object>(){submission.SubmissionID, new Guid("5F121850-3B2A-44AA-8D9B-FD9BC21818AC") },new List<string>(){"SubmissionID","ItemID"})[0]).AnswerValue},
                {"Study Abroad Status:", abroad_Status},
                {"Classification:", StuClassification},
                {"Student GPA:", cum_gpa }
            };
        }

    }
}
