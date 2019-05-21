using System;
using System.Drawing;
using TGC.Group.Model.Player;

namespace TGC.Group.Model.UI
{
    public class LifeIndicator : CircularIndicator
    {
        public LifeIndicator(int meterSize, int meterX0, int meterY0) : base(meterSize, meterX0, meterY0)
        {
        }

        protected override void renderEffect(Character character)
        {
            renderEffect(character.ActualStats.Life, character.MaxStats.Life);
        }

        protected override void renderText(Character character)
        {
            double lifeLevel = character.ActualStats.Life;

            var oXPosition = this.meterX0 + toInt(Scale(this.meterSize, 60));
            var oYPosition = this.meterY0 + toInt(Scale(this.meterSize, 32));

            var o2LevelXPosition = lifeLevel >= 10
                ? this.meterX0 + toInt(Scale(this.meterSize, 48))
                : this.meterX0 + toInt(Scale(this.meterSize, 65));
            var o2LevelYPosition = this.meterY0 + toInt(Scale(this.meterSize, 74));

            this.textBig.drawText("❤", oXPosition, oYPosition, Color.Bisque);
            this.textBig.drawText("" + lifeLevel, o2LevelXPosition, o2LevelYPosition, Color.Bisque);
        }
    }
}