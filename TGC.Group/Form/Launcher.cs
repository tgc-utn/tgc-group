using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
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
            cmbAutos.Items.AddRange(new object[] { 1, 2, 3, 4});
            cmbTiempo.Items.AddRange(new object[] { 5, 10, 15 });
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            GameForm gameForm = new GameForm();

            gameForm.NombreJugador1 = txtName.Text;
            gameForm.CantidadAutos = Convert.ToInt32(cmbAutos.Text);
            gameForm.TiempoDeJuego = Convert.ToInt32(cmbTiempo.Text);
            this.Hide();
            gameForm.Show();
            this.Close();
        }


    }
}
