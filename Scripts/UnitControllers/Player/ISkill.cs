namespace TankLike.Combat.Abilities
{
    public interface ISkill
    {
        void AddSkill(SkillHolder holder);
        void EquipSkill(SkillHolder holder);
        void ReEquipSkill();
    }
}
