using System;
using UnityEngine;
using UnityEngine.UI;

public class AnyButton : MonoBehaviour
{
    public static event EventHandler OnAnyButtonClick;
    public static void ResetStaticData()
    {
        OnAnyButtonClick = null;
    }
    private void Start()
    {
        GetComponent<Button>().onClick.AddListener(() => OnAnyButtonClick?.Invoke(null, EventArgs.Empty));
    }

    // for disabled button recoloring by Animator
    public void DisabledButtonColor()
    {
        GetComponentInChildren<UseColor>().RecolorButtonDisabled();
    }
    public void EnabledButtonColor()
    {
        GetComponentInChildren<UseColor>().RecolorButtonEnabled();
    }
}
