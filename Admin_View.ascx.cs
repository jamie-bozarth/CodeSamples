using System;
using System.Collections.Generic;
using System.Web.UI.WebControls;
using Jenzabar.Portal.Framework;
using System.Web.UI;
using System.Data;
using System.Diagnostics;
using CNTRDesignObjects.Viewables;
using System.Linq;
using CNTRDesignObjects.Facades;
using CNTRDesignObjects.Helpers;
using CNTRDesignObjects.Iter_ables;
using CNTRDesignObjects.Factory;
using Jenzabar.Portal.Framework.NHibernateFWK;
using CNTRDesignObjects;
using CNTRDesignObjects.TEMP_Objects;
using Jenzabar.Portal.Framework.Web.UI;
using CNTRNHibernateLibrary;
using CNTRDesignObjects.Helpers.HTML_Generator;
using System.Web.UI.HtmlControls;
using ICS_NHibernate;
using System.IO;
using CNTRDesignObjects.Interfaces.CNTR_able_Interfaces;
//using CNTRDesignObjects.Dictionary;
using CNTRDesignObjects.Object_ables;
using CNTRDesignObjects.Object_ables.Forms;
using CNTRDesignObjects.Object_ables.Flows;
using CNTRDesignObjects.Dictionary;
using System.Web;
using System.Text;
using CNTRViewPortlet.lib;
using System.Web.Services;
using MigraDoc.Rendering;
using System.Text.RegularExpressions;
using MigraDoc.DocumentObjectModel;
using CNTRCovidForm.lib;
using MigraDoc.DocumentObjectModel.Shapes;
using System.Drawing;

namespace CNTRCovidForm.Views

{
    public partial class Admin_View : PortletViewBase
    {
        public DataTable dt;
        public CNTR_Viewable current_Viewable;

        public List<Form_able> master_Forms_List_Admin = new List<Form_able>() { new Covid_Formable()};
        public List<Form_able> master_Forms_List;
        protected int current_index;
        protected int range;

        protected void Page_Load(object sender, EventArgs e)
        {


            HttpContext.Current.Session["data_Columns"] = new List<string>();
            List<string> listable_List = new List<string>();
            int userId = Convert.ToInt32(PortalUser.Current.HostID.TrimStart('0'));
            master_Forms_List = master_Forms_List_Admin;
            
            Html_Generator generator = new Html_Generator();
            TEMP_HeaderFacade headerFacade = new TEMP_HeaderFacade();
            Panel header = generator.getHeader(headerFacade.getHeaders(), new Dictionary<string, object>());
            //div_Listable_header.Controls.Add(header);
            if (IsFirstLoad)
            {
                
                hf_Selected_Object.Value = "0";
                hf_Range.Value = "50";
                hf_Current_Index.Value = "0";
                current_index = Convert.ToInt32(hf_Current_Index.Value);
                range = Convert.ToInt32(hf_Range.Value);
                createDataTabs();
            }
            else
            {
                hf_Selected_Object.Value = listableList.SelectedItem.Value;
                current_index = Convert.ToInt32(hf_Current_Index.Value);
                range = Convert.ToInt32(hf_Range.Value);

            }
            currentPortlet.Text = master_Forms_List[Convert.ToInt32(hf_Selected_Object.Value)].Name + " Submissions | " + hf_Selected_Object.Value;
            LoadHeader(master_Forms_List[Convert.ToInt32(hf_Selected_Object.Value)]);
            div_Listable_header_Title.Text = master_Forms_List[Convert.ToInt32(hf_Selected_Object.Value)].Name;
            var flow_Object = master_Forms_List[Convert.ToInt32(hf_Selected_Object.Value)] as Flow_Form_able;
            if (flow_Object != null)
            {
                //List<object> flowId = new List<object> { flow_Object.Flow_ID };
                LoadFlow(master_Forms_List[Convert.ToInt32(hf_Selected_Object.Value)], master_Forms_List[Convert.ToInt32(hf_Selected_Object.Value)].Index_Value, master_Forms_List[Convert.ToInt32(hf_Selected_Object.Value)].Index);
            }
            else
                LoadForm(master_Forms_List[Convert.ToInt32(hf_Selected_Object.Value)].facade, master_Forms_List[Convert.ToInt32(hf_Selected_Object.Value)].Index_Value, master_Forms_List[Convert.ToInt32(hf_Selected_Object.Value)].Index, master_Forms_List[Convert.ToInt32(hf_Selected_Object.Value)]);
            View_Singleton single = View_Singleton.getInstance();
            single.setView(this);
            HttpContext.Current.Session["current_Viewable"] = current_Viewable;
            HttpContext.Current.Session["current_Listable"] = current_Viewable.returnListable();
            HttpContext.Current.Session["master_Forms_List"] = master_Forms_List;
            HttpContext.Current.Session["hidden_field"] = Convert.ToInt32(hf_Selected_Object.Value);

        }


