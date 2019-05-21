using System;
using System.Drawing;
using System.Windows.Forms;
using Microsoft.DirectX.Direct3D;
using TGC.Core.Direct3D;
using TGC.Core.Mathematica;
using TGC.Core.Text;
using TGC.Group.Model.Player;
using TGC.Group.Model.Resources.Sprites;
using TGC.Group.TGCUtils;
using Screen = TGC.Group.Model.Utils.Screen;

namespace TGC.Group.Model.UI
{
    public abstract class CircularIndicator
    {
        private Effect effect;

        private readonly Drawer2D drawer = new Drawer2D();

        private const int DefaultMeterSize = 145;

        protected readonly int meterSize;
        protected readonly int meterX0;
        protected readonly int meterY0;

        private readonly CustomSprite blackCircle;

        private CustomVertex.TransformedColored[] vertices;

        protected readonly TgcText2D textBig = new TgcText2D();
        protected readonly TgcText2D textSmall = new TgcText2D();

        private static float ScalingFactor(float defaultSize)
        {
            return defaultSize / DefaultMeterSize;
        }

        protected static float Scale(float toScale, float defaultSize)
        {
            return toScale*ScalingFactor(defaultSize);
        }

        protected static int toInt(float number)
        {
            return (int)Math.Floor(number);
        }

        public CircularIndicator(int meterSize, int meterX0, int meterY0)
        {
            this.meterSize = meterSize;
            this.meterX0 = meterX0;
            this.meterY0 = meterY0;
            
            blackCircle = BitmapRepository.CreateSpriteFromBitmap(BitmapRepository.BlackCircle);
            blackCircle.Scaling = new TGCVector2(Scale(this.meterSize, .295f), Scale(this.meterSize, .295f));
            blackCircle.Position = new TGCVector2(meterX0 - Scale(this.meterSize, 3), meterY0 - Scale(this.meterSize, 3));
            blackCircle.Color = Color.FromArgb(120, 0, 0, 0);
            
            this.textBig.changeFont(new System.Drawing.Font("Arial Narrow Bold", Scale(this.meterSize, 25)));
            this.textSmall.changeFont(new System.Drawing.Font("Arial Narrow Bold", Scale(this.meterSize, 15)));
        }

        public void init()
        {
            string compilationErrors;
            
            try
            {
                this.effect = Effect.FromFile(D3DDevice.Instance.Device, "../../../Shaders/Oxygen.fx", null, null, ShaderFlags.None, null, out compilationErrors);
            }
            catch(Exception e)
            {
                throw new Exception("No pudo cargar el archivo csm");
            }
            if(this.effect == null)
            {
                throw new Exception("Errores de compilación oxigen.fx: " + compilationErrors);
            }

            this.effect.Technique = "OxygenTechnique";

            var black = 0x000000;
            var green = 0x00FF00;
            var red = 0xFF0000;
            var yellow = 0xFFFF00;

            vertices = new CustomVertex.TransformedColored[6];
            vertices[0] = new CustomVertex.TransformedColored(this.meterX0, this.meterY0, 0, 1, black);
            vertices[1] = new CustomVertex.TransformedColored(this.meterX0 + this.meterSize, this.meterY0, 0, 1, red);
            vertices[2] = new CustomVertex.TransformedColored(this.meterX0, this.meterY0 + this.meterSize, 0, 1, green);
            vertices[3] = new CustomVertex.TransformedColored(this.meterX0, this.meterY0 + this.meterSize, 0, 1, green);
            vertices[4] = new CustomVertex.TransformedColored(this.meterX0 + this.meterSize, this.meterY0  , 0, 1, red);
            vertices[5] = new CustomVertex.TransformedColored(this.meterX0 + this.meterSize, this.meterY0 + this.meterSize, 0, 1, yellow);
        }

        public void render(Character character)
        {
            renderBlackCircle();
            renderEffect(character);
            renderText(character);
        }

        private void renderBlackCircle()
        {
            this.drawer.BeginDrawSprite();
            this.drawer.DrawSprite(this.blackCircle);
            this.drawer.EndDrawSprite();
        }

        protected void renderEffect(float actualStat, float maxStat)
        {
            /**********OXYGEN METER SHADER***********/
            this.effect.Begin(FX.None);
            this.effect.BeginPass(0);
            this.effect.SetValue("oxygen", actualStat / maxStat);
            D3DDevice.Instance.Device.RenderState.AlphaBlendEnable = true;
            D3DDevice.Instance.Device.VertexFormat = CustomVertex.TransformedColored.Format;
            D3DDevice.Instance.Device.DrawUserPrimitives(PrimitiveType.TriangleList, this.vertices.Length / 3, this.vertices);
            this.effect.EndPass();
            this.effect.BeginPass(1);
            D3DDevice.Instance.Device.RenderState.AlphaBlendEnable = true;
            D3DDevice.Instance.Device.VertexFormat = CustomVertex.TransformedColored.Format;
            D3DDevice.Instance.Device.DrawUserPrimitives(PrimitiveType.TriangleList, this.vertices.Length / 3, this.vertices);
            this.effect.EndPass();
            this.effect.End();
            /****************************************/
        }

        protected abstract void renderEffect(Character character);
        protected abstract void renderText(Character character);
    }
}