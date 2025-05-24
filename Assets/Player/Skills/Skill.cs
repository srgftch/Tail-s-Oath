using UnityEngine;

public enum SkillType { DamageBoost, HealthBoost, SpeedBoost, AttackRangeBoost }

[System.Serializable]
public class Skill
{
    public string skillName;
    public string description;
    public SkillType type;

    public void Activate(GameObject player)
    {
        switch (type)
        {
            case SkillType.DamageBoost:
                player.GetComponent<PAttack>().AttackUp(3);
                break;
            case SkillType.HealthBoost:
                player.GetComponent<PHealth>().MaxHealthUp(10);
                break;
            case SkillType.SpeedBoost:
                player.GetComponent<Player_control>().SpeedUp(2);
                break;
            case SkillType.AttackRangeBoost:
                player.GetComponent<PAttack>().AttackRangeUp(0.1f);
                break;
        }
    }
}
