using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using RunePact.Core;

namespace RunePact.Presentation
{
    public sealed partial class BattleScreen
    {
        Sprite bossSprite;
        int animationSpeed=1,shownBossPhase=1;
        bool menuOpen;
        Text speedLabel,relicLabel,bossLabel;
        Image bossBar;
        RectTransform bossHud;
        readonly Image[] wards=new Image[6],wardGlows=new Image[6],sigils=new Image[6],stateIcons=new Image[6];
        readonly Image[,] motes=new Image[6,6];

        Sprite TrimmedSprite(Texture2D texture,int x,int y,int width,int height)
        {
            var pixels=texture.GetPixels32();int left=width,right=-1,bottom=height,top=-1;
            for(int row=0;row<height;row++)for(int col=0;col<width;col++)if(pixels[(y+row)*texture.width+x+col].a>40){left=Math.Min(left,col);right=Math.Max(right,col);bottom=Math.Min(bottom,row);top=Math.Max(top,row);}
            if(right<left)return Sprite.Create(texture,new Rect(x,y,width,height),new Vector2(.5f,0));
            return Sprite.Create(texture,new Rect(x+left,y+bottom,right-left+1,top-bottom+1),new Vector2(.5f,0),100,0,SpriteMeshType.FullRect);
        }
        void LoadBoss()
        {
            var source=Resources.Load<Texture2D>("Art/Regent-v1");if(source==null)return;
            var pixels=source.GetPixels32();int width=source.width,height=source.height;
            // Remove somente o fundo claro conectado às bordas, sem modificar a arte-fonte.
            //
            // Remove only light background connected to the edges without modifying source art.
            var visited=new bool[pixels.Length];var queue=new Queue<int>();
            Action<int> visit=index=>{
                if(visited[index])return;visited[index]=true;var c=pixels[index];
                if(c.a<40||(Math.Min(c.r,Math.Min(c.g,c.b))>170&&Math.Max(c.r,Math.Max(c.g,c.b))-Math.Min(c.r,Math.Min(c.g,c.b))<35)){c.a=0;pixels[index]=c;queue.Enqueue(index);}
            };
            for(int x=0;x<width;x++){visit(x);visit((height-1)*width+x);}for(int y=0;y<height;y++){visit(y*width);visit(y*width+width-1);}
            while(queue.Count>0){int i=queue.Dequeue(),x=i%width,y=i/width;if(x>0)visit(i-1);if(x<width-1)visit(i+1);if(y>0)visit(i-width);if(y<height-1)visit(i+width);}
            var texture=new Texture2D(width,height,TextureFormat.RGBA32,false);texture.SetPixels32(pixels);texture.Apply();texture.filterMode=FilterMode.Point;
            bossSprite=TrimmedSprite(texture,0,0,width,height);
        }
        void BuildStatusVisuals(RectTransform parent,int id)
        {
            sigils[id]=Orb(parent,FantasySkin.Ring,29,147,164,34,Color.clear);
            wardGlows[id]=Orb(parent,FantasySkin.Circle,21,-12,178,184,Color.clear);
            wards[id]=Orb(parent,FantasySkin.Ring,12,-23,196,206,Color.clear);
            stateIcons[id]=Orb(parent,FantasySkin.Shield,91,36,38,44,Color.clear);
            for(int n=0;n<6;n++)motes[id,n]=Orb(parent,FantasySkin.Spark,20+n*29,90,10,10,Color.clear);
        }
        void BuildEnhancements()
        {
            animationSpeed=PlayerPrefs.GetInt("RunePactSpeed",1)==2?2:1;mutedAudio=PlayerPrefs.GetInt("RunePactMuted",0)==1;
            var speed=ButtonAt(root,animationSpeed+"×",402,26,94,49,ToggleSpeed,panel,21);speedLabel=speed.GetComponentInChildren<Text>();
            var relic=ButtonAt(root,"Relíquias • 0",195,106,300,31,ShowRelics,ink,13);relicLabel=relic.GetComponentInChildren<Text>();
            bossHud=Surface(root,"Boss status",584,107,432,66,ink);
            bossLabel=Label(bossHud,"",10,2,412,34,15,gold,TextAnchor.MiddleCenter,true);
            Round(bossHud,"Boss empty",17,43,398,12,panel);bossBar=Round(bossHud,"Boss health",17,43,398,12,red).GetComponent<Image>();
        }
        void ToggleSpeed()
        {
            animationSpeed=animationSpeed==1?2:1;PlayerPrefs.SetInt("RunePactSpeed",animationSpeed);PlayerPrefs.Save();
            if(!paused)Time.timeScale=animationSpeed;if(speedLabel!=null)speedLabel.text=animationSpeed+"×";
        }
        void ShowStartMenu()
        {
            var p=Modal("R U N E P A C T",500);menuOpen=true;paused=true;Time.timeScale=0;
            foreach(var b in p.GetComponentsInChildren<Button>())b.gameObject.SetActive(false);
            var closeShadow=p.Find("× shadow");if(closeShadow!=null)closeShadow.gameObject.SetActive(false);
            Picture(p,sprites[0],60,105,200,255);Picture(p,sprites[1],865,105,200,255);
            Label(p,"Santuário do Crepúsculo",275,85,580,50,29,cream,TextAnchor.MiddleCenter,true);
            Label(p,"TESTE RÁPIDO\n1 encontro  →  recompensa  →  Regente de Âmbar\nCartas, equipamentos e um pacto entre três guerreiros.",270,142,590,94,19,muted,TextAnchor.MiddleCenter);
            ButtonAt(p,"CONTINUAR JORNADA",310,264,510,60,()=>{menuOpen=false;ClearOverlay();if(Match.Outcome!=0)StartCoroutine(ResultAfter());},new Color(.28f,.47f,.39f),23);
            ButtonAt(p,"NOVA JORNADA",310,341,510,55,()=>{menuOpen=false;ResetBattle();},panel,20);
            Label(p,"Toque em um personagem para inspecionar seus efeitos. Velocidade 1×/2× no topo.",50,433,1030,35,17,gold,TextAnchor.MiddleCenter);
        }
        void FitSafeArea()
        {
            var canvas=root.GetComponentInParent<Canvas>();float scale=canvas.scaleFactor;if(scale<=0)return;
            var safe=Screen.safeArea;float fit=Mathf.Min(safe.width/(1600*scale),safe.height/(900*scale));
            root.localScale=Vector3.one*fit;root.anchoredPosition=(safe.center-new Vector2(Screen.width,Screen.height)*.5f)/scale;
        }
        void RefreshEnhancements()
        {
            bool boss=Match.Formation==EnemyFormation.Regent;bossHud.gameObject.SetActive(boss);
            var shadow=root.Find("Boss status shadow");if(shadow!=null)shadow.gameObject.SetActive(boss);
            if(boss){var f=Match.Fighters[3];bossLabel.text="REGENTE DE ÂMBAR • FASE "+Match.BossPhase+" • "+f.Hp+" / "+f.MaxHp;bossBar.rectTransform.sizeDelta=new Vector2(398f*f.Hp/f.MaxHp,12);}
            relicLabel.text=journey.Relics.Count==0?"Relíquias • nenhuma":Journey.RelicTitle(journey.Relics[0]);
            for(int i=0;i<6;i++){
                bool hide=boss&&i>=4;units[i].gameObject.SetActive(!hide);
                var f=Match.Fighters[i];Sprite sprite=i<3?sprites[i]:f.Role==EnemyRole.Regent?bossSprite:f.Role==EnemyRole.Assassin?sprites[5]:f.Role==EnemyRole.Acolyte||f.Role==EnemyRole.Pyromancer?sprites[4]:sprites[3];
                if(sprite==null)sprite=sprites[i];portraits[i].sprite=sprite;
                float height=boss&&i==3?236:188;float width=height*sprite.rect.width/sprite.rect.height;
                portraits[i].rectTransform.sizeDelta=new Vector2(width,height);
                var intent=units[i].Find("Intention") as RectTransform;var intentShadow=units[i].Find("Intention shadow") as RectTransform;
                float intentY=boss&&i==3?102:44;intent.anchoredPosition=new Vector2(-16,intentY);intentShadow.anchoredPosition=new Vector2(-16,intentY-5);
                if(f.Stun>0&&i>=3)intentTexts[i].text="ATORDOADO • próxima ação impedida";
            }
            if(boss&&shownBossPhase!=Match.BossPhase){shownBossPhase=Match.BossPhase;StartCoroutine(TurnBanner("A runa desperta • FASE 2",gold));StartCoroutine(Impact(3,Effect.Channel,gold));}
            if(!boss)shownBossPhase=1;
        }
        void AnimateCharacters()
        {
            float time=Time.time;
            for(int i=0;i<6;i++){
                var f=Match.Fighters[i];float breath=f.Alive?Mathf.Sin(time*2.1f+i*1.7f):0;
                portraits[i].rectTransform.localScale=new Vector3((i<3?1:-1)*(1-breath*.006f),1+breath*.009f,1);
                portraits[i].rectTransform.localRotation=Quaternion.Euler(0,0,f.Alive?Mathf.Sin(time*1.3f+i)*.45f:0);
                bool ward=f.Alive&&f.Shield>0;float pulse=.5f+.5f*Mathf.Sin(time*2.6f+i);
                wards[i].color=ward?new Color(.42f,.83f,1f,.22f+pulse*.15f):Color.clear;
                wardGlows[i].color=ward?new Color(.4f,.82f,1f,.045f+pulse*.025f):Color.clear;
                Color state=f.Burn>0?new Color(1,.40f,.12f):f.Weak>0||f.Stun>0?violet:f.Mark>0||f.Vulnerable>0?gold:teal;
                bool debuff=f.Burn>0||f.Weak>0||f.Stun>0||f.Mark>0||f.Vulnerable>0;
                bool magic=f.Alive&&(debuff||f.Channel>0||f.Regen>0||f.Thorns>0||f.Fortified>0);
                sigils[i].color=magic?new Color(state.r,state.g,state.b,.3f+pulse*.25f):Color.clear;
                stateIcons[i].sprite=f.Burn>0?FantasySkin.Flame:f.Stun>0||f.Weak>0?FantasySkin.Spark:f.Regen>0||f.Thorns>0?FantasySkin.Leaf:f.Mark>0||f.Vulnerable>0?FantasySkin.Arrow:FantasySkin.Shield;
                stateIcons[i].color=magic?new Color(state.r,state.g,state.b,.65f):ward?new Color(.48f,.88f,1,.4f):Color.clear;
                stateIcons[i].rectTransform.anchoredPosition=new Vector2(91,-36+Mathf.Sin(time*2+i)*3);
                for(int n=0;n<6;n++){
                    float phase=Mathf.Repeat(time*.45f+n/6f+i*.13f,1);
                    motes[i,n].sprite=f.Burn>0?FantasySkin.Flame:FantasySkin.Spark;
                    motes[i,n].rectTransform.anchoredPosition=new Vector2(33+n*28+Mathf.Sin(time+n)*5,-154+phase*132);
                    motes[i,n].color=magic?new Color(state.r,state.g,state.b,Mathf.Sin(phase*Mathf.PI)*.7f):Color.clear;
                }
            }
        }
        RectTransform BuildRewardCard(Transform parent,RewardOffer offer,float x,float y)
        {
            var r=Surface(parent,"Reward "+offer.Kind,x,y,330,465,parchment);
            Color accent=offer.Kind==RewardKind.Relic?new Color(.52f,.36f,.16f):offer.Owner==0?new Color(.24f,.43f,.37f):offer.Owner==1?new Color(.44f,.32f,.52f):new Color(.54f,.34f,.22f);
            Round(r,"Artwork",9,9,312,180,accent);Orb(r,FantasySkin.Ring,92,27,146,146,new Color(1,1,1,.12f));
            string category=offer.Kind==RewardKind.Card?"NOVA CARTA • "+(offer.Card==CardVariant.Tremor?"ÉPICA":"RARA"):offer.Kind==RewardKind.Upgrade?"APRIMORAMENTO • +6 PODER":"RELÍQUIA • PASSIVA";
            Ability ability=null;
            if(offer.Kind!=RewardKind.Relic){
                var card=new Card(offer.Owner,offer.Kind==RewardKind.Upgrade?offer.Slot:2,offer.Kind==RewardKind.Card?offer.Card:CardVariant.Standard);ability=Match.Describe(card);
                Picture(r,sprites[offer.Owner],81,16,169,170);Orb(r,FantasySkin.Circle,19,20,52,52,gold);Label(r,ability.Cost.ToString(),19,20,52,51,29,writing,TextAnchor.MiddleCenter,true);
                var symbol=ability.Friendly?FantasySkin.Shield:FantasySkin.Spark;Orb(r,symbol,252,130,48,49,gold);
            }else{
                Orb(r,FantasySkin.Circle,115,44,100,100,gold);Orb(r,FantasySkin.Ring,108,36,114,114,cream);
                Orb(r,offer.Relic==Relic.AncientEmber?FantasySkin.Flame:offer.Relic==Relic.CeremonialArrow?FantasySkin.Arrow:offer.Relic==Relic.OakBark?FantasySkin.Leaf:FantasySkin.Spark,135,63,60,60,accent);
            }
            Label(r,category,12,194,306,24,13,accent,TextAnchor.MiddleCenter,true);
            var title=Label(r,offer.Title.ToUpperInvariant(),16,222,298,54,24,writing,TextAnchor.MiddleCenter,true);title.resizeTextForBestFit=true;title.resizeTextMinSize=18;title.resizeTextMaxSize=24;
            string detail=offer.Kind==RewardKind.Relic?offer.Detail:ability.Description;
            if(offer.Kind==RewardKind.Upgrade)detail="Poder "+ability.Power+" → "+(ability.Power+6)+"\n"+offer.Detail;
            Label(r,detail,23,281,284,78,17,writing,TextAnchor.MiddleCenter);
            Label(r,offer.Kind==RewardKind.Card?"Entra na mão inicial do chefe":offer.Kind==RewardKind.Upgrade?Match.Fighters[offer.Owner].Name+" • só esta habilidade":"Efeito ativo durante toda a jornada",19,364,292,26,13,accent,TextAnchor.MiddleCenter,true);
            return r;
        }
        void ShowRelics()
        {
            if(busy||rewardOpen)return;var p=Modal("RELÍQUIAS DO PACTO",370);
            if(journey.Relics.Count==0)Label(p,"Você ainda não possui relíquias.\nAo vencer o primeiro encontro, poderá escolher uma como recompensa.",50,110,1030,120,24,cream,TextAnchor.MiddleCenter);
            else for(int i=0;i<journey.Relics.Count;i++){Label(p,Journey.RelicTitle(journey.Relics[i]),45,95+i*85,1040,35,25,gold);Label(p,Journey.RelicDetail(journey.Relics[i]),45,134+i*85,1040,35,22,cream);}
        }
        void ShowFighterDetails(int id)
        {
            var f=Match.Fighters[id];var p=Modal(f.Name+" • ESTADOS",600);Picture(p,portraits[id].sprite,45,105,225,270);
            Label(p,"VIDA "+f.Hp+" / "+f.MaxHp+"     ESCUDO "+f.Shield,300,85,775,43,24,cream,TextAnchor.MiddleLeft,true);
            var lines=new List<string>();
            if(f.Shield>0)lines.Add("Escudo: absorve dano até o início do próximo turno.");
            if(f.Fortified>0)lines.Add("Fortificado: preserva o escudo uma vez.");
            if(f.Burn>0)lines.Add("Queimadura: "+(f.Enemy&&Match.Relics.Contains(Relic.AncientEmber)?9:6)+" de dano por turno; "+f.Burn+" restante(s).");
            if(f.Mark>0)lines.Add("Marca: próximo ataque recebe +10 (+15 com Flecha).");
            if(f.Channel>0)lines.Add("Canalização: +"+f.Channel+" no próximo ataque.");
            if(f.Weak>0)lines.Add("Fraqueza: -25% de poder na próxima ação.");
            if(f.Vulnerable>0)lines.Add("Vulnerável: +25% de dano nos próximos "+f.Vulnerable+" ataques.");
            if(f.Regen>0)lines.Add("Regeneração: +10 de vida nos próximos "+f.Regen+" turnos.");
            if(f.Thorns>0)lines.Add("Espinhos: devolve 7 por ataque; "+f.Thorns+" turnos.");
            if(f.Stun>0)lines.Add("Atordoamento: impede a próxima ação.");
            if(lines.Count==0)lines.Add(f.Alive?"Nenhum efeito ativo.":"Derrotado. Suas cartas não são mais compradas.");
            Label(p,string.Join("\n",lines),300,143,775,335,20,cream);
            ButtonAt(p,"VOLTAR AO COMBATE",320,502,690,57,ClearOverlay,new Color(.28f,.47f,.39f),22);
        }
        void OnApplicationPause(bool value){if(value)SaveJourney();}
        void OnApplicationQuit(){SaveJourney();}
    }
}
