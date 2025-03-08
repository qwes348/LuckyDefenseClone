using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MythCombineListContent : MonoBehaviour
{
    [SerializeField]
    private Image bgImage;
    [SerializeField]
    private Image unitImage;
    [SerializeField]
    private TextMeshProUGUI progressText;
    [SerializeField]
    private Color normalColor;
    [SerializeField]
    private Color highlightedColor;

    private UnitData myUnitData;
    
    public UnitData MyUnitData => myUnitData;
    public MythCombineCanvas CombineCanvas { get; set; }

    public void Init(UnitData data, int progress)
    {
        myUnitData = data;
        unitImage.sprite = data.ThumbnailSprite;
        progressText.text = $"진행률 {progress}%";
        bgImage.color = normalColor;
    }

    public void Select()
    {
        bgImage.color = highlightedColor;
        CombineCanvas.OnListContentSelected(this);
    }

    public void Deselect()
    {
        bgImage.color = normalColor;
    }
}
