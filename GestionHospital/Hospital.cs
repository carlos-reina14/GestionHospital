using System;
using System.Collections.Generic;

namespace GestionHospital
{
    public class Hospital
    {
        private readonly List<Persona> _personas = new List<Persona>();
        private readonly List<Cita> _citas = new List<Cita>();

        private readonly GestionPersonal _gestionPersonal;
        private readonly GestionCitas _gestionCitas;

        public Hospital()
        {
            _gestionPersonal = new GestionPersonal(_personas);
            _gestionCitas = new GestionCitas(_citas, _gestionPersonal);
        }

        public void IniciarAplicacion()
        {
            string opcion;
            do
            {
                MostrarMenuPrincipal();
                opcion = Console.ReadLine();
                TratarOpcionPrincipal(opcion);
            }
            while (opcion != "0");
        }

        private void MostrarMenuPrincipal()
        {
            Console.WriteLine(@"
--- Menú Principal del Hospital ---
1. Gestión de Personal
2. Gestión de Citas
0. Salir
Seleccione una opción: ");
        }

        private void TratarOpcionPrincipal(string opcion)
        {
            switch (opcion)
            {
                case "1": GestionarPersonal(); break;
                case "2": GestionarCitas(); break;
                case "0": Console.WriteLine("Saliendo de la aplicación. ¡Hasta pronto!"); return;
                default: Console.WriteLine("Opción no válida. Intente de nuevo."); break;
            }
            LimpiarPantalla();
        }

        private void GestionarPersonal()
        {
            string opcionPersonal;
            do
            {
                MostrarMenuGestionPersonal();
                opcionPersonal = Console.ReadLine();
                TratarOpcionGestionPersonal(opcionPersonal);
            }
            while (opcionPersonal != "0");
        }

        private void MostrarMenuGestionPersonal()
        {
            Console.WriteLine(@"
--- Gestión de Personal y Pacientes ---
1. Dar de alta un médico
2. Dar de alta un paciente
3. Dar de alta personal administrativo
4. Listar médicos
5. Listar pacientes de un médico
6. Eliminar un paciente
7. Ver la lista de todas las personas del hospital
8. Modificar datos de una persona
0. Volver al Menú Principal
Seleccione una opción: ");
        }

        private void TratarOpcionGestionPersonal(string opcionPersonal)
        {
            switch (opcionPersonal)
            {
                case "1": _gestionPersonal.DarDeAltaMedico(); break;
                case "2": _gestionPersonal.DarDeAltaPaciente(); break;
                case "3": _gestionPersonal.DarDeAltaPersonalAdministrativo(); break;
                case "4": _gestionPersonal.ListarMedicos(); break;
                case "5": _gestionPersonal.ListarPacientesDeMedico(); break;
                case "6": _gestionPersonal.EliminarPaciente(); break;
                case "7": _gestionPersonal.VerListaDePersonasDelHospital(); break;
                case "8": _gestionPersonal.ModificarDatosPersona(); break;
                case "0": Console.WriteLine("Volviendo al Menú Principal..."); return;
                default: Console.WriteLine("Opción no válida. Intente de nuevo."); break;
            }
            LimpiarPantalla();
        }

        private void LimpiarPantalla()
        {
            Console.WriteLine("\nPresione cualquier tecla para continuar...");
            Console.ReadKey();
            Console.Clear();
        }

        private void GestionarCitas()
        {
            string opcionCitas;
            do
            {
                MostrarMenuGestionCitas();
                opcionCitas = Console.ReadLine();
                TratarOpcionGestionCitas(opcionCitas);
            }
            while (opcionCitas != "0");
        }

        private void MostrarMenuGestionCitas()
        {
            Console.WriteLine(@"
--- Gestión de Citas ---
1. Programar una cita
2. Cancelar una cita
3. Modificar una cita
4. Listar todas las citas
5. Registrar historial médico de una cita
6. Ver historial médico de paciente
0. Salir");
        }

        private void TratarOpcionGestionCitas(string opcionCitas)
        {
            switch (opcionCitas)
            {
                case "1": _gestionCitas.ProgramarCita(); break;
                case "2": _gestionCitas.CancelarCita(); break;
                case "3": _gestionCitas.ModificarCita(); break;
                case "4": _gestionCitas.ListarCitas(); break;
                case "5": _gestionCitas.RegistrarHistorialMedico(); break;
                case "6": _gestionCitas.VerHistorialMedicoPaciente(); break;
                case "0": Console.WriteLine("Volviendo al Menú Principal..."); return;
                default: Console.WriteLine("Opción no válida. Intente de nuevo."); break;
            }
            LimpiarPantalla();
        }
    }
}