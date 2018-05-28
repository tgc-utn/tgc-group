using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TGC.Core.Input;

namespace TGC.Group.Model.Interfaz {
    class Boton : ElementoTexto {
        Action action;

        public Boton(string contenido, float xpos, float ypos, Action action) 
            : base(contenido, xpos, ypos) {
            this.action = action;
        }

        public override void Render() {
            base.Render();
        }

        public override void Update(float deltaTime, TgcD3dInput input) {
            if (input.buttonPressed(TgcD3dInput.MouseButtons.BUTTON_LEFT)) {
                if (input.Xpos <= getRect().X + getRect().Width &&
                    input.Xpos >= getRect().X &&
                    input.Ypos <= getRect().Y + getRect().Height &&
                    input.Ypos >= getRect().Y) {

                    action();
                }
            }
        }
    }
}
