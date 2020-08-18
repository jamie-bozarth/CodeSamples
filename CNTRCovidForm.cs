using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Jenzabar.Common.Web.UI.Controls;
using Jenzabar.Portal.Framework;
using Jenzabar.Portal.Framework.Security.Authorization;
using Jenzabar.Portal.Framework.Web.UI;
using Jenzabar.ICS.Web.Portlets.LoginPortlet;
using Jenzabar.Common.ApplicationBlocks.ExceptionManagement;
using ICS_NHibernate;
using Jenzabar.Portal.Framework.NHibernateFWK;
using System.Web;

namespace CNTRCovidForm
{
    public class CNTRCovidForm : PortletBase
    {
        protected override PortletViewBase GetCurrentScreen()
        {
            PortletViewBase screen = null;
            switch (this.CurrentPortletScreenName)
            {
                default:
                    screen = this.LoadPortletView("ICS/CNTRCovidForm/Views/Default_View.ascx");
                    break;
            }
            return screen;
        }
    }
}
