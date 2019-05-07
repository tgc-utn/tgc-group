using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TGC.Core.SceneLoader;

namespace TGC.Group.Model.Resources.Meshes
{
    class SharkMesh
    {
        private static readonly TgcMesh Shark = new TgcSceneLoader()
                        .loadSceneFromFile(Game.Default.MediaDirectory + "shark-TgcScene.xml").Meshes[0];

        public static List<TgcMesh> All()
        {
            return new List<TgcMesh> { Shark };
        }
    }
}
