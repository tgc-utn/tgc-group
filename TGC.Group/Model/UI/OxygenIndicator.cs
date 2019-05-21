using System;
using System.Drawing;
using TGC.Group.Model.Player;

namespace TGC.Group.Model.UI
{
    public class OxygenIndicator : CircularIndicator
    {
        public OxygenIndicator(int meterSize, int meterX0, int meterY0) : base(meterSize, meterX0, meterY0)
        {
        }

        protected override void renderEffect(Character character)
        {
            renderEffect(character.ActualStats.Oxygen, character.MaxStats.Oxygen);
        }

        protected override void renderText(Character character)
        {
            double o2Level = Math.Floor(character.ActualStats.Oxygen);

            var oXPosition = this.meterX0 + toInt(Scale(this.meterSize, 54));
            var oYPosition = this.meterY0 + toInt(Scale(this.meterSize, 32));

            var twoXPosition = this.meterX0 + toInt(Scale(this.meterSize, 79));
            var twoYPosition = this.meterY0 + toInt(Scale(this.meterSize, 45));

            var o2LevelXPosition = o2Level >= 10
                ? this.meterX0 + toInt(Scale(this.meterSize, 55))
                : this.meterX0 + toInt(Scale(this.meterSize, 65));
            var o2LevelYPosition = this.meterY0 + toInt(Scale(this.meterSize, 74));

            this.textBig.drawText("O", oXPosition, oYPosition, Color.Bisque);
            this.textSmall.drawText("2", twoXPosition, twoYPosition, Color.Bisque);
            this.textBig.drawText("" + o2Level, o2LevelXPosition, o2LevelYPosition, Color.Bisque);
        }
    }
}