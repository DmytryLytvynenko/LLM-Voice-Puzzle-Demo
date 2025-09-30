using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "NewItemData", menuName = "ItemData/NewItemData")]
public class ItemData : ScriptableObject
{
    [field:SerializeField] public string Name { get; private set; }
    [field:SerializeField, TextArea(5,10)] public string Description { get; private set; }
    [field:SerializeField] public GameObject Prefab { get; private set; }
}
