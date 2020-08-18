using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Script.Services;
using System.Web.Services;
using System.Web.UI.WebControls;
using CNTRDesignObjects.Iter_ables;
using ICS_NHibernate;
using Jenzabar.Portal.Framework.Web.UI;
using CNTRDesignObjects.Viewables;
using CNTRDesignObjects.Object_ables;
using CNTRDesignObjects.TEMP_Objects;
using System.Data;
using System.Text;
using System.Diagnostics;
using Jenzabar.Portal.Framework.NHibernateFWK;

namespace CNTRCovidForm.lib
{
    /// <summary>
    /// Summary description for ViewPortletWebService
    /// </summary>
    [WebService(Namespace = "https://centrenet.centre.edu/WebService")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    [ScriptService]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    // [System.Web.Script.Services.ScriptService]
    public class CovidFormWebService : System.Web.Services.WebService
    {

        [WebMethod]
        public string HelloWorld()
        {
            return "Hello World";
        }

        [WebMethod(EnableSession = true)]
        public List<Guid> GetFieldGuids()
        {
            if (Session["field_guids"] == null)
            {
                List<Guid> failure = new List<Guid>();

                return failure;
            }
            else
            {
                List<Guid> guidStrings = (List<Guid>)Session["field_guids"];
                return guidStrings;
            }

        }
        [WebMethod(EnableSession = true)]
        public string ParseTemplateFields(List<Guid> guids, Dictionary<string, object> controls)
        {
            CNTR_Template_Field_Facade field_facade = new CNTR_Template_Field_Facade();
            CNTR_Template_Facade template_facade = new CNTR_Template_Facade();
            List<CNTR_Template_Field> fields = new List<CNTR_Template_Field>();
            foreach (Guid guid in guids)
            {
                List<Abstract_Iter_able> list = field_facade.GetByIdWhere(guid, "Field_ID");
                if (list.Any())
                {
                    foreach (CNTR_Template_Field field in list)
                    {
                        fields.Add(field);
                    }
                }
            }
            List<Abstract_Iter_able> template_list = template_facade.GetByIdWhere(fields[0].Template_ID, "Template_ID");
            CNTR_Template template = new CNTR_Template();
            foreach (CNTR_Template temp in template_list)
            {
                template = temp;
            }

            string new_html_string = template.Html_String;
            foreach (CNTR_Template_Field field in fields)
            {
                foreach (string key in controls.Keys)
                {
                    Dictionary<string, object> control = (Dictionary<string, object>)controls[key];
                    if (control["idString"].ToString().Contains("template_Input") && control["idString"].ToString().Contains(field.Field_ID.ToString()))
                    {
                        field.Replacement_Value = control["valueString"].ToString();
                        field_facade.Add(field);
                        new_html_string = new_html_string.Replace(field.Placeholder_Value, field.Replacement_Value);
                    }
                }
            }
            return new_html_string;
        }

        [WebMethod(EnableSession = true)]
        public List<string> GetAnswerGuids()
        {


            if (Session["answer_Guids"] == null)
            {
                List<string> failure = new List<string>() { "Answer Guids was null" };
                return failure;
            }
            else
            {
                List<Guid> answer_Guids = (List<Guid>)Session["answer_Guids"];
                List<string> guidStrings = new List<string>();
                foreach (Guid id in answer_Guids)
                {
                    guidStrings.Add(id.ToString());
                }
                /*var jsonserializer = new System.Web.Script.Serialization.JavaScriptSerializer();
                string json = jsonserializer.Serialize(guidStrings);*/
                return guidStrings;
            }

        }
        [WebMethod(EnableSession = true)]
        public List<string> ParseControls(List<string> guidList, Dictionary<string, object> controls)
        {
            List<string> values = new List<string>();
            foreach (string guid in guidList)
            {

                foreach (string key in controls.Keys)
                {
                    Dictionary<string, object> control = (Dictionary<string, object>)controls[key];
                    if (control["idString"].ToString().Contains("variable_WebControl") && control["idString"].ToString().Contains(guid))
                    {
                        values.Add(guid);
                        try
                        {
                            //values.Add(control["idString"].ToString());

                            values.Add(control["valueString"].ToString());
                            break;
                        }
                        catch
                        {
                            try
                            {

                                values.Add(control["innerText"].ToString());
                                break;
                            }
                            catch
                            {
                                values.Add("Had no value and no inner text");
                                break;
                            }
                        }
                    }

                }
            }
            /*foreach (string key in controls.Keys)
            {
                Dictionary<string, object> control = (Dictionary<string, object>)controls[key];
                try
                {
                    //values.Add(control["idString"].ToString());
                    values.Add(control["valueString"].ToString());
                }
                catch
                {
                    try
                    {
                        values.Add(control["innerText"].ToString());
                    }
                    catch
                    {
                        values.Add("Had no value and no inner text");
                    }
                }
            }*/
            return values;

        }


        [WebMethod(EnableSession = true)]
        public List<string> GetColumns()
        {


            if (HttpContext.Current.Session["current_Viewable"] == null)
            {
                List<string> failure = new List<string>();
                failure.Add("No Columns Detected");
                return failure;
            }
            else
            {
                CNTR_Viewable view = (CNTR_Viewable)Session["current_Viewable"];
                int selected = (int)Session["hidden_field"];
                List<Form_able> masterList = (List<Form_able>)Session["master_Forms_List"];
                DataTable dt = (DataTable)view.getView(masterList[selected], 0, 0, 1, 0, "", "", out int filteredCount);
                //Session["dt"] = dt;
                dt.Columns.Add(new DataColumn("Options"));
                List<string> data_Columns = new List<string>();
                foreach (DataColumn data_Column in dt.Columns)
                {
                    data_Columns.Add(data_Column.ColumnName);
                }
                return data_Columns;
            }

        }


        [WebMethod(EnableSession = true)]
        public void GetData(int draw, int start, int length)
        {

            int order = Convert.ToInt32(HttpContext.Current.Request["order[0][column]"]);
            string direction = HttpContext.Current.Request["order[0][dir]"];
            string search = HttpContext.Current.Request["search[value]"];
            CNTR_Viewable view = (CNTR_Viewable)Session["current_Viewable"];
            int selected = (int)Session["hidden_field"];
            List<Form_able> masterList = (List<Form_able>)Session["master_Forms_List"];
            EventLog.WriteEntry("ICSNET", "Getting Data");

            if (length == -1)
            {
                length = view.getCount();
            }
            EventLog.WriteEntry("ICSNET", "Getting View");
            DataTable dt = (DataTable)view.getView(masterList[selected], 0, start, length, order, search, direction, out int filteredCount);

            EventLog.WriteEntry("ICSNET", "Retrieved View");
            dt.Columns.Add(new DataColumn("Options"));
            StringBuilder jsonString = new StringBuilder();
            DataSet data_Set = new DataSet();
            data_Set.Merge(dt);
            string list_Data = "[]";
            if (data_Set != null && data_Set.Tables[0].Rows.Count > 0)
            {
                jsonString.Append("[");
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    jsonString.Append("{");
                    for (int k = 0; k < data_Set.Tables[0].Columns.Count; k++)
                    {
                        if (k < data_Set.Tables[0].Columns.Count - 1)
                        {
                            jsonString.Append("\"" + data_Set.Tables[0].Columns[k].ColumnName.ToString() + "\":" + "\"" + data_Set.Tables[0].Rows[i][k].ToString() + "\",");
                        }
                        else if (k == data_Set.Tables[0].Columns.Count - 1)
                        {
                            jsonString.Append("\"" + data_Set.Tables[0].Columns[k].ColumnName.ToString() + "\":" + "null");
                        }
                    }
                    if (i == dt.Rows.Count - 1)
                    {
                        jsonString.Append("}");
                    }
                    else
                    {
                        jsonString.Append("},");
                    }
                }
                jsonString.Append("]");
                list_Data = jsonString.ToString();

            }



            string modified_Data = "{ \"draw\": " + draw.ToString() + ",\"recordsTotal\": " + view.getCount().ToString() + ",\"recordsFiltered\": " + filteredCount.ToString() + ", \"data\": " + list_Data + "}";


            /*var xdoc = XDocument.Parse(list_Data);
            var items = xdoc.Descendants("Item")
                            .ToDictionary(i => (string)i.Attribute("Key"),
                                            i => (string)i.Attribute("Value"));
            var jsonserializer = new System.Web.Script.Serialization.JavaScriptSerializer();
            string json = jsonserializer.Serialize(items);*/
            HttpContext.Current.Response.Write(modified_Data);



        }

        [WebMethod(EnableSession = true)]
        public string submit_Clicked()
        {
            HttpContext.Current.Session["button_Clicked"] = "save";
            return "save";
        }
        [WebMethod(EnableSession = true)]
        public string resubmit_Clicked()
        {
            HttpContext.Current.Session["button_Clicked"] = "save";
            return "resubmit";
        }
        [WebMethod(EnableSession = true)]
        public string edit_Clicked(int row)
        {
            HttpContext.Current.Session["button_Clicked"] = "edit";
            return "edit";
        }
        [WebMethod(EnableSession = true)]
        public string review_Clicked()
        {
            HttpContext.Current.Session["button_Clicked"] = "review";
            return "review";
        }
        [WebMethod(EnableSession = true)]
        public string print_Clicked()
        {
            HttpContext.Current.Session["button_Clicked"] = "print";
            return "print";
        }
        [WebMethod(EnableSession = true)]
        public string delete_Clicked()
        {
            HttpContext.Current.Session["button_Clicked"] = "delete";
            return "delete";
        }

        [WebMethod(EnableSession = true)]
        public string test_name()
        {
            View_Singleton single = View_Singleton.getInstance();
            PortletViewBase portlet = single.getView();
            return portlet.ParentPortlet.CurrentPortletViewName;
        }
        private void readyViewObject(List<Abstract_Iter_able> view_Objects, JCF_Submission review_Obj, List<Guid> answer_Guids, Dictionary<Guid, object> question_Dictionary_List, Dictionary<Guid, string> question_Answer_Dictionary_List, Dictionary<Guid, string> question_Options_Dictionary_List
                                    , List<Guid> review_Answer_Guids, Dictionary<Guid, object> review_Question_Dictionary_List, Dictionary<Guid, string> review_Question_Answer_Dictionary_List, Dictionary<Guid, string> review_Question_Options_Dictionary_List)
        {
            JCF_Form_Facade form_Facade = new JCF_Form_Facade();
            JICSBaseFacade<JCF_FormItem> form_Item_facade = new JICSBaseFacade<JCF_FormItem>();
            JICSBaseFacade<JCF_Answer> answer_Facade = new JICSBaseFacade<JCF_Answer>();
            ICS_Portal_User_Facade user_Facade = new ICS_Portal_User_Facade();

            int i = 0;
            Dictionary<int, string> type_Dictionary = new Dictionary<int, string> {
                { 0, "primary" },
                { 1, "warning" },
                { 2, "success" },
                { 3, "danger" },
                { 4, "info" }

            };


            foreach (JCF_Submission view_Obj in view_Objects)
            {


                List<JCF_FormItem> questions = form_Item_facade.GetQuery().Where(x => x.FormID == view_Obj.FormID).ToList();
                //List<Abstract_Iter_able> results_Objects;
                //results_Objects = answer_Facade.GetByIdWhere(view_Obj.SubmissionID, "SubmissionID");
                /*List<JCF_FormItem> questions = new List<JCF_FormItem>();

                foreach (JCF_FormItem question in form_items)
                {
                    questions.Add(question);

                }*/
                var ordered_form_items = questions.OrderBy(o => o.RowNum).ThenBy(o => o.QuestionNum);
                foreach (JCF_FormItem item in ordered_form_items)
                {

                    if (item.IsActive)
                    {
                        //object_Options.Add(type_Dictionary[i % 5]);

                        //question_Objects.Add(item);

                        /*List<string> query_Strings = new List<string>() { "SubmissionID", "ItemID" };
                        List<object> query_Objects = new List<object>() { view_Obj.SubmissionID, item.ID };
                        List<Abstract_Iter_able> items = answer_Facade.GetByIdWhere(query_Objects, query_Strings);*/
                        List<JCF_Answer> items = answer_Facade.GetQuery().Where(x => x.SubmissionID == view_Obj.SubmissionID && x.ItemID == item.ID).ToList();
                        if (items.Count > 0)
                        {
                            //answer_Objects.Add(items[0]);
                            JCF_Answer jCF_Answer = items[0];
                            question_Answer_Dictionary_List.Add(jCF_Answer.AnswerID, jCF_Answer.AnswerValue);
                            question_Options_Dictionary_List.Add(jCF_Answer.AnswerID, type_Dictionary[i % 5]);
                            question_Dictionary_List.Add(jCF_Answer.AnswerID, item);
                            answer_Guids.Add(jCF_Answer.AnswerID);
                            //question_Types.Add(item.Type.ToString());

                        }
                        else
                        {
                            JCF_Answer new_Answer = new JCF_Answer();
                            new_Answer.SubmissionID = view_Obj.SubmissionID;
                            new_Answer.AnswerID = Guid.NewGuid();
                            new_Answer.ItemID = item.ID;
                            new_Answer.AnswerValue = "";
                            answer_Facade.Save(new_Answer);
                            answer_Facade.Flush();
                            //answer_Objects.Add(new_Answer);
                            question_Answer_Dictionary_List.Add(new_Answer.AnswerID, new_Answer.AnswerValue);
                            question_Options_Dictionary_List.Add(new_Answer.AnswerID, type_Dictionary[i % 5]);
                            question_Dictionary_List.Add(new_Answer.AnswerID, item);
                            answer_Guids.Add(new_Answer.AnswerID);

                            //question_Types.Add(item.Type.ToString());
                        }

                    }

                }
                i++;
            }

            List<JCF_FormItem> review_Questions = form_Item_facade.GetQuery().Where(x => x.FormID == review_Obj.FormID).ToList();
            /*List<JCF_FormItem> review_Questions = new List<JCF_FormItem>();
            foreach (JCF_FormItem question in review_items)
            {
                review_Questions.Add(question);
            }*/
            var ordered_review_items = review_Questions.OrderBy(o => o.RowNum).ThenBy(o => o.QuestionNum);
            foreach (JCF_FormItem item in ordered_review_items)
            {
                if (item.Required == true || item.Required == false)
                {
                    //object_Options.Add(type_Dictionary[i % 5]);

                    //question_Objects.Add(item);

                    //List<string> query_Strings = new List<string>() { "SubmissionID", "ItemID" };
                    //List<object> query_Objects = new List<object>() { review_Obj.SubmissionID, item.ID };
                    List<JCF_Answer> items = answer_Facade.GetQuery().Where(x => x.SubmissionID == review_Obj.SubmissionID && x.ItemID == item.ID).ToList();
                    if (items.Count > 0)
                    {
                        //answer_Objects.Add(items[0]);
                        JCF_Answer jCF_Answer = items[0];
                        review_Question_Answer_Dictionary_List.Add(jCF_Answer.AnswerID, jCF_Answer.AnswerValue);

                        review_Question_Options_Dictionary_List.Add(jCF_Answer.AnswerID, type_Dictionary[i % 5]);
                        review_Question_Dictionary_List.Add(jCF_Answer.AnswerID, item);
                        review_Answer_Guids.Add(jCF_Answer.AnswerID);

                        //review_question_Types.Add(item.Type.ToString());

                    }
                    else
                    {
                        Guid answer_Guid = Guid.NewGuid();
                        JCF_Answer new_Answer = new JCF_Answer();
                        new_Answer.SubmissionID = review_Obj.SubmissionID;
                        new_Answer.ItemID = item.ID;
                        new_Answer.AnswerValue = "";
                        new_Answer.AnswerID = answer_Guid;
                        answer_Facade.Save(new_Answer);
                        answer_Facade.Flush();
                        //answer_Objects.Add(new_Answer);
                        review_Question_Answer_Dictionary_List.Add(new_Answer.AnswerID, new_Answer.AnswerValue);
                        review_Question_Options_Dictionary_List.Add(new_Answer.AnswerID, type_Dictionary[i % 5]);
                        review_Question_Dictionary_List.Add(new_Answer.AnswerID, item);
                        review_Answer_Guids.Add(new_Answer.AnswerID);
                        //review_question_Types.Add(item.Type.ToString());
                    }

                }

            }

        }

        private void readyViewObject(List<Flow_Rec> view_Objects, List<Guid> answer_Guids, Dictionary<Guid, object> question_Dictionary_List, Dictionary<Guid, string> question_Answer_Dictionary_List, Dictionary<Guid, string> question_Options_Dictionary_List)
        {
            JICSBaseFacade<JCF_FormItem> facade = new JICSBaseFacade<JCF_FormItem>();
            JICSBaseFacade<JCF_Answer> answer_Facade = new JICSBaseFacade<JCF_Answer>();
            JICSBaseFacade<JCF_Submission> submission_Facade = new JICSBaseFacade<JCF_Submission>();

            int i = 0;
            Dictionary<int, string> type_Dictionary = new Dictionary<int, string> {
                { 0, "primary" },
                { 1, "warning" },
                { 2, "success" },
                { 3, "danger" },
                { 4, "info" }
            };
            foreach (Flow_Rec view_Obj in view_Objects)
            {
                //List<Abstract_Iter_able> results_Objects;
                JCF_Submission submission = submission_Facade.GetQuery().Where(x => x.SubmissionID == view_Obj.Submission_ID).ToList()[0];
                //results_Objects = answer_Facade.GetByIdWhere(view_Obj.Submission_ID, "SubmissionID");
                List<JCF_FormItem> form_items = facade.GetQuery().Where(x => x.FormID == submission.FormID).ToList();


                foreach (JCF_FormItem item in form_items)
                {
                    if (item.IsActive)
                    {
                        //object_Options.Add(type_Dictionary[i % 5]);

                        //question_Objects.Add(item);

                        List<string> query_Strings = new List<string>() { "SubmissionID", "ItemID" };
                        List<object> query_Objects = new List<object>() { view_Obj.Submission_ID, item.ID };
                        List<JCF_Answer> items = answer_Facade.GetQuery().Where(x => x.SubmissionID == submission.SubmissionID && x.ItemID == item.ID).ToList();
                        if (items.Count > 0)
                        {
                            //answer_Objects.Add(items[0]);
                            JCF_Answer jCF_Answer = items[0];
                            question_Answer_Dictionary_List.Add(jCF_Answer.AnswerID, jCF_Answer.AnswerValue);

                            question_Options_Dictionary_List.Add(jCF_Answer.AnswerID, type_Dictionary[i % 5]);
                            question_Dictionary_List.Add(jCF_Answer.AnswerID, item);
                            answer_Guids.Add(jCF_Answer.AnswerID);
                            //question_Types.Add(item.Type.ToString());

                        }
                        else
                        {
                            JCF_Answer new_Answer = new JCF_Answer();
                            new_Answer.SubmissionID = view_Obj.Submission_ID;
                            new_Answer.ItemID = item.ID;
                            new_Answer.AnswerValue = "";
                            new_Answer.AnswerID = Guid.NewGuid();
                            answer_Facade.Save(new_Answer);
                            answer_Facade.Flush();
                            //answer_Objects.Add(new_Answer);
                            question_Answer_Dictionary_List.Add(new_Answer.AnswerID, new_Answer.AnswerValue);
                            question_Options_Dictionary_List.Add(new_Answer.AnswerID, type_Dictionary[i % 5]);
                            question_Dictionary_List.Add(new_Answer.AnswerID, item);
                            answer_Guids.Add(new_Answer.AnswerID);
                            //question_Types.Add(item.Type.ToString());
                        }

                    }

                }
                i++;
            }
        }

        private void readyViewObject(List<JCF_Submission> view_Objects, List<Guid> answer_Guids, Dictionary<Guid, object> question_Dictionary_List, Dictionary<Guid, string> question_Answer_Dictionary_List, Dictionary<Guid, string> question_Options_Dictionary_List)
        {

            JICSBaseFacade<JCF_FormItem> facade = new JICSBaseFacade<JCF_FormItem>();
            JICSBaseFacade<JCF_Answer> answer_Facade = new JICSBaseFacade<JCF_Answer>();

            int i = 0;
            Dictionary<int, string> type_Dictionary = new Dictionary<int, string> {
                { 0, "primary" },
                { 1, "warning" },
                { 2, "success" },
                { 3, "danger" },
                { 4, "info" }
            };
            foreach (JCF_Submission view_Obj in view_Objects)
            {


                List<JCF_FormItem> questions = facade.GetQuery().Where(x => x.FormID == view_Obj.FormID).ToList();
                //List<Abstract_Iter_able> results_Objects;
                //results_Objects = answer_Facade.GetByIdWhere(view_Obj.SubmissionID, "SubmissionID");
                //List<JCF_FormItem> questions = new List<JCF_FormItem>();

                /*foreach (JCF_FormItem question in form_items)
                {
                    questions.Add(question);

                }*/
                var ordered_form_items = questions.OrderBy(o => o.RowNum).ThenBy(o => o.QuestionNum);
                foreach (JCF_FormItem item in ordered_form_items)
                {

                    if (item.IsActive)
                    {
                        //object_Options.Add(type_Dictionary[i % 5]);

                        //question_Objects.Add(item);

                        //List<string> query_Strings = new List<string>() { "SubmissionID", "ItemID" };
                        //List<object> query_Objects = new List<object>() { view_Obj.SubmissionID, item.ID };
                        List<JCF_Answer> items = answer_Facade.GetQuery().Where(x => x.SubmissionID == view_Obj.SubmissionID && x.ItemID == item.ID).ToList();
                        if (items.Count > 0)
                        {
                            //answer_Objects.Add(items[0]);
                            JCF_Answer jCF_Answer = items[0];
                            question_Answer_Dictionary_List.Add(jCF_Answer.AnswerID, jCF_Answer.AnswerValue);
                            question_Options_Dictionary_List.Add(jCF_Answer.AnswerID, type_Dictionary[i % 5]);
                            question_Dictionary_List.Add(jCF_Answer.AnswerID, item);
                            answer_Guids.Add(jCF_Answer.AnswerID);
                            //question_Types.Add(item.Type.ToString());

                        }
                        else
                        {
                            JCF_Answer new_Answer = new JCF_Answer();
                            new_Answer.SubmissionID = view_Obj.SubmissionID;
                            new_Answer.ItemID = item.ID;
                            new_Answer.AnswerValue = "";
                            new_Answer.AnswerID = Guid.NewGuid();
                            answer_Facade.Save(new_Answer);
                            answer_Facade.Flush();
                            //answer_Objects.Add(new_Answer);
                            question_Answer_Dictionary_List.Add(new_Answer.AnswerID, new_Answer.AnswerValue);
                            question_Options_Dictionary_List.Add(new_Answer.AnswerID, type_Dictionary[i % 5]);
                            question_Dictionary_List.Add(new_Answer.AnswerID, item);
                            answer_Guids.Add(new_Answer.AnswerID);
                            //question_Types.Add(item.Type.ToString());
                        }

                    }

                }
                i++;
            }

        }

        private void readyViewObject(List<Abstract_Iter_able> view_Objects, List<Guid> answer_Guids, Dictionary<Guid, object> question_Dictionary_List, Dictionary<Guid, string> question_Answer_Dictionary_List, Dictionary<Guid, string> question_Options_Dictionary_List)
        {

            int i = 0;
            Dictionary<int, string> type_Dictionary = new Dictionary<int, string> {
                { 0, "primary" },
                { 1, "warning" },
                { 2, "success" },
                { 3, "danger" },
                { 4, "info" }
            };
            foreach (Abstract_Iter_able view_Obj in view_Objects)
            {


                Dictionary<string, object> variable_Items = view_Obj.getListOfVariables();
                List<string> typelist = view_Obj.getListofTypes();
                int j = 0;
                foreach (KeyValuePair<string, object> variable in variable_Items)
                {
                    Guid variable_Guid = Guid.NewGuid();
                    question_Answer_Dictionary_List.Add(variable_Guid, variable.Value.ToString());
                    question_Options_Dictionary_List.Add(variable_Guid, type_Dictionary[i % 5]);
                    question_Dictionary_List.Add(variable_Guid, variable.Key);
                    answer_Guids.Add(variable_Guid);
                    //question_Types.Add(typelist[j]);
                    j++;
                }
                i++;
            }

        }

        [WebMethod(EnableSession = true)]
        public List<string> generate_Objectable_Html()
        {
            List<Guid> answer_Guids = new List<Guid>();
            Dictionary<Guid, object> question_Dictionary_List = new Dictionary<Guid, object>();
            Dictionary<Guid, string> question_Answer_Dictionary_List = new Dictionary<Guid, string>();
            Dictionary<Guid, string> question_Options_Dictionary_List = new Dictionary<Guid, string>();

            List<Guid> review_Answer_Guids = new List<Guid>();
            Dictionary<Guid, object> review_Question_Dictionary_List = new Dictionary<Guid, object>();
            Dictionary<Guid, string> review_Question_Answer_Dictionary_List = new Dictionary<Guid, string>();
            Dictionary<Guid, string> review_Question_Options_Dictionary_List = new Dictionary<Guid, string>();

            string view_Type = (string)HttpContext.Current.Session["view_Type"];
            List<Abstract_Iter_able> formObj_Initial = (List<Abstract_Iter_able>)HttpContext.Current.Session["formObj_Initial"];
            Abstract_Iter_able evalObj_Initial = (Abstract_Iter_able)HttpContext.Current.Session["evalObj_Initial"];

            List<string> html_Strings = new List<string>();
            CNTRDesignObjects.Helpers.HTML_Generator.Html_Generator form_html_Generator = new CNTRDesignObjects.Helpers.HTML_Generator.Html_Generator();
            string left = "";
            string right = "";

            switch (view_Type)
            {
                case "Edit":
                    var submission_Type = formObj_Initial[0] as JCF_Submission;
                    var flow_Type = formObj_Initial[0] as Flow_Rec;
                    if (submission_Type != null)
                    {
                        List<JCF_Submission> form_List = new List<JCF_Submission>();
                        foreach (JCF_Submission obj in formObj_Initial)
                        {
                            form_List.Add(obj);
                        }
                        readyViewObject(form_List, answer_Guids, question_Dictionary_List, question_Answer_Dictionary_List, question_Options_Dictionary_List);
                    }
                    else if (flow_Type != null)
                    {
                        List<Flow_Rec> form_List = new List<Flow_Rec>();
                        foreach (Flow_Rec obj in formObj_Initial)
                        {
                            form_List.Add(obj);
                        }
                        readyViewObject(form_List, answer_Guids, question_Dictionary_List, question_Answer_Dictionary_List, question_Options_Dictionary_List);
                    }
                    else
                    {
                        readyViewObject(formObj_Initial, answer_Guids, question_Dictionary_List, question_Answer_Dictionary_List, question_Options_Dictionary_List);
                    }

                    left = form_html_Generator.generate_Html(answer_Guids, question_Dictionary_List, question_Answer_Dictionary_List, question_Options_Dictionary_List, "Edit");

                    break;

                case "Evaluation":
                    readyViewObject(formObj_Initial, (JCF_Submission)evalObj_Initial, answer_Guids, question_Dictionary_List, question_Answer_Dictionary_List, question_Options_Dictionary_List
                                    , review_Answer_Guids, review_Question_Dictionary_List, review_Question_Answer_Dictionary_List, review_Question_Options_Dictionary_List);
                    left = form_html_Generator.generate_Html(answer_Guids, question_Dictionary_List, question_Answer_Dictionary_List, question_Options_Dictionary_List, "ReadOnly");
                    right = form_html_Generator.generate_Html(review_Answer_Guids, review_Question_Dictionary_List, review_Question_Answer_Dictionary_List, review_Question_Options_Dictionary_List, "Edit");


                    break;

                case "ReadOnly":
                    submission_Type = formObj_Initial[0] as JCF_Submission;
                    flow_Type = formObj_Initial[0] as Flow_Rec;
                    if (submission_Type != null)
                    {
                        List<JCF_Submission> form_List = new List<JCF_Submission>();
                        foreach (JCF_Submission obj in formObj_Initial)
                        {
                            form_List.Add(obj);
                        }
                        readyViewObject(form_List, answer_Guids, question_Dictionary_List, question_Answer_Dictionary_List, question_Options_Dictionary_List);
                    }
                    else if (flow_Type != null)
                    {
                        List<Flow_Rec> form_List = new List<Flow_Rec>();
                        foreach (Flow_Rec obj in formObj_Initial)
                        {
                            form_List.Add(obj);
                        }
                        readyViewObject(form_List, answer_Guids, question_Dictionary_List, question_Answer_Dictionary_List, question_Options_Dictionary_List);
                    }
                    else
                    {
                        readyViewObject(formObj_Initial, answer_Guids, question_Dictionary_List, question_Answer_Dictionary_List, question_Options_Dictionary_List);
                    }
                    left = form_html_Generator.generate_Html(answer_Guids, question_Dictionary_List, question_Answer_Dictionary_List, question_Options_Dictionary_List, "ReadOnly");
                    break;
                default:
                    submission_Type = formObj_Initial[0] as JCF_Submission;
                    flow_Type = formObj_Initial[0] as Flow_Rec;
                    if (submission_Type != null)
                    {
                        List<JCF_Submission> form_List = new List<JCF_Submission>();
                        foreach (JCF_Submission obj in formObj_Initial)
                        {
                            form_List.Add(obj);
                        }
                        readyViewObject(form_List, answer_Guids, question_Dictionary_List, question_Answer_Dictionary_List, question_Options_Dictionary_List);
                        left = form_html_Generator.generate_Html(answer_Guids, question_Dictionary_List, question_Answer_Dictionary_List, question_Options_Dictionary_List, "ReadOnly");
                    }
                    else if (flow_Type != null)
                    {
                        List<Flow_Rec> form_List = new List<Flow_Rec>();
                        foreach (Flow_Rec obj in formObj_Initial)
                        {
                            form_List.Add(obj);
                        }
                        readyViewObject(form_List, answer_Guids, question_Dictionary_List, question_Answer_Dictionary_List, question_Options_Dictionary_List);
                        left = form_html_Generator.generate_Html(answer_Guids, question_Dictionary_List, question_Answer_Dictionary_List, question_Options_Dictionary_List, "ReadOnly");
                    }
                    else
                    {
                        readyViewObject(formObj_Initial, answer_Guids, question_Dictionary_List, question_Answer_Dictionary_List, question_Options_Dictionary_List);
                        left = form_html_Generator.generate_Html(answer_Guids, question_Dictionary_List, question_Answer_Dictionary_List, question_Options_Dictionary_List, "ReadOnly");
                    }
                    break;

            }
            left = left.Remove(left.Length - 6, 6);
            left = left.Remove(0, 5);
            right = right.Remove(right.Length - 6, 6);
            right = right.Remove(0, 5);
            HttpContext.Current.Session["answer_Guids"] = answer_Guids;
            html_Strings.Add(left);
            html_Strings.Add(right);

            return html_Strings;
        }
        [Serializable]
        public class Save_Object
        {
            public string ID { get; set; }

            public object Value { get; set; }
        }
        [WebMethod(EnableSession = true)]
        public void saveObjectable(List<Save_Object> saveObjects)
        {
            EventLog.WriteEntry("ICSNET", "Entered Save Objectable");
            JICSBaseFacade<JCF_Answer> facade = new JICSBaseFacade<JCF_Answer>();
            foreach (Save_Object obj in saveObjects)
            {
                JCF_Answer answer = (JCF_Answer)facade.GetQuery().Where(x => x.AnswerID.ToString() == obj.ID).FirstOrDefault();
                answer.AnswerValue = (string)obj.Value;
                EventLog.WriteEntry("ICSNET", "Answer Value: " + answer.AnswerValue);
                facade.Save(answer);
                facade.Flush();
            }
            EventLog.WriteEntry("ICSNET", "Exited Save Objectable");
        }

        [WebMethod(EnableSession = true)]
        public void testCalendar(Save_Object saveObject)
        {
            EventLog.WriteEntry("ICSNET", saveObject.Value.GetType().ToString());
        }
        [WebMethod(EnableSession = true)]
        public List<string> GetSortable()
        {
            List<string> resultList = new List<string>(); ;

            return resultList;
        }

    }
}
