using CitizenApi.Models;

namespace CitizenApi.Services;

public class CitizenCsvService
{
    private readonly IConfiguration configuration;

    public CitizenCsvService(IConfiguration configuration)
    {
        this.configuration = configuration;
    }

    public List<Citizen> ReadCitizens()
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

        return citizens;
    }

    public void SaveCitizens(List<Citizen> citizens)
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
    }

    private string GetCsvFilePath()
    {
        string relativePath = configuration["CsvSettings:FilePath"] ?? "DataFiles/citizens.csv";
        string fullPath = Path.Combine(Directory.GetCurrentDirectory(), relativePath);

        string? folderPath = Path.GetDirectoryName(fullPath);

        if (!string.IsNullOrWhiteSpace(folderPath))
        {
            Directory.CreateDirectory(folderPath);
        }

        if (!File.Exists(fullPath))
        {
            File.Create(fullPath).Dispose();
        }

        return fullPath;
    }

    private string CleanValue(string value)
    {
        return value.Replace(",", " ");
    }
}