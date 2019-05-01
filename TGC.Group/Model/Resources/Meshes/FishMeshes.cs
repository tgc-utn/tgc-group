using System.Collections.Generic;
using TGC.Core.SceneLoader;

namespace TGC.Group.Model.Resources.Meshes
{
    static class FishMeshes
    {
        private static readonly TgcMesh Fish = new TgcSceneLoader()
            .loadSceneFromFile(Game.Default.MediaDirectory + "yellow_fish-TgcScene.xml").Meshes[0];

        public static List<TgcMesh> All()
        {
            return new List<TgcMesh>{Fish};
        }
    }
}