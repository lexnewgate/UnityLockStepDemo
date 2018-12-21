﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class SkillStatusSystem : SystemBase
{
    public override void OnEntityCreate(EntityBase entity)
    {
    }

    public override Type[] GetFilter()
    {
        return new Type[] {
            typeof(SkillStatusComponent),
        };
    }

    public override void FixedUpdate(int deltaTime)
    {
        List<EntityBase> list = GetEntityList();

        for (int i = 0; i < list.Count; i++)
        {
            SkillStatusLogic(list[i], deltaTime);
        }
    }

    public void SkillStatusLogic(EntityBase entity, int deltaTime)
    {
        SkillStatusComponent sc = entity.GetComp<SkillStatusComponent>();

        if (sc.m_skillStstus != SkillStatusEnum.Finish
            && sc.m_skillStstus != SkillStatusEnum.None)
        {
            //Debug.Log("ID: " + entity.ID + " m_skillStstus " + sc.m_skillStstus + " frame " + m_world.FrameCount +" skillTime "+ sc.m_skillTime +" LaterTime " + sc.m_currentSkillData.LaterTime);

            sc.m_skillTime += deltaTime;
            sc.FXTimer += deltaTime;

            if (sc.m_skillTime > sc.m_currentSkillData.BeforeTime)
            {
                if (sc.m_skillStstus != SkillStatusEnum.Current)
                {
                    sc.m_isEnter = true;
                    sc.m_skillStstus = SkillStatusEnum.Current;
                    m_world.eventSystem.DispatchEvent(GameUtils.c_SkillStatusEnter, entity);
                }
                else
                {
                    sc.m_isEnter = false;
                }
            }
            if (sc.m_skillTime > sc.m_currentSkillData.HitTime)
            {
                if (sc.m_currentSkillData.SkillInfo.m_Moment)
                {
                    if (sc.m_isTriggerSkill == false)
                    {
                        sc.m_isTriggerSkill = true;
                        sc.m_isHit = true;
                        m_world.eventSystem.DispatchEvent(GameUtils.c_SkillHit, entity);
                    }
                    else
                    {
                        sc.m_isHit = false;
                    }
                }
                else
                {
                    sc.m_skillTriggerTimeSpace -= deltaTime;
                    //Debug.Log("m_skillTriggerTimeSpace " + sc.m_skillTriggerTimeSpace);

                    if (sc.m_skillTriggerTimeSpace < 0)
                    {
                        sc.m_skillTriggerTimeSpace = sc.m_currentSkillData.SkillInfo.m_TriggerSpaceTime;
                        //加个伤害间隔
                        sc.m_isHit = true;
                        m_world.eventSystem.DispatchEvent(GameUtils.c_SkillHit, entity);
                    }
                    else
                    {
                        sc.m_isHit = false;
                    }
                }
            }

            if (sc.m_skillTime > sc.m_currentSkillData.CurrentTime)
            {
                sc.m_isHit = false;

                if (sc.m_skillStstus != SkillStatusEnum.Later)
                {
                    sc.m_isEnter = true;
                    sc.m_skillStstus = SkillStatusEnum.Later;
                    m_world.eventSystem.DispatchEvent(GameUtils.c_SkillStatusEnter, entity);
                }
                else
                {
                    sc.m_isEnter = false;
                }
            }

            if (sc.m_skillTime > sc.m_currentSkillData.LaterTime)
            {
                if (sc.m_skillStstus != SkillStatusEnum.Finish)
                {
                    sc.m_isEnter = true;
                    sc.m_skillStstus = SkillStatusEnum.Finish;
                    m_world.eventSystem.DispatchEvent(GameUtils.c_SkillStatusEnter, entity);
                }
                else
                {
                    sc.m_isEnter = false;
                }
            }
        }
    }
}
