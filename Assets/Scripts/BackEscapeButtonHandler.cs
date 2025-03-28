using UnityEngine;

public class BackEscapeButtonHandler : MonoBehaviour
{
    [SerializeField] private MenuPopupGroup menuPopupGroup;
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape)) menuPopupGroup.Toggle();
    }
}