        public void ChangeView(Object sender, EventArgs e)
        {
            Guid submission_Guid = new Guid(hf_Submission.Value);
            CNTR_CovidForm_Rec flow = new JICSBaseFacade<CNTR_CovidForm_Rec>().GetQuery().Where(x => x.SubmissionID == submission_Guid).SingleOrDefault();
            int data = Convert.ToInt32(hf_Selected_Object.Value);
            switch (hf_isButtonClicked.Value)
            {
                case "edit":
                    Session["viewDesired"] = "edit";
                    Session["form_Obj"] = flow;
                    Session["formable"] = master_Forms_List[data];
                    Session["viewableType"] = master_Forms_List[data].Name;
                    ParentPortlet.NextScreen("Objectable_View");
                    break;

                case "review":
                    Session["viewDesired"] = "edit";
                    
                    Session["form_Obj"] = flow;
                    Session["formable"] = master_Forms_List[data];
                    Session["viewableType"] = master_Forms_List[data].Name;

                    ParentPortlet.NextScreen("Objectable_View");
                    break;

                case "delete":
                    new CNTR_CovidForm_Rec_Facade().Remove(flow);
                    break;

                case "print":
                    List_able listObj = current_Viewable.returnListable();
                    Abstract_Iter_able form_obj = listObj.get_iterable_List()[data];
                    EventLog.WriteEntry("ICSNET", "ListObject:" + listObj.getType(), EventLogEntryType.Information);
                    switch (listObj.getType())
                    {
                        case "ICS_NHibernate.JCF_Submission":
                            JCF_Submission obj = (JCF_Submission)listObj.get_iterable_List()[data];
                            this.print_Form(obj);
                            break;
                        case "ICS_NHibernate.Flow_Rec":
                            Flow_Rec flow_Obj = (Flow_Rec)listObj.get_iterable_List()[data];
                            JCF_Submission submission_Object = new JICSBaseFacade<JCF_Submission>().GetQuery().Where(x => x.SubmissionID == flow_Obj.Submission_ID).SingleOrDefault();
                            if (submission_Object != null)
                            {
                                this.print_Form(submission_Object);
                            }
                            break;
                        case "ICS_NHibernate.CNTR_CovidForm_Rec":
                            this.print_Form(flow);
                            break;
                        default:
                            break;
                    }

                    break;

            }

        }

        private void LoadHeader(Form_able form)
        {
            Dictionary<string, object> header_Items = form.list_Header_Items;
            Html_Generator htmlG = new Html_Generator();
            string html = "";
            foreach (KeyValuePair<string, object> header in header_Items)
            {
                html += "<div class=\"row\"><div class=\"col-lg-12\"><label id=\"variable_Label_" + header.Key + "\" class=\"col-lg-6 control-label\">" + header.Key + "</label>";
                html += "<label id = \"variable_WebControl_" + Guid.NewGuid() + "\" class=\"col-lg-6\"> " + header.Value + "</label>";
                html += "</div>";
                html += "</div>";
            }
           

            div_Listable_header.Controls.Add(new LiteralControl(html));
        }

        private void LoadFlow(Form_able abstract_Iter_able, List<object> value, List<string> index)
        {
            IQuery_able facade = abstract_Iter_able.facade.getFacade();
            current_Viewable = new CNTR_Viewable();
            List<string> key_List = abstract_Iter_able.list_Column_Headers;
            List_able list_Able = new List_able();           
            List<Abstract_Iter_able> object_List = facade.GetByIdWhere(value, index);
            string iterable_Title = abstract_Iter_able.GetType().ToString();
            list_Able.setListableState(iterable_Title, object_List);
            current_Viewable.setViewState(list_Able, iterable_Title, key_List);
            string key_String = "";
            int j = 0;
            foreach (string key in key_List)
            {
                key_String += "Key" + j + "= " + key + " | ";
                j++;
            }
        }

        private void LoadForm(Abstract_Iter_able abstract_Iter_able, List<object> value, List<string> index, Form_able form)
        {
            IQuery_able facade = abstract_Iter_able.getFacade();
            current_Viewable = new CNTR_Viewable();
            List<string> key_List = abstract_Iter_able.getListOfKeys();
            List<Guid> aggregate_Indexes;
            List_able list_Able = new List_able();
            if (form == null)
            {
                aggregate_Indexes = new List<Guid> { };

            }
            else
            {
                aggregate_Indexes = form.list_View_Items;
            }
            List<Abstract_Iter_able> object_List;
            object_List = facade.GetAll();

            string iterable_Title = abstract_Iter_able.GetType().ToString();
            list_Able.setListableState(iterable_Title, object_List.Cast<Abstract_Iter_able>().ToList());
            current_Viewable.setViewState(list_Able, iterable_Title, key_List);

            string key_String = "";
            int j = 0;
            foreach (string key in key_List)
            {
                key_String += "Key" + j + "= " + key + " | ";
                j++;
            }
        }

