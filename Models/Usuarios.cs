namespace WebApi.Models;

public class Usuarios
{
    public int ID_USUARIO { get; set; }
    public string? NOME { get; set; }
    public string? SOBRENOME { get; set; }
    public string? USUARIO { get; set; }
    public string? SENHA { get; set; }
    public string? EMAIL { get; set; }
    public DateTime DATA_CADASTRO { get; set; }

    public string NomeCompleto()
    {
        return $"{NOME} {SOBRENOME}";
    }
}
