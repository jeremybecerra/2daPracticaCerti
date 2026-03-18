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
    private readonly ILogger<CitizenController> logger;

    public CitizenController(
        ExternalObjectService externalObjectService,
        CitizenCsvService citizenCsvService,
        ILogger<CitizenController> logger)
    {
        this.externalObjectService = externalObjectService;
        this.citizenCsvService = citizenCsvService;
        this.logger = logger;
    }

    [HttpGet]
    public ActionResult<List<Citizen>> GetAll()
    {
        try
        {
            var citizens = citizenCsvService.ReadCitizens();
            logger.LogInformation("Returned all citizens");
            return Ok(citizens);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error getting all citizens");
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpGet("{ci}")]
    public ActionResult<Citizen> GetByCi(string ci)
    {
        try
        {
            var citizens = citizenCsvService.ReadCitizens();
            var citizenFound = citizens.FirstOrDefault(c => c.CI == ci);

            if (citizenFound == null)
            {
                logger.LogWarning("Citizen not found with CI {CI}", ci);
                return NotFound("Citizen not found");
            }

            logger.LogInformation("Returned citizen with CI {CI}", ci);
            return Ok(citizenFound);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error getting citizen with CI {CI}", ci);
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpPost]
    public async Task<ActionResult<Citizen>> Create([FromBody] CreateCitizenRequest request)
    {
        try
        {
            var citizens = citizenCsvService.ReadCitizens();

            if (string.IsNullOrWhiteSpace(request.FirstName) ||
                string.IsNullOrWhiteSpace(request.LastName) ||
                string.IsNullOrWhiteSpace(request.CI))
            {
                logger.LogWarning("Invalid citizen creation request");
                return BadRequest("FirstName, LastName and CI are required");
            }

            var ciExists = citizens.Any(c => c.CI == request.CI);

            if (ciExists)
            {
                logger.LogWarning("Citizen creation failed because CI already exists: {CI}", request.CI);
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

            logger.LogInformation("Citizen created with CI {CI}", newCitizen.CI);

            return CreatedAtAction(nameof(GetByCi), new { ci = newCitizen.CI }, newCitizen);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error creating citizen");
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpPut("{ci}")]
    public ActionResult<Citizen> Update(string ci, [FromBody] UpdateCitizenRequest request)
    {
        try
        {
            var citizens = citizenCsvService.ReadCitizens();
            var citizenFound = citizens.FirstOrDefault(c => c.CI == ci);

            if (citizenFound == null)
            {
                logger.LogWarning("Citizen not found for update with CI {CI}", ci);
                return NotFound("Citizen not found");
            }

            if (string.IsNullOrWhiteSpace(request.FirstName) ||
                string.IsNullOrWhiteSpace(request.LastName))
            {
                logger.LogWarning("Invalid update request for CI {CI}", ci);
                return BadRequest("FirstName and LastName are required");
            }

            citizenFound.FirstName = request.FirstName;
            citizenFound.LastName = request.LastName;

            citizenCsvService.SaveCitizens(citizens);

            logger.LogInformation("Citizen updated with CI {CI}", ci);

            return Ok(citizenFound);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error updating citizen with CI {CI}", ci);
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpDelete("{ci}")]
    public IActionResult Delete(string ci)
    {
        try
        {
            var citizens = citizenCsvService.ReadCitizens();
            var citizenFound = citizens.FirstOrDefault(c => c.CI == ci);

            if (citizenFound == null)
            {
                logger.LogWarning("Citizen not found for delete with CI {CI}", ci);
                return NotFound("Citizen not found");
            }

            citizens.Remove(citizenFound);
            citizenCsvService.SaveCitizens(citizens);

            logger.LogInformation("Citizen deleted with CI {CI}", ci);

            return Ok("Citizen deleted");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error deleting citizen with CI {CI}", ci);
            return StatusCode(500, "Internal server error");
        }
    }
}