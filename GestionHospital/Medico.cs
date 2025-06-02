using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace GestionHospital
{
    public class Medico : Persona
    {
        private string _numeroColegiado { get; set; }

        public string Especialidad { get; set; }
        public string NumeroColegiado 
        { 
            get { return _numeroColegiado; }
            set
            {
                if (!EsNumeroColegiadoValido(value))
                    throw new ArgumentException("Formato de Número Colegiado no válido. Debe ser exactamente de 9 dígitos.");
                _numeroColegiado = value;
            } 
        }
        public List<Paciente> PacientesAsignados { get; private set; }

        public Medico(string nombre, string apellidos, string dni, string especialidad, string numeroColegiado)
            : base(nombre, apellidos, dni)
        {
            Especialidad = especialidad;
            NumeroColegiado = numeroColegiado;
            PacientesAsignados = new List<Paciente>();
        }

        private static bool EsNumeroColegiadoValido(string numeroColegiado)
        {
            if (string.IsNullOrEmpty(numeroColegiado))
                return false;
            return Regex.IsMatch(numeroColegiado, @"^\d{9}$");
        }

        public void AsignarPaciente(Paciente paciente)
        {
            if (paciente == null)
                throw new ArgumentNullException(nameof(paciente), "El paciente no puede ser nulo.");
            if (PacientesAsignados.Contains(paciente))
                throw new InvalidOperationException("El paciente ya está asignado a este médico.");
            PacientesAsignados.Add(paciente);
        }

        public void EliminarPaciente(Paciente paciente)
        {
            if (paciente == null)
                throw new ArgumentNullException(nameof(paciente), "El paciente no puede ser nulo.");
            if (!PacientesAsignados.Contains(paciente))
                throw new InvalidOperationException("El paciente no está asignado a este médico.");
            PacientesAsignados.Remove(paciente);
        }

        public void ListarPacientes()
        {
            Console.WriteLine($"\nPacientes del Dr./Dra. {this.Apellidos} (DNI: {this.Dni}):");

            if (PacientesAsignados.Count == 0)
            {
                Console.WriteLine("No hay pacientes asignados a este médico.");
                return;
            }
            foreach (var paciente in PacientesAsignados)
                Console.WriteLine(paciente);
        }

        public override string ToString()
        {
            return $"{base.ToString()} - Especialidad: {Especialidad} - Nº Colegiado: {NumeroColegiado}";
        }
    }
}