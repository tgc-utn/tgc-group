using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Text;
using System.Text;
using System.Windows.Forms;

namespace TGC.Group.Form
{
    public partial class Launcher : System.Windows.Forms.Form
    {
        public Launcher()
        {
            InitializeComponent();
        }

        private void Launchercs_Load(object sender, EventArgs e)
        {
            System.Drawing.Text.PrivateFontCollection privateFonts = new PrivateFontCollection();
            string fontDir = "";

            try
            {
                fontDir = Environment.CurrentDirectory + @"\..\..\Media\Twisted_Chano\Font\";
                privateFonts.AddFontFile(fontDir + "Rockit.ttf");

                Font FUENTE_SIZE_10 = new Font(privateFonts.Families[0], 10);
                Font FUENTE_SIZE_6 = new Font(privateFonts.Families[0], 6);

                this.labelNombre.Font = FUENTE_SIZE_10;
                this.labelNroOponentes.Font = FUENTE_SIZE_10;
                this.labelTiempo.Font = FUENTE_SIZE_10;
                this.btnStart.Font = FUENTE_SIZE_6;
                this.btnSalir.Font = FUENTE_SIZE_6;
            }
            catch (Exception)
            { }

            cmbAutos.Items.AddRange(new object[] { 1, 2, 3, 4 });
            cmbTiempo.Items.AddRange(new object[] {5, 10, 15 });

            cmbAutos.Text = "4";
            cmbTiempo.Text = "5";
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            GameForm gameForm = new GameForm();

            if (txtName.Text == "")
            {
                gameForm.NombreJugador1 = "jugador 1";
            }
            else
            {
                gameForm.NombreJugador1 = txtName.Text;
            }

            gameForm.CantidadOponentes = Convert.ToInt32(cmbAutos.Text);
            gameForm.TiempoDeJuego = Convert.ToInt32(cmbTiempo.Text);
            this.Hide();
            gameForm.Show();
            this.Show();
        }

        private void btnSalir_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
