using CitizenApi.Models;

namespace CitizenApi.Services;

public class CitizenCsvService
{
    private readonly IConfiguration configuration;
    private readonly ILogger<CitizenCsvService> logger;

    public CitizenCsvService(IConfiguration configuration, ILogger<CitizenCsvService> logger)
    {
        this.configuration = configuration;
        this.logger = logger;
    }

    public List<Citizen> ReadCitizens()
    {
        try
        {
            string filePath = GetCsvFilePath();
            string[] lines = File.ReadAllLines(filePath);

            List<Citizen> citizens = new List<Citizen>();

            foreach (string line in lines)
            {
                if (string.IsNullOrWhiteSpace(line))
                {
                    continue;
                }

                string[] values = line.Split(',');

                if (values.Length < 5)
                {
                    continue;
                }

                Citizen citizen = new Citizen
                {
                    FirstName = values[0],
                    LastName = values[1],
                    CI = values[2],
                    BloodGroup = values[3],
                    PersonalAsset = values[4]
                };

                citizens.Add(citizen);
            }

            logger.LogInformation("Read {Count} citizens from CSV", citizens.Count);

            return citizens;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error reading citizens CSV");
            throw new Exception("Error reading citizens file.");
        }
    }

    public void SaveCitizens(List<Citizen> citizens)
    {
        try
        {
            string filePath = GetCsvFilePath();
            List<string> lines = new List<string>();

            foreach (Citizen citizen in citizens)
            {
                string line = string.Join(",",
                    CleanValue(citizen.FirstName),
                    CleanValue(citizen.LastName),
                    CleanValue(citizen.CI),
                    CleanValue(citizen.BloodGroup),
                    CleanValue(citizen.PersonalAsset));

                lines.Add(line);
            }

            File.WriteAllLines(filePath, lines);

            logger.LogInformation("Saved {Count} citizens to CSV", citizens.Count);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error writing citizens CSV");
            throw new Exception("Error writing citizens file.");
        }
    }

    private string GetCsvFilePath()
    {
        string fullPath = configuration["Data:Location"] ?? "C:\\TEMP\\2daPracticaCerti\\CitizenApi\\DataFiles\\citizens.csv";

        string? folderPath = Path.GetDirectoryName(fullPath);

        if (!string.IsNullOrWhiteSpace(folderPath))
        {
            Directory.CreateDirectory(folderPath);
        }

        if (!File.Exists(fullPath))
        {
            File.Create(fullPath).Dispose();
            logger.LogInformation("CSV file created at {Path}", fullPath);
        }

        return fullPath;
    }

    private string CleanValue(string value)
    {
        return value.Replace(",", " ");
    }
}