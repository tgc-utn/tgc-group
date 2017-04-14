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

        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            GameForm gameForm = new GameForm();

            gameForm.NombreJugador1 = txtName.Text;
            this.Hide();
            gameForm.Show();
            this.Close();
        }
    }
}
