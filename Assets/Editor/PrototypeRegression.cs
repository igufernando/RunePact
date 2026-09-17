using System;
using System.Linq;
using System.Security.Cryptography;
using RunePact.Core;

public static class PrototypeRegression
{
    public static void Run(Action<bool,string> check)
    {
        var oak=new Battle(4,null,1,EnemyFormation.Vanguard,new[]{Relic.OakBark});
        oak.Resolve(Effect.Guard,20,3,true);check(oak.Fighters[3].Shield==20,"oak never benefits enemies");
        oak.Resolve(Effect.Guard,20,0,false);oak.Resolve(Effect.Guard,20,0,false);check(oak.Fighters[0].Shield==50,"oak only first allied shield");
        oak.BeginEnemy();oak.BeginPlayer();oak.Resolve(Effect.Guard,20,0,false);check(oak.Fighters[0].Shield==30,"oak refreshes each turn");
        var ember=new Battle(4,null,1,EnemyFormation.Vanguard,new[]{Relic.AncientEmber});ember.Fighters[3].Burn=2;ember.Fighters[0].Burn=2;
        ember.BeginEnemy();check(ember.Fighters[3].Hp==146,"ember buffs damage on enemies");ember.BeginPlayer();check(ember.Fighters[0].Hp==139,"ember does not buff incoming burn");
        var arrow=new Battle(4,null,1,EnemyFormation.Vanguard,new[]{Relic.CeremonialArrow});
        arrow.Fighters[0].Mark=1;arrow.Resolve(Effect.Strike,10,0,true);check(arrow.Fighters[0].Hp==125,"arrow never buffs enemy attacks");
        arrow.Fighters[3].Mark=1;arrow.Resolve(Effect.Strike,10,3,false);check(arrow.Fighters[3].Hp==130,"arrow buffs friendly attacks");
        var mage=new Battle(4,null,1,EnemyFormation.Vanguard,new[]{Relic.LivingGrimoire});mage.Play(0,3,out _);
        check(mage.GetCost(new Card(1,0))==0,"mage discount independent of other heroes");mage.Play(mage.Hand.FindIndex(x=>x.Owner==1&&x.Slot==0),3,out _);
        check(mage.GetCost(new Card(1,0))==1,"mage discount consumed once");mage.BeginEnemy();mage.BeginPlayer();check(mage.GetCost(new Card(1,0))==0,"mage discount resets");
        var states=new Battle(5);states.Resolve(Effect.Vulnerable,0,3,false);states.Resolve(Effect.Strike,20,3,false);states.Resolve(Effect.Strike,20,3,false);states.Resolve(Effect.Strike,20,3,false);
        check(states.Fighters[3].Hp==85&&states.Fighters[3].Vulnerable==0,"vulnerable expires after exactly two hits");
        states.Fighters[0].Hp=50;states.Resolve(Effect.Regenerate,12,0,false);states.BeginEnemy();states.BeginPlayer();states.BeginEnemy();states.BeginPlayer();
        check(states.Fighters[0].Hp==82&&states.Fighters[0].Regen==0,"regeneration heals now and two turns");
        var stun=new Battle(7);stun.Resolve(Effect.Stun,0,4,false);stun.Resolve(Effect.Stun,0,4,false);stun.BeginEnemy();
        check(!stun.ExecuteIntent(1)&&stun.Fighters[4].Stun==0,"stun skips one action without stacking");
        var weak=new Battle(9);weak.Round=2;weak.Plan();weak.Resolve(Effect.Weaken,0,3,false);weak.BeginEnemy();
        int target=weak.Intents[0].Target,before=weak.Fighters[target].Hp;weak.ExecuteIntent(0);
        check(weak.Fighters[target].Hp==before-33&&weak.Fighters[3].Weak==0,"weak survives tick and weakens next action");
        var thorns=new Battle(10);thorns.Resolve(Effect.Thorns,7,0,false);thorns.Round=2;thorns.Plan();thorns.BeginEnemy();int enemyHp=thorns.Fighters[3].Hp;thorns.ExecuteIntent(0);
        check(thorns.Fighters[3].Hp==enemyHp-7,"thorns reflect true damage on attack");
        thorns.BeginPlayer();thorns.BeginEnemy();thorns.BeginPlayer();check(thorns.Fighters[0].Thorns==0,"thorns expire");
        foreach(CardVariant variant in Enum.GetValues(typeof(CardVariant))){var c=new Card(1,2,variant);check(!string.IsNullOrEmpty(states.Describe(c).Description),"catalog describes "+variant);}
        var bosses=new Battle(10,null,2,EnemyFormation.Regent);check(bosses.Fighters.Count(x=>x.Enemy&&x.Alive)==1&&bosses.Fighters[3].MaxHp==120,"quick boss solo 120 hp");
        bosses.Fighters[3].Hp=60;bosses.Plan();bosses.Plan();check(bosses.Fighters[3].Shield==12,"boss phase shield granted once");
        var boost=new Battle(3);boost.SkillBoost[0]=6;check(boost.Describe(new Card(0,0)).Power==34&&boost.Describe(new Card(0,1)).Power==28,"individual upgrade affects only selected ability");
        CheckPersistence(check);
        var relics=new System.Collections.Generic.HashSet<Relic>();var variants=new System.Collections.Generic.HashSet<CardVariant>();
        for(int seed=0;seed<45;seed++){
            var journey=new Journey(seed,new[]{seed%2,seed%2,seed%2});var offer=journey.GetOffers();relics.Add(offer[2].Relic);variants.Add(offer[0].Card);
            for(int stage=0;stage<2;stage++){
                var battle=journey.Current;int turns=0;
                while(battle.Outcome==0&&turns++<30){
                    for(int move=0;move<12&&battle.Outcome==0;move++){
                        int index=battle.Hand.FindIndex(c=>battle.Fighters[c.Owner].Alive&&battle.GetCost(c)<=battle.Energy);
                        if(index<0)break;var ability=battle.Describe(battle.Hand[index]);
                        int id=battle.Fighters.Where(f=>f.Alive&&f.Enemy!=ability.Friendly).OrderBy(f=>ability.Friendly?(float)f.Hp/f.MaxHp:f.Hp).First().Id;
                        check(battle.Play(index,id,out _),"varied journey legal move");
                    }
                    if(battle.Outcome!=0)break;battle.BeginEnemy();for(int i=0;i<battle.Intents.Count;i++)battle.ExecuteIntent(i);battle.BeginPlayer();
                    if(battle.PlayerTurn||battle.Outcome!=0){check(Journey.TryRestore(journey.Save(),out var resume)&&resume.Save()==journey.Save(),"varied battle exact roundtrip");}
                }
                check(battle.Outcome!=0,"varied journey battle terminates");
                check(battle.Fighters.All(f=>f.Hp>=0&&f.Hp<=f.MaxHp&&f.Shield>=0),"varied state bounds");
                if(stage==0){
                    // Força apenas o checkpoint para exercitar todas as recompensas, mesmo quando o bot perde.
                    //
                    // Force only the checkpoint to exercise every reward even when the bot loses.
                    battle.Fighters.Take(3).ToList().ForEach(f=>f.Hp=f.MaxHp);battle.Fighters.Skip(3).ToList().ForEach(f=>f.Hp=0);battle.CheckOutcome();
                    check(journey.ChooseReward(seed%3)&&journey.Encounter==2&&journey.Formation==EnemyFormation.Regent,"one encounter leads directly to boss");
                    if(seed%3==0)check(journey.Current.Hand.Any(c=>c.Variant==offer[0].Card),"reward card immediately available");
                }
            }
        }
        check(relics.Count==5&&variants.Count==9,"all five relics and nine journey cards reachable");
    }
    static void CheckPersistence(Action<bool,string> check)
    {
        var journey=new Journey(123,new[]{1,1,0});var b=journey.Current;b.Play(0,3,out _);b.Resolve(Effect.Fortify,28,0,false);b.Resolve(Effect.Regenerate,12,1,false);b.Resolve(Effect.Vulnerable,1,3,false);b.SkillBoost[6]=6;b.Upgrade(1,out _);
        string save=journey.Save();check(Journey.TryRestore(save,out var restored)&&restored.Save()==save,"exact save includes all battle state");
        for(int turn=0;turn<5&&b.Outcome==0;turn++){
            foreach(var battle in new[]{b,restored.Current}){battle.BeginEnemy();for(int i=0;i<battle.Intents.Count;i++)battle.ExecuteIntent(i);battle.BeginPlayer();}
            check(journey.Save()==restored.Save(),"restored rng and subsequent reshuffle deterministic");
        }
        check(!Journey.TryRestore("legacy|broken",out _)&&!Journey.TryRestore(save.Substring(0,save.Length-4),out _),"legacy and truncated saves rejected");
        char replacement=save[60]=='A'?'B':'A';string corrupt=save.Substring(0,60)+replacement+save.Substring(61);check(!Journey.TryRestore(corrupt,out _),"checksum rejects modified save");
        var victory=new Journey(55);victory.Current.Fighters.Skip(3).ToList().ForEach(f=>f.Hp=0);victory.Current.CheckOutcome();
        check(Journey.TryRestore(victory.Save(),out var result)&&result.AwaitingReward,"victory restores pending reward");
        var defeat=new Journey(55);defeat.Current.Fighters.Take(3).ToList().ForEach(f=>f.Hp=0);defeat.Current.CheckOutcome();
        check(Journey.TryRestore(defeat.Save(),out result)&&result.Current.Outcome==-1,"defeat remains terminal");
        var old=new Journey(43);foreach(var card in old.Current.Deck)card.Variant=CardVariant.Standard;
        // Recria o cabeçalho V4 removendo os 18 bytes de metadados adicionados na V5.
        // Recreate the V4 header by removing the 18 metadata bytes added in V5.
        byte[] current=Convert.FromBase64String(old.Save().Split(':')[1]);
        byte[] bytes=new byte[current.Length-18];
        Array.Copy(current,0,bytes,0,16);Array.Copy(current,34,bytes,16,current.Length-34);
        bytes[0]=4;bytes[1]=bytes[2]=bytes[3]=0;
        using(var hash=SHA256.Create()){
            string legacy=Convert.ToBase64String(hash.ComputeHash(bytes))+":"+Convert.ToBase64String(bytes);
            check(Journey.TryRestore(legacy,out var migrated)&&migrated.QuickMode&&migrated.Difficulty==EncounterDifficulty.Normal,"V4 save migration defaults to quick normal");
        }
    }
}
