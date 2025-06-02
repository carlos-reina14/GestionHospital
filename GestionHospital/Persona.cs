using System;
using System.Text.RegularExpressions;

namespace GestionHospital
{
    public abstract class Persona
    {
        private string _dni { get; set; }

        public string Dni
        {
            get { return _dni; }
            set
            {
                if (!EsDniValido(value))
                    throw new ArgumentException("Formato de DNI no válido. Debe tener 8 números seguidos de 1 letra (ej. 12345678A).");
                _dni = value.ToUpper();
            }
        }
        public string Nombre { get; set; }
        public string Apellidos { get; set; }

        protected Persona(string nombre, string apellidos, string dni)
        {
            Nombre = nombre;
            Apellidos = apellidos;
            Dni = dni;
        }

        private static bool EsDniValido(string dni)
        {
            if (string.IsNullOrEmpty(dni))
                return false;
            return Regex.IsMatch(dni, @"^\d{8}[A-Za-z]$");
        }

        public override string ToString()
        {
            return $"{Nombre} {Apellidos} - DNI: {Dni}";
        }
    }
}