using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;

public class PExp : MonoBehaviour
{
    public int level = 1;
    public int experience = 0;
    public List<int> experienceThresholds = new List<int> { 10, 20, 30 };
    [SerializeField] private Slider sliderExp;
    [SerializeField] private PSkills skillSelectorUI;
    [SerializeField] private Text lvlv;

    public List<Skill> allSkills = new List<Skill>();

    public void AddExperience(int amount)
    {
        experience += amount;
        sliderExp.value = experience;
        Debug.Log("exp");
        CheckLevelUp();
    }

    private void CheckLevelUp()
    {
        if (level - 1 < experienceThresholds.Count && experience >= experienceThresholds[level - 1])
        {
            experience = 0;
            level++;
            lvlv.text = "Уровень: " + level ;
            sliderExp.value = 0;
            sliderExp.maxValue = experienceThresholds[level - 1];
            Debug.Log("Level Up! Новый уровень: " + level);
            ShowSkillSelection();
        }
    }

    private void ShowSkillSelection()
    {
        // Возьмем случайные 3 навыка для выбора
        
        List<Skill> choices = new List<Skill>();
        while (choices.Count < 3 && allSkills.Count > 0)
        {
            Skill s = allSkills[Random.Range(0, allSkills.Count)];
            if (!choices.Contains(s)) choices.Add(s);
        }
        Debug.Log("skills");
        skillSelectorUI.ShowSkills(choices, OnSkillSelected);
    }

    private void OnSkillSelected(Skill chosen)
    {
        chosen.Activate(gameObject);
        Debug.Log("Выбран навык: " + chosen.skillName);
        // Можно добавить в список активных у игрока, если нужно
    }
}
