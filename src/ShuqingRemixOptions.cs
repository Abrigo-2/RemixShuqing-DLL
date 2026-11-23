using Menu.Remix.MixedUI;
using MoreSlugcats;
using UnityEngine;

public class ShuqingRemixOptions : OptionInterface
{
    public readonly Configurable<int> HorizontalBoostX;
    public readonly Configurable<int> HorizontalBoostY;

    public readonly Configurable<int> VerticalBoostX;
    public readonly Configurable<int> VerticalBoostY;

    public ShuqingRemixOptions()
    {
        HorizontalBoostX = config.Bind("Suqing_H_Boost_Run", 22, new ConfigurableInfo("Changes X (Run) ammount of the boost vector for plain Horizontal boosts.", new ConfigAcceptableRange<int>(0, 100), "Settings", "Horizontal Boost - Run"));
        HorizontalBoostY = config.Bind("Suqing_H_Boost_Rise", 9, new ConfigurableInfo("Changes Y (Rise) ammount of the boost vector for plain Horizontal boosts.", new ConfigAcceptableRange<int>(0, 100), "Settings", "Horizontal Boost - Rise"));

        VerticalBoostX = config.Bind("Suqing_V_Boost_Run", 2, new ConfigurableInfo("Changes X (Run) ammount of the boost vector for plain Vertical boosts.", new ConfigAcceptableRange<int>(0, 100), "Settings", "Vertical Boost - Run"));
        VerticalBoostY = config.Bind("Suqing_V_Boost_Rise", 16, new ConfigurableInfo("Changes Y (Rise) ammount of the boost vector for plain Vertical boosts.", new ConfigAcceptableRange<int>(0, 100), "Settings", "Vertical Boost - Rise"));
    }

    public override void Initialize()
    {
        int num = 100;
        bool is_Big_Text = false;

        OpTab mopTab = new OpTab(this, OptionInterface.Translate("Settings"));
        OpContainer opContainer = new OpContainer(new Vector2(0f, 0f));
        mopTab.AddItems(opContainer);

        UIelement[] array = new UIelement[11]
        {
            new OpLabel(new Vector2(200f, 450f), new Vector2(200f, 200f), OptionInterface.Translate("The Boost Stamina option is at the original Shuqing. Sorry!"), FLabelAlignment.Center, bigText: false),

            new OpUpdown(HorizontalBoostX, new Vector2(20f, 440f), 60f)
            {
                description = "Changes X (Run) ammount of the boost vector for plain Horizontal boosts. (Default: 22)"
            },
            new OpLabel(new Vector2(90f, 405f), new Vector2(num, num), OptionInterface.Translate("Horizontal Boost Run"), FLabelAlignment.Left),
            new OpUpdown(HorizontalBoostY, new Vector2(20f, 400f), 60f)
            {
                description = "Changes Y (Rise) ammount of the boost vector for plain Horizontal boosts. (Default: 9)"
            },
            new OpLabel(new Vector2(90f, 365f), new Vector2(num, num), OptionInterface.Translate("Horizontal Boost Rise"), FLabelAlignment.Left),

            new OpUpdown(VerticalBoostX, new Vector2(220f, 440f), 60f)
            {
                description = "Changes X (Run) ammount of the boost vector for plain Vertical boosts. (Default: 2)"
            },
            new OpLabel(new Vector2(290f, 405f), new Vector2(num, num), OptionInterface.Translate("Vertical Boost Run"), FLabelAlignment.Left),
            new OpUpdown(VerticalBoostY, new Vector2(220f, 400f), 60f)
            {
                description = "Changes Y (Rise) ammount of the boost vector for plain Vertical boosts. (Default: 16)"
            },
            new OpLabel(new Vector2(290f, 365f), new Vector2(num, num), OptionInterface.Translate("Vertical Boost Rise"), FLabelAlignment.Left),

            new OpLabel(new Vector2(200f, 270f), new Vector2(200f, 200f), OptionInterface.Translate("The Legacy Walljumping option is at Rain World Remix."), FLabelAlignment.Center, bigText: false),
            new OpLabel(new Vector2(200f, 235f), new Vector2(200f, 200f), OptionInterface.Translate("The Boost Stamina option is at the original Shuqing mod. Sorry!"), FLabelAlignment.Center, bigText: false)
        };
        mopTab.AddItems(array);


        OpTab creditTab = new OpTab(this, OptionInterface.Translate("Graffiti Credits"));
        OpContainer creditContainer = new OpContainer(new Vector2(0f, 0f));
        mopTab.AddItems(creditContainer);
        UIelement[] array2 = new UIelement[2]
        {
            new OpLabel(new Vector2(20f, 500f), new Vector2(250f, 20f), "RWS_of3: abrigo2", FLabelAlignment.Left, bigText: is_Big_Text),
            new OpLabel(new Vector2(20f, 480f), new Vector2(250f, 20f), "RWS_of3: abrigo2", FLabelAlignment.Left, bigText: is_Big_Text)
        };
        creditTab.AddItems(array2);


        Tabs = new OpTab[2] { mopTab, creditTab };
    }
}