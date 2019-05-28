using BanditPlus;
using EntityStates;
using RoR2;
using UnityEngine;
using UnityEngine.Networking;

namespace BanditPlus
{
    public class NewUtility : BaseState
    {
        private float totalDuration = 0.2f;

        public static void SetUtility(GameObject gameObject)
        {
            SkillLocator component = gameObject.GetComponent<SkillLocator>();
            GenericSkill skillslot = component.utility;

            SkillManagement.SetSkill(ref skillslot, typeof(NewUtility));
            component.utility = skillslot;
            //config();
        }

        public override void OnEnter()
        {
            base.OnEnter();
            Chat.AddMessage("NewUtility");
            ResetSkills();
        }

        public override void OnExit()
        {
            base.OnExit();
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            if(fixedAge >= this.totalDuration)
            {
                outer.SetNextStateToMain();
                return;
            }
        }
        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.Any;
        }

        private void ResetSkills()
        {
            SkillLocator skill = gameObject?.GetComponentInChildren<SkillLocator>();
            blam(skill);
        }

        public void blam(SkillLocator skillLocator)
        {

            GenericSkill[] skills = { skillLocator?.primary, skillLocator?.secondary, skillLocator?.special };
            foreach (var skill in skills)
            {
                while (skill?.stock < skill?.maxStock)
                {
                    skill?.RunRecharge(1f);
                }
            }

        }
    }
}
