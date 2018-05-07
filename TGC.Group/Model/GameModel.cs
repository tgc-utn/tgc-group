using Microsoft.DirectX.DirectInput;
using System.Drawing;
using TGC.Core.Direct3D;
using TGC.Core.Example;
using TGC.Core.Geometry;
using TGC.Core.Input;
using TGC.Core.Mathematica;
using TGC.Core.SceneLoader;
using TGC.Core.Textures;
using TGC.Group.Model;
using TGC.Group.Model.Vehiculos;
using TGC.Core.Text;
using System.Collections.Generic;

namespace TGC.Group.Model
{
    public class GameModel : TgcExample
    {

        public GameModel(string mediaDir, string shadersDir) : base(mediaDir, shadersDir)
        {
            Category = Game.Default.Category;
            Name = Game.Default.Name;
            Description = Game.Default.Description;
        }

        private Camioneta auto;
        private CamaraEnTerceraPersona camaraInterna;
        private TGCVector3 camaraDesplazamiento = new TGCVector3(0,5,40);
        private TGCBox cubo;
        private TgcScene scene;
        private TgcText2D textoVelocidadVehiculo, textoOffsetH, textoOffsetF, textoPosicionVehiculo, textoVectorAdelante, textoVectorCostado;
        private TgcMesh jabon;
        private TgcMesh cama;
        private TgcMesh mesaDeLuz;
        private TgcMesh ropero;
        private TgcMesh armario;
        private TgcMesh escritorio;
        private TgcMesh silla;

        private List<TgcMesh> losMeshes = new List<TgcMesh>();

        public override void Init()
        {

            //en caso de querer cargar una escena
            TgcSceneLoader loader = new TgcSceneLoader();
            this.scene = loader.loadSceneFromFile(MediaDir + "Texturas\\Habitacion\\escenaFinal-TgcScene.xml");

            this.jabon = new TgcSceneLoader().loadSceneFromFile(MediaDir + "MeshCreator\\Meshes\\Bathroom\\Jabon\\Jabon-TgcScene.xml").Meshes[0];
            this.cama = new TgcSceneLoader().loadSceneFromFile(MediaDir + "MeshCreator\\Meshes\\Habitacion\\Cama\\Cama-TgcScene.xml").Meshes[0];
            cama.Move(new TGCVector3(-36f, 0, -124f));
            cama.RotateY(FastMath.PI);
            losMeshes.Add(cama);

            this.mesaDeLuz = this.dameMesh("MeshCreator\\Meshes\\Habitacion\\MesaDeLuz\\MesaDeLuz-TgcScene.xml", new TGCVector3(1, 1, 1), new TGCVector3(0, FastMath.PI, 0), new TGCVector3(22f, 0, -158f));
            this.ropero = this.dameMesh("MeshCreator\\Meshes\\Habitacion\\Placard\\Placard-TgcScene.xml", new TGCVector3(1, 1, 1), new TGCVector3(0, -FastMath.PI_HALF, 0), new TGCVector3(-205f ,0, -122f));
            this.armario = this.dameMesh("MeshCreator\\Meshes\\Habitacion\\Armario\\Armario-TgcScene.xml", new TGCVector3(1,1,1), new TGCVector3(0,-FastMath.PI_HALF,0), new TGCVector3(-30f,0,110f));
            this.escritorio = this.dameMesh("MeshCreator\\Meshes\\Habitacion\\Escritorio\\Escritorio-TgcScene.xml", new TGCVector3(1, 1, 1), new TGCVector3(0, 0, 0), new TGCVector3(183f,0f,-107f));

            //creo el vehiculo liviano
            //si quiero crear un vehiculo pesado (camion) hago esto
            // VehiculoPesado camion = new VehiculoPesado(rutaAMesh);
            // se hace esta distinción de vehiculo liviano y pesado por que cada uno tiene diferentes velocidades,
            // peso, salto, etc.
            this.auto = new Camioneta(MediaDir, new TGCVector3(0f, 0f, 0f));
            this.auto.mesh.AutoTransform = false;
            //creo un cubo para tomarlo de referencia (para ver como se mueve el auto)
            this.cubo = TGCBox.fromSize(new TGCVector3(-50, 10, -20), new TGCVector3(15, 15, 15), Color.Black);

            //creo la camara en tercera persona (la clase CamaraEnTerceraPersona hereda de la clase real del framework
            //que te permite configurar la posicion, el lookat, etc. Lo que hacemos al heredar, es reescribir algunos
            //metodos y setear valores default para que la camara quede mirando al auto en 3era persona

            this.camaraInterna = new CamaraEnTerceraPersona(auto.posicion() + camaraDesplazamiento, 0.8f, -33);
            this.Camara = camaraInterna;

        }

