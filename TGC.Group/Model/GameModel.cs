using Microsoft.DirectX;
using Microsoft.DirectX.DirectInput;
using System.Drawing;
using TGC.Core.Direct3D;
using TGC.Core.Example;
using TGC.Core.Geometry;
using TGC.Core.Input;
using TGC.Core.SceneLoader;
using TGC.Core.Textures;
using TGC.Core.Utils;

namespace TGC.Group.Model
{
    /// <summary>
    ///     Ejemplo para implementar el TP. 
    ///     Inicialmente puede ser renombrado o copiado para hacer mas ejemplos pequeños, 
    ///     en el caso de copiar para que se ejecute el ejemplo deben cambiar el momento 
    ///     de instanciacion en <see cref="Form.GameForm.InitGraphics()"/> line 97.
    /// </summary>
    public class GameModel : TgcExample
    {
        //Caja que se muestra en el ejemplo.
        private TgcBox Box { get; set; }
        
        //Mesh de TgcLogo.
        private TgcMesh Mesh { get; set; }

        //Boleano para ver si dibujamos el boundingbox
        private bool BoundingBox { get; set; }

        /// <summary>
        /// Constructor del juego.
        /// </summary>
        /// <param name="mediaDir">Ruta donde esta la carpeta con los assets</param>
        /// <param name="shadersDir">Ruta donde esta la carpeta con los shaders</param>
        public GameModel(string mediaDir, string shadersDir) : base(mediaDir, shadersDir)
        {
            Category = Game.Default.Category;
            Name = Game.Default.Name;
            Description = Game.Default.Description;
        }

        /// <summary>
        ///     Se llama una sola vez, al principio cuando se ejecuta el ejemplo.
        ///     Escribir aquí todo el código de inicialización: cargar modelos, texturas, estructuras de optimizacion, 
        ///     todo procesamiento que podemos precalcular para nuestro juego.
        ///     
        ///     Borrar el codigo ejemplo no utilizado.
        /// </summary>
        public override void Init()
        {
            //Device de DirectX para crear primitivas.
            var d3dDevice = D3DDevice.Instance.Device;

			//Textura de la carperta Media. Game.Default es un archivo de configuracion (Game.settings) util para poner cosas.
            //Pueden abrir el Game.settings que se ubica dentro de nuestro proyecto para configurar.
            var pathTexturaCaja = MediaDir + Game.Default.TexturaCaja;

            //Cargamos una textura, tener en cuenta que cargar una textura significa crear una copia en memoria.
            //Es importante cargar texturas en Init, en un loop de render podemos tener grandes problemas si instanciamos muchas.
            var texture = TgcTexture.createTexture(pathTexturaCaja);

            //Creamos una caja 3D ubicada en (0, 0, 0), dimensiones (5, 10, 5) y la textura como color.
            var center = new Vector3(0, 0, 0);
            var size = new Vector3(5, 10, 5);
            //Construimos una caja segun los parametros, recomendamos utilizar siempre el centro en el origen, para facilitar las transformaciones.
            Box = TgcBox.fromSize(center, size, texture);
            //Posicion donde quiero que este la caja, es comun que se utilizen estructuras internas para las transformaciones.
            //Entonces actualizamos la posicion logica, luego podemos utilizar esto en render para posicionar donde corresponda con transformaciones.
            Box.Position = new Vector3(-25, 0, 0);
			
            //Cargo el unico mesh que tiene la escena.
            Mesh = new TgcSceneLoader().loadSceneFromFile(MediaDir + "LogoTGC-TgcScene.xml").Meshes[0];
            //Defino una escala en el modelo logico del mesh que es muy grande.
			Mesh.Scale = new Vector3(0.5f, 0.5f, 0.5f);

			//Suelen utilizarse objetos que manejan el comportamiento de la camara.
            //Lo que en realidad necesitamos graficamente es una matriz de View.
            //El framework maneja una camara estatica, pero debe ser inicializada.
            //Posicion de la camara. 
            Vector3 cameraPosition = new Vector3(0, 0, 125);
			//Quiero que la camara mire hacia el origen (0,0,0).
			Vector3 lookAt = Vector3.Empty;
			//Configuro donde esta la posicion de la camara y hacia donde mira.
			Camara.SetCamera(cameraPosition, lookAt);
            //Internamente el framework construye la matriz de view con estos dos vectores.
            //Luego en nuestro juego tendremos que crear una camara que cambie la matriz de 
            //view con variables como movimientos o animaciones de esenas.
        }

