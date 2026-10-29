using Abstracciones.Agente;
using Abstracciones.Clientes;
using Abstracciones.Comunes;
using Abstracciones.Excepciones;
using DA.Comunes;
using DA.Contexto;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace DA.Agentes;

public class AgenteDA : IAgenteDA
{
    private readonly SistemaSoporteContexto _ctx;

    public AgenteDA(SistemaSoporteContexto ctx) => _ctx = ctx;

    public async Task<ResultadoPaginado<Agente>> BuscarAsync(AgenteFiltro filtro, CancellationToken ct)
    {
        // AsNoTracking: solo leemos, EF no necesita vigilar los objetos.
        IQueryable<Agente> consulta = _ctx.Agentes.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(filtro.Q))
        {
            var texto = filtro.Q.Trim();
            consulta = consulta.Where(c =>
                c.Nombre.Contains(texto) ||
                c.Correo.Contains(texto));
                
        }

        var total = await consulta.CountAsync(ct);

        // Orden estable (Nombre, luego Id) para que la paginación no repita ni se salte filas.
        var items = await consulta
            .OrderBy(c => c.Nombre).ThenBy(c => c.Id)
            .Skip((filtro.Pagina - 1) * filtro.Tamano)
            .Take(filtro.Tamano)
            .ToListAsync(ct);

        return new ResultadoPaginado<Agente>
        {
            Items = items,
            Total = total,
            Pagina = filtro.Pagina,
            Tamano = filtro.Tamano
        };
    }

    public Task<Agente?> ObtenerPorIdAsync(int id, CancellationToken ct) =>
        _ctx.Agentes.AsNoTracking().FirstOrDefaultAsync(c => c.Id == id, ct);

    public Task<Agente?> ObtenerPorCorreoAsync(string correo, CancellationToken ct) =>
        _ctx.Agentes.AsNoTracking().FirstOrDefaultAsync(c => c.Correo == correo, ct);

    public async Task<Agente> AgregarAsync(Agente agente, CancellationToken ct)
    {
        _ctx.Agentes.Add(agente);
        try
        {
            await _ctx.SaveChangesAsync(ct);
        }
        catch (DbUpdateException ex) when (ErroresSql.EsDuplicado(ex))
        {
            throw new ConflictoException($"Ya existe un cliente con el correo '{agente.Correo}'.");
        }
        return agente;
    }

    public async Task<bool> ActualizarAsync(Agente datos, CancellationToken ct)
    {
        // Entidad rastreada: EF detecta SOLO las propiedades que cambiaron y genera un UPDATE mínimo.
        var existente = await _ctx.Agentes.FirstOrDefaultAsync(c => c.Id == datos.Id, ct);
        if (existente is null) return false;

        existente.Nombre = datos.Nombre;
        existente.Correo = datos.Correo;
        existente.CapMaxTicket = datos.CapMaxTicket;
        existente.Activo = datos.Activo;

        try
        {
            await _ctx.SaveChangesAsync(ct);
        }
        catch (DbUpdateException ex) when (ErroresSql.EsDuplicado(ex))
        {
            throw new ConflictoException($"Ya existe otro agente con el correo '{datos.Correo}'.");
        }
        return true;
    }

    public async Task<bool> EliminarAsync(int id, CancellationToken ct)
    {
        var existente = await _ctx.Agentes.FirstOrDefaultAsync(c => c.Id == id, ct);
        if (existente is null) return false;

        _ctx.Agentes.Remove(existente);
        try
        {
            await _ctx.SaveChangesAsync(ct);
        }
        catch (DbUpdateException ex) when (ErroresSql.EsLlaveForanea(ex))
        {
            throw new ConflictoException("El agente tiene tickets asociados y no se puede eliminar.");
        }
        return true;
    }

    public Task<bool> TieneTicketsAsync(int agenteId, CancellationToken ct) =>
        _ctx.Tickets.AnyAsync(t => t.AgenteId == agenteId, ct);

    public async Task<List<Agente>> BuscarPorNombreSpAsync(string? nombre, CancellationToken ct)
    {
        // Parámetro SQL: el valor viaja separado del texto SQL, así NO se puede inyectar código.
        var parametro = new SqlParameter("@Nombre", (object?)nombre ?? DBNull.Value);
        return await _ctx.Agentes
            .FromSqlRaw("EXEC dbo.sp_BuscarCliente @Nombre", parametro)
            .AsNoTracking()
            .ToListAsync(ct);
    }
}
