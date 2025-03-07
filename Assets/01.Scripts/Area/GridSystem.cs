using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GridSystem : MonoBehaviour
{
    [SerializeField]
    private int width;
    [SerializeField]
    private int height;
    [SerializeField]
    private Row[] rows;
    [SerializeField]
    private  Define.PlayerType playerType;
    [SerializeField]
    private Dictionary<UnitData, List<Cell>> unitCoordDict = new Dictionary<UnitData, List<Cell>>();
    
    private const float CellSize = 0.6f;

    private GridGraphicController gridGraphic;

    private void Start()
    {
        Init();
    }

    private void Init()
    {
        if (playerType == Define.PlayerType.LocalPlayer)
        {
            InGameManagers.FieldMgr.playerGrid = this;
            gridGraphic = GetComponent<GridGraphicController>();
        }
        else
            InGameManagers.FieldMgr.opponentGrid = this;
        
        rows = new Row[height];
        for (int y = 0; y < height; y++)
        {
            rows[y] = new Row();
            rows[y].columns = new Cell[width];
            for (int x = 0; x < width; x++)
            {
                rows[y].columns[x] = new Cell(x, y, this);
            }
        }
    }

    public void OnNewUnitspawned(UnitController unit)
    {
        Cell targetCell = null;
        
        // 같은 유닛이 배치된 셀이 있는지 확인
        if (unitCoordDict.TryGetValue(unit.MyUnitData, out List<Cell> value))
        {
            // 같은 유닛 셀을 돌면서 겹쳐서 배치가 가능한지 확인
            foreach (var cell in value.Where(cell => cell.MyUnits.Count < 3))
            {
                targetCell = cell;
            }
        }
        
        // 같은 유닛이있는 들어갈 자리를 못찾았다면 첫번째 빈 셀에 들어감
        if(targetCell == null)
        {
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    if (rows[y].columns[x].IsOccupied)
                    {
                        continue;
                    }
                    targetCell = rows[y].columns[x];
                    break;
                }
                if (targetCell != null)
                    break;
            }
        }

        if (targetCell == null)
        {
            Debug.LogErrorFormat("유닛이 들어갈 Cell을 찾지못함: {0}", unit.name);
            return;
        }
        
        targetCell.AddUnit(unit);
        unit.gameObject.SetActive(true);
    }

    // 월드 포지션을 그리드 좌표로 변환 (그리드내의 유효한 좌표는 아닐 수 있음)
    public Vector2Int WorldToGridPosition(Vector3 worldPos)
    {
        Vector3 localSpacePos = transform.InverseTransformPoint(worldPos);
        int x = Mathf.RoundToInt(localSpacePos.x / CellSize);
        int y = Mathf.RoundToInt(localSpacePos.y / CellSize);
        return new Vector2Int(x, y);
    }

    // 주어진 좌표가 그리드내에 유효한 좌표인지 확인
    public bool IsWithinGrid(Vector2Int gridPos)
    {
        return gridPos.x >= 0 && gridPos.x < width && gridPos.y >= 0 && gridPos.y < height;
    }
    
    public Cell GetCell(Vector2Int gridPos)
    {
        return rows[gridPos.y].columns[gridPos.x];
    }

    // 유닛 이동
    public void MoveUnits(Cell fromCell, Cell destCell)
    {
        if (destCell.IsOccupied)
            return;
        
        List<UnitController> units = new List<UnitController>();
        for (int i = fromCell.MyUnits.Count - 1; i >= 0; i--)
        {
            units.Add(fromCell.RemoveAndGetUnit());
        }
        destCell.AddUnits(units);
    }

    private void OnDrawGizmos()
    {
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                Gizmos.matrix = Matrix4x4.TRS(transform.position, transform.rotation, Vector3.one);
                try
                {
                    Gizmos.color = rows[y].columns[x].IsOccupied ? Color.red : Color.blue;
                }
                catch (Exception e)
                {
                    Gizmos.color = Color.blue;
                }
                Gizmos.DrawWireCube(new Vector3(x, y, 0) * CellSize, Vector3.one * CellSize);
            }
        }
    }
    
    [Serializable]
    public class Row
    {
        public Cell[] columns;
    }

    [Serializable]
    public class Cell
    {
        [SerializeField]
        private Vector2Int coord;
        [SerializeField]
        private List<UnitController> myUnits = new List<UnitController>();

        private GridSystem myGrid;
        private bool isSelected;

        public bool IsOccupied => myUnits.Count > 0;
        public Vector2Int Coord => coord;
        public List<UnitController> MyUnits => myUnits;

        public Cell(int x, int y, GridSystem grid)
        {
            coord = new Vector2Int(x, y);
            myGrid = grid;
        }

        // 유닛이 새로 생성된 경우 큐에 추가
        public void AddUnit(UnitController unit)
        {
            //TODO: 여기서 유닛의 포지션을 세팅해주자
            if (myUnits.Count >= 3)
            {
                Debug.LogErrorFormat("Cell 정원 초과인데 Add시도 포착됨: {0}", unit.MyUnitData.UnitName);
            }
            myUnits.Add(unit);
            unit.MyCell = this;
            
            RegisterUnitCoordDict(unit);
            
            UpdateUnitsPosition();
        }

        // 한번에 여러마리의 유닛을 추가할 때
        public void AddUnits(List<UnitController> units)
        {
            if (IsOccupied)
                return;
            myUnits = units;
            foreach (var unit in units)
            {
                unit.MyCell = this;
            }
            
            RegisterUnitCoordDict(units[0]);
            
            UpdateUnitsPosition();
        }

        // 나중에 같은 유닛찾을 때 쉽기위해 딕셔너리에 해당 유닛을 가진 셀로 이 셀을 등록하는 과정
        public void RegisterUnitCoordDict(UnitController unit)
        {
            // 좌표 딕셔너리에 이 셀을 넣어준다
            // 1캐릭당 한 셀을 넣는게 아니고 한 종류당 한 셀
            if (myGrid.unitCoordDict.ContainsKey(unit.MyUnitData))
            {
                // key는 있는데 셀 리스트는 원인불명의 이유로 null인 경우를 위한 분기
                myGrid.unitCoordDict[unit.MyUnitData] ??= new List<Cell>();
            }
            else
            {
                // key가 없는 경우 -> 이 유닛을 이번 게임에서 처음 뽑은 경우
                myGrid.unitCoordDict.Add(unit.MyUnitData, new List<Cell>());
            }
            // 이 유닛을 가지고있는 셀 리스트에 이 셀을 등록한다
            if(!myGrid.unitCoordDict[unit.MyUnitData].Contains(this))
                myGrid.unitCoordDict[unit.MyUnitData].Add(this);
        }

        // 유닛 판매할 경우 큐에서 1개씩 제거
        public UnitController RemoveAndGetUnit()
        {
            var unit = myUnits[^1];
            unit.MyCell = null;
            myUnits.Remove(unit);
            if(myUnits.Count == 0)
                myGrid.unitCoordDict[unit.MyUnitData].Remove(this);
            return unit;
        }

        // 합성 이동 등 클리어해야할 경우
        public void ClearUnits()
        {
            myGrid.unitCoordDict[myUnits[0].MyUnitData].Remove(this);
            foreach (var unit in myUnits)
            {
                unit.MyCell = null;
            }
            myUnits.Clear();
        }

        // 이 셀의 유닛 수가 바꼈을 때 유닛들의 위치를 재조정
        private void UpdateUnitsPosition()
        {
            switch (myUnits.Count)
            {
                case 1:
                    myUnits[0].transform.DOMove(GetWorldPosition(), 0.15f);
                    break;
                case 2:
                    myUnits[0].transform.DOMove(GetWorldPosition() + (Vector3.right * 0.1f) + (Vector3.down *  0.1f), 0.15f);
                    myUnits[1].transform.DOMove(GetWorldPosition() + (Vector3.left * 0.1f) + (Vector3.up * 0.1f), 0.15f);
                    break;
                default:
                    myUnits[0].transform.DOMove(GetWorldPosition() + (Vector3.left * 0.1f) + (Vector3.down *  0.1f), 0.15f);
                    myUnits[1].transform.DOMove(GetWorldPosition() + (Vector3.left * 0.1f) + (Vector3.up * 0.1f), 0.15f);
                    myUnits[2].transform.DOMove(GetWorldPosition() + (Vector3.right * 0.1f), 0.15f);
                    break;
            }
        }

        public Vector3 GetWorldPosition()
        {
            Vector3 pos = new Vector3(coord.x * CellSize, coord.y * CellSize, 0);
            return pos + myGrid.transform.position;
        }
    }
}
