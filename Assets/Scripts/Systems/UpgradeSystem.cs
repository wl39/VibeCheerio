using Data;
using UnityEngine;

namespace Systems
{
    public class UpgradeSystem
    {
        // effect: 승급 타일이 합체에 참여할 때의 결과값 계산
        public int Apply(UpgradeEffect effect, int a, int b, int level /*1 or 2*/)
        {
            // level==2(승급+)는 보너스를 주도록 간단 가중
            switch (effect)
            {
                case UpgradeEffect.Add: return a + b + (level == 2 ? 1 : 0);
                case UpgradeEffect.Multiply: return a * b + (level == 2 ? 1 : 0);
                case UpgradeEffect.Max: return Mathf.Max(a, b) + (level == 2 ? 1 : 0);
                case UpgradeEffect.Abs: return Mathf.Abs(a) + Mathf.Abs(b) + (level == 2 ? 0 : 0);
                case UpgradeEffect.DoubleMerge:
                    // 같은 턴에 한 번 더 합체 시도는 상태머신 훅이 필요하므로 값은 +1로만 처리
                    return Mathf.Max(a, b) + 1 + (level == 2 ? 1 : 0);
                default: return Mathf.Max(a, b) + 1;
            }
        }
    }
}
