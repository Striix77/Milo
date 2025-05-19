using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SkillTree", menuName = "Scriptable Objects/SkillTree")]
public class SkillTree : ScriptableObject
{
    public enum SkillType
    {
        HPBoost,
        SpeedBoost,
        PlusJump,
        PlusDash,
        CooldownBoost,
        PeaRate,
        FrostTime,
        DMGHP,
        HurricaneBoost,
        PeaPhase,
        BiteDash
    }

    public List<SkillType> activeSkills = new List<SkillType>();
}
