using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace QLCongViecMVC.Filters
{
    public class CheckLoginAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var session = context.HttpContext.Session;
            if (string.IsNullOrEmpty(session.GetString("NguoiDungID")))
            {
                context.Result = new RedirectToActionResult("DangNhap", "NguoiDung", null);
            }
            base.OnActionExecuting(context);
        }
    }
}
