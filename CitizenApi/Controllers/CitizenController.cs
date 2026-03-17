using Microsoft.AspNetCore.Mvc;
using CitizenApi.Models;
using CitizenApi.Services;

namespace CitizenApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CitizenController : ControllerBase
{
    private readonly ExternalObjectService externalObjectService;
    private readonly CitizenCsvService citizenCsvService;

    public CitizenController(
        ExternalObjectService externalObjectService,
        CitizenCsvService citizenCsvService)
    {
        this.externalObjectService = externalObjectService;
        this.citizenCsvService = citizenCsvService;
    }

    [HttpGet]
    public ActionResult<List<Citizen>> GetAll()
    {
        var citizens = citizenCsvService.ReadCitizens();
        return Ok(citizens);
    }

    [HttpGet("{ci}")]
    public ActionResult<Citizen> GetByCi(string ci)
    {
        var citizens = citizenCsvService.ReadCitizens();
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
        var citizens = citizenCsvService.ReadCitizens();

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
        string personalAsset = await externalObjectService.GetRandomPersonalAssetAsync();

        Citizen newCitizen = new Citizen
        {
            FirstName = request.FirstName,
            LastName = request.LastName,
            CI = request.CI,
            BloodGroup = bloodGroups[random.Next(bloodGroups.Length)],
            PersonalAsset = personalAsset
        };

        citizens.Add(newCitizen);
        citizenCsvService.SaveCitizens(citizens);

        return CreatedAtAction(nameof(GetByCi), new { ci = newCitizen.CI }, newCitizen);
    }

    [HttpPut("{ci}")]
    public ActionResult<Citizen> Update(string ci, [FromBody] UpdateCitizenRequest request)
    {
        var citizens = citizenCsvService.ReadCitizens();
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

        citizenCsvService.SaveCitizens(citizens);

        return Ok(citizenFound);
    }

    [HttpDelete("{ci}")]
    public IActionResult Delete(string ci)
    {
        var citizens = citizenCsvService.ReadCitizens();
        var citizenFound = citizens.FirstOrDefault(c => c.CI == ci);

        if (citizenFound == null)
        {
            return NotFound("Citizen not found");
        }

        citizens.Remove(citizenFound);
        citizenCsvService.SaveCitizens(citizens);

        return Ok("Citizen deleted");
    }
}