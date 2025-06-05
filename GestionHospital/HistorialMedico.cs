using System;

namespace GestionHospital
{
    public class HistorialMedico
    {
        public Guid IdCitaAsociada { get; private set; }
        public DateTime FechaConsulta { get; private set; }
        public string DniMedico { get; private set; }
        public string Diagnostico { get; set; }
        public string Tratamiento { get; set; }
        public string NotasMedicas { get; set; }

        public HistorialMedico(Guid idCitaAsociada, DateTime fechaConsulta, string dniMedico, string diagnostico, string tratamiento, string notasMedicas)
        {
            IdCitaAsociada = idCitaAsociada;
            FechaConsulta = fechaConsulta;
            DniMedico = dniMedico;
            Diagnostico = diagnostico;
            Tratamiento = tratamiento;
            NotasMedicas = notasMedicas;
        }

        public override string ToString()
        {
            return $"  - Fecha: {FechaConsulta:dd/MM/yyyy HH:mm} (Cita ID: {IdCitaAsociada.ToString().Substring(0, 8)})" +
                   $"\n    Médico DNI: {DniMedico}" +
                   $"\n    Diagnóstico: {Diagnostico}" +
                   $"\n    Tratamiento: {Tratamiento}" +
                   $"\n    Notas/Recetas: {NotasMedicas}";
        }
    }
}
