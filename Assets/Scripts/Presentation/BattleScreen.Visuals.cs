using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using RunePact.Core;

namespace RunePact.Presentation
{
    public sealed partial class BattleScreen
    {
        RectTransform Round(Transform parent,string name,float x,float y,float w,float h,Color color)
        {
            var r=Box(parent,name,x,y,w,h,color);
            var image=r.GetComponent<Image>();image.sprite=FantasySkin.Rounded;image.type=Image.Type.Sliced;
            image.pixelsPerUnitMultiplier=.65f;
            return r;
        }
        RectTransform Surface(Transform parent,string name,float x,float y,float w,float h,Color color)
        {
            Round(parent,name+" shadow",x,y+5,w,h,new Color(.04f,.07f,.07f,.50f));
            var r=Round(parent,name,x,y,w,h,color);
            var rim=Orb(r,FantasySkin.Frame,0,0,w,h,new Color(.76f,.60f,.34f,.75f));rim.type=Image.Type.Sliced;
            Box(r,"Upper bevel",14,5,w-28,2,new Color(1f,.91f,.64f,.24f));
            Box(r,"Lower bevel",14,h-7,w-28,2,new Color(.03f,.045f,.04f,.55f));
            if(w>280&&h>100){
                foreach(float cx in new[]{13f,w-19})foreach(float cy in new[]{13f,h-19}){
                    Box(r,"Iron stud shadow",cx+1,cy+2,6,6,new Color(.05f,.08f,.08f,.8f));
                    Orb(r,FantasySkin.Rune,cx,cy,6,6,gold);
                }
            }
            return r;
        }
        Image Orb(Transform parent,Sprite shape,float x,float y,float w,float h,Color color)
        {
            var im=Picture(parent,shape,x,y,w,h);im.preserveAspect=false;im.color=color;return im;
        }
        void Pill(Transform parent,string text,float x,float y,float w,float h,Color accent)
        {
            Round(parent,"Badge",x,y,w,h,ink);
            Orb(parent,FantasySkin.Circle,x+11,y+10,10,10,accent);
            Label(parent,text,x+28,y,w-37,h,14,cream,TextAnchor.MiddleCenter,true);
        }
        Color EffectColor(Effect effect,int owner)
        {
            switch(effect){
                case Effect.Mend: case Effect.Regenerate: case Effect.Thorns: case Effect.Purify: return new Color(.57f,1f,.62f);
                case Effect.Guard: case Effect.Rally: case Effect.Fortify: case Effect.Oath: return new Color(.48f,.88f,1f);
                case Effect.Burn: return new Color(1f,.53f,.20f);
                case Effect.Channel: case Effect.Volley: case Effect.Weaken: case Effect.Stun: return violet;
                case Effect.Mark: case Effect.Vulnerable: case Effect.Execute: return gold;
                default: return owner==1||owner==4?violet:gold;
            }
        }
        bool FriendlyEffect(Effect effect)
        {
            return effect==Effect.Guard||effect==Effect.Rally||effect==Effect.Fortify||effect==Effect.Channel||effect==Effect.Mend||effect==Effect.Regenerate||effect==Effect.Thorns||effect==Effect.Oath||effect==Effect.Purify;
        }
        Vector2 UnitCenter(int id){return positions[id]+new Vector2(110,78);}

        IEnumerator CastTravel(int owner,int target,Effect effect)
        {
            Color color=EffectColor(effect,owner);
            Vector2 start=UnitCenter(owner),end=UnitCenter(target);
            var stage=Box(fxRoot,"Cast",0,0,1600,900);
            var halo=Orb(stage,FantasySkin.Ring,start.x-47,start.y-47,94,94,color);
            bool melee=!FriendlyEffect(effect)&&(owner==0||owner==3)&&effect!=Effect.Volley;
            bool support=FriendlyEffect(effect);
            bool arrow=!support&&(owner==2||owner==5);
            var projectile=Orb(stage,melee?FantasySkin.Slash:effect==Effect.Burn?FantasySkin.Flame:arrow?FantasySkin.Arrow:FantasySkin.Spark,start.x-22,start.y-22,44,44,color);
            var core=Orb(projectile.transform,FantasySkin.Circle,13,13,18,18,cream);
            var trail=new List<Image>();
            for(int i=0;i<9;i++)trail.Add(Orb(stage,FantasySkin.Circle,start.x,start.y,8,8,color));
            float duration=melee?.34f:support?.50f:.42f;
            var basePosition=new Vector2(110,-166);
            for(float elapsed=0;elapsed<duration;elapsed+=Time.deltaTime){
                float t=Mathf.Clamp01(elapsed/duration);
                halo.rectTransform.localScale=Vector3.one*(.65f+t*.85f);halo.color=new Color(color.r,color.g,color.b,(1-t)*.7f);
                Vector2 point=Vector2.Lerp(start,end,t)+Vector2.up*(-Mathf.Sin(t*Mathf.PI)*(support?35:65));
                projectile.rectTransform.anchoredPosition=new Vector2(point.x-22,-point.y+22);
                projectile.rectTransform.localEulerAngles=new Vector3(0,0,arrow?Mathf.Atan2(-(end.y-start.y),end.x-start.x)*Mathf.Rad2Deg:effect==Effect.Burn?0:-t*200);
                for(int i=0;i<trail.Count;i++){
                    float q=Mathf.Clamp01(t-i*.022f);
                    Vector2 p=Vector2.Lerp(start,end,q)+Vector2.up*(-Mathf.Sin(q*Mathf.PI)*(support?35:65));
                    trail[i].rectTransform.anchoredPosition=new Vector2(p.x,-p.y);
                    trail[i].color=new Color(color.r,color.g,color.b,(1-i/9f)*.55f);
                }
                float pulse=Mathf.Sin(t*Mathf.PI),facing=owner<3?1:-1;
                if(melee){portraits[owner].rectTransform.anchoredPosition=basePosition+new Vector2(facing*pulse*55,0);portraits[owner].rectTransform.localRotation=Quaternion.Euler(0,0,-facing*pulse*9);}
                else if(arrow){portraits[owner].rectTransform.anchoredPosition=basePosition+new Vector2(-facing*pulse*9,0);}
                else {portraits[owner].rectTransform.localScale=new Vector3(facing*(1+pulse*.035f),1+pulse*.025f,1);}
                yield return null;
            }
            portraits[owner].rectTransform.anchoredPosition=basePosition;Destroy(stage.gameObject);
        }

