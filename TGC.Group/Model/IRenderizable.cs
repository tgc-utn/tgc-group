using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TGC.Group.Model
{
    public interface IRenderizable
    {

        /// <summary>
        /// Estos 3 metodos de Update, Render y Dispose se llaman igual que los de GameModel pero no son llamados cada frame automaticamente.
        /// Lo que quiero decir es que lo unico que tienen en comun es el nombre y la logica que por convencion llevan adentro.
        /// El Update y el Render NO tienen que usar PreUpdate() y PostUpdate() ( o PreRender() y PostRender() ) al principio y al final de los metodos.
        /// <summary>

        void Init();

        void Update(float elapsedTime);

        void Render();

        void Dispose();

        Boolean SePuedeRenderizar();
    }
}
