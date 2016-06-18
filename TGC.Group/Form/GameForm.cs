using System;
using System.Threading;
using System.Windows.Forms;
using TGC.Core;
using TGC.Core.Camara;
using TGC.Core.Direct3D;
using TGC.Core.Input;
using TGC.Core.Shaders;
using TGC.Core.Sound;
using TGC.Core.Textures;
using TGC.Core.UserControls;
using TGC.Core.UserControls.Modifier;
using TGC.Group.Model;

namespace TGC.Group
{
    public partial class GameForm : Form
    {
        /// <summary>
        /// Ejemplo del juego a correr
        /// </summary>
        private GameModel Modelo { get; set; }

        /// <summary>
        /// Obtener o parar el estado del RenderLoop.
        /// </summary>
        private bool ApplicationRunning { get; set; }

        /// <summary>
        /// Constructor de la ventana
        /// </summary>
        public GameForm()
        {
            InitializeComponent();
        }

        private void GameForm_Load(object sender, EventArgs e)
        {
            //Iniciar graficos
            InitGraphics();

            //Titulo de la ventana principal
            Text = Modelo.Name + " - " + Modelo.Description;

            //Focus panel3D
            panel3D.Focus();

            //Inicio el ciclo de Render
            InitRenderLoop();
        }

        private void GameForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (ApplicationRunning)
            {
                ShutDown();
            }
        }

        public void InitGraphics()
        {
            ApplicationRunning = true;

            //Configuracion
            var settings = Game.Default;

            //Directorio actual de ejecucion
            var currentDirectory = Environment.CurrentDirectory + "\\";

            var mediaDirectory = currentDirectory + settings.MediaDirectory;
            var shadersDirectory = currentDirectory + settings.ShadersDirectory;

            //Inicio Device
            D3DDevice.Instance.InitializeD3DDevice(panel3D);

            //Inicio Input
            TgcD3dInput.Instance.Initialize(this, panel3D);

            //Inicio Sound
            TgcDirectSound.Instance.InitializeD3DDevice(this);

            //Cargar shaders del framework
            TgcShaders.Instance.loadCommonShaders(currentDirectory + settings.ShadersDirectory);

			//TODO hay que sacar esto del ejemplo... ya que aca no se tiene nada, se los paso para que no explote.
			var userVars = new TgcUserVars(new DataGridView());
			var modifiers = new TgcModifiers(new Panel());

			Modelo = new GameModel(mediaDirectory, shadersDirectory, userVars, modifiers, new TgcAxisLines(), new TgcRotationalCamera());

            //Cargar juego
            ExecuteModel();
        }

        public void InitRenderLoop()
        {
            while (ApplicationRunning)
            {
                //Renderizo si es que hay un ejemplo activo
                if (Modelo != null)
                {
                    //Solo renderizamos si la aplicacion tiene foco, para no consumir recursos innecesarios
                    if (ApplicationActive())
                    {
                        Modelo.Update();
                        Modelo.Render();
                    }
                    else
                    {
                        //Si no tenemos el foco, dormir cada tanto para no consumir gran cantidad de CPU
                        Thread.Sleep(100);
                    }
                }
                // Process application messages
                Application.DoEvents();
            }
        }

        /// <summary>
        ///     Indica si la aplicacion esta activa.
        ///     Busca si la ventana principal tiene foco o si alguna de sus hijas tiene.
        /// </summary>
        public bool ApplicationActive()
        {
            if (ContainsFocus)
            {
                return true;
            }

            foreach (var form in OwnedForms)
            {
                if (form.ContainsFocus)
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        ///     Arranca a ejecutar un ejemplo.
        ///     Para el ejemplo anterior, si hay alguno.
        /// </summary>
        /// <param name="example"></param>
        public void ExecuteModel()
        {
            //Ejecutar Init
            try
            {
                Modelo.ResetDefaultConfig();
                Modelo.Init();
                panel3D.Focus();
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, "Error en Init() del juego", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        ///     Cuando el Direct3D Device se resetea.
        ///     Se reinica el ejemplo actual, si hay alguno.
        /// </summary>
        public void OnResetDevice()
        {
            var exampleBackup = Modelo;

            if (exampleBackup != null)
            {
                StopCurrentExample();
            }

            D3DDevice.Instance.DoResetDevice();

            if (exampleBackup != null)
            {
                ExecuteModel();
            }
        }

        /// <summary>
        ///     Deja de ejecutar el ejemplo actual
        /// </summary>
        public void StopCurrentExample()
        {
            if (Modelo != null)
            {
				Modelo.Dispose();
                Modelo = null;
            }
        }

        /// <summary>
        ///     Finalizar aplicacion
        /// </summary>
        public void ShutDown()
        {
            ApplicationRunning = false;

            if (Modelo != null)
            {
                StopCurrentExample();
            }

            //Liberar Device al finalizar la aplicacion
            D3DDevice.Instance.Dispose();
            TexturesPool.Instance.clearAll();
        }
    }
}