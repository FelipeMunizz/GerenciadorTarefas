using System.ComponentModel.DataAnnotations;

namespace WebApi.Models;

public class UsuariosProjeto
{
    [Key]
    public int IdUsuarioProjeto { get; set; }
    public int IdUsuario { get; set; }
    public int IdProjeto { get; set; }
    public bool Responsavel { get; set; }
    public virtual Usuarios Usuario { get; set; }
    public virtual Projetos Projeto { get; set; }
}
