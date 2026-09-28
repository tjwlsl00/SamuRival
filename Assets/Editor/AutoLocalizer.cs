using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using TMPro;
using UnityEngine.Localization.Components;
using UnityEngine.Localization.Tables;
using UnityEditor.Localization;
using System.IO;

public class AutoLocalizer : EditorWindow
{
    [MenuItem("Tools/1. 프로젝트 모든 씬 텍스트 자동 추출 및 리프레시 연결")]
    public static void LocalizeAllScenes()
    {
        // 1. 스트링 테이블 컬렉션 가져오기
        var tableCollection = LocalizationEditorSettings.GetStringTableCollection("MyGameLabels");
        if (tableCollection == null)
        {
            Debug.LogError("스트링 테이블 'MyGameLabels'를 찾을 수 없습니다! 이름을 확인해주세요.");
            return;
        }

        var sharedTableData = tableCollection.SharedData;
        var currentTable = tableCollection.GetTable("ja-JP") as StringTable;

        if (currentTable == null)
        {
            Debug.LogError("ja-JP 테이블을 찾을 수 없습니다.");
            return;
        }

        // 현재 작업 중이던 씬이 있다면 저장 여부 묻기 (데이터 날림 방지)
        if (!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
        {
            Debug.LogWarning("작업이 취소되었습니다.");
            return;
        }

        // 2. 프로젝트 내의 모든 씬(.unity) 파일 경로 수집
        string[] allSceneGuids = AssetDatabase.FindAssets("t:Scene", new[] { "Assets" });
        int totalTextCount = 0;

        Debug.Log($"총 {allSceneGuids.Length}개의 씬을 탐색하며 실시간 이벤트를 연결합니다...");

        foreach (string guid in allSceneGuids)
        {
            string scenePath = AssetDatabase.GUIDToAssetPath(guid);
            
            // 3. 각 씬을 백그라운드에서 강제로 엽니다.
            SceneAsset sceneAsset = AssetDatabase.LoadAssetAtPath<SceneAsset>(scenePath);
            var scene = EditorSceneManager.OpenScene(scenePath, OpenSceneMode.Single);

            // 4. 해당 씬 안에 있는 모든 TextMeshProUGUI 컴포넌트 수집
            TextMeshProUGUI[] textComponents = FindObjectsByType<TextMeshProUGUI>(FindObjectsSortMode.None);
            int sceneTextCount = 0;

            foreach (var textComp in textComponents)
            {
                string originalText = textComp.text.Trim();
                if (string.IsNullOrEmpty(originalText) || float.TryParse(originalText, out _)) continue;

                // 고유 Key 생성
                string cleanKey = "KEY_" + originalText.Replace(" ", "_").Replace("\n", "_").Replace("\r", "_");
                if (cleanKey.Length > 20) cleanKey = cleanKey.Substring(0, 20);

                var sharedEntry = sharedTableData.GetEntry(cleanKey);
                if (sharedEntry == null)
                {
                    sharedEntry = sharedTableData.AddKey(cleanKey);
                }

                var tableEntry = currentTable.GetEntry(sharedEntry.Id);
                if (tableEntry == null)
                {
                    currentTable.AddEntry(sharedEntry.Id, originalText);
                }

                // 컴포넌트 추가 및 실시간 리프레시 이벤트 연결
                var localizeEvent = textComp.GetComponent<LocalizeStringEvent>();
                if (localizeEvent == null)
                {
                    localizeEvent = textComp.gameObject.AddComponent<LocalizeStringEvent>();
                }

                // 테이블 명과 Key값 세팅
                localizeEvent.StringReference.SetReference(tableCollection.SharedData.TableCollectionName, cleanKey);

                // 최신 에디터 API에서는 영구 리스너를 지울 때 개수만큼 반복문을 돌려 인덱스로 안전하게 삭제합니다.
                int listenerCount = localizeEvent.OnUpdateString.GetPersistentEventCount();
                for (int i = listenerCount - 1; i >= 0; i--)
                {
                    UnityEditor.Events.UnityEventTools.RemovePersistentListener(localizeEvent.OnUpdateString, i);
                }
                
                // 에디터 모드에서 TextMeshProUGUI.text = string 에 이벤트를 직렬화하여 박아넣습니다.
                var targetMethod = typeof(TextMeshProUGUI).GetProperty("text").GetSetMethod();
                var action = System.Delegate.CreateDelegate(typeof(UnityEngine.Events.UnityAction<string>), textComp, targetMethod) as UnityEngine.Events.UnityAction<string>;
                
                // OnUpdateString에 연결 구조를 최종 완성합니다.
                UnityEditor.Events.UnityEventTools.AddPersistentListener(localizeEvent.OnUpdateString, action);

                // 오브젝트가 변경되었음을 에디터 시스템에 기록
                EditorUtility.SetDirty(textComp.gameObject);
                sceneTextCount++;
                totalTextCount++;
            }

            // 씬에 변경사항이 있다면 저장
            if (sceneTextCount > 0)
            {
                EditorSceneManager.MarkSceneDirty(scene);
                EditorSceneManager.SaveScene(scene);
                Debug.Log($"[{scene.name}] 씬에서 {sceneTextCount}개 텍스트 리프레시 이벤트 결합 완료.");
            }
        }

        // 5. 스트링 테이블 최종 저장
        EditorUtility.SetDirty(tableCollection);
        EditorUtility.SetDirty(sharedTableData);
        EditorUtility.SetDirty(currentTable);
        AssetDatabase.SaveAssets();
        
        Debug.Log($"🏁 [전체 작업 완료] 모든 씬을 돌며 총 {totalTextCount}개의 텍스트 컴포넌트에 실시간 언어 리프레시 시스템을 에러 없이 완벽하게 구축했습니다!");
    }

    [MenuItem("Tools/2. 프로젝트 모든 씬 텍스트 Auto Size 일괄 활성화")]
    public static void EnableAutoSizeAllScenes()
    {
        if (!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
        {
            Debug.LogWarning("작업이 취소되었습니다.");
            return;
        }

        string[] allSceneGuids = AssetDatabase.FindAssets("t:Scene", new[] { "Assets" });
        int updatedCount = 0;

        Debug.Log($"총 {allSceneGuids.Length}개의 씬을 탐색하며 한 줄 고정 및 Auto Size 설정을 갱신합니다...");

        foreach (string guid in allSceneGuids)
        {
            string scenePath = AssetDatabase.GUIDToAssetPath(guid);
            var scene = EditorSceneManager.OpenScene(scenePath, OpenSceneMode.Single);

            TextMeshProUGUI[] textComponents = FindObjectsByType<TextMeshProUGUI>(FindObjectsSortMode.None);
            int sceneUpdatedCount = 0;

            foreach (var textComp in textComponents)
            {
                // 빈 칸이거나 숫자로만 된 UI는 건드리지 않고 스킵
                if (string.IsNullOrEmpty(textComp.text.Trim()) || float.TryParse(textComp.text.Trim(), out _)) continue;

                // 이미 세팅이 올바르게 되어 있다면 스킵 (불필요한 Dirty 방지)
                if (textComp.enableAutoSizing && !textComp.enableWordWrapping) continue;

                // 1. [핵심] 줄바꿈(Wrap)을 꺼서 강제로 한 줄로 만듭니다.
                textComp.enableWordWrapping = false;

                // 2. 자동 크기 제어(Auto Sizing)를 켭니다.
                textComp.enableAutoSizing = true;
                
                // 3. 최소/최대 폰트 크기 유연하게 지정
                float currentSize = textComp.fontSize;
                textComp.fontSizeMax = currentSize > 0 ? currentSize : 36f; // 현재 크기를 최대치로 고정
                textComp.fontSizeMin = 8f; // 글자가 아무리 길어도 최소 8포인트까지 줄어들며 한 줄 유지

                EditorUtility.SetDirty(textComp);
                sceneUpdatedCount++;
                updatedCount++;
            }

            if (sceneUpdatedCount > 0)
            {
                EditorSceneManager.MarkSceneDirty(scene);
                EditorSceneManager.SaveScene(scene);
                Debug.Log($"[{scene.name}] 씬에서 {sceneUpdatedCount}개 텍스트 한 줄 고정 및 크기 자동 조절 적용.");
            }
        }

        AssetDatabase.SaveAssets();
        Debug.Log($"🏁 [오토사이즈 일괄 적용 완료] 총 {updatedCount}개의 UI 텍스트 상자를 '줄바꿈 없음 + 크기 자동 축소'로 변경했습니다!");
    }

    [MenuItem("Tools/3. 프로젝트 모든 씬 텍스트 중앙정렬 일괄 수정")]
    public static void SetCenterAlignmentAllScenes()
    {
        if (!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
        {
            Debug.LogWarning("작업이 취소되었습니다.");
            return;
        }

        string[] allSceneGuids = AssetDatabase.FindAssets("t:Scene", new[] { "Assets" });
        int alignedCount = 0;

        Debug.Log($"총 {allSceneGuids.Length}개의 씬을 탐색하며 중앙 정렬 조정을 진행합니다...");

        foreach (string guid in allSceneGuids)
        {
            string scenePath = AssetDatabase.GUIDToAssetPath(guid);
            var scene = EditorSceneManager.OpenScene(scenePath, OpenSceneMode.Single);

            TextMeshProUGUI[] textComponents = FindObjectsByType<TextMeshProUGUI>(FindObjectsSortMode.None);
            int sceneAlignedCount = 0;

            foreach (var textComp in textComponents)
            {
                // 빈 칸이거나 숫자로만 된 UI는 건드리지 않고 스킵
                if (string.IsNullOrEmpty(textComp.text.Trim()) || float.TryParse(textComp.text.Trim(), out _)) continue;

                // 이미 완전히 가운데 정렬(Center + Middle = CenterOptions)인 상태가 아니라면 변경 적용
                if (textComp.alignment != TextAlignmentOptions.Center)
                {
                    // 가로 정렬: Center(가운데), 세로 정렬: Geometry/Middle(중앙) 설정을 원클릭으로 주입합니다.
                    textComp.alignment = TextAlignmentOptions.Center;

                    EditorUtility.SetDirty(textComp.gameObject);
                    sceneAlignedCount++;
                    alignedCount++;
                }
            }

            if (sceneAlignedCount > 0)
            {
                EditorSceneManager.MarkSceneDirty(scene);
                EditorSceneManager.SaveScene(scene);
            }
        }

        AssetDatabase.SaveAssets();
        Debug.Log($"🏁 [중앙정렬 교정 완료] 총 {alignedCount}개의 UI 텍스트 컴포넌트들을 가로/세로 정가운데 정렬로 보정했습니다!");
    }
}