using System.Collections.Generic;
using TGC.Core.SceneLoader;

namespace TGC.Group.Model.Resources.Meshes
{
    public static class CoralMeshes
    {
        private static readonly TgcMesh BrainCoral = new TgcSceneLoader()
            .loadSceneFromFile(Game.Default.MediaDirectory + "brain_coral-TgcScene.xml").Meshes[0];
        private static readonly TgcMesh Coral = new TgcSceneLoader()
            .loadSceneFromFile(Game.Default.MediaDirectory + "coral-TgcScene.xml").Meshes[0];
        private static readonly TgcMesh PillarCoral = new TgcSceneLoader()
            .loadSceneFromFile(Game.Default.MediaDirectory + "pillar_coral-TgcScene.xml").Meshes[0];
        private static readonly TgcMesh TreeCoral = new TgcSceneLoader()
            .loadSceneFromFile(Game.Default.MediaDirectory + "tree_coral-TgcScene.xml").Meshes[0];

        public static List<TgcMesh> All()
        {
            return new List<TgcMesh> {BrainCoral, Coral, PillarCoral, TreeCoral};
        }
    }
}