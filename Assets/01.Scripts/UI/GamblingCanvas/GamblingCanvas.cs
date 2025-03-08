using System.Collections.Generic;
using UnityEngine;

public class GamblingCanvas : MonoBehaviour
{
    [SerializeField]
    private List<GamblingUiContent> gamblingUiContent;

    public void SetActiveCanvas(bool active)
    {
        if (active)
        {
            gamblingUiContent.ForEach(content => content.Init());
        }
        gameObject.SetActive(active);
    }
}
