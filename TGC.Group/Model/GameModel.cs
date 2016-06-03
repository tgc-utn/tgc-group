using Microsoft.DirectX;
using Microsoft.DirectX.DirectInput;
using TGC.Core;
using TGC.Core.Camara;
using TGC.Core.Direct3D;
using TGC.Core.Example;
using TGC.Core.Geometry;
using TGC.Core.Input;
using TGC.Core.Textures;
using TGC.Core.UserControls;
using TGC.Core.UserControls.Modifier;

namespace TGC.Group.Model
{
    /// <summary>
    ///     Ejemplo para implementar el TP
    /// </summary>
    public class GameModel : TgcExample
    {
        //Caja que se muestra en el ejemplo
        public TgcBox Box { get; set; }

        public GameModel(string mediaDir, string shadersDir, TgcUserVars userVars, TgcModifiers modifiers,
            TgcAxisLines axisLines, TgcCamera camara)
            : base(mediaDir, shadersDir, userVars, modifiers, axisLines, camara)
        {
            Category = Game.Default.Category;
            Name = Game.Default.Name;
            Description = Game.Default.Description;
        }

        /// <summary>
        ///     Método que se llama una sola vez, al principio cuando se ejecuta el ejemplo.
        ///     Escribir aquí todo el código de inicialización: cargar modelos, texturas, modifiers, uservars, etc.
        ///     Borrar todo lo que no haga falta.
        /// </summary>
        public override void Init()
        {
            //Device de DirectX para crear primitivas
            var d3dDevice = D3DDevice.Instance.Device;

            ///////////////CONFIGURAR CAMARA ROTACIONAL//////////////////
            //Es la camara que viene por default, asi que no hace falta hacerlo siempre
            //Configurar centro al que se mira y distancia desde la que se mira
            Camara.setCamera(new Vector3(20, 20, 20), new Vector3(0, 0, 0));

            //Textura de la carperta Media
            var mediaFolder = MediaDir + Game.Default.TexturaCaja;

            //Cargamos una textura
            var texture = TgcTexture.createTexture(mediaFolder);

            //Creamos una caja 3D ubicada en (0, 0, 0), dimensiones (5, 10, 5) y la textura como color.
            var center = new Vector3(0, 0, 0);
            var size = new Vector3(5, 10, 5);
            Box = TgcBox.fromSize(center, size, texture);
        }

        public override void Update()
        {
            //TODO aca debe ir toda la logica de calculos de lo que fue pasando....
        }

        /// <summary>
        ///     Método que se llama cada vez que hay que refrescar la pantalla.
        ///     Escribir aquí todo el código referido al renderizado.
        ///     Borrar todo lo que no haga falta
        /// </summary>
        public override void Render()
        {
            //Inicio el render de la escena
            IniciarEscena();

			//Ejecuto el render de la super clase
			base.Render();

            //Device de DirectX para renderizar
            var d3dDevice = D3DDevice.Instance.Device;

            ///////////////INPUT//////////////////
            //conviene deshabilitar ambas camaras para que no haya interferencia

            //Capturar Input teclado
            if (TgcD3dInput.Instance.keyPressed(Key.F))
            {
                //Tecla F apretada
            }

            //Capturar Input Mouse
            if (TgcD3dInput.Instance.buttonPressed(TgcD3dInput.MouseButtons.BUTTON_LEFT))
            {
                //Boton izq apretado
            }

            //Render de la caja
            Box.render();            

            //Finaliza el render
            FinalizarEscena();
        }

        /// <summary>
        ///     Método que se llama cuando termina la ejecución del ejemplo.
        ///     Hacer dispose() de todos los objetos creados.
        /// </summary>
        public override void Close()
        {
            //Elimino los recursos de la super clase
            base.Close();

            //Dispose de la caja
            Box.dispose();
        }
    }
}