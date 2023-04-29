using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.SceneManagement;

[CreateAssetMenu(fileName = "New Character Database", menuName = "Chatacters/Character Database")]
public class CharacterDatabase : ScriptableObject
{
    public Character[] characters = new Character[0];

    public Character[] GetAllCharacters() => characters;

    public Character GetCharacterById(int id)
    {
        foreach (var character in characters)
        {
            if (character.id == id)
            {
                return character;
            }
        }

        return null;
    }
}
