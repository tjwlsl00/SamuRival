using UnityEngine;
using UnityEditor; 

public class TimeScaleViewer : EditorWindow
{
    [MenuItem("Window/TimeScale Viewer")]
    public static void ShowWindow()
    {
        // UnityEditor.EditorWindow의 GetWindow 메서드를 정상적으로 호출합니다.
        EditorWindow.GetWindow<TimeScaleViewer>("TimeScale 보기");
    }

    void OnGUI()
    {
        GUILayout.Space(10);
        GUILayout.Label("현재 시스템 타임 스케일", EditorStyles.boldLabel);
        
        // 실시간으로 Time.timeScale 값을 에디터 윈도우에 표시
        EditorGUILayout.FloatField("Time Scale : ", Time.timeScale);

        GUILayout.Space(10);
        if (GUILayout.Button("강제 1f로 초기화 (재생)"))
        {
            Time.timeScale = 1f;
        }

        // 에디터 창이 실시간으로 수치 변화를 반영하도록 리프레시
        Repaint();
    }
}
