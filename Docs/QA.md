# QA do protótipo (PT-BR)

## Rodada de evolução

A rodada visual mantém os personagens aprovados, reorganiza o campo e renova cartas, HUD e arsenal. Foram conferidos seleção de alvos, impacto de marca, proteção de equipe, cura efetiva, transição de turno e arsenal no executável Windows. As regras continuam passando em 2.511 verificações. A validação visual foi feita em desktop; testes de toque em aparelho ainda estão pendentes.

A evolução passou em 2.511 verificações e 80 batalhas simuladas. Os testes adicionais cobrem marca, canalização, duração de Fortificado, cura do Acólito, bloqueio de recompensa fora de vitória, recuperação de caídos, persistência de melhorias e término da jornada. O build Windows foi gerado com sucesso. A seleção e aplicação da Marca Rúnica foram conferidas na interface; o balanceamento da jornada ainda precisa de playtests humanos.

## Verificações automatizadas de combate

O protótipo inicial passou em **2.608 verificações** distribuídas por **80 batalhas simuladas com seeds**: 79 vitórias e 1 derrota para o bot simples de teste. Isso valida o comportamento das regras e o término das partidas, não o balanceamento ou diversão para jogadores.

Comportamentos cobertos:

- Tamanhos do baralho e mão, intenções inimigas e troca de equipamentos.
- Alvos inválidos, energia, ataques, escudos, perfuração, cura e limpeza de queimadura.
- Bônus de Pacto, custos e limites da forja, reinício de turno, compra, embaralhamento, eliminação, vitória e derrota.

Execute **RunePact > Test combat rules** na Unity após mudanças nas regras. O arquivo gerado `CombatTestResults.txt` é intencionalmente ignorado pelo Git.

## Verificações visuais manuais

O build inicial para Windows foi verificado para a cena de batalha, seis sprites, troca de equipamento, upgrades da forja, seleção de cartas, alvos, dano, queimadura, progressão do turno da IA e atualizações do HUD.

## Ainda não validado

- Aparelhos Android, hardware de toque, áreas seguras e desempenho em dispositivos modestos.
- Acessibilidade, multiplayer, persistência, monetização e requisitos de lojas.
- Balanceamento por playtests humanos e retenção de longo prazo.

---

# Prototype QA (EN)

## Evolution milestone

The visual milestone preserves the approved characters, rearranges the field, and refreshes cards, HUD, and equipment panels. Target selection, mark impact, team protection, actual healing, turn transitions, and equipment UI were checked in the Windows executable. Rules still pass 2,511 assertions. Visual validation was on desktop; device touch testing remains pending.

The evolution passed 2,511 assertions and 80 simulated battles. Additional tests cover mark, channel, Fortified duration, Acolyte healing, reward gating, fallen ally recovery, upgrade persistence, and journey completion. The Windows build succeeded. Runic mark selection and application were checked in the UI; journey balance still requires human playtests.

## Automated combat checks

The initial prototype passed **2,608 assertions** across **80 seeded simulated battles**: 79 wins and 1 loss for the simple test bot. This validates rule behavior and termination, not player-facing balance or enjoyment.

Covered behavior:

- Deck and hand sizes, enemy intentions, and equipment swapping.
- Invalid targets, energy, attacks, shields, piercing, healing, and burn cleanup.
- Pact bonus, forge costs and caps, turn reset, drawing, reshuffling, elimination, victory, and defeat.

Run **RunePact > Test combat rules** in Unity after rules change. The generated `CombatTestResults.txt` is intentionally ignored by Git.

## Manual visual checks

The initial Windows build was checked for the battle scene, six sprites, equipment switching, forge upgrades, card selection, targets, damage, burn, AI turn progression, and HUD updates.

## Not yet validated

- Android devices, touch hardware, safe areas, and low-end performance.
- Accessibility, multiplayer, persistence, monetization, and storefront requirements.
- Human-playtest balance and long-term retention.
