# Rodada V5 — jornada, legibilidade e preparação mobile (PT-BR)

## Escopo jogável

O protótipo preserva os sprites dos personagens e a HUD de fantasia acolhedora já aprovados. O menu permite escolher jornada rápida (um encontro e o Regente de Âmbar) ou padrão (dois encontros e o Regente), além de três dificuldades para os encontros comuns: calma, normal e intensa. O chefe tem 120 de vida no modo rápido e 220 no padrão. Após cada encontro comum, uma recompensa entra imediatamente na jornada; a tela final mostra vitória/derrota, duração, cartas usadas e médias locais. Reiniciar cria uma nova jornada; o menu permite continuar a salva.

## Combate

Cada guerreiro possui uma carta única no baralho: Aura concede escudo e retaliação, Lyra cura/limpa efeitos e regenera, Kael executa alvos marcados ou vulneráveis. Cartas mostram categoria (ataque, suporte ou controle), raridade e sinal visual do equipamento. A ordem inimiga aparece no topo; a carta e o alvo escolhidos recebem realce. Selecionar um alvo mostra o custo e exige confirmação antes de gastar energia. Também é possível arrastar uma carta ao alvo; o painel de confirmação permanece. Efeitos de magia, cura, proteção e chefe têm sons distintos; estados ativos permanecem visíveis nos personagens.

## Arquitetura e teste

As regras continuam em C# puro em `Assets/Scripts/Core`; a interface, o arraste e a adaptação visual ficam em `Assets/Scripts/Presentation`. Salvamento V5 inclui modo, dificuldade, tempo, cartas usadas e estado completo do encontro, com leitura de salvamentos V4. O teste automatizado cobre as habilidades únicas e ambas as rotas. Um relatório local gerado por simulações (`BalanceTestResults.txt`, ignorado pelo Git) compara vitórias, rodadas e cartas por modo/dificuldade; ele não substitui playtests humanos.

O jogo é landscape-first. A escala respeita a área segura e, em orientação vertical, apresenta um aviso para girar o aparelho em vez de reduzir a batalha a alvos minúsculos. O toque real e a performance Android ainda precisam ser testados em dispositivos.

---

# V5 milestone — journey, clarity, and mobile preparation (EN)

## Playable scope

The prototype preserves the approved character sprites and welcoming fantasy HUD. The menu offers a quick journey (one encounter and the Amber Regent) or a standard journey (two encounters and the Regent), plus three difficulties for normal encounters: calm, normal, and fierce. The boss has 120 health in quick mode and 220 in standard mode. A reward enters the journey immediately after each normal encounter; the final screen shows win/loss, duration, cards played, and local averages. Restart creates a fresh journey; the menu can resume the saved one.

## Combat

Each warrior has one unique card in the deck: Aura grants shield and retaliation, Lyra heals/cleanses and regenerates, and Kael executes marked or vulnerable targets. Cards show role (attack, support, or control), rarity, and an equipment-specific visual cue. Enemy action order appears at the top; the chosen card and target are highlighted. Selecting a target previews the cost and requires confirmation before spending energy. A card can also be dragged onto a target while retaining confirmation. Magic, healing, protection, and boss effects have distinct sounds; active states remain visible on the characters.

## Architecture and testing

Rules remain engine-independent C# under `Assets/Scripts/Core`; UI, dragging, and visual adaptation live in `Assets/Scripts/Presentation`. V5 saves include mode, difficulty, elapsed time, cards used, and the full encounter state, with V4 save reading. Automated tests cover the unique skills and both routes. A local generated simulation report (`BalanceTestResults.txt`, Git-ignored) compares wins, rounds, and cards by mode/difficulty; it does not replace human playtesting.

The game is landscape-first. Scaling respects the safe area and portrait orientation shows a rotate-device prompt instead of shrinking combat to tiny targets. Real touch and Android performance still require on-device validation.
