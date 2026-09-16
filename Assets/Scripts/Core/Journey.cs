using System.Linq;

namespace RunePact.Core
{
    public sealed class Journey
    {
        public Battle Current { get; private set; }
        public int Encounter { get; private set; } = 1;
        public bool AwaitingReward => Current.Outcome == 1 && Encounter < 3;
        public bool Complete => Current.Outcome == 1 && Encounter == 3;
        readonly int seed;

        public Journey(int seed, int[] gear = null)
        {
            this.seed = seed;
            Current = new Battle(seed, gear);
        }

        public bool ChooseReward(int choice)
        {
            if (!AwaitingReward || choice < 0 || choice > 2) return false;
            var previous = Current;
            Encounter++;
            Current = new Battle(seed + Encounter, previous.Fighters.Take(3).Select(f => f.Gear).ToArray(), Encounter);
            Current.Shards = previous.Shards + (choice == 2 ? 4 : 0);
            // Descanso parcial recupera caídos; equipamentos e melhorias acompanham a jornada.
            //
            // Partial rest revives fallen allies; equipment and upgrades persist through the journey.
            for (int i = 0; i < 3; i++)
            {
                var hero = Current.Fighters[i];
                hero.Tier = previous.Fighters[i].Tier;
                hero.MaxHp = previous.Fighters[i].MaxHp + (choice == 1 ? 15 : 0);
                hero.Hp = System.Math.Min(hero.MaxHp, previous.Fighters[i].Hp + (choice == 0 ? 65 : choice == 1 ? 45 : 30));
            }
            Current.Plan();
            return true;
        }
    }
}
