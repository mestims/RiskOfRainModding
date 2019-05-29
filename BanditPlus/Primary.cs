using BanditPlus;
using RoR2;
using UnityEngine;

namespace EntityStates.Bandit
{
    public class Primary : BaseState
    {
        bool didDoubleShot;

        public static void SetPrimary(GameObject gameObject)
        {
            SkillLocator component = gameObject.GetComponent<SkillLocator>();
            GenericSkill skillslot = component.primary;
            SkillManagement.SetSkill(ref skillslot, typeof(Primary));
            component.primary = skillslot;
            config();
        }

        public override void OnEnter()
        {
            base.OnEnter();
            var random = Random.value;
            didDoubleShot = random < doubleShotChance;
            if (didDoubleShot)
            {
                fireSkill(doubleShotAnimDuration, doubleShotAnimDuration, doubleShotAnimStart);            
            }
            else
            {
                fireSkill(baseMaxDuration / attackSpeedStat, baseMinDuration / attackSpeedStat, 2f);
            }
            
        }

        public override void OnExit()
        {
            if (didDoubleShot)
            {
                fireSkill(baseMaxDuration / attackSpeedStat, baseMinDuration / attackSpeedStat, 2f);
            }
            
            base.OnExit();
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            buttonReleased |= !inputBank.skill1.down;
            if (fixedAge >= maxDuration && isAuthority)
            {
                outer.SetNextStateToMain();
                return;
            }
            return;
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            if (buttonReleased && fixedAge >= minDuration)
            {
                return InterruptPriority.Any;
            }
            return InterruptPriority.Skill;
        }

        private void fireSkill(float max, float min, float animStart)
        {
            AddRecoil(-1f * recoilAmplitude, -2f * recoilAmplitude, -0.5f * recoilAmplitude, 0.5f * recoilAmplitude);
            maxDuration = max;
            minDuration = min;
            Ray aimRay = GetAimRay();
            StartAimMode(aimRay, animStart, false);
            Util.PlaySound(attackSoundString, gameObject);
            PlayAnimation("Gesture, Additive", "FireShotgun", "FireShotgun.playbackRate", maxDuration);
            PlayAnimation("Gesture, Override", "FireShotgun", "FireShotgun.playbackRate", maxDuration);
            string muzzleName = "MuzzleShotgun";
            if (effectPrefab)
            {
                EffectManager.instance.SimpleMuzzleFlash(effectPrefab, gameObject, muzzleName, false);
            }
            if (isAuthority)
            {

                new BulletAttack
                {
                    owner = gameObject,
                    weapon = gameObject,
                    origin = aimRay.origin,
                    aimVector = aimRay.direction,
                    minSpread = 0f,
                    maxSpread = characterBody.spreadBloomAngle,
                    bulletCount = 1,
                    procCoefficient = 1f,
                    damage = damageCoefficient * damageStat,
                    force = force,
                    falloffModel = BulletAttack.FalloffModel.DefaultBullet,
                    tracerEffectPrefab = tracerEffectPrefab,
                    muzzleName = muzzleName,
                    hitEffectPrefab = hitEffectPrefab,
                    isCrit = Util.CheckRoll(critStat, characterBody.master),
                    HitEffectNormal = false,
                    radius = 0f
                }.Fire();
            }
            characterBody.AddSpreadBloom(spreadBloomValue);
        }

        public static void config()
        {
            parse("DamageMultiplier", damageCoefficient, out damageCoefficient);

            parse("Knockback", force, out force);

            parse("automatic_usetime", baseMaxDuration, out baseMaxDuration);

            parse("Time_between_buttonmashes", baseMinDuration, out baseMinDuration);

            parse("Recoil", recoilAmplitude, out recoilAmplitude);

            parse("Max_spread", spreadBloomValue, out spreadBloomValue);

            parse("ProcChance", procCoefficient, out procCoefficient);
        }

        public static bool parse(string field, float defaultvalue, out float value)
        {
            return SkillManagement.configfloat(nameof(Primary), field, defaultvalue, out value);
        }

        public static GameObject effectPrefab = Resources.Load<GameObject>("Prefabs/Effects/MuzzleFlashes/MuzzleflAshBanditShotgun");

        public static GameObject hitEffectPrefab = Resources.Load<GameObject>("Prefabs/Effects/ImpactEffects/HitsparkBandit");

        public static GameObject tracerEffectPrefab = Resources.Load<GameObject>("Prefabs/Effects/Tracers/TracerBanditShotgun");

        public static float damageCoefficient = 1.7f;

        public static float force = 1000f;

        public static float baseMaxDuration = 0.7f;

        public static float baseMinDuration = 0.05f;

        public const string attackSoundString = "Play_bandit_M1_shot";

        public static float recoilAmplitude = 1.3f;

        public static float spreadBloomValue = 0.5f;

        public static float procCoefficient = 1f;

        public static float doubleShotChance = 0.2f;

        public static float doubleShotAnimDuration = 0.1f;

        public static float doubleShotAnimStart = 0.1f;

        private float maxDuration;

        private float minDuration;

        private bool buttonReleased;
    }
}
