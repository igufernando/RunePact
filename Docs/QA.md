# QA do protótipo (PT-BR)

## Rodada atual: visual V6

Regras: **4.512 verificações aprovadas**, incluindo 80 batalhas base e 45 jornadas variadas. Os 17 recortes de arte (nove poses e oito itens) passaram na verificação de conteúdo visível, transparência e margens das células. Build Windows gerado com sucesso.

Inspeção no executável isolado: poses nas cartas, ícones, raridade azul/violeta, HUD e painel de estados, ataque por seleção e clique sem confirmação (155 → 127 de vida), defesa após seleção e arraste (+28 escudo), retorno à pose base, arte de carta/runa/grimório na recompensa e escolha da relíquia levando ao chefe com 120 de vida. O arraste iniciado sem seleção prévia não foi confiável na automação; precisa ser conferido com mouse/toque humano. Clique na carta seguido de clique no alvo foi validado.

O cenário padrão da revisão mostra seis cartas variadas com energia extra exclusivamente para QA; isso não altera a partida normal nem o salvamento do jogador. As animações são poses e transformações de sprites, não sequências completas quadro a quadro. Android e interação por toque continuam pendentes.

## Marco anterior: V5

As regras passaram em **4.512 verificações**, incluindo 80 batalhas base e 45 jornadas variadas com salvamento exato. Testes novos cobrem os três níveis de dificuldade, uma carta única por guerreiro, efeitos exclusivos, rota padrão com dois encontros e chefe de 220 de vida, migração de salvamento V4, além da persistência do modo e do cronômetro. O build Windows compilou. A inspeção visual confirmou menu, combate, ordem inimiga, seleção de carta, arraste para o alvo, prévia/cancelar/confirmar sem gasto antecipado e resolução de ataque. O gesto ainda precisa de validação com toque em aparelho real.

Um bot simples de referência fez 24 jornadas por combinação de modo/dificuldade: rápido 24/24 vitórias em todos os níveis; padrão 24/24 em calma e normal, 23/24 em intensa. Médias de cartas usadas: rápido 35/41/47 e padrão 69/82/91 (calma/normal/intensa). Isso mostra aumento de duração e pressão, mas **não comprova diversão ou balanceamento humano**. O relatório gerado `BalanceTestResults.txt` é ignorado pelo Git.

Android não foi compilado ou validado nesta rodada. O layout é horizontal; orientação vertical mostra aviso para girar. Testes em múltiplos tamanhos de janela, hardware de toque, áreas seguras reais e desempenho mobile permanecem pendentes.

## Marco anterior: teste rápido V4

Regras passaram em **5.153 verificações**: 80 batalhas base e 45 jornadas variadas. Para garantir cobertura das recompensas, o teste força o checkpoint de vitória entre etapas quando necessário; não representa 45 vitórias humanas ou comprovação de balanceamento. Cobertura adicional: nove cartas de jornada alcançáveis, cinco relíquias, efeitos nos lados corretos, novos estados, chefe solo com 120 de vida, fase 2 sem escudo duplicado, aprimoramento individual, salvamento exato e rejeição de dados alterados/truncados.

O build Windows compilou com sucesso. Na interface foram conferidos menu, campo de batalha, velocidade 2×, seleção de alvo, proteção persistente nos três aliados e fechamento/reabertura preservando energia, mão e escudos. A revisão isolada confirmou cartas de recompensa legíveis, escolha levando diretamente ao chefe, carta nova na mão inicial, arte do chefe junto aos três aliados, barra com 120 de vida e atordoamento visual que impede a ação sem dano ao grupo. Foi corrigido e retestado o conflito da barra de espaço com o último alvo clicado: o atalho agora encerra o turno. Os testes são desktop; não afirmar validação Android.

`RunePact > Build isolated visual review` produz um executável separado em `Builds/Review`. Argumentos `-runepact-review-rewards`, `-runepact-review-boss` e `-runepact-review-states` montam cenários controlados sem escrever na jornada salva. Esses atalhos não são compilados no executável normal. A revisão usa uma definição própria, sem habilitar serviços de conexão de um Development Build.

