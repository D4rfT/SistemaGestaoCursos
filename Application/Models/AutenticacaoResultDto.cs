namespace Application.Models
{
    public class AutenticacaoResultDto
    {
        public string Token { get; set; }
        public DateTime ExpiraEm { get; set; }
        public string NomeUsuario { get; set; }
        public string Email { get; set; }
        public string Role { get; set; }
    }
}