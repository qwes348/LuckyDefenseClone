using UnityEditor;
using UnityEditor.AddressableAssets;
using UnityEditor.AddressableAssets.Settings.GroupSchemas;
using UnityEngine;

public static class CustomSimplifyAddressableName
{
    [MenuItem("Oniboogie/어드레서블/어드레서블 네임 단순화 커스텀")]
    public static void SimplifyAddressableNames()
    {
        // Addressable Asset Settings를 가져옴
        var settings = AddressableAssetSettingsDefaultObject.Settings;

        if (settings == null)
        {
            // Addressable Asset Settings가 없을 경우 오류 메시지 출력
            EditorUtility.DisplayDialog("실패", "Addressable Asset Settings 못찾음", "OK");
            return;
        }

        // 모든 Addressable 그룹을 순회
        foreach (var group in settings.groups)
        {
            // 그룹이 null이거나 BundledAssetGroupSchema가 없는 경우 건너뜀
            if (group == null || group.HasSchema<BundledAssetGroupSchema>() == false)
                continue;

            // 그룹 내의 모든 엔트리를 순회
            foreach (var entry in group.entries)
            {
                if (entry == null)
                    continue;

                // 엔트리의 GUID를 통해 경로를 가져옴
                string assetPath = AssetDatabase.GUIDToAssetPath(entry.guid);
                if (string.IsNullOrEmpty(assetPath))
                    continue;

                // 간소화된 이름 생성
                string simplifiedName = GetSimplifiedName(assetPath);

                if (!string.IsNullOrEmpty(simplifiedName))
                {
                    // 엔트리의 Address를 설정
                    entry.SetAddress(simplifiedName);
                }
            }
        }

        // 변경사항 저장
        AssetDatabase.SaveAssets();
        EditorUtility.DisplayDialog("완료", "어드레서블 이름 단순화 완료", "OK");
    }

    private static string GetSimplifiedName(string assetPath)
    {
        // "Assets/" 접두사를 제거
        string relativePath = assetPath.StartsWith("Assets/") ? assetPath.Substring(7) : assetPath;

        // 경로를 구성 요소로 분리
        string[] pathComponents = relativePath.Split('/');

        // 최소한 폴더와 파일 이름이 있어야 함
        if (pathComponents.Length < 2)
            return null;

        // 상위 폴더와 확장자를 제외한 파일 이름 결합
        string folderName = pathComponents[0];
        // 폴더이름 앞에 00. 01. 같은 숫자 제거
        folderName = folderName.Remove(0, 3);
        string fileNameWithoutExtension = System.IO.Path.GetFileNameWithoutExtension(pathComponents[^1]);

        return $"{folderName}/{fileNameWithoutExtension}";
    }
}
