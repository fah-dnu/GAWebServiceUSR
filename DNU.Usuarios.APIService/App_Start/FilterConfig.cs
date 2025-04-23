using System.Web;
using System.Web.Mvc;

namespace DNU.Usuarios.APIService
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }
    }
}
