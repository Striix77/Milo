using System;
using System.Collections.Generic;
using Microsoft.Unity.VisualStudio.Editor;
using UnityEngine;
using UnityEngine.UI;

public class SkillTreeManager : MonoBehaviour
{
    public SkillTree skillTree;
    public GameObject skillTreeUI;
    public GameObject skillTreePanel;
    public GameObject player;
    public PlayerMovementStats playerMovementStats;
    public PlayerShooter playerShooter;
    public Slider healthSlider;

    void Awake()
    {
        skillTree.activeSkills = new List<SkillTree.SkillType>();
        skillTree.activeSkills.Clear();
        playerMovementStats.MaxWalkSpeed = 9.2f;
        playerMovementStats.NumberOfJumps = 2;
        playerMovementStats.NumberOfDashes = 1;
        playerMovementStats.DashCooldown = 3f;
        Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("Player Projectiles"), LayerMask.NameToLayer("Ground"), false);

    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            skillTreePanel.SetActive(!skillTreePanel.activeSelf);
            skillTreeUI.SetActive(skillTreePanel.activeSelf);
        }
    }

    public void AddSkill(SkillTree.SkillType skill)
    {
        if (!skillTree.activeSkills.Contains(skill))
        {
            skillTree.activeSkills.Add(skill);
            ManageSkill(skill);
            Debug.Log($"Skill {skill} added to the skill tree.");
        }
        else
        {
            Debug.Log($"Skill {skill} is already in the skill tree.");
        }
    }

    public void AddSkill(String skill)
    {
        if (Enum.TryParse(skill, out SkillTree.SkillType skillType))
        {
            AddSkill(skillType);
        }
        else
        {
            Debug.Log($"Skill {skill} is not a valid skill type.");
        }
    }

    public void ManageSkill(SkillTree.SkillType skill)
    {
        if (skill.Equals(SkillTree.SkillType.HPBoost))
        {
            // Implement HP Boost logic
            PlayerStatusManager playerStatus = player.GetComponent<PlayerStatusManager>();
            playerStatus.health += playerStatus.health * 25 / 100;
            healthSlider.maxValue = playerStatus.health;
            Debug.Log("HP Boost applied. New health: " + playerStatus.health);
        }
        else if (skill.Equals(SkillTree.SkillType.PeaPhase))
        {
            // Implement Pea Phase logic
            // Disable collision between PlayerProjectiles (assume layer 8) and Ground (assume layer 6)
            Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("Player Projectiles"), LayerMask.NameToLayer("Ground"), true);
            Debug.Log("Pea Phase applied: Player projectiles now ignore ground collisions.");
        }
        else if (skill.Equals(SkillTree.SkillType.SpeedBoost))
        {
            playerMovementStats.MaxWalkSpeed += playerMovementStats.MaxWalkSpeed * 15 / 100;
            Debug.Log("Speed Boost applied. New speed: " + playerMovementStats.MaxWalkSpeed);
        }
        else if (skill.Equals(SkillTree.SkillType.PlusJump))
        {
            playerMovementStats.NumberOfJumps += 1;

        }
        else if (skill.Equals(SkillTree.SkillType.PlusDash))
        {
            playerMovementStats.NumberOfDashes += 1;

        }
        else if (skill.Equals(SkillTree.SkillType.CooldownBoost))
        {
            // Implement Cooldown Boost logic
            player.GetComponent<PlayerAbilities>().startTimeBtwAbility1 -= player.GetComponent<PlayerAbilities>().startTimeBtwAbility1 * 20 / 100;
            playerMovementStats.DashCooldown -= playerMovementStats.DashCooldown * 20 / 100;
        }
        else if (skill.Equals(SkillTree.SkillType.PeaRate))
        {
            // Implement Pea Rate logic
            playerShooter.startTimeBtwFire -= playerShooter.startTimeBtwFire * 20 / 100;
        }
        else if (skill.Equals(SkillTree.SkillType.FrostTime))
        {
            // Implement Frost Time logic
            player.GetComponent<PlayerAbilities>().freezeTime -= player.GetComponent<PlayerAbilities>().freezeTime * 20 / 100;
        }
        else if (skill.Equals(SkillTree.SkillType.DMGHP))
        {
            // Implement DMG HP logic
            PlayerStatusManager playerStatus = player.GetComponent<PlayerStatusManager>();
            playerStatus.health += playerStatus.health / 2;
            healthSlider.maxValue = playerStatus.health;
            if (playerStatus.health > playerStatus.maxHealth)
            {
                playerStatus.health = playerStatus.maxHealth;
            }
            Debug.Log("HP halved. New health: " + playerStatus.health);
        }
    }

    public float ApplyDmgBuff(float damage)
    {
        if (skillTree.activeSkills.Contains(SkillTree.SkillType.DMGHP))
        {
            damage += damage * 30 / 100;
        }
        return damage;
    }

    public float ApplyHurricaneBuff(float points)
    {
        if (skillTree.activeSkills.Contains(SkillTree.SkillType.HurricaneBoost))
        {
            points = points * 0.15f;
        }
        else
        {
            points = points * 0.1f;
        }
        return points;
    }



}
