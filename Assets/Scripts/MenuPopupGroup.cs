using UnityEngine;

public class MenuPopupGroup : MonoBehaviour
{
    private void Start()
    {
        Show();
    }
    public void Show()
    {
        GetComponent<Canvas>().enabled = true;
    }
    public void Hide()
    {
        GetComponent<Canvas>().enabled = false;
    }
    public void Toggle()
    {
        GetComponent<Canvas>().enabled = !GetComponent<Canvas>().enabled;
    }
}
