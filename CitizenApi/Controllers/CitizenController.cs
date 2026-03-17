using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Json;
using CitizenApi.Models;

namespace CitizenApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CitizenController : ControllerBase
{
    private static List<Citizen> citizens = new List<Citizen>();

    private readonly IHttpClientFactory httpClientFactory;
    private readonly IConfiguration configuration;


    public CitizenController(
        IHttpClientFactory httpClientFactory,
        IConfiguration configuration)
    {
        this.httpClientFactory = httpClientFactory;
        this.configuration = configuration;
    }

    [HttpGet]
    public ActionResult<List<Citizen>> GetAll()
    {
        return Ok(citizens);
    }

    [HttpGet("{ci}")]
    public ActionResult<Citizen> GetByCi(string ci)
    {
        var citizenFound = citizens.FirstOrDefault(c => c.CI == ci);

        if (citizenFound == null)
        {
            return NotFound("Citizen not found");
        }

        return Ok(citizenFound);
    }

    [HttpPost]
    public async Task<ActionResult<Citizen>> Create([FromBody] CreateCitizenRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.FirstName) ||
            string.IsNullOrWhiteSpace(request.LastName) ||
            string.IsNullOrWhiteSpace(request.CI))
        {
            return BadRequest("FirstName, LastName and CI are required");
        }

        var ciExists = citizens.Any(c => c.CI == request.CI);

        if (ciExists)
        {
            return BadRequest("CI already exists");
        }

        string[] bloodGroups =
        {
            "A+",
            "A-",
            "B+",
            "B-",
            "AB+",
            "AB-",
            "O+",
            "O-"
        };

        Random random = new Random();

        string personalAsset = await GetRandomPersonalAsset();

        Citizen newCitizen = new Citizen
        {
            FirstName = request.FirstName,
            LastName = request.LastName,
            CI = request.CI,
            BloodGroup = bloodGroups[random.Next(bloodGroups.Length)],
            PersonalAsset = personalAsset
        };

        citizens.Add(newCitizen);


        return CreatedAtAction(nameof(GetByCi), new { ci = newCitizen.CI }, newCitizen);
    }

    [HttpPut("{ci}")]
    public ActionResult<Citizen> Update(string ci, [FromBody] UpdateCitizenRequest request)
    {
        var citizenFound = citizens.FirstOrDefault(c => c.CI == ci);

        if (citizenFound == null)
        {
            return NotFound("Citizen not found");
        }

        if (string.IsNullOrWhiteSpace(request.FirstName) ||
            string.IsNullOrWhiteSpace(request.LastName))
        {
            return BadRequest("FirstName and LastName are required");
        }

        citizenFound.FirstName = request.FirstName;
        citizenFound.LastName = request.LastName;

        return Ok(citizenFound);
    }

    [HttpDelete("{ci}")]
    public IActionResult Delete(string ci)
    {
        var citizenFound = citizens.FirstOrDefault(c => c.CI == ci);

        if (citizenFound == null)
        {
            return NotFound("Citizen not found");
        }

        citizens.Remove(citizenFound);

        return Ok("Citizen deleted");
    }

    private async Task<string> GetRandomPersonalAsset()
    {
        try
        {
            string url = configuration["ExternalApi:ObjectsUrl"] ?? "";

            var client = httpClientFactory.CreateClient();


            var objects = await client.GetFromJsonAsync<List<ExternalObject>>(url);

            if (objects == null || objects.Count == 0)
            {
                return "Unknown asset";
            }

            Random random = new Random();
            var selectedObject = objects[random.Next(objects.Count)];

            if (string.IsNullOrWhiteSpace(selectedObject.Name))
            {
                return "Unknown asset";
            }

            return selectedObject.Name;
        }
        catch
        {
            return "Unknown asset";
        }
    }
}