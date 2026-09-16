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
        foreach(var path in new[]{"Assets/Resources/Art/Warriors.png","Assets/Resources/Art/Arena.png"}) {
            var importer=(TextureImporter)AssetImporter.GetAtPath(path);
            importer.textureType=TextureImporterType.Default;importer.isReadable=path.Contains("Warriors");importer.filterMode=FilterMode.Point;importer.textureCompression=TextureImporterCompression.Uncompressed;importer.maxTextureSize=2048;importer.mipmapEnabled=false;importer.alphaSource=TextureImporterAlphaSource.FromInput;importer.SaveAndReimport();
        }
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
        // Bots com seed jogam partidas completas para cobrir reembaralhamento, eliminação, novo alvo da IA e término.
        // Seeded bots play whole matches to cover reshuffling, elimination, AI retarget and termination.
        int wins=0, losses=0,draws=0;
        for(int seed=0;seed<80;seed++) {
            var sim=new Battle(seed,new[]{seed%2,(seed/2)%2,(seed/4)%2});int turns=0;
            while(sim.Outcome==0&&turns++<35) {
                int moves=0;
                while(sim.Outcome==0&&moves++<12) {
                    int index=sim.Hand.FindIndex(x=>sim.Fighters[x.Owner].Alive&&sim.Describe(x).Cost<=sim.Energy);
                    if(index<0)break;
                    var ability=sim.Describe(sim.Hand[index]);var target=sim.Fighters.Where(f=>f.Alive&&f.Enemy!=ability.Friendly).OrderBy(f=>ability.Friendly?(float)f.Hp/f.MaxHp:f.Hp).First();
                    check(sim.Play(index,target.Id,out _),"simulation legal action");
                }
                if(sim.Outcome!=0)break;sim.BeginEnemy();for(int i=0;i<sim.Intents.Count;i++)sim.ExecuteIntent(i);sim.BeginPlayer();
            }
            check(sim.Outcome!=0,"simulation terminates");check(sim.Fighters.All(x=>x.Hp>=0&&x.Hp<=x.MaxHp&&x.Shield>=0),"resource bounds");
            if(sim.Outcome==1)wins++;else if(sim.Outcome==-1)losses++;else draws++;
        }
        string result="PASS: "+count+" assertions; 80 seeded battles; wins="+wins+" losses="+losses+" draws="+draws;
        File.WriteAllText("CombatTestResults.txt",result);Debug.Log("RUNEPACT_TESTS "+result);
    }
}
