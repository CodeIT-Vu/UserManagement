using System.Collections.Generic;
using System.Web.Mvc;
using UserManagement.Web.Constants;
using UserManagement.Web.Exceptions;

namespace UserManagement.Web.Filters
{
    public class ValidateRequestAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            ModelStateDictionary modelState = filterContext.Controller.ViewData.ModelState;
            if (modelState.IsValid)
            {
                return;
            }

            Dictionary<string, string[]> errors = new Dictionary<string, string[]>();

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
