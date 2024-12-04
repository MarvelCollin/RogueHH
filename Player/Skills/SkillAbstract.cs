using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SkillAbstract : BaseSkillUI
{
    public enum SkillType
    {
        Buff,
        Active
    }

    public Sprite skillSprite;
    public string skillName;
    public string skillDescription;
    public int skillLevel;
    public float skillCooldown;
    private float currentCooldown;
    public SkillType skillType;
    public float activeTime;
    private float currentActiveTime;

    public ParticleSystem skillParticleSystem;

    void Start()
    {
        InitializeSkill();
    }

    void Update()
    {
        UpdateCooldown();
        UpdateUI();
    }

    public abstract void UseSkill();

    public void UseSkillWithParticles()
    {
        UseSkill();
        if (skillParticleSystem != null)
        {
            GenerateParticleSystem(skillParticleSystem);
        }
    }

    public bool IsCooldownValid(float minCooldown, float maxCooldown)
    {
        return skillCooldown >= minCooldown && skillCooldown <= maxCooldown;
    }

    protected void ResetCooldown()
    {
        currentCooldown = skillCooldown;
    }

    protected bool CanUseSkill()
    
{   
        return currentCooldown <= 0;
    }

    public void InitializeSkill()
    {
        currentCooldown = 0f;
        if (skillType == SkillType.Buff)
        {
            currentActiveTime = activeTime;
        }
    }

    public void UpdateCooldown()
    {
        if (currentCooldown > 0)
        {
            currentCooldown -= Time.deltaTime;
        }
        if (skillType == SkillType.Buff && currentActiveTime > 0)
        {
            currentActiveTime -= Time.deltaTime;
        }
    }

    public void unlockSkill(){
        HideLockedTextAndShadow();
    }


    public void GenerateParticleSystem(ParticleSystem particleSystemPrefab)
    {
        ParticleSystem particleSystem = Instantiate(particleSystemPrefab, Player.Instance.transform.position, Quaternion.identity);
        particleSystem.transform.SetParent(Player.Instance.transform);
        particleSystem.Play();
        particleSystem.gameObject.SetActive(true);

        float duration = skillType == SkillType.Buff ? activeTime : 1f;
        Player.Instance.StartCoroutine(StopParticleSystemAfterDuration(particleSystem, duration));
    }

    private IEnumerator StopParticleSystemAfterDuration(ParticleSystem particleSystem, float duration)
    {
        yield return new WaitForSeconds(duration);
        particleSystem.Stop();
        particleSystem.gameObject.SetActive(false);
        Destroy(particleSystem.gameObject);
    }

    public override void UpdateUI()
    {
        if (skillImage != null)
        {
            skillImage.sprite = skillSprite;
        }
        if (lockedText != null)
        {
            lockedText.text = skillName;
        }
    }

}
