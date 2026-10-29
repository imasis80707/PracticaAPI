using Abstracciones.Agentes;
using Abstracciones.Clientes;
using Abstracciones.Comentarios;
using Abstracciones.Etiquetas;
using Abstracciones.Tickets;
using DA.Agentes;
using DA.Clientes;
using DA.Comentarios;
using DA.Contexto;
using DA.Etiquetas;
using DA.Semilla;
using DA.Tickets;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace DA;

/// <summary>
/// La capa DA registra SUS propias dependencias. El API solo llama a AddDatos(...).
/// Así, agregar un módulo nuevo en DA no obliga a tocar el API.
/// ****PROFESOR: **** Tiempos de vida: DbContext y los DA son Scoped (uno por petición HTTP).
/// TicketDAMemoria es Singleton porque DEBE conservar la lista entre peticiones.
/// </summary>
public static class RegistroDA
{
    public static IServiceCollection AddDatos(this IServiceCollection servicios, string cadenaConexion, bool ticketsEnMemoria)
    {
        servicios.AddDbContext<SistemaSoporteContexto>(o => o.UseSqlServer(cadenaConexion));

        // Módulo Clientes
        servicios.AddScoped<IClienteDA, ClienteDA>();

        // Módulo Clientes
        servicios.AddScoped<IAgenteDA, AgenteDA>();

        // Módulo Etiquetas
        servicios.AddScoped<IEtiquetaDA, EtiquetaDA>();

        // Módulo Comentarios
        servicios.AddScoped<IComentarioDA, ComentarioDA>();

        // Módulo Tickets (dos implementaciones intercambiables)
        if (ticketsEnMemoria)
            servicios.AddSingleton<ITicketDA, TicketDAMemoria>();
        else
            servicios.AddScoped<ITicketDA, TicketDA>();

        servicios.AddScoped<SemillaDatos>();
        return servicios;
    }
}
