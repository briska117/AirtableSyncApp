using AirTableDatabase.DBModels;
using AirTableWebApi.Services.AirTableFields;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AirTableWebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AirTableFieldsController : ControllerBase
    {
        private readonly IAirTableFieldsService airTableFieldsService;

        public AirTableFieldsController(IAirTableFieldsService airTableFieldsService)
        {
            this.airTableFieldsService = airTableFieldsService;
        }

        [HttpPost]
        public async Task<ActionResult> AddAirTableField(AirTableField airTableField)
        {
            try
            {
                await this.airTableFieldsService.AddAirTableField(airTableField);
                return Ok(airTableField);
            }
            catch (Exception)
            {

                throw;
            }
             
        }
    }
}
