using System;
using System.Collections.Generic;
using UnityEngine.Serialization;

// 재료 유닛 정보를 담는 클래스
[Serializable]
public class MaterialUnit
{
    public string unitName;
    public int unitId;
}

// 신화 조합 정보를 담는 클래스
[Serializable]
public class MythCombineData
{
    public string unitName;
    public int unitId;
    public List<MaterialUnit> materialUnits;
}

// 전체 JSON 데이터를 담는 클래스
[Serializable]
public class MythCombineDataWrapper
{
    public List<MythCombineData> mythCombineDatas; // 조합 데이터 리스트
}
