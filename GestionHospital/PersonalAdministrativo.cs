namespace GestionHospital
{
    public class PersonalAdministrativo : Persona
    {
        public string Cargo { get; set; }
        public string Departamento { get; set; }
        
        public PersonalAdministrativo(string nombre, string apellidos, string dni, string cargo, string departamento)
            : base(nombre, apellidos, dni)
        {
            Cargo = cargo;
            Departamento = departamento;
        }

        public override string ToString()
        {
            return $"{base.ToString()} - Cargo: {Cargo} - Departamento: {Departamento}";
        }
    }
}