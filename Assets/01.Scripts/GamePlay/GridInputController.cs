using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;

public class GridInputController : MonoBehaviour
{
    private GridSystem grid;
    private GridGraphicController gridGraphic;
    private GridSystem.Cell pointerDownCell;
    private bool isDragging;

    private void Awake()
    {
        grid = GetComponent<GridSystem>();
        gridGraphic = GetComponent<GridGraphicController>();
    }

    private void Update()
    {
        HandleInput();
        IsPointerOverUI();
    }

    private void HandleInput()
    {
        if (Input.GetMouseButtonDown(0) && !IsPointerOverUI())
        {
            var gridPos = MousePosToGridPos();
            if (grid.IsWithinGrid(gridPos))
            {
                pointerDownCell = grid.GetCell(gridPos);
                if (pointerDownCell == null || !pointerDownCell.IsOccupied)
                {
                    gridGraphic.SetActiveAttackRange(false);
                    pointerDownCell = null;
                    InGameUiManager.Instance.UnitManage.SetActive(false);
                    return;
                }
                
                gridGraphic.OnCellPointerDown(pointerDownCell);
            }
            else
            {
                gridGraphic.SetActiveAttackRange(false);
                pointerDownCell = null;
                InGameUiManager.Instance.UnitManage.SetActive(false);
            }
        }
        else if (Input.GetMouseButtonDown(0) && IsPointerOverUI())
        {
            // UI위에서 클릭을 시작했으면 드래그로 판정 안함
            isDragging = false;
        }

        if (pointerDownCell != null && Input.GetMouseButton(0) && !IsPointerOverUI()) // 드래그
        {
            var gridPos = MousePosToGridPos();
            if (grid.IsWithinGrid(gridPos))
            {
                isDragging = true;
                var cell = grid.GetCell(gridPos);
                if (cell != null && pointerDownCell != cell)
                {
                    // TODO: 반투명 셀 그래픽 On / 셀 포인터 그래픽들은 전부 on / 공격범위 on 
                    gridGraphic.OnCellDragging(grid.GetCell(gridPos));
                    gridGraphic.SetActiveAttackRange(true);
                }
                else // PointerDownCell과 지금 드래그 포인터아래에 있는 Cell이 동일한 셀일 때
                {
                    // TODO: 반투명 셀 그래픽은 On / 셀 포인터 그래픽들은 전부 off / 공격범위 off
                    gridGraphic.OnCellDraggingHide();
                    gridGraphic.SetActiveAttackRange(false);
                }
            }
            else
            {
                gridGraphic.OnCellDraggingHide();
            }
        }

        if (pointerDownCell != null && Input.GetMouseButtonUp(0) && !IsPointerOverUI())
        {
            var gridPos = MousePosToGridPos();
            if (grid.IsWithinGrid(gridPos))
            {
                var cell = grid.GetCell(gridPos);
                if (cell != null && pointerDownCell != cell)    // 유닛 이동
                {
                    if (!isDragging)
                        return;
                    grid.MoveUnits(pointerDownCell, cell);
                    gridGraphic.SetActiveAttackRange(false);
                    InGameUiManager.Instance.UnitManage.SetActive(false);
                }
                else  // 처음 클릭한 셀에서 포인터Up했을 때
                {
                    // TODO: 반투명 셀 그래픽은 off / 셀 포인터 그래픽들 전부 off / 공격범위 on
                    // TODO: 판매 합성 UI On
                    gridGraphic.SetActiveAttackRange(true);
                    InGameUiManager.Instance.UnitManage.Init(pointerDownCell);
                    InGameUiManager.Instance.UnitManage.SetActive(true);
                }
            }
            isDragging = false;
            gridGraphic.OnCellPointerUp();
        }
    }

    private Vector2Int MousePosToGridPos()
    {
        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mouseWorldPos.z = 0;
        Vector2Int gridPos = grid.WorldToGridPosition(mouseWorldPos);
        return gridPos;
    }

    private bool IsPointerOverUI()
    {
        PointerEventData pointerEventData = new PointerEventData(EventSystem.current)
        {
            position = Input.mousePosition
        };
        
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(pointerEventData, results);
        return results.Any(t => t.gameObject.layer == LayerMask.NameToLayer("UI"));
    }
}
