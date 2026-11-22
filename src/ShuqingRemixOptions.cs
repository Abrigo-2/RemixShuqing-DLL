using Menu.Remix.MixedUI;
using MoreSlugcats;
using UnityEngine;

public class ShuqingRemixOptions : OptionInterface
{
    public readonly Configurable<int> HorizontalBoostX;
    public readonly Configurable<int> HorizontalBoostY;

    public ShuqingRemixOptions()
    {
        HorizontalBoostX = config.Bind("HorizontalBoostX", 22, new ConfigurableInfo("Changes X (Run) ammount of the horizontal boost vector.", new ConfigAcceptableRange<int>(0, 100), "", "Horizontal Boost - Run"));
        HorizontalBoostY = config.Bind("HorizontalBoostX", 9, new ConfigurableInfo("Changes Y (Rise) ammount of the horizontal boost vector.", new ConfigAcceptableRange<int>(0, 100), "", "Horizontal Boost - Rise"));
    }

    public override void Initialize()
    {
        OpTab mopTab = new OpTab(this, OptionInterface.Translate("Main Setting"));
        Tabs = new OpTab[1] { mopTab };
        OpContainer opContainer = new OpContainer(new Vector2(0f, 0f));
        mopTab.AddItems(opContainer);
        int num = 100;
        UIelement[] array = new UIelement[3]
        {
            new OpLabel(new Vector2(0f, 450f), new Vector2(200f, 200f), OptionInterface.Translate("The Boost Stamina option is at the original Shuqing mod. Sorry!"), FLabelAlignment.Left, bigText: false),
            //new OpLabel(new Vector2(30f, 400f), new Vector2(num, num), OptionInterface.Translate("Infinite Stamina"), FLabelAlignment.Left),
            new OpUpdown(HorizontalBoostX, new Vector2(30f, 400f), 200f),
            new OpUpdown(HorizontalBoostY, new Vector2(30f, 350f), 200f),
        };
        mopTab.AddItems(array);
    }
}