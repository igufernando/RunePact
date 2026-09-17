using System;
using System.Linq;
using RunePact.Core;

public static class PrototypeBalance
{
    public static string Run()
    {
        string report="Seeded baseline bot (24 journeys per mode/difficulty)\n";
        foreach(bool quick in new[]{true,false})foreach(EncounterDifficulty difficulty in Enum.GetValues(typeof(EncounterDifficulty))){
            int wins=0,cards=0,rounds=0;
            for(int seed=0;seed<24;seed++){
                var journey=new Journey(seed+300,null,quick,difficulty);
                for(int stage=0;stage<journey.TotalEncounters;stage++){
                    var battle=journey.Current;
                    int turns=0;
                    while(battle.Outcome==0&&turns++<35){
                        for(int move=0;move<12&&battle.Outcome==0;move++){
                            int index=battle.Hand.FindIndex(c=>battle.Fighters[c.Owner].Alive&&battle.GetCost(c)<=battle.Energy);
                            if(index<0)break;
                            var ability=battle.Describe(battle.Hand[index]);
                            var target=battle.Fighters.Where(f=>f.Alive&&f.Enemy!=ability.Friendly).OrderBy(f=>ability.Friendly?(float)f.Hp/f.MaxHp:f.Hp).First();
                            battle.Play(index,target.Id,out _);
                        }
                        if(battle.Outcome!=0)break;
                        battle.BeginEnemy();for(int i=0;i<battle.Intents.Count;i++)battle.ExecuteIntent(i);battle.BeginPlayer();
                    }
                    rounds+=battle.Round;
                    if(battle.Outcome!=1||journey.Complete)break;
                    journey.ChooseReward(seed%3);
                }
                cards+=journey.TotalCardsPlayed;
                if(journey.Complete)wins++;
            }
            report+=(quick?"Quick":"Standard")+" / "+difficulty+": wins "+wins+"/24, mean cards "+cards/24+", mean rounds "+rounds/24+"\n";
        }
        return report;
    }
}
