﻿using JIYUWU.Core.ObjectActionValidator;
using Microsoft.AspNetCore.Mvc.Filters;

namespace UMES.Core.Filters
{
    public class ActionExecuteFilter : IActionFilter
    {

        public void OnActionExecuting(ActionExecutingContext context)
        {
            //验证方法参数
            context.ActionParamsValidator();
        }
        public void OnActionExecuted(ActionExecutedContext context)
        {

        }
    }
}