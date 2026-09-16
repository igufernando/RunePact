# Teste rápido e clareza de combate — português

## Escopo atual

Esta versão substitui o percurso de três encontros documentado nas versões anteriores: **um encontro normal → uma recompensa → chefe → resultado**. O Regente de Âmbar luta sozinho, com 120 de vida. Ao planejar um turno com metade da vida ou menos, entra na fase 2 e recebe 12 de escudo uma única vez. Os valores são deliberadamente leves para teste, não o balanceamento final.

## Cartas e recompensas

A recompensa aparece como carta ilustrada com custo, habilidade, proprietário e descrição. Há três opções: adicionar uma carta, aprimorar uma habilidade específica em +6 de poder ou receber uma relíquia. A melhoria não aumenta todas as cartas do personagem. A carta adicionada já aparece na mão inicial contra o chefe.

O catálogo contém as três cartas anteriores e seis novas: Manto de Espinhos, Fratura Âmbar, Golpe Sísmico, Fonte Serena, Selo do Ocaso e Brasa Ancestral. As nove cartas e cinco relíquias podem aparecer nas ofertas de diferentes jornadas. Rara/épica são classificações fixas de apresentação, sem gacha, compras ou sorteios pagos.

- Vulnerável: os próximos dois ataques recebidos causam +25% de dano.
- Regeneração: cura imediata da carta e +10 nos dois próximos inícios de turno.
- Espinhos: devolve 7 de dano verdadeiro por ataque; dura dois ciclos de turno.
- Atordoamento: impede a próxima ação inimiga; não acumula.
- Fraqueza: -25% de poder na próxima ação; Bastião a remove.

## Apresentação e controle

Menu simples de continuar/nova jornada, velocidade 1×/2×, painel de relíquias, barra de vida e fase do chefe e inspeção de estados ao tocar em um personagem sem carta selecionada. O menu de pausa oferece velocidade e som.

Personagens têm respiração sutil ancorada nos pés, sombras de contato e sprites recortados em memória para eliminar margens vazias. Escudos ganham uma barreira contínua; queimadura, marcas e estados mágicos têm partículas e símbolos persistentes. As referências de arte aprovadas não foram substituídas.

O layout mantém a referência horizontal de 1600×900 e se ajusta à área segura real. Janelas verticais recebem enquadramento proporcional, não uma HUD vertical redesenhada; a orientação do produto mobile continua horizontal. Validação física Android permanece pendente.

## Salvamento

Formato versionado 4, com validação de limites e checksum. Inclui mão, baralho, descarte, ordem futura de embaralhamento, vida de ambos os lados, estados, energia, ações, Pacto, relíquias, habilidades aprimoradas e intenções. Vitórias pendentes de recompensa e resultados finais são preservados.

Salva após ações do jogador, forja, troca de equipamento, recompensa e término do turno inimigo. Se o aplicativo fechar no meio da sequência inimiga, retoma o último ponto seguro anterior à sequência; não tenta serializar uma animação em andamento. O salvamento legado permanece na chave antiga, mas não é carregado como se fosse um estado exato compatível.

## Correções de regras

Brasa Antiga aumenta somente a queimadura em inimigos; Casca de Carvalho só melhora escudos aliados; Flecha Cerimonial não aumenta ataques inimigos. Grimório Vivo desconta a primeira carta de Lyra mesmo que outro aliado tenha agido antes. Sentinelas priorizam a proteção de um aliado ferido.

---

# Quick test and combat clarity — English

## Current scope

This version replaces the three-encounter route documented in earlier milestones: **one normal encounter → one reward → boss → result**. The Amber Regent fights alone with 120 health. When planning a turn at half health or below, it enters phase two and gains 12 shield once. Values are intentionally light for testing, not final balance.

## Cards and rewards

Rewards appear as illustrated cards with cost, ability, owner, and description. Three choices are offered: add a card, upgrade one specific ability by +6 power, or receive a relic. An upgrade does not improve every card belonging to that hero. Added cards are immediately available in the opening boss hand.

The catalog includes the previous three cards and six new ones: Thorn Mantle, Amber Fracture, Seismic Strike, Serene Spring, Dusk Seal, and Ancestral Ember. All nine cards and five relics are reachable across different journeys. Rare/epic are fixed presentation labels, without gacha, purchases, or paid draws.

- Vulnerable: the next two incoming attacks deal +25% damage.
- Regeneration: immediate card healing plus 10 at the next two turn starts.
- Thorns: reflects 7 true damage per attack for two turn cycles.
- Stun: skips the next enemy action; does not stack.
- Weakness: -25% power on the next action; Bastion removes it.

## Presentation and controls

Simple continue/new-journey menu, 1×/2× speed, relic panel, boss health/phase bar, and status inspection by tapping a character without a selected card. Pause offers speed and sound settings.

Characters have subtle foot-anchored breathing, contact shadows, and in-memory sprite trimming to remove empty margins. Shields have a persistent barrier; burn, marks, and magical states use persistent particles and symbols. Approved art references were not replaced.

The layout keeps a horizontal 1600×900 reference and adapts to the actual safe area. Vertical windows receive proportional framing, not a redesigned portrait HUD; mobile product orientation remains landscape. Physical Android validation remains pending.

## Saving

Version 4 format with bounds validation and checksum. Includes hand, deck, discard, future shuffle order, health on both sides, states, energy, actions, Pact, relics, upgraded abilities, and intentions. Pending reward victories and terminal results are preserved.

Saves after player actions, forging, equipment swaps, rewards, and enemy-turn completion. Closing during an enemy sequence resumes the previous safe point, rather than attempting to serialize an in-flight animation. Legacy data remains under its old key, but is not loaded as though it were a compatible exact snapshot.

## Rule fixes

Ancient Ember only buffs burn on enemies; Oak Bark only improves allied shields; Ceremonial Arrow does not empower enemy attacks. Living Grimoire discounts Lyra's first card even if another ally acted first. Sentinels prioritize shielding an injured ally.
