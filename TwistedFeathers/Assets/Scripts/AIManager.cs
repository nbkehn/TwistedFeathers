﻿using System.Collections;
using System.Collections.Generic;
using TwistedFeathers;
using UnityEngine;

namespace TwistedFeathers
{

    public class AIManager : MonoBehaviour
    {

        public Skill chooseAttack(Monster enemy, List<TwistedFeathers.Player> battle_players,
            List<Monster> battle_enemy)
        {
            Debug.Log("Choosing Attack for enemy type: " + enemy.getEnemyType());

            int type = enemy.getEnemyType();
            Skill selected = null;
            if (type == 1) //thief goose
            {
                //choose primary attack (knife)
                Debug.Log("Choosing knife for thief");
                selected = enemy.AttackSkills["Knife Attack"];
                Debug.Log("retrieved skill success");


            }
            else if (type == 2) //necro crow
            {
                //Debug.Log("Shouldn't be here");
                //choose attack
                //  if Paralysis Spell
                //      "flip coin"
                //      if(heads -> Dark Magic)
                //      else-> Paralysis Spell//attack secondary
                //  else
                //      Dark Magic//attack primary
                if (enemy.AttackSkills.ContainsKey("Dark Magic II"))
                {
                    selected = enemy.AttackSkills["Dark Magic II"];
                }
                if (enemy.AttackSkills.ContainsKey("Paralysis Spell"))
                {
                    System.Random rand = new System.Random();
                    int result = rand.Next(1, 3);
                    if (result == 1)
                    {
                        selected = enemy.AttackSkills["Paralysis Spell"];
                    }
                }
                else if(selected == null)
                {
                    selected = enemy.AttackSkills["Dark Magic"];
                }

            }
            else
            {
                throw new System.Exception("Invalid Enemy Type");
            }

            Debug.Log("Here is the selected skill: " + selected.Name);

            return selected;
        }

        public Skill chooseUtil(Monster enemy, List<TwistedFeathers.Player> battle_players, List<Monster> battle_enemy)
        {
            int type = enemy.getEnemyType();
            Skill selected = null;
            if (type == 1) //thief goose
            {
                //if health is <= 75%
                if (((float) enemy.Current_hp / (float) enemy.Max_hp) <= .75)
                {
                    // if Rally is unlocked//
                    //  random generate number 1 out of 3
                    //  choose Rally if 3//
                    //  else choose Hiss
                    //else
                    //  choose Hiss//
                    if (enemy.UtilitySkills.ContainsKey("Rally"))
                    {
                        System.Random rand = new System.Random();
                        int result = rand.Next(1, 4);
                        if (result == 3)
                        {
                            selected = enemy.UtilitySkills["Rally"];
                        }

                        selected = enemy.UtilitySkills["Hiss"];
                    }
                    else
                    {
                        selected = enemy.UtilitySkills["Hiss"];
                    }
                }
                else
                {
                    selected = enemy.UtilitySkills["Hiss"];
                }
            }
            else if (type == 2) //crow
            {
                //  for battle_enemy list
                //      if enemy with < 50%
                //          Healing// utility primary
                //      else
                //          Fear Curse//utility secondary
                if (enemy.UtilitySkills.ContainsKey("Resurrection") && GameObject.Find("CombatManager").GetComponent<CombatManager>().checkForDeadEnemy())
                {
                    selected = enemy.UtilitySkills["Resurrection"];
                }
                else
                {
                    foreach (Monster enem in battle_enemy)
                    {
                        if (((float)enem.Current_hp / (float)enem.Max_hp) <= .5)
                        {
                            selected = enemy.UtilitySkills["Healing"];
                        }
                    }
                }
                
                if (selected == null && enemy.UtilitySkills.ContainsKey("Fear Curse II"))
                {
                    selected = enemy.UtilitySkills["Fear Curse II"];
                } else
                {
                    selected = enemy.UtilitySkills["Fear Curse"];
                }
            }
            else
            {
                throw new System.Exception("Invalid Enemy Type");
            }

            return selected;
        }

        public List<BattleParticipant> selectTarget(Monster enemy, List<TwistedFeathers.Player> battle_players,
            List<Monster> battle_enemy)
        {
            int type = enemy.getEnemyType();
            List<BattleParticipant> targets = new List<BattleParticipant>();
            Player target = null;
            if (type == 1) //thief goose
            {
                //for battle_players
                foreach (Player play in battle_players)
                {
                    if (((float) play.Current_hp / (float) play.Max_hp) <= .5)
                    {
                        target = play;
                        break;
                    }
                }

                if (target == null)
                {
                    System.Random rand = new System.Random();
                    int result = rand.Next(1, 3);
                    int i = 0;
                    foreach (Player play in battle_players)
                    {
                        i++;
                        if (i == result)
                        {
                            target = play;
                            break;
                        }
                    }
                }

                //   if player is < 50% health
                //       set Target to player
                //       break;
                //if target still null
                //   flip coin
            }
            else if (type == 2) //necro crow
            {
                //set target to healthiest player->
                //set target to first player in battle list
                //if battle_player[1] health is greater than current target
                //  set target to player 2
                target = battle_players[0];
                foreach (Player play in battle_players)
                {
                    if (play.Current_hp > target.Current_hp)
                    {
                        target = play;
                    }
                }

            }
            else
            {
                throw new System.Exception("Invalid Enemy Type");
            }

            if (target != null)
            {
                targets.Add(target);
                return targets;
            }
            else
            {
                target = battle_players[0];
                targets.Add(target);
                return targets;
            }

        }

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}
