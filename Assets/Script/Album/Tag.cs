using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "New Item")]
public class Tag : ScriptableObject
{
    public int id;
    public new string name;
    public string description;
    public Sprite image;
}
