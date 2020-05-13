using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TGC.Examples.Camara;

namespace TGC.Group.Model
{

    internal class GameManager
    {
        private List<IRenderizable> Renderizables = new List<IRenderizable>();
        public Camara Camara { get; set; }

        public void Update(float elapsedTime)
        {
            List<IRenderizable> RenderizablesAuxiliar = new List<IRenderizable>(Renderizables);
            RenderizablesAuxiliar.ForEach(delegate (IRenderizable unRenderizable) { unRenderizable.Update(elapsedTime); });
            Camara.Update(elapsedTime);
        }
        public void Render()
        {
            Renderizables.ForEach(delegate (IRenderizable unRenderizable) { unRenderizable.Render(); });
        }

        public void Dispose()
        {
            Renderizables.ForEach(delegate (IRenderizable unRenderizable) { unRenderizable.Dispose(); });
        }

        public void AgregarRenderizable(IRenderizable unRenderizable)
        {
            Renderizables.Add(unRenderizable);
            unRenderizable.Init();
        }

        public void QuitarRenderizable(IRenderizable unRenderizable)
        {
            Renderizables.Remove(unRenderizable);
            unRenderizable.Dispose();
        }





        #region Singleton

        private static volatile GameManager instance;
        private static readonly object syncRoot = new object();

        /// <summary>
        ///     Permite acceder a una instancia de la clase GameManager desde cualquier parte del codigo.
        /// </summary>
        public static GameManager Instance
        {
            get
            {
                lock (syncRoot)
                {
                    if (instance == null)
                        instance = new GameManager();
                }
                return instance;
            }
        }

        #endregion Singleton

    }
}