        private void Add_New_List(List<Abstract_Iter_able> list)
        {
            Facade_Table_Facade facade_Table_Facade = new Facade_Table_Facade();
            List<Abstract_Iter_able> return_List = new List<Abstract_Iter_able>();
            List<Abstract_Iter_able> table = facade_Table_Facade.JB_GetAll();
            foreach (Abstract_Iter_able item in list)
            {
                List<Abstract_Iter_able> existing_List = facade_Table_Facade.GetByIdWhere(item.GetType().ToString(), "Facade_Name");
                if (existing_List.Count() == 0)
                {
                    return_List.Add(item);
                }
            }
            Update_List(table);
            Add_List(return_List);
            return;
        }
        
        private List<string> Update_List(List<Abstract_Iter_able> abstract_List)
        {
            List<string> return_List = new List<string>();
            Facade_Table_Facade facade_Facade = new Facade_Table_Facade();
            foreach (Abstract_Iter_able jB_Abstract in abstract_List)
            {
                List<Abstract_Iter_able> existing_List = facade_Facade.GetByIdWhere(jB_Abstract.GetType().ToString(), "Facade_Name");
                if (existing_List.Count() > 0)
                {
                    Facade_Table facade = (Facade_Table)existing_List[0];
                    facade.Facade_Description = ((Facade_Table)jB_Abstract).Facade_Description;
                    facade.Facade_Name = jB_Abstract.GetType().ToString();
                    MemoryStream memStream = new MemoryStream();
                    StreamWriter sw = new StreamWriter(memStream);
                    sw.Write(jB_Abstract);
                    facade.Facade_Obj = memStream.GetBuffer();
                    return_List.Add(facade.Facade_ID.ToString());
                    facade_Facade.Add(facade);
                }
            }
            return return_List;
        }

        private List<string> Add_List(List<Abstract_Iter_able> abstract_List)
        {
            List<string> return_List = new List<string>();
            Facade_Table_Facade facade_Facade = new Facade_Table_Facade();
            foreach (Abstract_Iter_able jB_Abstract in abstract_List)
            {
                Facade_Table facade = new Facade_Table();
                facade.Facade_Description = "Test";
                facade.Facade_Name = jB_Abstract.GetType().ToString();
                facade.Facade_ID = Guid.NewGuid();
                MemoryStream memStream = new MemoryStream();
                StreamWriter sw = new StreamWriter(memStream);
                sw.Write(jB_Abstract);
                facade.Facade_Obj = memStream.GetBuffer();
                return_List.Add(facade.Facade_ID.ToString());
                facade_Facade.Add(facade);

            }
            return return_List;
        }


        protected void ddl_ListableList_clicked(Object sender, EventArgs e)
        { }

        /**
         * Creates the button tabs for the data tabs (Damages)
         */
        private void createDataTabs()
        {
            int data = Convert.ToInt32(hf_Selected_Object.Value);

            DataSet listableDS = new DataSet();
            DataTable listableDT = listableDS.Tables.Add();
            listableDT.Columns.Add("Listable_ID", typeof(string));
            listableDT.Columns.Add("Listable", typeof(string));
            for (int i = 0; i < master_Forms_List.Count; i++)
            {
                listableDT.Rows.Add(i.ToString(), master_Forms_List[i].Name);
            }
            listableList.DataValueField = "Listable_ID";
            listableList.DataTextField = "Listable";
            listableList.DataSource = listableDS;
            listableList.DataBind();
            listableList.SelectedIndex = data;
        }
        
        public void edit_Clicked(Object sender, EventArgs e)
        {
            Button clickedButton = (Button)sender;
            int row = Int32.Parse(clickedButton.ID.Substring(5));
            int data = Convert.ToInt32(hf_Selected_Object.Value);
            List_able listObj = current_Viewable.returnListable();
            List<string> variables = listObj.get_iterable_List()[row].getListOfKeys();
            List<string> types = listObj.get_iterable_List()[row].getListofTypes();
            Session["formable"] = master_Forms_List[data];
            if (Convert.ToInt32(hf_Selected_Object.Value).Equals(0))
            {
                Session["form_Obj"] = new List<Abstract_Iter_able> { listObj.get_iterable_List()[row] };
                Session["viewableType"] = master_Forms_List[data].Name;
                Session["viewDesired"] = "Edit";
                ParentPortlet.NextScreen("Objectable_View");
            }
            else
            {
                Session["form_Obj"] = new List<Abstract_Iter_able> { listObj.get_iterable_List()[row] };
                Session["viewableType"] = master_Forms_List[data].Name;
                Session["viewDesired"] = "Edit";
                ParentPortlet.NextScreen("Objectable_View");
            }
        }
        [WebMethod]
        public void test_edit_Clicked(int row)
        {
           
            int data = Convert.ToInt32(hf_Selected_Object.Value);
            List_able listObj = current_Viewable.returnListable();
            List<string> variables = listObj.get_iterable_List()[row].getListOfKeys();
            List<string> types = listObj.get_iterable_List()[row].getListofTypes();
            Session["formable"] = master_Forms_List[data];
            if (Convert.ToInt32(hf_Selected_Object.Value).Equals(0))
            {
                Session["form_Obj"] = new List<Abstract_Iter_able> { listObj.get_iterable_List()[row] };
                Session["viewableType"] = master_Forms_List[data].Name;
                Session["viewDesired"] = "Edit";               
                HttpContext.Current.Response.Write("{}");
                ParentPortlet.NextScreen("Objectable_View");
            }
            else
            {
                Session["form_Obj"] = new List<Abstract_Iter_able> { listObj.get_iterable_List()[row] };
                Session["viewableType"] = master_Forms_List[data].Name;
                Session["viewDesired"] = "Edit";
                HttpContext.Current.Response.Write("{}");
                ParentPortlet.NextScreen("Objectable_View");
            }
            HttpContext.Current.Response.Write("{}");
        }

