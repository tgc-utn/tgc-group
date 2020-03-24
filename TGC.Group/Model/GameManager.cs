using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TGC.Group.Model
{

    public sealed class GameManager
    {
        private readonly List<IRenderizable> Renderizables;


        public void Update(float elapsedTime)
        {
            Renderizables.ForEach(delegate (IRenderizable unRenderizable) { unRenderizable.Update(elapsedTime); });
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
