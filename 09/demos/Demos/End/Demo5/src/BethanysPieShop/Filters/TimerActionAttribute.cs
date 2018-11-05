using System.Diagnostics;
using System.Globalization;
using System.Text;
using Microsoft.AspNetCore.Mvc.Filters;

namespace BethanysPieShop.Filters
{
    public class TimerAction : ActionFilterAttribute
    {
        private readonly Stopwatch _stopWatch = new Stopwatch();
        
        public TimerAction()
        {
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            _stopWatch.Reset();
            _stopWatch.Start();
        }

        public override void OnActionExecuted(ActionExecutedContext context)
        {
            _stopWatch.Stop();
        }

        public override void OnResultExecuted(ResultExecutedContext context)
        {
            base.OnResultExecuted(context);
            _stopWatch.Stop();
            var elapsed = Encoding.ASCII.GetBytes(_stopWatch.Elapsed.TotalMilliseconds.ToString(CultureInfo.InvariantCulture));
            //context.HttpContext.Response.Body.Write(elapsed, 0, elapsed.Length);
        }
    }
}
