namespace WebApi.Helpers.Interfaces;

public interface IEmailHelpers
{
    bool Enviar(string email, string assunto, string mensagem);
}
