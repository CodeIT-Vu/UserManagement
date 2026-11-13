using System.Collections.Generic;
using System.Web.Mvc;
using UserManagement.Web.Constants;
using UserManagement.Web.Exceptions;

namespace UserManagement.Web.Filters
{
    /// <summary>
    /// Chặn action khi dữ liệu đầu vào không hợp lệ.
    /// </summary>
    public class ValidateModelStateAttribute : ActionFilterAttribute
    {
        /// <summary>
        /// Gom lỗi ModelState theo trường và dừng action nếu dữ liệu không hợp lệ.
        /// </summary>
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            ModelStateDictionary modelState = filterContext.Controller.ViewData.ModelState;
            if (modelState.IsValid)
            {
                return;
            }

            Dictionary<string, string[]> errors = new Dictionary<string, string[]>();

            // Giữ tên trường để trình duyệt đặt lỗi cạnh đúng ô nhập liệu.
            foreach (KeyValuePair<string, ModelState> field in modelState)
            {
                List<string> messages = new List<string>();

                foreach (ModelError error in field.Value.Errors)
                {
                    messages.Add(string.IsNullOrEmpty(error.ErrorMessage)
                        ? ErrorMessages.InvalidRequest
                        : error.ErrorMessage);
                }

                if (messages.Count > 0)
                {
                    errors.Add(field.Key, messages.ToArray());
                }
            }

            throw new RequestValidationException(ErrorMessages.InvalidRequest, errors);
        }
    }
}
