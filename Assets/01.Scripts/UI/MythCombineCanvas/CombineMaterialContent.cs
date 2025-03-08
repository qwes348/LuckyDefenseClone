using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CombineMaterialContent : MonoBehaviour
{
    [SerializeField]
    private Image bgImage;
    [SerializeField]
    private Image unitImage;
    [SerializeField]
    private Image checkImage;
    [SerializeField]
    private TextMeshProUGUI holdStateText;
    [SerializeField]
    private Color holdColor;

    public void Init(UnitData data, bool ishold)
    {
        bgImage.color = Define.UnitFootholdColorDict[data.Grade];
        unitImage.sprite = data.ThumbnailSprite;
        checkImage.gameObject.SetActive(ishold);
        holdStateText.text = ishold ? "보유" : "미보유";
        holdStateText.color = ishold ? holdColor : Color.white;
    }
}
