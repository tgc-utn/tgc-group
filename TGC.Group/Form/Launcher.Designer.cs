namespace TGC.Group.Form
{
    partial class Launcher
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Launcher));
            this.btnStart = new System.Windows.Forms.Button();
            this.labelNombre = new System.Windows.Forms.Label();
            this.txtName = new System.Windows.Forms.TextBox();
            this.labelNroOponentes = new System.Windows.Forms.Label();
            this.cmbAutos = new System.Windows.Forms.ComboBox();
            this.cmbTiempo = new System.Windows.Forms.ComboBox();
            this.labelTiempo = new System.Windows.Forms.Label();
            this.btnSalir = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // btnStart
            // 
            this.btnStart.Location = new System.Drawing.Point(563, 401);
            this.btnStart.Name = "btnStart";
            this.btnStart.Size = new System.Drawing.Size(93, 34);
            this.btnStart.TabIndex = 3;
            this.btnStart.Text = "Jugar";
            this.btnStart.UseVisualStyleBackColor = true;
            this.btnStart.Click += new System.EventHandler(this.btnStart_Click);
            // 
            // labelNombre
            // 
            this.labelNombre.AutoSize = true;
            this.labelNombre.BackColor = System.Drawing.Color.Transparent;
            this.labelNombre.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.749999F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelNombre.ForeColor = System.Drawing.Color.DeepSkyBlue;
            this.labelNombre.Location = new System.Drawing.Point(12, 260);
            this.labelNombre.Name = "labelNombre";
            this.labelNombre.Size = new System.Drawing.Size(60, 16);
            this.labelNombre.TabIndex = 1;
            this.labelNombre.Text = "Nombre:";
            // 
            // txtName
            // 
            this.txtName.CharacterCasing = System.Windows.Forms.CharacterCasing.Lower;
            this.txtName.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtName.Location = new System.Drawing.Point(177, 255);
            this.txtName.MaxLength = 10;
            this.txtName.Name = "txtName";
            this.txtName.Size = new System.Drawing.Size(173, 20);
            this.txtName.TabIndex = 1;
            // 
            // labelNroOponentes
            // 
            this.labelNroOponentes.AutoSize = true;
            this.labelNroOponentes.BackColor = System.Drawing.Color.Transparent;
            this.labelNroOponentes.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.749999F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelNroOponentes.ForeColor = System.Drawing.Color.DeepSkyBlue;
            this.labelNroOponentes.Location = new System.Drawing.Point(12, 303);
            this.labelNroOponentes.Name = "labelNroOponentes";
            this.labelNroOponentes.Size = new System.Drawing.Size(105, 16);
            this.labelNroOponentes.TabIndex = 3;
            this.labelNroOponentes.Text = "Nro. Oponentes:";
            // 
            // cmbAutos
            // 
            this.cmbAutos.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbAutos.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cmbAutos.FormattingEnabled = true;
            this.cmbAutos.Location = new System.Drawing.Point(177, 300);
            this.cmbAutos.Name = "cmbAutos";
            this.cmbAutos.Size = new System.Drawing.Size(63, 21);
            this.cmbAutos.TabIndex = 3;
            // 
            // cmbTiempo
            // 
            this.cmbTiempo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbTiempo.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cmbTiempo.FormattingEnabled = true;
            this.cmbTiempo.Location = new System.Drawing.Point(177, 346);
            this.cmbTiempo.Name = "cmbTiempo";
            this.cmbTiempo.Size = new System.Drawing.Size(63, 21);
            this.cmbTiempo.Sorted = true;
            this.cmbTiempo.TabIndex = 2;
            // 
            // labelTiempo
            // 
            this.labelTiempo.AutoSize = true;
            this.labelTiempo.BackColor = System.Drawing.Color.Transparent;
            this.labelTiempo.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.749999F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelTiempo.ForeColor = System.Drawing.Color.DeepSkyBlue;
            this.labelTiempo.Location = new System.Drawing.Point(12, 349);
            this.labelTiempo.Name = "labelTiempo";
            this.labelTiempo.Size = new System.Drawing.Size(58, 16);
            this.labelTiempo.TabIndex = 5;
            this.labelTiempo.Text = "Tiempo:";
            // 
            // btnSalir
            // 
            this.btnSalir.Location = new System.Drawing.Point(662, 401);
            this.btnSalir.Name = "btnSalir";
            this.btnSalir.Size = new System.Drawing.Size(93, 34);
            this.btnSalir.TabIndex = 4;
            this.btnSalir.Text = "Salir";
            this.btnSalir.UseVisualStyleBackColor = true;
            this.btnSalir.Click += new System.EventHandler(this.btnSalir_Click);
            // 
            // Launcher
            // 
            this.AcceptButton = this.btnStart;
            this.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("$this.BackgroundImage")));
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.ClientSize = new System.Drawing.Size(767, 447);
            this.Controls.Add(this.btnSalir);
            this.Controls.Add(this.cmbTiempo);
            this.Controls.Add(this.labelTiempo);
            this.Controls.Add(this.cmbAutos);
            this.Controls.Add(this.labelNroOponentes);
            this.Controls.Add(this.txtName);
            this.Controls.Add(this.labelNombre);
            this.Controls.Add(this.btnStart);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "Launcher";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Load += new System.EventHandler(this.Launchercs_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnStart;
        private System.Windows.Forms.Label labelNombre;
        private System.Windows.Forms.TextBox txtName;
        private System.Windows.Forms.Label labelNroOponentes;
        private System.Windows.Forms.ComboBox cmbAutos;
        private System.Windows.Forms.ComboBox cmbTiempo;
        private System.Windows.Forms.Label labelTiempo;
        private System.Windows.Forms.Button btnSalir;
    }
}
