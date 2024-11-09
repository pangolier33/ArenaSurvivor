using TMPro;

namespace Plugins.Animated.Pulsating
{
    public sealed class PulsatingTextMP : PulsatingUI<TextMeshProUGUI> 
    {
        public override void ValueApply()
        {
            target.alpha = value;
        }
    }
}
