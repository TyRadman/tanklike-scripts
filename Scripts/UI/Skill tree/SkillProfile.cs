using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

namespace TankLike.Combat.SkillTree
{
    using Combat.Abilities;
    using Upgrades;
    using UnitControllers;
    using System.Text;
    using Utils;

    /// <summary>
    /// Holds the skill and information related to it for the skill tree like the skill points required, rank, etc.
    /// </summary>
    [CreateAssetMenu(fileName = "SP_NAME_00", menuName = Directories.SKILL_TREE + "Skill Profile")]
    public class SkillProfile : ScriptableObject
    {
        public SkillHolder SkillHolder;
        public int SkillPointsRequired = 1;
        public VideoClip PreviewVideo;
        [field: SerializeField] public List<SkillUpgrade> Upgrades { get; set; } = new List<SkillUpgrade>();

        public void RegisterSkill(TankComponents entity)
        {
            if (entity is not PlayerComponents player)
            {
                return;
            }

            if (SkillHolder is WeaponHolder)
            {
                player.Upgrades.SetBaseWeapon(this);
            }
            else if (SkillHolder is HoldAbilityHolder)
            {
                player.Upgrades.SetChargeAttack(this);
            }
            else if (SkillHolder is SuperAbilityHolder)
            {
                player.Upgrades.SetSuperAbility(this);
            }
            else if (SkillHolder is BoostAbilityHolder)
            {
                player.Upgrades.SetBoostAbility(this);
            }
        }

        public void EquipSkill(TankComponents entity)
        {
            SkillHolder.ApplySkill(entity);

            if (entity is not PlayerComponents player)
            {
                return;
            }

            // TODO: must work for all components, not just the player
            //Upgrades.ForEach(u => u.SetUp(entity as PlayerComponents));
        }
    }

    public class SkillProperties
    {
        public bool IsComparisonValue;
        public string Name;
        public string PreviousValue;
        public string Value;
        public Color DisplayColor;
        public string UnitString;
        public string CustomDescription = string.Empty;

        public static Color OldValueColor = Colors.Red;
        public static Color NewValueColor = Colors.Green;

        // Combo 1: Vibrant Red and Green
        public static readonly Color VibrantRed = new Color(0.902f, 0.196f, 0.196f); // #E63232
        public static readonly Color VibrantGreen = new Color(0.196f, 0.784f, 0.275f); // #32C846

        // Combo 2: Deep Terracotta and Rich Sage
        public static readonly Color DeepTerracotta = new Color(0.941f, 0.392f, 0.294f); // #F0644B
        public static readonly Color RichSage = new Color(0.392f, 0.706f, 0.549f); // #64B48C

        // Combo 3: Bold Brick Red and Bright Lime
        public static readonly Color BoldBrickRed = new Color(0.784f, 0.235f, 0.078f); // #C83C14
        public static readonly Color BrightLime = new Color(0.471f, 0.941f, 0.353f); // #78F05A

        // Combo 4: Bright Orange and Deep Teal
        public static readonly Color BrightOrange = new Color(1f, 0.471f, 0.078f); // #FF7814
        public static readonly Color DeepTeal = new Color(0f, 0.667f, 0.588f); // #00AA96

        // Combo 5: Crisp Gray and Sharp Yellow
        public static readonly Color CrispGray = new Color(0.745f, 0.745f, 0.745f); // #BEBEBE
        public static readonly Color SharpYellow = new Color(1f, 0.863f, 0f); // #FFD900

        // Combo 6: Dark Burgundy and Cool Mint
        public static readonly Color DarkBurgundy = new Color(0.549f, 0f, 0.157f); // #8C0028
        public static readonly Color CoolMint = new Color(0.392f, 1f, 0.392f); // #64FF64

        // Combo 7: Rich Blue and Hot Pink
        public static readonly Color RichBlue = new Color(0.157f, 0.471f, 0.745f); // #2878BF
        public static readonly Color HotPink = new Color(1f, 0.314f, 0.392f); // #FF5075

        // Combo 8: Warm Sepia and Sky Blue
        public static readonly Color WarmSepia = new Color(0.51f, 0.314f, 0.118f); // #82501E
        public static readonly Color SkyBlue = new Color(0.314f, 0.706f, 0.902f); // #50B4E6

        // Combo 9: Dark Charcoal and Electric Green
        public static readonly Color DarkCharcoal = new Color(0.157f, 0.196f, 0.235f); // #28323C
        public static readonly Color ElectricGreen = new Color(0.196f, 1f, 0.196f); // #32FF32

        // Combo 10: Rusty Orange and Aqua Blue
        public static readonly Color RustyOrange = new Color(0.824f, 0.353f, 0.078f); // #D25A14
        public static readonly Color AquaBlue = new Color(0.196f, 0.784f, 0.941f); // #32C8F0




        public string GetPropertiesAsString()
        {
            if (IsComparisonValue)
            {
                return GetComparisonText();
            }
            else
            {
                return GetValueText();
            }
        }

        public string GetValueText()
        {
            StringBuilder propertiesText = new StringBuilder();

            if (CustomDescription.Length == 0)
            {
                propertiesText.Append($"\n {Name}: {$"{Value} {UnitString}".Color(DisplayColor)}");
            }
            else
            {
                propertiesText.Append(CustomDescription);
            }

            return propertiesText.ToString();
        }

        public string GetComparisonText()
        {
            StringBuilder propertiesText = new StringBuilder();

            if (CustomDescription.Length == 0)
            {
                propertiesText.Append($"\n{Name}: \n{$"{PreviousValue.Color(BoldBrickRed)} -> {Value.Color(BrightLime)} {UnitString}"}");
            }
            else
            {
                propertiesText.Append(CustomDescription);
            }

            return propertiesText.ToString();
        }
    }
}
