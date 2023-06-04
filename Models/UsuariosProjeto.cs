using System.ComponentModel.DataAnnotations;

namespace WebApi.Models;

public class UsuariosProjeto
{
    public int UsuariosProjetoId { get; set; }
    public int UsuariosId { get; set; }
    public int ProjetoId { get; set; }
    public bool Responsavel { get; set; }
    public virtual Usuarios Usuario { get; set; }
    public virtual Projetos Projeto { get; set; }
}
