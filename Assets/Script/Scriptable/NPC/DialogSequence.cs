using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(fileName = "DialogSequence", menuName = "MineCrawler/DialogSequence")]
public class DialogSequence : ScriptableObject
{
    public List<string> sequence;
}
