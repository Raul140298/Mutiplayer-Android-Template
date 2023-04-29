using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class PlayerCard : MonoBehaviour
{
    [SerializeField] private CharacterDatabase characterDatabase;

    [SerializeField] private GameObject visuals;

    [SerializeField] private Image characterIconImage;

    [SerializeField] private TMP_Text playerNameText;

    [SerializeField] private TMP_Text characterNameText;

    public void UpdateDisplay(CharacterSelectState state)
    {
        if (state.characterId != -1)
        {
            var character = characterDatabase.GetCharacterById(state.characterId);
            characterIconImage.sprite = character.icon;
            characterIconImage.enabled = true;
            characterNameText.text = character.displayName;
        }
        else
        {
            characterIconImage.enabled = false;
        }

        playerNameText.text = $"Player {state.clientId}";

        visuals.SetActive(true);
    }

    public void DisableDisplay()
    {
        visuals.SetActive(false);
    }
}
