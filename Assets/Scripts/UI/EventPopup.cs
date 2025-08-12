using UnityEngine;
using TMPro;
using Core;

public class EventPopup : MonoBehaviour
{
    [SerializeField] GameRunner runner;
    [SerializeField] GameObject root; // 이 스크립트가 붙은 패널
    [SerializeField] TMP_Text title;

    int pendingWildId = -1;

    public void OpenWild(int wildId, string name)
    {
        pendingWildId = wildId;
        title.text = $"이벤트: 와일드 '{name}' 생성?";
        root.SetActive(true);
    }

    public void BtnConfirm()
    {
        if (pendingWildId >= 0) runner.SpawnWildRandom(pendingWildId);
        Close();
    }
    public void BtnSkip() => Close();
    void Close() { root.SetActive(false); pendingWildId = -1; }
}
