using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewItemList", menuName = "NewItemList")]
public class ItemList : ScriptableObject
{
    [SerializeField] public List<ItemData> itemDatas = new List<ItemData>();
}
