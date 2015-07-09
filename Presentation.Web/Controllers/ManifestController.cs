using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Services.Description;
using System.Web.UI.WebControls;

namespace OS2Indberetning.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    public class ManifestController : Controller
    {
        // GET: Manifest
        public ContentResult Manifest()
        {
            return new ContentResult()
            {
                ContentType = "text/cache-manifest",
                Content = string.Format(
                    @"CACHE MANIFEST

CACHE:
{0}
{1}
{2}
/fonts/glyphicons-halflings-regular.woff2
/Content/Bootstrap/loading.gif
/Content/Bootstrap/sprite.png
/Content/favicon.ico
/Content/Bootstrap/loading-image.gif
/fonts/fontawesome-webfont.woff?v=4.2.0
/App/ApproveReports/Modals/newApproverModal.html
/App/ApproveReports/Modals/ConfirmDeleteApproverModal.html
/App/ApproveReports/Modals/editApproverModal.html
/App/ApproveReports/Modals/ConfirmDeleteSubstituteModal.html
/App/ApproveReports/Modals/editSubstituteModal.html
/App/Admin/HTML/Substitutes/Modals/newSubstituteModal.html
/App/Admin/HTML/Administration/Modal/RemoveAdminModalTemplate.html
/App/Admin/HTML/Administration/Modal/AddAdminModalTemplate.html
/App/Admin/HTML/Account/ConfirmDeleteAccountTemplate.html
/App/Admin/HTML/Email/AddNewMailNotificationTemplate.html
/App/Admin/HTML/Email/ConfirmDeleteMailNotificationTemplate.html
/App/Admin/HTML/Email/EditMailNotificationTemplate.html
/App/Admin/HTML/Address/ConfirmDeleteAddressTemplate.html
/App/Admin/HTML/Address/EditAddressTemplate.html
/App/Admin/HTML/Address/AddNewAddressTemplate.html
/App/Admin/HTML/Reports/InnerView.html
/App/Admin/HTML/AddressLaundry/AddressLaundryView.html
/App/Admin/HTML/Substitutes/View.html
/App/Admin/HTML/Reports/View.html
/App/Admin/HTML/Administration/View.html
/App/Admin/HTML/OrgUnit/View.html
/App/Admin/HTML/Account/View.html
/App/Admin/HTML/Rate/View.html
/App/Admin/HTML/Email/View.html
/App/Admin/HTML/Address/View.html
/App/Admin/AdminView.html
/App/ApproveReports/SettingsView.html
/App/ApproveReports/View.html
/App/ApproveReports/ApproveReportsView.html
/App/MyReports/View.html
/App/MyReports/MyReportsView.html
/App/Driving/DrivingView.html
/App/Settings/AlternativeWorkAddressTemplate.html
/App/Settings/SettingsView.html
/odata/Addresses/Service.GetMapStart

NETWORK:
*
",
                    Scripts.Url("~/bundles/libraries"),
                    Scripts.Url("~/bundles/angular"),
                    Styles.Url("~/Content/css")
                    )
            };
        }
    }
}