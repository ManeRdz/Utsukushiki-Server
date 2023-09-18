namespace server.Models.Dtos
{
    public class RegisterUserDto
    {
        public string UserUsername { get; set; } = null!;

        public string UserEmail { get; set; } = null!;

        public string UserPassword { get; set; } = null!;
    }
}
