using System;
using TGC.Group.Model.Player;
using TGC.Group.Model.Utils;

namespace TGC.Group.Model.UI
{
    public class StatsIndicators
    {
        private OxygenIndicator oxygenIndicator;
        private LifeIndicator lifeIndicator;

        public StatsIndicators(int baseX0, int baseY0)
        {
            var oxygenMeterSize = (int) Math.Floor(Screen.Height * 0.25f);
            var lifeMeterSize = (int) Math.Floor(Screen.Height * 0.1f);

            this.oxygenIndicator = new OxygenIndicator(oxygenMeterSize, baseX0, baseY0);
            this.lifeIndicator = new LifeIndicator(lifeMeterSize, baseX0 - lifeMeterSize, baseY0 - lifeMeterSize);
        }

        public void init()
        {
            this.oxygenIndicator.init();
            this.lifeIndicator.init();
        }

        public void render(Character character)
        {
            this.oxygenIndicator.render(character);
            this.lifeIndicator.render(character);
        }
    }
}