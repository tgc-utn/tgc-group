using System.Drawing;
using TGC.Core.Camara;
using TGC.Core.Direct3D;
using TGC.Core.Input;
using TGC.Core.Text;
using Microsoft.DirectX.DirectInput;
using TGC.Core.SceneLoader;
using TGC.Core.Mathematica;
using System.Collections.Generic;
using TGC.Core.Terrain;
using System;
using TGC.Group.Model._2D;
using System.Linq;
using TGC.Core.Shaders;

namespace TGC.Group.Model
{
    class Boton
    {
        private HUDSprite menuItem;
        private HUDSprite menuItemSelec;
        public HUDTexto texto;

        public Boton(CustomBitmap sprite, CustomBitmap spriteSelec, string texto, int indice, Drawer2D drawer)
        {
            CustomSprite menuSprite = new CustomSprite();
            menuSprite.Bitmap = sprite;
            CustomSprite menuSpriteSelec = new CustomSprite();
            menuSpriteSelec.Bitmap = spriteSelec;

            menuItem = new HUDSprite(HUD.AnclajeHorizontal.LEFT, HUD.AnclajeVertical.TOP, new TGCVector2(0.05f, 0.5f + (float)indice / 17), new TGCVector2(1, 1), drawer, menuSprite);
            menuItemSelec = new HUDSprite(HUD.AnclajeHorizontal.LEFT, HUD.AnclajeVertical.TOP, new TGCVector2(0.05f, 0.5f + (float)indice / 17), new TGCVector2(1, 1), drawer, menuSpriteSelec);
            menuItem.Init();
            menuItemSelec.Init();

            TgcText2D texto2D = new TgcText2D();
            texto2D.Align = TgcText2D.TextAlign.CENTER;
            texto2D.Size = new Size((int)(menuItem.Sprite.Scaling.X * 350), 20);
            texto2D.changeFont(new Font("Calibri", D3DDevice.Instance.Width / 96f, FontStyle.Italic | FontStyle.Bold));
            texto2D.Text = texto;

            this.texto = new HUDTexto(HUD.AnclajeHorizontal.LEFT, HUD.AnclajeVertical.TOP, new TGCVector2(0.1f, 0.5175f + (float)indice / 17), drawer, texto2D);
            this.texto.Init();
        }

        public void Render(bool seleccionado)
        {
            if (seleccionado)
            {
                menuItemSelec.Render();
                texto.Texto2D.Color = Color.FromArgb(0, 101, 225);
            }
            else
            {
                menuItem.Render();
                texto.Texto2D.Color = Color.White;
            }
            texto.Render();
        }
        public void Dispose()
        {
            menuItem.Dispose();
            menuItemSelec.Dispose();
            texto.Dispose();
        }

        public bool checkCollision(TGCVector2 posicion)
        {
            return posicion.X > menuItem.Sprite.Position.X && posicion.X < menuItem.Sprite.Position.X + menuItem.Sprite.Scaling.X * 512
                && posicion.Y > menuItem.Sprite.Position.Y && posicion.Y < menuItem.Sprite.Position.Y + menuItem.Sprite.Scaling.Y * 50;
        }
    }
    class EscenaMenu : Escena
    {
        enum Items
        {
            INICIAR,
            CONTROLES,
            CAMBIARVEHICULO,
            SALIR
        }

        private Pasto pasto;
        private TgcMesh paredes;
        private TgcSkyBox skyBox;
        private List<Boton> botones = new List<Boton>();
        private int botonSeleccionado;

        public int BotonSeleccionado
        {
            get => botonSeleccionado;
            set => botonSeleccionado = Math.Max(0, Math.Min(botones.Count - 1, value));
        }

        private List<Jugador> jugadores = new List<Jugador>();
        private int jugadorActivo = 0;

        private int JugadorActivo
        {
            get => jugadorActivo;
            set
            {
                if (value >= jugadores.Count) jugadorActivo = 0;
                else if (value < 0) jugadorActivo = jugadores.Count - 1;
                else jugadorActivo = value;
            }
        }

        private Luz sol;

