using Abstracciones.Tickets;
using System;
using System.Collections.Generic;
using System.Text;

namespace Abstracciones.Agente
{
    public class Agente
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string Correo { get; set; } = string.Empty;
        public string Telefono { get; set; } = string.Empty;

        public int CapMaxTicket { get; set; }
        public DateTime FechaRegistro { get; set; }
        public bool Activo { get; set; }
        public List<Ticket> Tickets { get; set; } = new();
    }
}
