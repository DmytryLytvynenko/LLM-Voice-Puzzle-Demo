using TMPro;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(TMP_InputField))]
public class WorldUIInputField : MonoBehaviour
{
    private TMP_InputField m_inputField;
    void Start()
    {
        m_inputField = GetComponent<TMP_InputField>();
    }
    public void OnSelected()
    {
        print("Selected " + gameObject.name);
        ReferanceContainer.PlayerMovement.DisableMovement();
    }
    public void OnDeselected()
    {
        print("Deselected " + gameObject.name);
        ReferanceContainer.PlayerMovement.EnableMovement();
    }

}
