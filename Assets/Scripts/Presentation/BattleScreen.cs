using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using RunePact.Core;

namespace RunePact.Presentation
{
    public sealed partial class BattleScreen : MonoBehaviour
    {
        public Battle Match {get;private set;}
        Journey journey;
        bool rewardOpen, paused, firstLaunch=true;
        readonly Color ink=new Color(.13f,.17f,.18f,1), panel=new Color(.22f,.28f,.28f,1);
        readonly Color gold=new Color(.94f,.76f,.44f), cream=new Color(1f,.94f,.81f), muted=new Color(.74f,.80f,.73f);
        readonly Color teal=new Color(.47f,.84f,.69f), red=new Color(.96f,.57f,.47f);
        readonly Color parchment=new Color(.96f,.89f,.73f), writing=new Color(.23f,.21f,.20f), violet=new Color(.72f,.58f,.92f);
        Font font; RectTransform root, handRoot, overlay, fxRoot;
        Text roundLabel, energyLabel, hint, deckLabel, pactLabel, shardLabel, logLabel, turnLabel;
        Button endButton; Text endText;
        readonly RectTransform[] units=new RectTransform[6];
        readonly Image[] portraits=new Image[6],hpBars=new Image[6], outlines=new Image[6];
        readonly Text[] hpTexts=new Text[6],statuses=new Text[6],intentTexts=new Text[6];
        readonly Vector2[] positions={new Vector2(475,285),new Vector2(45,235),new Vector2(245,325),new Vector2(905,285),new Vector2(1335,235),new Vector2(1135,325)};
        readonly Sprite[] sprites=new Sprite[6];
        int selected=-1,seed=7241; bool busy, mutedAudio;
        float noticeUntil; string notice="";
        AudioSource sound; AudioClip attackSound,guardSound,clickSound;
        public static BattleScreen Active;
        void Awake()
        {
            Active=this; Application.targetFrameRate=60;
            font=Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            FantasySkin.Initialize();LoadSprites(); BuildScreen();BuildEnhancements();if(TryReview())return;ResetBattle();ShowStartMenu();
        }
        void LoadSprites()
        {
            Texture2D source=Resources.Load<Texture2D>("Art/Warriors");
            var tex=new Texture2D(source.width,source.height,TextureFormat.RGBA32,false);
            var pixels=source.GetPixels32();
            // Importação de chroma-key em tempo de execução sem alteração; o atlas-fonte gerado permanece intacto.
            // Non-destructive runtime chroma-key import; generated source atlas remains intact.
            for(int i=0;i<pixels.Length;i++) {var c=pixels[i];if(c.r>160&&c.b>145&&c.g<115&&Math.Min(c.r,c.b)-c.g>85)c.a=0;pixels[i]=c;}
            tex.SetPixels32(pixels);tex.Apply();tex.filterMode=FilterMode.Point;tex.wrapMode=TextureWrapMode.Clamp;
            int w=tex.width/3,h=tex.height/2;
            for(int i=0;i<6;i++)sprites[i]=TrimmedSprite(tex,i%3*w,(1-i/3)*h,w,h);
            LoadBoss();
        }
        RectTransform Box(Transform parent,string name,float x,float y,float w,float h,Color? fill=null)
        {
            var g=new GameObject(name,typeof(RectTransform));g.transform.SetParent(parent,false);
            var r=(RectTransform)g.transform;r.anchorMin=r.anchorMax=new Vector2(0,1);r.pivot=new Vector2(0,1);r.anchoredPosition=new Vector2(x,-y);r.sizeDelta=new Vector2(w,h);
            if(fill.HasValue){var im=g.AddComponent<Image>();im.color=fill.Value;im.raycastTarget=false;}return r;
        }
        Text Label(Transform p,string value,float x,float y,float w,float h,int size,Color color,TextAnchor align=TextAnchor.MiddleLeft,bool bold=false)
        {
            var r=Box(p,"Text",x,y,w,h);var t=r.gameObject.AddComponent<Text>();t.font=font;t.fontSize=size;t.color=color;t.text=value;t.alignment=align;t.fontStyle=bold?FontStyle.Bold:FontStyle.Normal;t.raycastTarget=false;t.horizontalOverflow=HorizontalWrapMode.Wrap;t.verticalOverflow=VerticalWrapMode.Truncate;return t;
        }
        Image Picture(Transform p,Sprite sprite,float x,float y,float w,float h)
        {var r=Box(p,"Sprite",x,y,w,h);var im=r.gameObject.AddComponent<Image>();im.sprite=sprite;im.preserveAspect=true;im.raycastTarget=false;return im;}
        Button ButtonAt(Transform p,string label,float x,float y,float w,float h,Action action,Color? bg=null,int size=19)
        {
            var r=Surface(p,label,x,y,w,h,bg??panel);var image=r.GetComponent<Image>();image.raycastTarget=true;
            var b=r.gameObject.AddComponent<Button>();b.targetGraphic=image;var c=b.colors;c.normalColor=Color.white;c.highlightedColor=new Color(1.2f,1.2f,1.15f);c.pressedColor=new Color(.7f,.8f,.8f);c.disabledColor=new Color(.45f,.45f,.45f);b.colors=c;
            var nav=b.navigation;nav.mode=Navigation.Mode.None;b.navigation=nav;
            r.gameObject.AddComponent<FriendlyMotion>();
            b.onClick.AddListener(()=>{Tone(clickSound);action();});Label(r,label,6,0,w-12,h-3,size,cream,TextAnchor.MiddleCenter,true);return b;
        }
        void Border(RectTransform r,Color c,int thickness=2)
        {var o=r.gameObject.AddComponent<Outline>();o.effectColor=c;o.effectDistance=new Vector2(thickness,-thickness);o.useGraphicAlpha=false;}
        void BuildScreen()
        {
            var camera=new GameObject("Camera",typeof(Camera),typeof(AudioListener));camera.GetComponent<Camera>().clearFlags=CameraClearFlags.SolidColor;camera.GetComponent<Camera>().backgroundColor=ink;
            var cg=new GameObject("Battle Canvas",typeof(Canvas),typeof(CanvasScaler),typeof(GraphicRaycaster));var canvas=cg.GetComponent<Canvas>();canvas.renderMode=RenderMode.ScreenSpaceOverlay;
            var scaler=cg.GetComponent<CanvasScaler>();scaler.uiScaleMode=CanvasScaler.ScaleMode.ScaleWithScreenSize;scaler.referenceResolution=new Vector2(1600,900);scaler.screenMatchMode=CanvasScaler.ScreenMatchMode.Expand;
            var outer=cg.transform as RectTransform;
            root=Box(outer,"Landscape safe gameplay",0,0,1600,900,ink);root.anchorMin=root.anchorMax=new Vector2(.5f,.5f);root.pivot=new Vector2(.5f,.5f);root.anchoredPosition=Vector2.zero;
            var bg=Box(root,"Sanctuary",0,0,1600,700);var raw=bg.gameObject.AddComponent<RawImage>();raw.texture=Resources.Load<Texture2D>("Art/Arena");raw.uvRect=new Rect(0,.10f,1,.87f);raw.raycastTarget=false;
            Box(root,"Atmosphere",0,0,1600,640,new Color(.17f,.22f,.20f,.10f));
            var brand=Surface(root,"Adventure title",20,14,360,72,ink);
            Label(brand,"R U N E P A C T",22,7,325,34,27,gold,TextAnchor.MiddleLeft,true);
            Label(brand,"Santuário do Crepúsculo",23,42,320,22,15,muted);
            var progress=Surface(root,"Journey",530,14,335,72,ink);
            roundLabel=Label(progress,"",12,9,311,27,21,cream,TextAnchor.MiddleCenter,true);
            turnLabel=Label(progress,"",12,39,311,22,14,teal,TextAnchor.MiddleCenter);
            var resources=Surface(root,"Shards",886,26,170,49,panel);
            shardLabel=Label(resources,"",8,0,154,47,17,gold,TextAnchor.MiddleCenter,true);
            ButtonAt(root,"Arsenal",1074,26,151,49,ShowGear,panel,18);
            ButtonAt(root,"?",1240,26,48,49,ShowHelp,panel,22);
            ButtonAt(root,"Pausa",1302,26,87,49,ShowPause,panel,15);
            ButtonAt(root,"Som",1404,26,75,49,()=>{mutedAudio=!mutedAudio;Notify(mutedAudio?"Som desativado.":"Som ativado.");},panel,15);
            ButtonAt(root,"Reiniciar",1488,26,90,49,ConfirmReset,panel,14);
            Pill(root,"Seu pacto",28,106,147,31,teal);
            Pill(root,"Os exilados",1402,106,170,31,red);
            foreach(int id in new[]{1,4,0,3,2,5})BuildUnit(id);
            fxRoot=Box(root,"Effects",0,0,1600,900);
            Surface(root,"Hand tray",16,640,1568,247,ink);
            var hintPlate=Surface(root,"Guidance",310,593,980,39,new Color(.18f,.23f,.23f));
            hint=Label(hintPlate,"",12,0,956,37,16,cream,TextAnchor.MiddleCenter);
            Orb(root,FantasySkin.Circle,59,660,104,104,new Color(.18f,.29f,.33f));
            Orb(root,FantasySkin.Ring,61,662,100,100,gold);
            energyLabel=Label(root,"",64,675,94,50,34,cream,TextAnchor.MiddleCenter,true);
            Label(root,"ENERGIA",66,723,91,19,12,gold,TextAnchor.MiddleCenter,true);
            pactLabel=Label(root,"",28,770,169,46,14,teal,TextAnchor.MiddleCenter);
            deckLabel=Label(root,"",28,827,169,40,13,muted,TextAnchor.MiddleCenter);
            handRoot=Box(root,"Cards",220,653,1140,222);
            endButton=ButtonAt(root,"Encerrar\nturno",1390,673,172,84,()=>EndTurn(),new Color(.28f,.47f,.39f),22);endText=endButton.GetComponentInChildren<Text>();
            Label(root,"ESPAÇO ou toque",1390,772,172,20,12,muted,TextAnchor.MiddleCenter);
            logLabel=Label(root,"",1390,803,172,67,12,muted,TextAnchor.UpperLeft);
            Label(root,"1–6  escolher carta    •    toque no alvo    •    Esc  cancelar",370,880,860,18,11,muted,TextAnchor.MiddleCenter);
            var ev=new GameObject("Input",typeof(EventSystem),typeof(StandaloneInputModule));
            ev.GetComponent<EventSystem>().sendNavigationEvents=false;
            sound=gameObject.AddComponent<AudioSource>();attackSound=Synth(160,.12f);guardSound=Synth(480,.18f);clickSound=Synth(720,.035f);
        }
        void BuildUnit(int id)
        {
            var r=Box(root,"Fighter "+id,positions[id].x,positions[id].y,220,210);units[id]=r;
            Orb(r,FantasySkin.Circle,30,152,166,27,new Color(.08f,.13f,.12f,.38f));
            Orb(r,FantasySkin.Circle,63,157,96,13,new Color(.04f,.07f,.06f,.48f));
            portraits[id]=Picture(r,sprites[id],0,0,204,188);
            portraits[id].rectTransform.pivot=new Vector2(.5f,0);
            portraits[id].rectTransform.anchoredPosition=new Vector2(110,-166);
            portraits[id].rectTransform.localScale=new Vector3(id<3?1:-1,1,1);
            BuildStatusVisuals(r,id);
            outlines[id]=Orb(r,FantasySkin.Ring,20,142,185,46,Color.clear);
            var hit=Box(r,"Select target",0,-25,220,222,Color.clear);hit.GetComponent<Image>().raycastTarget=true;
            var btn=hit.gameObject.AddComponent<Button>();btn.targetGraphic=hit.GetComponent<Image>();int n=id;btn.onClick.AddListener(()=>ClickUnit(n));
            var plate=Surface(r,"Vitals",8,179,204,46,ink);
            hpTexts[id]=Label(plate,"",10,2,184,22,13,cream,TextAnchor.MiddleCenter,true);
            hpTexts[id].resizeTextForBestFit=true;hpTexts[id].resizeTextMinSize=10;hpTexts[id].resizeTextMaxSize=13;
            Round(plate,"Empty health",10,29,184,9,new Color(.08f,.12f,.12f));hpBars[id]=Round(plate,"Health",10,29,184,9,id<3?teal:red).GetComponent<Image>();
            statuses[id]=Label(r,"",-12,230,244,37,12,gold,TextAnchor.UpperCenter,true);
            var intentPlate=Surface(r,"Intention",-16,-44,252,39,id<3?new Color(.23f,.32f,.29f):new Color(.34f,.25f,.25f));
            intentTexts[id]=Label(intentPlate,"",8,2,236,33,13,cream,TextAnchor.MiddleCenter,true);
        }
        void ResetBattle()
        {
            StopAllCoroutines();busy=false;selected=-1;ClearOverlay();
            foreach(Transform effect in fxRoot)Destroy(effect.gameObject);
            rewardOpen=false;paused=false;Time.timeScale=animationSpeed;
            Journey restored;
            if(firstLaunch&&PlayerPrefs.HasKey("RunePactJourneyV4")&&Journey.TryRestore(PlayerPrefs.GetString("RunePactJourneyV4"),out restored)){journey=restored;notice="Jornada retomada do último ponto salvo.";}
            else {int[] load=Match==null?null:Match.Fighters.Take(3).Select(f=>f.Gear).ToArray();journey=new Journey(seed++,load);}
            firstLaunch=false;Match=journey.Current;
            for(int i=0;i<6;i++)units[i].anchoredPosition=new Vector2(positions[i].x,-positions[i].y);
            SaveJourney();Refresh();
        }
        void Refresh()
        {
            roundLabel.text=(journey.Encounter==2?"CHEFE":"ENCONTRO 1 / 1")+"  •  Rodada "+Match.Round;turnLabel.text=Match.Outcome!=0?"BATALHA CONCLUÍDA":Match.Formation==EnemyFormation.Regent&&Match.BossPhase==2?"REGENTE • FASE 2":busy?(Match.PlayerTurn?"CONJURANDO…":"TURNO DOS EXILADOS"):"SEU TURNO";turnLabel.color=Match.Formation==EnemyFormation.Regent&&Match.BossPhase==2?gold:!Match.PlayerTurn?red:teal;
            energyLabel.text=Match.Energy+" / 4";shardLabel.text="◆  "+Match.Shards+" fragmentos";
            pactLabel.text="PACTO  "+Match.Harmony+" / 3\n"+(Match.HarmonyUsed?"+1 energia ativada":"3 guerreiros = +1 energia");
            deckLabel.text="BARALHO  "+Match.Deck.Count+"\nDESCARTE  "+Match.Discard.Count;
            endButton.interactable=!busy&&Match.Outcome==0;endText.text=busy?"Aguarde…":"Encerrar\nturno";
            logLabel.text=string.Join("\n",Match.Log.Skip(Math.Max(0,Match.Log.Count-2)));
            for(int i=0;i<6;i++) {
                var f=Match.Fighters[i];hpTexts[i].text=f.Name+"   "+f.Hp+" / "+f.MaxHp;
                hpBars[i].rectTransform.sizeDelta=new Vector2(184f*f.Hp/f.MaxHp,9);
                portraits[i].color=f.Alive?Color.white:new Color(.3f,.35f,.4f,.35f);
                statuses[i].text=!f.Alive?"DERROTADO":(f.Shield>0?"ESCUDO "+f.Shield+"  ":"")+(f.Burn>0?"QUEIMA "+f.Burn+"t":"");
                if(f.Alive)statuses[i].text+=(f.Mark>0?" • MARCA":"")+(f.Fortified>0?" • FIRME":"")+(f.Channel>0?" • CANAL +"+f.Channel:"");
                if(f.Alive)statuses[i].text+=(f.Weak>0?" • FRAQUEZA":"")+(f.Vulnerable>0?" • VULNERÁVEL":"")+(f.Regen>0?" • REGEN "+f.Regen:"")+(f.Thorns>0?" • ESPINHOS":"")+(f.Stun>0?" • ATORDOADO":"");
                statuses[i].fontSize=11;
                bool valid=selected>=0&&selected<Match.Hand.Count&&Match.CanTarget(Match.Hand[selected],i);
                outlines[i].color=valid?gold:Color.clear;
                var intent=Match.Intents.FirstOrDefault(x=>x.Owner==i);
                intentTexts[i].text=!f.Alive?"":i<3?Match.GearName(i)+"  I"+new string('I',f.Tier):intent==null?"":intent.Label+" "+intent.Power+" → "+(intent.Effect==Effect.Volley?"todos":Match.Fighters[intent.Target].Name);
            }
            RefreshEnhancements();RebuildHand();UpdateHint();
        }
        void RebuildHand()
        {
            foreach(Transform child in handRoot){child.gameObject.SetActive(false);Destroy(child.gameObject);}
            for(int i=0;i<Match.Hand.Count;i++) {
                int n=i;var c=Match.Hand[i];var a=Match.Describe(c);var f=Match.Fighters[c.Owner];
                int cost=Match.GetCost(c);bool can=!busy&&f.Alive&&Match.Energy>=cost&&Match.Outcome==0;
                Color tint=c.Owner==0?new Color(.26f,.47f,.43f):c.Owner==1?new Color(.45f,.35f,.54f):new Color(.57f,.38f,.25f);
                var r=Surface(handRoot,"Card "+i,i*190,selected==i?-18:0,180,220,selected==i?new Color(1f,.95f,.78f):parchment);
                if(selected==i)Border(r,gold,3);
                Round(r,"Illustration",6,6,168,76,tint);
                Orb(r,FantasySkin.Ring,61,11,63,63,new Color(1,1,1,.13f));
                Picture(r,sprites[c.Owner],51,-1,85,85);
                Orb(r,FantasySkin.Circle,8,8,39,39,gold);
                Orb(r,FantasySkin.Circle,11,10,33,33,tint);
                Label(r,cost.ToString(),12,11,30,29,22,cream,TextAnchor.MiddleCenter,true);
                Label(r,(i+1).ToString(),150,12,20,20,12,cream,TextAnchor.MiddleCenter);
                var name=Label(r,a.Name,7,86,166,25,18,writing,TextAnchor.MiddleCenter,true);
                name.resizeTextForBestFit=true;name.resizeTextMinSize=14;name.resizeTextMaxSize=18;
                Label(r,c.Variant!=CardVariant.Standard?(c.Variant==CardVariant.Tremor?"JORNADA • ÉPICA":"JORNADA • RARA"):a.Source+(c.Slot==2?" • N"+(f.Tier+1):""),8,111,164,19,11,tint,TextAnchor.MiddleCenter);
                Label(r,a.Description,12,136,156,59,13,writing,TextAnchor.UpperLeft);
                Round(r,"Role ribbon",7,197,166,17,tint);
                Label(r,f.Name+" • "+(a.Friendly?"SUPORTE":"ATAQUE"),7,196,166,18,10,cream,TextAnchor.MiddleCenter,true);
                var img=r.GetComponent<Image>();img.raycastTarget=true;var b=r.gameObject.AddComponent<Button>();b.targetGraphic=img;b.onClick.AddListener(()=>SelectCard(n));
                b.interactable=can;r.gameObject.AddComponent<FriendlyMotion>().Lift=9;
                if(!can)Round(r,"Unavailable",0,0,180,220,new Color(.20f,.22f,.22f,.42f));
            }
        }
        public void SelectCard(int index)
        {
            if(busy||Match.Outcome!=0||index<0||index>=Match.Hand.Count||overlay!=null)return;
            var c=Match.Hand[index];if(!Match.Fighters[c.Owner].Alive){Notify("Este guerreiro foi derrotado.");return;}
            if(Match.Energy<Match.GetCost(c)){Notify("Energia insuficiente. Encerre o turno ou escolha outra carta.");return;}
            selected=selected==index?-1:index;notice="";Tone(clickSound);Refresh();
        }
        public void ClickUnit(int id)
        {
            if(busy||overlay!=null||Match.Outcome!=0)return;
            if(selected<0){ShowFighterDetails(id);return;}
            var card=Match.Hand[selected];var a=Match.Describe(card);int owner=card.Owner;
            var before=Match.Fighters.Select(f=>f.Hp).ToArray();
            var shields=Match.Fighters.Select(f=>f.Shield).ToArray();
            if(!Match.Play(selected,id,out var error)){Notify(error);return;}
            SaveJourney();
            selected=-1;StartCoroutine(PlayerAction(owner,id,a,before,shields));
        }
        IEnumerator PlayerAction(int owner,int target,Ability ability,int[] before,int[] shields)
        {
            busy=true;RebuildHand();endButton.interactable=false;
            for(int i=0;i<6;i++)outlines[i].color=Color.clear;
            Tone(ability.Friendly?guardSound:attackSound);
            yield return ActionAnimation(owner,target,ability.Effect,before,shields);
            busy=false;Refresh();
            if(Match.Outcome!=0)yield return ResultAfter();
        }
        IEnumerator ActionAnimation(int owner,int target,Effect effect,int[] before,int[] shields)
        {
            yield return CastTravel(owner,target,effect);
            Color color=EffectColor(effect,owner);
            Refresh();
            for(int i=0;i<6;i++){
                var f=Match.Fighters[i];
                bool affected=i==target||before[i]!=f.Hp||shields[i]!=f.Shield;
                if(!affected)continue;
                StartCoroutine(Impact(i,effect,color));
                int delta=f.Hp-before[i],shieldDelta=f.Shield-shields[i];
                if(delta!=0)StartCoroutine(FloatText(i,delta.ToString("+0;-0"),delta>0?teal:red));
                else if(shieldDelta!=0)StartCoroutine(FloatText(i,(shieldDelta>0?"+":"")+shieldDelta+" escudo",color));
                else if(i==target&&effect==Effect.Channel)StartCoroutine(FloatText(i,"Canalizado",violet));
                else if(i==target&&effect==Effect.Mark)StartCoroutine(FloatText(i,"Marcado",gold));
            }
            yield return new WaitForSeconds(.68f);
        }
        IEnumerator FloatText(int target,string value,Color color)
        {
            var p=positions[target];var label=Label(fxRoot,value,p.x-10,p.y+12,240,45,27,color,TextAnchor.MiddleCenter,true);float t=0;
            var shadow=label.gameObject.AddComponent<Shadow>();shadow.effectColor=new Color(.05f,.08f,.09f,.95f);shadow.effectDistance=new Vector2(2,-2);
            while(t<.95f){t+=Time.deltaTime;label.rectTransform.anchoredPosition=new Vector2(p.x-10,-p.y-12+t*55);label.color=new Color(color.r,color.g,color.b,Mathf.Min(1,(.95f-t)*3));yield return null;}Destroy(label.gameObject);
        }
        public void EndTurn(){if(!busy&&Match.Outcome==0&&overlay==null)StartCoroutine(EnemyTurn());}
        IEnumerator EnemyTurn()
        {
            busy=true;selected=-1;Match.BeginEnemy();Refresh();yield return TurnBanner("Vez dos exilados",red);
            for(int i=0;i<Match.Intents.Count&&Match.Outcome==0;i++) {
                var intent=Match.Intents[i];var before=Match.Fighters.Select(f=>f.Hp).ToArray();var shields=Match.Fighters.Select(f=>f.Shield).ToArray();
                int target=intent.Target;
                if(!Match.Fighters[target].Alive)target=Match.Fighters.FindIndex(f=>f.Alive&&f.Enemy==FriendlyEffect(intent.Effect));
                if(Match.ExecuteIntent(i)){Tone(FriendlyEffect(intent.Effect)?guardSound:attackSound);yield return ActionAnimation(intent.Owner,target,intent.Effect,before,shields);yield return new WaitForSeconds(.15f);}
            }
            Match.BeginPlayer();SaveJourney();Refresh();if(Match.Outcome==0)yield return TurnBanner("Sua vez",teal);
            busy=false;Refresh();if(Match.Outcome!=0)yield return ResultAfter();
        }
        void Notify(string value){notice=value;noticeUntil=Time.unscaledTime+4;UpdateHint();}
        void UpdateHint()
        {
            if(notice!=""&&Time.unscaledTime<noticeUntil){hint.text=notice;return;}
            hint.text=Match.Outcome!=0?"Batalha concluída.":busy?(Match.PlayerTurn?"Seu guerreiro está agindo…":"Os exilados estão agindo…"):selected>=0&&selected<Match.Hand.Count?Match.Describe(Match.Hand[selected]).Name+"  →  toque em um "+(Match.Describe(Match.Hand[selected]).Friendly?"ALIADO":"INIMIGO")+" destacado":"Escolha uma carta e seu alvo. Os três guerreiros juntos ativam o Pacto!";
        }
        RectTransform Modal(string title,int height=510)
        {
            ClearOverlay();overlay=Box(root,"Overlay",0,0,1600,900,new Color(.01f,.025f,.04f,.88f));overlay.GetComponent<Image>().raycastTarget=true;
            var p=Surface(overlay,"Panel",235,(900-height)/2,1130,height,ink);Border(p,gold,2);
            Label(p,title,30,20,970,44,29,gold,TextAnchor.MiddleLeft,true);
            ButtonAt(p,"×",1050,22,49,43,ClearOverlay,panel,26);return p;
        }
        void ClearOverlay(){if(overlay!=null){overlay.gameObject.SetActive(false);Destroy(overlay.gameObject);}overlay=null;if(paused){paused=false;Time.timeScale=animationSpeed;}}
        void ShowPause()
        {
            if(busy||rewardOpen)return;
            var p=Modal("JORNADA EM PAUSA",360);
            paused=true;Time.timeScale=0;SaveJourney();
            Label(p,"Batalha salva: vida, energia, cartas, efeitos e ordem do baralho.\nO jogo retoma exatamente a última ação concluída.",35,88,1060,65,21,cream,TextAnchor.MiddleCenter);
            ButtonAt(p,"VELOCIDADE  "+animationSpeed+"×",45,180,500,56,()=>{ToggleSpeed();ShowPause();},panel,21);
            ButtonAt(p,"SOM  "+(mutedAudio?"DESLIGADO":"LIGADO"),570,180,515,56,()=>{mutedAudio=!mutedAudio;PlayerPrefs.SetInt("RunePactMuted",mutedAudio?1:0);ShowPause();},panel,21);
            ButtonAt(p,"CONTINUAR",300,265,530,62,ClearOverlay,new Color(.28f,.47f,.39f),23);
        }
        void ShowGear()
        {
            if(busy||rewardOpen)return;var p=Modal("ARSENAL DO PACTO",560);
            Label(p,"Troque o equipamento antes da primeira ação. Forje durante seu turno: 1 energia + fragmentos.",30,73,1060,35,18,muted);
            for(int i=0;i<3;i++) {
                int n=i;var f=Match.Fighters[i];var col=Surface(p,"Equipment",30+i*365,124,340,337,panel);
                Picture(col,sprites[i],8,9,91,98);Label(col,f.Name,115,19,210,30,22,cream,TextAnchor.MiddleLeft,true);
                Label(col,"NÍVEL "+(f.Tier+1)+" / 3",115,57,210,24,14,teal);
                Label(col,Match.GearName(i),15,114,310,31,21,gold,TextAnchor.MiddleCenter,true);
                var a=Match.Describe(new Card(i,2));Label(col,a.Name+"\n"+a.Description,20,151,300,80,17,cream,TextAnchor.UpperLeft);
                var swap=ButtonAt(col,"TROCAR EQUIPAMENTO",15,238,310,38,()=>{if(Match.ChangeGear(n)){SaveJourney();Refresh();ShowGear();}},ink,15);swap.interactable=Match.Round==1&&Match.TotalPlayed==0&&Match.Outcome==0;
                var forge=ButtonAt(col,f.Tier>=2?"NÍVEL MÁXIMO":"FORJAR  •  1 energia + "+(f.Tier+1)+" ◆",15,285,310,38,()=>{if(Match.Upgrade(n,out var error)){SaveJourney();Refresh();ShowGear();Tone(guardSound);}else {ClearOverlay();Notify(error);}},new Color(.16f,.32f,.30f),15);forge.interactable=f.Alive&&f.Tier<2&&Match.Outcome==0&&Match.Energy>=1&&Match.Shards>=f.Tier+1;
            }
            Label(p,"As cartas base permanecem. O equipamento altera a habilidade especial em todas as cópias do baralho.",30,480,1060,43,17,muted,TextAnchor.MiddleCenter);
        }
        void ShowHelp()
        {
            if(busy||rewardOpen)return;
            var p=Modal("COMO JOGAR",530);
            Label(p,"1   Selecione uma das seis cartas e toque no alvo destacado.\n2   Você recebe 4 de energia e compra até 6 cartas por turno.\n3   Escudos duram até o próximo turno de quem os recebeu.\n4   Use cartas dos 3 aliados no turno: o Pacto concede +1 energia.\n5   Veja a próxima ação acima de cada inimigo e prepare sua defesa.\n6   EQUIPAMENTOS: troque habilidades antes da primeira ação ou use a forja.\n7   Derrote os 3 exilados. Cartas de aliados caídos saem do ciclo de compra.",40,88,1050,296,21,cream);
            Label(p,"Marca: próximo ataque +10. Canalização: próximo ataque recebe o bônus indicado.\nFortificado: preserva escudo uma vez. Queima: 6/turno. Perfurante ignora escudo.\n1 encontro → recompensa → chefe (120 de vida). Toque em um personagem para ver seus estados.",40,400,1050,100,17,gold);
        }
        void ConfirmReset()
        {if(busy||rewardOpen)return;var p=Modal("REINICIAR A JORNADA?",250);Label(p,"A jornada volta ao primeiro encontro. Seus equipamentos escolhidos serão mantidos.",35,80,1050,50,21,cream);ButtonAt(p,"REINICIAR",35,157,500,55,ResetBattle);ButtonAt(p,"CONTINUAR",560,157,535,55,ClearOverlay);}
        IEnumerator ResultAfter()
        {
            yield return new WaitForSeconds(.65f);
            if(journey.AwaitingReward){ShowRewards();yield break;}
            var p=Modal(journey.Complete?"JORNADA CONCLUÍDA":Match.Outcome==2?"EMPATE NO SANTUÁRIO":"O PACTO CAIU",350);
            Label(p,"Rodadas: "+Match.Round+"  •  Cartas usadas: "+Match.CardsPlayed+"  •  Relíquias: "+journey.Relics.Count+"\nExperimente outras relíquias, cartas e equipamentos.",35,92,1060,100,23,cream,TextAnchor.MiddleCenter);
            ButtonAt(p,"JOGAR NOVAMENTE",290,230,550,66,ResetBattle,new Color(.16f,.35f,.35f),24);
        }
        void ShowRewards()
        {
            var p=Modal("VITÓRIA • ESCOLHA UMA RECOMPENSA",690);
            rewardOpen=true;
            foreach(var button in p.GetComponentsInChildren<Button>())button.gameObject.SetActive(false);
            var closeShadow=p.Find("× shadow");if(closeShadow!=null)closeShadow.gameObject.SetActive(false);
            Label(p,"A SEGUIR: REGENTE DE ÂMBAR • 120 DE VIDA\nTodos recuperam 35 de vida. Escolha uma carta, um aprimoramento ou uma relíquia.",35,76,1060,65,19,cream,TextAnchor.MiddleCenter);
            var offers=journey.GetOffers();
            for(int i=0;i<3;i++){
                int choice=i;
                var card=BuildRewardCard(p,offers[i],35+i*365,154);
                ButtonAt(card,"ESCOLHER",20,399,290,48,()=>{
                    if(!journey.ChooseReward(choice))return;
                    rewardOpen=false;ClearOverlay();Match=journey.Current;selected=-1;notice="";SaveJourney();Refresh();
                },new Color(.28f,.47f,.39f),15);
            }
        }
        void Update()
        {
            if(Match==null)return;UpdateHint();AnimateCharacters();FitSafeArea();if(menuOpen)return;
            if(Input.GetKeyDown(KeyCode.Escape)&&!rewardOpen){if(overlay!=null)ClearOverlay();else{selected=-1;Refresh();}}
            if(Input.GetKeyDown(KeyCode.P)&&!rewardOpen){if(paused)ClearOverlay();else ShowPause();}
            if(overlay!=null)return;
            for(int i=0;i<6;i++)if(Input.GetKeyDown(KeyCode.Alpha1+i))SelectCard(i);
            if(Input.GetKeyDown(KeyCode.Space))EndTurn();
        }
        void SaveJourney(){if(!reviewMode&&journey!=null&&(Match==null||Match.PlayerTurn||Match.Outcome!=0)){PlayerPrefs.SetString("RunePactJourneyV4",journey.Save());PlayerPrefs.Save();}}
        AudioClip Synth(float frequency,float duration)
        {
            int count=(int)(22050*duration);var data=new float[count];for(int i=0;i<count;i++){float t=i/22050f;data[i]=Mathf.Sin(t*frequency*Mathf.PI*2)*(1f-i/(float)count)*.09f;}
            var clip=AudioClip.Create("Rune tone",count,1,22050,false);clip.SetData(data,0);return clip;
        }
        void Tone(AudioClip clip){if(!mutedAudio&&sound!=null&&clip!=null)sound.PlayOneShot(clip);}
    }
}
