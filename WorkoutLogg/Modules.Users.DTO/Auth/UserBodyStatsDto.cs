
namespace Modules.Users.DTO.Auth
{
    public record UserBodyStatsDto
    {
        public int Kg { get; set; }
        public int Cm { get; set; }
        public double Fat { get; set; }
    }
}