        public EscenaMenu(TgcCamera Camera, string MediaDir, string ShadersDir, TgcText2D DrawText, float TimeBetweenUpdates, TgcD3dInput Input) : base(Camera, MediaDir, ShadersDir, DrawText, TimeBetweenUpdates, Input)
        {
            TgcScene escena = new TgcSceneLoader().loadSceneFromFile(MediaDir + "Cancha-TgcScene.xml");

            pasto = new Pasto(escena.Meshes[0], TGCShaders.Instance.LoadEffect(ShadersDir + "CustomShaders.fx"), 32, .5f);
            paredes = escena.getMeshByName("Box_5");
            Camera.SetCamera(new TGCVector3(20, 10, -20), new TGCVector3(0, 5, -7));
            initJugadores(escena);

            sol = new Luz(Color.White, new TGCVector3(0, 30, -50));

            CustomBitmap menuItem = new CustomBitmap(MediaDir + "\\Textures\\HUD\\menuItem.png", D3DDevice.Instance.Device);
            CustomBitmap menuItemSelec = new CustomBitmap(MediaDir + "\\Textures\\HUD\\menuItemSelec.png", D3DDevice.Instance.Device);

            botones.Add(new Boton(menuItem, menuItemSelec, "Iniciar", 0, drawer2D));
            botones.Add(new Boton(menuItem, menuItemSelec, "Controles", 1, drawer2D));
            botones.Add(new Boton(menuItem, menuItemSelec, "< Cambiar vehículo >", 2, drawer2D));
            botones.Add(new Boton(menuItem, menuItemSelec, "Salir", 3, drawer2D));

            skyBox = new TgcSkyBox();
            skyBox.Center = new TGCVector3(0, 500, 0);
            skyBox.Size = new TGCVector3(10000, 10000, 10000);
            var texturesPath = MediaDir + "Textures\\SkyBox LostAtSeaDay\\";
            skyBox.setFaceTexture(TgcSkyBox.SkyFaces.Up, texturesPath + "lostatseaday_up.jpg");
            skyBox.setFaceTexture(TgcSkyBox.SkyFaces.Down, texturesPath + "lostatseaday_dn.jpg");
            skyBox.setFaceTexture(TgcSkyBox.SkyFaces.Left, texturesPath + "lostatseaday_lf.jpg");
            skyBox.setFaceTexture(TgcSkyBox.SkyFaces.Right, texturesPath + "lostatseaday_rt.jpg");
            skyBox.setFaceTexture(TgcSkyBox.SkyFaces.Front, texturesPath + "lostatseaday_bk.jpg");
            skyBox.setFaceTexture(TgcSkyBox.SkyFaces.Back, texturesPath + "lostatseaday_ft.jpg");
            skyBox.Init();
        }

        private void initJugadores(TgcScene escena)
        {
            Jugador auto = new Jugador("Auto", escena.Meshes[2], new TGCVector3(0, 5, 0), new TGCVector3(-.5f, 0, 0));
            Jugador tractor = new Jugador("Tractor", escena.Meshes[5], new TGCVector3(0, 5, 0), new TGCVector3(-.5f, 0, 0));
            Jugador patrullero = new Jugador("Patrullero", escena.Meshes[3], new TGCVector3(0, 5, 0), new TGCVector3(-.5f, 0, 0));
            Jugador tanque = new Jugador("Tanque", escena.Meshes[4], new TGCVector3(0, 5, 0), new TGCVector3(-.5f, 0, 0));

            jugadores.Add(auto);
            jugadores.Add(tractor);
            jugadores.Add(patrullero);
            jugadores.Add(tanque);

            foreach(var jugador in jugadores)
            {
                jugador.Mesh.Effect = TGCShaders.Instance.LoadEffect(ShadersDir + "CustomShaders.fx");
                jugador.Mesh.Technique = "BlinnPhong";
            }
        }
        public override void Dispose()
        {
            botones.ForEach(boton => boton.Dispose());
        }

        public override void Render()
        {
            D3DDevice.Instance.Device.Clear(Microsoft.DirectX.Direct3D.ClearFlags.Target | Microsoft.DirectX.Direct3D.ClearFlags.ZBuffer, Color.White, 1.0f, 0);
            D3DDevice.Instance.Device.BeginScene();
            //TexturesManager.Instance.clearAll();

            skyBox.Render();
            pasto.Render();
            paredes.Render();

            for (int i = 0; i < botones.Count; i++)
                botones[i].Render(i == botonSeleccionado);


            jugadores[jugadorActivo].Mesh.Effect.SetValue("eyePosition", TGCVector3.TGCVector3ToFloat3Array(Camera.Position));
            jugadores[jugadorActivo].Render(sol);
        }

        private float tiempoMovido = 0; // Workaround por el evento de las teclas
        public override Escena Update(float ElapsedTime)
        {
            Boton mouse = botones.FirstOrDefault(boton => boton.checkCollision(new TGCVector2(Input.Xpos, Input.Ypos)));
            if (Input.XposRelative != 0 && Input.YposRelative != 0)
                botonSeleccionado = botones.IndexOf(mouse);

            if (Input.keyDown(Key.Return) || (mouse != null && Input.buttonDown(TgcD3dInput.MouseButtons.BUTTON_LEFT)))
                switch ((Items)botonSeleccionado)
                {
                    case Items.INICIAR:
                        return CambiarEscena(new EscenaJuego(Camera, MediaDir, ShadersDir, DrawText, TimeBetweenUpdates, Input, jugadores, jugadores[jugadorActivo]));
                    case Items.CONTROLES:
                        return CambiarEscena(new EscenaControles(Camera, MediaDir, ShadersDir, DrawText, TimeBetweenUpdates, Input));
                    case Items.SALIR:
                        Form.GameForm.ActiveForm.Close();
                        break;
                }

            if(tiempoMovido <= 0)
            {
                if ((Items)botonSeleccionado == Items.CAMBIARVEHICULO)
                {
                    if (Input.keyDown(Key.RightArrow) || Input.buttonDown(TgcD3dInput.MouseButtons.BUTTON_LEFT))
                    {
                        JugadorActivo++;
                        tiempoMovido = 0.2f;
                    }
                    if (Input.keyDown(Key.LeftArrow))
                    {
                        JugadorActivo--;
                        tiempoMovido = 0.2f;
                    }
                }
                if (Input.keyDown(Key.UpArrow))
                {
                    BotonSeleccionado--;
                    tiempoMovido = 0.2f;
                }
                if (Input.keyDown(Key.DownArrow))
                {
                    BotonSeleccionado++;
                    tiempoMovido = 0.2f;
                }
            }
            else
                tiempoMovido -= ElapsedTime;

            pasto.Update(ElapsedTime);

            return this;
        }
    }
}
