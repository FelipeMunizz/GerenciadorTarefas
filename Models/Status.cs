using System.ComponentModel.DataAnnotations;

namespace WebApi.Models;

public class Status
{
    [Key]
    public int IdStatus { get; set; }
    public string? NomeStatus { get; set; }
}
