using System;
using System.Collections.Generic;
using System.Linq;

namespace GestionHospital
{
    public class Hospital
    {
        private List<Persona> _personas = new List<Persona>();
        private static int _siguienteNumHistoriaClinica = 1;

        public void IniciarAplicacion()
        {
            string opcion;
            Console.WriteLine("Bienvenido al Sistema de Gestión del Hospital");
            do
            {
                MostrarMenu();
                opcion = Console.ReadLine();
                TratarOpcion(opcion);
            }
            while (opcion != "9");
        }

        private void MostrarMenu()
        {
            Console.WriteLine("\n--- Menú de Gestión del Hospital ---");
            Console.WriteLine("1. Dar de alta un médico");
            Console.WriteLine("2. Dar de alta un paciente");
            Console.WriteLine("3. Dar de alta personal administrativo");
            Console.WriteLine("4. Listar médicos");
            Console.WriteLine("5. Listar pacientes de un médico");
            Console.WriteLine("6. Eliminar un paciente");
            Console.WriteLine("7. Ver la lista de todas las personas del hospital");
            Console.WriteLine("8. Modificar datos de una persona");
            Console.WriteLine("9. Salir");
            Console.Write("Seleccione una opción: ");
        }

        private void TratarOpcion(string opcion)
        {
            switch (opcion)
            {
                case "1":
                    DarDeAltaMedico();
                    break;
                case "2":
                    DarDeAltaPaciente();
                    break;
                case "3":
                    DarDeAltaPersonalAdministrativo();
                    break;
                case "4":
                    ListarMedicos();
                    break;
                case "5":
                    ListarPacientesDeMedico();
                    break;
                case "6":
                    EliminarPaciente();
                    break;
                case "7":
                    VerListaDePersonasDelHospital();
                    break;
                case "8":
                    ModificarDatosPersona();
                    break;
                case "9":
                    Console.WriteLine("Saliendo de la aplicación. ¡Hasta pronto!");
                    return;
                default:
                    Console.WriteLine("Opción no válida. Por favor, intente de nuevo.");
                    break;
            }
            LimpiarPantalla();
        }

        public void DarDeAltaMedico()
        {
            Console.WriteLine("\n--- Dar de Alta Médico ---");

            try
            {
                Medico nuevoMedico = CrearMedico();
                _personas.Add(nuevoMedico);
                Console.WriteLine($"Médico {nuevoMedico.Nombre} {nuevoMedico.Apellidos} (DNI: {nuevoMedico.Dni} - Num. Colegiado: {nuevoMedico.NumeroColegiado}) dado de alta.");
            }
            // Captura errores de formato de DNI, Colegiado, etc.
            catch (ArgumentException ex)
            {
                Console.WriteLine($"Error al dar de alta médico: {ex.Message}");
            }
            // Captura errores de lógica de negocio (ej. DNI/colegiado duplicado)
            catch (InvalidOperationException ex)
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
                throw new InvalidOperationException("Ya existe un médico con este número colegiado.");

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
                throw new InvalidOperationException($"Ya existe una persona con el DNI '{dni.ToUpper()}'.");

            return (nombre, apellidos, dni);
        }

        private bool EsDniDuplicado(string dni)
        {
            return _personas.Any(p => p.Dni == dni.ToUpper());
        }

        private bool EsNumeroColegiadoDuplicado(string numColegiado)
        {
            // OfType<Medico>() para filtrar solo los objetos de tipo Medico
            return _personas.OfType<Medico>().Any(m => m.NumeroColegiado == numColegiado);
        }

        private void LimpiarPantalla()
        {
            Console.WriteLine("\nPresione cualquier tecla para continuar...");
            Console.ReadKey();
            Console.Clear();
        }

