using Microsoft.DirectX.DirectInput;
using System;
using System.Collections.Generic;
using System.Drawing;
using TGC.Core.Mathematica;
using TGC.Core.SceneLoader;

namespace TGC.Group.Model
{
    public class EscenarioPlaya : Escenario
    {
        private TgcScene escena;
        
        private List<MeshTipoCaja> cajas;
        // Planos de limite

        public EscenarioPlaya(GameModel contexto, Personaje personaje) : base (contexto, personaje){
            
        }

        protected override void Init()
        {
            var MediaDir = contexto.MediaDir;
            var loader = new TgcSceneLoader();
            this.escena = loader.loadSceneFromFile(MediaDir + "primer-nivel\\Playa final\\Playa-TgcScene.xml");

            planoIzq = loader.loadSceneFromFile(MediaDir + "primer-nivel\\pozo-plataformas\\tgc-scene\\plataformas\\planoHorizontal-TgcScene.xml").Meshes[0];
            planoIzq.AutoTransform = false;

            planoDer = planoIzq.createMeshInstance("planoDer");
            planoDer.AutoTransform = false;
            planoDer.Transform = TGCMatrix.Translation(-12, 0, -22) * TGCMatrix.Scaling(1, 1, 2.2f);
            planoDer.BoundingBox.transform(planoDer.Transform);

            planoIzq.Transform = TGCMatrix.Translation(25, 0, -22) * TGCMatrix.Scaling(1, 1, 2.2f);
            planoIzq.BoundingBox.transform(planoIzq.Transform);

            planoFront = loader.loadSceneFromFile(MediaDir + "primer-nivel\\pozo-plataformas\\tgc-scene\\plataformas\\planoVertical-TgcScene.xml").Meshes[0];
            planoFront.AutoTransform = false;

            planoBack = planoFront.createMeshInstance("planoBack");
            planoBack.AutoTransform = false;
            planoBack.Transform = TGCMatrix.Translation(50, 0, 70);
            planoBack.BoundingBox.transform(planoBack.Transform);

            planoFront.Transform = TGCMatrix.Translation(50, 0, -197);
            planoFront.BoundingBox.transform(planoFront.Transform);

            planoPiso = loader.loadSceneFromFile(MediaDir + "primer-nivel\\pozo-plataformas\\tgc-scene\\plataformas\\planoPiso-TgcScene.xml").Meshes[0];
            planoPiso.AutoTransform = false;
            planoPiso.BoundingBox.transform(TGCMatrix.Scaling(1, 1, 2) * TGCMatrix.Translation(0, 0, 200));

            GenerarCajas();
        }

        private void GenerarCajas() {
            cajas = new List<MeshTipoCaja>();
            
            cajas.Add(new MeshTipoCaja());
        }

        public override void Render() {
            escena.RenderAll();
            cajas.ForEach((caja) => { caja.Render(); });

            if (contexto.BoundingBox) {
                cajas.ForEach((caja) => {caja.RenderizaRayos(); }) ;
                planoBack.BoundingBox.Render();
                planoFront.BoundingBox.Render();
                planoIzq.BoundingBox.Render();
                planoDer.BoundingBox.Render();
                planoPiso.BoundingBox.Render();
            }
        }

        public override void Update()
        {
            movimiento = personaje.movimiento;

            CalcularColisionesConPlanos();

            CalcularColisionesConMeshes();

            personaje.Movete(movimiento);
        }

        public override void CalcularColisionesConPlanos()
        {
            if (personaje.moving)
            {
                //personaje.playAnimation("Caminando", true); // esto creo que esta mal, si colisiono no deberia caminar.

                if (ChocoConLimite(personaje, planoIzq))
                    NoMoverHacia(Key.A);

                if (ChocoConLimite(personaje, planoBack))
                {
                    NoMoverHacia(Key.S);
                    planoBack.BoundingBox.setRenderColor(Color.AliceBlue);
                }
                else
                { // esto no hace falta despues
                    planoBack.BoundingBox.setRenderColor(Color.Yellow);
                }

                if (ChocoConLimite(personaje, planoDer))
                    NoMoverHacia(Key.D);

                if (ChocoConLimite(personaje, planoFront))
                { // HUBO CAMBIO DE ESCENARIO
                  /* Aca deberiamos hacer algo como no testear mas contra las cosas del escenario anterior y testear
                    contra las del escenario actual. 
                  */

                    planoFront.BoundingBox.setRenderColor(Color.AliceBlue);
                }
                else
                {
                    planoFront.BoundingBox.setRenderColor(Color.Yellow);
                }

                if (ChocoConLimite(personaje, planoPiso))
                {
                    if (movimiento.Y < 0)
                    {
                        movimiento.Y = 0; // Ojo, que pasa si quiero saltar desde arriba de la plataforma?
                        personaje.ColisionoEnY();
                    }
                }
            }
        }

        public override void CalcularColisionesConMeshes()
        {
            if (personaje.moving)
            {
                foreach (MeshTipoCaja caja in cajas)
                {
                    if (caja.ChocoConFrente(personaje))
                    {
                        var movimientoCaja = TGCMatrix.Translation(0, 0, movimiento.Z * 3); // + distancia minima del rayo
                        caja.Update(movimientoCaja);
                        break;
                    }
                    else if (caja.ChocoAtras(personaje))
                    {
                        NoMoverHacia(Key.S);
                        break;
                    }
                    else if (caja.ChocoALaIzquierda(personaje))
                    {
                        NoMoverHacia(Key.D);
                        break;
                    }
                    else if (caja.ChocoALaDerecha(personaje))
                    {
                        NoMoverHacia(Key.A);
                        break;
                    }
                    else if (caja.ChocoArriba(personaje))
                    {
                        if (movimiento.Y < 0)
                        {
                            movimiento.Y = 0; // Ojo, que pasa si quiero saltar desde arriba de la plataforma?
                            personaje.ColisionoEnY();
                        }
                        break;
                    }
                }
            }
        }
    }
}