using UnityEngine;
using System.Collections;
using UnityEngine.Localization.Settings;

public class Global_LanguageManager : MonoBehaviour
{
    // 싱글톤
    public static Global_LanguageManager Instance;

    // bool
    private bool isSwitching = false;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void OnClickChangeLanguage(string localeCode)
    {
        if (isSwitching) return;

        StartCoroutine(SwitchLanguageRoutine(localeCode));
    }

    private IEnumerator SwitchLanguageRoutine(string localeCode)
    {
        isSwitching = true;

        Debug.Log($"[Language] '{localeCode}' 언어로 전환을 시도합니다...");

        // 1. 유니티 로컬라이제이션 시스템이 초기화 완료될 때까지 안전하게 대기
        yield return LocalizationSettings.InitializationOperation;

        // 2. 프로젝트 셋팅 내에 해당 언어 코드가 등록되어 있는지 찾기
        var targetLocale = LocalizationSettings.AvailableLocales.GetLocale(localeCode);

        if (targetLocale != null)
        {
            // 3. 게임 시스템의 현재 활성화된 언어를 전역적으로 변경
            LocalizationSettings.SelectedLocale = targetLocale;
            Debug.Log($"[Language] 전환 성공! 현재 언어: {targetLocale.LocaleName}");
        }
        else
        {
            Debug.LogError($"[Language] 에러: 프로젝트 설정에서 '{localeCode}' 언어팩을 찾을 수 없습니다. Localization 테이블 설정을 확인하세요.");
        }

        isSwitching = false;
    }
}