using System;
namespace PerrijosGatijos.Models.Class
{
    public class UserModel
    {
        public string Nombre { get; set; }

        public string ApellidoPaterno { get; set; }

        public string Apellidomaterno { get; set; }

        public int Edad { get; set; }

        public string Direccion { get; set; }

        public Municipality Municipio { get; set; }

        public string CorreoElectronico { get; set; }

    }
}

