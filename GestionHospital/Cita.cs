using System;

namespace GestionHospital
{
    public enum EstadoCita
    {
        Pendiente,
        Confirmada,
        Cancelada
    }

    public class Cita
    {
        public Guid IdCita { get; private set; }
        public DateTime FechaHora { get; set; }
        public string DniPaciente { get; set; }
        public string DniMedico { get; set; }
        public EstadoCita Estado { get; set; }
        public bool HistorialGenerado { get; set; }

        public Cita(string dniPaciente, string dniMedico, DateTime fechaHora)
        {
            if (fechaHora < DateTime.Now)
                throw new ArgumentException("La fecha y hora de la cita no pueden ser en el pasado.");
            if (string.IsNullOrWhiteSpace(dniMedico))
                throw new ArgumentException("El DNI del médico es obligatorio para la cita.");
            if (string.IsNullOrWhiteSpace(dniPaciente))
                throw new ArgumentException("El DNI del paciente es obligatorio para la cita.");

            IdCita = Guid.NewGuid();
            DniPaciente = dniPaciente;
            DniMedico = dniMedico;
            FechaHora = fechaHora;
            Estado = EstadoCita.Pendiente;
            HistorialGenerado = false;
        }

        public override string ToString()
        {
            return $"ID Cita: {IdCita.ToString().Substring(0, 8)} - Fecha: {FechaHora:dd/MM/yyyy HH:mm} - Estado: {Estado}";
        }
    }
}