namespace WebApi.Models;

public class Usuarios
{
    public int IdUsuario { get; set; }
    public string? Nome { get; set; }
    public string? Sobrenome { get; set; }
    public string? Usuario { get; set; }
    public string? Senha { get; set; }
    public string? Email { get; set; }
    public DateTime DataCadastro { get; set; }

    public string NomeCompleto()
    {
        return $"{Nome} {Sobrenome}";
    }
}
