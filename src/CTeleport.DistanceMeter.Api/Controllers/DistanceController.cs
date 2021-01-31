namespace CTeleport.DistanceMeter.Api.Controllers
{
    using System.Threading;
    using System.Threading.Tasks;
    using Application.Services;
    using Domain.Models;
    using Microsoft.AspNetCore.Mvc;

    [ApiController]
    [Route("api/[controller]")]
    public class DistanceController : ControllerBase
    {
        private readonly IDistanceMeasurementService service;

        public DistanceController(IDistanceMeasurementService service)
        {
            this.service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetAsync([FromQuery] IataCouple couple, CancellationToken token)
        {
            var distance = await service.GetDistanceAsync(couple, token);
            if (distance.IsError)
            {
                return BadRequest(distance.Error.Message);
            }

            return Ok(distance.Data);
        }
    }
}