using System.Net.Mime;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc;

namespace ParameterTestWebApplication.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class MyController : ControllerBase
    {
        [Route("form")]
        [Consumes("multipart/form-data")]
        [Produces(MediaTypeNames.Text.Plain)]
        [HttpPost]
        public IActionResult PostForm([FromForm] FormRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            return Ok(request.ToString());
        }

        [Route("body")]
        [Consumes(MediaTypeNames.Application.Json)]
        [Produces(MediaTypeNames.Text.Plain)]
        [HttpPost]
        public IActionResult PostBody([FromBody] BodyRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            return Ok(request.ToString());
        }

        [Route("query")]
        [Produces(MediaTypeNames.Text.Plain)]
        [HttpGet]
        public IActionResult GetQuery([FromQuery] QueryRequest2 request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            return Ok(request.ToString());
        }
    }
    
    public record FormRequest(
        [FromForm(Name = "id")] string Id,
        [FromForm(Name = "first_name")] string FirstName,
        [FromForm(Name = "last_name")] string LastName);

    public record BodyRequest(
        [property: JsonPropertyName("id")] string Id,
        [property: JsonPropertyName("first_name")] string FirstName,
        [property: JsonPropertyName("last_name")] string LastName);

    public record QueryRequest(
        [FromQuery(Name = "id")] string Id,
        [FromQuery(Name = "first_name")] string FirstName,
        [FromQuery(Name = "last_name")] string LastName);


    public class QueryRequest2
    {
        [FromQuery(Name = "id")] 
        public string Id { get; set; }
        [FromQuery(Name = "first_name")] 
        public string FirstName { get; set; }
        [FromQuery(Name = "last_name")]
        public string LastName { get; set; }

        public override string ToString()
        {
            return $"QueryRequest{{ Id = {Id}, FirstName = {FirstName}, LastName = {LastName} }}";
        }
    }

}