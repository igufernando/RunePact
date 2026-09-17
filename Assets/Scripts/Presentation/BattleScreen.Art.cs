using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using RunePact.Core;

namespace RunePact.Presentation
{
    public sealed partial class BattleScreen
    {
        readonly Sprite[,] actionSprites=new Sprite[3,3];
        readonly Sprite[] itemSprites=new Sprite[8];
        readonly bool[] acting=new bool[6];

        void LoadActionArt()
        {
            var actions=Resources.Load<Texture2D>("Art/WarriorActions-v1");
            if(actions!=null)for(int hero=0;hero<3;hero++)for(int pose=0;pose<3;pose++){
                int x=pose*actions.width/3,y=(2-hero)*actions.height/3;
                actionSprites[hero,pose]=TrimmedSprite(actions,x,y,(pose+1)*actions.width/3-x,(3-hero)*actions.height/3-y);
            }
            var items=Resources.Load<Texture2D>("Art/Rewards-v2");
            if(items!=null)for(int i=0;i<8;i++){
                int x=i%4*items.width/4,y=(1-i/4)*items.height/2;
                itemSprites[i]=TrimmedSprite(items,x,y,(i%4+1)*items.width/4-x,(2-i/4)*items.height/2-y);
            }
        }
        bool IsWard(Effect fx) => fx==Effect.Guard||fx==Effect.Fortify||fx==Effect.Rally||fx==Effect.Oath;
        bool IsHealing(Effect fx) => fx==Effect.Mend||fx==Effect.Purify||fx==Effect.Regenerate;
        int PoseSlot(int hero,Effect fx)
        {
            if(IsWard(fx))return fx==Effect.Oath?2:1;
            if(IsHealing(fx)||fx==Effect.Channel||fx==Effect.Thorns)return 2;
            if(hero==2&&(fx==Effect.Mark||fx==Effect.Execute||fx==Effect.Vulnerable))return 2;
            return 0;
        }
        Sprite AbilityPortrait(int hero,Effect fx) => actionSprites[hero,PoseSlot(hero,fx)]??sprites[hero];
        Sprite AbilityIcon(Effect fx,int hero)
        {
            if(IsWard(fx))return FantasySkin.Shield;
            if(IsHealing(fx))return FantasySkin.Heart;
            if(fx==Effect.Burn)return FantasySkin.Flame;
            if(fx==Effect.Mark||fx==Effect.Vulnerable||fx==Effect.Stun||fx==Effect.Weaken||fx==Effect.Channel)return FantasySkin.Rune;
            if(fx==Effect.Thorns)return FantasySkin.Leaf;
            return hero==2?FantasySkin.Arrow:hero==1||fx==Effect.Volley?FantasySkin.Spark:FantasySkin.Sword;
        }
        string AbilityPurpose(Effect fx,int hero)
        {
            if(IsWard(fx))return "DEFESA";
            if(IsHealing(fx))return "CURA";
            if(fx==Effect.Channel||fx==Effect.Thorns)return "SUPORTE";
            if(fx==Effect.Mark||fx==Effect.Vulnerable||fx==Effect.Stun||fx==Effect.Weaken)return "CONTROLE";
            if(fx==Effect.Burn||hero==1)return "MAGIA";
            return "ATAQUE";
        }
        Color PurposeColor(Effect fx,int hero)
        {
            if(IsWard(fx))return new Color(.25f,.52f,.65f);
            if(IsHealing(fx)||fx==Effect.Thorns)return new Color(.28f,.51f,.34f);
            if(fx==Effect.Channel||hero==1)return new Color(.46f,.31f,.57f);
            if(fx==Effect.Mark||fx==Effect.Stun||fx==Effect.Weaken||fx==Effect.Vulnerable)return new Color(.60f,.40f,.20f);
            return new Color(.61f,.29f,.23f);
        }
        void CardDecoration(RectTransform card,CardRarity rarity,float width,float height)
        {
            Color accent=rarity==CardRarity.Epic?violet:rarity==CardRarity.Rare?new Color(.43f,.81f,.95f):gold;
            var frame=Orb(card,FantasySkin.Frame,0,0,width,height,accent);frame.type=Image.Type.Sliced;
            if(rarity==CardRarity.Common)return;
            int count=rarity==CardRarity.Epic?3:2;
            for(int i=0;i<count;i++)Orb(card,FantasySkin.Rune,width*.5f+(i-(count-1)*.5f)*15-5,-5,10,12,accent);
            card.gameObject.AddComponent<RunicShimmer>().Initialize(frame,accent,rarity==CardRarity.Epic);
        }
        void BeginAct(int id,Effect fx)
        {
            acting[id]=true;
            if(id<3){var sprite=AbilityPortrait(id,fx);portraits[id].sprite=sprite;float height=205;portraits[id].rectTransform.sizeDelta=new Vector2(height*sprite.rect.width/sprite.rect.height,height);}
        }
        void EndAct(int id)
        {
            acting[id]=false;var rect=portraits[id].rectTransform;
            rect.anchoredPosition=new Vector2(110,-166);rect.localRotation=Quaternion.identity;rect.localScale=new Vector3(id<3?1:-1,1,1);
            if(id<3){portraits[id].sprite=sprites[id];rect.sizeDelta=new Vector2(188*sprites[id].rect.width/sprites[id].rect.height,188);}
        }
        IEnumerator Windup(int owner,Effect fx)
        {
            BeginAct(owner,fx);var rect=portraits[owner].rectTransform;float facing=owner<3?1:-1;
            for(float elapsed=0;elapsed<.22f;elapsed+=Time.deltaTime){
                float t=elapsed/.22f;float bend=Mathf.Sin(t*Mathf.PI);
                rect.localRotation=Quaternion.Euler(0,0,facing*bend*(IsWard(fx)?5:-7));
                rect.localScale=new Vector3(facing*(1+bend*.035f),1-bend*.035f,1);
                yield return null;
            }
        }
    }
    public sealed class RunicShimmer : MonoBehaviour
    {
        Image frame;Color tint;bool epic;
        public void Initialize(Image image,Color color,bool isEpic){frame=image;tint=color;epic=isEpic;}
        void Update(){if(frame!=null)frame.color=Color.Lerp(tint,Color.white,(.5f+.5f*Mathf.Sin(Time.unscaledTime*(epic?2.8f:1.8f)))*(epic?.38f:.20f));}
    }
}
