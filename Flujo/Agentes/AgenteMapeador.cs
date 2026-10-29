using Abstracciones.Agente;
using Abstracciones.Clientes;

namespace Flujo.Agentes;

/// <summary>
/// Mapeo manual entidad -> DTO. Se hace a mano a propósito: es corto, explícito, se puede depurar
/// y no depende de librerías (AutoMapper v15 en adelante tiene licencia comercial).
/// </summary>
internal static class AgenteMapeador
{
    public static AgenteDto ADto(Agente c) =>
        new(c.Id, c.Nombre, c.Correo, c.Telefono,c.CapMaxTicket , c.FechaRegistro, c.Activo);
}
