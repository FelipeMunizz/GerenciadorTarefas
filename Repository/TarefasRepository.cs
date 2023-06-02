using Microsoft.EntityFrameworkCore;
using WebApi.Data;
using WebApi.Models;
using WebApi.Repository.Interfaces;

namespace WebApi.Repository;

public class TarefasRepository : ITarefasRepository
{
    private readonly AppDbContext _context;

    public TarefasRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task Add(Tarefas tarefas)
    {
        _context.Tarefas.Add(tarefas);
        await _context.SaveChangesAsync();
    }

    public async Task Delete(int idTarefas)
    {
        Tarefas tarefa = await ObterTarefa(idTarefas);
        _context.Tarefas.Remove(tarefa);
        await _context.SaveChangesAsync();
    }

    public async Task<List<Tarefas>> ListarTarefas(int idProjeto)
    {
        return await _context.Tarefas.Where(x => x.IdProjeto == idProjeto).ToListAsync();
    }

    public async Task<Tarefas> ObterTarefa(int idTarefa)
    {
        return await _context.Tarefas.SingleOrDefaultAsync(x => x.IdTarefa == idTarefa);
    }

    public async Task Update(Tarefas tarefas)
    {
        _context.Tarefas.Update(tarefas);
        await _context.SaveChangesAsync();
    }
}
