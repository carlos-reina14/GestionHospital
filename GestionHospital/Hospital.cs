using System;
using System.Collections.Generic;
using System.Linq;

namespace GestionHospital
{
    public class Hospital
    {
        private List<Persona> personas = new List<Persona>();
        private static int _siguienteNumHistoriaClinica = 1;

        public Hospital()
        {
            personas = new List<Persona>();
        }

        // --- Métodos de gestión de personal ---

        public void DarDeAltaMedico()
        {
            Console.WriteLine("\n--- Dar de Alta Médico ---");

            try
            {
                Medico nuevoMedico = CrearMedico();
                personas.Add(nuevoMedico);
                Console.WriteLine($"Médico {nuevoMedico.Nombre} {nuevoMedico.Apellidos} (DNI: {nuevoMedico.Dni} - Num. Colegiado: {nuevoMedico.NumeroColegiado}) dado de alta.");
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine($"Error al dar de alta médico: {ex.Message}");
            }
        }

        private Medico CrearMedico()
        {
            (string nombre, string apellidos, string dni) = ObtenerDatosGeneralesPersona();

            Console.Write("Especialidad: ");
            string especialidad = Console.ReadLine();

            Console.Write("Número Colegiado: ");
            string numColegiado = Console.ReadLine();
            if (EsNumeroColegiadoDuplicado(numColegiado))
            {
                Console.WriteLine("Error: Ya existe un médico con este número colegiado.");
                return null;
            }
            return new Medico(nombre, apellidos, dni, especialidad, numColegiado);
        }

        private (string Nombre, string Apellidos, string Dni) ObtenerDatosGeneralesPersona()
        {
            Console.Write("Nombre: ");
            string nombre = Console.ReadLine();
            Console.Write("Apellidos: ");
            string apellidos = Console.ReadLine();
            Console.Write("DNI: ");
            string dni = Console.ReadLine();
            if (EsDniDuplicado(dni))
            {
                Console.WriteLine("Error: Ya existe una persona con este DNI.");
                dni = null; // Asignar null para evitar crear un objeto con DNI duplicado
            }

            return (nombre, apellidos, dni);
        }

        private bool EsDniDuplicado(string dni)
        {
            return personas.Any(p => p.Dni == dni);
        }

        private bool EsNumeroColegiadoDuplicado(string numColegiado)
        {
            // Usamos OfType<Medico>() para filtrar solo los objetos de tipo Medico
            return personas.OfType<Medico>().Any(m => m.NumeroColegiado == numColegiado);
        }

        public void DarDeAltaPaciente(Paciente paciente)
        {
            Console.WriteLine("\n--- Dar de Alta Paciente ---");
            List<Medico> medicosDisponibles = personas.OfType<Medico>().ToList();
            if (medicosDisponibles.Count == 0)
            {
                Console.WriteLine("No hay médicos registrados. Primero debe dar de alta un médico.");
                return;
            }

            Console.WriteLine("Médicos disponibles:");
            ListarMedicos();

            try
            {
                Paciente nuevoPaciente = CrearPaciente();
                personas.Add(nuevoPaciente);
                Console.WriteLine($"Paciente {nuevoPaciente.Nombre} {nuevoPaciente.Apellidos} (DNI: {nuevoPaciente.Dni}) dado de alta con Nº Historia Clínica: {nuevoPaciente.NumeroHistoriaClinica}.");
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine($"Error al dar de alta paciente: {ex.Message}");
            }
        }

        private Paciente CrearPaciente()
        {
            (string nombre, string apellidos, string dni) = ObtenerDatosGeneralesPersona();

            string numeroHistoriaClinica = GenerarNumeroHistoriaClinica();

            Console.Write("Síntomas: ");
            string sintomas = Console.ReadLine();

            Console.Write("DNI del Médico a Asignar: ");
            string dniMedico = Console.ReadLine();
            if (personas.FirstOrDefault(m => m.Dni == dniMedico) == null)
            {
                Console.WriteLine("DNI de médico no válido o no encontrado.");
                return null;
            }

            return new Paciente(nombre, apellidos, dni, numeroHistoriaClinica, sintomas, null);
        }

        public void ListarMedicos()
        {
            /*Console.WriteLine("\n--- Lista de Médicos ---");
            if (medicos.Count == 0)
            {
                Console.WriteLine("No hay médicos registrados.");
                return;
            }
            foreach (var medico in medicos)
                Console.WriteLine(medico);*/
        }

        private string GenerarNumeroHistoriaClinica()
        {
            string numeroGenerado;
            do
            {
                // Formato HCL-YYYY-NNNN (HCL-YYYY- seguido de 4 o 6 dígitos secuenciales)
                numeroGenerado = $"HCL-{DateTime.Now:yyyy}-{_siguienteNumHistoriaClinica:D4}";
                _siguienteNumHistoriaClinica++;
            } 
            while (EsNumeroHistoriaClinicaDuplicado(numeroGenerado));
            return numeroGenerado;
        }

        private bool EsNumeroHistoriaClinicaDuplicado(string numeroHistoriaClinica)
        {
            return personas.OfType<Paciente>().Any(p => p.NumeroHistoriaClinica == numeroHistoriaClinica);
        }
    }
}
