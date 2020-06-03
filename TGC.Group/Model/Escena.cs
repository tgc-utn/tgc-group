using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TGC.Core.Camara;
using TGC.Core.Input;
using TGC.Core.Text;

namespace TGC.Group.Model
{
    abstract class Escena
    {
        // Comun
        protected TgcCamera Camera;
        protected String MediaDir;
        protected TgcText2D DrawText;

        protected float TimeBetweenUpdates;
        protected TgcD3dInput Input;

        public Escena(TgcCamera Camera, String MediaDir, TgcText2D DrawText, float TimeBetweenUpdates, TgcD3dInput Input)
        {
            this.Camera = Camera;
            this.MediaDir = MediaDir;
            this.DrawText = DrawText;
            this.TimeBetweenUpdates = TimeBetweenUpdates;
            this.Input = Input;
        }

        public abstract Escena Update(float ElapsedTime);
        public abstract void Render();
        public abstract void Dispose();

        protected Escena CambiarEscena(Escena escena)
        {
            this.Dispose();
            return escena;
        }
    }
}
