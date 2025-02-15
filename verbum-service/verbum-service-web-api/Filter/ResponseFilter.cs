using Microsoft.AspNetCore.Mvc;
using verbum_service_domain.Utils;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace verbum_service.Filter
{
    public static class ResponseFilter
    {
        public static IActionResult OkOrNoContent<T>(T list, ControllerBase controller)
        {
            return ObjectUtils.IsEmpty(list) ? controller.NoContent() : controller.Ok(list);
        }
    }
}