## Rodada de evolução

A rodada de sistemas passou em 2.515 verificações e 80 batalhas simuladas. Ela cobre formações, relíquias, desconto do Grimório Vivo, fase 2 do Regente, recompensas e restauração de ponto salvo. A interface ainda precisa de testes manuais em diferentes aparelhos Android antes de publicação.

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
- Acessibilidade, multiplayer, persistência em aparelhos reais, monetização e requisitos de lojas.
- Balanceamento por playtests humanos e retenção de longo prazo.

---

# Prototype QA (EN)

## Current milestone: visual V6

Rules: **4,512 assertions passed**, including 80 base battles and 45 varied journeys. All 17 art cells (nine poses and eight items) passed visible-content, transparency, and cell-margin checks. Windows build succeeded.

Isolated executable inspection: card poses, icons, blue/violet rarity, HUD and status panel, immediate select-and-click attack (155 → 127 HP), defense after selecting and dragging (+28 shield), return to base pose, card/rune/grimoire reward artwork, and relic selection advancing to the 120-HP boss. Dragging without prior selection was unreliable in automation and needs human mouse/touch verification. Selecting a card and clicking its target was verified.

The default review scenario shows six varied cards with extra energy for QA only; it does not change normal runs or the player's save. Animations use sprite poses and transforms, not complete frame-by-frame sequences. Android and touch interaction remain untested.

## Previous milestone: V5

Rules passed **4,512 assertions**, including 80 base battles and 45 varied journeys with exact saves. New tests cover the three difficulty bands, one unique card per warrior, unique effects, the standard two-encounter route and 220-health boss, V4 save migration, and mode/timer persistence. The Windows build compiled. Visual inspection confirmed the menu, combat, enemy order, card selection, dragging onto a target, preview/cancel/confirm without early resource spending, and attack resolution. The gesture still needs validation on real touch hardware.

A simple baseline bot ran 24 journeys per mode/difficulty combination: quick mode won 24/24 at every level; standard won 24/24 on calm and normal, 23/24 on fierce. Mean cards played: quick 35/41/47 and standard 69/82/91 (calm/normal/fierce). This demonstrates increased duration and pressure but **does not prove human-facing fun or balance**. The generated `BalanceTestResults.txt` report is Git-ignored.

Android was not built or validated in this milestone. Layout is landscape-first; portrait orientation shows a rotate-device prompt. Tests across window sizes, touch hardware, real safe areas, and mobile performance remain pending.

## Earlier milestone: quick test V4

Rules passed **5,153 assertions**: 80 base battles and 45 varied journeys. To guarantee reward coverage, the test forces a victory checkpoint between stages when needed; this does not represent 45 human wins or proof of balance. Additional coverage: nine reachable journey cards, five relics, correct effect sides, new states, solo 120-health boss, phase two without duplicate shields, individual upgrades, exact saving, and rejection of modified/truncated data.

The Windows build compiled successfully. UI checks covered menu, battlefield, 2× speed, target selection, persistent protection on all three allies, and closing/reopening with energy, hand, and shields preserved. Isolated review confirmed readable reward cards, selection leading directly to the boss, the new card in the opening hand, boss art alongside all three allies, the 120-health bar, and visible stun skipping the boss action without party damage. The spacebar conflict with the last clicked target was fixed and retested: the shortcut now ends the turn. Tests are desktop-only; Android validation must not be claimed.

`RunePact > Build isolated visual review` produces a separate executable under `Builds/Review`. Arguments `-runepact-review-rewards`, `-runepact-review-boss`, and `-runepact-review-states` create controlled scenarios without writing the saved journey. These shortcuts are not compiled into the regular executable. Review uses a dedicated define without enabling Development Build connection services.

## Evolution milestone

The systems milestone passed 2,515 assertions and 80 simulated battles. It covers formations, relics, Living Grimoire discount, Regent phase 2, rewards, and checkpoint restoration. The interface still needs manual tests on different Android devices before publishing.

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
- Accessibility, multiplayer, real-device persistence, monetization, and storefront requirements.
- Human-playtest balance and long-term retention.
