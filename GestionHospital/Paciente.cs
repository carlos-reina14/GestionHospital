using System;
using System.Text.RegularExpressions;

namespace GestionHospital
{
    public class Paciente : Persona
    {
        private string _numeroHistoriaClinica;

        public string NumeroHistoriaClinica
        {
            get { return _numeroHistoriaClinica; }
            private set
            {
                if (!EsNumeroHistoriaClinicaValido(value))
                    throw new ArgumentException("Formato de Número de Historia Clínica no válido. Debe ser así: HCL-YYYY-0000");
                _numeroHistoriaClinica = value;
            }
        }
        public string Sintomas { get; set; }
        public string DniMedico { get; set; }

        public Paciente(string nombre, string apellidos, string dni, string numeroHistoriaClinica, string sintomas, string dniMedico)
            : base(nombre, apellidos, dni)
        {
            NumeroHistoriaClinica = numeroHistoriaClinica;
            Sintomas = sintomas;
            DniMedico = dniMedico;
        }

        private static bool EsNumeroHistoriaClinicaValido(string numeroHistoriaClinica)
        {
            if (string.IsNullOrEmpty(numeroHistoriaClinica))
                return false;
            return Regex.IsMatch(numeroHistoriaClinica, @"^HCL-\d{4}-\d{4}$");
        }

        public override string ToString()
        {
            return $"{base.ToString()} - Nº Historia Clínica: {NumeroHistoriaClinica} - Síntomas: {Sintomas} - Médico: {DniMedico}";
        }
    }
}