        public override void Update()
        {
            this.PreUpdate();
           
            if (Input.keyDown(Key.NumPad4))
            {
                this.camaraInterna.rotateY(-0.005f);
            }
            if (Input.keyDown(Key.NumPad6))
            {
                this.camaraInterna.rotateY(0.005f);
            }
            if (Input.keyDown(Key.RightArrow))
            {
                this.camaraInterna.OffsetHeight += 0.05f;
            }
            if (Input.keyDown(Key.LeftArrow))
            {
                this.camaraInterna.OffsetHeight -= 0.05f;
            }

            if (Input.keyDown(Key.UpArrow))
            {
                this.camaraInterna.OffsetForward += 0.05f;
            }
            if (Input.keyDown(Key.DownArrow))
            {
                this.camaraInterna.OffsetForward -= 0.05f;
            }

            string dialogo;

            dialogo = "Velocidad = {0}km";
            dialogo = string.Format(dialogo, auto.getVelocidadActual());
            textoVelocidadVehiculo = Textos.newText(dialogo, 120, 10);

            dialogo = "Posicion = ({0} | {1} | {2})";
            dialogo = string.Format(dialogo, auto.getPosicion().X, auto.getPosicion().Y, auto.getPosicion().Z);
            textoPosicionVehiculo = Textos.newText(dialogo, 120, 25);

            dialogo = "VectorAdelante = ({0} | {1} | {2})";
            dialogo = string.Format(dialogo, auto.getVectorAdelante().X, auto.getVectorAdelante().Y, auto.getVectorAdelante().Z);
            textoVectorAdelante = Textos.newText(dialogo, 120, 40);

            dialogo = "VectorCostado = ({0} | {1} | {2})";
            dialogo = string.Format(dialogo, auto.getVectorCostado().X, auto.getVectorCostado().Y, auto.getVectorCostado().Z);
            textoVectorCostado = Textos.newText(dialogo, 120, 55);

            dialogo = "OffsetHeight = {0}";
            dialogo = string.Format(dialogo, this.camaraInterna.OffsetHeight);
            textoOffsetH = Textos.newText(dialogo, 120, 70);

            dialogo = "OffsetForward = {0}";
            dialogo = string.Format(dialogo, this.camaraInterna.OffsetForward);
            textoOffsetF = Textos.newText(dialogo, 120, 85);

            this.auto.setElapsedTime(ElapsedTime);

            //si el usuario teclea la W y ademas no tecla la D o la A
            if (Input.keyDown(Key.W))
            {
                //hago avanzar al auto hacia adelante. Le paso el Elapsed Time que se utiliza para
                //multiplicarlo a la velocidad del auto y no depender del hardware del computador
                this.auto.getEstado().advance();

            }

            //lo mismo que para avanzar pero para retroceder
            if (Input.keyDown(Key.S))
            {
                this.auto.getEstado().back();
            }

            //si el usuario teclea D
            if (Input.keyDown(Key.D))
            {
                this.auto.getEstado().right(camaraInterna);
                
            }else if (Input.keyDown(Key.A))
            {
                this.auto.getEstado().left(camaraInterna);
            }

            //Si apreta espacio, salta
            if (Input.keyDown(Key.Space))
            {
                this.auto.getEstado().jump();
            }

            if (!Input.keyDown(Key.W) && !Input.keyDown(Key.S))
            {
                this.auto.getEstado().speedUpdate();
            }

            //esto es algo turbio que tengo que hacer, por que sino es imposible modelar el salto
            this.auto.getEstado().jumpUpdate();


            //Hacer que la camara siga al personaje en su nueva posicion
            this.camaraInterna.Target = (TGCVector3.transform(auto.posicion(), auto.transformacion)) + auto.getVectorAdelante() * 30 ;

            this.PostUpdate();
        }

        public override void Render()
        {

            this.PreRender();

            this.textoVelocidadVehiculo.render();

            this.textoPosicionVehiculo.render();
            this.textoVectorAdelante.render();
            this.textoVectorCostado.render();
            this.textoOffsetF.render();
            this.textoOffsetH.render();
            
            this.scene.RenderAll();
            
            this.auto.Transform();
            this.auto.Render();

            this.cubo.Transform =
                TGCMatrix.Scaling(cubo.Scale)
                            * TGCMatrix.RotationYawPitchRoll(cubo.Rotation.Y, cubo.Rotation.X, cubo.Rotation.Z)
                            * TGCMatrix.Translation(cubo.Position);
            this.cubo.Render();
            //this.jabon.Render();
            foreach (var mesh in losMeshes)
            {
                mesh.Render();
            }

            this.PostRender();
        }

        public override void Dispose()
        {
            //Dispose del auto.
            this.auto.dispose();
            //Dispose del cubo
            this.cubo.Dispose();
            //Dispose Scene
            this.scene.DisposeAll();
        }

        private TgcMesh dameMesh(string ruta, TGCVector3 escala, TGCVector3 rotacion, TGCVector3 traslado)
        {
            var mesh = new TgcSceneLoader().loadSceneFromFile(MediaDir + ruta).Meshes[0];
            mesh.Scale = escala;
            mesh.RotateX(rotacion.X);
            mesh.RotateY(rotacion.Y);
            mesh.RotateZ(rotacion.Z);
            mesh.Move(traslado);
            losMeshes.Add(mesh);
            return mesh;
        }
    }
}