        public void DarDeAltaPaciente()
        {
            Console.WriteLine("\n--- Dar de Alta Paciente ---");
            ListarMedicos();

            try
            {
                Paciente nuevoPaciente = CrearPaciente();
                if (nuevoPaciente == null) return;
                _personas.Add(nuevoPaciente);

                Medico medicoAsignado = _personas.OfType<Medico>().FirstOrDefault(m => m.Dni == nuevoPaciente.DniMedico);
                medicoAsignado.AsignarPaciente(nuevoPaciente);
                Console.WriteLine($"Paciente {nuevoPaciente.Nombre} {nuevoPaciente.Apellidos} (DNI: {nuevoPaciente.Dni}) dado de alta con Nº Historia Clínica: {nuevoPaciente.NumeroHistoriaClinica} y asignado al Dr./Dra. {medicoAsignado.Apellidos}.");
            }
            // Captura errores de formato (DNI, NHistClinica, DNI médico asignado)
            catch (ArgumentException ex)
            {
                Console.WriteLine($"Error al dar de alta paciente: {ex.Message}");
            }
            // Captura errores de lógica (DNI duplicado, asignación duplicada)
            catch (InvalidOperationException ex)
            {
                Console.WriteLine($"Error al dar de alta paciente: {ex.Message}");
            }
        }

        public void ListarMedicos()
        {
            Console.WriteLine("\n--- Lista de Médicos ---");
            List<Medico> medicos = _personas.OfType<Medico>().ToList();
            if (medicos.Count == 0)
            {
                Console.WriteLine("No hay médicos registrados.");
                return;
            }
            foreach (var medico in medicos)
                Console.WriteLine(medico);
        }

        private Paciente CrearPaciente()
        {
            (string nombre, string apellidos, string dni) = ObtenerDatosGeneralesPersona();

            string numeroHistoriaClinica = GenerarNumeroHistoriaClinica();

            Console.Write("Síntomas: ");
            string sintomas = Console.ReadLine();

            Console.Write("DNI del Médico a Asignar: ");
            string dniMedico = Console.ReadLine();
            if (!_personas.OfType<Medico>().Any(m => m.Dni == dniMedico.ToUpper()))
            {
                Console.WriteLine("Error: El DNI del médico a asignar no corresponde a un médico registrado.");
                return null;
            }

            return new Paciente(nombre, apellidos, dni, numeroHistoriaClinica, sintomas, dniMedico.ToUpper());
        }

        private string GenerarNumeroHistoriaClinica()
        {
            string numeroGenerado;
            do
            {
                // Formato HCL-YYYY-NNNN (HCL-YYYY- seguido de 4 dígitos secuenciales)
                numeroGenerado = $"HCL-{DateTime.Now:yyyy}-{_siguienteNumHistoriaClinica:D4}";
                _siguienteNumHistoriaClinica++;
            }
            while (EsNumeroHistoriaClinicaDuplicado(numeroGenerado));
            return numeroGenerado;
        }

        private bool EsNumeroHistoriaClinicaDuplicado(string numeroHistoriaClinica)
        {
            // OfType<Paciente>() para filtrar solo los objetos de tipo Paciente
            return _personas.OfType<Paciente>().Any(p => p.NumeroHistoriaClinica == numeroHistoriaClinica);
        }

        public void DarDeAltaPersonalAdministrativo()
        {
            Console.WriteLine("\n--- Dar de Alta Personal Administrativo ---");
            try
            {
                PersonalAdministrativo nuevoPersonal = CrearPersonalAdministrativo();
                _personas.Add(nuevoPersonal);
                Console.WriteLine($"Personal Administrativo {nuevoPersonal.Nombre} {nuevoPersonal.Apellidos} (DNI: {nuevoPersonal.Dni}) dado de alta.");
            }
            // Captura errores de formato de DNI
            catch (ArgumentException ex)
            {
                Console.WriteLine($"Error al dar de alta personal administrativo: {ex.Message}");
            }
            // Captura errores de lógica (DNI duplicado)
            catch (InvalidOperationException ex)
            {
                Console.WriteLine($"Error al dar de alta personal administrativo: {ex.Message}");
            }
        }

        private PersonalAdministrativo CrearPersonalAdministrativo()
        {
            (string nombre, string apellidos, string dni) = ObtenerDatosGeneralesPersona();

            Console.Write("Puesto: ");
            string puesto = Console.ReadLine();
            Console.Write("Departamento: ");
            string departamento = Console.ReadLine();
            return new PersonalAdministrativo(nombre, apellidos, dni, puesto, departamento);
        }

