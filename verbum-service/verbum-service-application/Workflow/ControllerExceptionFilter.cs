﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Net;
using verbum_service_domain.Common.ErrorModel;

namespace verbum_service_application.Workflow
{
    public class ControllerExceptionFilter : IExceptionFilter
    {
        public void OnException(ExceptionContext context)
        {
            ObjectResult? result = null;
            if (context.Exception is BusinessException businessException)
            {
                result = new ObjectResult(new
                {
                    businessException.Errors
                })
                {
                    StatusCode = (int)HttpStatusCode.BadRequest
                };
            }
            else //when unexpected error thrown
            {
                result = new ObjectResult(new
                {
                    context.Exception.Message, // Or a different generic message
                    context.Exception.Source,
                    context.Exception.StackTrace,
                    ExceptionType = context.Exception.GetType().FullName

                })
                {
                    StatusCode = (int)HttpStatusCode.InternalServerError
                };
            }
            // Set the result
            context.Result = result;
        }
    }
}
