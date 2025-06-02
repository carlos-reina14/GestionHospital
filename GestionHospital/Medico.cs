using System;
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

        public Medico(string nombre, string apellidos, string dni, string especialidad, string numeroColegiado)
            : base(nombre, apellidos, dni)
        {
            Especialidad = especialidad;
            NumeroColegiado = numeroColegiado;
        }

        private static bool EsNumeroColegiadoValido(string numeroColegiado)
        {
            if (string.IsNullOrEmpty(numeroColegiado))
                return false;
            return Regex.IsMatch(numeroColegiado, @"^\d{9}$");
        }

        public override string ToString()
        {
            return $"{base.ToString()} - Especialidad: {Especialidad} - Nº Colegiado: {NumeroColegiado}";
        }
    }
}