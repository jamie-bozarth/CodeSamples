using CNTRDesignObjects.Object_ables;
using ICS_NHibernate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CNTRCovidForm.lib
{
    [Serializable]
    public class Covid_Formable : Form_able
    {
        public Covid_Formable()
        {
            Name = "Covid Reporting and Assesment Form";
            review_Items = new List<Guid> {};  // HousingRequest
            list_View_Items = new List<Guid> {};  // HousingRequest
            facade = new CNTR_CovidForm_Rec();
            list_Column_Headers = new List<string>() { "UserID", "SubmittedDate", "OnCampus","Symptomatic" };
            list_Header_Items = new Dictionary<string, object>(){
                {"Current Year:", DateTime.Today.Year }
            };
            Index = new List<string> { };
            Index_Value = new List<object> {};
        }
    }
}