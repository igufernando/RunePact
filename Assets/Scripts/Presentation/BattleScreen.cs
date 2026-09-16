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
    public sealed class BattleScreen : MonoBehaviour
    {
        public Battle Match {get;private set;}
        readonly Color ink=new Color(.035f,.055f,.075f,.97f), panel=new Color(.075f,.105f,.135f,.97f);
        readonly Color gold=new Color(.89f,.72f,.39f), cream=new Color(.95f,.92f,.82f), muted=new Color(.60f,.70f,.72f);
        readonly Color teal=new Color(.25f,.82f,.73f), red=new Color(.96f,.43f,.39f);
        Font font; RectTransform root, handRoot, overlay, fxRoot;
        Text roundLabel, energyLabel, hint, deckLabel, pactLabel, shardLabel, logLabel, turnLabel;
        Button endButton; Text endText;
        readonly RectTransform[] units=new RectTransform[6];
        readonly Image[] portraits=new Image[6],hpBars=new Image[6], outlines=new Image[6];
        readonly Text[] hpTexts=new Text[6],statuses=new Text[6],intentTexts=new Text[6];
        readonly Vector2[] positions={new Vector2(440,292),new Vector2(156,165),new Vector2(186,374),new Vector2(946,292),new Vector2(1240,165),new Vector2(1210,374)};
        readonly Sprite[] sprites=new Sprite[6];
        int selected=-1,seed=7241; bool busy, mutedAudio;
        float noticeUntil; string notice="";
        AudioSource sound; AudioClip attackSound,guardSound,clickSound;
        public static BattleScreen Active;
        void Awake()
        {
            Active=this; Application.targetFrameRate=60;
            font=Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            LoadSprites(); BuildScreen(); ResetBattle();
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
            for(int i=0;i<6;i++)sprites[i]=Sprite.Create(tex,new Rect(i%3*w,(1-i/3)*h,w,h),new Vector2(.5f,.5f),100,0,SpriteMeshType.FullRect);
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
            var r=Box(p,label,x,y,w,h,bg??panel);var image=r.GetComponent<Image>();image.raycastTarget=true;
            var b=r.gameObject.AddComponent<Button>();b.targetGraphic=image;var c=b.colors;c.normalColor=Color.white;c.highlightedColor=new Color(1.2f,1.2f,1.15f);c.pressedColor=new Color(.7f,.8f,.8f);c.disabledColor=new Color(.45f,.45f,.45f);b.colors=c;
            var nav=b.navigation;nav.mode=Navigation.Mode.None;b.navigation=nav;
            b.onClick.AddListener(()=>{Tone(clickSound);action();});Label(r,label,6,0,w-12,h,size,cream,TextAnchor.MiddleCenter,true);return b;
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
            var bg=Box(root,"Sanctuary",0,70,1600,550);var raw=bg.gameObject.AddComponent<RawImage>();raw.texture=Resources.Load<Texture2D>("Art/Arena");raw.uvRect=new Rect(0,.12f,1,.86f);raw.raycastTarget=false;
            Box(root,"Upper shade",0,75,1600,64,new Color(.025f,.045f,.07f,.6f));
            Box(root,"Header",0,0,1600,76,ink);Box(root,"Gold edge",0,74,1600,2,gold*.7f);
            Label(root,"R U N E P A C T",28,8,350,37,27,gold,TextAnchor.MiddleLeft,true);
            Label(root,"SANTUÁRIO DO CREPÚSCULO  /  PROTÓTIPO",30,44,440,22,12,muted);
            roundLabel=Label(root,"",525,12,235,27,22,cream,TextAnchor.MiddleCenter,true);
            turnLabel=Label(root,"",525,41,235,23,14,teal,TextAnchor.MiddleCenter);
            shardLabel=Label(root,"",800,22,190,35,17,gold);
            ButtonAt(root,"EQUIPAMENTOS",1000,17,215,43,()=>ShowGear(),panel,16);
            ButtonAt(root,"?",1230,17,46,43,ShowHelp,panel,21);
            ButtonAt(root,"SOM",1290,17,80,43,()=>{mutedAudio=!mutedAudio;Notify(mutedAudio?"Som desativado.":"Som ativado.");},panel,15);
            ButtonAt(root,"REINICIAR",1385,17,185,43,ConfirmReset,panel,15);
            Label(root,"SEU PACTO",32,86,280,25,14,teal,TextAnchor.MiddleLeft,true);
            Label(root,"OS EXILADOS  •  INTENÇÕES VISÍVEIS",1110,86,460,25,14,red,TextAnchor.MiddleRight,true);
            for(int i=0;i<6;i++) BuildUnit(i);
            fxRoot=Box(root,"Effects",0,0,1600,900);
            Box(root,"Hand area",0,618,1600,282,ink);Box(root,"Hand line",0,618,1600,2,gold*.7f);
            hint=Label(root,"",195,623,1190,28,17,cream,TextAnchor.MiddleCenter);
            energyLabel=Label(root,"",22,659,148,65,43,gold,TextAnchor.MiddleCenter,true);
            Label(root,"ENERGIA",22,723,148,21,13,muted,TextAnchor.MiddleCenter,true);
            pactLabel=Label(root,"",14,762,165,38,15,teal,TextAnchor.MiddleCenter);
            deckLabel=Label(root,"",14,813,165,62,13,muted,TextAnchor.MiddleCenter);
            handRoot=Box(root,"Cards",185,660,1200,210);
            endButton=ButtonAt(root,"ENCERRAR\nTURNO",1398,681,174,93,()=>EndTurn(),new Color(.16f,.35f,.35f),21);Border((RectTransform)endButton.transform,teal,1);endText=endButton.GetComponentInChildren<Text>();
            Label(root,"ESPAÇO / TOQUE",1398,779,174,22,12,muted,TextAnchor.MiddleCenter);
            logLabel=Label(root,"",1390,811,195,74,12,muted,TextAnchor.UpperLeft);
            Label(root,"1–6  selecionar carta     •     clique/toque no alvo     •     Esc  cancelar",185,875,1100,20,12,muted,TextAnchor.MiddleCenter);
            var ev=new GameObject("Input",typeof(EventSystem),typeof(StandaloneInputModule));
            sound=gameObject.AddComponent<AudioSource>();attackSound=Synth(160,.12f);guardSound=Synth(480,.18f);clickSound=Synth(720,.035f);
        }
        void BuildUnit(int id)
        {
            var r=Box(root,"Fighter "+id,positions[id].x,positions[id].y,220,210);units[id]=r;
            var shadow=Box(r,"Ground shadow",30,159,162,14,new Color(0,0,0,.32f));
            portraits[id]=Picture(r,sprites[id],8,-15,204,188);
            if(id>=3) {portraits[id].rectTransform.localScale=new Vector3(-1,1,1);portraits[id].rectTransform.anchoredPosition=new Vector2(212,15);}
            var marker=Box(r,"Target line",28,167,166,3,Color.clear);outlines[id]=marker.GetComponent<Image>();
            var hit=Box(r,"Select target",0,-25,220,222,Color.clear);hit.GetComponent<Image>().raycastTarget=true;
            var btn=hit.gameObject.AddComponent<Button>();btn.targetGraphic=hit.GetComponent<Image>();int n=id;btn.onClick.AddListener(()=>ClickUnit(n));
            var plate=Box(r,"Vitals",20,178,180,43,new Color(.035f,.06f,.085f,.92f));
            hpTexts[id]=Label(plate,"",8,0,164,22,13,cream,TextAnchor.MiddleCenter,true);
            Box(plate,"Empty health",8,26,164,7,new Color(.20f,.16f,.18f));hpBars[id]=Box(plate,"Health",8,26,164,7,id<3?teal:red).GetComponent<Image>();
            statuses[id]=Label(r,"",-15,223,250,23,13,gold,TextAnchor.MiddleCenter,true);
            intentTexts[id]=Label(r,"",-38,-50,294,25,13,id<3?muted:cream,TextAnchor.MiddleCenter,true);
        }
        void ResetBattle()
        {
            StopAllCoroutines();busy=false;selected=-1;ClearOverlay();
            int[] load=Match==null?null:Match.Fighters.Take(3).Select(f=>f.Gear).ToArray();Match=new Battle(seed++,load);
            for(int i=0;i<6;i++)units[i].anchoredPosition=new Vector2(positions[i].x,-positions[i].y);
            notice="";Refresh();
        }
        void Refresh()
        {
            roundLabel.text="RODADA "+Match.Round.ToString("00");turnLabel.text=Match.Outcome!=0?"BATALHA CONCLUÍDA":busy?"TURNO DOS EXILADOS":"SEU TURNO";turnLabel.color=busy?red:teal;
            energyLabel.text=Match.Energy+" / 4";shardLabel.text="◆  "+Match.Shards+" fragmentos";
            pactLabel.text="PACTO  "+Match.Harmony+" / 3\n"+(Match.HarmonyUsed?"+1 energia ativada":"3 guerreiros = +1 energia");
            deckLabel.text="BARALHO  "+Match.Deck.Count+"\nDESCARTE  "+Match.Discard.Count;
            endButton.interactable=!busy&&Match.Outcome==0;endText.text=busy?"AGUARDE…":"ENCERRAR\nTURNO";
            logLabel.text=string.Join("\n",Match.Log.Skip(Math.Max(0,Match.Log.Count-2)));
            for(int i=0;i<6;i++) {
                var f=Match.Fighters[i];hpTexts[i].text=f.Name+"   "+f.Hp+" / "+f.MaxHp;
                hpBars[i].rectTransform.sizeDelta=new Vector2(164f*f.Hp/f.MaxHp,7);
                portraits[i].color=f.Alive?Color.white:new Color(.3f,.35f,.4f,.35f);
                statuses[i].text=!f.Alive?"DERROTADO":(f.Shield>0?"ESCUDO "+f.Shield+"  ":"")+(f.Burn>0?"QUEIMA "+f.Burn+"t":"");
                bool valid=selected>=0&&selected<Match.Hand.Count&&Match.CanTarget(Match.Hand[selected],i);
                outlines[i].color=valid?gold:Color.clear;
                var intent=Match.Intents.FirstOrDefault(x=>x.Owner==i);
                intentTexts[i].text=!f.Alive?"":i<3?Match.GearName(i)+"  I"+new string('I',f.Tier):intent==null?"":intent.Label+" "+intent.Power+" → "+(intent.Effect==Effect.Volley?"todos":Match.Fighters[intent.Target].Name);
                // Unidades da fileira traseira compartilham uma coluna: legenda abaixo, sem cobrir os atributos da unidade superior.
                // Back-row units share a column: place their caption below, not over the upper unit's vitals.
                if(i==2||i==5) {
                    if(f.Alive)statuses[i].text=(statuses[i].text.Length>0?statuses[i].text+" • ":"")+intentTexts[i].text;
                    intentTexts[i].text="";
                    statuses[i].rectTransform.sizeDelta=new Vector2(340,23);
                    statuses[i].rectTransform.anchoredPosition=new Vector2(-60,-223);
                    statuses[i].fontSize=12;
                }
            }
            RebuildHand();UpdateHint();
        }
        void RebuildHand()
        {
            foreach(Transform child in handRoot)Destroy(child.gameObject);
            for(int i=0;i<Match.Hand.Count;i++) {
                int n=i;var c=Match.Hand[i];var a=Match.Describe(c);var f=Match.Fighters[c.Owner];
                bool can=!busy&&f.Alive&&Match.Energy>=a.Cost&&Match.Outcome==0;
                Color tint=c.Owner==0?new Color(.18f,.43f,.43f):c.Owner==1?new Color(.37f,.30f,.49f):new Color(.47f,.31f,.22f);
                var r=Box(handRoot,"Card "+i,i*198,selected==i?-9:0,188,209,panel);Border(r,selected==i?gold:can?tint:new Color(.18f,.2f,.23f),selected==i?3:1);
                Box(r,"Art background",3,3,182,64,new Color(tint.r*.5f,tint.g*.5f,tint.b*.5f));
                Picture(r,sprites[c.Owner],60,-1,70,70);
                Label(r,(i+1).ToString(),155,8,22,22,12,muted,TextAnchor.MiddleCenter);
                Box(r,"Cost",7,7,31,31,tint);Label(r,a.Cost.ToString(),7,7,31,31,22,cream,TextAnchor.MiddleCenter,true);
                Label(r,a.Name,9,71,170,26,18,cream,TextAnchor.MiddleCenter,true);
                Label(r,a.Source+(c.Slot==2?" • N"+(f.Tier+1):""),8,99,172,22,11,gold,TextAnchor.MiddleCenter);
                Label(r,a.Description,12,126,164,56,14,cream,TextAnchor.UpperLeft);
                Label(r,f.Name+"  /  "+(a.Friendly?"SUPORTE":"ATAQUE"),10,184,168,20,11,can?muted:new Color(.9f,.4f,.4f),TextAnchor.MiddleCenter,true);
                var img=r.GetComponent<Image>();img.raycastTarget=true;var b=r.gameObject.AddComponent<Button>();b.targetGraphic=img;b.onClick.AddListener(()=>SelectCard(n));
                if(!can)Box(r,"Unavailable",0,0,188,209,new Color(.02f,.03f,.04f,.40f));
            }
        }
        public void SelectCard(int index)
        {
            if(busy||Match.Outcome!=0||index<0||index>=Match.Hand.Count||overlay!=null)return;
            var c=Match.Hand[index];if(!Match.Fighters[c.Owner].Alive){Notify("Este guerreiro foi derrotado.");return;}
            if(Match.Energy<Match.Describe(c).Cost){Notify("Energia insuficiente. Encerre o turno ou escolha outra carta.");return;}
            selected=selected==index?-1:index;notice="";Tone(clickSound);Refresh();
        }
        public void ClickUnit(int id)
        {
            if(busy||overlay!=null||Match.Outcome!=0)return;
            if(selected<0){Notify(Match.Fighters[id].Name+" • selecione primeiro uma carta na mão.");return;}
            var card=Match.Hand[selected];var a=Match.Describe(card);int owner=card.Owner;
            var before=Match.Fighters.Select(f=>f.Hp).ToArray();
            if(!Match.Play(selected,id,out var error)){Notify(error);return;}
            selected=-1;Refresh();Tone(a.Friendly?guardSound:attackSound);
            StartCoroutine(ActionAnimation(owner,id,a.Friendly,before,a.Power));
            if(Match.Outcome!=0)StartCoroutine(ResultAfter());
        }
        IEnumerator ActionAnimation(int owner,int target,bool friendly,int[] before,int power)
        {
            var point=positions[target]+new Vector2(100,85);var start=positions[owner]+new Vector2(100,85);
            var mote=Box(fxRoot,"Spell",start.x,start.y,12,12,friendly?teal:gold);
            float t=0;while(t<.23f){t+=Time.deltaTime;var p=Vector2.Lerp(start,point,t/.23f);mote.anchoredPosition=new Vector2(p.x,-p.y);yield return null;}Destroy(mote.gameObject);
            for(int i=0;i<6;i++)if(before[i]!=Match.Fighters[i].Hp)StartCoroutine(FloatText(i,(Match.Fighters[i].Hp-before[i]).ToString("+0;-0"),Match.Fighters[i].Hp<before[i]?red:teal));
            if(friendly)StartCoroutine(FloatText(target,"+"+power,teal));
            t=0;var original=new Vector2(positions[target].x,-positions[target].y);while(t<.2f){t+=Time.deltaTime;units[target].anchoredPosition=original+new Vector2(Mathf.Sin(t*100)*3,0);yield return null;}units[target].anchoredPosition=original;
        }
        IEnumerator FloatText(int target,string value,Color color)
        {
            var p=positions[target];var label=Label(fxRoot,value,p.x+30,p.y+40,160,45,30,color,TextAnchor.MiddleCenter,true);float t=0;
            while(t<.85f){t+=Time.deltaTime;label.rectTransform.anchoredPosition=new Vector2(p.x+30,-p.y-40+t*50);label.color=new Color(color.r,color.g,color.b,1-t/.85f);yield return null;}Destroy(label.gameObject);
        }
        public void EndTurn(){if(!busy&&Match.Outcome==0&&overlay==null)StartCoroutine(EnemyTurn());}
        IEnumerator EnemyTurn()
        {
            busy=true;selected=-1;Match.BeginEnemy();Refresh();yield return new WaitForSeconds(.6f);
            for(int i=0;i<Match.Intents.Count&&Match.Outcome==0;i++) {
                var intent=Match.Intents[i];var before=Match.Fighters.Select(f=>f.Hp).ToArray();
                if(Match.ExecuteIntent(i)){Refresh();Tone(attackSound);yield return ActionAnimation(intent.Owner,intent.Target,intent.Effect==Effect.Guard,before,intent.Power);yield return new WaitForSeconds(.45f);}
            }
            Match.BeginPlayer();busy=false;Refresh();if(Match.Outcome!=0)yield return ResultAfter();
        }
        void Notify(string value){notice=value;noticeUntil=Time.unscaledTime+4;UpdateHint();}
        void UpdateHint()
        {
            if(notice!=""&&Time.unscaledTime<noticeUntil){hint.text=notice;return;}
            hint.text=Match.Outcome!=0?"Batalha concluída.":busy?"Os exilados estão agindo…":selected>=0&&selected<Match.Hand.Count?Match.Describe(Match.Hand[selected]).Name+"  →  toque em um "+(Match.Describe(Match.Hand[selected]).Friendly?"ALIADO":"INIMIGO")+" destacado":"Escolha uma carta e depois seu alvo. Use os três guerreiros para ativar o Pacto.";
        }
        RectTransform Modal(string title,int height=510)
        {
            ClearOverlay();overlay=Box(root,"Overlay",0,0,1600,900,new Color(.01f,.025f,.04f,.88f));overlay.GetComponent<Image>().raycastTarget=true;
            var p=Box(overlay,"Panel",235,(900-height)/2,1130,height,ink);Border(p,gold,2);
            Label(p,title,30,20,970,44,29,gold,TextAnchor.MiddleLeft,true);
            ButtonAt(p,"×",1050,22,49,43,ClearOverlay,panel,26);return p;
        }
        void ClearOverlay(){if(overlay!=null)Destroy(overlay.gameObject);overlay=null;}
        void ShowGear()
        {
            if(busy)return;var p=Modal("ARSENAL DO PACTO",560);
            Label(p,"Troque o equipamento antes da primeira ação. Forje durante seu turno: 1 energia + fragmentos.",30,73,1060,35,18,muted);
            for(int i=0;i<3;i++) {
                int n=i;var f=Match.Fighters[i];var col=Box(p,"Equipment",30+i*365,124,340,337,panel);
                Picture(col,sprites[i],8,9,91,98);Label(col,f.Name,115,19,210,30,22,cream,TextAnchor.MiddleLeft,true);
                Label(col,"NÍVEL "+(f.Tier+1)+" / 3",115,57,210,24,14,teal);
                Label(col,Match.GearName(i),15,114,310,31,21,gold,TextAnchor.MiddleCenter,true);
                var a=Match.Describe(new Card(i,2));Label(col,a.Name+"\n"+a.Description,20,151,300,80,17,cream,TextAnchor.UpperLeft);
                var swap=ButtonAt(col,"TROCAR EQUIPAMENTO",15,238,310,38,()=>{if(Match.ChangeGear(n)){Refresh();ShowGear();}},ink,15);swap.interactable=Match.Round==1&&Match.TotalPlayed==0&&Match.Outcome==0;
                var forge=ButtonAt(col,f.Tier>=2?"NÍVEL MÁXIMO":"FORJAR  •  1 energia + "+(f.Tier+1)+" ◆",15,285,310,38,()=>{if(Match.Upgrade(n,out var error)){Refresh();ShowGear();Tone(guardSound);}else {ClearOverlay();Notify(error);}},new Color(.16f,.32f,.30f),15);forge.interactable=f.Alive&&f.Tier<2&&Match.Outcome==0&&Match.Energy>=1&&Match.Shards>=f.Tier+1;
            }
            Label(p,"As cartas base permanecem. O equipamento altera a habilidade especial em todas as cópias do baralho.",30,480,1060,43,17,muted,TextAnchor.MiddleCenter);
        }
        void ShowHelp()
        {
            var p=Modal("COMO JOGAR",530);
            Label(p,"1   Selecione uma das seis cartas e toque no alvo destacado.\n2   Você recebe 4 de energia e compra até 6 cartas por turno.\n3   Escudos duram até o próximo turno de quem os recebeu.\n4   Use cartas dos 3 aliados no turno: o Pacto concede +1 energia.\n5   Veja a próxima ação acima de cada inimigo e prepare sua defesa.\n6   EQUIPAMENTOS: troque habilidades antes da primeira ação ou use a forja.\n7   Derrote os 3 exilados. Cartas de aliados caídos saem do ciclo de compra.",40,88,1050,296,21,cream);
            Label(p,"Queima: 6 de dano direto por turno. Perfurante: ignora escudo.\nApós a rodada 12, a instabilidade causa dano crescente aos dois times.",40,400,1050,76,18,gold);
        }
        void ConfirmReset()
        {if(busy)return;var p=Modal("REINICIAR ESTA BATALHA?",250);Label(p,"O combate atual será descartado. Seus equipamentos escolhidos serão mantidos.",35,80,1050,50,21,cream);ButtonAt(p,"REINICIAR",35,157,500,55,ResetBattle);ButtonAt(p,"CONTINUAR",560,157,535,55,ClearOverlay);}
        IEnumerator ResultAfter()
        {
            yield return new WaitForSeconds(.65f);var p=Modal(Match.Outcome==1?"VITÓRIA DO PACTO":Match.Outcome==2?"EMPATE NO SANTUÁRIO":"O PACTO CAIU",350);
            Label(p,"Rodadas: "+Match.Round+"  •  Cartas usadas: "+Match.CardsPlayed+"\nExperimente outros equipamentos e compare suas estratégias.",35,92,1060,100,23,cream,TextAnchor.MiddleCenter);
            ButtonAt(p,"JOGAR NOVAMENTE",290,230,550,66,ResetBattle,new Color(.16f,.35f,.35f),24);
        }
        void Update()
        {
            if(Match==null)return;UpdateHint();
            if(Input.GetKeyDown(KeyCode.Escape)){if(overlay!=null)ClearOverlay();else{selected=-1;Refresh();}}
            if(overlay!=null)return;
            for(int i=0;i<6;i++)if(Input.GetKeyDown(KeyCode.Alpha1+i))SelectCard(i);
            if(Input.GetKeyDown(KeyCode.Space))EndTurn();
        }
        AudioClip Synth(float frequency,float duration)
        {
            int count=(int)(22050*duration);var data=new float[count];for(int i=0;i<count;i++){float t=i/22050f;data[i]=Mathf.Sin(t*frequency*Mathf.PI*2)*(1f-i/(float)count)*.09f;}
            var clip=AudioClip.Create("Rune tone",count,1,22050,false);clip.SetData(data,0);return clip;
        }
        void Tone(AudioClip clip){if(!mutedAudio&&sound!=null&&clip!=null)sound.PlayOneShot(clip);}
    }
}
