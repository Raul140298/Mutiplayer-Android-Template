using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.SceneManagement;

[CreateAssetMenu(fileName = "New Character", menuName = "Chatacters/Character")]
public class Character : ScriptableObject
{
    public int id = -1;
    public string displayName = "New Display Name";
    public Sprite icon;
}
