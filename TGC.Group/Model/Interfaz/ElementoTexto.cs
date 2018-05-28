using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TGC.Core.Direct3D;
using TGC.Core.Input;

namespace TGC.Group.Model.Interfaz {
    class ElementoTexto : Elemento {
        public ElementoTexto(string contenido, float xpos, float ypos) 
            : base(contenido, xpos, ypos) {
        }

        public override void Update(float deltaTime, TgcD3dInput input) {
        }

        public override void Render() {
            texto.render();
        }

        public override void Dispose() {
            texto.Dispose();
        }

    }
}
