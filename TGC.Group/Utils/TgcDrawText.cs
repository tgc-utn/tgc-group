using Microsoft.DirectX.Direct3D;
using System.Drawing;
using System.Drawing.Text;
using Font = System.Drawing.Font;

namespace TGC.UtilsGroup
{
    /// <summary>
    ///     Herramienta para dibujar texto genérico del Framework
    /// </summary>
    public class TgcDrawText
    {
        private readonly Microsoft.DirectX.Direct3D.Font dxFont;

        public TgcDrawText(Device d3dDevice, string fuente, int size, string MediaDir)
        {
            System.Drawing.Text.PrivateFontCollection privateFonts = new PrivateFontCollection();
            Font FUENTE_SIZE;

            if (fuente == "Rock it")
            {
                privateFonts.AddFontFile(MediaDir + "Font\\Rockit.ttf");
                FUENTE_SIZE = new Font(privateFonts.Families[0], size);
            }
            else
                FUENTE_SIZE = new Font (fuente, size, FontStyle.Regular, GraphicsUnit.Pixel);

            //Creo el Sprite
            TextSprite = new Sprite(d3dDevice);

            //Fuente
            dxFont = new Microsoft.DirectX.Direct3D.Font(d3dDevice, FUENTE_SIZE);
        }

        /// <summary>
        ///     Sprite para renderizar texto
        /// </summary>
        public Sprite TextSprite { get; }

        /// <summary>
        ///     Dibujar un texto en la posición indicada, con el color indicado.
        ///     Utilizar la fuente default del Framework.
        /// </summary>
        /// <param name="text">Texto a dibujar</param>
        /// <param name="x">Posición X de la pantalla</param>
        /// <param name="y">Posición Y de la pantalla</param>
        /// <param name="color">Color del texto</param>
        public void drawText(string text, int x, int y, Color color)
        {
            TextSprite.Begin(SpriteFlags.AlphaBlend);
            dxFont.DrawText(TextSprite, text, x, y, color);
            TextSprite.End();
        }
    }
}