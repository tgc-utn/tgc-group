using TGC.Core.Text;
using TGC.Core.Direct3D;
using TGC.Core.Mathematica;
using System.Drawing;

namespace TGC.Group.Model._2D
{
    class UIEscenaJuego
    {
        public string TextoTurbo { set { textoTurboHud.Texto = value; } }
        public Color ColorTextoTurbo { set { textoTurboHud.Color = value; } }
        public string TextoGolAzul { set { textoGolAzulHud.Texto = value; } }
        public string TextoGolRojo { set { textoGolRojoHud.Texto = value; } }
        public string TextoReloj { set { textoRelojHud.Texto = value; } }

        private Drawer2D drawer2D;

        //Sprites
        private HUDSprite relojBG;
        private HUDSprite equipoRojoHUD;
        private HUDSprite equipoAzulHUD;
        private HUDSprite turboHUD;
        //Texto
        private HUDTexto textoGolAzulHud;
        private HUDTexto textoGolRojoHud;
        private HUDTexto textoRelojHud;
        private HUDTexto textoTurboHud;

        public UIEscenaJuego() { }

        public void Init(string MediaDir, Drawer2D drawer2D)
        {
            this.drawer2D = drawer2D;

            CustomSprite contadorTiempo = new CustomSprite();
            contadorTiempo.Bitmap = new CustomBitmap(MediaDir + "\\Textures\\ContadorTiempo.png", D3DDevice.Instance.Device);
            relojBG = new HUDSprite(HUD.AnclajeHorizontal.CENTER, HUD.AnclajeVertical.TOP, new TGCVector2(0.0f,0.0f), new TGCVector2(1.0f,1.0f), drawer2D,contadorTiempo);
            relojBG.Init();

            CustomSprite medidorTurbo = new CustomSprite();
            medidorTurbo.Bitmap = new CustomBitmap(MediaDir + "\\Textures\\MedidorTurbo.png", D3DDevice.Instance.Device);
            turboHUD = new HUDSprite(HUD.AnclajeHorizontal.RIGHT, HUD.AnclajeVertical.BOTTOM, new TGCVector2(.05f, .05f), new TGCVector2(1, 1), drawer2D, medidorTurbo);
            turboHUD.Init();
            
            CustomSprite equipoAzulSprite = new CustomSprite();
            equipoAzulSprite.Bitmap = new CustomBitmap(MediaDir + "\\Textures\\EquipoAzul.png", D3DDevice.Instance.Device);
            equipoAzulHUD = new HUDSprite(HUD.AnclajeHorizontal.LEFT, HUD.AnclajeVertical.TOP, new TGCVector2(0.40f, 0.015f),new TGCVector2(1.0f, 1.5f),drawer2D,equipoAzulSprite);
            equipoAzulHUD.Init();

            CustomSprite equipoRojoSprite = new CustomSprite();
            equipoRojoSprite.Bitmap = new CustomBitmap(MediaDir + "\\Textures\\EquipoRojo.png", D3DDevice.Instance.Device);
            equipoRojoHUD = new HUDSprite(HUD.AnclajeHorizontal.RIGHT, HUD.AnclajeVertical.TOP, new TGCVector2(0.40f,0.015f),new TGCVector2(1.0f,1.5f),drawer2D,equipoRojoSprite);
            equipoRojoHUD.Init();

            TgcText2D textoTurbo = new TgcText2D();
            textoTurbo.Align = TgcText2D.TextAlign.CENTER;
            textoTurbo.Size = new Size((int)(250 * medidorTurbo.Scaling.X), 50);
            textoTurbo.Color = Color.White;
            textoTurbo.changeFont(new Font("TimesNewRoman", 50, FontStyle.Bold));
            textoTurboHud = new HUDTexto(HUD.AnclajeHorizontal.RIGHT, HUD.AnclajeVertical.BOTTOM, new TGCVector2(.05f, 170f / 1080), drawer2D, textoTurbo);
            textoTurboHud.Init();

            TgcText2D textoGolAzul = new TgcText2D();
            textoGolAzul.Align = TgcText2D.TextAlign.CENTER;
            TGCVector2 equipoAzulSize = equipoAzulHUD.Size;
            textoGolAzul.Size = new Size((int)equipoAzulSize.X, (int)equipoAzulSize.Y);
            textoGolAzul.Color = Color.Black;
            textoGolAzul.changeFont(new Font("TimesNewRoman", 30, FontStyle.Bold));
            textoGolAzulHud = new HUDTexto(HUD.AnclajeHorizontal.LEFT,HUD.AnclajeVertical.TOP, new TGCVector2(0.40f, 0.025f), drawer2D,textoGolAzul);
            textoGolAzulHud.Init();

            TgcText2D textoGolRojo = new TgcText2D();
            textoGolRojo.Align = TgcText2D.TextAlign.CENTER;
            TGCVector2 equipoRojoSize = equipoAzulHUD.Size;
            textoGolRojo.Size = new Size((int)equipoRojoSize.X, (int)equipoRojoSize.Y);
            textoGolRojo.Color = Color.Black;
            textoGolRojo.changeFont(new Font("TimesNewRoman", 30, FontStyle.Bold));
            textoGolRojoHud = new HUDTexto(HUD.AnclajeHorizontal.RIGHT, HUD.AnclajeVertical.TOP, new TGCVector2(0.40f, 0.025f), drawer2D, textoGolRojo);
            textoGolRojoHud.Init();

            TgcText2D textoConTiempo = new TgcText2D();
            textoConTiempo.Align = TgcText2D.TextAlign.CENTER;
            TGCVector2 relojBGSize = relojBG.Size;
            textoConTiempo.Size = new Size((int)relojBGSize.X, (int)relojBGSize.Y);
            textoConTiempo.Color = Color.Black;
            textoConTiempo.changeFont(new Font("TimesNewRoman", 20, FontStyle.Bold));
            textoRelojHud = new HUDTexto(HUD.AnclajeHorizontal.CENTER, HUD.AnclajeVertical.TOP, new TGCVector2(0, 0.045f), drawer2D, textoConTiempo);
            textoRelojHud.Init();
        }

        public void Render()
        {
            equipoAzulHUD.Render();
            equipoRojoHUD.Render();
            turboHUD.Render();
            textoTurboHud.Render();
            relojBG.Render();

            textoGolAzulHud.Render();
            textoGolRojoHud.Render();
            textoRelojHud.Render();
        }

        public void Dispose()
        {
            relojBG.Dispose();
            equipoAzulHUD.Dispose();
            equipoRojoHUD.Dispose();
            turboHUD.Dispose();
            textoTurboHud.Dispose();
            textoGolAzulHud.Dispose();
            textoGolRojoHud.Dispose();
            textoRelojHud.Dispose();
        }

    }
}
