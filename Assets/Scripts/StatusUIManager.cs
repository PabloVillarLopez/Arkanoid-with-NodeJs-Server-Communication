using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class StatusUIManager : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI statusText;
    [SerializeField]
    private GameObject connectingPanel;

    public static bool canRefresh = true;
    public static bool canRefreshInit = true;

    // Update is called once per frame
    void Update()
    {
        HandleUI();
    }

    public void HandleUI()
    {
        if (!BallScript.canStart && canRefreshInit)
        {
            statusText.text = "Waiting to connect...";
            canRefreshInit = false;
        }
        else if (BallScript.canStart && canRefresh)
        {
            statusText.text = "Connecting";
            StartCoroutine(WaitAndHide());
            canRefresh = false;
        }
    }

    public IEnumerator WaitAndHide()
    {
        yield return new WaitForSeconds(1f);
        connectingPanel.SetActive(false);
    }
}
