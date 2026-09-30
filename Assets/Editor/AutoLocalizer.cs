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

        string originalScenePath = UnityEngine.SceneManagement.SceneManager.GetActiveScene().path;

        if (!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
        {
            Debug.LogWarning("작업이 취소되었습니다.");
            return;
        }

        string[] allSceneGuids = AssetDatabase.FindAssets("t:Scene", new[] { "Assets" });
        int totalTextCount = 0;

        Debug.Log($"총 {allSceneGuids.Length}개의 씬을 탐색하며 실시간 이벤트를 연결합니다...");

        foreach (string guid in allSceneGuids)
        {
            string scenePath = AssetDatabase.GUIDToAssetPath(guid);
            var scene = EditorSceneManager.OpenScene(scenePath, OpenSceneMode.Single);

            TextMeshProUGUI[] textComponents = FindObjectsByType<TextMeshProUGUI>(FindObjectsSortMode.None);
            int sceneTextCount = 0;

            foreach (var textComp in textComponents)
            {
                string originalText = textComp.text.Trim();
                if (string.IsNullOrEmpty(originalText) || float.TryParse(originalText, out _)) continue;

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

                Undo.RecordObject(textComp.gameObject, "Add Localization Component");

                var localizeEvent = textComp.GetComponent<LocalizeStringEvent>();
                if (localizeEvent == null)
                {
                    localizeEvent = textComp.gameObject.AddComponent<LocalizeStringEvent>();
                }

                Undo.RecordObject(localizeEvent, "Update Localization Reference");
                localizeEvent.StringReference.SetReference(tableCollection.SharedData.TableCollectionName, cleanKey);

                int listenerCount = localizeEvent.OnUpdateString.GetPersistentEventCount();
                for (int i = listenerCount - 1; i >= 0; i--)
                {
                    UnityEditor.Events.UnityEventTools.RemovePersistentListener(localizeEvent.OnUpdateString, i);
                }
                
                var targetMethod = typeof(TextMeshProUGUI).GetProperty("text").GetSetMethod();
                var action = System.Delegate.CreateDelegate(typeof(UnityEngine.Events.UnityAction<string>), textComp, targetMethod) as UnityEngine.Events.UnityAction<string>;
                
                UnityEditor.Events.UnityEventTools.AddPersistentListener(localizeEvent.OnUpdateString, action);
                localizeEvent.OnUpdateString.SetPersistentListenerState(0, UnityEngine.Events.UnityEventCallState.RuntimeOnly);

                var persistentEvent = localizeEvent.OnUpdateString;
                System.Reflection.MethodInfo setMethod = typeof(UnityEditor.Events.UnityEventTools).GetMethod("SetPersistentDisplayName", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic);
                if (setMethod != null)
                {
                    setMethod.Invoke(null, new object[] { persistentEvent, 0, "TextMeshProUGUI.text" });
                }

                EditorUtility.SetDirty(textComp);
                EditorUtility.SetDirty(localizeEvent);
                EditorUtility.SetDirty(textComp.gameObject);
                sceneTextCount++;
                totalTextCount++;
            }

            if (sceneTextCount > 0)
            {
                EditorSceneManager.MarkSceneDirty(scene);
                EditorSceneManager.SaveScene(scene);
                Debug.Log($"[{scene.name}] 씬에서 {sceneTextCount}개 텍스트 리프레시 이벤트 결합 완료.");
            }
        }

        // ⭐️ [컴파일 에러 해결 및 데이터 디스크 저장] 에러 유발 라인을 제거하고 표준 에디터 직렬화 방식으로 안전하게 저장
        EditorUtility.SetDirty(tableCollection);
        EditorUtility.SetDirty(sharedTableData);
        EditorUtility.SetDirty(currentTable);
        AssetDatabase.SaveAssets();

        if (!string.IsNullOrEmpty(originalScenePath) && File.Exists(originalScenePath))
        {
            EditorSceneManager.OpenScene(originalScenePath, OpenSceneMode.Single);
        }
        
        Debug.Log($"🏁 [전체 작업 완료] 모든 씬을 돌며 총 {totalTextCount}개의 텍스트 컴포넌트에 실시간 언어 리프레시 시스템을 연결했습니다!");
    }

    [MenuItem("Tools/2. 프로젝트 모든 씬 텍스트 Auto Size 및 Geometry 세로정렬 일괄 활성화")]
    public static void EnableAutoSizeAllScenes()
    {
        string originalScenePath = UnityEngine.SceneManagement.SceneManager.GetActiveScene().path;

        if (!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
        {
            Debug.LogWarning("작업이 취소되었습니다.");
            return;
        }

        string[] allSceneGuids = AssetDatabase.FindAssets("t:Scene", new[] { "Assets" });
        int updatedCount = 0;

        Debug.Log($"총 {allSceneGuids.Length}개의 씬을 탐색하며 한 줄 고정, Auto Size 및 Geometry 세로 정렬(5번째)을 적용합니다...");

        foreach (string guid in allSceneGuids)
        {
            string scenePath = AssetDatabase.GUIDToAssetPath(guid);
            var scene = EditorSceneManager.OpenScene(scenePath, OpenSceneMode.Single);

            TextMeshProUGUI[] textComponents = FindObjectsByType<TextMeshProUGUI>(FindObjectsSortMode.None);
            int sceneUpdatedCount = 0;

            foreach (var textComp in textComponents)
            {
                if (string.IsNullOrEmpty(textComp.text.Trim()) || float.TryParse(textComp.text.Trim(), out _)) continue;

                Undo.RecordObject(textComp, "Update TMP Alignment and AutoSize");

                bool isChanged = false;

                var currentHorizontal = textComp.horizontalAlignment;
                var targetAlignment = (TextAlignmentOptions)((int)currentHorizontal | (int)VerticalAlignmentOptions.Geometry);

                if (textComp.alignment != targetAlignment || textComp.verticalAlignment != VerticalAlignmentOptions.Geometry)
                {
                    textComp.alignment = targetAlignment;
                    textComp.verticalAlignment = VerticalAlignmentOptions.Geometry;
                    isChanged = true;
                }

                if (textComp.enableWordWrapping)
                {
                    textComp.enableWordWrapping = false;
                    isChanged = true;
                }

                if (!textComp.enableAutoSizing)
                {
                    textComp.enableAutoSizing = true;
                    isChanged = true;
                }

                float currentSize = textComp.fontSize;
                float targetMax = currentSize > 0 ? currentSize : 36f;
                float targetMin = 8f;

                if (Mathf.Abs(textComp.fontSizeMax - targetMax) > 0.01f || Mathf.Abs(textComp.fontSizeMin - targetMin) > 0.01f)
                {
                    textComp.fontSizeMax = targetMax;
                    textComp.fontSizeMin = targetMin;
                    isChanged = true;
                }

                if (isChanged)
                {
                    textComp.SetAllDirty();
                    textComp.ForceMeshUpdate();
                    
                    EditorUtility.SetDirty(textComp);
                    EditorUtility.SetDirty(textComp.gameObject);

                    sceneUpdatedCount++;
                    updatedCount++;
                }
            }

            if (sceneUpdatedCount > 0)
            {
                EditorSceneManager.MarkSceneDirty(scene);
                EditorSceneManager.SaveScene(scene);
                Debug.Log($"[{scene.name}] 씬에서 {sceneUpdatedCount}개 텍스트 설정 변경 완료.");
            }
        }

        AssetDatabase.SaveAssets();

        if (!string.IsNullOrEmpty(originalScenePath) && System.IO.File.Exists(originalScenePath))
        {
            EditorSceneManager.OpenScene(originalScenePath, OpenSceneMode.Single);
        }

        Debug.Log($"🏁 [오토사이즈 & Geometry 정렬 일괄 적용 완료] 총 {updatedCount}개의 UI 텍스트 상자를 변경했습니다!");
    }
}
