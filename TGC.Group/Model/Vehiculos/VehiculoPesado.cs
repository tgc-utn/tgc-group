using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TGC.Group.Model.Vehiculos
{
    class VehiculoPesado : Vehiculo
    {
        public VehiculoPesado(string rutaAMesh) : base(rutaAMesh)
        {
            this.velocidadRotacion *= 0.7f;
        }
    }
}