        public void ListarPacientesDeMedico()
        {
            Console.WriteLine("\n--- Listar Pacientes de un Médico ---");
            ListarMedicos();

            Console.Write("Ingrese el DNI del médico para ver sus pacientes: ");
            string dniMedico = Console.ReadLine();

            List<Medico> medicosEncontrados = _personas.OfType<Medico>().ToList();
            Medico medicoSeleccionado = medicosEncontrados.FirstOrDefault(m => m.Dni == dniMedico.ToUpper());
            if (medicoSeleccionado == null)
            {
                Console.WriteLine("Médico no encontrado.");
                return;
            }

            medicoSeleccionado.ListarPacientes();
        }

        public void EliminarPaciente()
        {
            Console.WriteLine("\n--- Eliminar Paciente ---");
            ListarPacientes();

            Console.Write("Ingrese el DNI del paciente a eliminar: ");
            string dniPacienteAEliminar = Console.ReadLine();

            List<Paciente> pacientesEncontrados = _personas.OfType<Paciente>().ToList();
            Paciente pacienteAEliminar = pacientesEncontrados.FirstOrDefault(p => p.Dni == dniPacienteAEliminar.ToUpper());

            if (pacienteAEliminar == null)
            {
                Console.WriteLine("Paciente no encontrado.");
                return;
            }

            _personas.Remove(pacienteAEliminar);

            EliminarPacienteDeMedico(pacienteAEliminar);

            Console.WriteLine($"Paciente {pacienteAEliminar.Nombre} {pacienteAEliminar.Apellidos} (DNI: {pacienteAEliminar.Dni}) eliminado correctamente.");
        }

        private void ListarPacientes()
        {
            Console.WriteLine("\n--- Lista de Pacientes ---");
            List<Paciente> pacientes = _personas.OfType<Paciente>().ToList();
            if (pacientes.Count == 0)
            {
                Console.WriteLine("No hay pacientes registrados.");
                return;
            }

            foreach (var paciente in pacientes)
                Console.WriteLine(paciente);
        }

        private void EliminarPacienteDeMedico(Paciente pacienteAEliminar)
        {
            Medico medicoDelPaciente = _personas.OfType<Medico>().FirstOrDefault(m => m.Dni == pacienteAEliminar.DniMedico);
            if (medicoDelPaciente != null)
            {
                try
                {
                    medicoDelPaciente.EliminarPaciente(pacienteAEliminar);
                }
                catch (InvalidOperationException ex)
                {
                    Console.WriteLine($"Advertencia: {ex.Message}");
                }
            }
        }

        public void VerListaDePersonasDelHospital()
        {
            Console.WriteLine("\n--- Todas las Personas del Hospital ---");
            ListarMedicos();
            ListarPacientes();
            ListarPersonalAdministrativo();
        }

        private void ListarPersonalAdministrativo()
        {
            Console.WriteLine("\n--- Lista de Personal Administrativo ---");
            var personalAdmin = _personas.OfType<PersonalAdministrativo>().ToList();
            if (personalAdmin.Count == 0)
            {
                Console.WriteLine("No hay personal administrativo registrado.");
                return;
            }
            foreach (var pa in personalAdmin)
                Console.WriteLine(pa);
        }

        public void ModificarDatosPersona()
        {
            Console.WriteLine("\n--- Modificar Datos de una Persona ---");
            Console.Write("Ingrese el DNI de la persona a modificar: ");
            string dni = Console.ReadLine();
            Persona personaAModificar = _personas.FirstOrDefault(p => p.Dni == dni.ToUpper());

            if (personaAModificar == null)
            {
                Console.WriteLine("Persona no encontrada.");
                return;
            }

            Console.WriteLine($"Datos actuales: {personaAModificar}");

            try
            {
                CambiarDatosGeneralesPersona(personaAModificar);

                // Lógica específica para cada tipo de persona
                if (personaAModificar is Medico medico)
                {
                    CambiarDatosMedico(medico);
                }
                else if (personaAModificar is Paciente paciente)
                {
                    CambiarDatosPaciente(paciente);
                }
                else if (personaAModificar is PersonalAdministrativo administrativo)
                {
                    CambiarDatosPersonalAdministrativo(administrativo);
                }

                Console.WriteLine("Datos actualizados correctamente.");
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine($"Error al modificar datos: {ex.Message}");
            }
            catch (InvalidOperationException ex)
            {
                Console.WriteLine($"Error al modificar datos: {ex.Message}");
            }

            Console.WriteLine($"Datos modificados: {personaAModificar}");
        }

