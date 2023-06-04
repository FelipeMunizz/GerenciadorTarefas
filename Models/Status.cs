using System.ComponentModel.DataAnnotations;

namespace WebApi.Models;

public class Status
{
    public int StatusId { get; set; }
    public string? NomeStatus { get; set; }
}
