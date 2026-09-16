# Prototype QA / QA do protótipo

## Automated combat checks / Verificações automatizadas de combate

The initial prototype passed **2,608 assertions** across **80 seeded simulated battles**: 79 wins and 1 loss for the simple test bot. This validates rule behavior and termination, not player-facing balance or enjoyment.

O protótipo inicial passou em **2.608 verificações** distribuídas por **80 batalhas simuladas com seeds**: 79 vitórias e 1 derrota para o bot simples de teste. Isso valida o comportamento das regras e o término das partidas, não o balanceamento ou diversão para jogadores.

Covered behavior / Comportamentos cobertos:

- Deck and hand sizes, enemy intentions, and equipment swapping / tamanhos do baralho e mão, intenções inimigas e troca de equipamentos.
- Invalid targets, energy, attacks, shields, piercing, healing, and burn cleanup / alvos inválidos, energia, ataques, escudos, perfuração, cura e limpeza de queimadura.
- Pact bonus, forge costs/caps, turn reset, drawing, reshuffling, elimination, victory, and defeat / bônus de Pacto, custos/limites da forja, reinício de turno, compra, embaralhamento, eliminação, vitória e derrota.

Run **RunePact > Test combat rules** in Unity after rules change. The generated `CombatTestResults.txt` is intentionally ignored by Git.

Execute **RunePact > Test combat rules** na Unity após mudanças nas regras. O arquivo gerado `CombatTestResults.txt` é intencionalmente ignorado pelo Git.

## Manual visual checks / Verificações visuais manuais

The initial Windows build was checked for the battle scene, six sprites, equipment switching, forge upgrades, card selection, targets, damage, burn, AI turn progression, and HUD updates.

O build inicial para Windows foi verificado para a cena de batalha, seis sprites, troca de equipamento, upgrades da forja, seleção de cartas, alvos, dano, queimadura, progressão do turno da IA e atualizações do HUD.

## Not yet validated / Ainda não validado

- Android devices, touch hardware, safe areas, and low-end performance / aparelhos Android, hardware de toque, áreas seguras e desempenho em dispositivos modestos.
- Accessibility, multiplayer, persistence, monetization, and storefront requirements / acessibilidade, multiplayer, persistência, monetização e requisitos de lojas.
- Human-playtest balance and long-term retention / balanceamento por playtests humanos e retenção de longo prazo.
