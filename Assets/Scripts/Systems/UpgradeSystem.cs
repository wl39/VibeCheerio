using Data;
using Upgrades;
using UnityEngine;

namespace Systems
{
    public class UpgradeSystem
    {
        // effect: 승급 타일이 합체에 참여할 때의 결과값 계산
        public int Apply(UpgradeSpec spec, int a, int b, int level /*1 or 2*/)
        {
            if (spec == null)
                return Mathf.Max(a, b) + 1;

            UpgradeEffectSpec mergeEffect = null;
            foreach (var eff in spec.effects)
            {
                if (eff.timing == TriggerTiming.OnMergePre || eff.timing == TriggerTiming.OnMergePost)
                {
                    mergeEffect = eff;
                    break;
                }
            }

            if (mergeEffect == null)
                return Mathf.Max(a, b) + 1;

            // level==2(승급+)는 보너스를 주도록 간단 가중
            switch (mergeEffect.type)
            {
                case "Add": return a + b + (level == 2 ? 1 : 0);
                case "Multiply": return a * b + (level == 2 ? 1 : 0);
                case "Max": return Mathf.Max(a, b) + (level == 2 ? 1 : 0);
                case "Abs": return Mathf.Abs(a) + Mathf.Abs(b) + (level == 2 ? 0 : 0);
                case "DoubleMerge":
                    // 같은 턴에 한 번 더 합체 시도는 상태머신 훅이 필요하므로 값은 +1로만 처리
                    return Mathf.Max(a, b) + 1 + (level == 2 ? 1 : 0);
                default: return Mathf.Max(a, b) + 1;
            }
        }
    }
}