        IEnumerator Impact(int target,Effect effect,Color color)
        {
            Vector2 center=UnitCenter(target);
            var stage=Box(fxRoot,"Impact",center.x,center.y,1,1);
            var ring=Orb(stage,FantasySkin.Ring,-58,-58,116,116,color);
            Sprite symbol=effect==Effect.Burn?FantasySkin.Flame:effect==Effect.Mend||effect==Effect.Purify||effect==Effect.Regenerate||effect==Effect.Thorns?FantasySkin.Leaf:
                effect==Effect.Guard||effect==Effect.Rally||effect==Effect.Fortify||effect==Effect.Oath?FantasySkin.Shield:
                effect==Effect.Strike||effect==Effect.Pierce||effect==Effect.Execute?FantasySkin.Slash:FantasySkin.Spark;
            var glyph=Orb(stage,symbol,-43,-51,86,102,color);
            if(effect==Effect.Mend||effect==Effect.Purify){Label(stage,"+",-32,-45,64,77,61,cream,TextAnchor.MiddleCenter,true);}
            var particles=new List<Image>();
            for(int i=0;i<12;i++)particles.Add(Orb(stage,effect==Effect.Burn?FantasySkin.Flame:effect==Effect.Mend?FantasySkin.Leaf:FantasySkin.Spark,-6,-6,12,12,color));
            bool friendly=FriendlyEffect(effect);
            bool reactionPose=target<3&&!acting[target]&&IsWard(effect);
            if(reactionPose)BeginAct(target,effect);
            var original=new Vector2(positions[target].x,-positions[target].y);
            for(float elapsed=0;elapsed<.65f;elapsed+=Time.deltaTime){
                float t=Mathf.Clamp01(elapsed/.65f);
                ring.rectTransform.localScale=Vector3.one*(.45f+t*1.5f);
                ring.color=new Color(color.r,color.g,color.b,(1-t)*.85f);
                glyph.color=new Color(color.r,color.g,color.b,(1-t)*.9f);
                glyph.rectTransform.localScale=Vector3.one*(.70f+Mathf.Sin(t*Mathf.PI)*.35f);
                glyph.rectTransform.localEulerAngles=new Vector3(0,0,symbol==FantasySkin.Slash?-65+t*120:Mathf.Sin(t*6)*8);
                for(int i=0;i<particles.Count;i++){
                    float angle=(i*30+17)*Mathf.Deg2Rad;
                    Vector2 p=new Vector2(Mathf.Cos(angle),Mathf.Sin(angle))*(18+80*t);
                    if(friendly||effect==Effect.Burn)p.y+=t*35;
                    particles[i].rectTransform.anchoredPosition=p;
                    particles[i].color=new Color(color.r,color.g,color.b,1-t);
                    particles[i].rectTransform.localScale=Vector3.one*(1-t*.7f);
                }
                if(!friendly&&elapsed<.22f){
                    units[target].anchoredPosition=original+new Vector2(Mathf.Sin(elapsed*85)*5*(1-t),0);
                    portraits[target].color=Color.Lerp(Color.white,new Color(1,.64f,.48f),Mathf.Sin(t*Mathf.PI));
                }
                yield return null;
            }
            units[target].anchoredPosition=original;
            portraits[target].color=Match.Fighters[target].Alive?Color.white:new Color(.3f,.35f,.4f,.35f);
            if(reactionPose)EndAct(target);
            Destroy(stage.gameObject);
        }
        IEnumerator TurnBanner(string title,Color color)
        {
            var plate=Surface(fxRoot,"Turn banner",600,225,400,65,ink);
            Label(plate,title,12,0,376,62,25,color,TextAnchor.MiddleCenter,true);
            var group=plate.gameObject.AddComponent<CanvasGroup>();
            for(float time=0;time<.7f;time+=Time.deltaTime){
                group.alpha=Mathf.Clamp01(Mathf.Min(time*8,(.7f-time)*5));
                yield return null;
            }
            // A sombra é irmã do painel; limpa a decoração junto com o aviso.
            //
            // The shadow is a panel sibling; remove decoration with the notice.
            var shadow=fxRoot.Find("Turn banner shadow");if(shadow!=null)Destroy(shadow.gameObject);
            Destroy(plate.gameObject);
        }
    }
}
