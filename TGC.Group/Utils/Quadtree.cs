using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;
using System;
using System.Collections.Generic;
using System.Linq;
using TGC.Core.BoundingVolumes;
using TGC.Core.Collision;
using TGC.Core.Geometry;
using TGC.Core.SceneLoader;
using TGC.Core.Shaders;
using TGC.Core.Utils;
using TGC.Group.Model;

namespace TGC.UtilsGroup
{
    /// <summary>
    ///     Herramienta para crear y utilizar un Quadtree para renderizar por Frustum Culling
    /// </summary>
    public class Quadtree
    {
        private readonly QuadtreeBuilder builder;
        private List<TgcBoxDebug> debugQuadtreeBoxes;
        private List<TgcMesh> modelos;
        private QuadtreeNode quadtreeRootNode;
        private TgcBoundingAxisAlignBox sceneBounds;

        public Quadtree()
        {
            builder = new QuadtreeBuilder();
        }

        /// <summary>
        ///     Crear nuevo Quadtree
        /// </summary>
        /// <param name="modelos">Modelos a optimizar</param>
        /// <param name="sceneBounds">Límites del escenario</param>
        public void create(List<TgcMesh> modelos, TgcBoundingAxisAlignBox sceneBounds)
        {
            this.modelos = modelos;
            this.sceneBounds = sceneBounds;

            //Crear Quadtree
            quadtreeRootNode = builder.crearQuadtree(modelos, sceneBounds);

            //Deshabilitar todos los mesh inicialmente
            foreach (var mesh in modelos)
            {
                mesh.Enabled = false;
            }
        }

        /// <summary>
        ///     Crear meshes para debug
        /// </summary>
        public void createDebugQuadtreeMeshes()
        {
            debugQuadtreeBoxes = builder.createDebugQuadtreeMeshes(quadtreeRootNode, sceneBounds);
        }

        /// <summary>
        ///     Renderizar en forma optimizado utilizando el Quadtree para hacer FrustumCulling
        /// </summary>
        public void render(TgcFrustum frustum, bool debugEnabled, string Technique, int soloObjetos, Effect unEfecto)
        {
            var pMax = sceneBounds.PMax;
            var pMin = sceneBounds.PMin;
            int contador = 0;

            findVisibleMeshes(frustum, quadtreeRootNode,
                pMin.X, pMin.Y, pMin.Z,
                pMax.X, pMax.Y, pMax.Z, contador);

            //Renderizar
            foreach (var mesh in modelos)
            {
                if (soloObjetos == 2)
                {
                    if (mesh.Enabled)
                    {
                        if ((mesh.Name == "Pasto") || (mesh.Name == "Plane_5") || (mesh.Name == "Room-1-Floor-0"))
                        {
                            mesh.Effect = unEfecto;
                            mesh.Technique = Technique;
                            mesh.render();
                            mesh.Enabled = false;
                        }
                    }
                }
                else
                {
                    if ((soloObjetos == 1) &&
                        ((mesh.Name.IndexOf("Room") != -1) || (mesh.Name == "Pasto"))
                        )
                        continue;
                    else
                    {
                        if (mesh.Enabled)
                        {
                            if (mesh.Name.IndexOf("PowerUp") == -1)
                            {
                                if (Technique == "")
                                {
                                    mesh.Effect = TgcShaders.Instance.TgcMeshShader;
                                    mesh.Technique = TgcShaders.Instance.getTgcMeshTechnique(mesh.RenderType);
                                }
                                else
                                {

                                    mesh.Effect = unEfecto;
                                    mesh.Technique = Technique;
                                }
                            }

                            mesh.render();
                            mesh.Enabled = false;
                        }
                    }
                }
            }

            if (debugEnabled)
            {
                foreach (var debugBox in debugQuadtreeBoxes)
                {
                    debugBox.render();
                }
            }
        }

