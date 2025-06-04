using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace GestionHospital
{
    public class Hospital
    {
        private List<Persona> _personas = new List<Persona>();
        private List<Cita> _citas = new List<Cita>();
        private static int _siguienteNumHistoriaClinica = 1;
        private static readonly string[] formatosFechaHoraAceptados = new string[]
            {
                "dd/MM/yyyy HH:mm",
                "d/M/yyyy HH:mm",
                "dd/M/yyyy HH:mm",
                "d/MM/yyyy HH:mm"
            };

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
            while (opcion != "13");
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
            Console.WriteLine("9. Programar una cita");
            Console.WriteLine("10. Cancelar una cita");
            Console.WriteLine("11. Modificar una cita");
            Console.WriteLine("12. Listar todas las citas");
            Console.WriteLine("13. Salir");
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
                    ProgramarCita();
                    break;
                case "10":
                    CancelarCita();
                    break;
                case "11":
                    ModificarCita();
                    break;
                case "12":
                    ListarCitas();
                    break;
                case "13":
                    Console.WriteLine("Saliendo del sistema. ¡Gracias por usar el Sistema de Gestión del Hospital!");
                    break;
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
            Persona personaAModificar = BuscarPersonaPorDni(dni);

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

        private Persona BuscarPersonaPorDni(string dni)
        {
            return _personas.FirstOrDefault(p => p.Dni == dni.ToUpper());
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

        public void ProgramarCita()
        {
            Console.WriteLine("\n--- Programar Nueva Cita ---");

            // 1. Obtener Médico y Paciente
            (Medico medico, Paciente paciente) = ObtenerMedicoYPacienteParaCita();
            if (medico == null || paciente == null) return;

            // 2. Obtener Fecha y Hora de la Cita
            DateTime fechaHoraCita;
            try
            {
                fechaHoraCita = ObtenerFechaHoraCita();
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                return;
            }

            // 3. Verificar Disponibilidad
            if (!VerificarDisponibilidadCita(fechaHoraCita, medico.Dni, paciente.Dni))
                return;

            // 4. Crear y Añadir Cita
            try
            {
                Cita nuevaCita = new Cita(paciente.Dni, medico.Dni, fechaHoraCita);
                _citas.Add(nuevaCita);
                Console.WriteLine($"Cita programada con éxito: {nuevaCita}");
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine($"Error al programar cita: {ex.Message}");
            }
        }

        private (Medico Medico, Paciente Paciente) ObtenerMedicoYPacienteParaCita()
        {
            ListarMedicos();
            Console.Write("Ingrese DNI del médico para la cita: ");
            string dniMedico = Console.ReadLine();

            if (!(BuscarPersonaPorDni(dniMedico) is Medico medico))
            {
                Console.WriteLine("Médico no encontrado.");
                return (null, null);
            }

            ListarPacientes();
            Console.Write("Ingrese DNI del paciente para la cita: ");
            string dniPaciente = Console.ReadLine();

            if (!(BuscarPersonaPorDni(dniPaciente) is Paciente paciente))
            {
                Console.WriteLine("Paciente no encontrado.");
                return (null, null);
            }
            return (medico, paciente);
        }

        private DateTime ObtenerFechaHoraCita()
        {
            Console.Write("Ingrese Fecha de la cita (ej. DD/MM/YYYY o D/M/YYYY): ");
            string fechaStr = Console.ReadLine();
            Console.Write("Ingrese Hora de la cita (HH:MM): ");
            string horaStr = Console.ReadLine();

            if (!DateTime.TryParseExact($"{fechaStr} {horaStr}", formatosFechaHoraAceptados, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime fechaHoraCita))
                throw new ArgumentException("Formato de fecha u hora no válido. Por favor, use formatos como DD/MM/YYYY HH:MM o D/M/YYYY HH:MM.");
            if (fechaHoraCita < DateTime.Now)
                throw new ArgumentException("La fecha y hora de la cita no pueden ser en el pasado.");
            
            return fechaHoraCita;
        }

        private bool VerificarDisponibilidadCita(DateTime fechaHora, string dniMedico, string dniPaciente, Guid? idCitaExcluir = null)
        {
            // Comprueba si el médico ya tiene una cita programada a esa hora (excluyendo la cita que se está modificando)
            if (_citas.Any(c => c.DniMedico == dniMedico && c.FechaHora == fechaHora && c.Estado == EstadoCita.Pendiente && c.IdCita != idCitaExcluir))
            {
                Medico medico = BuscarPersonaPorDni(dniMedico) as Medico;
                Console.WriteLine($"El médico {medico.Nombre} {medico.Apellidos} ya tiene otra cita programada para esa fecha y hora.");
                return false;
            }

            // Comprueba si el paciente ya tiene una cita programada a esa hora (excluyendo la cita que se está modificando)
            if (_citas.Any(c => c.DniPaciente == dniPaciente && c.FechaHora == fechaHora && c.Estado == EstadoCita.Pendiente && c.IdCita != idCitaExcluir))
            {
                Paciente paciente = BuscarPersonaPorDni(dniPaciente) as Paciente;
                Console.WriteLine($"El paciente {paciente.Nombre} {paciente.Apellidos} ya tiene otra cita programada para esa fecha y hora.");
                return false;
            }
            return true;
        }

        public void CancelarCita()
        {
            Console.WriteLine("\n--- Cancelar Cita ---");
            ListarCitas();

            Console.Write("Ingrese el ID de la cita a cancelar: ");
            string idCitaStr = Console.ReadLine();

            Cita citaACancelar = BuscarCitaPorIdParcial(idCitaStr);
            if (citaACancelar == null)
            {
                Console.WriteLine("Cita no encontrada.");
                return;
            }
            if (citaACancelar.Estado != EstadoCita.Pendiente)
            {
                Console.WriteLine("Solo se pueden cancelar citas pendientes.");
                return;
            }

            citaACancelar.Estado = EstadoCita.Cancelada;
            Console.WriteLine($"Cita cancelada: {citaACancelar}");
        }

        private Cita BuscarCitaPorIdParcial(string idParcial)
        {
            if (string.IsNullOrWhiteSpace(idParcial) || idParcial.Length < 8) 
                return null;
            return _citas.FirstOrDefault(c => c.IdCita.ToString().StartsWith(idParcial, StringComparison.OrdinalIgnoreCase));
        }

        public void ListarCitas()
        {
            Console.WriteLine("\n--- Lista de Citas ---");
            if (_citas.Count == 0)
            {
                Console.WriteLine("No hay citas programadas.");
                return;
            }
            foreach (var cita in _citas.OrderBy(c => c.FechaHora))
                Console.WriteLine(cita);
        }

        public void ModificarCita()
        {
            Console.WriteLine("\n--- Modificar Cita ---");
            ListarCitas();

            Console.Write("Ingrese el ID de la cita a modificar: ");
            string idCitaStr = Console.ReadLine();

            Cita citaAModificar = BuscarCitaPorIdParcial(idCitaStr);

            if (citaAModificar == null)
            {
                Console.WriteLine("Cita no encontrada.");
                return;
            }

            if (citaAModificar.Estado != EstadoCita.Pendiente)
            {
                Console.WriteLine($"La cita no está en estado 'Pendiente' y no puede ser modificada. Estado actual: {citaAModificar.Estado}");
                return;
            }

            ElegirModificacionCita(citaAModificar);
        }

        private void ElegirModificacionCita(Cita citaAModificar)
        {
            Console.WriteLine($"\nModificando cita: {citaAModificar}");
            Console.WriteLine("¿Qué desea modificar?");
            Console.WriteLine("1. Fecha y Hora");
            Console.WriteLine("2. Médico Asignado");
            Console.WriteLine("3. Paciente Asignado");
            Console.WriteLine("4. Marcar como Confirmada");
            Console.Write("Seleccione una opción (o dejar en blanco para no modificar nada): ");
            string opcion = Console.ReadLine();

            try
            {
                switch (opcion)
                {
                    case "1":
                        ModificarFechaHoraCita(citaAModificar);
                        break;
                    case "2":
                        ModificarMedicoAsignadoCita(citaAModificar);
                        break;
                    case "3":
                        ModificarPacienteAsignadoCita(citaAModificar);
                        break;
                    case "4":
                        MarcarCitaComoConfirmada(citaAModificar);
                        break;
                    case "":
                        Console.WriteLine("No se realizó ningún cambio.");
                        break;
                    default:
                        Console.WriteLine("Opción no válida.");
                        break;
                }
                Console.WriteLine($"Datos de cita actualizados: {citaAModificar}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al modificar la cita: {ex.Message}");
            }
        }

        private void ModificarFechaHoraCita(Cita cita)
        {
            Console.Write("Nueva Fecha (DD/MM/YYYY): ");
            string nuevaFechaStr = Console.ReadLine();
            Console.Write("Nueva Hora (HH:MM): ");
            string nuevaHoraStr = Console.ReadLine();

            if (!DateTime.TryParseExact($"{nuevaFechaStr} {nuevaHoraStr}", formatosFechaHoraAceptados, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime nuevaFechaHora))
            {
                Console.WriteLine("Formato de fecha u hora no válido.");
                return;
            }

            if (nuevaFechaHora < DateTime.Now)
            {
                Console.WriteLine("Error: La nueva fecha y hora no pueden ser en el pasado.");
                return;
            }

            if (!VerificarDisponibilidadCita(nuevaFechaHora, cita.DniMedico, cita.DniPaciente, cita.IdCita))
                return;

            cita.FechaHora = nuevaFechaHora;
            Console.WriteLine("Fecha y hora de la cita actualizadas.");
        }

        private void ModificarMedicoAsignadoCita(Cita cita)
        {
            ListarMedicos();
            Console.Write("Nuevo DNI del Médico: ");
            string nuevoDniMedico = Console.ReadLine();

            if (!(BuscarPersonaPorDni(nuevoDniMedico) is Medico nuevoMedico))
            {
                Console.WriteLine("Médico no encontrado. No se cambió el médico.");
                return;
            }

            if (!VerificarDisponibilidadCita(cita.FechaHora, nuevoMedico.Dni, cita.DniPaciente, cita.IdCita))
                return;

            // Validar si es el mismo médico
            if (cita.DniMedico.ToUpper() == nuevoMedico.Dni.ToUpper())
            {
                Console.WriteLine("El paciente ya está asignado a este médico para esta cita. No se realizó ningún cambio.");
                return;
            }

            cita.DniMedico = nuevoMedico.Dni;
            Console.WriteLine("Médico asignado a la cita actualizado.");
        }

        private void ModificarPacienteAsignadoCita(Cita cita)
        {
            ListarPacientes();
            Console.Write("Nuevo DNI del Paciente: ");
            string nuevoDniPaciente = Console.ReadLine();

            if (!(BuscarPersonaPorDni(nuevoDniPaciente) is Paciente nuevoPaciente))
            {
                Console.WriteLine("Paciente no encontrado. No se cambió el paciente.");
                return;
            }

            if (!VerificarDisponibilidadCita(cita.FechaHora, cita.DniMedico, nuevoPaciente.Dni, cita.IdCita))
                return;

            // Validar si es el mismo paciente
            if (cita.DniPaciente.ToUpper() == nuevoPaciente.Dni.ToUpper())
            {
                Console.WriteLine("El paciente ya es este en esta cita. No se realizó ningún cambio.");
                return;
            }

            cita.DniPaciente = nuevoPaciente.Dni;
            Console.WriteLine("Paciente asignado a la cita actualizado.");
        }

        private void MarcarCitaComoConfirmada(Cita cita)
        {
            if (cita.Estado == EstadoCita.Confirmada)
            {
                Console.WriteLine("La cita ya está marcada como Confirmada.");
                return;
            }
            if (cita.Estado == EstadoCita.Cancelada)
            {
                Console.WriteLine("No se puede marcar como Confirmada una cita Cancelada.");
                return;
            }

            cita.Estado = EstadoCita.Confirmada;
            Console.WriteLine($"Cita marcada como Confirmada con éxito: {cita}");
        }
    }
}