using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CharacterSelectButton : MonoBehaviour
{
    [SerializeField] private Image iconImage;

    private CharacterSelectDisplay characterSelect;

    private Character character;

    public void SetCharacter(CharacterSelectDisplay characterSelect, Character character)
    {
        iconImage.sprite = character.icon;

        this.characterSelect = characterSelect;
        this.character = character;
    }

    public void SelectCharacter()
    {
        characterSelect.Select(character);
    }
}
