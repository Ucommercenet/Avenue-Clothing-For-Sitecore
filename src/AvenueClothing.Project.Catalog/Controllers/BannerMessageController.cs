using System.Web;
using System.Web.Mvc;
using AvenueClothing.Foundation.MvcExtensions;
using AvenueClothing.Project.Catalog.ViewModels;
using Sitecore.Analytics;
using Sitecore.Data.Items;
using Sitecore.Mvc.Presentation;
using Sitecore.Web.UI.WebControls;

namespace AvenueClothing.Project.Catalog.Controllers
{
    public class BannerMessageController : BaseController
    {
        public ActionResult Rendering()
        {
            var item = new BannerMessageViewModel();

            var dataSourceId = RenderingContext.Current.Rendering.DataSource;
            var dataSource = Sitecore.Context.Database.GetItem(dataSourceId);
            item.Title = RefactorTitle(FieldRenderer.Render(dataSource, "Title"), dataSource);
            item.Message = new HtmlString(FieldRenderer.Render(dataSource, "Message"));

            item.Image = new HtmlString(FieldRenderer.Render(dataSource, "Image"));

            return View(item);
        }

        private HtmlString RefactorTitle(string title, Item dataSource)
        {
            switch (dataSource.Name)
            {
                case "Abandoned Checkout":
                    title = RefactorAbandonedCheckoutTitle(title);
                    break;
                default:
                    break;
            }

            return new HtmlString(title);
        }

        private static string RefactorAbandonedCheckoutTitle(string title)
        {
            if (Tracker.Current.Session.CustomData.ContainsKey("FirstName"))
            {
                title = title.Replace("&lt;first name&gt;",
                    Tracker.Current.Session.CustomData["FirstName"].ToString());

            }
            return title;
        }
    }
}