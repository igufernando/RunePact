using System;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEditor.Build.Reporting;
using UnityEngine;
using RunePact.Core;
using RunePact.Presentation;

public static class PrototypeBuild
{
    [MenuItem("RunePact/Prepare scene and test")]
    public static void Prepare()
    {
        foreach(var path in new[]{"Assets/Resources/Art/Warriors.png","Assets/Resources/Art/Arena.png","Assets/Resources/Art/Regent-v1.png","Assets/Resources/Art/WarriorActions-v1.png","Assets/Resources/Art/Rewards-v2.png"}) {
            var importer=(TextureImporter)AssetImporter.GetAtPath(path);
            importer.textureType=TextureImporterType.Default;importer.isReadable=!path.Contains("Arena");importer.filterMode=FilterMode.Point;importer.textureCompression=TextureImporterCompression.Uncompressed;importer.maxTextureSize=2048;importer.npotScale=TextureImporterNPOTScale.None;importer.mipmapEnabled=false;importer.alphaSource=TextureImporterAlphaSource.FromInput;importer.SaveAndReimport();
        }
        ValidateAtlas("Assets/Resources/Art/WarriorActions-v1.png",3,3);
        ValidateAtlas("Assets/Resources/Art/Rewards-v2.png",4,2);
        Directory.CreateDirectory("Assets/Scenes");
        var scene=EditorSceneManager.NewScene(NewSceneSetup.EmptyScene,NewSceneMode.Single);
        new GameObject("RunePact Battle",typeof(BattleScreen));
        EditorSceneManager.SaveScene(scene,"Assets/Scenes/Battle.unity");
        EditorBuildSettings.scenes=new[]{new EditorBuildSettingsScene("Assets/Scenes/Battle.unity",true)};
        PlayerSettings.companyName="Independent Prototype";PlayerSettings.productName="RunePact";
        PlayerSettings.defaultScreenWidth=1600;PlayerSettings.defaultScreenHeight=900;PlayerSettings.fullScreenMode=FullScreenMode.Windowed;PlayerSettings.resizableWindow=true;PlayerSettings.runInBackground=true;
        PlayerSettings.defaultInterfaceOrientation=UIOrientation.LandscapeLeft;
        PlayerSettings.SetScriptingBackend(UnityEditor.Build.NamedBuildTarget.Standalone,ScriptingImplementation.Mono2x);
        PlayerSettings.colorSpace=ColorSpace.Gamma;
        AssetDatabase.SaveAssets();Test();
    }
    [MenuItem("RunePact/Build Windows prototype")]
    public static void Run()
    {
        Prepare();Directory.CreateDirectory("Builds/Windows");
        var report=BuildPipeline.BuildPlayer(new BuildPlayerOptions {scenes=new[]{"Assets/Scenes/Battle.unity"},locationPathName="Builds/Windows/RunePact.exe",target=BuildTarget.StandaloneWindows64,options=BuildOptions.None});
        if(report.summary.result!=BuildResult.Succeeded)throw new Exception("Build failed: "+report.summary.result);
        Debug.Log("RUNEPACT_BUILD_SUCCESS "+report.summary.totalSize);
    }
    static void ValidateAtlas(string path,int columns,int rows)
    {
        var texture=AssetDatabase.LoadAssetAtPath<Texture2D>(path);
        var pixels=texture.GetPixels32();
        for(int row=0;row<rows;row++)for(int column=0;column<columns;column++){
            int left=column*texture.width/columns,right=(column+1)*texture.width/columns;
            int bottom=row*texture.height/rows,top=(row+1)*texture.height/rows;
            int visible=0,transparent=0,edge=0;
            for(int y=bottom;y<top;y++)for(int x=left;x<right;x++){
                byte alpha=pixels[y*texture.width+x].a;
                if(alpha<10)transparent++;
                if(alpha>40){visible++;if(x==left||x==right-1||y==bottom||y==top-1)edge++;}
            }
            int area=(right-left)*(top-bottom);
            if(visible<area/100||transparent<area/5||edge>0)
                throw new Exception("Invalid sprite alpha or cell margins: "+path+" cell "+column+","+row+" visible="+visible+" transparent="+transparent+" edge="+edge);
        }
        Debug.Log("RUNEPACT_ART_VALIDATED "+path+" cells="+(columns*rows));
    }
    [MenuItem("RunePact/Build isolated visual review")]
    public static void Review()
    {
        Prepare();Directory.CreateDirectory("Builds/Review");
        var report=BuildPipeline.BuildPlayer(new BuildPlayerOptions {scenes=new[]{"Assets/Scenes/Battle.unity"},locationPathName="Builds/Review/RunePactReview.exe",target=BuildTarget.StandaloneWindows64,options=BuildOptions.None,extraScriptingDefines=new[]{"RUNEPACT_REVIEW"}});
        if(report.summary.result!=BuildResult.Succeeded)throw new Exception("Review build failed: "+report.summary.result);
        Debug.Log("RUNEPACT_REVIEW_BUILD_SUCCESS");
    }
    [MenuItem("RunePact/Test combat rules")]
    public static void Test()
    {
        int count=0;Action<bool,string> check=(ok,label)=>{if(!ok)throw new Exception("FAIL: "+label);count++;};
        var b=new Battle(1);check(b.Hand.Count==6&&b.Deck.Count==12,"18-card deck / 6-card hand");
        check(b.Fighters.Count==6&&b.Intents.Count==3,"3v3 and intents");
        check(b.ChangeGear(0),"pre-battle gear swap");check(b.Describe(new Card(0,2)).Effect==Effect.Burn,"equipment changes skill");
        check(!b.Play(0,0,out _),"attack cannot target ally");check(b.Energy==4&&b.Hand.Count==6,"invalid action is atomic");
        check(b.Play(0,3,out _),"play basic attack");check(b.Fighters[3].Hp==127&&b.Energy==3,"attack damage and energy");
        check(!b.ChangeGear(0),"gear locked after action");
        var guard=new Fighter(0,"test",100,false){Shield=20};Battle.Damage(guard,30,false);check(guard.Hp==90&&guard.Shield==0,"shield absorbs damage");
        guard.Shield=20;Battle.Damage(guard,10,true);check(guard.Hp==80&&guard.Shield==20,"pierce preserves shield");
        b.Fighters[0].Hp=130;b.Fighters[0].Burn=2;b.Resolve(Effect.Mend,50,0,false);check(b.Fighters[0].Hp==145&&b.Fighters[0].Burn==0,"heal cap and cleanse");
        var c=new Battle(2);for(int owner=0;owner<3;owner++){int idx=c.Hand.FindIndex(x=>x.Owner==owner&&x.Slot==0);check(c.Play(idx,3,out _),"harmony contributor "+owner);}check(c.Energy==2&&c.HarmonyUsed,"one harmony refund");
        var u=new Battle(2);int original=u.Describe(new Card(0,2)).Power;check(u.Upgrade(0,out _),"upgrade available");check(u.Describe(new Card(0,2)).Power>original&&u.Shards==2&&u.Energy==3,"upgrade modifies all card copies and costs");
        check(u.Upgrade(0,out _)&&!u.Upgrade(0,out _),"upgrade cap");
        var t=new Battle(3);t.Fighters[0].Shield=30;t.Fighters[0].Burn=2;t.BeginEnemy();check(t.Fighters[0].Shield==30&&!t.PlayerTurn,"shield lasts through enemy turn");t.BeginPlayer();check(t.Fighters[0].Shield==0&&t.Fighters[0].Burn==1&&t.Fighters[0].Hp==139,"start-turn shield reset and burn");
        check(t.Round==2&&t.Hand.Count==6&&t.Energy==4&&t.Shards==4,"new turn draw and resources");
        var dead=new Battle(3);dead.Fighters[1].Hp=0;dead.BeginEnemy();dead.BeginPlayer();check(dead.Hand.All(x=>x.Owner!=1),"fallen unit cards excluded");
        dead.Fighters.Where(x=>x.Enemy).ToList().ForEach(x=>x.Hp=0);dead.CheckOutcome();check(dead.Outcome==1,"victory");
        var lose=new Battle(5);lose.Fighters.Where(x=>!x.Enemy).ToList().ForEach(x=>x.Hp=0);lose.CheckOutcome();check(lose.Outcome==-1,"defeat");
        var combo=new Battle(7);
        combo.Resolve(Effect.Mark,10,3,false);int markedHp=combo.Fighters[3].Hp;
        combo.Resolve(Effect.Strike,20,3,false);
        check(combo.Fighters[3].Hp==markedHp-30&&combo.Fighters[3].Mark==0,"mark consumed by next attack");
        combo.Resolve(Effect.Channel,14,1,false);
        int spell=combo.Hand.FindIndex(x=>x.Owner==1&&x.Slot==0);
        int spellHp=combo.Fighters[4].Hp;
        check(combo.Play(spell,4,out _)&&combo.Fighters[4].Hp==spellHp-39&&combo.Fighters[1].Channel==0,"channel powers one attack");
        combo.Resolve(Effect.Fortify,28,0,false);combo.BeginEnemy();combo.BeginPlayer();
        check(combo.Fighters[0].Shield==28&&combo.Fighters[0].Fortified==0,"fortified retains shield once");
        combo.BeginEnemy();combo.BeginPlayer();check(combo.Fighters[0].Shield==0,"fortified expires");
        var journey=new Journey(9);
        check(!journey.ChooseReward(0),"reward unavailable before victory");
        for(int encounter=1;encounter<=2;encounter++){
            check(journey.Encounter==encounter,"journey encounter index");
            journey.Current.Fighters.Where(x=>x.Enemy).ToList().ForEach(x=>x.Hp=0);
            journey.Current.CheckOutcome();
            if(encounter<2){
                journey.Current.Fighters[0].Hp=0;
                journey.Current.Fighters[1].Tier=2;
                check(!journey.ChooseReward(3),"invalid reward rejected");
                check(journey.ChooseReward(1),"reward advances journey");
                check(journey.Current.Fighters[0].Hp==35&&journey.Current.Fighters[1].Tier==2,"checkpoint recovery and upgrade persistence");
                check(journey.Current.SkillBoost[(9+encounter)%3*3]==6,"individual card upgrade reward persists");
            }
        }
        check(journey.Complete&&!journey.ChooseReward(0),"journey ends after boss");
        var healer=new Battle(8);healer.Round=2;healer.Fighters[3].Hp=50;healer.Plan();healer.BeginEnemy();
        int healingIntent=healer.Intents.FindIndex(x=>x.Owner==4);
        check(healer.ExecuteIntent(healingIntent)&&healer.Fighters[3].Hp==74,"acolyte heals ally");
        var relicJourney=new Journey(11);relicJourney.Current.Fighters.Where(x=>x.Enemy).ToList().ForEach(x=>x.Hp=0);relicJourney.Current.CheckOutcome();
        var offers=relicJourney.GetOffers();check(offers.Length==3&&offers.Select(x=>x.Kind).Distinct().Count()==3,"reward offer categories");
        check(relicJourney.ChooseReward(2)&&relicJourney.Relics.Count==1,"relic reward persists");
        string snapshot=relicJourney.Save();Journey restored;check(Journey.TryRestore(snapshot,out restored)&&restored.Encounter==2&&restored.Relics.Count==1,"journey checkpoint restores");
        var grimoire=new Battle(13,null,1,EnemyFormation.Vanguard,new[]{Relic.LivingGrimoire});
        int mage=grimoire.Hand.FindIndex(x=>x.Owner==1);check(grimoire.GetCost(grimoire.Hand[mage])==0,"living grimoire discounts first mage card");
        var boss=new Battle(14,null,2,EnemyFormation.Regent);boss.Fighters[3].Hp=60;boss.Plan();check(boss.BossPhase==2&&boss.Fighters[3].Shield==12,"regent phase two awakens");
        var shieldwall=new Battle(15,null,1,EnemyFormation.Shieldwall);check(shieldwall.Fighters.Skip(3).Any(x=>x.Role==EnemyRole.Sentinel),"shieldwall formation");
        var pyre=new Battle(16,null,1,EnemyFormation.Pyre);check(pyre.Fighters.Skip(3).Count(x=>x.Role==EnemyRole.Pyromancer)==2,"pyre formation");
        var calm=new Battle(16,null,1,EnemyFormation.Vanguard,null,null,null,EncounterDifficulty.Calm);
        var fierce=new Battle(16,null,1,EnemyFormation.Vanguard,null,null,null,EncounterDifficulty.Fierce);
        check(calm.Fighters[3].MaxHp<b.Fighters[3].MaxHp&&fierce.Fighters[3].MaxHp>b.Fighters[3].MaxHp,"three encounter difficulty bands");
        var skills=new Battle(8);
        check(skills.Deck.Count(x=>x.Variant==CardVariant.Oath||x.Variant==CardVariant.Purify||x.Variant==CardVariant.Execution)==3,"one unique card per warrior");
        skills.Resolve(Effect.Oath,21,0,false);check(skills.Fighters[0].Shield==21&&skills.Fighters[0].Thorns==2,"Aura oath shields and retaliates");
        skills.Fighters[1].Burn=2;skills.Fighters[1].Weak=1;skills.Fighters[1].Hp=50;skills.Resolve(Effect.Purify,19,1,false);
        check(skills.Fighters[1].Hp==69&&skills.Fighters[1].Burn==0&&skills.Fighters[1].Weak==0&&skills.Fighters[1].Regen==2,"Lyra purifies and regenerates");
        skills.Fighters[3].Mark=1;int executionHp=skills.Fighters[3].Hp;skills.Resolve(Effect.Execute,30,3,false);
        check(skills.Fighters[3].Hp==executionHp-58,"Kael executes marked target");
        var standard=new Journey(22,null,false,EncounterDifficulty.Fierce);check(standard.TotalEncounters==3&&standard.Difficulty==EncounterDifficulty.Fierce,"standard route and difficulty");
        for(int stage=1;stage<=2;stage++){standard.Current.Fighters.Skip(3).ToList().ForEach(f=>f.Hp=0);standard.Current.CheckOutcome();check(standard.ChooseReward(1)&&standard.Encounter==stage+1,"standard checkpoint "+stage);}
        check(standard.Formation==EnemyFormation.Regent&&standard.Current.Fighters[3].MaxHp==220,"standard boss length");
        standard.ElapsedSeconds=75;check(Journey.TryRestore(standard.Save(),out var standardRestored)&&standardRestored.Save()==standard.Save(),"standard save roundtrip with timer");
        // Bots com seed jogam partidas completas para cobrir reembaralhamento, eliminação, novo alvo da IA e término.
        // Seeded bots play whole matches to cover reshuffling, elimination, AI retarget and termination.
        int wins=0, losses=0,draws=0;
        for(int seed=0;seed<80;seed++) {
            var sim=new Battle(seed,new[]{seed%2,(seed/2)%2,(seed/4)%2});int turns=0;
            while(sim.Outcome==0&&turns++<35) {
                int moves=0;
                while(sim.Outcome==0&&moves++<12) {
                    int index=sim.Hand.FindIndex(x=>sim.Fighters[x.Owner].Alive&&sim.GetCost(x)<=sim.Energy);
                    if(index<0)break;
                    var ability=sim.Describe(sim.Hand[index]);var target=sim.Fighters.Where(f=>f.Alive&&f.Enemy!=ability.Friendly).OrderBy(f=>ability.Friendly?(float)f.Hp/f.MaxHp:f.Hp).First();
                    check(sim.Play(index,target.Id,out _),"simulation legal action");
                }
                if(sim.Outcome!=0)break;sim.BeginEnemy();for(int i=0;i<sim.Intents.Count;i++)sim.ExecuteIntent(i);sim.BeginPlayer();
            }
            check(sim.Outcome!=0,"simulation terminates");check(sim.Fighters.All(x=>x.Hp>=0&&x.Hp<=x.MaxHp&&x.Shield>=0),"resource bounds");
            if(sim.Outcome==1)wins++;else if(sim.Outcome==-1)losses++;else draws++;
        }
        PrototypeRegression.Run(check);
        string result="PASS: "+count+" assertions; 80 seeded base battles + 45 varied journeys; base wins="+wins+" losses="+losses+" draws="+draws;
        File.WriteAllText("CombatTestResults.txt",result);Debug.Log("RUNEPACT_TESTS "+result);
        string balance=PrototypeBalance.Run();File.WriteAllText("BalanceTestResults.txt",balance);Debug.Log("RUNEPACT_BALANCE\n"+balance);
    }
}
