namespace GestionHospital
{
    public class Program
    {
        static void Main()
        {
            InicioPruebas();
        }

        public static void InicioPruebas()
        {
            Hospital hospital = new Hospital();
            hospital.IniciarAplicacion();
        }
    }
}