        private void CambiarDatosGeneralesPersona(Persona personaAModificar)
        {
            Console.Write("Nuevo Nombre (dejar en blanco para no modificar): ");
            string nuevoNombre = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(nuevoNombre)) personaAModificar.Nombre = nuevoNombre;

            Console.Write("Nuevos Apellidos (dejar en blanco para no modificar): ");
            string nuevosApellidos = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(nuevosApellidos)) personaAModificar.Apellidos = nuevosApellidos;
        }

        private void CambiarDatosMedico(Medico medico)
        {
            Console.Write("Nueva Especialidad (dejar en blanco para no cambiar): ");
            string nuevaEspecialidad = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(nuevaEspecialidad)) medico.Especialidad = nuevaEspecialidad;

            Console.Write("Nuevo Número Colegiado (dejar en blanco para no cambiar): ");
            string nuevoNumColegiado = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(nuevoNumColegiado))
            {
                if (EsNumeroColegiadoDuplicado(nuevoNumColegiado))
                    throw new InvalidOperationException("Ya existe un médico con este número colegiado.");
                medico.NumeroColegiado = nuevoNumColegiado;
            }
        }

        private void CambiarDatosPaciente(Paciente paciente)
        {
            Console.Write("Nuevos Síntomas (dejar en blanco para no cambiar): ");
            string nuevosSintomas = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(nuevosSintomas)) paciente.Sintomas = nuevosSintomas;

            Console.WriteLine("\n--- Médicos disponibles para asignación ---");
            ListarMedicos();
            Console.Write("Nuevo DNI del Médico Asignado (dejar en blanco para no cambiar): ");
            string nuevoDniMedico = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(nuevoDniMedico))
            {
                AsignacionNuevoMedico(nuevoDniMedico, paciente);
            }
        }

        private void AsignacionNuevoMedico(string nuevoDniMedico, Paciente paciente)
        {
            // Valida si el nuevo médico existe antes de asignar
            Medico nuevoMedicoAsignado = _personas.OfType<Medico>().FirstOrDefault(m => m.Dni == nuevoDniMedico.ToUpper());
            if (nuevoMedicoAsignado == null)
                Console.WriteLine("Error: El nuevo DNI del médico a asignar no corresponde a un médico registrado. No se cambió el médico asignado.");
            else
            {
                // Eliminar el paciente del médico anterior si existe y es diferente al nuevo médico
                Medico medicoAnterior = _personas.OfType<Medico>().FirstOrDefault(m => m.Dni == paciente.DniMedico);
                if (medicoAnterior != null && medicoAnterior != nuevoMedicoAsignado)
                {
                    try
                    {
                        medicoAnterior.EliminarPaciente(paciente);
                        Console.WriteLine($"Paciente desasignado del Dr./Dra. {medicoAnterior.Apellidos}.");
                    }
                    catch (InvalidOperationException ex)
                    {
                        Console.WriteLine($"Advertencia al desasignar del médico anterior: {ex.Message}");
                    }
                }

                try
                {
                    paciente.DniMedico = nuevoMedicoAsignado.Dni;
                    nuevoMedicoAsignado.AsignarPaciente(paciente);
                    Console.WriteLine($"Paciente reasignado al Dr./Dra. {nuevoMedicoAsignado.Apellidos}.");
                }
                catch (InvalidOperationException ex)
                {
                    Console.WriteLine($"Error al reasignar paciente: {ex.Message}");
                }
            }
        }

        private void CambiarDatosPersonalAdministrativo(PersonalAdministrativo administrativo)
        {
            Console.Write("Nuevo Puesto (dejar en blanco para no cambiar): ");
            string nuevoPuesto = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(nuevoPuesto)) administrativo.Cargo = nuevoPuesto;

            Console.Write("Nuevo Departamento (dejar en blanco para no cambiar): ");
            string nuevoDepartamento = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(nuevoDepartamento)) administrativo.Departamento = nuevoDepartamento;
        }
    }
}