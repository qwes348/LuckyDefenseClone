using System;
using System.Collections.Generic;
using UnityEngine;

public static class Calculator
{
    public static Dictionary<Define.UnitGrade, float> GetFinalSpawnProbability(int upgradeLevel)
    {
        Dictionary<Define.UnitGrade, float> resultDict = new Dictionary<Define.UnitGrade, float>();
        
        // 노말은 확률 감소
        float pNormal = Define.DefaultSpawnProbabilityDict[Define.UnitGrade.Normal] * (1 - Sigmoid(upgradeLevel, Define.SpawnProbabilityAdjustmentDict[Define.UnitGrade.Normal]));
        // 희귀랑 영웅은 증가
        float pRare = Define.DefaultSpawnProbabilityDict[Define.UnitGrade.Rare] * Sigmoid(upgradeLevel, Define.SpawnProbabilityAdjustmentDict[Define.UnitGrade.Rare]);
        float pHero = Define.DefaultSpawnProbabilityDict[Define.UnitGrade.Hero] * Sigmoid(upgradeLevel, Define.SpawnProbabilityAdjustmentDict[Define.UnitGrade.Hero]);
        
        float totalProbability = pNormal + pRare + pHero;
        // 정규화
        pNormal /= totalProbability;
        pRare /= totalProbability;
        pHero /= totalProbability;
        
        resultDict.Add(Define.UnitGrade.Normal, pNormal);
        resultDict.Add(Define.UnitGrade.Rare, pRare);
        resultDict.Add(Define.UnitGrade.Hero, pHero);
        return resultDict;
    }

    // 비선형 공식 -> 확률이 마이너스가 되는것을 막는다
    private static float Sigmoid(float x, float k)
    {
        return 1.0f / (1.0f + (float)Math.Exp(-k * x));
    }
}
