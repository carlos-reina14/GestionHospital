using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace GestionHospital
{
    public class GestionCitas
    {
        private readonly List<Cita> _citas;
        private readonly GestionPersonal _gestionPersonal;
        private static readonly string[] formatosFechaHoraAceptados = new string[]
        {
            "dd/MM/yyyy HH:mm",
            "d/M/yyyy HH:mm",
            "dd/M/yyyy HH:mm",
            "d/MM/yyyy HH:mm"
        };

        public GestionCitas(List<Cita> citas, GestionPersonal gestionPersonal)
        {
            _citas = citas;
            _gestionPersonal = gestionPersonal;
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
            _gestionPersonal.ListarMedicos();
            Console.Write("Ingrese DNI del médico para la cita: ");
            string dniMedico = Console.ReadLine();

            if (!(_gestionPersonal.BuscarPersonaPorDni(dniMedico) is Medico medico))
            {
                Console.WriteLine("Médico no encontrado.");
                return (null, null);
            }

            _gestionPersonal.ListarPacientes();
            Console.Write("Ingrese DNI del paciente para la cita: ");
            string dniPaciente = Console.ReadLine();

            if (!(_gestionPersonal.BuscarPersonaPorDni(dniPaciente) is Paciente paciente))
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
                Medico medico = _gestionPersonal.BuscarPersonaPorDni(dniMedico) as Medico;
                Console.WriteLine($"El médico {medico.Nombre} {medico.Apellidos} ya tiene otra cita programada para esa fecha y hora.");
                return false;
            }

            // Comprueba si el paciente ya tiene una cita programada a esa hora (excluyendo la cita que se está modificando)
            if (_citas.Any(c => c.DniPaciente == dniPaciente && c.FechaHora == fechaHora && c.Estado == EstadoCita.Pendiente && c.IdCita != idCitaExcluir))
            {
                Paciente paciente = _gestionPersonal.BuscarPersonaPorDni(dniPaciente) as Paciente;
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
            string idCita = Console.ReadLine();

            Cita citaACancelar = BuscarCitaPorIdParcial(idCita);
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
            {
                string nombreMedico = _gestionPersonal.BuscarPersonaPorDni(cita.DniMedico) is Medico medico ? $"{medico.Nombre} {medico.Apellidos}" : "Médico desconocido";
                string nombrePaciente = _gestionPersonal.BuscarPersonaPorDni(cita.DniPaciente) is Paciente paciente ? $"{paciente.Nombre} {paciente.Apellidos}" : "Paciente desconocido";

                Console.WriteLine($@"{cita}
        Dr./Dra.: {nombreMedico} (DNI: {cita.DniMedico})
        Paciente: {nombrePaciente} (DNI: {cita.DniPaciente})");
            }
        }

        public void ModificarCita()
        {
            Console.WriteLine("\n--- Modificar Cita ---");
            ListarCitas();

            Console.Write("Ingrese el ID de la cita a modificar: ");
            string idCita = Console.ReadLine();

            Cita citaAModificar = BuscarCitaPorIdParcial(idCita);

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
            Console.WriteLine($@"
Modificando cita: {citaAModificar}
¿Qué desea modificar?
1. Fecha y Hora
2. Médico Asignado
3. Paciente Asignado
4. Marcar como Confirmada
Seleccione una opción (o dejar en blanco para no modificar nada): ");
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
            _gestionPersonal.ListarMedicos();
            Console.Write("Nuevo DNI del Médico: ");
            string nuevoDniMedico = Console.ReadLine();

            if (!(_gestionPersonal.BuscarPersonaPorDni(nuevoDniMedico) is Medico nuevoMedico))
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
            _gestionPersonal.ListarPacientes();
            Console.Write("Nuevo DNI del Paciente: ");
            string nuevoDniPaciente = Console.ReadLine();

            if (!(_gestionPersonal.BuscarPersonaPorDni(nuevoDniPaciente) is Paciente nuevoPaciente))
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

        public void RegistrarHistorialMedico()
        {
            Console.WriteLine("\n--- Registrar Historial Médico (Post-Consulta) ---");
            ListarCitas();

            // 1. Verificar si hay una cita para registrar historial
            if(!HayCitaParaHistorial(out Cita citaParaHistorial, out Paciente paciente)) return;

            // 2. Mostrar información de la cita y paciente
            MostrarDatosCita(citaParaHistorial, paciente);

            // 3. Obtener datos del historial médico
            (string diagnostico, string tratamiento, string notas) = ObtenerDatosHistorial();

            // 4. Crear nueva entrada de historial médico
            CrearEntradaHistorial(citaParaHistorial, paciente, diagnostico, tratamiento, notas);

            Console.WriteLine("\nHistorial médico registrado con éxito.");
        }

        private bool HayCitaParaHistorial(out Cita cita, out Paciente paciente)
        {
            paciente = null;

            Console.Write("Ingrese el ID de la cita para registrar el historial: ");
            string idCita = Console.ReadLine();

            cita = BuscarCitaPorIdParcial(idCita);
            if (cita == null)
            {
                Console.WriteLine("Cita no encontrada o ID inválido.");
                return false;
            }

            // Validaciones específicas para el registro del historial
            if (cita.Estado == EstadoCita.Cancelada)
            {
                Console.WriteLine("No se puede registrar historial para una cita cancelada.");
                return false;
            }
            if (cita.FechaHora > DateTime.Now)
            {
                Console.WriteLine("La cita aún no ha ocurrido. No se puede registrar historial médico.");
                return false;
            }
            if (cita.HistorialGenerado)
            {
                Console.WriteLine("Ya existe una entrada de historial para esta cita.");
                return false;
            }

            // Obtener el paciente asociado a la cita
            if (!(_gestionPersonal.BuscarPersonaPorDni(cita.DniPaciente) is Paciente p))
            {
                Console.WriteLine("Error: Paciente de la cita no encontrado en el sistema.");
                return false;
            }
            paciente = p;
            return true;
        }

        private void MostrarDatosCita(Cita cita, Paciente paciente)
        {
            // Para mostrar el nombre del médico de la cita
            string nombreMedicoCita = _gestionPersonal.BuscarPersonaPorDni(cita.DniMedico) is Medico medicoCita ? $"{medicoCita.Nombre} {medicoCita.Apellidos}" : "Médico desconocido";

            Console.WriteLine($"\nRegistrando historial para Paciente: {paciente.Nombre} {paciente.Apellidos} (DNI: {paciente.Dni})");
            Console.WriteLine($"Cita con Médico: {nombreMedicoCita} el {cita.FechaHora:dd/MM/yyyy HH:mm}");
        }

        private (string Diagnostico, string Tratamiento, string Notas) ObtenerDatosHistorial()
        {
            Console.Write("Ingrese Diagnóstico: ");
            string diagnostico = Console.ReadLine();
            Console.Write("Ingrese Tratamiento: ");
            string tratamiento = Console.ReadLine();
            Console.Write("Ingrese Notas Médicas / Recetas: ");
            string notasMedicas = Console.ReadLine();

            return (diagnostico, tratamiento, notasMedicas);
        }

        private void CrearEntradaHistorial(Cita cita, Paciente paciente, string diagnostico, string tratamiento, string notas)
        {
            HistorialMedico nuevaEntrada = new HistorialMedico(
                cita.IdCita,
                cita.FechaHora,
                cita.DniMedico,
                diagnostico,
                tratamiento,
                notas
            );

            paciente.HistorialMedico.Add(nuevaEntrada);
            cita.HistorialGenerado = true;
        }

        public void VerHistorialMedicoPaciente()
        {
            Console.WriteLine("\n--- Ver Historial Médico de Paciente ---");
            _gestionPersonal.ListarPacientes();

            Console.Write("Ingrese DNI del paciente para ver su historial: ");
            string dniPaciente = Console.ReadLine();

            if (!(_gestionPersonal.BuscarPersonaPorDni(dniPaciente) is Paciente paciente))
            {
                Console.WriteLine("Paciente no encontrado.");
                return;
            }

            ListarHistorialPaciente(paciente);
        }

        private void ListarHistorialPaciente(Paciente paciente)
        {
            Console.WriteLine($"\nHistorial Médico de: {paciente.Nombre} {paciente.Apellidos} (DNI: {paciente.Dni})");

            if (paciente.HistorialMedico.Count == 0)
            {
                Console.WriteLine("No hay entradas en el historial médico para este paciente.");
                return;
            }

            foreach (var entrada in paciente.HistorialMedico.OrderBy(e => e.FechaConsulta))
            {
                string nombreMedico = _gestionPersonal.BuscarPersonaPorDni(entrada.DniMedico) is Medico medico ? $"{medico.Nombre} {medico.Apellidos}" : "Médico desconocido";

                Console.WriteLine($@"
--- Entrada de Historial ---
    Fecha Consulta: {entrada.FechaConsulta:dd/MM/yyyy HH:mm}
    Cita ID: {entrada.IdCitaAsociada.ToString().Substring(0, 8)}
    Médico: {nombreMedico}
    Diagnóstico: {entrada.Diagnostico}
    Tratamiento: {entrada.Tratamiento}
    Notas/Recetas: {entrada.NotasMedicas}");
            }
        }
    }
}