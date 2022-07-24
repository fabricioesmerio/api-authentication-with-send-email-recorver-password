namespace apiLogin.Models
{
    public class Solicitacao
    {
        public Guid Id { get; set; }
        public Guid UsuarioId { get; set; }
        public int Codigo { get; set; }
        public DateTime Vencimento { get; set; }
    }
}