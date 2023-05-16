namespace Domain.Models;

public class Employee
{
    public string Id { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string FullName => $"{FirstName} {LastName}";
    public string ProjectId { get; set; }
    public string ProjectName { get; set; }
    public bool Active { get; set; }
    public string TrimmedId => Id?.TrimStart('0');
    public string IdName => $"{TrimmedId} - {FirstName} {LastName}";
    public string CommonName => $"{FirstName?.Split(' ')[0]} {LastName?.Split(' ')[0]}";
    public string IdCommonName => $"{TrimmedId} - {CommonName}";
    public string Position { get; set; }
   
}
