using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;

public class PSkills : MonoBehaviour
{
    public GameObject panel;
    public Button skillButtonPrefab;
    public Transform buttonContainer;
    private Action<Skill> onSkillSelected;

    public void ShowSkills(List<Skill> skills, Action<Skill> callback)
    {
        panel.SetActive(true);
        onSkillSelected = callback;

        foreach (Transform child in buttonContainer)
            Destroy(child.gameObject);

        foreach (Skill skill in skills)
        {
            Button btn = Instantiate(skillButtonPrefab, buttonContainer);
            btn.GetComponentInChildren<TMP_Text>().text = skill.skillName;
            btn.onClick.AddListener(() => SelectSkill(skill));
        }
    }

    private void SelectSkill(Skill skill)
    {
        panel.SetActive(false);
        onSkillSelected?.Invoke(skill);
    }
}
