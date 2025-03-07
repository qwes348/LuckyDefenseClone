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

    private void Start()
    {
        Init();
    }

    private void Init()
    {
        if (playerType == Define.PlayerType.LocalPlayer)
            InGameManagers.FieldMgr.playerGrid = this;
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
        // unit.transform.position = (Vector3)(CellSize * targetCell.Coord) + transform.position;
        unit.gameObject.SetActive(true);
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
        private Vector2 coord;
        [SerializeField]
        private List<UnitController> myUnits = new List<UnitController>();

        private GridSystem myGrid;

        public bool IsOccupied => myUnits.Count > 0;
        public Vector2 Coord => coord;
        public List<UnitController> MyUnits => myUnits;

        public Cell(int x, int y, GridSystem grid)
        {
            coord = new Vector2(x, y);
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
            
            // 좌표 딕셔너리에 이 셀을 넣어준다
            // 1캐릭당 한 셀을 넣는게 아니고 한 종류당 한 셀
            if (myGrid.unitCoordDict.ContainsKey(unit.MyUnitData))
            {
                myGrid.unitCoordDict[unit.MyUnitData] ??= new List<Cell>();
            }
            else
            {
                myGrid.unitCoordDict.Add(unit.MyUnitData, new List<Cell>());
            }
            if(!myGrid.unitCoordDict[unit.MyUnitData].Contains(this))
                myGrid.unitCoordDict[unit.MyUnitData].Add(this);
            
            UpdateUnitsPosition();
        }

        // 유닛 판매할 경우 큐에서 1개씩 제거
        public UnitController RemoveAndGetUnit()
        {
            var unit = myUnits[^1];
            myUnits.Remove(unit);
            if(myUnits.Count == 0)
                myGrid.unitCoordDict[unit.MyUnitData].Remove(this);
            return unit;
        }

        // 합성 이동 등 클리어해야할 경우
        public void ClearUnits()
        {
            myGrid.unitCoordDict[myUnits[0].MyUnitData].Remove(this);
            myUnits.Clear();
        }

        private void UpdateUnitsPosition()
        {
            switch (myUnits.Count)
            {
                case 1:
                    myUnits[0].transform.position = (Vector3)(CellSize * coord) + myGrid.transform.position;
                    break;
                case 2:
                    myUnits[0].transform.DOMove((Vector3)(CellSize * coord) + myGrid.transform.position + (Vector3.right * 0.1f) + (Vector3.down *  0.1f), 0.15f);
                    myUnits[1].transform.position = (Vector3)(CellSize * coord) + myGrid.transform.position + (Vector3.left * 0.1f) + (Vector3.up * 0.1f);
                    break;
                default:
                    myUnits[0].transform.DOMove((Vector3)(CellSize * coord) + myGrid.transform.position + (Vector3.left * 0.1f) + (Vector3.down *  0.1f), 0.15f);
                    myUnits[1].transform.DOMove((Vector3)(CellSize * coord) + myGrid.transform.position + (Vector3.left * 0.1f) + (Vector3.up * 0.1f), 0.15f);
                    myUnits[2].transform.position = (Vector3)(CellSize * coord) + myGrid.transform.position + (Vector3.right * 0.1f);
                    break;
            }
        }
    }
}
