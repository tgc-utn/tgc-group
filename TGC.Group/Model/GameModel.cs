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

namespace TGC.Group.Model
{
    /// <summary>
    ///     Ejemplo para implementar el TP.
    ///     Inicialmente puede ser renombrado o copiado para hacer más ejemplos chicos, en el caso de copiar para que se
    ///     ejecute el nuevo ejemplo deben cambiar el modelo que instancia GameForm <see cref="Form.GameForm.InitGraphics()" />
    ///     line 97.
    /// </summary>
    public class GameModel : TgcExample
    {
        /// <summary>
        ///     Constructor del juego.
        /// </summary>
        /// <param name="mediaDir">Ruta donde esta la carpeta con los assets</param>
        /// <param name="shadersDir">Ruta donde esta la carpeta con los shaders</param>
        public GameModel(string mediaDir, string shadersDir) : base(mediaDir, shadersDir)
        {
            Category = Game.Default.Category;
            Name = Game.Default.Name;
            Description = Game.Default.Description;
        }

        private VehiculoLiviano auto;
        private CamaraEnTerceraPersona camaraInterna;
        private TGCBox cubo;
        private TgcPlane piso;

        public override void Init()
        {
            //Crear piso
            var pisoTexture = TgcTexture.createTexture(D3DDevice.Instance.Device, MediaDir + "Texturas\\pasto.jpg");
            piso = new TgcPlane(new TGCVector3(-500, 0, -500), new TGCVector3(1000, 0, 1000), TgcPlane.Orientations.XZplane, pisoTexture);

            //creo el vehiculo liviano
            //si quiero crear un vehiculo pesado (camion) hago esto
            // VehiculoPesado camion = new VehiculoPesado(rutaAMesh);
            // se hace esta distinción de vehiculo liviano y pesado por que cada uno tiene diferentes velocidades,
            // peso, salto, etc.
            auto = new VehiculoLiviano(MediaDir + "meshCreator\\meshes\\Vehiculos\\Camioneta\\Camioneta-TgcScene.xml");
            //lo escale por que el vehiculo se veia muy chico. pero se podria acercar la camara de ultima
            auto.escalar(new TGCVector3(0.1f, 0.1f, 0.1f));
            //lo roto 180 grados por que sino, el frente queda mirando a la camara
            auto.rotarEnY(FastMath.PI);

            //creo un cubo para tomarlo de referencia (para ver como se mueve el auto)
            cubo = TGCBox.fromSize(new TGCVector3(-50, 10, -20), new TGCVector3(15, 15, 15), Color.Black);

            //creo la camara en tercera persona (la clase CamaraEnTerceraPersona hereda de la clase real del framework
            //que te permite configurar la posicion, el lookat, etc. Lo que hacemos al heredar, es reescribir algunos
            //metodos y setear valores default para que la camara quede mirando al auto en 3era persona
            camaraInterna = new CamaraEnTerceraPersona(auto.posicion(), 30, -75);
            Camara = camaraInterna;

        }

        public override void Update()
        {
            PreUpdate();

            //si el usuario teclea la W y ademas no tecla la D o la A
            if (Input.keyDown(Key.W) && !(Input.keyDown(Key.D) || Input.keyDown(Key.A)))
            {
                //hago avanzar al auto hacia adelante. Le paso el Elapsed Time que se utiliza para
                //multiplicarlo a la velocidad del auto y no depender del hardware del computador
                auto.avanzar(ElapsedTime);

            }

            //lo mismo que para avanzar pero para retroceder
            if (Input.keyDown(Key.S) && !(Input.keyDown(Key.D) || Input.keyDown(Key.A)))
            {
                auto.retroceder(ElapsedTime);
            }

            //si el usuario teclea D
            if (Input.keyDown(Key.D))
            {
                //pregunto si además teclea W (así tomo la decisión de girar a la derecha) o
                // si tecleo S (asi tomo la decision de girar a la derecha en retroceso)
                //le paso la camara para que una vez que el auto gire un determinado angulo,
                //la camara acompañe ese giro
                if(Input.keyDown(Key.W)){
                    auto.avanzarHaciaLaDerecha(ElapsedTime, camaraInterna);
                }
                else if (Input.keyDown(Key.S)) {
                    auto.retrocederHaciaLaDerecha(ElapsedTime, camaraInterna);
                }
            }

            //lo mismo que arriba
            if (Input.keyDown(Key.A))
            {
                if (Input.keyDown(Key.W))
                {
                    auto.avanzarHaciaLaIzquierda(ElapsedTime, camaraInterna);
                }
                else if (Input.keyDown(Key.S))
                {
                    auto.retrocederHaciaLaIzquierda(ElapsedTime, camaraInterna);
                }
            }

            //Si apreta espacio, salta
            if (Input.keyDown(Key.Space))
            {
                auto.saltar(ElapsedTime);
            }

            //esto es algo turbio que tengo que hacer, por que sino es imposible modelar el salto
            auto.actualizarSalto(ElapsedTime);


            //Hacer que la camara siga al personaje en su nueva posicion
            camaraInterna.Target = auto.posicion();  

            PostUpdate();
        }

        public override void Render()
        {
            
            PreRender();

            piso.Render();

            auto.transformar();
            
            auto.Render();

            cubo.Transform =
                TGCMatrix.Scaling(cubo.Scale)
                            * TGCMatrix.RotationYawPitchRoll(cubo.Rotation.Y, cubo.Rotation.X, cubo.Rotation.Z)
                            * TGCMatrix.Translation(cubo.Position);
            cubo.Render();

            PostRender();
        }

        public override void Dispose()
        {
            //Dispose del auto.
            auto.dispose();

            //Dispose del cubo
            cubo.Dispose();
        }
    }
}