using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MythCombineCanvas : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI unitNameText;
    [SerializeField]
    private TextMeshProUGUI progressText;
    [SerializeField]
    private List<MythCombineListContent> listContents = new List<MythCombineListContent>();
    [SerializeField]
    private List<CombineMaterialContent> materialContents = new List<CombineMaterialContent>();
    [SerializeField]
    private Image mythUnitImage;
    [SerializeField]
    private Button spawnButton;
    
    private MythCombineListContent currentSelectedContent = null;

    private void Start()
    {
        spawnButton.onClick.AddListener(SpawnMythUnit);
    }

    public void Init()
    {
        var mythCombineDatas = InGameManagers.UnitSpawnMgr.MythCombineDatas;
        for (int i = 0; i < mythCombineDatas.Count; i++)
        {
            MythCombineData mythCombineData = mythCombineDatas[i];
            if (listContents.Count <= i)
            {
                MythCombineListContent clone = Instantiate(listContents[0], listContents[0].transform.parent);
                listContents.Add(clone);
            }

            int progress = GetProgress(mythCombineData);
            listContents[i].CombineCanvas = this;
            listContents[i].Init(InGameManagers.UnitSpawnMgr.GetUnitDataById(mythCombineData.unitId), progress);
        }
        
        listContents[0].Select();
    }

    public void SetActiveCanvas(bool active)
    {
        if(active)
            Init();
        gameObject.SetActive(active);
    }

    // 신화 유닛 목록에서 새 컨텐츠가 선택됨
    public void OnListContentSelected(MythCombineListContent content)
    {
        if(currentSelectedContent != null)
            currentSelectedContent.Deselect();
        
        unitNameText.text = content.MyUnitData.UnitName;
        var combineData = InGameManagers.UnitSpawnMgr.MythCombineDatas.First(d => d.unitId == content.MyUnitData.UnitId);
        int progress = GetProgress(combineData);
        progressText.text = $"진행률: {progress}%";
        InitMaterialContents(combineData);

        mythUnitImage.sprite = content.MyUnitData.ThumbnailSprite;
        spawnButton.gameObject.SetActive(progress >= 100);
        currentSelectedContent = content;
    }

    // 필요 영웅 UI 초기화
    private void InitMaterialContents(MythCombineData combineData)
    {
        for (int i = 0; i < combineData.materialUnits.Count; i++)
        {
            if (materialContents.Count <= i)
            {
                var clone = Instantiate(materialContents[0], materialContents[0].transform.parent);
                materialContents.Add(clone);
            }
            var unitData = InGameManagers.UnitSpawnMgr.GetUnitDataById(combineData.materialUnits[i].unitId);
            bool isHold = InGameManagers.FieldMgr.playerGrid.IsUnitExist(unitData);
            materialContents[i].Init(unitData, isHold);
        }
    }

    // 신화 소환(UI버튼에 연결)
    public void SpawnMythUnit()
    {
        var combineData = InGameManagers.UnitSpawnMgr.MythCombineDatas.First(data => data.unitId == currentSelectedContent.MyUnitData.UnitId);
        InGameManagers.UnitSpawnMgr.CombineMythUnit(combineData, Define.PlayerType.LocalPlayer);
        SetActiveCanvas(false);
    }

    // 진행률 리턴
    private int GetProgress(MythCombineData mythCombineData)
    {
        int holdCount = 0;
        for (int i = 0; i < mythCombineData.materialUnits.Count; i++)
        {
            MaterialUnit material = mythCombineData.materialUnits[i];
            UnitData materialUnitData = InGameManagers.UnitSpawnMgr.GetUnitDataById(material.unitId);
            bool isHold = InGameManagers.FieldMgr.playerGrid.IsUnitExist(materialUnitData);
            if (isHold)
                holdCount++;
        }
        int progress = Mathf.RoundToInt(((float)holdCount / mythCombineData.materialUnits.Count) * 100f);
        return progress;
    }
}
