using Abstracciones.Clientes;
using Abstracciones.Comunes;
using System;
using System.Collections.Generic;
using System.Text;

namespace Abstracciones.Agente
{
    public interface IAgenteFlujo
    {
        Task<ResultadoPaginado<AgenteDto>> ListarAsync(AgenteFiltro filtro, CancellationToken ct);
        Task<ClienteDto> ObtenerAsync(int id, CancellationToken ct);
        Task<ClienteDto> CrearAsync(AgenteCrearDto dto, CancellationToken ct);
        Task ActualizarAsync(int id, AgenteActualizarDto dto, CancellationToken ct);
        Task EliminarAsync(int id, CancellationToken ct);
        Task<List<ClienteDto>> BuscarPorNombreSpAsync(string? nombre, CancellationToken ct);
    }
}
