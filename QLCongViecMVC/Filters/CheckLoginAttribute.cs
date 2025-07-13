using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace QLCongViecMVC.Filters
{
    public class CheckLoginAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var httpContext = context.HttpContext;
            var isLoggedIn = httpContext.Session.GetString("NguoiDungID") != null;

            if (!isLoggedIn)
            {
                context.Result = new RedirectToActionResult("DangNhap", "NguoiDung", null);
            }

            base.OnActionExecuting(context);
        }
    }
}
