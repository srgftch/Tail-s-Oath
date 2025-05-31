using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;

public class PSkills : MonoBehaviour
{
    public Button skillButtonPrefab;
    public Transform buttonContainer;
    private Action<Skill> onSkillSelected;

    public void ShowSkills(List<Skill> skills, Action<Skill> callback)
    {
        Debug.Log("PANEL is: ");

        gameObject.SetActive(true);
        onSkillSelected = callback;

        foreach (Transform child in buttonContainer)
            Destroy(child.gameObject);

        foreach (Skill skill in skills)
        {
            Button btn = Instantiate(skillButtonPrefab, buttonContainer);
            btn.GetComponentInChildren<TMP_Text>().text = skill.skillName;
            btn.onClick.AddListener(() => SelectSkill(skill));
        }
        Debug.Log("skills more");
    }

    private void SelectSkill(Skill skill)
    {
        gameObject.SetActive(false);
        onSkillSelected?.Invoke(skill);
    }
}