        /// <summary>
        /// Se llama en cada frame.
        /// Se debe escribir toda la logica de computo del modelo, 
        /// asi como tambien verificar entradas del usuario y reacciones ante ellas..
        /// </summary>
        public override void Update()
        {
            PreUpdate();

            //Capturar Input teclado
            if (Input.keyPressed(Key.F))
            {
                BoundingBox = !BoundingBox;
            }

            //Capturar Input Mouse
            if (Input.buttonUp(TgcD3dInput.MouseButtons.BUTTON_LEFT))
            {
                //Como ejemplo podemos hacer un movimiento simple de la camara.
                //en este caso le sumamos un valor en Y
                Camara.SetCamera(Camara.Position+new Vector3(0, 10f, 0), Camara.LookAt);
                //Ver ejemplos de camara para otras operaciones posibles.

                //Si superamos cierto Y volvemos a la posicion original.
                if (Camara.Position.Y > 300f)
                    Camara.SetCamera(new Vector3(Camara.Position.X, 0f, Camara.Position.Z), Camara.LookAt);
            }
        }

        /// <summary>
        ///     Se llama cada vez que hay que refrescar la pantalla.
        ///     Escribir aquí todo el código referido al renderizado.
        ///     Borrar todo lo que no haga falta.
        /// </summary>
        public override void Render()
        {
            //Inicio el render de la escena, para ejemplos simples. cuando tenemos postprocesado 
            //o shaders es mejor realizar las operaciones segun nuestra conveniencia.
            PreRender();

            //Dibuja un texto por pantalla
            DrawText.drawText("Con la tecla F se dibuja el bounding box.", 0, 20, Color.OrangeRed);
            DrawText.drawText("Con clic izquierdo subimos la camara [Actual]: "+TgcParserUtils.printVector3(Camara.Position), 0, 30, Color.OrangeRed);

            //Siempre antes de renderizar el modelo necesitamos actualizar la matriz de transformacion.
            //Debemos recordar el orden en cual debemos multiplicar las matrices,
            //en caso de tener modelos herarquicos, tenemos control total.
            Box.Transform = Matrix.Scaling(Box.Scale) *
                            Matrix.RotationYawPitchRoll(Box.Rotation.Y, Box.Rotation.X, Box.Rotation.Z) *
                            Matrix.Translation(Box.Position);
            //A modo ejemplo realizamos toda las multiplicaciones, pero aqui solo nos hacia falta la traslacion.
            //Finalmente invocamos al render de la caja
            Box.render();

            //Cuando tenemos modelos mesh podemos utilizar un metodo que hace la matriz de transformacion estandar.
            //es util cuando tenemos transformaciones simples, pero OJO cuando tenemos transformaciones herarquicas o complciadas.
            Mesh.UpdateMeshTransform();
            //Render del mesh
            Mesh.render();

            //Render de BoundingBox, muy util para debug de colisiones.
            if (BoundingBox)
            {
                Box.BoundingBox.render();
                Mesh.BoundingBox.render();
            }

            //Finaliza el render y presenta en pantalla, al igual que el preRender se debe para 
            //casos puntuales es mejor utilizar a mano las ooperaciones de EndScene y PresentScene
            PostRender();
        }

        /// <summary>
        ///     Se llama cuando termina la ejecución del ejemplo.
        ///     Hacer Dispose() de todos los objetos creados.
        ///     Es muy imporatante liverar los recursos, sobretodo los graficos ya que quedan bloqueados en el device de video.
        /// </summary>
        public override void Dispose()
        {
            //Dispose de la caja.
            Box.dispose();
            //Dispose del mesh.
            Mesh.dispose();
        }
    }
}