        /// <summary>
        ///     Recorrer recursivamente el Quadtree para encontrar los nodos visibles
        /// </summary>
        private void findVisibleMeshes(TgcFrustum frustum, QuadtreeNode node,
            float boxLowerX, float boxLowerY, float boxLowerZ,
            float boxUpperX, float boxUpperY, float boxUpperZ,
            int contador)
        {
            var children = node.children;

            //es hoja, cargar todos los meshes
            if (children == null)
            {
                selectLeafMeshes(node, contador);
                contador++;
            }

            //recursividad sobre hijos
            else
            {
                var midX = FastMath.Abs((boxUpperX - boxLowerX) / 2);
                var midZ = FastMath.Abs((boxUpperZ - boxLowerZ) / 2);

                //00
                testChildVisibility(frustum, children[0], boxLowerX + midX, boxLowerY, boxLowerZ + midZ, boxUpperX,
                    boxUpperY, boxUpperZ, contador);

                //01
                testChildVisibility(frustum, children[1], boxLowerX + midX, boxLowerY, boxLowerZ, boxUpperX, boxUpperY,
                    boxUpperZ - midZ, contador);

                //10
                testChildVisibility(frustum, children[2], boxLowerX, boxLowerY, boxLowerZ + midZ, boxUpperX - midX,
                    boxUpperY, boxUpperZ, contador);

                //11
                testChildVisibility(frustum, children[3], boxLowerX, boxLowerY, boxLowerZ, boxUpperX - midX, boxUpperY,
                    boxUpperZ - midZ, contador);
            }
        }

        /// <summary>
        ///     Hacer visible las meshes de un nodo si es visible por el Frustum
        /// </summary>
        private void testChildVisibility(TgcFrustum frustum, QuadtreeNode childNode,
            float boxLowerX, float boxLowerY, float boxLowerZ, float boxUpperX, float boxUpperY, float boxUpperZ, int contador)
        {
            //test frustum-box intersection
            var caja = new TgcBoundingAxisAlignBox(
                new Vector3(boxLowerX, boxLowerY, boxLowerZ),
                new Vector3(boxUpperX, boxUpperY, boxUpperZ));
            var c = TgcCollisionUtils.classifyFrustumAABB(frustum, caja);

            //complementamente adentro: cargar todos los hijos directamente, sin testeos
            if (c == TgcCollisionUtils.FrustumResult.INSIDE)
            {
                addAllLeafMeshes(childNode, contador);
            }

            //parte adentro: seguir haciendo testeos con hijos
            else if (c == TgcCollisionUtils.FrustumResult.INTERSECT)
            {
                findVisibleMeshes(frustum, childNode, boxLowerX, boxLowerY, boxLowerZ, boxUpperX, boxUpperY, boxUpperZ, contador);
            }
        }

        /// <summary>
        ///     Hacer visibles todas las meshes de un nodo, buscando recursivamente sus hojas
        /// </summary>
        private void addAllLeafMeshes(QuadtreeNode node, int contador)
        {
            var children = node.children;

            //es hoja, cargar todos los meshes
            if (children == null)
            {
                selectLeafMeshes(node, contador);
            }
            //pedir hojas a hijos
            else
            {
                for (var i = 0; i < children.Length; i++)
                {
                    addAllLeafMeshes(children[i], contador);
                }
            }
        }

        /// <summary>
        ///     Hacer visibles todas las meshes de un nodo
        /// </summary>
        private void selectLeafMeshes(QuadtreeNode node, int contador)
        {
            var models = node.models;

            foreach (var m in models)
            {
                m.Enabled = true;

                if (contador == 0)
                {
                    foreach (var unAuto in GameModel.ListaMeshAutos)
                    {
                        if ((m.Name != "Room-1-Roof-0") && (m.Name != "Room-1-Floor-0") &&
                        (m.Name != "Pasto") && (m.Name != "Plane_5"))
                        {
                            //me fijo si hubo alguna colision
                             if (TgcCollisionUtils.testObbAABB(unAuto.ObbMesh, m.BoundingBox))
                             {
                                if (m.Name.IndexOf("PowerUp Vida") != -1)
                                {
                                    m.Enabled = false;

                                    if (modelos.IndexOf(m) != -1)
                                    {
                                        modelos.Remove(m);
                                        unAuto.ModificadorVida += 4f;
                                        unAuto.ReproducirSonidoPowerUpVida();
                                    }

                                    unAuto.colisiono = false;

                                    continue;
                                }

                                unAuto.meshColisionado = m;
                                unAuto.colisiono = true;

                                if ((m.Name.IndexOf ("Palmera") != -1) || (m.Name.IndexOf ("Pino") != -1) || (m.Name.IndexOf("ArbolBananas") != -1))
                                    unAuto.ModificadorVida = -5;

                                if ((m.Name.IndexOf("Roca") != -1) || (m.Name.IndexOf("Estructura") != -1) || (m.Name.IndexOf("Glorieta") != -1))
                                    unAuto.ModificadorVida = -2.5f;
                            }
                        }
                    }
                }
            }
        }
    }
}