        private void review_Clicked(object sender, EventArgs e)
        {
            Button clickedButton = (Button)sender;
            int row = Int32.Parse(clickedButton.ID.Substring(7));
            int data = Convert.ToInt32(hf_Selected_Object.Value);
            List_able listable = current_Viewable.returnListable();
            Abstract_Iter_able form_obj = listable.get_iterable_List()[row];
            JCF_Submission_Facade submission_Facade = new JCF_Submission_Facade();
            Flow_Rec_Facade flow_Rec_Facade = new Flow_Rec_Facade();
            Flow_Rec parent_Flow;
            JCF_Submission submission;
            List<Abstract_Iter_able> form_List = new List<Abstract_Iter_able>();
            if (form_obj.GetType().Equals(typeof(JCF_Submission)))
            {
                List<Abstract_Iter_able> flow_List = flow_Rec_Facade.GetByIdWhere(((JCF_Submission)form_obj).SubmissionID, "Submission_ID");
                if (flow_List.Count > 0)
                {
                    parent_Flow = (Flow_Rec)flow_Rec_Facade.GetByIdWhere(((JCF_Submission)form_obj).SubmissionID, "Submission_ID")[0];
                    List<Abstract_Iter_able> flow_objs = flow_Rec_Facade.GetByIdWhere(parent_Flow.ID, "Parrent_Flow");
                    List<Abstract_Iter_able> submissions;
                    submission = (JCF_Submission)submission_Facade.GetByIdWhere(((JCF_Submission)form_obj).SubmissionID, "SubmissionID")[0];
                    form_List.Add(submission);
                    foreach (Flow_Rec flow_obj in flow_objs)
                    {
                        submissions = submission_Facade.GetByIdWhere(flow_obj.Submission_ID, "SubmissionID");
                        if (submissions.Count > 0)
                        {
                            form_List.Add(submissions[0]);
                        }

                    }
                }
                else
                {
                    submission = (JCF_Submission)submission_Facade.GetByIdWhere(((JCF_Submission)form_obj).SubmissionID, "SubmissionID")[0];
                    form_List.Add(submission);
                }


                Session["form_Obj"] = form_List;
                Session["viewDesired"] = "Edit";
            }
            else
            {
                parent_Flow = (Flow_Rec)flow_Rec_Facade.GetByIdWhere(((Flow_Rec)form_obj).Submission_ID, "Submission_ID")[0];
                Flow_Rec flow_obj = (Flow_Rec)flow_Rec_Facade.GetByIdWhere(parent_Flow.ID, "Parrent_Flow")[0];
                submission = (JCF_Submission)submission_Facade.GetByIdWhere(flow_obj.Submission_ID, "SubmissionID")[0];
                Session["viewDesired"] = "Evaluation";
                while (parent_Flow.Parrent_Flow != new Guid())
                {
                    form_List.Add((Abstract_Iter_able)submission_Facade.GetByIdWhere(parent_Flow.Submission_ID, "SubmissionID")[0]);
                    parent_Flow = (Flow_Rec)flow_Rec_Facade.GetByIdWhere(parent_Flow.Parrent_Flow, "ID")[0];
                }
                if (parent_Flow.Parrent_Flow == new Guid())
                {
                    form_List.Add((Abstract_Iter_able)submission_Facade.GetByIdWhere(parent_Flow.Submission_ID, "SubmissionID")[0]);
                }
                Session["form_Obj"] = form_List;
            }
            Session["formable"] = master_Forms_List[data];
            Session["eval_Obj"] = submission;
            Session["viewableType"] = master_Forms_List[data].Name;

            Session["eval_Obj"] = submission;
            ParentPortlet.NextScreen("Objectable_View");
        }

        private void remove(GridViewRow row)
        {
            throw new NotImplementedException();
        }

