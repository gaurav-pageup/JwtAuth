namespace JWTAuth.WebApi.Models
{
    public class Employee
    {
        public int Id { get; set; }
        public int EmployeeID { get; set; }
        public string? EmployeeName { get; set; }
        public string? JobTitle { get; set; }
        public string? Gender { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime ModifiedDate { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
    }
}
