using UnityEngine;
using Core;

namespace Systems
{
    public class InputRouter : MonoBehaviour
    {
        [SerializeField] GameRunner runner;
        Vector2 start;
        const float threshold = 40f;

        void Update()
        {
#if UNITY_STANDALONE || UNITY_EDITOR
            if (Input.GetKeyDown(KeyCode.UpArrow)) runner.OnSwipe(new Vector2Int(0, 1));
            if (Input.GetKeyDown(KeyCode.DownArrow)) runner.OnSwipe(new Vector2Int(0, -1));
            if (Input.GetKeyDown(KeyCode.LeftArrow)) runner.OnSwipe(new Vector2Int(-1, 0));
            if (Input.GetKeyDown(KeyCode.RightArrow)) runner.OnSwipe(new Vector2Int(1, 0));
#endif
            if (Input.GetMouseButtonDown(0)) start = Input.mousePosition;
            if (Input.GetMouseButtonUp(0))
            {
                var d = (Vector2)Input.mousePosition - start;
                if (d.magnitude < threshold) return;
                runner.OnSwipe(Mathf.Abs(d.x) > Mathf.Abs(d.y)
                    ? new Vector2Int(d.x > 0 ? 1 : -1, 0)
                    : new Vector2Int(0, d.y > 0 ? 1 : -1));
            }
        }
    }
}
