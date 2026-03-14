namespace CitizenApi.Models;

public class CreateCitizenRequest
{
    public string FirstName { get; set; } = "";
    public string LastName { get; set; } = "";
    public string CI { get; set; } = "";
}