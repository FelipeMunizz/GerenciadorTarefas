namespace WebApi.DTOs;

public class AlterarSenhaDTO
{
    public string? Usuario { get; set; }
    public string? SenhaAtual { get; set; }
    public string? NovaSenha { get; set; }
    public string? ConfirmarSenha { get; set; }
}
