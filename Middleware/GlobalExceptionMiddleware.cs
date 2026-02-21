using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FinancialProfileManagerAPI.Middleware
{
    public class ApiErrorResponse
    {
        public string Message { get; set; }
        public int StatusCode { get; set; }
        public string TraceId { get; set; }
        public Dictionary<string, string[]> Errors { get; set; } = new();
    }

    public class GlobalExceptionMiddleware
    {
        private readonly RequestDelegate _next;

        public GlobalExceptionMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex);
            }
        }

        private static Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/json";

            var response = new ApiErrorResponse
            {
                TraceId = context.TraceIdentifier
            };

            if (exception is ArgumentException argEx)
            {
                context.Response.StatusCode = StatusCodes.Status400BadRequest;
                response.StatusCode = 400;
                response.Message = argEx.Message;
            }
            else if (exception is InvalidOperationException invEx)
            {
                context.Response.StatusCode = StatusCodes.Status409Conflict;
                response.StatusCode = 409;
                response.Message = invEx.Message;
            }
            else
            {
                context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                response.StatusCode = 500;
                response.Message = "Internal server error";
            }

            return context.Response.WriteAsJsonAsync(response);
        }
    }
}
