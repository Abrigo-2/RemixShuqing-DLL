using Menu.Remix.MixedUI;
using MoreSlugcats;
using UnityEngine;

public class ShuqingRemixOptions : OptionInterface
{
    public readonly Configurable<int> BoostAngleHorizontal;
    public readonly Configurable<int> BoostAngleVertical;
    public readonly Configurable<bool> HideTutorials;
    public readonly Configurable<bool> UntestedWallpounceLeniency;

    public ShuqingRemixOptions()
    {
        BoostAngleHorizontal = config.Bind("Suqing_H_Boost_Angle", 22, new ConfigurableInfo("Angle between player and floor during horizontal boosts.", new ConfigAcceptableRange<int>(0, 90), "Settings", "Horizontal Boost Angle"));
        BoostAngleVertical = config.Bind("Suqing_V_Boost_Angle", 83, new ConfigurableInfo("Angle between player and floor during vertical boosts.", new ConfigAcceptableRange<int>(0, 90), "Settings", "Vertical Boost Angle"));
        HideTutorials = config.Bind("Suqing_Hide_Tutorials", defaultValue:false);
        UntestedWallpounceLeniency = config.Bind("Suqing_Always_Wallpounce_On", defaultValue: false);
    }

    public override void Initialize()
    {
        int num = 100;
        bool is_Big_Text = false;
        Color cheatColor = new Color(0.85f, 0.35f, 0.4f);

    OpTab mopTab = new OpTab(this, OptionInterface.Translate("Settings"));
        OpContainer opContainer = new OpContainer(new Vector2(0f, 0f));
        mopTab.AddItems(opContainer);

        UIelement[] array = new UIelement[10]
        {
            //new OpLabel(new Vector2(200f, 450f), new Vector2(200f, 200f), OptionInterface.Translate("The Boost Stamina option is at the original Shuqing. Sorry!"), FLabelAlignment.Center, bigText: false),

            new OpUpdown(BoostAngleHorizontal, new Vector2(20f, 440f), 60f)
            {
                description = "Angle between player and floor during HORIZONTAL boosts. (Default: 22°)"
            },
            new OpLabel(new Vector2(90f, 405f), new Vector2(num, num), OptionInterface.Translate("Horizontal Boost Angle"), FLabelAlignment.Left),
            new OpUpdown(BoostAngleVertical, new Vector2(20f, 400f), 60f)
            {
                description = "Angle between player and floor during VERTICAL boosts. (Default: 83°)"
            },
            new OpLabel(new Vector2(90f, 365f), new Vector2(num, num), OptionInterface.Translate("Vertical Boost Angle"), FLabelAlignment.Left),

            new OpCheckBox(HideTutorials, new Vector2(260f, 440f))
            {
                description = "Onscreen tutorials for Shuqing's mechanics will no longer show."
            },
            new OpLabel(new Vector2(290f, 405f), new Vector2(num, num), OptionInterface.Translate("Hide Tutorials"), FLabelAlignment.Left),
            new OpCheckBox(UntestedWallpounceLeniency, new Vector2(260f, 400f))
            {
                colorEdge = cheatColor,
                description = "! UNTESTED BEHAVIOR ! Enables Wallpouncing leniency even if already allowed by RWRemix."
            },
            new OpLabel(new Vector2(290f, 365f), new Vector2(num, num), OptionInterface.Translate("Force Easy Wallpounces"), FLabelAlignment.Left)
            {
                color = cheatColor
            },

            new OpLabel(new Vector2(200f, 290f), new Vector2(200f, 200f), OptionInterface.Translate("The Legacy Walljumping option is at Rain World Remix."), FLabelAlignment.Center, bigText: false),
            new OpLabel(new Vector2(200f, 255f), new Vector2(200f, 200f), OptionInterface.Translate("The Boost Stamina option is at the original Shuqing mod. Sorry!"), FLabelAlignment.Center, bigText: false)
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