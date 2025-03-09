using System;
using UnityEngine;

[RequireComponent(typeof(GridSystem))]
public class GridGraphicController : MonoBehaviour
{
    // TODO: 모든 셀 반투명 그래픽
    [SerializeField]
    private SpriteRenderer pointerDownSprite;   // 클릭을 시작한 셀을 표시해줄 스프라이트
    [SerializeField]
    private SpriteRenderer draggingSprite;  // 드래그중 현재 포인터 아래 셀을 표시해줄 스프라이트
    [SerializeField]
    private SpriteRenderer attackRangeSprite;   // 공격범위 스프라이트

    private GridSystem grid;

    private void Awake()
    {
        grid = GetComponent<GridSystem>();
        pointerDownSprite.gameObject.SetActive(false);
        draggingSprite.gameObject.SetActive(false);
        SetActiveAttackRange(false);
    }

    private void Start()
    {
        InGameUiManager.Instance.UnitManage.onActiveStateChanged += active =>
        {
            if (!active)
                SetActiveAttackRange(false);
        };
    }

    public void OnCellPointerDown(GridSystem.Cell cell)
    {
        pointerDownSprite.transform.position = cell.GetWorldPosition();
        InitAttackRange(cell);
    }

    public void OnCellDragging(GridSystem.Cell cell)
    {
        pointerDownSprite.gameObject.SetActive(true);
        draggingSprite.transform.position = cell.GetWorldPosition();
        draggingSprite.gameObject.SetActive(true);
        attackRangeSprite.gameObject.SetActive(true);
    }

    // 드래그중에 마우스가 그리드내에 없거나 포인터 다운한 셀과 겹쳐있을 때 잠시 드래그 이미지를 끔
    public void OnCellDraggingHide()
    {
        pointerDownSprite.gameObject.SetActive(false);
        draggingSprite.gameObject.SetActive(false);
    }

    public void OnCellPointerUp()
    {
        pointerDownSprite.gameObject.SetActive(false);
        draggingSprite.gameObject.SetActive(false);
    }

    public void InitAttackRange(GridSystem.Cell cell)
    {
        attackRangeSprite.transform.position = cell.GetWorldPosition();
        attackRangeSprite.transform.localScale = cell.MyUnits[0].MyUnitData.AttackRagne * Vector3.one;
    }

    public void SetActiveAttackRange(bool active)
    {
        attackRangeSprite.gameObject.SetActive(active);
    }
}