        private void add_clicked(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        private void submit_Clicked(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        private void delete_Clicked(object sender, EventArgs e)
        {
            Button clickedButton = (Button)sender;
            CNTR_Resource_Schedule_Facade resource_Schedule_Facade = new CNTR_Resource_Schedule_Facade();

            string deleteLogMessage = PortalUser.Current.FirstName + " " + PortalUser.Current.LastName + " has deleted a Submission." + "\n";

            int row = Int32.Parse(clickedButton.ID.Substring(7));
            List_able listObj = current_Viewable.returnListable();
            switch (listObj.getType())
            {
                case "ICS_NHibernate.JCF_Submission":
                    JCF_Submission obj = (JCF_Submission)listObj.get_iterable_List()[row];
                    JCF_Submission_Facade submission_Facade = new JCF_Submission_Facade();
                    JCF_Answer_Facade answer_Facade = new JCF_Answer_Facade();
                    Flow_Rec_Facade flow_Rec_Facade = new Flow_Rec_Facade();
                    submission_Facade.Remove(obj);
                    deleteLogMessage += "SubmissionID: " + obj.SubmissionID.ToString() + "\n";
                    deleteLogMessage += "SubmissionUser: " + obj.UserID.ToString() + "\n";
                    deleteLogMessage += "SubmissionFormID: " + obj.FormID.ToString() + "\n";
                    deleteLogMessage += "SubmissionSubmittedDate: " + obj.SubmittedDate.ToString() + "\n";

                    try
                    {
                        List<CNTR_Resource_Schedule> resourceList = resource_Schedule_Facade.GetByIdWhere(obj.SubmissionID, "Submission_ID").Cast<CNTR_Resource_Schedule>().ToList();
                        foreach (CNTR_Resource_Schedule schedule in resourceList)
                        {
                            resource_Schedule_Facade.Remove(schedule);
                            deleteLogMessage += "ResourceID: " + schedule.Schedule_ID.ToString() + "\n";
                            deleteLogMessage += "ResourceStartTime: " + schedule.Start_Date.ToString() + "\n";
                            deleteLogMessage += "ResourceEndTime: " + schedule.End_Date.ToString() + "\n";
                            deleteLogMessage += "ResourceUser: " + schedule.User_ID.ToString() + "\n";
                        }

                    }
                    catch
                    {
                        EventLog.WriteEntry("ICSNET", "Error while clearing resource scheduled when deleting submission.");
                    }

                    List<Flow_Rec> flow_List = flow_Rec_Facade.GetByIdWhere(obj.SubmissionID, "Submission_ID").Cast<Flow_Rec>().ToList();
                    if (flow_List.Any())
                    {
                        Flow_Rec parent_Flow;
                        if (flow_List[0].Parrent_Flow == new Guid())
                            parent_Flow = flow_List[0];
                        else
                        {
                            flow_List = flow_Rec_Facade.GetByIdWhere(flow_List[0].Parrent_Flow, "ID").Cast<Flow_Rec>().ToList();
                            if (flow_List.Any())
                                parent_Flow = flow_List[0];
                            else
                                parent_Flow = null;
                        }
                        if (parent_Flow != null)
                        {
                            flow_List = flow_Rec_Facade.GetByIdWhere(parent_Flow.ID, "Parrent_Flow").Cast<Flow_Rec>().ToList();
                            List<JCF_Submission> submission_List = new List<JCF_Submission>();
                            foreach (Flow_Rec flow in flow_List)
                            {
                                List<JCF_Submission> submissions = submission_Facade.GetByIdWhere(flow.Submission_ID, "SubmissionID").Cast<JCF_Submission>().ToList();
                                if (submissions.Any())
                                {
                                    submission_List.Add(submissions[0]);
                                    submission_Facade.Remove(submissions[0]);
                                }
                                flow_Rec_Facade.Remove(flow);
                            }
                            foreach (JCF_Submission sub in submission_List)
                            {
                                List<Abstract_Iter_able> answer_List = answer_Facade.GetByIdWhere(sub.SubmissionID, "SubmissionID");
                                foreach (JCF_Answer answer in answer_List)
                                {
                                    answer_Facade.Remove(answer);
                                }

                            }

                            flow_Rec_Facade.Remove(parent_Flow);
                        }
                    }

                    List<Abstract_Iter_able> answers_List = answer_Facade.GetByIdWhere(obj.SubmissionID, "SubmissionID");
                    foreach (JCF_Answer x in answers_List)
                    {
                        answer_Facade.Remove(x);
                    }
                    break;
                default:

                    break;
            }
            var flow_Object = master_Forms_List[Convert.ToInt32(hf_Selected_Object.Value)] as Flow_Form_able;
            if (flow_Object != null)
                LoadFlow(master_Forms_List[Convert.ToInt32(hf_Selected_Object.Value)], master_Forms_List[Convert.ToInt32(hf_Selected_Object.Value)].Index_Value, master_Forms_List[Convert.ToInt32(hf_Selected_Object.Value)].Index);
            else
                LoadForm(master_Forms_List[Convert.ToInt32(hf_Selected_Object.Value)].facade, master_Forms_List[Convert.ToInt32(hf_Selected_Object.Value)].Index_Value, master_Forms_List[Convert.ToInt32(hf_Selected_Object.Value)].Index, master_Forms_List[Convert.ToInt32(hf_Selected_Object.Value)]);
            EventLog.WriteEntry("ICSNET", deleteLogMessage, EventLogEntryType.Warning);
            return;
        }


        private void open_Clicked(object sender, EventArgs e)
        {
            Button clickedButton = (Button)sender;
            int row = Int32.Parse(clickedButton.ID.Substring(5));
            List_able listObj = current_Viewable.returnListable();
            JCF_Submission obj = (JCF_Submission)listObj.get_iterable_List()[row];
            Object formID = obj.FormID;
            Object formNo = obj.SubmissionID;

            JCF_Form_Facade form_facade = new JCF_Form_Facade();
            JCF_Form form = (JCF_Form)form_facade.GetByIdWhere(formID, "FormID")[0];
            Guid lookupID = form.PortletID;

            Form_URL_Table_Facade formurl_facade = new Form_URL_Table_Facade();
            Form_URL_Table formurltable = (Form_URL_Table)formurl_facade.GetByIdWhere(lookupID, "PortletID")[0];

            //Response.Redirect("~/Admin/CNTR_Design/Jenzabar_Contained_Form.jnz?submission_id=" + formNo + "&form=" + formID);
            Response.Redirect(formurltable.PortletURL + "?submission_id=" + formNo + "&form=" + formID);

        }
        private void print_Form(JCF_Submission submission)
        {
            string filename = string.Empty;
            Document document = new Document();
            Section section;

            JCF_Submission_Facade submission_Facade = new JCF_Submission_Facade();
            Flow_Rec_Facade flow_Rec_Facade = new Flow_Rec_Facade();
            Flow_Rec parent_Flow;

            List<Abstract_Iter_able> form_List = new List<Abstract_Iter_able>();

            List<Abstract_Iter_able> flow_List = flow_Rec_Facade.GetByIdWhere(submission.SubmissionID, "Submission_ID");
            if (flow_List.Count > 0)
            {
                parent_Flow = (Flow_Rec)flow_Rec_Facade.GetByIdWhere(submission.SubmissionID, "Submission_ID")[0];
                List<Abstract_Iter_able> flow_objs = flow_Rec_Facade.GetByIdWhere(parent_Flow.ID, "Parrent_Flow");
                List<Abstract_Iter_able> submissions;

                form_List.Add(submission);
                foreach (Flow_Rec flow_obj in flow_objs)
                {
                    submissions = submission_Facade.GetByIdWhere(flow_obj.Submission_ID, "SubmissionID");
                    if (submissions.Count > 0)
                    {
                        form_List.Add(submissions[0]);
                    }

                }
            }
            else
            {
                form_List.Add(submission);
            }
            Paragraph answer_Paragraph = new Paragraph();
            Paragraph question_Pragraph = new Paragraph();
            Paragraph header_Paragaraph = new Paragraph();
            List<Color> color_List = new List<Color>() { Colors.Red, Colors.Orange, Colors.Yellow, Colors.Green, Colors.Blue, Colors.Indigo, Colors.Violet };
            int i = 0;
            foreach (JCF_Submission submissionForm in form_List)
            {
                section = document.AddSection();
                header_Paragaraph = section.AddParagraph(GetFormHeader(submissionForm));
                header_Paragaraph.Format.Alignment = ParagraphAlignment.Left;
                header_Paragaraph.Format.Font.Name = "Times New Roman";
                header_Paragaraph.Format.Font.Color = Colors.Black;
                header_Paragaraph.Format.Borders.Distance = "5pt";
                header_Paragaraph.Format.Borders.Color = color_List[i % 7];
                header_Paragaraph.Format.Font.Size = 16;
                List<JCF_FormItem> questions = new JICSBaseFacade<JCF_FormItem>().GetQuery().Where(x => x.FormID == submissionForm.FormID).OrderBy(x => x.QuestionNum).ThenBy(x => x.RowNum).ToList();
                foreach (JCF_FormItem question in questions)
                {
                    question_Pragraph = section.AddParagraph(StripHTML(current_Viewable.nullCheck(question.Text).ToString()));
                    question_Pragraph.Format.Alignment = ParagraphAlignment.Left;
                    question_Pragraph.Format.Font.Name = "Times New Roman";
                    question_Pragraph.Format.Font.Color = Colors.Black;
                    question_Pragraph.Format.Font.Size = 12;
                    question_Pragraph.Format.Borders.Distance = "5pt";
                    question_Pragraph.Format.Borders.Color = color_List[i % 7];

                    List<JCF_Answer> answers = new JICSBaseFacade<JCF_Answer>().GetQuery().Where(x => x.ItemID == question.ID && x.SubmissionID == submissionForm.SubmissionID).ToList();
                    if (answers.Count > 0)
                    {
                        foreach (JCF_Answer answer in answers)
                        {
                            answer_Paragraph = section.AddParagraph(StripHTML(current_Viewable.nullCheck(answer.AnswerValue).ToString()));
                            answer_Paragraph.Format.Alignment = ParagraphAlignment.Justify;
                            answer_Paragraph.Format.Font.Name = "Times New Roman";
                            answer_Paragraph.Format.Font.Color = Colors.DarkGray;
                            answer_Paragraph.Format.Font.Size = 12;
                        }
                    }
                    else
                    {
                        answer_Paragraph = section.AddParagraph(" ");
                        answer_Paragraph.Format.Alignment = ParagraphAlignment.Justify;
                        answer_Paragraph.Format.Font.Size = 12;
                    }
                }
                i++;
            }

            PdfDocumentRenderer pdfRenderer = new PdfDocumentRenderer(false);
            pdfRenderer.Document = document;
            pdfRenderer.RenderDocument();
            filename = GetSubmissionFileName(submission);
            EventLog.WriteEntry("ICSNET", "FILENAME:" + filename, EventLogEntryType.Information);
            Response.Clear();
            Response.ContentType = "application/pdf";
            MemoryStream stream = new MemoryStream();
            pdfRenderer.PdfDocument.Save(stream, false);
            Response.AddHeader("Content-Disposition", "attachment; filename=" + filename);
            Response.AddHeader("content-length", stream.Length.ToString());
            Response.BinaryWrite(stream.ToArray());
            Response.Flush();
            stream.Close();
            Response.End();
        }

        private string GetSubmissionFileName(JCF_Submission submission)
        {
            string return_String;
            if (submission.UserID != new Guid())
            {
                ICS_Portal_User user = new JICSBaseFacade<ICS_Portal_User>().GetQuery().Where(x => x.ID == submission.UserID).SingleOrDefault();
                //return_String = user.HostID.TrimStart('0')+"_"+user.LastName+"_"+user.FirstName+ "_JCF_" + ".pdf";
                return_String = user.HostID.TrimStart('0') + "_JCF.pdf";
            }
            else
            {
                External_Submitter external_User = new JICSBaseFacade<External_Submitter>().GetQuery().Where(x => x.Submission_ID == submission.SubmissionID).SingleOrDefault();
                if (external_User != null)
                {
                    return_String = "External_JCF.pdf";
                }
                else
                    return_String = "ERROR_JCF.pdf";
            }
            return return_String;
        }

        private string GetFormHeader(JCF_Submission submission)
        {
            string return_String = "";
            JCF_Form jcf_Form = new JICSBaseFacade<JCF_Form>().GetQuery().Where(x => x.FormID == submission.FormID).SingleOrDefault();
            return_String += "Form Name:" + jcf_Form.Name + "\n";
            if (submission.UserID != new Guid())
            {
                ICS_Portal_User user = new JICSBaseFacade<ICS_Portal_User>().GetQuery().Where(x => x.ID == submission.UserID).SingleOrDefault();
                return_String += "Submitter Name:" + user.LastName + "," + user.FirstName + "\n";
                return_String += "Submitter ID:" + user.ID + "\n";
                return_String += "Submitted Date:" + submission.SubmittedDate.ToString("dddd, dd MMMM yyyy");
            }
            else
            {
                External_Submitter external_User = new JICSBaseFacade<External_Submitter>().GetQuery().Where(x => x.Submission_ID == submission.SubmissionID).SingleOrDefault();
                if (external_User != null)
                {
                    return_String += "External Submitter Email:" + external_User.User_Email + "\n";
                    return_String += "Submitted Date:" + submission.SubmittedDate.ToString("dddd, dd MMMM yyyy");
                }
                else
                    return_String += "Error Loading Submission:" + submission.SubmissionID;
            }
            return return_String;

        }
        public static string StripHTML(string input)
        {
            return Regex.Replace(input, "<.*?>", String.Empty);
        }
        private void print_Form(CNTR_CovidForm_Rec submission)
        {
            string filename = string.Empty;
            Document document = new Document();
            Section section;

            Paragraph answer_Paragraph = new Paragraph();
            Paragraph question_Pragraph = new Paragraph();
            Paragraph header_Paragaraph = new Paragraph();
            List<Color> color_List = new List<Color>() { Colors.Goldenrod, Colors.Orange, Colors.Yellow, Colors.Green, Colors.Blue, Colors.Indigo, Colors.Violet };
            int i = 0;
            section = document.AddSection();
            MigraDoc.DocumentObjectModel.Shapes.Image centre_Logo = section.Headers.Primary.AddImage("E:/ICSFileServer/Themes/Centre_College_Theme/images/stacked_logo_RGB.jpg");
            ICS_Portal_User user = new JICSBaseFacade<ICS_Portal_User>().GetQuery().Where(x => x.ID == submission.UserID).SingleOrDefault();
            
            
            Paragraph centre_Header = section.Headers.Primary.AddParagraph();
            centre_Header.AddText("COVID-19 Self Reporting Form");
            centre_Header.Format.Font.Size = 12;
            centre_Header.Format.Alignment = ParagraphAlignment.Left;

            Paragraph centre_Header_Line2 = section.Headers.Primary.AddParagraph();
            if (user != null)
            {
                centre_Header_Line2.AddText(user.LastName + ", " + user.FirstName + " - " + user.HostID.TrimStart('0'));
            }
            else
                centre_Header_Line2.AddText("Error: User Not Found");
            centre_Header_Line2.Format.Font.Size = 12;
            centre_Header_Line2.Format.Alignment = ParagraphAlignment.Left;

            Paragraph centre_Header_Line3 = section.Headers.Primary.AddParagraph();
            centre_Header_Line3.AddText("Completed: " + submission.SubmittedDate.ToString("dddd, dd MMMM yyyy"));
            centre_Header_Line3.Format.Font.Size = 12;
            centre_Header_Line3.Format.Alignment = ParagraphAlignment.Left;
            

            centre_Logo.Height = "2.5cm";
            centre_Logo.LockAspectRatio = true;
            centre_Logo.RelativeVertical = RelativeVertical.Line;
            centre_Logo.RelativeHorizontal = RelativeHorizontal.Margin;
            centre_Logo.Top = ShapePosition.Top;
            centre_Logo.Left = ShapePosition.Right;
            centre_Logo.WrapFormat.Style = WrapStyle.Through;

            Paragraph centre_Footer = section.Footers.Primary.AddParagraph();
            centre_Footer.AddText("Centre College · 600 West Walnut St · Danville, KY · USA");
            centre_Footer.Format.Font.Size = 9;
            centre_Footer.Format.Alignment = ParagraphAlignment.Center;


            header_Paragaraph = section.AddParagraph("COVID-19 Self Reporting Form");
            header_Paragaraph.Format.Alignment = ParagraphAlignment.Left;
            header_Paragaraph.Format.Font.Name = "Times New Roman";
            header_Paragaraph.Format.Font.Color = Colors.Black;
            header_Paragaraph.Format.Borders.Distance = "5pt";
            header_Paragaraph.Format.Borders.Color = color_List[i % 7];
            header_Paragaraph.Format.Shading.Color = color_List[i % 7];
            header_Paragaraph.Format.SpaceBefore = "2cm";
            header_Paragaraph.Format.Font.Size = 16;

            Dictionary<string,object> questions = new Dictionary<string, object>(){
                { "SubmissionID", submission.SubmissionID },
                { "SubmittedDate:", submission.SubmittedDate },
                { "Temperature over 100.4:", submission.Temp },
                { "ShortnessOfBreath:", submission.ShortnessOfBreath },
                { "Chills:", submission.Chills },
                { "MusclePain:", submission.MusclePain },
                { "SoreThroat:", submission.SoreThroat },
                { "SmellOrTaste:", submission.SmellOrTaste },
                { "CentreID:", submission.CentreID }
            };
            foreach (KeyValuePair<string,object> question in questions)
            {
                question_Pragraph = section.AddParagraph(StripHTML(current_Viewable.nullCheck(question.Key).ToString()));
                question_Pragraph.Format.Alignment = ParagraphAlignment.Left;
                question_Pragraph.Format.Font.Name = "Times New Roman";
                question_Pragraph.Format.Font.Color = Colors.Black;
                question_Pragraph.Format.Font.Size = 12;
                question_Pragraph.Format.Borders.Distance = "5pt";

                answer_Paragraph = section.AddParagraph(StripHTML(current_Viewable.nullCheck(question.Value).ToString()));
                answer_Paragraph.Format.Alignment = ParagraphAlignment.Justify;
                answer_Paragraph.Format.Font.Name = "Times New Roman";
                answer_Paragraph.Format.Font.Color = Colors.DarkGray;
                answer_Paragraph.Format.Font.Size = 12;
                answer_Paragraph.Format.LeftIndent = "2cm";
            }

            PdfDocumentRenderer pdfRenderer = new PdfDocumentRenderer(false);
            pdfRenderer.Document = document;
            pdfRenderer.RenderDocument();
            filename = "default.pdf";
            EventLog.WriteEntry("ICSNET", "FILENAME:" + filename, EventLogEntryType.Information);
            Response.Clear();
            Response.ContentType = "application/pdf";
            MemoryStream stream = new MemoryStream();
            pdfRenderer.PdfDocument.Save(stream, false);
            Response.AddHeader("Content-Disposition", "attachment; filename=" + filename);
            Response.AddHeader("content-length", stream.Length.ToString());
            Response.BinaryWrite(stream.ToArray());
            Response.Flush();
            stream.Close();
            Response.End();
        }

    }
}